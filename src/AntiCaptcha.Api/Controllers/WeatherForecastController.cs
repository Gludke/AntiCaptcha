using Microsoft.AspNetCore.Mvc;
using Anticaptcha_example.Api;
using Anticaptcha_example.Helper;
using Microsoft.Playwright;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Channel = "chrome",
                    Headless = false,
                    SlowMo = 40,
                    Timeout = 0,
                    DownloadsPath = "",
                    Args = new List<string> { "--start-maximized" }
                });

            Page = await browser.NewPageAsync(
                new BrowserNewPageOptions
                {
                    ViewportSize = ViewportSize.NoViewport,
                    AcceptDownloads = true,
                    Locale = "pt-BR",
                    //UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246"
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4500.0 Safari/537.36 Edge/100.0.10586.0"
                });

            await Page.GotoAsync("https://cvmweb.cvm.gov.br/SWB/default.asp?sg_sistema=scw");

            await Page.FrameLocator("frame[name=\"Main\"]").FrameLocator("frame[name=\"SubMain\"]").GetByRole(AriaRole.Link, new() { Name = "Acesso Gov BR" }).ClickAsync();
            await Task.Delay(1000);

            //Interação extra, para acumular cache
            await Page.GetByRole(AriaRole.Link, new() { Name = "Logomarca GovBR" }).ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = " Entrar com o gov.br" }).ClickAsync();

            // Simula um movimento do mouse
            await Page.Mouse.MoveAsync(100, 200);
            await Task.Delay(1000);

            await Page.GetByPlaceholder("Digite seu CPF").ClickAsync();
            await Page.Keyboard.TypeAsync("157.010.968-07", new() { Delay = 100 }); // Adiciona um atraso entre os caracteres para simular a digitação

            await Page.GetByRole(AriaRole.Button, new() { Name = "Continuar" }).ClickAsync();

            // Simula um movimento do mouse
            await Page.Mouse.MoveAsync(400, 300);
            await Task.Delay(2000);

            await Page.GetByPlaceholder("Digite sua senha atual").ClickAsync();
            await Page.Keyboard.TypeAsync("Th0m45@K03n", new() { Delay = 100 });

            await Task.Delay(1000);

            await Page.GetByLabel("Botão Entrar. Aperte a tecla enter para entrar.").ClickAsync();

            try
            {
                QuebrarCaptcha(Page.Url);
            }
            catch (Exception ex)
            {
                throw;
            }

            await Page.PauseAsync();

            return true;
        }

        private static void QuebrarCaptcha(string urlAtual)
        {
            DebugHelper.VerboseMode = true;

            var api = new HCaptchaProxyless
            {
                ClientKey = "7e63e104e9bdedfe4e53c0438d3e3e66",
                WebsiteUrl = new Uri($"{urlAtual}"),
                WebsiteKey = "93b08d40-d46c-400a-ba07-6f91cda815b9",
                SoftId = 0
            };

            // use to set invisible mode
            //api.IsInvisible = true

            // use to set Hcaptcha Enterprise parameters like rqdata, sentry, apiEndpoint, endpoint, reportapi, assethost, imghost
            //api.EnterprisePayload.Add("rqdata", "rqdata value from target website");
            //api.EnterprisePayload.Add("sentry", "true");

            if (!api.CreateTask())
                DebugHelper.Out("API v2 send failed. " + api.ErrorMessage, DebugHelper.Type.Error);
            else if (!api.WaitForResult())
                DebugHelper.Out("Could not solve the captcha.", DebugHelper.Type.Error);
            else
                DebugHelper.Out("Result: " + api.GetTaskSolution().GRecaptchaResponse, DebugHelper.Type.Success);
        }
    }
}