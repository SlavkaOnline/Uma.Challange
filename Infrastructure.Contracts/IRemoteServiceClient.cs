using System;
using System.Threading.Tasks;

namespace Infrastructure.Contracts
{
    public interface IRemoteServiceClient<T>
    {
        Task<T> GetAsync(string path);
        Task<bool> IsResourceModified(string path, DateTime dt);
    }
}