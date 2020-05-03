using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServiceHub.Controllers
{
    [Route("")]
    [ApiController]
    public class DefaultController : ControllerBase
    {

        // GET: Empty/Default controller
        [HttpGet]
        public async void Get()
        {
            //Response.Headers.Add();
            await Response.WriteAsync("<!DOCTYPE html>" +
                "<html lang=\"en\">" +
                "<head>" +
                "<meta charset=\"UTF - 8\">" +
                "<meta name=\"viewport\" content=\"width = device - width, initial - scale = 1.0\">" +
                "<title>Service Hub</title>" +
                "<style>body{font-family: Segoe UI,SegoeUI,Segoe WP,Helvetica Neue,Helvetica,Tahoma,Arial,sans-serif;font-weight: 400;}</style></head>" +
                "<body>" +
                "<center><h1>Service Hub</h1><hr><p>Nothing to see here</p></center>" +
                "</body>" +
                "</html>");
        }
    }
}