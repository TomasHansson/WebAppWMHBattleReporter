using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppWMHBattleReporter.Models;
using WebAppWMHBattleReporter.Utility;

namespace WebAppWMHBattleReporter.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }

            List<Faction> factions = await _db.Factions.ToListAsync();
            foreach (string factionName in StaticDetails.CurrentFactions)
            {
                if (!factions.Any(f => f.Name == factionName))
                {
                    Faction faction = new Faction()
                    {
                        Name = factionName
                    };
                    _db.Factions.Add(faction);
                }
            }
            await _db.SaveChangesAsync();

            factions = await _db.Factions.ToListAsync();

            List<Theme> themes = await _db.Themes.ToListAsync();
            foreach (Dictionary<string, string> factionThemes in StaticDetails.CurrentThemes)
            {
                string factionName = factionThemes.First().Value;
                int factionId = factions.First(f => f.Name == factionName).Id;
                foreach (KeyValuePair<string, string> theme in factionThemes)
                {
                    if (!themes.Any(t => t.Name == theme.Key && t.FactionId == factionId))
                    {
                        Theme newTheme = new Theme()
                        {
                            Name = theme.Key,
                            FactionId = factionId
                        };
                        _db.Themes.Add(newTheme);
                    }
                }
            }

            List<Caster> casters = await _db.Casters.ToListAsync();
            foreach (Dictionary<string, string> factionCasters in StaticDetails.CurrentCasters)
            {
                string factionName = factionCasters.First().Value;
                int factionId = factions.First(f => f.Name == factionName).Id;
                foreach (KeyValuePair<string, string> caster in factionCasters)
                {
                    if (!casters.Any(c => c.Name == caster.Key && c.FactionId == factionId))
                    {
                        Caster newCaster = new Caster()
                        {
                            Name = caster.Key,
                            FactionId = factionId
                        };
                        _db.Casters.Add(newCaster);
                    }
                }
            }

            await _db.SaveChangesAsync();

            if (!_db.Roles.Any(r => r.Name == StaticDetails.Administrator))
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Administrator)).GetAwaiter().GetResult();

            if (!_db.Roles.Any(r => r.Name == StaticDetails.EndUser))
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.EndUser)).GetAwaiter().GetResult();

            if (!_db.ApplicationUsers.Any(au => au.UserName == "Admin"))
            {
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    Region = StaticDetails.Europe
                }, "Admin123!").GetAwaiter().GetResult();

                IdentityUser user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin@admin.com");

                await _userManager.AddToRoleAsync(user, StaticDetails.Administrator);
            }
        }
    }
}
