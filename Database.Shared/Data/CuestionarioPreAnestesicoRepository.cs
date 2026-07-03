using Database.Shared.IRepository;
using Database.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace Database.Shared.Data
{
    public class CuestionarioPreAnestesicoRepository : ICuestionarioPreAnestesico
    {
        private readonly Context _context;

        public CuestionarioPreAnestesicoRepository(Context context)
        {
            _context = context;
        }

        public void Add(CuestionarioPreAnestesico cuestionario)
        {
            _context.CuestionariosPreAnestesicos.Add(cuestionario);
            _context.SaveChanges();
        }

        public IEnumerable<CuestionarioPreAnestesico> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _context.CuestionariosPreAnestesicos
                .Where(c => c.HospitalizacionId == hospitalizacionId)
                .OrderByDescending(c => c.FechaRegistro)
                .ToList();
        }

        public void Actualizar(CuestionarioPreAnestesico cuestionario)
        {
            var existente = _context.CuestionariosPreAnestesicos.Find(cuestionario.Id);
            if (existente == null)
                throw new System.Exception($"Cuestionario pre-anestésico con Id {cuestionario.Id} no encontrado.");

            existente.NombreCompleto          = cuestionario.NombreCompleto;
            existente.RegistroMedico          = cuestionario.RegistroMedico;
            existente.Edad                    = cuestionario.Edad;
            existente.FechaCuestionario       = cuestionario.FechaCuestionario;
            existente.Peso                    = cuestionario.Peso;
            existente.Estatura                = cuestionario.Estatura;
            existente.FechaUltimaRegla        = cuestionario.FechaUltimaRegla;
            existente.FechaProcedimiento      = cuestionario.FechaProcedimiento;
            existente.ProcedimientoProgramado = cuestionario.ProcedimientoProgramado;
            existente.Cirujano                = cuestionario.Cirujano;

            // Antecedentes
            existente.PA_Alergia           = cuestionario.PA_Alergia;
            existente.PA_AlergiaCual       = cuestionario.PA_AlergiaCual;
            existente.PA_Fuma              = cuestionario.PA_Fuma;
            existente.PA_FumaCuanto        = cuestionario.PA_FumaCuanto;
            existente.PA_Drogas            = cuestionario.PA_Drogas;
            existente.PA_DrogasCuales      = cuestionario.PA_DrogasCuales;
            existente.PA_Alcohol           = cuestionario.PA_Alcohol;
            existente.PA_AlcoholCuanto     = cuestionario.PA_AlcoholCuanto;
            existente.PA_Embarazo          = cuestionario.PA_Embarazo;
            existente.PA_Transfusion       = cuestionario.PA_Transfusion;
            existente.PA_Asma              = cuestionario.PA_Asma;
            existente.PA_Pulmones          = cuestionario.PA_Pulmones;
            existente.PA_Corazon           = cuestionario.PA_Corazon;
            existente.PA_AtaqueCardiaco    = cuestionario.PA_AtaqueCardiaco;
            existente.PA_Angina            = cuestionario.PA_Angina;
            existente.PA_Soplo             = cuestionario.PA_Soplo;
            existente.PA_Presion           = cuestionario.PA_Presion;
            existente.PA_Higado            = cuestionario.PA_Higado;
            existente.PA_Rinones           = cuestionario.PA_Rinones;
            existente.PA_Diabetes          = cuestionario.PA_Diabetes;
            existente.PA_Epilepsia         = cuestionario.PA_Epilepsia;
            existente.PA_Derrame           = cuestionario.PA_Derrame;
            existente.PA_Tiroides          = cuestionario.PA_Tiroides;
            existente.PA_Anestesico        = cuestionario.PA_Anestesico;
            existente.PA_AceptaTransfusion = cuestionario.PA_AceptaTransfusion;

            // Información adicional
            existente.AI_Medicamentos       = cuestionario.AI_Medicamentos;
            existente.AI_Actividad          = cuestionario.AI_Actividad;
            existente.AI_ActividadDetalle   = cuestionario.AI_ActividadDetalle;
            existente.AI_OperacionesPrevias = cuestionario.AI_OperacionesPrevias;
            existente.AI_Comentarios        = cuestionario.AI_Comentarios;

            _context.SaveChanges();
        }
    }
}