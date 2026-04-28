using EO.Models;

public interface IAllianceService
{
    Task<List<AllianceDto>> GetAllAsync();
    Task<AllianceDto?> GetByIdAsync(int id);

    Task<int> CreateAsync(AllianceDto dto);
    Task<bool> UpdateAsync(AllianceDto dto);
    Task<bool> DeleteAsync(int id);
}