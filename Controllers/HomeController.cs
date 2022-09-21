using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using _mpesaIntegration.Data;
using _mpesaIntegration.Models;
using Microsoft.AspNetCore.Mvc;
using mpesaIntegration.Models;
using Newtonsoft.Json;

namespace mpesaIntegration.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private ApplicationDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


     public async Task<string> GetToken() {

        var client = _clientFactory.CreateClient("mpesa");

        var authString = "L1WFwLyO5sYekeaW6v7ZgJPlifqk818j:teLMgeCkju44TNpW";

        var encodedString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authString));

        var _url = "/oauth/v1/generate?grant_type=client_credentials";

        var request = new HttpRequestMessage(HttpMethod.Get, _url);

        request.Headers.Add("Authorization", $"Basic {encodedString}");

        var response = await client.SendAsync(request);

        var mpesaResponse = await response.Content.ReadAsStringAsync();

        Token tokenObject = JsonConvert.DeserializeObject<Token>(mpesaResponse);

        return tokenObject.access_token;
    }

    class Token {
        public string access_token {get;set;}
        public string expires_in {get;set;}
    }

    // register url

    public IActionResult RegisterURLs() {
        return View();
    }

    [HttpGet]
    [Route("register-urls")]
    public async Task<string> RegisterMpesaUrls(){
        var jsonBody = JsonConvert.SerializeObject(new {
           ValidationURL = "https://c3f1-102-135-169-122.eu.ngrok.io/payments/confirmation",
           ConfirmationURL = "https://c3f1-102-135-169-122.eu.ngrok.io/payments/validation",
           ResponseType = "Completed",
           ShortCode = 600977
        });

        var jsonReadyBody = new StringContent(
            jsonBody.ToString(),
            Encoding.UTF8,
            "application/json"
        );

        var token = await GetToken();

        var client = _clientFactory.CreateClient("mpesa");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var url = "/mpesa/c2b/v1/registerurl";

        var response = await client.PostAsync(url, jsonReadyBody);

        return await response.Content.ReadAsStringAsync();
    }

    // Confirmation endpoint
    [HttpPost]
    [Route("payments/confirmation")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<JsonResult> PaymentConfirmation([FromBody]MpesaC2B c2bPayments){
        var respond = new {
            ResponseCode = 0,
            ResponseDesc = "Processed"
        };

        if(ModelState.IsValid) {
            _dbContext.Add(c2bPayments);
            var saveResponse = await _dbContext.SaveChangesAsync();
        } else {
            return Json(new { code = 0, errors = ModelState});
        }

        return Json(respond);

    }

    [HttpPost]
    [Route("payments/validation")]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<JsonResult> PaymentValidation([FromBody]MpesaC2B c2bPayments){
        var respond = new {
            ResponseCode = 0,
            ResponseDesc = "Processed"
        };
        return Json(respond);
    }





























    
}
