
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using USDemographicsAPI.Core.DomainModels;
using USDemographicsAPI.Core.Interfaces.IRepos;
using USDemographicsAPI.Core.Interfaces.IServices;
using USDemographicsAPI.Data;
using USDemographicsAPI.Services;

namespace USDemographicsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();
            builder.Logging.AddConsole();
            builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHostedService<BackgroundPopulationDataFetcherService>();

            builder.Services.AddScoped<IRepository<State>, Repository<State>>();
            builder.Services.AddScoped<IRepository<County>, Repository<County>>();

            builder.Services.AddScoped<IEsriAPIService, EsriAPIService>();
            builder.Services.AddScoped<ICountyService, CountyService>();
            builder.Services.AddScoped<IStateService, StateService>();

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true; 
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("v"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("v")
                );
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(); 
            }
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseDeveloperExceptionPage();

            app.Run();
            Console.WriteLine("hello");
        }
    }
}
