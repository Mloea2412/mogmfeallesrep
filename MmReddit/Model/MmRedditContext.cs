using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MmReddit.Model;

namespace MmReddit.Model
{
    public class MmRedditContext : DbContext
    {
        // DbSet-egenskaber, der repræsenterer tabeller i databasen
        public DbSet<User> Users { get; set; } // Tabel for brugere
        public DbSet<Post> Posts { get; set; } // Tabel for opslag

        public string DbPath { get; } // Sti til SQLite-databasefilen

        // Konstruktør med DbContextOptions som parameter
        public MmRedditContext(DbContextOptions<MmRedditContext> options)
            : base(options)
        {
            // Initialiserer databasens sti (databasefilens placering)
            DbPath = "bin/MmReddit.db";
        }

        // Standardkonstruktør (uden parametre)
        public MmRedditContext()
        {
            // Initialiserer databasens sti (databasefilens placering)
            DbPath = "bin/MmReddit.db";
        }

        // Metode til konfiguration af DbContext-options
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Angiver databasens forbindelsesstreng ved brug af SQLite
            options.UseSqlite($"Data Source={DbPath}");
        }

        // Metode til konfiguration af modelrelationer og tabeller
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Angiver, at entiteten 'Post' skal tilknyttes tabellen 'Post' i databasen.
            modelBuilder.Entity<Post>().ToTable("Post");
        }
    }
}
