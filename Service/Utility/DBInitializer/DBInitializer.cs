using DataAccess.Data;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Utility.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitializer(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initilize()
        {
            try
            {

                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (_roleManager.Roles.IsNullOrEmpty())
                {
                    _roleManager.CreateAsync(new(SD.SuperAdmin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.CompanyAdmin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Instructor)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SD.Employee)).GetAwaiter().GetResult();
                }

                _userManager.CreateAsync(new()
                {
                    Email = "Admin@eraasoft.com",
                    UserName = "Admin",
                }, "Admin123$").GetAwaiter().GetResult();

                var user = _userManager.FindByEmailAsync("Admin@eraasoft.com").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, SD.SuperAdmin).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
