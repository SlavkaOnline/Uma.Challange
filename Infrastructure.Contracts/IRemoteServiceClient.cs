using System.Threading.Tasks;

namespace Infrastructure.Contracts
{
    public interface IRemoteServiceClient<T>
    {
        Task<T> Get(string path);
    }
}