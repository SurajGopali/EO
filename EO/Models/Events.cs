namespace EO.Models
{
    public class Events
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; }
        public bool? IsRegistered { get; set; }
        public string? Image {  get; set; }
    }
}
