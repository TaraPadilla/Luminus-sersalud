using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using Database.Shared.Data;
using System;

namespace sistema.Models
{
    public class PacientesBaseViewModel
    {
        //Para demo:
        //utilizar si el cliente rgeistra un cobro inicial al registrar al paciente
        public bool RegistrarCobroInicial { get; set; }
        public int PacienteId { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public int? SexoId { get; set; }
        public SelectList SexoSelectListItems { get; set; }
        public int NivelEducativoId { get; set; }
        public SelectList NivelEducativoSelectListItems { get; set; }
        public int? SeguroEpssId { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string? Peso { get; set; }
        public string? NombreEncargado { get; internal set; }
        public string? DPIEncargado { get; internal set; }
        public string TipoDeSangre { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string Contrasennia { get; set; }
        public string Nit { get; set; }
        public string Dpi { get; set; }
        public string Direccion { get; set; }
        public string no_IGGS { get; set; }
        public bool esta_Afiliado { get; set; }
        public string CodigoEPS { get; set; }
        public string Religion { get; set; }
        public string Ocupacion { get; set; }
        public string EstadoCivil { get; set; }
        public string ContactoEmergencia { get; set; }
        public string NumeroContactoEmergencia { get; set; }
        public string NombreContactoEmergencia { get; set; }
        public string Nacionalidad { get; set; }
        public string PaisResidencia { get; set; }
        public string DepartamentoResidencia { get; set; }
        public string MunicipioResidencia { get; set; }
        public string LugarOrigen { get; set; }
        public string PesoAlNacer { get; set; }
        public string Talla { get; set; }
        public string CircunferenciaCefalica { get; set; }
        public string TipoParto { get; set; }
        public string ModalidadAtencion { get; set; }
        public int? TipoPacienteId { get; set; }
        public string UrlFotografiaPaciente { get; set; }
        public string UrlFirmaRegistro { get; set; }
        public string UrlFirmaPoliticas { get; set; }
        public string UrlFirmaConsentimiento { get; set; }
        public string UrlFirmaTarjetaCredito { get; set; }


        //Antecedentes personales otros
        public string AntecedentesPersonalesObservaciones { get; set; }
        public string AntecedentesPersonalesOtros { get; set; }

        //Alergias
        public string AlergiaAnestesiaLocal { get; set; }
        public string AlergiaAspirina { get; set; }
        public string AlergiaPenicilina { get; set; }
        public string AlergiaBarbiturios { get; set; }
        public string AlergiaSulfas { get; set; }
        public string AlergiaCodeina { get; set; }
        public string AlergiaMetales { get; set; }
        public string AlergiaLatex { get; set; }
        public string AlergiaYodo { get; set; }
        public string AlergiaPolen { get; set; }
        public string AlergiaAnimales { get; set; }
        public string AlergiaAlimentos { get; set; }
        public string AlergiaOtros { get; set; }
        public string AlergiaOtrosDescripcion { get; set; }



        //Informaci�n m�dica
        public string MedicaUsaLentesContacto { get; set; }
        public string MedicaUsaLentesContactoDescripcion { get; set; }
        public string MedicaArticulacionesArtificiales { get; set; }
        public DateTime? MedicaArticulacionesArtificialesFecha { get; set; }
        public string MedicaArticulacionesArtificialesComplicaciones { get; set; }
        public string MedicaTomaAlendronato { get; set; }
        public DateTime? MedicaTomaAlendronatoFecha { get; set; }
        public string MedicaTratamientoDolorHuesos { get; set; }
        public DateTime? MedicaTratamientoDolorHuesosFechaInicio { get; set; }
        public string MedicaTratamientoDolorHuesosDescripcionCaso { get; set; }
        public string MedicaSustanciasReguladorasDrogas { get; set; }
        public DateTime? MedicaSustanciasReguladorasDrogasFecha { get; set; }
        public string MedicaUsaTabaco { get; set; }
        public string MedicaBebidasAlcoholicas { get; set; }
        public string MedicaBebidasAlcoholicasDescripcion { get; set; }


        //Informaci�n dental
        public string DentalSangradoCepillar { get; set; }
        public string DentalDolorFrio { get; set; }
        public string DentalDolorPresionar { get; set; }
        public string DentalObjetosAtorados { get; set; }
        public string DentalBocaSeca { get; set; }
        public string DentalTratamientoPeriondal { get; set; }
        public string DentalTratamientoOrtodoncia { get; set; }
        public string DentalProblemasTratamientoDental { get; set; }
        public string DentalProblemasTratamientoDentalDescripcion { get; set; }
        public string DentalFluoradaAguaDomicilio { get; set; }
        public string DentalBebeAguaFiltrada { get; set; }
        public string DentalDolorOidos { get; set; }
        public string DentalMolestiaRuidoAlto { get; set; }
        public string DentalMolestiaRuidoAltoDescripcion { get; set; }
        public string DentalBrumismo { get; set; }
        public string DentalLesiones { get; set; }
        public string DentalLesionesDescripcion { get; set; }
        public string DentalDentaduraPlacas { get; set; }
        public string DentalDentaduraPlacasDescripcion { get; set; }
        public string DentalActividadesRecreacion { get; set; }
        public string DentalActividadesRecreacionDescripcion { get; set; }
        public string DentalLesionesCabeza { get; set; }
        public string DentalLesionesCabezaDescripcion { get; set; }

        //An�lisis facial
        public string FacialPatron { get; set; }
        public string FacialPatronObservaciones { get; set; }
        public string FacialPerfil { get; set; }
        public string FacialPerfilObservaciones { get; set; }
        public string FacialAsimetria { get; set; }
        public string FacialAsimetriaObservaciones { get; set; }
        public string FacialAlturaFacialEquilibrada { get; set; }
        public string FacialAlturaFacialEquilibradaObservaciones { get; set; }
        public string FacialAnchoFacialEquilibrada { get; set; }
        public string FacialAnchoFacialEquilibradaObservaciones { get; set; }
        public string FacialPerfilMaxilar { get; set; }
        public string FacialPerfilMaxilarObservaciones { get; set; }
        public string FacialPerfilMandibular { get; set; }
        public string FacialPerfilMandibularObservaciones { get; set; }
        public string FacialSurcoLabialMenton { get; set; }
        public string FacialSurcoLabialMentonObservaciones { get; set; }
        public string FacialLabiosReposo { get; set; }

        //�n�lisis funcional
        public string FuncionalActividadComisurial { get; set; }
        public string FuncionalActividadLingual { get; set; }
        public string FuncionalLabioSuperior { get; set; }
        public string FuncionalLabioInferior { get; set; }
        public string FuncionalMasetero { get; set; }
        public string FuncionalMentoniano { get; set; }
        public string FuncionalRespiracion { get; set; }
        public string FuncionalDeglucion { get; set; }

        //Patr�n facial
        public string PatronFacial { get; set; }
        public string CaracteristicaPatronFacial { get; set; }

        //Medico
        public string AntecedentesMedicos { get; set; }
        public string AntecedentesQuirurgicos { get; set; }
        public string AntecedentesTraumaticos { get; set; }
        public string AntecedentesAlergias { get; set; }
        public string AntecedentesVicios { get; set; }
        public string AntecedentesMedicamentos { get; set; }

        //Pedi�tricos
        public string NombrePadre { get; set; }
        public string NombreMadre { get; set; }
        public DateTime? MadreFechaNacimiento { get; set; }
        public int? MadreEmbarazos { get; set; }
        public int? MadrePartosNormales { get; set; }
        public int? MadreCesareas { get; set; }
        public int? MadreAbortos { get; set; }
        public int? MadreHijosVivos { get; set; }
        public int? MadreHijosMuertos { get; set; }
        public string MadreComplicaciones { get; set; }

        //Historia
        public string HistoriaMedicoPersonal { get; set; }
        public string HistoriaObservaciones { get; set; }
        public bool HistoriaProblemaEmocional { get; set; }
        public string HistoriaAlergiaOtros { get; set; }
        public bool HistoriaAlergiaComida { get; set; }
        public bool HistoriaAlergiaMedicina { get; set; }
        public bool HistoriaOperado { get; set; }
        public bool HistoriaHospitalizado { get; set; }
        public bool HistoriaSangraMuchoCortarse { get; set; }
        public bool HistoriaTratamientoMedico { get; set; }
        public string HistoriaEspecialidadMedico { get; set; }
        public string HistoriaTelefonoMedico { get; set; }


        //Datos de IGSS
        public string IgssTipoAfiliacion { get; set; }
        public string IgssNumeroAfiliacion { get; set; }
        public int? IgssCantidadDependientes { get; set; }
        public string IgssParentescoDependientes { get; set; }

        //Politicas de pago
        public bool PoliticasPagoAceptaTerminos { get; set; }

        //Datos de pago
        public string NumeroTarjetaCredito { get; set; }


        //Ginecologicos
        public string CicloMenstGine { get; set; }
        public string ETSGine { get; set; }
        public int? VIHGine { get; set; }
        public string GrupoFactorGine { get; set; }
        public string TorchGine { get; set; }
        public string InicioVidaSexualGine { get; set; }
        public int? ParejasSexGine { get; set; }
        public string ObesidadGine { get; set; }
        public string DesnutricionGine { get; set; }
        public string QGine { get; set; }
        public string PGine { get; set; }
        public string ABGine { get; set; }
        public string CGine { get; set; }
        public string FURGine { get; set; }
        public int? MuerteNeoGine { get; set; }
        public string FPPGine { get; set; }
        public string HVGine { get; set; }
        public string MuerteGine { get; set; }
        public bool? ControlPrenatalGine { get; set; }
        public string ComadronaGine { get; set; }
        public int? NoControlesGine { get; set; }

        // Campos Mama
        public string AbdomenObstetricoGine { get; set; }
        public string UteroGravioGine { get; set; }
        public string FCBGine { get; set; }
        public string AUGine { get; set; }
        public string PresentacionLeopoldGine { get; set; }
        public string OtrasGine { get; set; }
        public string ActividadUterinaGine { get; set; }
        public string MovimientoFetalPercetibleGine { get; set; }
        public string EspecifiqueGine { get; set; }
        public string TactoVaginalGine { get; set; }
        public string DGine { get; set; }
        public string CMSGine { get; set; }
        public string BPorcientoGine { get; set; }
        public string AltiutudGine { get; set; }
        public string VariedadPosicionGine { get; set; }
        public string MembranasOvularesGine { get; set; }
        public string LiquidoAmnioticoGine { get; set; }
        public string Especifique2Gine { get; set; }
        public string PelvisGine { get; set; }

        ////Datos de origen y Etnia de un Paciente
        public string EtniaPaciente { get; set; }
        public string OrigenPaciente { get; set; }


        public SelectList TipoPacienteSelectListItems { get; set; }
        public SelectList SeguroEpssSelectListItems { get; set; }
        public SelectList ModalidadAtencionSelectListItems { get; set; }
        public List<TipoPatologia> TiposPatologias { get; set; }
        public List<PatologiaPacienteViewModel> PatologiasPacienteViewModel { get; set; }
        public List<AntecedentePersonalPacienteViewModel> AntecedentesPersonalesViewModel { get; set; }
        public List<Vacuna> Vacunas { get; set; }
        public List<VacunaPacienteViewModel> VacunasPacienteViewModel { get; set; }
        //Alergias raras paciente
        public List<AlergiaRara> AlergiasRaras { get; set; }
        public List<AlergiaRaraPacienteViewModel> AlergiasRarasPacientesViewModel { get; set; }
        public List<PersonaSeguro> PersonaSeguroViewModel { get; set; }
        public List<PreguntaRegistroPacienteViewModel> PreguntasRegistroPacienteViewModel { get; set; }
        public List<PersonasSeguroPacienteViewModel> BeneficiariosEpssPacientesViewModel { get; set; }
        public List<ArchivoPacienteViewModel> ArchivosSubidos { get; set; }


        public void Init(IPacientes pacientesRepository)
        {
            TipoPacienteSelectListItems = new SelectList(pacientesRepository.GetTiposPaciente(), "Id", "NombreTipo");
            SexoSelectListItems = new SelectList(pacientesRepository.GetSexosList(), "Id", "DescripcionSexo");
            SeguroEpssSelectListItems = new SelectList(pacientesRepository.GetSegurosEpssList(), "Id", "Nombre");

            //Modalidad de atenci�n
            var modalidades = new List<ModalidadAtencionViewModel>();
            modalidades.Add(new ModalidadAtencionViewModel { Value = "presencial", NombreModalidad = "Presencial" });
            modalidades.Add(new ModalidadAtencionViewModel { Value = "virtual", NombreModalidad = "Virtual" });
            ModalidadAtencionSelectListItems = new SelectList(modalidades, "Value", "NombreModalidad");
        }

    }
    public class ModalidadAtencionViewModel
    {
        public string Value { get; set; }
        public string NombreModalidad { get; set; }
    }
    public class PatologiaPacienteViewModel
    {
        public int? Id { get; set; }
        public int TipoPatologiaId { get; set; }
        public string TipoPatologia { get; set; }
        public bool TipoPatologiaVerInputDescripcion { get; set; }
        public bool Madre { get; set; }
        public bool AbuelaMaterna { get; set; }
        public bool AbueloMaterno { get; set; }
        public bool OtrosMaterno { get; set; }
        public bool Padre { get; set; }
        public bool AbuelaPaterna { get; set; }
        public bool AbueloPaterno { get; set; }
        public bool Hermanos { get; set; }
        public bool OtrosPaterno { get; set; }
        public string DescripcionOtraPatologia { get; set; }
    }
    public class AntecedentePersonalPacienteViewModel
    {
        public int? Id { get; set; }
        public int AntecedenteId { get; set; }
        public string NombreAntecedente { get; set; }
        public bool PresentoAntecedente { get; set; }
        public DateTime? FechaAntecedente { get; set; }
    }
    public class PreguntaRegistroPacienteViewModel
    {
        public int? Id { get; set; }
        public int PreguntaId { get; set; }
        public string Pregunta { get; set; }
        public string Respuesta { get; set; }
    }
    public class VacunaPacienteViewModel
    {
        public int? Id { get; set; }
        public int VacunaId { get; set; }
        public string NombreVacuna { get; set; }
        public string NombrePaciente { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool Primera { get; set; }
        public bool Segunda { get; set; }
        public bool Tercera { get; set; }
        public bool PrimerRefuerzo { get; set; }
        public bool SegundoRefuerzo { get; set; }
        public DateTime? FechaPrimera { get; set; }
        public DateTime? FechaSegunda { get; set; }
        public DateTime? FechaTercera { get; set; }
        public DateTime? FechaPrimerRefuerzo { get; set; }
        public DateTime? FechaSegundoRefuerzo { get; set; }
    }

    public class AgregarAlergiaViewModel
    {

        public int PacienteId { get; set; }
        public int Item { get; set; }
        public string NombreAlergia { get; set; }

        public string Estado { get; set; }
    }

    //Creacion view Model de vacunaRaraPaciente
    public class AlergiaRaraPacienteViewModel
    {
        public int? Id { get; set; }

        public int AlergiaRaraId { get; set; }
        public string NombreAlergia { get; set; }

        public string Estado { get; set; }

    }
    public class PersonasSeguroPacienteViewModel
    {
        public int? Id { get; set; }
        public int? PacienteId { get; set; }
        public string Name { get; set; }
        public string Nit { get; set; }
        public string Tipo { get; set; }
    }
    public class ArchivoPacienteViewModel
    {
        public string NombreArchivo { get; set; }
        public string UrlArchivo { get; set; }
    }
}