﻿using System.Text;
using Bing.AspNetCore;
using Bing.AutoMapper;
using Bing.Core.Modularity;
using Bing.Logs.Exceptionless;
using Bing.Logs.NLog;
using Bing.Webs.Extensions;
using Bing.Webs.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Bing.Samples.Api
{
    /// <summary>
    /// 应用程序模块
    /// </summary>
    [DependsOnModule(typeof(AspNetCoreModule))]
    public class AppModule : AspNetCoreBingModule
    {
        /// <summary>
        /// 模块级别。级别越小越先启动
        /// </summary>
        public override ModuleLevel Level => ModuleLevel.Application;

        /// <summary>
        /// 添加服务。将模块服务添加到依赖注入服务容器中
        /// </summary>
        /// <param name="services">服务集合</param>
        public override IServiceCollection AddServices(IServiceCollection services)
        {
            // 注册Mvc
            services
                .AddMvc(options =>
                {
                    options.Filters.Add<ValidationModelAttribute>();
                    options.Filters.Add<ResultHandlerAttribute>();
                    options.Filters.Add<ExceptionHandlerAttribute>();
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddControllersAsServices();

            // 注册日志
            services.AddExceptionless(o =>
            {
                o.ApiKey = "ez9jumyxVxjTxqSm0oUQhCML3OGCkDfMGyW1hfmn";
                o.ServerUrl = "http://10.186.132.40:65000/";
            });

            // 注册AutoMapper
            services.AddAutoMapper();
            return services;
        }

        /// <summary>
        /// 应用AspNetCore的服务业务
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        public override void UseModule(IApplicationBuilder app)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            app.UseErrorLog();
            app.UseStaticHttpContext();
            app.UseMvc(routes =>
            {
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            Enabled = true;
        }
    }
}
