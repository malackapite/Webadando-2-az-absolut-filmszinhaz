using Backend.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backend.Controllers
{
    public abstract class TableController<TPrimaryKey, TDbFormat, TJsonFormat>(IDbContext context) : ControllerContext(context)
        where TDbFormat : class, IConvertible<TJsonFormat>
        where TJsonFormat : class, IConvertible<TDbFormat>
    {
        protected abstract DbSet<TDbFormat> DbSet { get; }

        [AllowAnonymous]
        public abstract Task<ActionResult<TJsonFormat>> Get([FromRoute] TPrimaryKey pk);

        [HttpGet, AllowAnonymous]
        public virtual async Task<ActionResult<IEnumerable<TJsonFormat>>> Get() => await ConvertAllToDTOAsync(DbSet);

        protected async Task<ActionResult<TJsonFormat>> PerformGetAsync(params object?[]? pk) => await CheckIfNotFoundAsync(async (TDbFormat record) => record.Convert(), pk);

        [HttpPost]
        public virtual async Task<ActionResult<TJsonFormat>> Post([FromBody] TJsonFormat data) => await CheckIfModelStateIsValidAsync(async () => await HandlePostAsync(data.Convert()));

        [HttpPut]
        public virtual async Task<ActionResult<TJsonFormat>> Put([FromBody] TJsonFormat data) => StatusCode(405);

        protected async Task<ActionResult<TJsonFormat>> PerformPutAsync(TDbFormat data, params object?[]? pk)
        {
            TDbFormat? record = await DbSet.FindAsync(pk);
            return record is null ? await HandlePostAsync(data) : await TrySaveRecord(record, (TDbFormat record) => {
                DbSet.Remove(record);
                DbSet.Add(data);
            });
        }

        protected async Task<ActionResult<TJsonFormat>> HandlePostAsync(TDbFormat record) => await TrySaveRecord(record, DbSet.Add);

        protected async Task<ActionResult<TJsonFormat>> PerformPatchAsync<TPatchDTO>(TPatchDTO data, params object?[]? pk) where TPatchDTO : IPatchDTO<TDbFormat>
            => await CheckIfNotFoundAsync(async (TDbFormat record) => await TrySaveRecord(record, data.Patch), pk)
        ;

        public abstract Task<ActionResult<TJsonFormat>> Delete([FromRoute] TPrimaryKey pk);

        [HttpDelete]
        public async Task<ActionResult<IEnumerable<TJsonFormat>>> Delete() => await HandleDbUpdateException<IEnumerable<TJsonFormat>>(async () => {
            List<TJsonFormat> records = await ConvertAllToDTOAsync(DbSet);
            await DbSet.ExecuteDeleteAsync();
            return records;
        });

        protected async Task<ActionResult<TJsonFormat>> PerformDeleteAsync(params object?[]? pk) => await CheckIfNotFoundAsync(async (TDbFormat record) => await TrySaveRecord(record, DbSet.Remove), pk);

        protected static async Task<List<TJsonFormat>> ConvertAllToDTOAsync(IQueryable<TDbFormat> records) => (await records.ToListAsync()).ConvertAll(static (TDbFormat record) => record.Convert());

        async Task<ActionResult<TJsonFormat>> TrySaveRecord(TDbFormat record, Func<TDbFormat, EntityEntry<TDbFormat>> action) => await TrySaveRecord(record, (TDbFormat record) => {
            action(record);
        });

        async Task<ActionResult<TJsonFormat>> TrySaveRecord(TDbFormat record, Action<TDbFormat> action) => await HandleDbUpdateException<TJsonFormat>(async () => {
            action(record);
            try
            {
                await context.SaveChangesAsync();
                await context.Entry(record).ReloadAsync();
                return record.Convert();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return Conflict(e.GetInnerMostExceptionMessage());
            }
        });

        protected async Task<ActionResult<TJsonFormat>> CheckIfNotFoundAsync(Func<TDbFormat, Task<ActionResult<TJsonFormat>>> handleRequest, params object?[]? pk)
        {
            TDbFormat? record = await DbSet.FindAsync(pk);
            return record is not null ? await handleRequest(record) : NotFound();
        }
    }
}
