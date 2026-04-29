namespace EO.Models
{
    public class HomeDashboardModel
    {
        public List<EventDto> UpcomingEvents { get; set; } = new();
        public List<BirthdayDto> UpcomingBirthdays { get; set; } = new();
        public List<NewMemberDto> NewMembers { get; set; } = new();
        public List<EventDto> TodayEvents { get; set; } = new();
        public List<AnniversaryDto> UpcomingAnniversaries { get; set; } = new();

        public int TotalEventsThisMonth { get; set; }
        public int RegisteredEvents { get; set; }
    }
}
