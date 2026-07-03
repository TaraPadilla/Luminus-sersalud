using System.Collections.Generic;

namespace sistema.Models.DataTables
{
    // Request estándar de jQuery DataTables en modo server-side.
    // Se recomienda recibirlo en el controller con [FromForm] DataTablesRequest request.
    public class DataTablesRequest
    {
        public int Draw { get; set; }

        // Paging
        public int Start { get; set; }
        public int Length { get; set; }

        // Search
        public DataTablesSearch Search { get; set; } = new DataTablesSearch();

        // Order & Columns (importantes para sorting dinámico)
        public List<DataTablesOrder> Order { get; set; } = new List<DataTablesOrder>();
        public List<DataTablesColumn> Columns { get; set; } = new List<DataTablesColumn>();
    }

    public class DataTablesSearch
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }

    public class DataTablesOrder
    {
        // índice de la columna (refiere a Columns[index])
        public int Column { get; set; }

        // "asc" | "desc"
        public string Dir { get; set; }
    }

    public class DataTablesColumn
    {
        // DataTables manda "data" (nombre del campo en el cliente)
        public string Data { get; set; }

        // "name" si lo configuras en DataTables
        public string Name { get; set; }

        public bool Searchable { get; set; }
        public bool Orderable { get; set; }

        public DataTablesSearch Search { get; set; } = new DataTablesSearch();
    }
}
