using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Infrastructure.Enums;
using ETicaretAPI.Infrastructure.Services;
using ETicaretAPI.Infrastructure.Services.Storage;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Infrastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IStorageService, StorageService>();
            serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
            serviceCollection.AddScoped<IMailService, MailService>();
        }
        public static void AddStorage<T>(this IServiceCollection servicesCollection) where T : Storage, IStorage
        {
            servicesCollection.AddScoped<IStorage, T>();
        }
        public static void AddStorage(this IServiceCollection servicesCollection, StorageType storageType)
        {
            switch (storageType)
            {
                case StorageType.Local:
                    servicesCollection.AddScoped<IStorage, LocalStorage>();
                    break;
                case StorageType.Azure:
                    servicesCollection.AddScoped<IStorage, AzureStorage>();
                    break;
                case StorageType.AWS:
                    //servicesCollection.AddScoped<IStorage, T>();
                    break;
                default:
                    servicesCollection.AddScoped<IStorage, LocalStorage>();
                    break;
            }
            
        }
    }
}
