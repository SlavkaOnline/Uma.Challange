using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Contracts;
using Newtonsoft.Json;

namespace WEBAPI.Infrastructure
{
    public class RemoteServiceClient<T> : IRemoteServiceClient<T>
    {
             private readonly HttpClient _httpClient;

             public RemoteServiceClient(HttpClient httpClient)
             {
                 _httpClient = httpClient;
             }

             public async Task<T> Get(string path)
             {
//                 var response = await _httpClient.GetAsync(path);
//                 var content = await response.Content.ReadAsStringAsync();
//                 
//                 return JsonConvert.DeserializeObject<T>(content);
                 await Task.Delay(2000);
                 return JsonConvert.DeserializeObject<T>(@"{'value': 'Hello World'}");
             }
             
    }
}