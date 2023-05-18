using Microsoft.EntityFrameworkCore;
using MvcWebPage.MLAVID;
using MvcWebPage.Models;

namespace MvcWebPage.Data
{
    public class MLAVID_DB : MLAVIDContext
    {
        //public DB ()
        //{
        //    var builder = new ConfigurationBuilder();
        //    builder.AddJsonFile("appsettings.json");
        //    var configuration = builder.Build();

        //    var contextOptions = new DbContextOptionsBuilder<MLAVIDContext>()
        //        .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        //        .Options;
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        public MLAVID_DB() : base(new DbContextOptions<MLAVIDContext>())
        {

        }
        public MLAVID_DB(DbContextOptions<MLAVIDContext> options) : base(options)
        {
        }


    }
}
