using EO.Models;

public interface IMemberService
{
    Task<List<MemberDto>> GetMembersAsync(bool isNew);
    Task<MemberDetailDto> GetMemberDetailAsync(string id);
    Task<List<BirthdayDto>> GetUpcomingBirthdaysAsync();
    Task<List<NewMemberDto>> GetNewJoineesThisMonthAsync();
    Task<List<AnniversaryDto>> GetUpcomingAnniversariesAsync();

}