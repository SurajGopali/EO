namespace EO.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; } = null!;
        public string? Tagline { get; set; }
        public string? CoverImage { get; set; }
        public string? WhatsappLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class GroupTag
    {
        public int Id { get; set; }
        public string TagName { get; set; } = null!;
    }

    public class GroupCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
    }
    public class GroupTagMap
    {
        public int GroupId { get; set; }
        public int TagId { get; set; }
    }
    public class GroupCategoryMap
    {
        public int GroupId { get; set; }
        public int CategoryId { get; set; }
    }

    public class GroupChampionsMap
    {
        public int GroupId { get; set; }
        public string UserId { get; set; } = null!;
    }
    public class GroupCoordinatorsMap
    {
        public int GroupId { get; set; }
        public string UserId { get; set; } = null!;
    }
}
