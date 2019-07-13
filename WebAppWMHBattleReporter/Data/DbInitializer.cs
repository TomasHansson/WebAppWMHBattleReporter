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

            if (_db.Roles.Any(r => r.Name == StaticDetails.Administrator))
                return;

            _roleManager.CreateAsync(new IdentityRole(StaticDetails.Administrator)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.EndUser)).GetAwaiter().GetResult();

            if (_db.ApplicationUsers.Any(au => au.UserName == "Admin"))
                return;

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
