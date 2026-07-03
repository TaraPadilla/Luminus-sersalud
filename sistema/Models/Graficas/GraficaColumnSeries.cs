using System.Collections.Generic;

namespace sistema.Models.Graficas
{
    public class GraficaColumnSeries
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<GraficaColumnData> data { get; set; }
    }
}
