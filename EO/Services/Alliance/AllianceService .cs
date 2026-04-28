using Microsoft.EntityFrameworkCore;
using EO.WebContext;
using EO.Models;

public class AllianceService : IAllianceService
{
    private readonly AppDbContext _context;

    public AllianceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AllianceDto>> GetAllAsync()
    {
        return await _context.Alliances
            .Include(a => a.AlliancePerks)
            .Select(a => new AllianceDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Logo = a.Logo,
                AllianceTypeId = a.AllianceTypeId,
                PerkIds = a.AlliancePerks.Select(p => p.PerkId).ToList()
            })
            .ToListAsync();
    }

    public async Task<AllianceDto> GetByIdAsync(int id)
    {
        var alliance = await _context.Alliances
            .Include(x => x.AlliancePerks)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (alliance == null)
            return null;

        return new AllianceDto
        {
            Id = alliance.Id,
            Name = alliance.Name,
            Description = alliance.Description,
            AllianceTypeId = alliance.AllianceTypeId,

            Logo = alliance.Logo,

            PerkIds = alliance.AlliancePerks.Select(p => p.PerkId).ToList()
        };
    }

    public async Task<int> CreateAsync(AllianceDto dto)
    {
        var alliance = new Alliance
        {
            Name = dto.Name,
            Description = dto.Description,
            Logo = dto.Logo,
            AllianceTypeId = dto.AllianceTypeId
        };

        _context.Alliances.Add(alliance);
        await _context.SaveChangesAsync();

        if (dto.PerkIds.Any())
        {
            foreach (var perkId in dto.PerkIds)
            {
                _context.AlliancePerks.Add(new AlliancePerk
                {
                    AllianceId = alliance.Id,
                    PerkId = perkId
                });
            }

            await _context.SaveChangesAsync();
        }

        return alliance.Id;
    }

    public async Task<bool> UpdateAsync(AllianceDto dto)
    {
        var alliance = await _context.Alliances
            .Include(a => a.AlliancePerks)
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (alliance == null) return false;

        alliance.Name = dto.Name;
        alliance.Description = dto.Description;
        alliance.Logo = dto.Logo;
        alliance.AllianceTypeId = dto.AllianceTypeId;

        // remove old perks
        _context.AlliancePerks.RemoveRange(alliance.AlliancePerks);

        // add new perks
        foreach (var perkId in dto.PerkIds)
        {
            _context.AlliancePerks.Add(new AlliancePerk
            {
                AllianceId = alliance.Id,
                PerkId = perkId
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var alliance = await _context.Alliances.FindAsync(id);
        if (alliance == null) return false;

        _context.Alliances.Remove(alliance);
        await _context.SaveChangesAsync();

        return true;
    }
}