
using PFDB.Logging;
using PFDB.SQLite;
using PFDB.WeaponStructure;
using PFDB.WeaponUtility;
using Frontend.Server.Controllers;
using Microsoft.AspNetCore.Builder;

namespace Frontend.Server
{
    public class Program
    {


		public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

			PFDBLogger logger = new PFDBLogger(".pfdblog");
            if (WeaponTable.InitializeEverything().success == false)
            {
                PFDBLogger.LogFatal("Quitting application.");
                return;
            }
			
			foreach (PhantomForcesVersion version in WeaponTable.ListOfVersions)
			{
				PFDBController.phantomForcesData.Add(version, new PhantomForcesDataModel(PhantomForcesDataModel.GetWeaponCollection(version)));
			}
			var app = builder.Build();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseSwagger();
                //app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

			app.MapControllerRoute("PFDBTest", "{controller=PFDB}");

            app.MapFallbackToFile("/index.html");


			app.Run();
        }
    }
}
