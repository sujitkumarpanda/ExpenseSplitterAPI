using ExpenseSplitterAPI.Model;
using ExpenseSplitterAPI.APIModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Services
{
    public interface IGroupService
    {
        Task<Group> CreateGroup(GroupRequestModel request);
        Task<List<GroupModel>> GetAllGroups();
        Task<Group> GetGroupById(int groupId);
        Task<bool> DeleteGroup(int groupId);
        Task<bool> AddUserToGroup(int groupId, int userId);
        Task<List<GroupModel>> GetUserGroups(int userId);
    }
}
