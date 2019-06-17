using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppWMHBattleReporter.Models;

namespace WebAppWMHBattleReporter.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Faction> Factions { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Caster> Casters { get; set; }
        public DbSet<BattleReport> BattleReports { get; set; }
    }
}
