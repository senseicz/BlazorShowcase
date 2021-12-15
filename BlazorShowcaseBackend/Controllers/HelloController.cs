using Microsoft.AspNetCore.Mvc;

namespace BlazorShowcase.Controllers
{
    [Route("api/[controller]")]
    public class HelloController
    {
        [HttpGet]
        public string Get()
        {
            return "Hello world";
        }
    }
}
