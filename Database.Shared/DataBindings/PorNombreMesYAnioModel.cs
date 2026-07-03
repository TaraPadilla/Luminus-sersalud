using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Database.Shared.DataBindings
{
    public class PorNombreMesYAnioModel 
    {
        public int Year {get;set;}
        public int Month {get;set;}
        public string MonthName
        {
            get 
            {
                return CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(this.Month);
            }
        }

        public decimal Total {get;set;}
    }
}