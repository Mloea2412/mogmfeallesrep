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


        public void AddSampleData()
        {
            // Opret en bruger
            var user = new User
            {
                Username = "BrugerNavn"
            };

            // Tilføj brugeren til DbContext og gem ændringer i databasen
            Users.Add(user);
            SaveChanges();

            // Opret en post og tilknyt den til brugeren
            var post = new Post
            {
                Title = "Titel for post",
                Content = "Indhold for post",
                User = user // Tilknyt posten til brugeren
            };

            Posts.Add(post);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().ToTable("Post");


        }


    }
}

