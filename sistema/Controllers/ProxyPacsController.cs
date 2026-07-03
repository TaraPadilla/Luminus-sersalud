using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace farmamest.Controllers
{
    [Route("api/pacs")]
    [ApiController]
    public class ProxyPacsController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public ProxyPacsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        [Route("orders")]
        public async Task<IActionResult> PostToMiddleware([FromBody] object data)
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token edc35a54397b0f9ac9da725d33a430c48d95df0b");

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://middleware-staging.dev-land.space/api/v1/orders/", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            else
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
    }
}