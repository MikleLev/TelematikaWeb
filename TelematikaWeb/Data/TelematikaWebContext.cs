using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelematikaWEB.Models;

namespace TelematikaWeb.Data
{
    public class TelematikaWebContext : DbContext
    {
        public TelematikaWebContext (DbContextOptions<TelematikaWebContext> options)
            : base(options)
        {
        }

        public DbSet<TelematikaWEB.Models.Cartridge> Cartridge { get; set; } = default!;
    }
}
