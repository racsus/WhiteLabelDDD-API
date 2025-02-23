using System.Reflection;
using Autofac;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WhiteLabel.Application.AutoMapper;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Infrastructure.Data;
using WhiteLabel.Infrastructure.Data.Pagination;
using WhiteLabel.Infrastructure.Data.Repositories;
using WhiteLabel.Infrastructure.Events;

namespace WhiteLabel.Infrastructure.DependencyInjection
{
    public static class ContainerSetup
    {
        public static void Initialize(
            ContainerBuilder builder,
            IConfiguration configuration,
            AuthConfiguration authConfiguration
        )
        {
            // Services and Generic Repository
            var coreAssemblyApplication = Assembly.GetAssembly(typeof(IBusinessService));
            //var coreAssemblyDomain = Assembly.GetAssembly(typeof(IHandle<BaseDomainEvent>));
            if (coreAssemblyApplication != null)
            {
                builder
                    .RegisterAssemblyTypes(coreAssemblyApplication)
                    .Where(t => t.Name.EndsWith("Service"))
                    .AsImplementedInterfaces();
                builder
                    .RegisterAssemblyTypes(coreAssemblyApplication)
                    .Where(t => t.Name.EndsWith("Handle"))
                    .AsImplementedInterfaces();
            }

            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>));

            // Other Services
            builder.RegisterType<GenericUnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<DomainEventDispatcher>().As<IDomainEventDispatcher>();
            builder
                .RegisterType<SpecificationBuilder>()
                .As<ISpecificationBuilder>()
                .SingleInstance();
            builder
                .RegisterType<EfCoreQueryableEvaluator>()
                .As<IQueryableEvaluator>()
                .SingleInstance();
            builder.Register(_ => configuration).As<IConfiguration>().SingleInstance();
            builder.Register(_ => authConfiguration).As<AuthConfiguration>().SingleInstance();

            // DbContext
            builder
                .RegisterType<AppDbContext>()
                .WithParameter(
                    "options",
                    Get(configuration.GetConnectionString("DefaultConnection"))
                )
                .InstancePerLifetimeScope();

            // AutoMapper configuration 2.2
            builder.Register(_ => new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ObjectProfile());
                mc.AddProfile(new ModelProfile());
            }));
            builder
                .Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>()
                .InstancePerLifetimeScope();
        }

        private static DbContextOptions<AppDbContext> Get(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlServer(connectionString);

            return builder.Options;
        }
    }
}
