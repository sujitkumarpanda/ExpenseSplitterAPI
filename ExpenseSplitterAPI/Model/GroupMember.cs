using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExpenseSplitterAPI.Model;

public partial class GroupMember
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int UserId { get; set; }
    [JsonIgnore]
    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
