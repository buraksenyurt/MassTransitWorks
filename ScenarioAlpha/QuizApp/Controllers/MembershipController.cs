using Microsoft.AspNetCore.Mvc;
using QuizApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace QuizApp.Controllers;

public class MembershipController(IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<MembershipController> logger) : Controller
{
    private readonly IHttpClientFactory _clientFactory = clientFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<MembershipController> _logger = logger;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        var client = _clientFactory.CreateClient();
        var loginRequest = new { loginViewModel.MemberName, loginViewModel.Password };
        _logger.LogInformation("Login request for {MemberName}", loginRequest.MemberName);
        var response = await client.PostAsJsonAsync($"{_configuration["AuthService:BaseUrl"]}/login", loginRequest);

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            HttpContext.Session.SetString("JWToken", tokenResponse.Token);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View();
    }
}

public record TokenResponse(string Token);