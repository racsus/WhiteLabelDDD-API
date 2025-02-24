﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Users;
using WhiteLabel.Infrastructure.Data.DbMapping.Users;
using WhiteLabel.Infrastructure.Events;

namespace WhiteLabel.Infrastructure.Data
{
    public sealed class AppDbContext : DbContext, IAppDbContext
    {
        private readonly IDomainEventDispatcher dispatcher;

        public AppDbContext(DbContextOptions options, IDomainEventDispatcher dispatcher)
            : base(options)
        {
            DataBase = Database;
            this.dispatcher = dispatcher;
        }

        //Users
        public DbSet<User> Users { get; set; }

        public DatabaseFacade DataBase { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Users
            modelBuilder.MapUser();
        }

        public override int SaveChanges()
        {
            var result = base.SaveChanges();

            // ignore events if no dispatcher provided
            if (dispatcher == null)
                return result;

            ProcessEvents();

            return result;
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = new()
        )
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            // ignore events if no dispatcher provided
            if (dispatcher == null)
                return result;

            ProcessEvents();

            return result;
        }

        private void ProcessEvents()
        {
            // dispatch events only if save was successful
            var entitiesWithEvents = ChangeTracker
                .Entries<BaseEntityWithEvents>()
                .Select(e => e.Entity)
                .Where(e => e.Events.Any())
                .ToArray();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.Events.ToArray();
                entity.Events.Clear();
                foreach (var domainEvent in events)
                    dispatcher.Dispatch(domainEvent);
            }
        }
    }
}
