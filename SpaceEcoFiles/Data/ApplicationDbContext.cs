using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SpaceEcoFiles.Models;

namespace SpaceEcoFiles.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SpaceEcoFiles.Models.DocType> DocType { get; set; }
        public DbSet<SpaceEcoFiles.Models.DocFormat> DocFormat { get; set; }
    }
}
