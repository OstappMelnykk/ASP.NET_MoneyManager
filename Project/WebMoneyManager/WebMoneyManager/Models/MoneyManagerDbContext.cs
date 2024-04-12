using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebMoneyManager.Models;

public partial class MoneyManagerDbContext : DbContext
{
    public MoneyManagerDbContext()
    {
    }

    public MoneyManagerDbContext(DbContextOptions<MoneyManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Goal> Goals { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=MoneyManagerDB;Username=postgres;Password=1212");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountsId).HasName("accounts_pkey");

            entity.ToTable("accounts");

            entity.Property(e => e.AccountsId).HasColumnName("accounts_id");
            entity.Property(e => e.AccountsTitle)
                .HasMaxLength(255)
                .HasColumnName("accounts_title");
            entity.Property(e => e.FkUsersId).HasColumnName("fk_users_id");

            entity.HasOne(d => d.FkUsers).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.FkUsersId)
                .HasConstraintName("accounts_fk_users_id_fkey");
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.GoalsId).HasName("goals_pkey");

            entity.ToTable("goals");

            entity.Property(e => e.GoalsId).HasColumnName("goals_id");
            entity.Property(e => e.FkAccountsId).HasColumnName("fk_accounts_id");
            entity.Property(e => e.GoalsAmounttocollect)
                .HasPrecision(18, 2)
                .HasColumnName("goals_amounttocollect");
            entity.Property(e => e.GoalsDescription).HasColumnName("goals_description");
            entity.Property(e => e.GoalsTitle)
                .HasMaxLength(255)
                .HasColumnName("goals_title");

            entity.HasOne(d => d.FkAccounts).WithMany(p => p.Goals)
                .HasForeignKey(d => d.FkAccountsId)
                .HasConstraintName("goals_fk_accounts_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionsId).HasName("transactions_pkey");

            entity.ToTable("transactions");

            entity.Property(e => e.TransactionsId).HasColumnName("transactions_id");
            entity.Property(e => e.FkAccountsIdFrom).HasColumnName("fk_accounts_id_from");
            entity.Property(e => e.FkAccountsIdTo).HasColumnName("fk_accounts_id_to");
            entity.Property(e => e.TransactionsDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("transactions_date");
            entity.Property(e => e.TransactionsDescription).HasColumnName("transactions_description");
            entity.Property(e => e.TransactionsSum)
                .HasPrecision(18, 2)
                .HasColumnName("transactions_sum");
            entity.Property(e => e.TransactionsType).HasColumnName("transactions_type");

            entity.HasOne(d => d.FkAccountsIdFromNavigation).WithMany(p => p.TransactionFkAccountsIdFromNavigations)
                .HasForeignKey(d => d.FkAccountsIdFrom)
                .HasConstraintName("transactions_fk_accounts_id_from_fkey");

            entity.HasOne(d => d.FkAccountsIdToNavigation).WithMany(p => p.TransactionFkAccountsIdToNavigations)
                .HasForeignKey(d => d.FkAccountsIdTo)
                .HasConstraintName("transactions_fk_accounts_id_to_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsersId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UsersId).HasColumnName("users_id");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(60)
                .HasColumnName("password_hash");
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(60)
                .HasColumnName("password_salt");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UsersEmail)
                .HasMaxLength(255)
                .HasColumnName("users_email");
            entity.Property(e => e.UsersName)
                .HasMaxLength(255)
                .HasColumnName("users_name");
            entity.Property(e => e.UsersPhonenumber)
                .HasMaxLength(40)
                .HasColumnName("users_phonenumber");
            entity.Property(e => e.UsersPhoto).HasColumnName("users_photo");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");

            entity.HasMany(d => d.Roles).WithMany(p => p.UsersNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_roles_role_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("users_roles_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("users_roles_pkey");
                        j.ToTable("users_roles");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
