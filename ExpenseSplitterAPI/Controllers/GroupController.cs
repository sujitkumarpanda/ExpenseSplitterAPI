using ExpenseSplitterAPI.APIModels;
using ExpenseSplitterAPI.Model;
using ExpenseSplitterAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseSplitterAPI.Controllers
{
    [Route("api/groups")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupRequestModel request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Group name is required.");

            var group = await _groupService.CreateGroup(request);
            return CreatedAtAction(nameof(GetGroupById), new { groupId = group.Id }, group);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<GroupModel>>> GetUserGroups(int userId)
        {
            var groups = await _groupService.GetUserGroups(userId);
            return Ok(groups);
        }


        [HttpGet]
        public async Task<ActionResult<List<Group>>> GetAllGroups()
        {
            var groups = await _groupService.GetAllGroups();
            return Ok(groups);
        }

        [HttpGet("{groupId}")]
        public async Task<ActionResult<Group>> GetGroupById(int groupId)
        {
            var group = await _groupService.GetGroupById(groupId);
            if (group == null)
                return NotFound("Group not found.");

            return Ok(group);
        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var deleted = await _groupService.DeleteGroup(groupId);
            if (!deleted)
                return NotFound("Group not found.");

            return NoContent();
        }

        [HttpDelete("{groupId}/removeUser/{userId}")]
        public async Task<IActionResult> RemoveUserFromGroup(int groupId, int userId)
        {
            var removed = await _groupService.RemoveUserFromGroup(groupId, userId);
            if (!removed)
                return BadRequest("Failed to remove user from group.");

            return Ok("User removed from group successfully.");
        }
        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] GroupRequestModel request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Group name is required.");

            var updatedGroup = await _groupService.UpdateGroup(groupId, request);

            if (updatedGroup == null)
                return NotFound("Group not found.");

            return Ok(updatedGroup);
        }


        [HttpPost("{groupId}/addUser/{userId}")]
        public async Task<IActionResult> AddUserToGroup(int groupId, int userId)
        {
            var added = await _groupService.AddUserToGroup(groupId, userId);
            if (!added)
                return BadRequest("Invalid group or user.");

            return Ok("User added to group successfully.");
        }
    }
}
