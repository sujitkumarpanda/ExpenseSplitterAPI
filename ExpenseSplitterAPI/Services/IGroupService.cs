using ExpenseSplitterAPI.Model;
using ExpenseSplitterAPI.APIModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Services
{
    public interface IGroupService
    {
        Task<bool> RemoveUserFromGroup(int groupId, int userId);
        Task<Group> CreateGroup(GroupRequestModel request);
        Task<List<GroupModel>> GetAllGroups();

        Task<GroupModel> UpdateGroup(int groupId, GroupRequestModel request);
        Task<GroupModel> GetGroupById(int groupId);
        Task<bool> DeleteGroup(int groupId);
        Task<bool> AddUserToGroup(int groupId, int userId);
        Task<List<GroupModel>> GetUserGroups(int userId);
    }
}
