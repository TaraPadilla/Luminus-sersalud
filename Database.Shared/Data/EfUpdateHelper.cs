using System;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.Data
{
    internal static class EfUpdateHelper
    {
        /// <summary>
        /// Saves a tracked entity, or merges scalar values into the DB row when detached.
        /// Throws a clear error instead of DbUpdateConcurrencyException when Id is missing or row not found.
        /// </summary>
        public static void UpdateEntity<T>(Context context, T entity, bool saveChanges = true) where T : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                if (saveChanges)
                    context.SaveChanges();
                return;
            }

            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null || idProperty.PropertyType != typeof(int))
                throw new InvalidOperationException($"{typeof(T).Name} must expose an int Id property.");

            var id = (int)idProperty.GetValue(entity)!;
            if (id <= 0)
                throw new InvalidOperationException($"{typeof(T).Name} Id inválido. Verifique que el formulario envíe el identificador.");

            var existing = context.Set<T>().Find(id);
            if (existing == null)
                throw new InvalidOperationException($"{typeof(T).Name} con Id {id} no fue encontrado.");

            context.Entry(existing).CurrentValues.SetValues(entity);

            if (saveChanges)
                context.SaveChanges();
        }
    }
}
