using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace MmReddit.Model
{
    public class MmRedditContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public string DbPath { get; }

        public MmRedditContext(DbContextOptions<MmRedditContext> options)
            : base(options)
        {
            DbPath = "bin/MmReddit.db";
        }

        public MmRedditContext()
        {
            DbPath = "bin/MmReddit.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().ToTable("Post");


        }


    }
}

