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
    public class AllianceController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public AllianceController(
            UserManager<ApplicationUser> userManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAllianceData()
        {
            var alliance = new
            {
                types = new[] { "All", "Hospitality", "Travel", "Fashion", "Lifestyle", "Others" },
                alliances = new object[]
                {
                    new {
                        name = "Tablet Hotels",
                        category = "Hospitality",
                        logo = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS1YyDq2jNA_NjBsqngmVECA0RNM9fnUTCzdg&s",
                        description = "Curated luxury boutique hotels around the world.",
                        perks = new[] { "Up to 15% discount", "Free room upgrades", "Priority booking" }
                    },
                    new {
                        name = "Fiverr",
                        category = "Others",
                        logo = "https://static.vecteezy.com/system/resources/previews/025/732/716/non_2x/fiverr-logo-icon-online-platform-for-freelancers-free-vector.jpg",
                        description = "Global freelance marketplace for digital services.",
                        perks = new[] { "Exclusive credits", "Discounted services", "Priority freelancers" }
                    },
                    new {
                        name = "FoundersCard",
                        category = "Others",
                        logo = "https://d2q86wmri3hsp2.cloudfront.net/assets/blank_card_no_shadow-df10a0136e2a32386b7af007f3c8e22995bc99fa82d7b62a7753e03e52e93765.png",
                        description = "Membership community for entrepreneurs and founders.",
                        perks = new[] { "VIP networking", "Business discounts", "Private events access" }
                    },
                    new {
                        name = "Sixt",
                        category = "Travel",
                        logo = "https://play-lh.googleusercontent.com/MWy4qJMCVD1OQ9fYg1ZFf9Qu_W39wvo1oYD1fKhWfZIl-QZehNfuC7gwsnz_9aZXYw",
                        description = "Premium car rental services worldwide.",
                        perks = new[] { "Discounted rentals", "Free upgrades", "Priority reservations" }
                    },
                    new {
                        name = "Marshall Goldsmith",
                        category = "Lifestyle",
                        logo = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSK0bNoTaHH0fFX3gGsyENi1kJ8wV_CgAMRUA&s",
                        description = "Executive coaching and leadership development programs.",
                        perks = new[] { "Exclusive workshops", "Discounted coaching", "Leadership sessions" }
                    },
                    new {
                        name = "SaffronStays",
                        category = "Hospitality",
                        logo = "https://www.visa.com/images/merchantoffers/2022-11/1669790053057.SaffronStays_Logo.jpeg",
                        description = "Handpicked luxury villas and vacation homes.",
                        perks = new[] { "Early check-in", "Special pricing", "Free experiences" }
                    },
                    new {
                        name = "Emirates",
                        category = "Travel",
                        logo = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/Emirates_banner_logo.svg/3840px-Emirates_banner_logo.svg.png",
                        description = "World-class international airline with premium services.",
                        perks = new[] { "Seat upgrades", "Extra baggage", "Lounge access" }
                    },
                    new {
                        name = "Nike",
                        category = "Fashion",
                        logo = "https://upload.wikimedia.org/wikipedia/commons/a/a6/Logo_NIKE.svg",
                        description = "Global leader in sportswear and athletic gear.",
                        perks = new[] { "10% discount", "Early access to drops", "Member-only collections" }
                    },
                    new {
                        name = "Airbnb",
                        category = "Travel",
                        logo = "https://upload.wikimedia.org/wikipedia/commons/6/69/Airbnb_Logo_Bélo.svg",
                        description = "Unique stays and experiences across the globe.",
                        perks = new[] { "Discounted stays", "Priority booking", "Exclusive listings" }
                    },
                    new {
                        name = "Zara",
                        category = "Fashion",
                        logo = "https://upload.wikimedia.org/wikipedia/commons/f/fd/Zara_Logo.svg",
                        description = "Trendy and fast-fashion clothing brand.",
                        perks = new[] { "Seasonal discounts", "Early sale access", "Member previews" }
                    },
                    new {
                        name = "WeWork",
                        category = "Lifestyle",
                        logo = "https://upload.wikimedia.org/wikipedia/commons/3/3b/WeWork_logo.svg",
                        description = "Flexible coworking spaces for modern professionals.",
                        perks = new[] { "Discounted memberships", "Free day passes", "Meeting room credits" }
                    },
                    new {
                        name = "Spotify",
                        category = "Lifestyle",
                        logo = "https://upload.wikimedia.org/wikipedia/commons/2/26/Spotify_logo_with_text.svg",
                        description = "Digital music streaming platform.",
                        perks = new[] { "Premium discounts", "Ad-free listening", "Exclusive playlists" }
                    },
                    new {
                        name = "Adidas",
                        category = "Fashion",
                        logo = "https://upload.wikimedia.org/wikipedia/commons/2/20/Adidas_Logo.svg",
                        description = "Popular brand for sportswear and lifestyle products.",
                        perks = new[] { "Flat discounts", "Early product launches", "Member rewards" }
                    },
                    new {
                        name = "Expedia",
                        category = "Travel",
                        logo = "https://upload.wikimedia.org/wikipedia/commons/5/5b/Expedia_Logo.svg",
                        description = "Online travel booking platform for flights and hotels.",
                        perks = new[] { "Travel discounts", "Bundle deals", "Reward points" }
                    }
                }
            };

            return Ok(alliance);
        }
    }
}

