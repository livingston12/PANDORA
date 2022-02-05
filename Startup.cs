using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Pandora.Configurations;
using Pandora.Core.Attributes;
using Pandora.Core.Interfaces;
using Pandora.Core.Migrations;
using Pandora.Managers;
using Pandora.Services;

namespace Pandora
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
                      {
                          options.UseGeneralRoutePrefix(Configuration.GetSection("Api:context").Value
                              + "/api/");
                          options.Filters.Add(new ApiExceptionFilterAttribute());
                      }).AddNewtonsoftJson(options =>
                      {
                          options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                          options.SerializerSettings.Converters.Add(new StringEnumConverter());
                      });
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin",
                    builder =>
                        builder.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost", "http://localhost:8080")

                );
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();

            services.AddDbContextPool<PandoraDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("PandoraDB"),
               sqlOptions => sqlOptions.CommandTimeout(60))
               .AddInterceptors(new AppInterceptor())
           );

            services.AddTransient<ITableService, TableService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IRestaurantService, RestaurantService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IRoomService, RoomService>();
            services.AddTransient<IDishService, DishService>();
            services.AddTransient<IFileManager, FileManager>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IIngredientService, IngredientService>();
            services.AddTransient<ICategoriesService, CategoryService>();
            services.AddTransient<IInvoiceService, InvoceService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("AllowAllOrigin");
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
            });
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
