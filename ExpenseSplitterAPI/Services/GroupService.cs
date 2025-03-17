using ExpenseSplitterAPI.APIModels;
using ExpenseSplitterAPI.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Services
{
    public class GroupService : IGroupService
    {
        private readonly ExpenseSplitterDbContext _context;

        public GroupService(ExpenseSplitterDbContext context)
        {
            _context = context;
        }

        public async Task<Group> CreateGroup(GroupRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Members == null || request.Members.Count == 0)
            {
                throw new ArgumentException("Group name and at least one member are required.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var group = new Group { Name = request.Name };

                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                foreach (var userId in request.Members)
                {
                    var membership = new GroupMember { GroupId = group.Id, UserId = userId };
                    _context.GroupMembers.Add(membership);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return group;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<GroupModel>> GetAllGroups()
        {
            var groups = await _context.Groups
                .Include(g => g.GroupMembers)
                .ThenInclude(gm => gm.User)
                .AsNoTracking()
                .ToListAsync();

            return groups.Select(g => new GroupModel
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Members = g.GroupMembers.Select(gm => new UserModel
                {
                    Id = gm.User.Id,
                    Name = gm.User.Name
                }).ToList()
            }).ToList();
        }


        public async Task<Group> GetGroupById(int groupId)
        {
            return await _context.Groups
                .Include(g => g.GroupMembers)
                .ThenInclude(gm => gm.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

        public async Task<bool> DeleteGroup(int groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null) return false;

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<GroupModel>> GetUserGroups(int userId)
        {
            var groups = await _context.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Include(gm => gm.Group)
                    .ThenInclude(g => g.GroupMembers)
                        .ThenInclude(gm => gm.User)
                .Select(gm => gm.Group)
                .ToListAsync();

            return groups.Select(g => new GroupModel
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Members = g.GroupMembers.Select(gm => new UserModel
                {
                    Id = gm.User.Id,
                    Name = gm.User.Name
                }).ToList()
            }).ToList();
        }


        public async Task<bool> AddUserToGroup(int groupId, int userId)
        {
            var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupId);
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!groupExists || !userExists) return false;

            var membership = new GroupMember { GroupId = groupId, UserId = userId };
            _context.GroupMembers.Add(membership);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
