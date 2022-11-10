
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SP.Profanity.Helpers;
using SP.Profanity.Interfaces;
using SP.Profanity.Services;

namespace SP.Profanity
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers();

            services.AddHealthChecks();

            services.AddSingleton<IAppSettings>(c => new AppSettings());
            services.AddSingleton<IMySqlSettings, MySqlSettings>();
            services.AddScoped<IWordService, WordService>();
            services.AddScoped<IFilterService, FilterService>();

            services.AddCors(options => options.AddPolicy("Cors", builder => {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));
            if(Environment.IsDevelopment())
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "SP Filtered Words API",
                        Description = "A simple API to handle word filtering in SP apps.",
                        Contact = new OpenApiContact
                        {
                            Name = "Samaritan's Purse Application Developers",
                            Email = string.Empty,
                            Url = new Uri("https://samaritanspurse.org"),
                        }
                    });
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT authorization header using the Bearer scheme. Example: \"Authorization: {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme()
                            {
                                Reference = new OpenApiReference() {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                            },
                            new[] {"readAccess", "writeAccess"}
                        }
                    });
                });
            }
            services.UseApiDashboardAuth();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseSwagger(options => {
                    options.PreSerializeFilters.Add((swagger, httpReq) => {
                        swagger.Servers.Clear();
                    });
                });
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "sp_api_profanity_filter v1"));
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors("Cors");
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
