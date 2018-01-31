﻿using AwesomeCMSCore.Extension;
using AwesomeCMSCore.Infrastructure.Config;
using AwesomeCMSCore.Infrastructure.Module.Views;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AwesomeCMSCore
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddReactJS();
            services.LoadInstalledModules(_hostingEnvironment.ContentRootPath);
            services.AddCustomizedDataStore(_configuration);
            //ModuleViewLocationExpander is used to help the view engine lookup up the right module folder the views
            services.Configure<RazorViewEngineOptions>(options => { options.ViewLocationExpanders.Add(new ModuleViewLocationExpander()); });
            services.AddCustomizedMvc(GlobalConfiguration.Modules, _configuration, _hostingEnvironment);

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.InitializeDbTestData();
            app.SetupReactJs();
            app.SetupEnv(env);
            app.UseStaticFiles();
            app.ServeStaticModuleFile(GlobalConfiguration.Modules);
            app.UseAuthentication();
            app.UseCustomizeMvc();
        }
    }
}