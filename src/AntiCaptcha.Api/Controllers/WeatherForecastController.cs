using Microsoft.AspNetCore.Mvc;
using System;
//using Anticaptcha_example.Api;
//using Anticaptcha_example.Helper;

namespace AntiCaptcha.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            //DebugHelper.VerboseMode = true;

            //var api = new HCaptchaProxyless
            //{
            //    ClientKey = "YOUR_API_KEY_HERE",
            //    WebsiteUrl = new Uri("http://makeawebsitehub.com/recaptcha/test.php"),
            //    WebsiteKey = "51829642-0000-0000-896c-594f89d700cc",
            //    // Specify softId to earn 10% commission with your app.
            //    // Get your softId here:
            //    // https://anti-captcha.com/clients/tools/devcenter
            //    SoftId = 0
            //};

            //// use to set invisible mode
            ////api.IsInvisible = true

            //// use to set Hcaptcha Enterprise parameters like rqdata, sentry, apiEndpoint, endpoint, reportapi, assethost, imghost
            ////api.EnterprisePayload.Add("rqdata", "rqdata value from target website");
            ////api.EnterprisePayload.Add("sentry", "true");

            //if (!api.CreateTask())
            //    DebugHelper.Out("API v2 send failed. " + api.ErrorMessage, DebugHelper.Type.Error);
            //else if (!api.WaitForResult())
            //    DebugHelper.Out("Could not solve the captcha.", DebugHelper.Type.Error);
            //else
            //    DebugHelper.Out("Result: " + api.GetTaskSolution().GRecaptchaResponse, DebugHelper.Type.Success);





            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}