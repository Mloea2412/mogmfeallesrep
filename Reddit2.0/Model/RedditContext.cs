using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Reddit2._0.Model;

namespace Reddit2._0.Model
{
    public class RedditContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public string DbPath { get; }

        public RedditContext(DbContextOptions<RedditContext> options)
            : base(options)
        {
            DbPath = " bin/Reddit.db";
        }

        public RedditContext()
        {
            DbPath = " bin/Reddit.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().ToTable("Post");


        }
    }
}

