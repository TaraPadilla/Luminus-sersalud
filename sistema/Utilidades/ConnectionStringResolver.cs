using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace farmamest.Utilidades
{
    /// <summary>
    /// Resolves PostgreSQL connection string from appsettings.
    /// Supports legacy key names used on existing client servers.
    /// </summary>
    public static class ConnectionStringResolver
    {
        public const string PrimaryName = "farmaowl";

        private static readonly string[] FallbackNames =
        {
            "farmaowl",
            "Farmamest",
            "farmamest",
            "DefaultConnection",
            "SerSalud",
            "Connection"
        };

        public static string Resolve(IConfiguration configuration)
        {
            return ApplyConnectionDefaults(NormalizeNpgsql(ResolveRaw(configuration)));
        }

        public static string ResolveRaw(IConfiguration configuration)
        {
            return ResolveRawFromEnvironment() ?? ResolveRawFromConfiguration(configuration);
        }

        public static string ResolveRawFromEnvironment()
        {
            return Environment.GetEnvironmentVariable("ConnectionStrings__farmaowl")
                ?? Environment.GetEnvironmentVariable("FARMAOWL_CONNECTION");
        }

        public static string ResolveRawFromConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
                return null;

            foreach (var name in FallbackNames)
            {
                var cs = configuration.GetConnectionString(name);
                if (!string.IsNullOrWhiteSpace(cs))
                    return cs;
            }

            var section = configuration.GetSection("ConnectionStrings");
            if (section.Exists())
            {
                foreach (var child in section.GetChildren())
                {
                    if (!string.IsNullOrWhiteSpace(child.Value))
                        return child.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Normalizes legacy keys (User ID, Server, SslMode) to Npgsql format for EF, Hangfire, etc.
        /// </summary>
        public static string NormalizeNpgsql(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                return builder.ConnectionString;
            }
            catch
            {
                return connectionString;
            }
        }

        /// <summary>
        /// Remote PostgreSQL (DigitalOcean) often needs longer timeouts than Npgsql default (15s).
        /// </summary>
        public static string ApplyConnectionDefaults(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return connectionString;

            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                if (builder.Timeout < 60)
                    builder.Timeout = 60;
                if (builder.KeepAlive < 30)
                    builder.KeepAlive = 30;
                if (builder.MaxPoolSize > 50)
                    builder.MaxPoolSize = 50;
                builder.Pooling = true;
                return builder.ConnectionString;
            }
            catch
            {
                return connectionString;
            }
        }

        public static bool LooksLikeRemoteDatabase(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return true;

            var lower = connectionString.ToLowerInvariant();
            if (lower.Contains("127.0.0.1") || lower.Contains("localhost"))
                return false;

            if (lower.Contains("ondigitalocean.com")
                || lower.Contains("amazonaws.com")
                || lower.Contains("azure.com"))
                return true;

            if (lower.Contains("host=") && !lower.Contains("host=127.0.0.1") && !lower.Contains("host=localhost"))
                return true;

            if (lower.Contains("server=") && !lower.Contains("server=127.0.0.1") && !lower.Contains("server=localhost"))
                return true;

            return false;
        }

        /// <summary>
        /// Detects template/placeholder values (e.g. Host=... from ESTRUCTURA-SERVIDOR) that break DNS at runtime.
        /// </summary>
        public static bool IsPlaceholderConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                return true;

            if (connectionString.IndexOf("...", StringComparison.Ordinal) >= 0)
                return true;

            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                var host = builder.Host?.Trim();
                if (string.IsNullOrWhiteSpace(host) || string.Equals(host, "...", StringComparison.Ordinal))
                    return true;

                var database = builder.Database?.Trim();
                if (string.IsNullOrWhiteSpace(database) || string.Equals(database, "...", StringComparison.Ordinal))
                    return true;

                var username = builder.Username?.Trim();
                if (string.IsNullOrWhiteSpace(username) || string.Equals(username, "...", StringComparison.Ordinal))
                    return true;

                return false;
            }
            catch
            {
                return true;
            }
        }

        public static string DescribeConnectionTarget(string connectionString)
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                return $"{builder.Host}:{builder.Port}/{builder.Database}";
            }
            catch
            {
                return "(cadena invalida)";
            }
        }

        public static string ResolveName(IConfiguration configuration)
        {
            if (configuration == null)
                return PrimaryName;

            foreach (var name in FallbackNames)
            {
                var cs = configuration.GetConnectionString(name);
                if (!string.IsNullOrWhiteSpace(cs))
                    return name;
            }

            var section = configuration.GetSection("ConnectionStrings");
            if (section.Exists())
            {
                foreach (var child in section.GetChildren())
                {
                    if (!string.IsNullOrWhiteSpace(child.Value))
                        return child.Key;
                }
            }

            return PrimaryName;
        }
    }
}
