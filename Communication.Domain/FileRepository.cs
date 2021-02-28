using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Communication.Domain
{
    public class FileRepository<TId,T> : IRepository<TId, T> where T : Entity<TId>
    {
        public FileRepository()
        {
            
        }

        protected const string BaseFilePath = @"C:\temp\";
        protected string FileName = "temp.json";
        public IQueryable<T> GetAll()
        {
            return JsonSerializer.Deserialize<IQueryable<T>>(GetJsonAsync().Result);
        }

        public async Task<T> FindAsync(TId id)
        {
            var data = await GetDataAsync<IEnumerable<T>>();
            return data.FirstOrDefault(x => x.Id.Equals(id));
        }

        public async Task<T> CreateAsync(T item)
        {
            var data = await GetDataAsync<List<T>>();
            var entity = data.FirstOrDefault(x => x.Id.Equals(item.Id));
            if (entity != null) return item;
            data.Add(item);
            await SaveDataAsync(JsonSerializer.Serialize(data));
            return item;
        }

        public async Task<T> UpdateAsync(TId id, T item)
        {
            var data = await GetDataAsync<List<T>>();
            var entity = data.FirstOrDefault(x => x.Id.Equals(id));
            if (entity == null) return null;
            data.Remove(entity);
            data.Add(item);
            return item;
        }

        public async Task<bool> DeleteAsync(TId id)
        {
            var data = await GetDataAsync<List<T>>();
            var entity = data.FirstOrDefault(x=>x.Id.Equals(id));
            if (entity == null) return false;
            data.Remove(entity);
            await SaveDataAsync(JsonSerializer.Serialize(data));
            return true;
        }

        protected async Task<string> GetJsonAsync()
        {
            var filePath = BaseFilePath + FileName;
            if (!File.Exists(filePath)) File.Create(filePath);

            using var streamReader= new StreamReader(filePath);
            var json = await streamReader.ReadToEndAsync();
            return json;
        }

        public async Task<bool> SaveDataAsync(string data)
        {
            var filePath = BaseFilePath + FileName;
            if (!File.Exists(filePath)) return false;

            await using var sw = new StreamWriter(filePath);
            await sw.WriteAsync(data);
            return true;
        }
        
        protected async Task<TOutput> GetDataAsync<TOutput>()
        {
            var json = await GetJsonAsync();
            return JsonSerializer.Deserialize<TOutput>(json);
        }
    }
}