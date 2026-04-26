using EO.Models;
using EO.Services.Profile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EO.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemberService _memberService;
        public ProfileController(IProfileService profileService, UserManager<ApplicationUser> userManager, IMemberService memberService)
        {
            _profileService = profileService;
            _userManager = userManager;
            _memberService = memberService;
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

            if (profile == null)
            {
                return View(new UpdateProfileRequest
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

            return View(profile);
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
        public async Task<IActionResult> Submit([FromBody] UpdateProfileRequest request)
        {
            if (request == null)
                return BadRequest("Request is null");

            var actor = await _userManager.GetUserAsync(User);

            if (actor == null)
                return Unauthorized();

            var targetUserId = string.IsNullOrEmpty(request.UserId)
                ? actor.Id
                : request.UserId;

            await _profileService.UpsertProfileAsync(request, targetUserId);

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
    }
}