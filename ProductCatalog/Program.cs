using ProductCatalog.Database;
using ProductCatalog.Repositories;
using Microsoft.EntityFrameworkCore;
using ServiceBus;

namespace ProductCatalog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<ProductDbContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING"));
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("cors", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
            builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
            {
                options.Audience = "api://2e8d6e7a-74a7-4501-941f-d19fe3a67c70";
                options.Authority = "https://login.microsoftonline.com/a196b8f9-5d27-489e-9107-c3a43d880930";
            });
            
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IMessageBus, MessageBus>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseCors("cors");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();
        }
    }
}