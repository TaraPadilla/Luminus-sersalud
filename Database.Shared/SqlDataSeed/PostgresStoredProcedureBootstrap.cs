using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Database.Shared.SqlDataSeed
{
    /// <summary>
    /// Ensures PostgreSQL functions used by FromSqlRaw/FromSqlInterpolated exist in the database.
    /// EF migrations do not create these functions; they are defined in <see cref="SqlQueriesContext"/>.
    /// </summary>
    public static class PostgresStoredProcedureBootstrap
    {
        private static readonly Regex DropFunctionHintRegex = new(
            @"DROP FUNCTION\s+(.+?)\s+first",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static void EnsureCreated(DbContext context, Action<Exception, string> onError = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var scripts = new SqlQueriesContext().GetStoredProcedureScripts();
            foreach (var sql in scripts)
            {
                if (!TryExecuteScript(context, sql, out var error))
                    onError?.Invoke(error, sql);
            }
        }

        private static bool TryExecuteScript(DbContext context, string sql, out Exception error)
        {
            error = null;
            try
            {
                context.Database.ExecuteSqlRaw(sql);
                return true;
            }
            catch (Exception ex) when (TryBuildDropFunctionSql(ex, out var dropSql))
            {
                try
                {
                    context.Database.ExecuteSqlRaw(dropSql);
                    context.Database.ExecuteSqlRaw(sql);
                    return true;
                }
                catch (Exception retryEx)
                {
                    error = retryEx;
                    return false;
                }
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }

        private static bool TryBuildDropFunctionSql(Exception ex, out string dropSql)
        {
            dropSql = null;
            var pgEx = FindPostgresException(ex);
            if (pgEx?.SqlState != "42P13" || string.IsNullOrWhiteSpace(pgEx.Hint))
                return false;

            var match = DropFunctionHintRegex.Match(pgEx.Hint);
            if (!match.Success)
                return false;

            dropSql = $"DROP FUNCTION IF EXISTS {match.Groups[1].Value.Trim()};";
            return true;
        }

        private static PostgresException FindPostgresException(Exception ex)
        {
            for (var current = ex; current != null; current = current.InnerException)
            {
                if (current is PostgresException pg)
                    return pg;
            }

            return null;
        }
    }
}
