namespace EO.Models
{
    public class Alliance
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Category { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }

        public ICollection<AlliancePerk> Perks { get; set; }
    }

    public class AlliancePerk
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public int AllianceId { get; set; }
        public Alliance Alliance { get; set; }
    }
    public class AllianceType
    {
        public int Id { get; set; }
        public string Name { get; set; }  
    }

    public class AllianceDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Logo { get; set; }
        public string Description { get; set; }
        public List<string> Perks { get; set; }
    }
}
