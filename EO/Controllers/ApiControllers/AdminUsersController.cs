using EO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EO.Controllers.ApiControllers
{
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateMemberForm()
        {
            var model = new RegisterMemberDto(); 
            return PartialView("_RegisterMemberPartial", model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] RegisterMemberDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                });
            }

            var user = new ApplicationUser
            {
                UserName = input.Email,
                Email = input.Email,
                PhoneNumber = input.MobileNumber,
                FirstName = input.FirstName,
                MiddleName = input.MiddleName,
                LastName = input.LastName,
                EoRole = input.EoRole,
                Gender = input.Gender,
                JoinedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "User creation failed",
                });
            }

            return Ok(new
            {
                success = true,
                message = "Member created successfully"
            });
        }
    }
}
