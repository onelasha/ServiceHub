using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

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
            /*
            var client = new RestClient("https://bluekc-prod.apigee.net/oauth/relationsso");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddParameter("SAMLRequest", "PHNhbWxwOlJlc3BvbnNlIElEPSJfYmJjYzFjMjAtMTBiNy00MjE1LTliMGItNGYwOGU1ZGRjYWRiIiBWZXJzaW9uPSIyLjAiIElzc3VlSW5zdGFudD0iMjAyMC0wNy0yMVQxODo0MTo0MVoiIERlc3RpbmF0aW9uPSJodHRwczovL21lbWJlcnMuYmx1ZWtjLmNvbS8iIHhtbG5zOnNhbWxwPSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6cHJvdG9jb2wiPjxzYW1sOklzc3VlciB4bWxuczpzYW1sPSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6YXNzZXJ0aW9uIj5ibHVla2M8L3NhbWw6SXNzdWVyPjxzYW1scDpTdGF0dXM+PHNhbWxwOlN0YXR1c0NvZGUgVmFsdWU9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDpzdGF0dXM6U3VjY2VzcyIgLz48L3NhbWxwOlN0YXR1cz48c2FtbDpBc3NlcnRpb24gSUQ9Il9lNTllYjNhMi04NjI0LTQyMzUtOTdkZS0yM2E4YzEwOGQ2NWUiIFZlcnNpb249IjIuMCIgSXNzdWVJbnN0YW50PSIyMDIwLTA3LTIxVDE4OjQxOjQxWiIgeG1sbnM6c2FtbD0idXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOmFzc2VydGlvbiI+PHNhbWw6SXNzdWVyPmJsdWVrYzwvc2FtbDpJc3N1ZXI+PHNhbWw6U3ViamVjdD48c2FtbDpOYW1lSUQ+Ymx1ZWtjLmNvbTwvc2FtbDpOYW1lSUQ+PHNhbWw6U3ViamVjdENvbmZpcm1hdGlvbiBNZXRob2Q9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDpjbTpiZWFyZXIiPjxzYW1sOlN1YmplY3RDb25maXJtYXRpb25EYXRhIFJlY2lwaWVudD0iaHR0cHM6Ly9tZW1iZXJzLmJsdWVrYy5jb20vIiAvPjwvc2FtbDpTdWJqZWN0Q29uZmlybWF0aW9uPjwvc2FtbDpTdWJqZWN0PjxzYW1sOkNvbmRpdGlvbnM+PHNhbWw6QXVkaWVuY2VSZXN0cmljdGlvbj48c2FtbDpBdWRpZW5jZT5odHRwczovL21lbWJlcnMuYmx1ZWtjLmNvbS88L3NhbWw6QXVkaWVuY2U+PC9zYW1sOkF1ZGllbmNlUmVzdHJpY3Rpb24+PC9zYW1sOkNvbmRpdGlvbnM+PHNhbWw6QXV0aG5TdGF0ZW1lbnQgQXV0aG5JbnN0YW50PSIyMDIwLTA3LTIxVDE4OjQxOjQxWiIgU2Vzc2lvbk5vdE9uT3JBZnRlcj0iMjAyMC0wNy0yMVQxOTo0MTo0MVoiPjxzYW1sOkF1dGhuQ29udGV4dD48c2FtbDpBdXRobkNvbnRleHRDbGFzc1JlZj51cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6YWM6Y2xhc3NlczpQYXNzd29yZDwvc2FtbDpBdXRobkNvbnRleHRDbGFzc1JlZj48L3NhbWw6QXV0aG5Db250ZXh0Pjwvc2FtbDpBdXRoblN0YXRlbWVudD48c2FtbDpBdHRyaWJ1dGVTdGF0ZW1lbnQ+PHNhbWw6QXR0cmlidXRlIE5hbWU9Imdyb3VwSWQiIE5hbWVGb3JtYXQ9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDphdHRybmFtZS1mb3JtYXQ6YmFzaWMiPjxzYW1sOkF0dHJpYnV0ZVZhbHVlIHR5cGU9InN0cmluZyI+NDMwODgwMDA8L3NhbWw6QXR0cmlidXRlVmFsdWU+PC9zYW1sOkF0dHJpYnV0ZT48c2FtbDpBdHRyaWJ1dGUgTmFtZT0ic3NuIiBOYW1lRm9ybWF0PSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6YXR0cm5hbWUtZm9ybWF0OmJhc2ljIj48c2FtbDpBdHRyaWJ1dGVWYWx1ZSB0eXBlPSJzdHJpbmciPjYyMzA4MTg2Njwvc2FtbDpBdHRyaWJ1dGVWYWx1ZT48L3NhbWw6QXR0cmlidXRlPjxzYW1sOkF0dHJpYnV0ZSBOYW1lPSJiaXJ0aERhdGUiIE5hbWVGb3JtYXQ9InVybjpvYXNpczpuYW1lczp0YzpTQU1MOjIuMDphdHRybmFtZS1mb3JtYXQ6YmFzaWMiPjxzYW1sOkF0dHJpYnV0ZVZhbHVlIHR5cGU9InN0cmluZyI+MTAvMDIvMTk4Nzwvc2FtbDpBdHRyaWJ1dGVWYWx1ZT48L3NhbWw6QXR0cmlidXRlPjxzYW1sOkF0dHJpYnV0ZSBOYW1lPSJlbWFpbCIgTmFtZUZvcm1hdD0idXJuOm9hc2lzOm5hbWVzOnRjOlNBTUw6Mi4wOmF0dHJuYW1lLWZvcm1hdDpiYXNpYyI+PHNhbWw6QXR0cmlidXRlVmFsdWUgdHlwZT0ic3RyaW5nIj5vbmUubGFzaGFAZ21haWwuY29tPC9zYW1sOkF0dHJpYnV0ZVZhbHVlPjwvc2FtbDpBdHRyaWJ1dGU+PHNhbWw6QXR0cmlidXRlIE5hbWU9ImNsaWVudElkIiBOYW1lRm9ybWF0PSJ1cm46b2FzaXM6bmFtZXM6dGM6U0FNTDoyLjA6YXR0cm5hbWUtZm9ybWF0OmJhc2ljIj48c2FtbDpBdHRyaWJ1dGVWYWx1ZSB0eXBlPSJzdHJpbmciPllFT3M2RzhIV0FxWWlzSVlNck80aFFBd0pYTEZYOG1kPC9zYW1sOkF0dHJpYnV0ZVZhbHVlPjwvc2FtbDpBdHRyaWJ1dGU+PC9zYW1sOkF0dHJpYnV0ZVN0YXRlbWVudD48L3NhbWw6QXNzZXJ0aW9uPjxTaWduYXR1cmUgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvMDkveG1sZHNpZyMiPjxTaWduZWRJbmZvPjxDYW5vbmljYWxpemF0aW9uTWV0aG9kIEFsZ29yaXRobT0iaHR0cDovL3d3dy53My5vcmcvMjAwMS8xMC94bWwtZXhjLWMxNG4jIiAvPjxTaWduYXR1cmVNZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwLzA5L3htbGRzaWcjcnNhLXNoYTEiIC8+PFJlZmVyZW5jZSBVUkk9IiI+PFRyYW5zZm9ybXM+PFRyYW5zZm9ybSBBbGdvcml0aG09Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvMDkveG1sZHNpZyNlbnZlbG9wZWQtc2lnbmF0dXJlIiAvPjxUcmFuc2Zvcm0gQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzEwL3htbC1leGMtYzE0biMiIC8+PC9UcmFuc2Zvcm1zPjxEaWdlc3RNZXRob2QgQWxnb3JpdGhtPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwLzA5L3htbGRzaWcjc2hhMSIgLz48RGlnZXN0VmFsdWU+QjNLNU9vajFPdW9MSTczaWhLaXUrTHFvMVg0PTwvRGlnZXN0VmFsdWU+PC9SZWZlcmVuY2U+PC9TaWduZWRJbmZvPjxTaWduYXR1cmVWYWx1ZT5QSUxqSnBuMDk0a0NnMHl5U3Z0dUVza1RsOHZaZk5vckdNcXdXRzc0SlFXNjZkK2tIb1hrTVQwdDJRazNON0ZJQ1g2N3YzYTdIZC9ncTVnQUNLMW9kcmF6THBidzYxN29OSnVPVUt6aHFwK0toV1prOFZqRkFoVUEyV1k4amJkc3BtMDNUZkZRUG5RWFNCcy96UjIwUmp5d3krcElTOG0ydUFPaWdkbnhYVm89PC9TaWduYXR1cmVWYWx1ZT48S2V5SW5mbz48WDUwOURhdGE+PFg1MDlDZXJ0aWZpY2F0ZT5NSUlDY0RDQ0FkbWdBd0lCQWdJQkFEQU5CZ2txaGtpRzl3MEJBUTBGQURCVk1Rc3dDUVlEVlFRR0V3SjFjekVUTUJFR0ExVUVDQXdLUTJGc2FXWnZjbTVwWVRFUk1BOEdBMVVFQ2d3SVVtVnNZWFJwYjI0eEhqQWNCZ05WQkFNTUZYSmxiR0YwYVc5dWFXNXpkWEpoYm1ObExtTnZiVEFlRncweU1EQTFNakV5TVRNeU1qaGFGdzB5TVRBMU1qRXlNVE15TWpoYU1GVXhDekFKQmdOVkJBWVRBblZ6TVJNd0VRWURWUVFJREFwRFlXeHBabTl5Ym1saE1SRXdEd1lEVlFRS0RBaFNaV3hoZEdsdmJqRWVNQndHQTFVRUF3d1ZjbVZzWVhScGIyNXBibk4xY21GdVkyVXVZMjl0TUlHZk1BMEdDU3FHU0liM0RRRUJBUVVBQTRHTkFEQ0JpUUtCZ1FETTdNbWc5d2ZCOFFucFM3bVdCRTU1UUJxMHFDb25jcmRhdjJkbUl0REFTYWs0MW4xaFE4ckk4QzV0OU1PVHpBcDZ3cUtKelFTNW1NWUVSTW82SkYyUHo0ek5BUFlwcThmSDY2d0ZDWmhDaThIbmEwZElCME1YQnd5eWNJdGd5QkY0OFdsdlJHRVJQZmtrNytmTmF1QnF6OHkrMjdtTC9VdlR1dDNmekNvYzl3SURBUUFCbzFBd1RqQWRCZ05WSFE0RUZnUVVsdjRDZEliSERoZGk4cE4wSGZQMmJlUE5BTVF3SHdZRFZSMGpCQmd3Rm9BVWx2NENkSWJIRGhkaThwTjBIZlAyYmVQTkFNUXdEQVlEVlIwVEJBVXdBd0VCL3pBTkJna3Foa2lHOXcwQkFRMEZBQU9CZ1FER2hEaFBjMDF6ZzdjQm9OWm93QTZNQlZ4T1A2WjAyTXprSFBaQTdUVjhMalZXeVJzbXhRUnREV1YwRjlpdDV6WVVaMGFiNVN4SnlSa2xPSEdaMXlING9FZXpzTTJhWEkxNUtzMUo4UEJXS3NFa2RpWVByM1F6RW14K00xaGZ0aVNNKzgzZ2N5dEpDQ0VOeG5sMEZWbXNHbGg5bWR6ZDV6VC9oaGFDb2w4N1hRPT08L1g1MDlDZXJ0aWZpY2F0ZT48L1g1MDlEYXRhPjwvS2V5SW5mbz48L1NpZ25hdHVyZT48L3NhbWxwOlJlc3BvbnNlPg==");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            Response.StatusCode = 302;
            Response.Headers.Add("Location", response.ResponseUri.ToString());
            //Response.Redirect()
            return;
            */

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