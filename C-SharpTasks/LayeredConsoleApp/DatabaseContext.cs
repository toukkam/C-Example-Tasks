using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace C_SharpTasks.LayeredConsoleApp
{
    // Entity Framework class that acts as a format for a table in the database.
    public class Task
    {
        [Key] // Use name as primary key to keep the task simple.
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
    }

    // Entity Framework database context used to initiate connection the the database.
    // This context is used by the functions in order to manipulate the database.
    public class DatabaseContext : DbContext
    {
        // Set Tasks as a table in the database.
        public DbSet<Task> Tasks { get; set; }

        // Function that configures the connection to the database, I use the Microsoft SQL Server 2017 with windows logon.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=DemoDB;Integrated Security=True;MultipleActiveResultSets=true");
        }
    }
}
