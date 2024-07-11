using EFcore.Domain;
using EFCore.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EFcore.Data
{
    public class FootballLeagueDBContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Match> Matches { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("")
                .UseLazyLoadingProxies()
                //doing no tracking on a global level
                //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .LogTo(Console.WriteLine, LogLevel.Information)
                 //Do NOT use thes following 2 in a production workload
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
            //with SQLlite it'll look like this (dont forget to download Microsoft.EntityFrameworkCore.Sqlite.Core
            //optionsBuilder.UseSqlite($"Data Source=FootballLeague_EfCore.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
            
    }

}
