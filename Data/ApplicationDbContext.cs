using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _mpesaIntegration.Models;
using Microsoft.EntityFrameworkCore;

namespace _mpesaIntegration.Data
{
    public class ApplicationDbContext: DbContext
    {
        public virtual DbSet<MpesaC2B> MpesaC2Bs { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }
       
    }
}