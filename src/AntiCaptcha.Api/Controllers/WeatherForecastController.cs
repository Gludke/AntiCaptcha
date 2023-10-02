using Microsoft.AspNetCore.Mvc;
//using Anticaptcha_example.Api;
//using Anticaptcha_example.Helper;
using PlaywrightExtraSharp.Models;
using PlaywrightExtraSharp.Plugins.ExtraStealth;
using PlaywrightExtraSharp;
using Microsoft.Playwright;

namespace AntiCaptcha.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public static IPage? Page { get; set; }

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
        public async Task<bool> Get()
        {
            // Initialization plugin builder
            var playwright = new PlaywrightExtra(BrowserTypeEnum.Chromium);

            // Install browser
            playwright.Install();

            // Use stealth plugin
            playwright.Use(new StealthExtraPlugin());

            // Launch the puppeteer browser with plugins
            await using var browser = await playwright.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Channel = "chrome",
                    Headless = false,
                    SlowMo = 40,
                    Timeout = 0,
                    DownloadsPath = "",
                    Args = new List<string> { "--start-maximized" }
                });

            // Create a new page
            Page = await playwright.NewPageAsync(
                new BrowserNewPageOptions
                {
                    ViewportSize = ViewportSize.NoViewport,
                    AcceptDownloads = true,
                    Locale = "pt-BR",
                    //UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246"
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.5938.92 Safari/537.36 Edge/100.0.10586.0"
                }
            );

            await Page.GotoAsync("https://cvmweb.cvm.gov.br/SWB/default.asp?sg_sistema=scw");

            await Page.FrameLocator("frame[name=\"Main\"]").FrameLocator("frame[name=\"SubMain\"]").GetByRole(AriaRole.Link, new() { Name = "Acesso Gov BR" }).ClickAsync();
            await Task.Delay(1000);

            await Page.GetByPlaceholder("Digite seu CPF").ClickAsync();
            await Page.Keyboard.TypeAsync("157.010.968-07", new() { Delay = 100 }); // Adiciona um atraso entre os caracteres para simular a digitação

            await Page.GetByRole(AriaRole.Button, new() { Name = "Continuar" }).ClickAsync();

            await Page.GetByPlaceholder("Digite sua senha atual").ClickAsync();
            await Page.Keyboard.TypeAsync("Th0m45@K03n", new() { Delay = 100 });

            await Task.Delay(1000);
            await Page.GetByLabel("Botão Entrar. Aperte a tecla enter para entrar.").ClickAsync();

            await Page.PauseAsync();

            return true;
        }

        //private static string QuebrarCaptcha(string urlAtual)
        //{
        //    DebugHelper.VerboseMode = true;

        //    var api = new HCaptchaProxyless
        //    {
        //        ClientKey = "7e63e104e9bdedfe4e53c0438d3e3e66",
        //        WebsiteUrl = new Uri($"{urlAtual}"),
        //        WebsiteKey = "93b08d40-d46c-400a-ba07-6f91cda815b9",
        //        SoftId = 0
        //    };

        //    // use to set invisible mode
        //    //api.IsInvisible = true

        //    // use to set Hcaptcha Enterprise parameters like rqdata, sentry, apiEndpoint, endpoint, reportapi, assethost, imghost
        //    //api.EnterprisePayload.Add("rqdata", "rqdata value from target website");
        //    //api.EnterprisePayload.Add("sentry", "true");

        //    string result = "";
        //    if (!api.CreateTask())
        //        DebugHelper.Out("API v2 send failed. " + api.ErrorMessage, DebugHelper.Type.Error);
        //    else if (!api.WaitForResult())
        //        DebugHelper.Out("Could not solve the captcha.", DebugHelper.Type.Error);
        //    else
        //    {
        //        result = api.GetTaskSolution().GRecaptchaResponse;
        //        DebugHelper.Out("Result: " + result, DebugHelper.Type.Success);
        //    }

        //    return result;
        //}
    }
}