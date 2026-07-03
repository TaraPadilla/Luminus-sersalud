using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace sistema.Helpers
{
    public static class DataTablesJsonHelper
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
            // Do not omit nulls: DataTables requires every column key on each row (tn/4 otherwise).
        };

        public static string DtStr(string? value) =>
            string.IsNullOrWhiteSpace(value) ? "-" : value;

        public static IActionResult Ok(object payload) =>
            new ContentResult
            {
                Content = JsonSerializer.Serialize(payload, Options),
                ContentType = "application/json"
            };
    }
}
