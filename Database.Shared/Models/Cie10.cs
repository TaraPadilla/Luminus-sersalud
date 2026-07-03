using System.ComponentModel.DataAnnotations;
namespace Database.Shared.Models
{
    public class Cie10{

        [Key]
        public string codigo {get;set;}
        public string descripcion {get;set;}
        
    }
}