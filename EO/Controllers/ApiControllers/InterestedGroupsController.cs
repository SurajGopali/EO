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
    public class InterestedGroupsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public InterestedGroupsController(
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetInterestedGroups()
        {
            var interestedGroups = new
            {
                groups = new object[]
                {
                    new {
                        groupName = "MyEO Light Mind Body Soul",
                        tagline = "Embrace tranquility, mindfulness & positive vibes.",
                        coverImage = "https://images.unsplash.com/photo-1506126613408-eca07ce68773",
                        whatsappLink = "https://chat.whatsapp.com/mindbodysoul",
                        category = "Wellness",
                        tags = new[] { "mindfulness", "meditation", "wellbeing" },
                        champions = new[] {
                            new { name = "Emma Richardson", eo = "EO London" }
                        },
                        coordinators = new[] {
                            new { name = "Daniel Carter", eo = "EO Sydney" }
                        }
                    },
                    new {
                        groupName = "MyEO Light PAWsome",
                        tagline = "A community for pet lovers and animal enthusiasts.",
                        coverImage = "https://images.unsplash.com/photo-1517849845537-4d257902454a",
                        whatsappLink = "https://chat.whatsapp.com/pawsome",
                        category = "Lifestyle",
                        tags = new[] { "pets", "animals", "community" },
                        champions = new[] {
                            new { name = "Liam Anderson", eo = "EO Toronto" }
                        },
                        coordinators = new[] {
                            new { name = "Sophia Martinez", eo = "EO Barcelona" }
                        }
                    },
                    new {
                        groupName = "MyEO Light Poetry",
                        tagline = "Where words flow and creativity thrives.",
                        coverImage = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d",
                        whatsappLink = "https://chat.whatsapp.com/poetry",
                        category = "Creative",
                        tags = new[] { "poetry", "writing", "expression" },
                        champions = new[] {
                            new { name = "Noah Bennett", eo = "EO New York" }
                        },
                        coordinators = new[] {
                            new { name = "Olivia Clark", eo = "EO Melbourne" }
                        }
                    },
                    new {
                        groupName = "Helping Hands",
                        tagline = "Extending support and creating meaningful impact.",
                        coverImage = "https://images.unsplash.com/photo-1521737604893-d14cc237f11d",
                        whatsappLink = "https://chat.whatsapp.com/helpinghands",
                        category = "Social Impact",
                        tags = new[] { "charity", "support", "community" },
                        champions = new[] {
                            new { name = "Ethan Walker", eo = "EO Chicago" }
                        },
                        coordinators = new[] {
                            new { name = "Ava Thompson", eo = "EO Singapore" }
                        }
                    },
                    new {
                        groupName = "MyEO Light Picasso",
                        tagline = "Unleashing creativity through art and design.",
                        coverImage = "https://images.unsplash.com/photo-1513364776144-60967b0f800f",
                        whatsappLink = "https://chat.whatsapp.com/picasso",
                        category = "Creative",
                        tags = new[] { "art", "design", "creativity" },
                        champions = new[] {
                            new { name = "Lucas Martin", eo = "EO Paris" }
                        },
                        coordinators = new[] {
                            new { name = "Isabella Rossi", eo = "EO Milan" }
                        }
                    },
                    new {
                        groupName = "MyEO Light Travel Trails 2",
                        tagline = "Explore destinations and share travel experiences.",
                        coverImage = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e",
                        whatsappLink = "https://chat.whatsapp.com/traveltrails",
                        category = "Travel",
                        tags = new[] { "travel", "exploration", "adventure" },
                        champions = new[] {
                            new { name = "William Scott", eo = "EO Dubai" }
                        },
                        coordinators = new[] {
                            new { name = "Mia Wilson", eo = "EO Cape Town" }
                        }
                    },
                    new {
                        groupName = "MyEO Light Books & Netflix",
                        tagline = "Discuss books, movies and binge-worthy series.",
                        coverImage = "https://images.unsplash.com/photo-1512820790803-83ca734da794",
                        whatsappLink = "https://chat.whatsapp.com/booksnetflix",
                        category = "Entertainment",
                        tags = new[] { "books", "movies", "series" },
                        champions = new[] {
                            new { name = "James Turner", eo = "EO Boston" }
                        },
                        coordinators = new[] {
                            new { name = "Charlotte Green", eo = "EO Dublin" }
                        }
                    },
                    new {
                        groupName = "Music Lovers",
                        tagline = "Connecting through rhythm, beats and melodies.",
                        coverImage = "https://images.unsplash.com/photo-1507874457470-272b3c8d8ee2",
                        whatsappLink = "https://chat.whatsapp.com/musiclovers",
                        category = "Entertainment",
                        tags = new[] { "music", "songs", "community" },
                        champions = new[] {
                            new { name = "Benjamin Hall", eo = "EO Los Angeles" }
                        },
                        coordinators = new[] {
                            new { name = "Amelia Young", eo = "EO Berlin" }
                        }
                    },
                    new {
                        groupName = "MyEO Light Leads and Needs",
                        tagline = "Collaborate, share leads and grow together.",
                        coverImage = "https://images.unsplash.com/photo-1556740738-b6a63e27c4df",
                        whatsappLink = "https://chat.whatsapp.com/leadsneeds",
                        category = "Business",
                        tags = new[] { "networking", "leads", "growth" },
                        champions = new[] {
                            new { name = "Henry King", eo = "EO Hong Kong" }
                        },
                        coordinators = new[] {
                            new { name = "Harper Adams", eo = "EO Amsterdam" }
                        }
                    },
                    new {
                        groupName = "MyEO Light Build My Space",
                        tagline = "Ideas and inspiration for building better spaces.",
                        coverImage = "https://images.unsplash.com/photo-1505691938895-1758d7feb511",
                        whatsappLink = "https://chat.whatsapp.com/buildspace",
                        category = "Lifestyle",
                        tags = new[] { "architecture", "design", "spaces" },
                        champions = new[] {
                            new { name = "Alexander Wright", eo = "EO San Francisco" }
                        },
                        coordinators = new[] {
                            new { name = "Ella Baker", eo = "EO Vancouver" }
                        }
                    }
                }
            };

            return Ok(new
            {
                success = true,
                message = "Interested Groups Fetched Successfully",
                InterestedGroups = interestedGroups
            });
        }
    }
}

