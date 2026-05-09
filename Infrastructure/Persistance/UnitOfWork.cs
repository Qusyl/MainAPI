using Application;
using Application.Interface;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context) => _context = context;

        public async Task<Result<ApplicationError>> SaveChangesAsync(CancellationToken cts = default)
        {
            try
            {
                var entites = _context.ChangeTracker.Entries<IAppEntity>()
                    .Where(e => e.State != EntityState.Unchanged)
                    .ToList();
                var events = entites.SelectMany(x => x.Entity.Events).ToList();
                foreach (var e in events) {
                    var message = new Domain.Entity.OutBoxMessage(e.EventType, JsonSerializer.Serialize(e, e.GetType()), e.Version, e.OccurredOn, default);
                    
                    await _context.OutBoxMessages.AddAsync(message);
                }
                await _context.SaveChangesAsync(cts);

                foreach(var entity in entites)
                {
                    entity.Entity.ClearEvents();
                }
                return Result<ApplicationError>.Success;
            }
            catch (DbUpdateConcurrencyException ex) {
                return Result<ApplicationError>.Failure(ApplicationError.ConcurrencyError);
            }
        }
    }
}
