using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SymbolContext : DbContext
    {

        public DbSet<Type> type { get; set; }

        public String DbPath { get; }

        public SymbolContext(String path)
        {
            //DbPath = "C:\\Users\\Milan\\Downloads\\database.s3db";
            DbPath = path;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={DbPath}");

    }
}
