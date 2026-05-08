namespace EO.Models
{
    public class CreateGroupVM
    {
        public int Id { get; set; }
        public string GroupName { get; set; }

        public string? Tagline { get; set; }

        public string? CoverImage { get; set; }

        public string? WhatsappLink { get; set; }

        public List<string>? CategoryName { get; set; }

        public List<string> Tags { get; set; } = new();

        public List<string> ChampionUserIds { get; set; } = new();

        public List<string> CoordinatorUserIds { get; set; } = new();
    }
    public class GroupFormDataVM
    {
        public List<GroupTag> Tags { get; set; } = new();
        public List<GroupCategory> Categories { get; set; } = new();
        public List<UserVM> Users { get; set; } = new();
    }

}
