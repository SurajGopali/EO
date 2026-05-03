using EO.Models;
using EO.Services.Common;
using EO.Services.Profile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace EO.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberService _memberService;
        private readonly ICommonService _commonService;
        public ProfileController(IProfileService profileService, UserManager<ApplicationUser> userManager, IMemberService memberService, ICommonService commonService)
        {
            _profileService = profileService;
            _userManager = userManager;
            _memberService = memberService;
            _commonService = commonService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Setup(string? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var userId = string.IsNullOrEmpty(id) ? user.Id : id;

            var profile = await _profileService.GetProfileAsync(userId);

            var fullName = $"{user.FirstName} {user.MiddleName} {user.LastName}"
                .Replace("  ", " ")
                .Trim();

            ViewData["FullName"] = fullName;

            if (profile == null)
            {
                return PartialView("_SetupPartial", new UpdateProfileRequest
                {
                    UserId = userId,
                    PersonalDetails = new(),
                    CompanyDetails = new(),
                    SocialLinks = new(),
                    Spouse = new SpouseDto
                    {
                        SocialLinks = new(),
                        Professional = new()
                    },
                    Children = new()
                });
            }

            profile.UserId = userId;

            return PartialView("_SetupPartial", profile);
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Submit(
        [FromForm] string data,
        [FromForm] IFormFile? SpouseProfileImageFile,
        [FromForm] List<IFormFile> ChildrenFiles)
        {
            if (string.IsNullOrEmpty(data))
                return BadRequest("Invalid request");

            var actor = await _userManager.GetUserAsync(User);
            if (actor == null)
                return Unauthorized();

            var request = JsonSerializer.Deserialize<UpdateProfileRequest>(
                data,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (request == null)
                return BadRequest("Invalid JSON payload");

            var targetUserId = string.IsNullOrEmpty(request.UserId)
                ? actor.Id
                : request.UserId;

            if (SpouseProfileImageFile != null && request.Spouse != null)
            {
                request.Spouse.ProfileImage =
                    await _commonService.SaveFileAsync(
                        SpouseProfileImageFile,
                        "spouse",
                        targetUserId);
            }

            for (int i = 0; i < request.Children.Count; i++)
            {
                var file = ChildrenFiles.ElementAtOrDefault(i);

                if (file != null && file.Length > 0)
                {
                    request.Children[i].ProfileImage =
                        await _commonService.SaveFileAsync(
                            file,
                            "children",
                            targetUserId);
                }
            }

            var result = await _profileService.UpsertProfileAsync(request, targetUserId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { success = true });
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Members(bool New = false)
        {
            var members = await _memberService.GetMembersAsync(New);

            return View(members);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MemberDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var member = await _memberService.GetMemberDetailAsync(id);

            if (member == null)
                return NotFound();

            return View(member);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleMemberStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Members");
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = await _profileService.GetAsync(userId);

            var dto = new ProfileUpdateDto
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,

                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,

                ProfileImage = user.ProfileImage
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileUpdateDto dto, IFormFile? imageFile)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();


            if (imageFile != null && imageFile.Length > 0)
            {
                var imagePath = await _commonService.SaveFileAsync(
                    imageFile,
                    "profile",
                    userId
                );

                dto.ProfileImage = imagePath;
            }


            var result = await _profileService.UpdateAsync(userId, dto);

            if (!result)
            {
                ModelState.AddModelError("", "Failed to update profile.");
                return View(dto);
            }

            return RedirectToAction("Info");
        }

        public async Task<IActionResult> Info()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _profileService.GetAsync(userId);

            return View(user);
        }
    }
}