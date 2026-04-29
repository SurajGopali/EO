namespace EO.Models
{
    public class Alliance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Logo { get; set; }

        public int AllianceTypeId { get; set; }
        public AllianceType AllianceType { get; set; }

        public ICollection<AlliancePerk> AlliancePerks { get; set; } = new List<AlliancePerk>();
    }

    public class Perk
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<AlliancePerk> AlliancePerks { get; set; } = new List<AlliancePerk>();
    }

    public class AllianceType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Alliance> Alliances { get; set; } = new List<Alliance>();
    }

    public class AlliancePerk
    {
        public int AllianceId { get; set; }
        public Alliance Alliance { get; set; }

        public int PerkId { get; set; }
        public Perk Perk { get; set; }
    }


    public class AllianceDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string? Logo { get; set; }

        public IFormFile? LogoFile { get; set; }

        public int AllianceTypeId { get; set; }
        public string? AllianceTypeName { get; set; }

        public List<int> PerkIds { get; set; } = new();

        public List<PerkDto> Perks { get; set; } = new();
    }
    public class PerkDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
