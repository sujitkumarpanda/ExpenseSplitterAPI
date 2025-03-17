using System;
using System.Collections.Generic;

namespace ExpenseSplitterAPI.Model;

public partial class Debt
{
    public int Id { get; set; }

    public int ExpenseId { get; set; }

    public int GroupId { get; set; }

    public int OwedByUserId { get; set; }

    public int OwedToUserId { get; set; }

    public decimal Amount { get; set; }

    public virtual Expense Expense { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual User OwedByUser { get; set; } = null!;

    public virtual User OwedToUser { get; set; } = null!;
}
