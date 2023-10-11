using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace Creddit.Model
{
    public class CredditContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public string DbPath { get; }

        public CredditContext(DbContextOptions<CredditContext> options)
            : base(options)
        {
            DbPath = " bin/Creddit.db";
        }

        public CredditContext()
        {
            DbPath = " bin/Creddit.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().ToTable("Post");


        }
    }
}

