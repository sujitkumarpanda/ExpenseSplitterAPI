using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitterAPI.Model;

public partial class ExpenseSplitterDbContext : DbContext
{
    public ExpenseSplitterDbContext()
    {
    }

    public ExpenseSplitterDbContext(DbContextOptions<ExpenseSplitterDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Debt> Debts { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=localhost;integrated security=SSPI;database=ExpenseSplitterDB;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Debt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Debts__3214EC07DE435DF0");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Expense).WithMany(p => p.Debts)
                .HasForeignKey(d => d.ExpenseId)
                .HasConstraintName("FK__Debts__ExpenseId__5DCAEF64");

            entity.HasOne(d => d.Group).WithMany(p => p.Debts)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Debts__GroupId__5EBF139D");

            entity.HasOne(d => d.OwedByUser).WithMany(p => p.DebtOwedByUsers)
                .HasForeignKey(d => d.OwedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Debts__OwedByUse__5FB337D6");

            entity.HasOne(d => d.OwedToUser).WithMany(p => p.DebtOwedToUsers)
                .HasForeignKey(d => d.OwedToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Debts__OwedToUse__60A75C0F");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Expenses__3214EC074FBCC520");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.Group).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__Expenses__GroupI__403A8C7D");

            entity.HasOne(d => d.Payer).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.PayerId)
                .HasConstraintName("FK__Expenses__PayerI__412EB0B6");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Groups__3214EC074097A5A3");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Groups)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_User_GroupId");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GroupMem__3214EC074940D025");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__GroupMemb__Group__3C69FB99");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__GroupMemb__UserI__3D5E1FD2");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC07E39506A8");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.FromUser).WithMany(p => p.PaymentFromUsers)
                .HasForeignKey(d => d.FromUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__FromUs__52593CB8");

            entity.HasOne(d => d.Group).WithMany(p => p.Payments)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK__Payments__GroupI__5165187F");

            entity.HasOne(d => d.ToUser).WithMany(p => p.PaymentToUsers)
                .HasForeignKey(d => d.ToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__ToUser__534D60F1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07225BF622");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534EB0F1577").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);

            entity.HasOne(d => d.Group).WithMany(p => p.Users)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("User_Groupid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
