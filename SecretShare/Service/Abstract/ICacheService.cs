namespace SecretShare.Service.Abstract
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> createItem);
        void Remove(string key);
    }
}
