using System.Diagnostics;
using EO.Models;
using EO.Services.Event;
using Microsoft.AspNetCore.Mvc;

namespace EO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        private readonly IMemberService _memberService;
        public HomeController(ILogger<HomeController> logger,IEventService eventService, IMemberService memberService)

        {
            _logger = logger;
            _eventService = eventService;
            _memberService = memberService;
        }

        public async Task<IActionResult> Index()
        {
            var upcomingEvents = await _eventService.GetUpcomingEventsAsync();
            var TodayEvents = await _eventService.GetTodayEventsAsync();
            var upcomingBirthdays = await _memberService.GetUpcomingBirthdaysAsync();
            var newMembers = await _memberService.GetNewJoineesThisMonthAsync();
            var UpcomingAnniversaries = await _memberService.GetUpcomingAnniversariesAsync();

            var model = new HomeDashboardModel
            {
                UpcomingEvents = upcomingEvents,
                TodayEvents = TodayEvents,
                UpcomingBirthdays = upcomingBirthdays,
                NewMembers = newMembers,
                UpcomingAnniversaries = UpcomingAnniversaries

            };

            return View(model);
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
    }
}
