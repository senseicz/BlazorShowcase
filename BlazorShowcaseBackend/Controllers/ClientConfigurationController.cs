using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorShowcaseBackend.Controllers
{
    [Route("client-configuration")]
    public class ClientConfigurationController
    {
        [Route("showcase-client")]
        [HttpGet]
        public JsonResult Get()
        {
            var clientConfig = new ClientConfiguration()
            {
                Authority = "https://localhost:7087",
                ClientId = "ShowcaseClient",
                RedirectUri = "https://localhost:7777/authentication/login-callback",
                PostLogoutRedirectUri = "https://localhost:7777/authentication/logged-out",
                ResponseType = "code",
                Scope = "ShowcaseBackend.Api openid profile"
            };

            return new JsonResult(clientConfig);
        }


        private class ClientConfiguration
        {
            [JsonPropertyName("authority")]
            public string Authority { get; set; }
            [JsonPropertyName("client_id")]
            public string ClientId { get; set; }
            [JsonPropertyName("redirect_uri")]
            public string RedirectUri { get; set; }
            [JsonPropertyName("post_logout_redirect_uri")]
            public string PostLogoutRedirectUri { get; set; }
            [JsonPropertyName("response_code")]
            public string  ResponseType { get; set; }
            [JsonPropertyName("scope")]
            public string Scope { get; set; }
        }
    }
}
