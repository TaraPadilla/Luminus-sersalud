namespace sistema.Models
{
    public class DepartamentosViewModel
    {
        public int? Id { get; set; }
        public string NombreDepartamento { get; set; }

        public string DepartamentoNombreMostrar {
            get
            {
                return $"{Id} - {NombreDepartamento}";
            }

        }


    }
}
