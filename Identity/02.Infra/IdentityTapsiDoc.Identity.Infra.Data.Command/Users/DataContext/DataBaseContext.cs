using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Infra.Data.Command.Users.DataContext
{
    public class DataBaseContext : IdentityDbContext<User, Role, string>
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => new { p.ProviderKey, p.LoginProvider });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.LoginProvider });



            modelBuilder.Entity<User>().Ignore(p => p.Email);
            modelBuilder.Entity<User>().Ignore(p => p.EmailConfirmed);
            modelBuilder.Entity<User>().Ignore(p => p.NormalizedEmail);
            modelBuilder.Entity<User>().Ignore(p => p.PhoneNumberConfirmed);
            modelBuilder.Entity<User>().Ignore(p => p.TwoFactorEnabled);
            modelBuilder.Entity<User>().Ignore(p => p.AccessFailedCount);
        }
    }
}
