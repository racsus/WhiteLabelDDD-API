using Autofac;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection;
using WhiteLabel.Application.AutoMapper;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Infrastructure.Data;
using WhiteLabel.Infrastructure.Data.Repositories;
using WhiteLabel.Infrastructure.Events;

namespace WhiteLabel.Infrastructure.DependencyInjection
{
	public static class ContainerSetup
	{

        public static void Initialize(ContainerBuilder builder, IConfiguration configuration, AuthConfiguration authConfiguration)
		{
            // Services and Generic Repository
            var coreAssembly = Assembly.GetAssembly(typeof(IBusinessService));
            builder.RegisterAssemblyTypes(coreAssembly).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>));

            // Other Services
            builder.RegisterType<GenericUnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<DomainEventDispatcher>().As<IDomainEventDispatcher>();
            builder.RegisterType<SpecificationBuilder>().As<ISpecificationBuilder>().SingleInstance();
            builder.RegisterType<EfCoreQueryableEvaluator>().As<IQueryableEvaluator>().SingleInstance();
            builder.Register(x => { return configuration; }).As<IConfiguration>().SingleInstance();
            builder.Register(x => { return authConfiguration; }).As<AuthConfiguration>().SingleInstance();


            // DbContext
            builder.RegisterType<AppDbContext>()
                .WithParameter("options", Get(configuration.GetConnectionString("DefaultConnection")))
                .InstancePerLifetimeScope();


            // AutoMapper configuration 2.2
            builder.Register(ctx => new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ObjectProfile());
                mc.AddProfile(new ModelProfile());
            }));
            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>().InstancePerLifetimeScope();
        }

        public static DbContextOptions<AppDbContext> Get(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlServer(connectionString);

            return builder.Options;
        }
    }
}
