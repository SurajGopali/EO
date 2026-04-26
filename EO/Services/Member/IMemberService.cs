using EO.Models;

public interface IMemberService
{
    Task<List<MemberDto>> GetMembersAsync(bool isNew);
    Task<MemberDetailDto> GetMemberDetailAsync(string id);
}