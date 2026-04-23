using EO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EO.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Setup()
        {
            var model = new UpdateProfileRequest
            {
                PersonalDetails = new PersonalDetailsDto(),
                CompanyDetails = new CompanyDetailsDto(),
                SocialLinks = new SocialLinksDto(),
                Spouse = new SpouseDto
                {
                    SocialLinks = new SocialLinksDto(),
                    Professional = new SpouseProfessionalDto()
                },
                Children = new List<ChildDto>()
            };

            ViewBag.Step = 1;
            return View(model);
        }

        [HttpPost]
        public IActionResult Next([FromBody] int step)
        {
            return Json(new { step = step + 1 });
        }

        [HttpPost]
        public IActionResult Previous([FromBody] int step)
        {
            return Json(new { step = step - 1 });
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] UpdateProfileRequest request)
        {
            var token = Request.Cookies["accessToken"];

            if (string.IsNullOrEmpty(token))
                return Unauthorized("User not logged in");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "https://localhost:7236/api/profile/update-profile",
                content
            );

            if (!response.IsSuccessStatusCode)
                return BadRequest(await response.Content.ReadAsStringAsync());

            return Ok(new { success = true });
        }
    }
}