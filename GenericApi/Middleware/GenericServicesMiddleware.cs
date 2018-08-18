using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using System;

//https://github.com/aspnet/Entropy/tree/42171b706540d23e0298c8f16a4b44a9ae805c0a/samples/Mvc.GenericControllers
//https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts

namespace GenericApi
{
    public static class GenericServicesMiddleware
    {
        public static void AddGenericServices(this IServiceCollection services)
        {
            AddGenericServices(services, typeof(DefaultSanitizer));
        }
        public static void AddGenericServices(this IServiceCollection services, Type UseSanitizer)
        {
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddSingleton<IAuthorizationHandler, SecureGenericApiHandler>();
            services.AddTransient(typeof(IInputSanitizer), UseSanitizer);
        }

    }



}