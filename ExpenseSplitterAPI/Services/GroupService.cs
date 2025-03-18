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
                var group = new Group { Name = request.Name,
                                        Description=request.Description
                
                                         };

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


        public async Task<GroupModel> GetGroupById(int groupId)
        {
            var group = await _context.Groups
                .Include(g => g.GroupMembers)  
                .ThenInclude(gm => gm.User)   
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
                return null;

            return new GroupModel
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Members = group.GroupMembers.Select(gm => new UserModel
                {
                    Id = gm.User.Id,
                    Name = gm.User.Name
                }).ToList()
            };
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
        public async Task<bool> RemoveUserFromGroup(int groupId, int userId)
        {
            var membership = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

            if (membership == null)
                return false;

            _context.GroupMembers.Remove(membership);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<GroupModel> UpdateGroup(int groupId, GroupRequestModel request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var group = await _context.Groups
                    .Include(g => g.GroupMembers) // Ensure members are loaded
                    .ThenInclude(m => m.User)     // Ensure user data is included
                    .FirstOrDefaultAsync(g => g.Id == groupId);

                if (group == null)
                    return null; // Group not found

                // ✅ Update group details
                group.Name = request.Name;
                group.Description = request.Description;

                // ✅ Ensure `request.Members` is not null
                var newMembers = request.Members ?? new List<int>();
                var existingMembers = group.GroupMembers?.Select(m => m.UserId).ToList() ?? new List<int>();

                var membersToRemove = existingMembers.Except(newMembers).ToList();
                var membersToAdd = newMembers.Except(existingMembers).ToList();

                // ✅ Remove users who are no longer part of the group
                _context.GroupMembers.RemoveRange(
                    group.GroupMembers.Where(m => membersToRemove.Contains(m.UserId))
                );

                // ✅ Add new members
                foreach (var userId in membersToAdd)
                {
                    _context.GroupMembers.Add(new GroupMember { GroupId = groupId, UserId = userId });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // ✅ Return updated group details
                return new GroupModel
                {
                    Id = group.Id,
                    Name = group.Name,
                    Description = group.Description,
                    Members = group.GroupMembers
                        .Where(m => m.User != null) // Ensure User exists
                        .Select(m => new UserModel
                        {
                            Id = m.User.Id,
                            Name = m.User.Name
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Error updating group: " + ex.Message); // Log for debugging
                throw;
            }
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
