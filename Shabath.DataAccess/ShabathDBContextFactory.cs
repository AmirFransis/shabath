using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shabath.DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;

namespace Shabath.DataAccess
{
    public class ShabathDBContextFactory : IDesignTimeDbContextFactory<ShabathDBContext>
    {

        private static string _connectionString;

        public ShabathDBContext CreateDbContext()
        {
            return CreateDbContext(null);
        }

        public ShabathDBContext CreateDbContext(string[] args)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                LoadConnectionString();
            }

            var builder = new DbContextOptionsBuilder<ShabathDBContext>();
            builder.UseSqlServer(_connectionString);

            return new ShabathDBContext(builder.Options);
        }

        private static void LoadConnectionString()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build();

            _connectionString = configuration.GetConnectionString("ShabathDB");
        }

    }
}
