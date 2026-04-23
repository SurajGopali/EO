using EO.Models;
using EO.WebContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EO.Controllers.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public ForumsController(
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetForums()
        {
            var forums = new
            {
                joinedForums = new object[]
                {
                    new {
                        id = "forum001",
                        name = "Tech Innovators",
                        category = "Technology",
                        description = "A community for tech enthusiasts to share ideas and innovations.",
                        membersCount = 1200,
                        userRole = "Member",
                        lastActivity = "2026-03-25T14:32:00Z",
                        image = "https://picsum.photos/id/1011/500/300"
                    },
                    new {
                        id = "forum002",
                        name = "Finance Gurus",
                        category = "Finance",
                        description = "Discuss investments, stocks, and personal finance tips.",
                        membersCount = 950,
                        userRole = "Moderator",
                        lastActivity = "2026-03-24T18:45:00Z",
                        image = "https://picsum.photos/id/1016/500/300"
                    },
                    new {
                        id = "forum001",
                        name = "Tech Innovators",
                        category = "Technology",
                        description = "A community for tech enthusiasts to share ideas and innovations.",
                        membersCount = 1200,
                        userRole = "Member",
                        lastActivity = "2026-03-25T14:32:00Z",
                        image = "https://picsum.photos/id/1011/400/200"
                    },
                    new {
                        id = "forum002",
                        name = "Finance Gurus",
                        category = "Finance",
                        description = "Discuss investments, stocks, and personal finance tips.",
                        membersCount = 950,
                        userRole = "Moderator",
                        lastActivity = "2026-03-24T18:45:00Z",
                        image = "https://picsum.photos/id/1025/400/200"
                    },
                    new {
                        id = "forum003",
                        name = "Mind & Body Wellness",
                        category = "Health",
                        description = "A supportive space for mental health, fitness, and wellness.",
                        membersCount = 700,
                        userRole = "Member",
                        lastActivity = "2026-03-23T09:10:00Z",
                        image = "https://picsum.photos/id/1035/400/200"
                    },
                    new {
                        id = "forum004",
                        name = "Startup Founders Hub",
                        category = "Business",
                        description = "Networking and discussion for startup founders and entrepreneurs.",
                        membersCount = 580,
                        userRole = "Member",
                        lastActivity = "2026-03-25T11:20:00Z",
                        image = "https://picsum.photos/id/1045/400/200"
                    },
                    new {
                        id = "forum005",
                        name = "Book Lovers Circle",
                        category = "Hobbies",
                        description = "Share reviews, recommendations, and discussions about books.",
                        membersCount = 340,
                        userRole = "Member",
                        lastActivity = "2026-03-22T16:05:00Z",
                        image = "https://picsum.photos/id/1055/400/200"
                    }
                },

                spouses = new object[]
                {
                    new {
                        id = "spouse001",
                        name = "Alice",
                        forumName = "Tech Innovators",
                        image = "https://picsum.photos/id/1012/100/100"
                    },
                    new {
                        id = "spouse002",
                        name = "Bob",
                        forumName = "Finance Gurus",
                        image = "https://picsum.photos/id/1013/100/100"
                    }
                },

                interestedToJoin = new object[]
                {
                    new {
                        id = "interested001",
                        name = "Charlie",
                        forumName = "Tech Innovators",
                        image = "https://picsum.photos/id/1014/100/100"
                    },
                    new {
                        id = "interested002",
                        name = "David",
                        forumName = "Finance Gurus",
                        image = "https://picsum.photos/id/1015/100/100"
                    }
                }
            };

            return Ok(new
            {
                success = true,
                message = "Forums Fetched Successfully",
                Forums = forums
            });
        }
    }
}

