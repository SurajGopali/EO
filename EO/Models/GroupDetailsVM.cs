namespace EO.Models
{
    public class UserVM
    {
        public string UserId { get; set; } = null!;
        public string? Name { get; set; }
    }
    public class GroupDetailsVM
    {
        public int Id { get; set; }

        public string GroupName { get; set; } = null!;
        public string? Tagline { get; set; }
        public string? CoverImage { get; set; }
        public string? WhatsappLink { get; set; }
        public string? CreatedBy { get; set; }

        public string? Category { get; set; }

        public List<string> Tags { get; set; } = new();

        public List<UserVM> Champions { get; set; } = new();

        public List<UserVM> Coordinators { get; set; } = new();
    }
    public class EditGroupVM
    {
        public int Id { get; set; }

        public string GroupName { get; set; } = null!;
        public string? Tagline { get; set; }
        public string? CoverImage { get; set; }
        public string? WhatsappLink { get; set; }
        public List<string> CategoryNames { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public List<string> ChampionUserIds { get; set; } = new();
        public List<string> CoordinatorUserIds { get; set; } = new();
    }
    public class EditGroupPageVM
    {
        public CreateGroupVM Group { get; set; } = new();

        public List<GroupTag> Tags { get; set; } = new();
        public List<GroupCategory> Categories { get; set; } = new();
        public List<UserVM> Users { get; set; } = new();

        public List<SimpleDto> SelectedTags { get; set; } = new();
        public List<SimpleDto> SelectedCategories { get; set; } = new();
        public List<SimpleDto> SelectedChampions { get; set; } = new();
        public List<SimpleDto> SelectedCoordinators { get; set; } = new();
    }
    public class CreateGroupPageVM
    {
        public CreateGroupVM Group { get; set; } = new();

        public List<UserVM> Users { get; set; } = new();
        public List<GroupCategory> Categories { get; set; } = new();
        public List<GroupTag> Tags { get; set; } = new();
    }
    public class SimpleDto
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }
}
