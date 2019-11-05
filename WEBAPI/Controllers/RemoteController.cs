using System;
using System.Threading.Tasks;
using InfrastructureCache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemoteController : ControllerBase
    {
        private readonly RemoteServiceProvider<ValueDto> _remoteServiceProvider;

        public RemoteController(RemoteServiceProvider<ValueDto> remoteServiceProvider)
        {
            _remoteServiceProvider = remoteServiceProvider;
        }

        [HttpGet]
        public async Task<ActionResult<ValueDto>> Get()
        {
          var (value, expire) = await _remoteServiceProvider.GetWithRetry("value");
          Response.GetTypedHeaders().Expires = expire;
          Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
          {
              Public = true,
          };
          return value;
        }
    }
}