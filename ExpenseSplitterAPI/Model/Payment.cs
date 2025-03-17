using System;
using System.Collections.Generic;

namespace ExpenseSplitterAPI.Model;

public partial class Payment
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int FromUserId { get; set; }

    public int ToUserId { get; set; }

    public decimal Amount { get; set; }

    public virtual User FromUser { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual User ToUser { get; set; } = null!;
}
