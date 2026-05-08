using EO.Models;

namespace EO.Services.Group
{
    public interface IGroupService
    {
        Task<int> CreateGroupAsync(CreateGroupVM model);

        Task<List<GroupDetailsVM>> GetAllGroupsAsync();

        Task<GroupDetailsVM> GetGroupByIdAsync(int id);

        Task<GroupFormDataVM> GetFormDataAsync();

        Task<EditGroupPageVM?> GetEditDataAsync(int id);
        Task UpdateGroupAsync(EditGroupVM model);

        Task DeleteGroupAsync(int id);
    }
}
