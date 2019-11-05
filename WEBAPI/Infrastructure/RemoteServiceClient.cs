using System;
using System.Net;
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

        public async Task<T> GetAsync(string path)
        {
            var response = await _httpClient.GetAsync(path);
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }
        
        public async Task<bool> IsResourceModified(string path, DateTime dt)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_httpClient.BaseAddress + "/" + path)
            };
            request.Headers.IfModifiedSince = dt;
            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
          
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotModified:
                    return false;
                case HttpStatusCode.OK:
                    return true;
            }
            
            return true;
        }
    }
}