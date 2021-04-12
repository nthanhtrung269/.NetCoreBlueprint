using Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Models;
using Blueprint.CleanArchitectureAndCQRSDesignPattern.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.IntegrationTestings.Controllers
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var dbContext = new DBContext(serviceProvider.GetRequiredService<DbContextOptions<DBContext>>(), null, null))
            {
                if (dbContext.BlueprintSetting.Any())
                {
                    return;
                }

                PopulateTestData(dbContext);
            }
        }
        public static void PopulateTestData(DBContext dbContext)
        {
            foreach (var item in dbContext.BlueprintSetting)
            {
                dbContext.Remove(item);
            }

            dbContext.SaveChanges();
            dbContext.BlueprintSetting.Add(new BlueprintSetting() { SettingName = "IsLoggingDatabase", SettingValue = "true" });

            dbContext.SaveChanges();
        }
    }
}
