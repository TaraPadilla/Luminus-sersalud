var HospitalizarDetallesVM = function () {
  let modelPagarFactura = {};
  var modelReceta = {};

  var self = this;

  self.quillEditor = null;


  // --- NUEVOS OBSERVABLES PARA HONORARIOS ---
  self.honorariosLista = ko.observableArray([]);
  self.totalHonorarios = ko.computed(function () {
    let total = 0;
    self.honorariosLista().forEach(item => {
      total += parseFloat(item.Monto || 0);
    });
    return total;
  });

  self.detalleTotales = ko.observableArray([]); // [{ Orden, Categoria, Monto, MontoTexto }]
  self.totalPagar = ko.observable();
  self.habitacionesDisponiblesCambio = ko.observableArray();
  //Registros hospitalización
  self.registrosHospitalizacion = ko.observableArray();
  self.historialCambiosHabitacion = ko.observableArray();

  self.valorTotalRegistrosHospitalizacion = ko.observable(0);
  self.valorTotalRegistrosHospitalizacionHistorial = ko.observable(0);

  // Observaciones Productos Hospitalizacion
  self.observacionesProductsHospiLista = ko.observableArray();
  self.observacionesProductsHospiId = ko.observable();
  self.observacionesProductsHospi = ko.observable("");
  self.observacionesProductsHospiUsuarioCrea = ko.observable();

  // self.descripcionOrdenMedica = ko.observable("");

  self.listaAmbulancias = ko.observableArray([]);

  self.observacionesProductsHospiAplicacionId = ko.observable();
  self.observacionesProductsHospiFecha = ko.observable();
  self.isUpdateObservacionProductsHospi = ko.observable(false);
  //Paquetes hospitalizacion
  self.paquetesExistentes = ko.observableArray();
  self.paquetesHospitalizacion = ko.observableArray();
  self.valorTotalPaquetes = ko.observable(0);

  //Nota enfermeria
  // Observables para el turno de enfermeria
  self.turnosEnfermeria = ko.observableArray();
  self.turnoFirmado = ko.observable();
  self.idTurnoEnfermeria = ko.observable();
  // Observables para la nota de enfermería
  self.notaEvolucion = ko.observable();
  self.notaSintomas = ko.observable();
  self.notaDiagnostico = ko.observable();
  self.notaFechaRegistro = ko.observable(new Date().toISOString());
  self.notaUserId = ko.observable($("#UserId").val());
  self.notasEnfermerias = ko.observableArray();

  // Observables para la nota de evolucion -- NoEvo = Nota Evolucion
  self.notaEvolucionNoEvo = ko.observable();
  self.notaSintomasNoEvo = ko.observable();
  self.notaDiagnosticoNoEvo = ko.observable();
  self.notaFechaRegistroNoEvo = ko.observable(new Date().toISOString());
  self.notaUserIdNoEvo = ko.observable($("#UserId").val());
  self.notasEvolucion = ko.observableArray();

  //Control glucometria
  self.controlFechaRegistro = ko.observable(new Date().toISOString());
  self.controlGMT = ko.observable();
  self.controlInsulina = ko.observable();
  self.controlUnidad = ko.observable();
  self.controlMedicamento = ko.observable();
  self.controlDosis = ko.observable(1);
  self.controlFirma = ko.observable();
  self.controlPersonaAplica = ko.observable();
  self.controlProfesional = ko.observable();
  self.controlesGlucometria = ko.observableArray();

  //Ingesta excreta
  // Propiedades para Ingesta/Excreta
  self.IngestasExcretas = ko.observableArray();

  self.ingestaIV = ko.observable();
  self.ingestaIV2 = ko.observable();
  self.ingestaIV3 = ko.observable();
  self.ingestaIV4 = ko.observable();
  self.ingestaIV5 = ko.observable();
  self.ingestaIV6 = ko.observable();
  self.ingestaPO = ko.observable();
  self.totalIngesta = ko.observable();
  self.excreta = ko.observable();
  self.orina = ko.observable();
  self.heces = ko.observable();
  self.vomito = ko.observable();
  self.sudoracion = ko.observable();
  self.drenajes = ko.observable();
  self.otrosLiquidos = ko.observable();
  self.cuantasHoras = ko.observable();
  self.ingestaExcretaFechaRegistro = ko.observable(new Date().toISOString());

  self.idIngestaExcretaForInfo = ko.observable();
  self.existInfoIngesta = ko.observable(false);
  self.infoIngestas = ko.observableArray();
  self.infoIngestaIV1 = ko.observable();
  self.infoIngestaIV2 = ko.observable();
  self.infoIngestaIV3 = ko.observable();
  self.infoIngestaIV4 = ko.observable();
  self.infoIngestaIV5 = ko.observable();
  self.infoIngestaIV6 = ko.observable();
  self.infoIngestaPO = ko.observable();

  self.paqueteAgregarSeleccionado = ko.observable();
  //Servicios
  self.serviciosExistentes = ko.observableArray();
  self.serviciosHospitalizacion = ko.observableArray();
  self.valorTotalServicios = ko.observable(0);
  self.servicioAgregarSeleccionado = ko.observable();
  self.cantidadServicioAgregar = ko.observable(0);
  self.valoresServicioAgregar = ko.observableArray(); // Lista de precios
  self.valorServicioAgregar = ko.observableArray(); // Lista de precios
  self.valorServicioSeleccionado = ko.observable(); // Precio seleccionado
  //Medicamentos
  self.registrosInventario = ko.observableArray();
  self.medicamentosExistentes = ko.observableArray();
  self.medicamentosHospitalizacion = ko.observableArray();
  self.valorTotalMedicamentos = ko.observable(0);
  self.medicamentoAgregarSeleccionado = ko.observable();
  self.cantidadMedicamentoAgregar = ko.observable(0);
  self.indicacionesMedicamentoAgregar = ko.observable();
  self.viaAdministracion = ko.observable();
  self.frecuenciaAdministracion = ko.observable();
  self.valorMedicamentoAgregar = ko.observable();
  self.unidadesVentaProducto = ko.observableArray();
  self.unidadVentaSeleccionadaProducto = ko.observable();
  self.valoresMedicamentoAgregar = ko.observableArray(); // Lista de precios
  self.valorMedicamentoSeleccionado = ko.observable(); // Precio seleccionado
  self.switchMedicamentosEquipos = ko.observable("medicamentos");

  //Examenes de laboratorio
  self.examenesExistentes = ko.observableArray();
  self.examenesHospitalizacion = ko.observableArray();
  self.valorTotalExamenes = ko.observable(0);
  self.examenAgregarSeleccionado = ko.observable();
  self.valorExamenAgregar = ko.observable();

  self.valoresExamenAgregar = ko.observableArray(); // Lista de precios
  self.valorExamenSeleccionado = ko.observable(); // Precio seleccionado

  // ============================================================
  // Control de carga: evita doble ejecución y llamadas duplicadas
  // ============================================================
  self.examenesComplementariosCargando = false;
  self.examenesComplementariosCargados = false;

  // Totalizador de exámenes complementarios (Laboratorio)
  self.totalExamenesComplementarios = ko.observable(0);
  self.totalExamenesComplementariosTexto = ko.observable("Q. 0.00");

  //Depositos
  self.depositosHospitalizacion = ko.observable();
  self.valorDepositoAgregar = ko.observable();
  self.valorTotalDepositosHospitalizacion = ko.observable(0);

  self.valorTotalAmbulancias = ko.observable(0);

  //Productos aplicacion
  self.productosAplicacionHospitalizacion = ko.observableArray();
  self.listaProductosAplicacion = ko.observableArray();
  //Evolucion del paciente
  self.examenesFisicosHospitalizacion = ko.observableArray();
  self.datosExamenFisicoAgregar = ko.observableArray();
  self.observacionesExamenFisicoAgregar = ko.observable();

  //Archivos
  self.archivosPaciente = ko.observableArray();
  self.archivosAutorizaciones = ko.observableArray([]);

  //Historial de consultas
  self.historialConsultasPaciente = ko.observableArray();

  //Historial de hospitalizaciones
  self.historialHospitalizacionesPaciente = ko.observableArray();

  //Receta
  self.listaRecetas = ko.observableArray();
  self.recetaIdAgregar = ko.observable();
  self.nombreRecetaAgregar = ko.observable();
  self.ingredientesRecetaAgregar = ko.observable();
  self.cantidadRecetaAgregar = ko.observable();
  self.indicacionesRecetaAgregar = ko.observable();
  self.recetaAgregarSeleccionada = ko.observable();

  //Receta aplicacion
  self.listaRecetasAplicacion = ko.observableArray();
  self.recetasAplicacion = ko.observableArray();
  self.recetasExistentes = ko.observableArray();
  self.valorTotalDietas = ko.observable();

  //Nota medica
  self.notasMedicas = ko.observableArray();
  self.notaHistoriaProblema = ko.observable();
  self.notaSintomas = ko.observable();
  self.notaDiagnostico = ko.observable();
  self.periodo = ko.observable();

  //Nota operatoria
  self.notasOperatorias = ko.observableArray();
  self.notaDiagnosticoOperatoria = ko.observable();
  self.notaOperatoriaDetalle = ko.observable(null); // para el modal de detalle

  // Observables para edicion de nota operatoria
  self.editNotaId = ko.observable(0);
  self.editNotaFechaOperacion = ko.observable("");
  self.editNotaHoraComenzo = ko.observable("");
  self.editNotaHoraTermino = ko.observable("");
  self.editNotaCirujano = ko.observable("");
  self.editNotaPrimerAyudante = ko.observable("");
  self.editNotaSegundoAyudante = ko.observable("");
  self.editNotaAnestesista = ko.observable("");
  self.editNotaInstrumentista = ko.observable("");
  self.editNotaCirculante = ko.observable("");
  self.editNotaDiagnosticoPreOp = ko.observable("");
  self.editNotaDiagnosticoPostOp = ko.observable("");
  self.editNotaOperacionEfectuada = ko.observable("");
  self.editNotaHallazgosTransOp = ko.observable("");
  self.editNotaDiagnostico = ko.observable("");

  // ── Edición Lista de Chequeo ─────────────────────────────────────────────
  self.editChkId = ko.observable(0);
  self.editChk_FechaChequeo = ko.observable("");
  self.editChk_HoraChequeo = ko.observable("");
  self.editChk_CI_NombreConfirma = ko.observable("");
  self.editChk_CI_ApellidoConfirma = ko.observable("");
  self.editChk_CI_FechaNacConfirma = ko.observable("");
  self.editChk_CI_Consentimiento = ko.observable("");
  self.editChk_CI_Operacion = ko.observable("");
  self.editChk_CI_LadoOperar = ko.observable("");
  self.editChk_CI_SitioMarcado = ko.observable("");
  self.editChk_CI_Alergia = ko.observable("");
  self.editChk_CI_EvalPreanestesica = ko.observable("");
  self.editChk_CI_AccesoIV = ko.observable("");
  self.editChk_CI_EquipoAnestesia = ko.observable("");
  self.editChk_CI_Medicamentos = ko.observable("");
  self.editChk_CI_Oximetro = ko.observable("");
  self.editChk_CI_EquipoAspiracion = ko.observable("");
  self.editChk_CI_ViaAerea = ko.observable("");
  self.editChk_CP_Presentacion = ko.observable("");
  self.editChk_CP_NombrePacienteCirujano = ko.observable("");
  self.editChk_CP_ApellidoPacienteCirujano = ko.observable("");
  self.editChk_CP_FechaNacCirujano = ko.observable("");
  self.editChk_CP_NombreCirugia = ko.observable("");
  self.editChk_CP_EventosCriticos = ko.observable("");
  self.editChk_CP_TiempoDuracion = ko.observable("");
  self.editChk_CP_ImagenesDiagnosticas = ko.observable("");
  self.editChk_CP_PerdidaSangre = ko.observable("");
  self.editChk_CP_Esterilidad = ko.observable("");
  self.editChk_CP_MaterialesAdicionales = ko.observable("");
  self.editChk_CP_EventosCriticosAnest = ko.observable("");
  self.editChk_CP_ProfilaxisAntibiotica = ko.observable("");
  self.editChk_CP_Tromboprofilaxis = ko.observable("");
  self.editChk_CP_ManejoDolor = ko.observable("");
  self.editChk_CS_NombreOperacion = ko.observable("");
  self.editChk_CS_NombreEnfermera = ko.observable("");
  self.editChk_CS_RecuentoCompleto = ko.observable("");
  self.editChk_CS_EtiquetadoMuestras = ko.observable("");
  self.editChk_CS_RepasoPostOp = ko.observable("");
  self.editChk_CS_PorQue = ko.observable("");
  self.editChk_CS_Traslado = ko.observable("");
  self.editChk_CS_Complicaciones = ko.observable("");
  self.editChk_CS_ServicioNumCama = ko.observable("");

  // ── Edición Cuestionario Pre-Anestésico ──────────────────────────────────
  self.editPaId = ko.observable(0);
  self.editPa_NombreCompleto = ko.observable("");
  self.editPa_RegistroMedico = ko.observable("");
  self.editPa_Edad = ko.observable("");
  self.editPa_FechaCuestionario = ko.observable("");
  self.editPa_Peso = ko.observable("");
  self.editPa_Estatura = ko.observable("");
  self.editPa_FechaUltimaRegla = ko.observable("");
  self.editPa_FechaProcedimiento = ko.observable("");
  self.editPa_ProcedimientoProgramado = ko.observable("");
  self.editPa_Cirujano = ko.observable("");
  self.editPa_AlergiaCual = ko.observable("");
  self.editPa_FumaCuanto = ko.observable("");
  self.editPa_DrogasCuales = ko.observable("");
  self.editPa_AlcoholCuanto = ko.observable("");
  self.editPa_TransfusionCual = ko.observable("");
  self.editPa_AsmaCual = ko.observable("");
  self.editPa_PulmonesCual = ko.observable("");
  self.editPa_CorazonCual = ko.observable("");
  self.editPa_AtaqueCardiacoCual = ko.observable("");
  self.editPa_AnginaCual = ko.observable("");
  self.editPa_SoploCual = ko.observable("");
  self.editPa_PresionCual = ko.observable("");
  self.editPa_HigadoCual = ko.observable("");
  self.editPa_RinonesCual = ko.observable("");
  self.editPa_DiabetesCual = ko.observable("");
  self.editPa_EpilepsiaCual = ko.observable("");
  self.editPa_DerrameCual = ko.observable("");
  self.editPa_TiroidesCual = ko.observable("");
  self.editPa_AnestesicoCual = ko.observable("");
  self.editPa_AceptaTransfusionCual = ko.observable("");
  self.editPa_EmbarazoCual = ko.observable("");
  self.editPa_AI_Medicamentos = ko.observable("");
  self.editPa_AI_ActividadDetalle = ko.observable("");
  self.editPa_AI_OperacionesPrevias = ko.observable("");
  self.editPa_AI_Comentarios = ko.observable("");

  // Firma nota operatoria
  self.firmaNotaOpId = ko.observable(null);
  self.firmaNotaOpBase64 = ko.observable(null);
  self.firmaNotaOpExistenteUrl = ko.observable(null);
  self.firmaNotaOpModo = ko.observable("canvas");

  // Campos extendidos nota operatoria
  self.notaFechaOperacion = ko.observable();
  self.notaHoraComenzo = ko.observable();
  self.notaHoraTermino = ko.observable();
  self.notaCirujano = ko.observable();
  self.notaPrimerAyudante = ko.observable();
  self.notaSegundoAyudante = ko.observable();
  self.notaAnestesista = ko.observable();
  self.notaInstrumentista = ko.observable();
  self.notaCirculante = ko.observable();
  self.notaDiagnosticoPreOp = ko.observable("");
  self.notaDiagnosticoPostOp = ko.observable("");
  self.notaOperacionEfectuada = ko.observable("");
  self.notaHallazgosTransOp = ko.observable("");

  // ── Kit de Ingreso ────────────────────────────────────────────────────────
  self.kitsIngreso = ko.observableArray();
  self.kitIngresoDetalle = ko.observable(null);
  self.kit_kitIdActivo = ko.observable(null);
  self.kit_productoSeleccionadoId = ko.observable();
  self.kit_cantidadAgregar = ko.observable(1);


  // Encabezado
  self.kit_NombreKit = ko.observable("Kit de Ingreso");
  self.kit_NombrePaciente = ko.observable("");
  self.kit_Medico = ko.observable("");
  self.kit_Procedimiento = ko.observable("");
  self.kit_Responsable = ko.observable("");
  self.kit_Fecha = ko.observable("");

  // Total utilizado: suma de Utilizado de todos los productos no eliminados
  self.kit_Utilizado = ko.pureComputed(function () {
    var total = 0;
    ko.utils.arrayForEach(self.kit_productosAgregados(), function (p) {
      if (!p.Eliminado()) {
        var used = parseFloat(ko.unwrap(p.Utilizado)) || 0;
        total += used;
      }
    });
    return total;
  });

  // Inventario / selección de producto
  self.kit_registrosInventario = ko.observableArray();
  self.kit_productosExistentes = ko.observableArray();
  self.kit_productoSeleccionado = ko.observable();
  // self.kit_unidadesVenta = ko.observableArray();
  // self.kit_unidadSeleccionada = ko.observable();
  self.kit_precios = ko.observableArray();
  // self.kit_precioSeleccionado = ko.observable();
  self.kit_cantidad = ko.observable(1);
  self.kit_productosAgregados = ko.observableArray();
  self.kit_total = ko.observable(0);

  // ── Cuestionario Pre-Anestésico ───────────────────────────────────────────
  self.cuestionariosPreAnest = ko.observableArray();
  self.cuestionarioPreAnestDetalle = ko.observable(null);

  // Datos del paciente
  self.pa_NombreCompleto = ko.observable("");
  self.pa_RegistroMedico = ko.observable("");
  self.pa_Edad = ko.observable("");
  self.pa_FechaCuestionario = ko.observable("");
  self.pa_Peso = ko.observable("");
  self.pa_Estatura = ko.observable("");
  self.pa_FechaUltimaRegla = ko.observable("");
  self.pa_FechaProcedimiento = ko.observable("");
  self.pa_ProcedimientoProgramado = ko.observable("");
  self.pa_Cirujano = ko.observable("");

  // Textos libres de antecedentes
  self.pa_AlergiaCual = ko.observable("");
  self.pa_FumaCuanto = ko.observable("");
  self.pa_DrogasCuales = ko.observable("");
  self.pa_AlcoholCuanto = ko.observable("");
  self.pa_TransfusionCual = ko.observable("");
  self.pa_AsmaCual = ko.observable("");
  self.pa_PulmonesCual = ko.observable("");
  self.pa_CorazonCual = ko.observable("");
  self.pa_AtaqueCardiacoCual = ko.observable("");
  self.pa_AnginaCual = ko.observable("");
  self.pa_SoploCual = ko.observable("");
  self.pa_PresionCual = ko.observable("");
  self.pa_HigadoCual = ko.observable("");
  self.pa_RinonesCual = ko.observable("");
  self.pa_DiabetesCual = ko.observable("");
  self.pa_EpilepsiaCual = ko.observable("");
  self.pa_DerrameCual = ko.observable("");
  self.pa_TiroidesCual = ko.observable("");
  self.pa_AnestesicoCual = ko.observable("");
  self.pa_AceptaTransfusionCual = ko.observable("");
  self.pa_EmbarazoCual = ko.observable("");

  // Información adicional
  self.pa_AI_Medicamentos = ko.observable("");
  self.pa_AI_ActividadDetalle = ko.observable("");
  self.pa_AI_OperacionesPrevias = ko.observable("");
  self.pa_AI_Comentarios = ko.observable("");

  // Usuarios con acceso
  self.usuariosAcceso = ko.observableArray();
  self.usuarioSeleccionado = ko.observable();
  self.listaUsuarios = ko.observableArray();

  // ── Lista de Chequeo Quirúrgica ───────────────────────────────────────────
  self.listasChequeo = ko.observableArray();
  self.listaChequeoDetalle = ko.observable(null);

  // Encabezado
  self.chk_NombrePaciente = ko.observable("");
  self.chk_ApellidoPaciente = ko.observable("");
  self.chk_FechaNacimiento = ko.observable("");
  self.chk_MedicoTratante = ko.observable("");
  self.chk_FechaChequeo = ko.observable("");
  self.chk_HoraChequeo = ko.observable("");

  // ENTRADA — Paciente
  self.chk_CI_NombreConfirma = ko.observable("");
  self.chk_CI_ApellidoConfirma = ko.observable("");
  self.chk_CI_FechaNacConfirma = ko.observable("");
  self.chk_CI_Operacion = ko.observable("");

  // PAUSA — Cirujano
  self.chk_CP_NombrePacienteCirujano = ko.observable("");
  self.chk_CP_ApellidoPacienteCirujano = ko.observable("");
  self.chk_CP_FechaNacCirujano = ko.observable("");
  self.chk_CP_NombreCirugia = ko.observable("");

  // SALIDA
  self.chk_CS_NombreOperacion = ko.observable("");
  self.chk_CS_NombreEnfermera = ko.observable("");
  self.chk_CS_PorQue = ko.observable("");
  self.chk_CS_Complicaciones = ko.observable("");
  self.chk_CS_ServicioNumCama = ko.observable("");

  //#region Variables TABS
  self.tabDietasAbierta = ko.observable(false);
  self.tabNotaMedica2Abierta = ko.observable(false);
  self.tabNotaOperatoriaAbierta = ko.observable(false);
  self.tabSignosVitalesAbierta = ko.observable(false);
  self.tabPagosAbierta = ko.observable(false);
  self.tabEnfermeriaAbierta = ko.observable(false);
  self.tabGlucometriaAbierta = ko.observable(false);
  self.tabIncretaExcretaAbierta = ko.observable(false);
  self.tabArchivosAbierta = ko.observable(false);
  self.tabHistorialConsultasAbierta = ko.observable(false);
  self.tabHistoriaClinica = ko.observable(false);
  self.tabHistorialHospitalizacionesAbierta = ko.observable(false);
  self.tabChequeoAbierta = ko.observable(false);
  self.tabPreanestesicoAbierta = ko.observable(false);
  self.tabKitIngresoAbierta = ko.observable(false);
  self.tabMedicamentosNoControladosAbierta = ko.observable(false);

  //#endregion

  self.solicitudesDeMedicamentos = ko.observableArray();

  self.valorTotalEmergencias = ko.observable(0);
  self.valorTotalEmergenciasTexto = ko.observable("Q. 0.00");


  self.hospitalizacionId = ko.observable(0);
  self.emergenciaId = ko.observable(0);
  self.productosEmergenciaHospi = ko.observableArray([]);
  self.serviciosEmergenciaHospi = ko.observableArray([]);
  self.examenesEmergenciaHospi = ko.observableArray([]);

  self.productoEmergenciaSeleccionado = ko.observable();
  self.servicioEmergenciaSeleccionado = ko.observable();
  self.examenEmergenciaSeleccionado = ko.observable();

  self.productosExistentesEmergencia = ko.observableArray([]);
  self.serviciosExistentesEmergencia = ko.observableArray([]);
  self.examenesExistentesEmergencia = ko.observableArray([]);

  self.preciosProductoEmergencia = ko.observableArray([]);
  self.unidadesVentaProductoEmergencia = ko.observableArray([]);
  self.unidadVentaSeleccionadaProductoEmergencia = ko.observable();
  self.precioSeleccionadoProductoEmergencia = ko.observable();

  self.preciosServicioEmergencia = ko.observableArray([]);
  self.precioSeleccionadoServicioEmergencia = ko.observable();

  self.preciosExamenEmergencia = ko.observableArray([]);
  self.precioSeleccionadoExamenEmergencia = ko.observable();

  self.cantidadProductoEmergenciaAgregar = ko.observable(1);
  self.cantidadServicioEmergenciaAgregar = ko.observable(1);
  self.cantidadExamenEmergenciaAgregar = ko.observable(1);


  self.productosExistentesEmergencia = ko.observableArray([]);
  self.serviciosExistentesEmergencia = ko.observableArray([]);
  self.examenesExistentesEmergencia = ko.observableArray([]);


  self.totalProductosEmergencia = ko.observable(0);
  self.totalServiciosEmergencia = ko.observable(0);
  self.totalExamenesEmergencia = ko.observable(0);

  self.listaPersonalEnfermeria = ko.observableArray([]);
  self.listaMedicosModal = ko.observableArray([]);  // sin filtro: Cirujano, 1er y 2do Ayudante
  self.listaAnestesistas = ko.observableArray([]);  // especialidadId=22
  self.listaInstrumentistas = ko.observableArray([]);  // unidadOrgId=12
  self.listaCirculantes = ko.observableArray([]);  // unidadOrgId=13

  self.habitacionId = ko.observable(0);

  self.medicamentosNoControlados = ko.observableArray([]);
  self.fechaProcedimientoNoControlados = ko.observable(moment().format('YYYY-MM-DD'));
  self.nombrePacienteNoControlados = ko.observable($("#PacienteNombreAdmision").val() || "");
  self.registrosPreviosNoControlados = ko.observableArray([]);
  self.mostrarAdvertenciaBalanceNoControlados = ko.observable(false);
  self.mensajeBalanceNoControlados = ko.observable("");

  self.listaProductosNoControlados = ko.observableArray([]);
  self.productoSeleccionadoNoControlado = ko.observable();
  self.productoSeleccionadoNoControladoId = ko.observable();

  self.documentos = ko.observableArray([]);

  self.firmaEnfermeriaId = ko.observable(null);
  self.firmaEnfermeriaBase64 = ko.observable(null);
  self.firmaEnfermeriaExistenteUrl = ko.observable(null);


  self.firmaTurnoId = ko.observable(null);
  self.firmaTurnoBase64 = ko.observable(null);
  self.firmaTurnoExistenteUrl = ko.observable(null);
  self._firmaTurnoModalEl = null;


  self.currentUserHasFingerprint = ko.observable(false);
  self.availableUsersForAuth = ko.observableArray([]);
  self.selectedAuthUserId = ko.observable(null);


  self.kitsGlobales = ko.observableArray();
  self.kitsLocales = ko.observableArray();

  self.insumosDirectosAplicados = ko.observableArray([]);
  self.valorTotalInsumosDirectos = ko.observable(0);
  self._acumuladoInsumosDirectos = 0;
  self._acumuladoInsumosDirectosListo = false;


  self.mostrarTabProductos = ko.observable(false);
  self.mostrarTabServicios = ko.observable(false);
  self.mostrarTabLaboratorios = ko.observable(false);
  self.mostrarTabDietas = ko.observable(false);

  self.tarifasDisponibles = ko.observableArray([]);
  self.tarifasEstancia = ko.observableArray([]);   // Lista de tarifas disponibles
  self.nuevaTarifaEstanciaId = ko.observable();     // ID de la tarifa seleccionada
  self.tarifaSeleccionadaTexto = ko.observable(""); // (opcional) para mostrar la tarifa actual

  self.totalPaquetesBadge = ko.observable(0);
  self.totalPaquetesAplicadosBadge = ko.observable(0);
  self.totalPaquetesPendientesBadge = ko.observable(0);
  self.filtroEstadoPaquetes = ko.observable('todos');
  self.paginaActualPaquetes = ko.observable(1);
  self.itemsPorPaginaPaquetes = ko.observable(10);
  self.filtroBusquedaPaquetes = ko.observable("");
  self.filtroActivo = ko.observable('todos');

  self._totalesPendientes = 0;
  // ── Modal para modificar tarifa/días de un cambio de habitación ──────────
  self.cambioHabitacionEditando = ko.observable(null);
  self.editTarifaCambio = ko.observable(0);
  self.editDiasCambio = ko.observable(1);

  // En modo edición de nota de enfermería
  self.editandoNotaEnfermeria = ko.observable(false);
  self.editNotaEnfermeriaId = ko.observable(null);

  // En modo edición de nota médica/evolución
  self.editandoNotaMedica = ko.observable(false);
  self.editNotaMedicaId = ko.observable(null);

  self.actualizarBadgesPaquetes = function () {
    var total = self.listaProductosAplicacion().length;
    var aplicados = self.listaProductosAplicacion().filter(function (item) {
      return ko.unwrap(item.Aplicado);
    }).length;
    var pendientes = total - aplicados;
    self.totalPaquetesBadge(total);
    self.totalPaquetesAplicadosBadge(aplicados);
    self.totalPaquetesPendientesBadge(pendientes);
  };

  self.listaProductosAplicacion.subscribe(function () {
    self.actualizarBadgesPaquetes();
  });


  self.filtrarPaquetes = function (estado) {
    self.filtroEstadoPaquetes(estado);
    self.paginaActualPaquetes(1);
    self.filtroActivo(estado);
  };

  self.elementosPaginadosPaquetes = ko.pureComputed(function () {
    var lista = self.listaProductosAplicacion();
    var busqueda = self.filtroBusquedaPaquetes().toLowerCase();
    var estado = self.filtroEstadoPaquetes();

    if (busqueda) {
      lista = lista.filter(function (item) {
        var nombre = ko.unwrap(item.Nombre) || '';
        var codigo = ko.unwrap(item.Codigo) || '';
        var tipo = ko.unwrap(item.Tipo) || '';
        return nombre.toLowerCase().includes(busqueda) ||
          codigo.toLowerCase().includes(busqueda) ||
          tipo.toLowerCase().includes(busqueda);
      });
    }

    if (estado === 'aplicados') {
      lista = lista.filter(function (item) { return ko.unwrap(item.Aplicado); });
    } else if (estado === 'pendientes') {
      lista = lista.filter(function (item) { return !ko.unwrap(item.Aplicado); });
    }

    var start = (self.paginaActualPaquetes() - 1) * self.itemsPorPaginaPaquetes();
    return lista.slice(start, start + self.itemsPorPaginaPaquetes());
  });

  self.totalPaginasPaquetes = ko.pureComputed(function () {
    var lista = self.listaProductosAplicacion().slice();
    var busqueda = self.filtroBusquedaPaquetes().toLowerCase();
    var estado = self.filtroEstadoPaquetes();

    if (busqueda) {
      lista = lista.filter(function (item) {
        var nombre = ko.unwrap(item.Nombre) || '';
        var codigo = ko.unwrap(item.Codigo) || '';
        var tipo = ko.unwrap(item.Tipo) || '';
        return nombre.toLowerCase().includes(busqueda) ||
          codigo.toLowerCase().includes(busqueda) ||
          tipo.toLowerCase().includes(busqueda);
      });
    }
    if (estado === 'aplicados') {
      lista = lista.filter(function (item) { return ko.unwrap(item.Aplicado); });
    } else if (estado === 'pendientes') {
      lista = lista.filter(function (item) { return !ko.unwrap(item.Aplicado); });
    }
    return Math.ceil(lista.length / self.itemsPorPaginaPaquetes()) || 1;
  });


  self.actualizarBadgesPaquetes();

  self.checkCurrentUserFingerprint = async function () {
    try {
      const res = await fetch("/api/WebAuthnVerify/HasCredential");
      const data = await res.json();
      self.currentUserHasFingerprint(data.hasCredential);
      if (!data.hasCredential) {
        await self.loadAvailableUsers();
      }
    } catch (e) {
      console.error(e);
      self.currentUserHasFingerprint(false);
    }
  };

  self.loadAvailableUsers = async function () {
    try {
      const res = await fetch("/api/WebAuthnVerify/ListUsersWithCredentials");
      const users = await res.json();
      self.availableUsersForAuth(users);
    } catch (e) {
      console.error(e);
      self.availableUsersForAuth([]);
    }
  };

  //#region Funciones TABS
  self.abrirTabNotaMedica2 = function () {
    if (!self.tabNotaMedica2Abierta()) {
      self.consultarNotasMedicas();
      self.tabNotaMedica2Abierta(true);
    }
  };
  self.abrirTabNotaOperatoria = function () {
    if (!self.tabNotaOperatoriaAbierta()) {
      self.consultarNotasOperatorias();
      self.tabNotaOperatoriaAbierta(true);
    }
  };
  self.abrirTabIncretaExcreta = function () {
    if (!self.tabIncretaExcretaAbierta()) {
      self.consultarIngestaExcreta();
      self.tabIncretaExcretaAbierta(true);
    }
  };
  self.abrirTabDietas = function () {
    if (!self.tabDietasAbierta()) {
      self.consultarRecetasAplicadas();
      self.tabDietasAbierta(true);
    }
  };
  self.abrirTabEnfermeria = function () {
    if (!self.tabEnfermeriaAbierta()) {
      self.consultarProductosAplicacionHospitalizacion();
      self.tabEnfermeriaAbierta(true);
    }
  };
  self.abrirTabSignosVitales = function () {
    if (!self.tabSignosVitalesAbierta()) {
      self.consultarExamenesFisicosHospitalizacion();
      self.tabSignosVitalesAbierta(true);
    }
  };
  self.abrirTabGlucometria = function () {
    if (!self.tabGlucometriaAbierta()) {
      self.consultarControlGlucometria();
      self.tabGlucometriaAbierta(true);
    }
  };
  self.abrirTabPagos = function () {
    if (!self.tabPagosAbierta()) {
      self.consultarDepositosHospitalizacion();
      self.tabPagosAbierta(true);
    }
  };
  self.abrirTabArchivos = function () {
    if (!self.tabArchivosAbierta()) {
      self.consultarArchivosPaciente();
      self.tabArchivosAbierta(true);
    }
  };
  self.abrirTabHistorialConsultas = function () {
    if (!self.tabHistorialConsultasAbierta()) {
      self.consultarHistorialConsultasPaciente();
      self.tabHistorialConsultasAbierta(true);
    }
  };
  self.abrirTabHistorialHospitalizaciones = function () {
    if (!self.tabHistorialHospitalizacionesAbierta()) {
      self.consultarHistorialHospitalizaciones();
      self.tabHistorialHospitalizacionesAbierta(true);
    }
  };

  self.abrirTabChequeo = function () {
    if (!self.tabChequeoAbierta()) {
      self.consultarListasChequeo();
      self.tabChequeoAbierta(true);
    }
  };

  self.abrirTabPreanestesico = function () {
    if (!self.tabPreanestesicoAbierta()) {
      self.consultarCuestionariosPreAnest();
      self.tabPreanestesicoAbierta(true);
    }
  };

  self.abrirTabKitIngreso = function () {
    if (!self.tabKitIngresoAbierta()) {
      self.consultarKitsIngreso();
      self.kit_cargarProductosInventario();
      self.tabKitIngresoAbierta(true);
    }
  };

  self.abrirTabMedicamentosNoControlados = function () {
    if (!self.tabMedicamentosNoControladosAbierta()) {
      self.tabMedicamentosNoControladosAbierta(true);
      self.cargarProductosNoControlados();
      self.cargarHistorialMedicamentosNoControlados();
    }
  };

  //#endregion

  // self.detallesHospitalizacion(hospitalizacionId, categoria) {
  //     window.location.href = `/Hospitalizacion/Detalles?hospitalizacionId=${hospitalizacionId}&categoria=${categoria}`;
  // };

  self.consultarArchivosPaciente = function () {
    let textoCargando = $("#texto-cargando-archivos-paciente");
    let textoError = $("#texto-error-consultar-archivos-paciente");
    textoCargando.show();
    textoError.hide();

    $.ajax({
      url: "/Hospitalizacion/ConsultarArchivosPaciente",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(), // Asegúrate de tener este ID en tu vista
      },
      success: function (result) {
        let data = JSON.parse(result);
        textoCargando.hide();

        if (data.Exitoso) {
          self.archivosPaciente(data.Resultado);
          console.log(data.Resultado); // Para verificar los datos
        } else {
          textoError.show();
        }
      },
      error: function () {
        textoCargando.hide();
        textoError.show();
      },
    });
  };

  // Función para cargar los archivos desde el servidor
  self.cargarArchivosAutorizaciones = function (hospitalizacionId) {
    $.ajax({
      url: "/Hospitalizacion/ConsultarArchivosAutorizaciones",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify({ hospitalizacionId: hospitalizacionId }),
      success: function (data) {
        var respuesta = JSON.parse(data);
        if (respuesta.Exitoso) {
          self.archivosAutorizaciones(respuesta.Resultado);
        } else {
          console.error("Error: ", respuesta.Mensaje);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error al cargar los archivos: ", error);
      },
    });
  };

  // Llamar a la función al inicializar
  self.cargarArchivosAutorizaciones();
  //self.cargarArchivosAutorizaciones(parseInt($("#HospitalizacionId").val(), 10));

  self.consultarHistorialConsultasPaciente = function () {
    let textoCargando = $("#texto-cargando-consultas");
    let textoError = $("#texto-error-consultar-consultas");
    textoCargando.show();
    textoError.hide();
    $.ajax({
      url: "/Hospitalizacion/ConsultarHistorialConsultasPaciente",
      method: "POST",
      data: {
        pacienteId: $("#PacienteId").val(),
      },
      success: function (result) {
        textoCargando.hide();
        let data = JSON.parse(result);
        if (data.Exitoso) {
          self.historialConsultasPaciente(data.Resultado);
        } else {
          textoError.show();
        }
      },
      error: function (resultError) {
        textoCargando.hide();
        textoError.show();
        console.log(resultError);
      },
    });
  };
  //Historial de hospitalizaciones
  self.consultarHistorialHospitalizaciones = function () {
    let textoCargando = $("#texto-cargando-historial-hospitalizaciones");
    let textoError = $("#texto-error-consultar-historial-hospitalizaciones");
    textoCargando.show();
    textoError.hide();
    let pacienteId = $("#PacienteId").val();
    showLoading();
    $.ajax({
      method: "POST",
      url: "/Hospitalizacion/ConsultarHistorialHospitalizacionesPaciente",
      data: {
        pacienteId: pacienteId,
      },
      success: function (result) {
        (hideLoading(), textoCargando.hide());
        let data = JSON.parse(result);
        if (data.Exitoso) {
          self.historialHospitalizacionesPaciente(data.Resultado);
          hideLoading();
        } else {
          textoError.show();
        }
      },
      error: function (data) {
        (hideLoading(), textoCargando.hide());
        textoError.show();
        console.log(data.error);
      },
    });
  };
  self.recetaAgregarSeleccionada.subscribe(function (value) {
    if (value && value.Ingredientes) {
      self.ingredientesRecetaAgregar(value.Ingredientes);
    } else {
      self.ingredientesRecetaAgregar(""); // O establece un valor por defecto si es necesario
    }
  });
  self.recetaIdAgregar.subscribe(function (id) {
    if (!id) {
      self.nombreRecetaAgregar("");
      self.ingredientesRecetaAgregar("");
      return;
    }

    var receta = ko.utils.arrayFirst(self.recetasExistentes(), function (r) {
      return r.Id == id;
    });

    if (receta) {
      self.nombreRecetaAgregar(receta.NombreReceta || "");
      self.ingredientesRecetaAgregar(receta.Ingredientes || "");
    } else {
      self.nombreRecetaAgregar("");
      self.ingredientesRecetaAgregar("");
    }
  });

  self.servicioAgregarSeleccionado.subscribe(function (value) {
    console.log("[Servicios] servicioAgregarSeleccionado changed:", value);

    if (value && value.Precio) {
      console.log("[Servicios] value.Precio detectado:", value.Precio);
      self.valorServicioAgregar(value.Precio);
    } else {
      console.log(
        "[Servicios] value.Precio no existe para este objeto (esperable si viene de ConsultarServiciosExistentes).",
      );
    }
  });

  self.medicamentoAgregarSeleccionado.subscribe(function (value) {
    if (value && value.Precio) {
      self.valorMedicamentoAgregar(value.Precio);
    } else {
      console.log("");
    }
  });
  self.examenAgregarSeleccionado.subscribe(function (value) {
    if (value && value.Precio) {
      self.valorExamenAgregar(value.Precio);
    } else {
      console.log("");
    }
  });

  self.consultarPaquetesExistentes = function () {
    let textoCargando = $("#texto-cargando-paquetes-disponibles");
    let textoError = $("#texto-error-consultar-paquetes-disponibles");
    textoCargando.show();
    textoError.hide();
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarPaquetesExistentes",
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.paquetesExistentes(data.Resultado);
          textoCargando.hide();
          textoError.hide();
        } else {
          textoCargando.hide();
          textoError.show();
        }
      },
      error: function (dataError) {
        hideLoading();
        textoCargando.hide();
        textoError.show();
        console.log(dataError);
      },
    });
  };

  self.agregarObservacionProductsHospi = function () {
    if (
      !self.observacionesProductsHospi() ||
      self.observacionesProductsHospi().trim() === ""
    ) {
      mensajeEmergenteError("La observación no puede estar vacía.");
      return;
    }

    if (self.isUpdateObservacionProductsHospi() === true) {
      var observacionEditada = {
        Id: self.observacionesProductsHospiId(),
        HospitalizacionProductoAplicacionId:
          self.observacionesProductsHospiAplicacionId(),
        Observacion: self.observacionesProductsHospi(),
        FechaCreacion: self.observacionesProductsHospiFecha(),
        UsuarioCreaId: self.observacionesProductsHospiUsuarioCrea(),
      };
      showLoading();
      $.ajax({
        url: "/HospitalizacionProductoObservacion/Modificar",
        method: "POST",
        data: observacionEditada,
        success: function (dataResult) {
          hideLoading();
          var data = JSON.parse(dataResult);
          if (data.exitoso) {
            self.consultarObservacionProductsHospi();
            self.observacionesProductsHospi(""); // Limpiar el textArea
            mensajeEmergente("Observación editada");
          } else {
            mensajeEmergenteError("Error: " + data.resultado);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError("Error al editar la observación");
        },
      });
    } else {
      var nuevaObservacion = {
        HospitalizacionProductoAplicacionId:
          self.observacionesProductsHospiAplicacionId(),
        Observacion: self.observacionesProductsHospi(),
        FechaCreacion: new Date().toISOString(),
        UsuarioCreaId: self.observacionesProductsHospiUsuarioCrea(),
      };
      showLoading();
      $.ajax({
        url: "/HospitalizacionProductoObservacion/Nuevo",
        method: "POST",
        data: nuevaObservacion,
        success: function (dataResult) {
          hideLoading();
          var data = JSON.parse(dataResult);
          if (data.exitoso) {
            self.consultarObservacionProductsHospi();
            self.observacionesProductsHospi(""); // Limpiar el textArea
            mensajeEmergente("Observación agregada");
          } else {
            mensajeEmergenteError("Error: " + data.resultado);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError("Error al agregar la nueva observación");
        },
      });
    }
  };

  self.consultarObservacionProductsHospi = function () {
    showLoading();
    $.ajax({
      url: "/HospitalizacionProductoObservacion/ListaObservaciones",
      method: "GET",
      data: {
        hospitalizacionProductoAplicacionId:
          self.observacionesProductsHospiAplicacionId(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.exitoso) {
          // Ordenar los elementos por FechaCreacion en orden descendente
          let resultadoOrdenado = data.resultado.sort(function (a, b) {
            return new Date(b.FechaCreacion) - new Date(a.FechaCreacion);
          });
          // Asignar los elementos ordenados al observable
          self.observacionesProductsHospiLista(resultadoOrdenado);
        } else {
          mensajeEmergenteError(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };

  self.modificarObservacionProductsHospi = function (value) {
    self.observacionesProductsHospiId(value.Id);
    self.observacionesProductsHospiFecha(value.FechaCreacion);
    self.observacionesProductsHospi(value.Observacion);
    self.observacionesProductsHospiUsuarioCrea(value.UsuarioCreaId);
    self.isUpdateObservacionProductsHospi(true);
  };

  self.eliminarObservacionProductsHospi = function (value) {
    if (confirm("¿Desea eliminar esta observación?")) {
      showLoading();
      $.ajax({
        url: "/HospitalizacionProductoObservacion/Eliminar",
        method: "POST",
        data: {
          id: value.Id,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.exitoso) {
            self.consultarObservacionProductsHospi();
            mensajeEmergente("Observación eliminada");
          } else {
            mensajeEmergenteError(data.mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };

  self.limpiarCamposObservacionProductsHospi = function () {
    self.observacionesProductsHospiLista("");
    self.observacionesProductsHospiId("");
    self.observacionesProductsHospi("");
    self.observacionesProductsHospiUsuarioCrea("");
    self.observacionesProductsHospiAplicacionId("");
    self.observacionesProductsHospiFecha("");
    self.isUpdateObservacionProductsHospi("");
  };

  self.consultarTurnoEnfermeria = function () {
    showLoading();
    $.ajax({
      url: "/TurnoEnfermeria/ListaTurnoEnfermeria",
      method: "POST",
      data: { hospitalizacionId: $("#HospitalizacionId").val() },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.exitoso) {
          var turnos = data.resultado.map(function (t) {
            return {
              Id: t.Id ?? t.id,
              FechaRegistro: t.FechaRegistro ?? t.fechaRegistro,
              Profesional: t.Profesional ?? t.profesional ?? "",
              NombreTurno: t.NombreTurno ?? t.nombreTurno,
              NumeroTurno: t.NumeroTurno ?? t.numeroTurno,
              Firmado: !!(t.Firmado ?? t.firmado),
              FirmaRuta: t.FirmaRuta ?? t.firmaRuta ?? null,
              UsuarioFirmaId: t.UsuarioFirmaId ?? t.usuarioFirmaId ?? null,
            };
          });
          self.turnosEnfermeria(turnos);
        } else {
          mensajeEmergenteError(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      }
    });
  };



  self.editarNotaEnfermeria = function (nota) {
    if (ko.unwrap(nota.Firmado)) {
      mensajeEmergenteError("No se puede editar una nota firmada.");
      return;
    }

    self.editandoNotaEnfermeria(true);
    self.editNotaEnfermeriaId(nota.Id);

    if (self.quillEditor) {
      self.quillEditor.root.innerHTML = nota.Diagnostico || "";
    }
    self.notaDiagnostico(nota.Diagnostico || "");

    $("#mdl-agregar-nota-enfermeria").dialog({
      width: 1000,
      open: function () {
        if (self.quillEditor && nota.Diagnostico) {
          self.quillEditor.root.innerHTML = nota.Diagnostico;
        }
      },
      close: function () {
        self.editandoNotaEnfermeria(false);
        self.editNotaEnfermeriaId(null);
        self.notaDiagnostico("");
        if (self.quillEditor) self.quillEditor.setText("");
      }
    });
  };


  $("#mdl-agregar-nota-operatoria").on("dialogclose", function () {
    // Resetea el baseText de los textareas afectados
    if (window.currentActiveId) {
      window.stopRecording(window.currentActiveId);
    }
    // Opcional: limpia manualmente baseText de cada ID
    ["diagnosticoPreOperatorio_NO", "diagnosticoPostOperatorio_NO",
      "operacionEfectuada_NO", "hallazgos_NO"].forEach(id => {
        if (window._dictationBaseText && window._dictationBaseText[id]) {
          delete window._dictationBaseText[id];
        }
      });
  });


  self.consultarNotaEnfermeria = function () {
    showLoading();
    $.ajax({
      url: "/NotaEnfermeria/ListaNotaEnfermeria",
      method: "POST",
      data: { hospitalizacionId: $("#HospitalizacionId").val() },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.exitoso) {
          var notas = data.resultado.map(function (n) {
            return {
              Id: n.Id ?? n.id,
              FechaRegistro: n.FechaRegistro ?? n.fechaRegistro,
              Profesional: n.Profesional ?? n.profesional ?? "",
              Diagnostico: n.Diagnostico ?? n.diagnostico ?? "",
              Evolucion: n.Evolucion ?? n.evolucion ?? "",
              Sintomas: n.Sintomas ?? n.sintomas ?? "",
              TurnoEnfermeriaId: n.TurnoEnfermeriaId ?? n.turnoEnfermeriaId ?? null,
              Firmado: ko.observable(!!(n.Firmado ?? n.firmado)),
              FirmaRuta: n.FirmaRuta ?? n.firmaRuta ?? null,
              FechaFirma: n.FechaFirma ?? n.fechaFirma ?? null,
            };
          });
          self.notasEnfermerias(notas);
        } else {
          mensajeEmergenteError(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      }
    });
  };

  // --- FUNCIÓN PARA CONSULTAR HONORARIOS ---
  self.consultarHonorarios = function () {
    const hospitalizacionId = $("#HospitalizacionId").val();
    if (!hospitalizacionId) return;

    $.ajax({
      url: '/CuentasPorCobrar/ConsultarHonorariosHospitalizacion',
      method: 'POST',
      data: { hospitalizacionId: hospitalizacionId },
      success: function (response) {
        let data = JSON.parse(response);
        if (data.Exitoso) {
          self.honorariosLista(data.Resultado);
        }
      },
      error: function () {
        console.error("Error al consultar honorarios médicos.");
      }
    });
  };

  self.verModalDetalleTotales = function () {
    $("#mdl-detalle-totales").dialog({
      width: 750,
      modal: true,
    });
  };

  self.limpiarModalNotaEnfermeria = function () {
    self.notaDiagnostico("");
    self.editandoNotaEnfermeria(false);
    self.editNotaEnfermeriaId(null);
    if (self.quillEditor) self.quillEditor.setText("");
  };

  // Método para agregar turno de enfermería
  self.agregarTurnoEnfermeria = function () {
    let dtFechaRegistro = new Date().toISOString();
    let dtNumeroTurno;
    let dtNombreTurno;

    // Función para restar un día calendario
    function restarUnDia(fecha) {
      let nuevaFecha = new Date(fecha);
      nuevaFecha.setDate(nuevaFecha.getDate() - 1);
      return nuevaFecha;
    }

    // Convertir el string ISO a un objeto de fecha
    let fechaRegistro = new Date(dtFechaRegistro);

    // Obtener la hora del registro para decidir el turno
    let horaRegistro = fechaRegistro.getHours();
    let diaRegistro = fechaRegistro.getDate();
    let mesRegistro = fechaRegistro.getMonth();
    let anioRegistro = fechaRegistro.getFullYear();

    // Asignación del número de turno y nombre del turno según el rango horario
    if (horaRegistro >= 1 && horaRegistro < 7) {
      dtNumeroTurno = 1;
      dtNombreTurno = `Turno #1 del día ${diaRegistro}/${mesRegistro + 1
        }/${anioRegistro}`;
    } else if (horaRegistro >= 7 && horaRegistro < 13) {
      dtNumeroTurno = 2;
      dtNombreTurno = `Turno #2 del día ${diaRegistro}/${mesRegistro + 1
        }/${anioRegistro}`;
    } else if (horaRegistro >= 13 && horaRegistro < 19) {
      dtNumeroTurno = 3;
      dtNombreTurno = `Turno #3 del día ${diaRegistro}/${mesRegistro + 1
        }/${anioRegistro}`;
    } else if ((horaRegistro >= 19 && horaRegistro <= 23) || horaRegistro < 1) {
      dtNumeroTurno = 4;

      // Si la hora de registro está antes de las 11:59, se mantiene el día del registro
      if (horaRegistro < 1) {
        // Si es después de medianoche pero antes de las 01:00, restamos un día calendario
        let fechaModificada = restarUnDia(fechaRegistro);
        dtNombreTurno = `Turno #4 del día ${fechaModificada.getDate()}/${fechaModificada.getMonth() + 1
          }/${fechaModificada.getFullYear()}`;
      } else {
        dtNombreTurno = `Turno #4 del día ${diaRegistro}/${mesRegistro + 1
          }/${anioRegistro}`;
      }
    }

    // console.log(`Número de turno: ${dtNumeroTurno}`);

    // console.log(`Nombre del turno: ${dtNombreTurno}`);

    // Validación para no permitir crear un turno con el mismo nombre
    let turnoExistente = self.turnosEnfermeria().find(function (turno) {
      return turno.NombreTurno === dtNombreTurno;
    });

    if (turnoExistente) {
      mensajeEmergenteError(
        "Ya existe un turno con el mismo nombre. No se puede agregar.",
      );
      return; // Detener la ejecución si ya existe el turno
    }

    var nuevoTurno = {
      FechaRegistro: dtFechaRegistro,
      NumeroTurno: dtNumeroTurno,
      NombreTurno: dtNombreTurno,
      HospitalizacionId: $("#HospitalizacionId").val(),
      Firmado: false,
      UserId: "",
    };

    showLoading();
    $.ajax({
      url: "/TurnoEnfermeria/Nuevo",
      method: "POST",
      data: nuevoTurno,
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarTurnoEnfermeria();
          mensajeEmergente("Turno de enfermería agregado");
        } else {
          mensajeEmergenteError("Error: " + data.resultado);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError("Error al agregar el turno de enfermería");
      },
    });
  };

  self.firmarTurnoEnfermeria = function (turnoId) {
    if (
      confirm(
        "¿Has verificado que este turno haya terminado? Recuerda que no podrás crear otro turno hasta que este esté finalizado.",
      )
    ) {
      if (
        confirm(
          "¿Estás seguro de que deseas firmar este turno de enfermería? Una vez firmado, no podrás agregar más notas a este turno.",
        )
      ) {
        showLoading();
        $.ajax({
          url: "/TurnoEnfermeria/FirmarTurno",
          method: "POST",
          data: {
            turnoId: turnoId,
          },
          success: function (dataResult) {
            hideLoading();
            var data = JSON.parse(dataResult);
            if (data.exitoso) {
              self.turnoFirmado(true);
              self.consultarTurnoEnfermeria();
              mensajeEmergente("Turno de enfermería firmado");
            } else {
              mensajeEmergenteError("Error: " + data.resultado);
            }
          },
          error: function (dataError) {
            hideLoading();
            console.log(dataError);
            mensajeEmergenteError("Error al firmar el turno de enfermería");
          },
        });
      }
    }
  };

  self.agregarNotaEnfermeria = function () {
    var hospId = parseInt($("#HospitalizacionId").val());
    if (!hospId || hospId === 0) {
      mensajeEmergenteError("No se encontró el ID de hospitalización.");
      return;
    }

    var contenido = self.notaDiagnostico();
    if (self.quillEditor && self.quillEditor.root) {
      contenido = self.quillEditor.root.innerHTML;
      self.notaDiagnostico(contenido);
    }

    if (self.editandoNotaEnfermeria()) {
      // Modo edición
      var notaId = self.editNotaEnfermeriaId();
      if (!notaId) {
        mensajeEmergenteError("No se pudo identificar la nota a editar.");
        return;
      }

      showLoading();
      $.ajax({
        url: "/NotaEnfermeria/ActualizarNotaEnfermeria",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({
          Id: notaId,
          Diagnostico: contenido,
          HospitalizacionId: hospId
        }),
        success: function (dataResult) {
          hideLoading();
          var data = JSON.parse(dataResult);
          if (data.exitoso) {
            self.consultarNotaEnfermeria(); // refrescar lista
            self.limpiarModalNotaEnfermeria();
            $("#mdl-agregar-nota-enfermeria").dialog("close");
            mensajeEmergente("Nota de enfermería actualizada correctamente.");
          } else {
            mensajeEmergenteError(data.resultado || "Error al actualizar.");
          }
        },
        error: function () {
          hideLoading();
          mensajeEmergenteError("Error de conexión al actualizar.");
        }
      });
    } else {
      if (!contenido || contenido.trim() === "") {
        mensajeEmergenteError("La descripción no puede estar vacía.");
        return;
      }

      var turnoId = self.idTurnoEnfermeria();
      if (!turnoId) {
        mensajeEmergenteError("No hay turno de enfermería activo.");
        return;
      }

      var nuevaNota = {
        Evolucion: "",
        Sintomas: "",
        Diagnostico: contenido,
        FechaRegistro: new Date().toISOString(),
        HospitalizacionId: hospId,
        UserId: "",
        TurnoEnfermeriaId: turnoId
      };

      showLoading();
      $.ajax({
        url: "/NotaEnfermeria/Nuevo",
        method: "POST",
        data: nuevaNota,
        success: function (dataResult) {
          hideLoading();
          var data = JSON.parse(dataResult);
          if (data.exitoso) {
            self.consultarNotaEnfermeria();
            self.limpiarModalNotaEnfermeria();
            $("#mdl-agregar-nota-enfermeria").dialog("close");
            mensajeEmergente("Nota de enfermería agregada correctamente.");
          } else {
            mensajeEmergenteError(data.resultado);
          }
        },
        error: function () {
          hideLoading();
          mensajeEmergenteError("Error al agregar la nota.");
        }
      });
    }
  };

  self.consultarNotaEvolucion = function () {
    showLoading();
    $.ajax({
      url: "/NotaEvolucion/ListaNotaEvolucion",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.notasEvolucion(data.resultado);
        } else {
          mensajeEmergenteError(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };

  // Función para limpiar el modal de nota de enfermería
  self.limpiarModalNotaEvolucion = function () {
    self.notaEvolucionNoEvo("");
    self.notaSintomasNoEvo("");
    self.notaDiagnosticoNoEvo("");
    self.notaFechaRegistroNoEvo("");
  };

  // Método para agregar nota de evolucion
  self.agregarNotaEvolucion = function () {
    // debugger;
    var nuevaNota = {
      Evolucion: self.notaEvolucionNoEvo(),
      Sintomas: self.notaSintomasNoEvo(),
      Diagnostico: self.notaDiagnosticoNoEvo(),
      FechaRegistro: new Date().toISOString(),
      HospitalizacionId: $("#HospitalizacionId").val(),
      UserId: "",
    };

    showLoading();
    $.ajax({
      url: "/NotaEvolucion/Nuevo",
      method: "POST",
      data: nuevaNota,
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarNotaEvolucion();
          self.limpiarModalNotaEvolucion(); // Limpiar el formulario

          $("#mdl-agregar-nota-evolucion").dialog("close");
          mensajeEmergente("Nota de evolucion agregada");
        } else {
          mensajeEmergenteError("Error: " + data.resultado);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError("Error al agregar la nota de enfermería");
      },
    });
  };

  self.consultarControlGlucometria = function () {
    showLoading();
    $.ajax({
      url: "/ControlGlucometria/ListaControlGlucometria2",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.exitoso) {
          // Ordenar data.resultado por FechaHora en orden descendente
          let resultadoOrdenado = data.resultado.sort((a, b) => {
            return new Date(b.FechaHora) - new Date(a.FechaHora);
          });

          self.controlesGlucometria(resultadoOrdenado); // Asignar los datos ordenados
        } else {
          mensajeEmergenteError(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };
  // Función para limpiar el modal de control de glucometría
  self.limpiarModalControlGlucometria = function () {
    self.controlFechaRegistro("");
    self.controlGMT("");
    self.controlInsulina("");
    self.controlUnidad("");
    self.controlMedicamento("");
    // self.controlDosis('');
    self.controlFirma("");
  };


  self.agregarControlGlucometria = function () {
    var hospId = parseInt($("#HospitalizacionId").val());
    if (!hospId || hospId === 0) {
      mensajeEmergenteError("No se encontró el ID de hospitalización.");
      return;
    }

    // Validar campos obligatorios
    if (!self.controlGMT() || self.controlGMT().trim() === "") {
      mensajeEmergenteError("El campo GMT es obligatorio");
      return;
    }
    if (!self.controlInsulina() || self.controlInsulina().trim() === "") {
      mensajeEmergenteError("El campo Insulina es obligatorio");
      return;
    }
    if (!self.controlUnidad() || self.controlUnidad().trim() === "") {
      mensajeEmergenteError("El campo Unidades es obligatorio");
      return;
    }
    if (!self.controlMedicamento() || self.controlMedicamento().trim() === "") {
      mensajeEmergenteError("El campo Medicamento es obligatorio");
      return;
    }

    var entityControlGlucometria = {
      FechaHora: new Date().toISOString(),
      GMT: self.controlGMT(),
      Insulina: self.controlInsulina(),
      Unidades: self.controlUnidad(),
      Medicamento: self.controlMedicamento(),
      Dosis: self.controlDosis(),
      Firma: self.controlFirma(),
      HospitalizacionId: hospId,
    };

    showLoading();
    $.ajax({
      url: "/ControlGlucometria/Nuevo",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({ entity: entityControlGlucometria, hospitalizacionId: hospId }),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarControlGlucometria();
          self.limpiarModalControlGlucometria();
          $("#mdl-agregar-control-glucometria").dialog("close");
          mensajeEmergente("Control de glucometría guardado.");
        } else {
          mensajeEmergenteError(data.resultado);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.error("Error ajax:", dataError.responseText);
        mensajeEmergenteError("Error al agregar el control de glucometría");
      },
    });
  };



  self.autorizarControlGlucometria = async function (control) {
    if (!control.Id) {
      mensajeEmergenteError("No se puede autorizar: falta el ID.");
      return;
    }

    showLoading();
    var beginRes = await fetch("/api/WebAuthnVerify/Begin?actionLabel=Autorizar+Control+Glucometria", { method: "POST" });
    if (!beginRes.ok) {
      hideLoading();
      var errData = await beginRes.json().catch(() => ({}));
      mensajeEmergenteError(errData.message || "No se pudo iniciar verificación.");
      return;
    }
    var options = await beginRes.json();
    hideLoading();

    var assertion;
    try {
      assertion = await navigator.credentials.get({
        publicKey: {
          challenge: base64UrlToBuffer(options.challenge),
          timeout: options.timeout ?? 60000,
          rpId: options.rpId,
          userVerification: options.userVerification ?? "required",
          allowCredentials: [],
        },
      });
    } catch (e) {
      mensajeEmergenteError(e.name === "NotAllowedError" ? "Verificación cancelada." : "Error al leer huella.");
      return;
    }

    var huellaPayload = {
      id: assertion.id,
      rawId: bufferToBase64Url(assertion.rawId),
      type: assertion.type,
      response: {
        authenticatorData: bufferToBase64Url(assertion.response.authenticatorData),
        clientDataJSON: bufferToBase64Url(assertion.response.clientDataJSON),
        signature: bufferToBase64Url(assertion.response.signature),
        userHandle: assertion.response.userHandle ? bufferToBase64Url(assertion.response.userHandle) : null,
      },
    };

    showLoading();
    $.ajax({
      url: "/ControlGlucometria/AutorizarControlGlucometria",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        controlGlucometriaId: control.Id,
        huellaPayload: huellaPayload,
      }),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          control.Autorizado = true;
          self.consultarControlGlucometria();
          mensajeEmergente("Control de glucometría autorizado.");
        } else {
          mensajeEmergenteError(data.resultado);
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error al autorizar.");
      },
    });
  };

  //Aplicar control glucometria

  self.aplicarControlGlucometria = function (value) {
    // debugger;
    if (confirm("¿Desea aplicar este control de glucometria?")) {
      showLoading();
      $.ajax({
        url: "/ControlGlucometria/AplicacionControlGlucometria2",
        method: "POST",
        data: {
          id: value.Id,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.exitoso) {
            self.consultarControlGlucometria();
            mensajeEmergente("Control aplicado");
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };

  // Función para consultar Ingesta/Excreta
  self.consultarIngestaExcreta = function () {
    showLoading();
    $.ajax({
      url: "/IngestaExcreta2/ListaIngestaExcreta",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.exitoso) {
          let resultadosConBalance = data.resultado.map(function (item) {
            let totalIngesta = parseFloat(item.TotalIngesta) || 0;
            let excreta = parseFloat(item.Excreta) || 0;
            let balance = totalIngesta - excreta;
            item.balance = balance;

            return item;
          });

          // Ordenar los resultados por FechaRegistro de manera descendente
          resultadosConBalance.sort(function (a, b) {
            return new Date(b.FechaRegistro) - new Date(a.FechaRegistro);
          });
          // console.log(JSON.stringify(resultadosConBalance, null, 2));
          // self.IngestasExcretas(resultadosConBalance)

          function añadirTurno(array) {
            // Crear un nuevo array con los datos actualizados
            const nuevoArray = array.map((item) => {
              let dtFechaRegistro = item.FechaRegistro;
              let dtNumeroTurno;
              let dtNombreTurno;

              // Función para restar un día calendario
              function restarUnDia(fecha) {
                let nuevaFecha = new Date(fecha);
                nuevaFecha.setDate(nuevaFecha.getDate() - 1);
                return nuevaFecha;
              }

              // Convertir el string ISO a un objeto de fecha
              let fechaRegistro = new Date(dtFechaRegistro);

              // Obtener la hora del registro para decidir el turno
              let horaRegistro = fechaRegistro.getHours();
              let diaRegistro = fechaRegistro.getDate();
              let mesRegistro = fechaRegistro.getMonth();
              let anioRegistro = fechaRegistro.getFullYear();

              // Asignación del número de turno y nombre del turno según el rango horario
              if (horaRegistro >= 1 && horaRegistro < 7) {
                dtNumeroTurno = 1;
                dtNombreTurno = `Turno #1 del día ${diaRegistro}/${mesRegistro + 1
                  }/${anioRegistro}`;
              } else if (horaRegistro >= 7 && horaRegistro < 13) {
                dtNumeroTurno = 2;
                dtNombreTurno = `Turno #2 del día ${diaRegistro}/${mesRegistro + 1
                  }/${anioRegistro}`;
              } else if (horaRegistro >= 13 && horaRegistro < 19) {
                dtNumeroTurno = 3;
                dtNombreTurno = `Turno #3 del día ${diaRegistro}/${mesRegistro + 1
                  }/${anioRegistro}`;
              } else if (
                (horaRegistro >= 19 && horaRegistro <= 23) ||
                horaRegistro < 1
              ) {
                dtNumeroTurno = 4;

                // Si la hora de registro está antes de las 01:00, restamos un día calendario
                if (horaRegistro < 1) {
                  let fechaModificada = restarUnDia(fechaRegistro);
                  dtNombreTurno = `Turno #4 del día ${fechaModificada.getDate()}/${fechaModificada.getMonth() + 1
                    }/${fechaModificada.getFullYear()}`;
                } else {
                  dtNombreTurno = `Turno #4 del día ${diaRegistro}/${mesRegistro + 1
                    }/${anioRegistro}`;
                }
              }

              // Retornar un nuevo objeto con las propiedades originales y las nuevas
              return {
                ...item,
                NumeroTurno: dtNumeroTurno,
                NombreTurno: dtNombreTurno,
                esResumenPorIntervalo: false,
                esResumenPorDia: false,
                Autorizado: item.Autorizado !== undefined ? item.Autorizado : false

              };
            });

            return nuevoArray;
          }

          function calcularResumenPorTurno(data) {
            // Función auxiliar para sumar propiedades numéricas
            const sumarPropiedades = (arr, prop) =>
              arr.reduce((acc, obj) => acc + (parseFloat(obj[prop]) || 0), 0);

            // Función auxiliar para ajustar fechas
            const ajustarFecha = (fechaBase, diasSumar) => {
              // Dividir el string para extraer año, mes y día
              const [año, mes, dia] = fechaBase.split("-").map(Number);
              // Crear una fecha local (meses inician en 0)
              const fecha = new Date(año, mes - 1, dia);
              // Sumar días
              fecha.setDate(fecha.getDate() + diasSumar);
              // Formatear la fecha de salida
              const añoFinal = fecha.getFullYear();
              const mesFinal = String(fecha.getMonth() + 1).padStart(2, "0");
              const diaFinal = String(fecha.getDate()).padStart(2, "0");
              return `${añoFinal}-${mesFinal}-${diaFinal}`;
            };

            // Agrupar objetos por NombreTurno
            const agrupadosPorTurno = data.reduce((acc, obj) => {
              if (!acc[obj.NombreTurno]) acc[obj.NombreTurno] = [];
              acc[obj.NombreTurno].push(obj);
              return acc;
            }, {});

            // Crear los objetos resumen
            const objetosResumen = Object.entries(agrupadosPorTurno).map(
              ([turno, objetos]) => {
                // Linea 1120
                const resumen = {
                  Id: null,
                  FechaRegistro: null,
                  IngestaIV: sumarPropiedades(objetos, "IngestaIV"),
                  IngestaIV2: sumarPropiedades(objetos, "IngestaIV2"),
                  IngestaIV3: sumarPropiedades(objetos, "IngestaIV3"),
                  IngestaIV4: sumarPropiedades(objetos, "IngestaIV4"),
                  IngestaIV5: sumarPropiedades(objetos, "IngestaIV5"),
                  IngestaIV6: sumarPropiedades(objetos, "IngestaIV6"),
                  IngestaPO: sumarPropiedades(objetos, "IngestaPO"),
                  TotalIngesta: sumarPropiedades(objetos, "TotalIngesta"),
                  Excreta: sumarPropiedades(objetos, "Excreta"),
                  Orina: sumarPropiedades(objetos, "Orina"),
                  Heces: sumarPropiedades(objetos, "Heces"),
                  Vomito: sumarPropiedades(objetos, "Vomito"),
                  Sudoracion: sumarPropiedades(objetos, "Sudoracion"),
                  Drenajes: sumarPropiedades(objetos, "Drenajes"),
                  OtrosLiquidos: sumarPropiedades(objetos, "OtrosLiquidos"),
                  CuantasHoras: sumarPropiedades(objetos, "CuantasHoras"),
                  UserId: null,
                  Enfermeria: null,
                  HospitalizacionId: 101,
                  balance: sumarPropiedades(objetos, "balance"),
                  NumeroTurno: objetos[0].NumeroTurno,
                  NombreTurno: turno,
                  esResumenPorIntervalo: true,
                  esResumenPorDia: false,
                  Autorizado: false,

                };

                // Obtener fecha base del NombreTurno
                const match = turno.match(/\d{1,2}\/\d{1,2}\/\d{4}/);
                if (!match) {
                  throw new Error(
                    `Formato de fecha no válido en NombreTurno: ${turno}`,
                  );
                }
                const fechaBase = match[0].split("/").reverse().join("-");

                if (resumen.NumeroTurno === 4) {
                  // Sumar un día a la fecha base y asignar la hora fija
                  resumen.FechaRegistro = `${ajustarFecha(
                    fechaBase,
                    1,
                  )} 00:58:58`;
                } else {
                  // Para otros turnos, calcular hora final según turno
                  const horaFin =
                    resumen.NumeroTurno === 3
                      ? "18:59:59"
                      : resumen.NumeroTurno === 2
                        ? "12:59:59"
                        : "06:59:59";
                  resumen.FechaRegistro = `${fechaBase} ${horaFin}`;
                }

                return resumen;
              },
            );
            // console.log(JSON.stringify(objetosResumen, null, 2))
            // Devolver el array original junto con los objetos resumen
            return [...data, ...objetosResumen];
          }

          function calcularResumenPorDia(data) {
            // Función auxiliar para sumar propiedades numéricas
            const sumarPropiedades = (arr, prop) =>
              arr.reduce((acc, obj) => acc + (parseFloat(obj[prop]) || 0), 0);

            // Función auxiliar para ajustar fechas
            const ajustarFecha = (fechaBase, diasSumar) => {
              // Si la fecha está en formato 'DD/MM/YYYY', convertirla a 'YYYY-MM-DD'
              const [dia, mes, año] = fechaBase.split("/").map(Number); // Cambiar el orden de los elementos
              const fecha = new Date(año, mes - 1, dia); // Meses inician en 0, así que restamos 1 al mes
              // Sumar días
              fecha.setDate(fecha.getDate() + diasSumar);
              // Formatear la fecha de salida
              const añoFinal = fecha.getFullYear();
              const mesFinal = String(fecha.getMonth() + 1).padStart(2, "0");
              const diaFinal = String(fecha.getDate()).padStart(2, "0");
              return `${añoFinal}-${mesFinal}-${diaFinal}`;
            };

            // Agrupar objetos resumen de turno por día (sin importar el número de turno)
            const agrupadosPorDia = data
              .filter((obj) => obj.esResumenPorIntervalo) // Solo incluir resúmenes por turno
              .reduce((acc, obj) => {
                // Extraer solo la fecha y el día del turno, sin considerar el número del turno
                const fechaDia = obj.FechaRegistro.split(" ")[0]; // Extraer solo la fecha
                const nombreTurnoDia = obj.NombreTurno.split(" del día ")[1]; // Obtener solo la fecha sin el número de turno

                // Usamos el nombre de turno (que incluye la fecha sin el número del turno) para agrupar
                if (!acc[nombreTurnoDia]) acc[nombreTurnoDia] = [];
                acc[nombreTurnoDia].push(obj);
                return acc;
              }, {});

            // Crear los objetos resumen diarios
            const objetosResumenDia = Object.entries(agrupadosPorDia).map(
              ([dia, objetos]) => {
                // Calcular la fecha de registro como el día siguiente
                const newFechaRegistro = `${ajustarFecha(dia, 1)} 00:59:59`;
                const nombreTurno = `Resumen del día ${ajustarFecha(dia, 0)}`;
                return {
                  Id: null,
                  FechaRegistro: newFechaRegistro,
                  IngestaIV: sumarPropiedades(objetos, "IngestaIV"),
                  IngestaIV2: sumarPropiedades(objetos, "IngestaIV2"),
                  IngestaIV3: sumarPropiedades(objetos, "IngestaIV3"),
                  IngestaIV4: sumarPropiedades(objetos, "IngestaIV4"),
                  IngestaIV5: sumarPropiedades(objetos, "IngestaIV5"),
                  IngestaIV6: sumarPropiedades(objetos, "IngestaIV6"),
                  IngestaPO: sumarPropiedades(objetos, "IngestaPO"),
                  TotalIngesta: sumarPropiedades(objetos, "TotalIngesta"),
                  Excreta: sumarPropiedades(objetos, "Excreta"),
                  Orina: sumarPropiedades(objetos, "Orina"),
                  Heces: sumarPropiedades(objetos, "Heces"),
                  Vomito: sumarPropiedades(objetos, "Vomito"),
                  Sudoracion: sumarPropiedades(objetos, "Sudoracion"),
                  Drenajes: sumarPropiedades(objetos, "Drenajes"),
                  OtrosLiquidos: sumarPropiedades(objetos, "OtrosLiquidos"),
                  CuantasHoras: sumarPropiedades(objetos, "CuantasHoras"),
                  UserId: null,
                  Enfermeria: null,
                  HospitalizacionId: objetos[0].HospitalizacionId, // Tomar el ID de hospitalización
                  balance: sumarPropiedades(objetos, "balance"),
                  NumeroTurno: null,
                  NombreTurno: nombreTurno,
                  esResumenPorIntervalo: false,
                  esResumenPorDia: true,
                  Autorizado: false,

                };
              },
            );

            // Retornar el array original con los nuevos resúmenes diarios
            return [...data, ...objetosResumenDia];
          }

          const itemsConTurno = añadirTurno(resultadosConBalance);
          const itemsConResumenPorTurno =
            calcularResumenPorTurno(itemsConTurno); // Linea 1245
          const itemsConResumenPorDia = calcularResumenPorDia(
            itemsConResumenPorTurno,
          );

          itemsConResumenPorDia.sort(function (a, b) {
            return new Date(b.FechaRegistro) - new Date(a.FechaRegistro);
          });

          // console.log(JSON.stringify(itemsConResumenPorDia, null, 2));
          self.IngestasExcretas(itemsConResumenPorDia);
        } else {
          alert(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert(dataError);
      },
    });
  };

  self.totalIngesta = ko.computed(function () {
    return (
      (parseFloat(self.ingestaIV()) || 0) +
      (parseFloat(self.ingestaIV2()) || 0) +
      (parseFloat(self.ingestaIV3()) || 0) +
      (parseFloat(self.ingestaIV4()) || 0) +
      (parseFloat(self.ingestaIV5()) || 0) +
      (parseFloat(self.ingestaIV6()) || 0) +
      (parseFloat(self.ingestaPO()) || 0)
    );
  });
  self.excreta = ko.computed(function () {
    return (
      (parseFloat(self.orina()) || 0) +
      (parseFloat(self.heces()) || 0) +
      (parseFloat(self.vomito()) || 0) +
      (parseFloat(self.sudoracion()) || 0) +
      (parseFloat(self.drenajes()) || 0) +
      (parseFloat(self.otrosLiquidos()) || 0)
    );
  });
  self.limpiarModalIngestaExcreta = function () {
    self.ingestaIV("");
    self.ingestaIV2("");
    self.ingestaIV3("");
    self.ingestaIV4("");
    self.ingestaIV5("");
    self.ingestaIV6("");
    self.ingestaPO("");
    self.orina("");
    self.heces("");
    self.vomito("");
    self.sudoracion("");
    self.drenajes("");
    self.otrosLiquidos("");
    self.cuantasHoras("");
    self.ingestaExcretaFechaRegistro("");
    console.log("Siii???");
  };
  // Función para agregar Ingesta/Excreta
  // self.agregarIngestaExcreta = function () {
  //   debugger;
  //   var nuevaIngestaExcreta = {
  //     IngestaIV: self.ingestaIV(),
  //     IngestaIV2: self.ingestaIV2(),
  //     IngestaIV3: self.ingestaIV3(),
  //     IngestaIV4: self.ingestaIV4(),
  //     IngestaIV5: self.ingestaIV5(),
  //     IngestaIV6: self.ingestaIV6(),
  //     IngestaPO: self.ingestaPO(),
  //     TotalIngesta: self.totalIngesta(),
  //     Excreta: self.excreta(),
  //     Orina: self.orina(),
  //     Heces: self.heces(),
  //     Vomito: self.vomito(),
  //     Sudoracion: self.sudoracion(),
  //     Drenajes: self.drenajes(),
  //     OtrosLiquidos: self.otrosLiquidos(),
  //     CuantasHoras: self.cuantasHoras(),
  //     FechaRegistro: new Date().toISOString(),
  //     HospitalizacionId: $("#HospitalizacionId").val(),
  //   };
  //   console.log(nuevaIngestaExcreta);

  //   showLoading();
  //   $.ajax({
  //     url: "/IngestaExcreta2/Nuevo",
  //     method: "POST",
  //     data: nuevaIngestaExcreta,
  //     success: function (dataResult) {
  //       hideLoading();
  //       var data = JSON.parse(dataResult);
  //       // debugger;
  //       if (data.exitoso) {
  //         self.consultarIngestaExcreta();
  //         self.limpiarModalIngestaExcreta(); // Limpiar el formulario

  //         $("#mdl-agregar-increta-excreta").dialog("close");
  //         mensajeEmergente("Ingesta/Excreta agregada exitosamente");
  //       } else {
  //         alert("Error: " + data.resultado);
  //       }
  //     },
  //     error: function (dataError) {
  //       hideLoading();
  //       console.log(dataError);
  //       alert("Error al agregar Ingesta/Excreta");
  //     },
  //   });
  // };

  self.agregarIngestaExcreta = function () {
    var hospId = parseInt($("#HospitalizacionId").val());
    if (!hospId || hospId === 0) {
      mensajeEmergenteError("No se encontró el ID de hospitalización.");
      return;
    }

    var nuevaIngestaExcreta = {
      IngestaIV: self.ingestaIV(),
      IngestaIV2: self.ingestaIV2(),
      IngestaIV3: self.ingestaIV3(),
      IngestaIV4: self.ingestaIV4(),
      IngestaIV5: self.ingestaIV5(),
      IngestaIV6: self.ingestaIV6(),
      IngestaPO: self.ingestaPO(),
      TotalIngesta: self.totalIngesta()?.toString(),
      Excreta: self.excreta()?.toString(),
      Orina: self.orina(),
      Heces: self.heces(),
      Vomito: self.vomito(),
      Sudoracion: self.sudoracion(),
      Drenajes: self.drenajes(),
      OtrosLiquidos: self.otrosLiquidos(),
      CuantasHoras: self.cuantasHoras(),
      FechaRegistro: new Date().toISOString(),
      HospitalizacionId: hospId,
    };

    showLoading();
    $.ajax({
      url: "/IngestaExcreta2/Nuevo",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({ entity: nuevaIngestaExcreta, hospitalizacionId: hospId }),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarIngestaExcreta();
          self.limpiarModalIngestaExcreta();
          $("#mdl-agregar-increta-excreta").dialog("close");
          mensajeEmergente("Ingesta/Excreta agregada exitosamente.");
        } else {
          mensajeEmergenteError(data.resultado);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.error("Error ajax:", dataError.responseText);
        mensajeEmergenteError("Error al agregar Ingesta/Excreta");
      },
    });
  };


  self.autorizarIngestaExcreta = async function (ingesta) {
    if (!ingesta.Id) {
      mensajeEmergenteError("No se puede autorizar: falta el ID del registro.");
      return;
    }

    // Paso 1: Pedir challenge
    showLoading();
    var beginRes = await fetch("/api/WebAuthnVerify/Begin?actionLabel=Autorizar+Ingesta/Excreta", {
      method: "POST"
    });

    if (!beginRes.ok) {
      hideLoading();
      var errData = await beginRes.json().catch(() => ({}));
      mensajeEmergenteError(errData.message || "No se pudo iniciar la verificación de huella.");
      return;
    }

    var options = await beginRes.json();
    hideLoading();

    // Paso 2: Solicitar huella
    var assertion;
    try {
      assertion = await navigator.credentials.get({
        publicKey: {
          challenge: base64UrlToBuffer(options.challenge),
          timeout: options.timeout ?? 60000,
          rpId: options.rpId,
          userVerification: options.userVerification ?? "required",
          allowCredentials: [],
        },
      });
    } catch (e) {
      if (e.name === "NotAllowedError") {
        mensajeEmergenteError("Verificación cancelada.");
      } else {
        mensajeEmergenteError("El dispositivo no pudo leer la huella.");
      }
      return;
    }

    // Paso 3: Serializar
    var huellaPayload = {
      id: assertion.id,
      rawId: bufferToBase64Url(assertion.rawId),
      type: assertion.type,
      response: {
        authenticatorData: bufferToBase64Url(assertion.response.authenticatorData),
        clientDataJSON: bufferToBase64Url(assertion.response.clientDataJSON),
        signature: bufferToBase64Url(assertion.response.signature),
        userHandle: assertion.response.userHandle ? bufferToBase64Url(assertion.response.userHandle) : null,
      },
    };

    // Paso 4: Enviar al backend
    showLoading();
    $.ajax({
      url: "/IngestaExcreta2/AutorizarIngestaExcreta",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        ingestaExcretaId: ingesta.Id,
        huellaPayload: huellaPayload,
      }),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          // Actualizar el estado localmente o refrescar toda la lista
          ingesta.Autorizado = true;
          self.consultarIngestaExcreta(); // opcional: recargar toda la tabla
          mensajeEmergente("Registro autorizado correctamente.");
        } else {
          mensajeEmergenteError(data.resultado);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.error("Error ajax:", dataError.responseText);
        mensajeEmergenteError("Error al autorizar el registro.");
      },
    });
  };

  //Ingesta Excreta
  self.consultarInfoIngesta = function (ingExcId) {
    showLoading();
    $.ajax({
      url: "/InfoIngesta/ListaInfoIngesta",
      method: "POST",
      data: {
        ingestaExcretaId: ingExcId,
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.infoIngestas(data.resultado);
          if (self.infoIngestas().length > 0) {
            self.existInfoIngesta(true);
          } else {
            self.existInfoIngesta(false);
          }
        } else {
          mensajeEmergenteError(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };

  self.verModalInfoIngesta = function (ingExcId) {
    console.log(ingExcId);
    self.idIngestaExcretaForInfo(ingExcId);
    self.consultarInfoIngesta(ingExcId);

    $("#mdl-info-ingesta").dialog({
      width: 1000,
    });
  };

  self.limpiarModalInfoIngesta = function () {
    self.infoIngestaIV1("");
    self.infoIngestaIV2("");
    self.infoIngestaIV3("");
    self.infoIngestaIV4("");
    self.infoIngestaIV5("");
    self.infoIngestaIV6("");
    self.infoIngestaPO("");
  };

  self.agregarInfoIngesta = function () {
    if (self.existInfoIngesta() === true) {
      self.limpiarModalInfoIngesta();
      return alert(
        "La información de ingesta ya ha sido registrada para esta ingesta/excreta.",
      );
    }

    if (
      confirm(
        "Una vez guardada esta info, no podrás editar ni crear otra información de ingesta, para esta ingesta y excreta. ¿Deseas continuar?",
      )
    ) {
      if (confirm("¿Estás seguro de guardar esta información de ingesta? ")) {
        var nuevaInfo = {
          IngestaExcreta2Id: self.idIngestaExcretaForInfo(),
          InfoIngestaIV1: self.infoIngestaIV1(),
          InfoIngestaIV2: self.infoIngestaIV2(),
          InfoIngestaIV3: self.infoIngestaIV3(),
          InfoIngestaIV4: self.infoIngestaIV4(),
          InfoIngestaIV5: self.infoIngestaIV5(),
          InfoIngestaIV6: self.infoIngestaIV6(),
          InfoIngestaPO: self.infoIngestaPO(),
        };

        showLoading();
        $.ajax({
          url: "/InfoIngesta/Nuevo",
          method: "POST",
          data: nuevaInfo,
          success: function (dataResult) {
            hideLoading();
            var data = JSON.parse(dataResult);
            if (data.exitoso) {
              self.limpiarModalInfoIngesta();
              self.consultarInfoIngesta(self.idIngestaExcretaForInfo());
              mensajeEmergente("Información de ingesta agregada");
            } else {
              mensajeEmergenteError("Error: " + data.resultado);
            }
          },
          error: function (dataError) {
            hideLoading();
            console.log(dataError);
            mensajeEmergenteError("Error al agregar información de ingesta");
          },
        });
      }
    }
  };

  self.consultarServiciosExistentes = function () {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarServiciosExistentes",
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.serviciosExistentes(data.Resultado);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert(dataError);
      },
    });
  };
  //self.consultarMedicamentosExistentes = function () {
  //    let textoCargando = $("#texto-cargando-medicamentos-existentes");
  //    let textoError = $("#texto-error-consultar-medicamentos-existentes");
  //    textoError.hide();
  //    textoCargando.show();
  //    $.ajax({
  //        url: "/Hospitalizacion/ConsultarMedicamentosExistentes",
  //        method: "POST",
  //        success: function (dataResult) {
  //            textoCargando.hide();
  //            let data = JSON.parse(dataResult);
  //            if (data.Exitoso) {
  //                textoError.hide();
  //                self.medicamentosExistentes(data.Resultado);
  //            } else {
  //                textoError.show();
  //            }
  //        },
  //        error: function (dataError) {
  //            textoCargando.hide();
  //            textoError.show();
  //            console.log(dataError);
  //        }
  //    });
  //};

  // Consultar medicamentos existentes
  self.consultarMedicamentosExistentes = function () {
    let textoCargando = $("#texto-cargando-medicamentos-existentes");
    let textoError = $("#texto-error-consultar-medicamentos-existentes");
    self.medicamentosExistentes([]);
    textoCargando.show();
    textoError.hide();

    const tipoProductoMap = {
      medicamentos: 1,
      equipos: 3,
      insumos: 2,
    };

    let tipoProductoId = tipoProductoMap[self.switchMedicamentosEquipos()] || 1;

    $.ajax({
      method: "POST",
      url: "/HospitalizacionPaquetes/ConsultarProductosExistentes",
      data: { BodegaId: $("#BodegaId").val() },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.registrosInventario(data.Resultado);

          let productosMap = new Map();

          // Optimización: Iterar una sola vez y filtrar directamente
          self.registrosInventario().forEach((vl) => {
            if (
              vl.TipoProductoId === tipoProductoId &&
              !productosMap.has(vl.ProductoId)
            ) {
              productosMap.set(vl.ProductoId, {
                ProductoId: vl.ProductoId,
                ProductoCodigo: vl.ProductoCodigo,
                ProductoNombre: vl.ProductoNombre,
                TipoProductoId: vl.TipoProductoId,
              });
            }
          });

          // Convertir el mapa en un array y asignarlo a la observación
          self.medicamentosExistentes(Array.from(productosMap.values()));

          textoCargando.hide();
        } else {
          textoCargando.hide();
          textoError.show();
        }
      },
      error: function (data) {
        textoCargando.show();
        textoError.show();
      },
    });
  };

  // Suscripción para ejecutar la función cuando cambie el observable 'switchMedicamentosEquipos'
  self.switchMedicamentosEquipos.subscribe(function (nuevoValor) {
    self.consultarMedicamentosExistentes(); // Ejecutar la función cada vez que el observable cambie
  });

  self.consultarExamenesExistentes = function () {
    let textoCargando = $("#text-cargando-examenes-existentes");
    let textoError = $("#texto-error-consultar-examenes-existentes");
    textoCargando.show();
    textoError.hide();
    $.ajax({
      url: "/Hospitalizacion/ConsultarExamenesExistentes",
      method: "POST",
      success: function (dataResult) {
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          // console.log(data);
          self.examenesExistentes(data.Resultado);
          textoCargando.hide();
        } else {
          textoCargando.hide();
          textoError.show();
        }
      },
      error: function (dataError) {
        textoCargando.hide();
        console.log(dataError);
        textoError.show();
      },
    });
  };
  self.consultarHabitacionesDisponibles = function () {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarHabitacionesDisponiblesCambio",
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.habitacionesDisponiblesCambio(data.Resultado);
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };
  self.consultarDatosExamenFisico = function () {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarDatosExamenFisico",
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          let datos = data.Resultado;

          // Encuentra el índice del elemento con DatoExamenFisicoHospId: 11
          let index = datos.findIndex(
            (item) => item.DatoExamenFisicoHospId === 11,
          );
          if (index !== -1) {
            // Extrae el elemento con DatoExamenFisicoHospId: 11
            let [elemento] = datos.splice(index, 1);

            // Inserta el elemento en la posición 4 (posición 5 visualmente)
            datos.splice(4, 0, elemento);
          }

          // Encuentra el índice del objeto "Presion arterial brazo izquierdo"
          let indexIzquierdo = datos.findIndex(
            (item) => item.NombreDato === "Presion arterial brazo izquierdo",
          );
          if (indexIzquierdo !== -1) {
            // Modifica el nombre a "Presion arterial"
            datos[indexIzquierdo].NombreDato = "Presion arterial";
          }

          // Encuentra el índice del objeto "Presion arterial brazo derecho"
          let indexDerecho = datos.findIndex(
            (item) => item.NombreDato === "Presion arterial brazo derecho",
          );
          if (indexDerecho !== -1) {
            // Elimina el objeto del array
            datos.splice(indexDerecho, 1);
          }

          // console.log(JSON.stringify(datos, null, 2));
          // Actualiza el observable con los datos modificados
          self.datosExamenFisicoAgregar(datos);
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };

  self.consultarRegistrosHospitalizacion = function () {
    showLoading();
    self.registrosHospitalizacion([]);
    $.ajax({
      url: "/Hospitalizacion/ConsultarRegistrosHospitalizacion",
      method: "POST",
      data: {
        pacienteId: $("#PacienteId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          $(data.Resultado).each(function (idx, vl) {
            vl.FechaInicio = ko.observable(vl.FechaInicio);
            vl.FechaFin = ko.observable(vl.FechaFin);
            self.registrosHospitalizacion.push(vl);
          });
          $(self.registrosHospitalizacion()).each(function (idx, vl) {
            if (vl.Id == $("#HospitalizacionId").val()) {
              self.periodo(vl.FechaInicio() + " - " + vl.FechaFin());
              self.habitacionId(vl.HabitacionId);

            }
          });
          self.actualizarTotales();
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };
  // self.consultarPaquetesHospitalizacion = function () {
  //   let textoCargando = $("#texto-cargando-paquetes-hospitalizacion");
  //   let textoError = $("#texto-error-carga-paquetes-hospitalizacion");
  //   textoCargando.show();
  //   textoError.hide();
  //   $.ajax({
  //     url: "/Hospitalizacion/ConsultarPaquetesHospitalizacion",
  //     method: "POST",
  //     data: {
  //       hospitalizacionId: $("#HospitalizacionId").val(),
  //     },
  //     success: function (dataResult) {
  //       let data = JSON.parse(dataResult);
  //       if (data.Exitoso) {
  //         self.paquetesHospitalizacion(data.Resultado);
  //         self.actualizarTotales();
  //         textoError.hide();
  //         textoCargando.hide();
  //       } else {
  //         textoCargando.hide();
  //         textoError.show();
  //       }
  //     },
  //     error: function (dataError) {
  //       textoCargando.hide();
  //       console.log(dataError);
  //       textoError.show();
  //     },
  //   });
  // };

  // Total de productos dentro de todos los paquetes
  self.totalProductosEnPaquetes = ko.computed(function () {
    let total = 0;
    ko.utils.arrayForEach(self.paquetesHospitalizacion(), function (paquete) {
      if (paquete.Productos && paquete.Productos.length) {
        total += paquete.Productos.length;
      }
    });
    return total;
  });

  // Total de servicios dentro de todos los paquetes
  self.totalServiciosEnPaquetes = ko.computed(function () {
    let total = 0;
    ko.utils.arrayForEach(self.paquetesHospitalizacion(), function (paquete) {
      if (paquete.Servicios && paquete.Servicios.length) {
        total += paquete.Servicios.length;
      }
    });
    return total;
  });

  // Total de laboratorios dentro de todos los paquetes
  self.totalLaboratoriosEnPaquetes = ko.computed(function () {
    let total = 0;
    ko.utils.arrayForEach(self.paquetesHospitalizacion(), function (paquete) {
      if (paquete.Laboratorios && paquete.Laboratorios.length) {
        total += paquete.Laboratorios.length;
      }
    });
    return total;
  });


  self.consultarPaquetesHospitalizacion = function () {
    let textoCargando = $("#texto-cargando-paquetes-hospitalizacion");
    let textoError = $("#texto-error-carga-paquetes-hospitalizacion");
    textoCargando.show();
    textoError.hide();

    $.ajax({
      url: "/Hospitalizacion/ConsultarPaquetesHospitalizacion",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          let paquetesNormalizados = [];

          $(data.Resultado).each(function (idx, item) {
            let paqueteNormalizado = {
              Id: item.Id,
              FechaHora: item.FechaHora || item.FechaYHora || moment().format('DD/MM/YYYY hh:mm:ss A'),
              Codigo: item.Codigo || item.PaqueteCodigo || 'N/A',
              Nombre: item.Nombre || item.PaqueteNombre || 'Sin nombre',
              Precio: item.Precio || item.Valor || 0,
              // ── Sublistas: deben incluirse para que los tabs de
              //    Productos / Servicios / Laboratorios funcionen
              Servicios: item.Servicios || [],
              Productos: item.Productos || [],
              Laboratorios: item.Laboratorios || [],
              Dietas: item.Dietas || []
            };

            paquetesNormalizados.push(paqueteNormalizado);
          });

          self.paquetesHospitalizacion([]);
          self.paquetesHospitalizacion(paquetesNormalizados);
          self.paquetesHospitalizacion.valueHasMutated();
          self.actualizarTotales();

          textoError.hide();
          textoCargando.hide();
        } else {
          textoCargando.hide();
          textoError.show();
        }
      },
      error: function (dataError) {
        textoCargando.hide();
        console.log(dataError);
        textoError.show();
      },
    });
  };


  self.consultarServiciosHospitalizacion = function () {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarServiciosHospitalizacion",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.serviciosHospitalizacion(data.Resultado);
          self.actualizarTotales();
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };
  self.consultarMedicamentosHospitalizacion = function () {
    let textoCargando = $("#texto-cargando-medicamentos-agregados");
    let textoError = $("#texto-error-consultar-medicamentos-agregados");
    textoCargando.show();
    textoError.hide();
    $.ajax({
      url: "/Hospitalizacion/ConsultarMedicamentosHospitalizacion",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        textoCargando.hide();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          $("#texto-error-consultar-medicamentos").hide();
          self.medicamentosHospitalizacion(data.Resultado);
          // Solo recalcular si ya terminó consultarInsumosDirectosAplicados
          if (self._acumuladoInsumosDirectosListo) {
            self.actualizarTotales();
          }
        } else {
          textoError.show();
        }
      },
      error: function (dataError) {
        textoCargando.hide();
        console.log(dataError);
        textoError.show();
      },
    });
  };
  self.consultarProductosAplicacionHospitalizacion = function () {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarProductosAplicacionHospitalizacion",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.productosAplicacionHospitalizacion(data.Resultado);
          self.actualizarTotales();
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };

  self.consultarInsumosDirectosAplicados = function (subtotalAplicado) {
    if (typeof subtotalAplicado === 'number' && subtotalAplicado > 0) {
      self._acumuladoInsumosDirectos = (self._acumuladoInsumosDirectos || 0) + subtotalAplicado;
      self.actualizarTotales();
      return;
    }
    $.ajax({
      url: "/Hospitalizacion/ConsultarTotalInsumosDirectosAplicados",
      method: "POST",
      data: { hospitalizacionId: $("#HospitalizacionId").val() },
      success: function (resp) {
        const data = (typeof resp === 'string') ? JSON.parse(resp) : resp;
        self._acumuladoInsumosDirectos = data.Exitoso ? (data.Total || 0) : 0;
        self._acumuladoInsumosDirectosListo = true;
        self.actualizarTotales();
      },
      error: function () {
        self._acumuladoInsumosDirectos = 0;
        self._acumuladoInsumosDirectosListo = true;
        self.actualizarTotales();
      }
    });
  };

  self.consultarExamenesHospitalizacion = function (force) {
    force = force === true;

    let textoCargando = $("#texto-cargando-examenes-hospitalizacion");
    let textoError = $("#texto-error-consultar-examenes-hospitalizacion");

    if (self.examenesComplementariosCargando) {
      console.warn("⚠ [VM] consultarExamenesHospitalizacion ignorado: ya está cargando");
      return;
    }

    if (force) {
      self.examenesComplementariosCargados = false;
    }

    if (self.examenesComplementariosCargados && !force) {
      console.warn("⚠ [VM] consultarExamenesHospitalizacion ignorado: ya está cargado (use force=true para recargar)");
      return;
    }

    console.log("=================================================");
    console.log("🧪 [VM] consultarExamenesHospitalizacion INVOCADO");
    console.log("➡ hospitalizacionId:", $("#HospitalizacionId").val());
    console.log("➡ force:", force);
    console.log("=================================================");

    self.examenesComplementariosCargando = true;

    textoCargando.show();
    textoError.hide();

    $.ajax({
      url: "/Hospitalizacion/ConsultarExamenesHospitalizacion",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        textoCargando.hide();

        console.log("🧪 [VM] Respuesta RAW del servidor:", dataResult);

        let data;
        try {
          data = typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        } catch (e) {
          console.error("❌ [VM] Error al parsear JSON en ConsultarExamenesHospitalizacion");
          console.error(e);
          console.error("RAW:", dataResult);
          textoError.show();
          return;
        }

        console.log("🧪 [VM] Respuesta PARSEADA:", data);
        console.log("🧪 [VM] data.Exitoso:", data && data.Exitoso);

        if (data && data.Exitoso) {
          let resultado = Array.isArray(data.Resultado) ? data.Resultado : [];

          console.log("✅ [VM] Cantidad de exámenes recibidos:", resultado.length);
          if (resultado.length > 0) console.log("✅ [VM] Primer examen:", resultado[0]);

          // Normalizar propiedades para que coincidan con la vista
          resultado = resultado.map(function (x) {
            const precioRaw = x.Precio ?? x.PrecioValor ?? x.Valor ?? 0;

            let precioNum = 0;
            if (typeof precioRaw === "number") {
              precioNum = precioRaw;
            } else if (typeof precioRaw === "string") {
              const normalized = precioRaw.replace(",", ".");
              const parsed = parseFloat(normalized);
              precioNum = isNaN(parsed) ? 0 : parsed;
            }

            return {
              ...x,
              FechaHora: x.FechaHora ?? x.FechaYHora ?? x.FechaHoraTexto ?? x.Fecha ?? null,
              Nombre: x.Nombre ?? x.NombreExamen ?? x.ExamenNombre ?? null,
              Precio: precioNum,
            };
          });

          // Asignación al observable
          self.examenesHospitalizacion(resultado);

          console.log("✅ [VM] Observable examenesHospitalizacion().length:", self.examenesHospitalizacion().length);

          // Recalcular totales
          self.actualizarTotales();

          // FIX: Solo marcar como cargado si la respuesta fue exitosa
          self.examenesComplementariosCargados = true;

        } else {
          console.warn("⚠ [VM] data.Exitoso = false o data inválida:", data);
          // FIX: En caso de error NO marcar como cargado para permitir reintentos
          self.examenesComplementariosCargados = false;
          textoError.show();
        }
      },
      error: function (dataError) {
        textoCargando.hide();
        console.error("❌ [VM] Error AJAX en ConsultarExamenesHospitalizacion");
        console.error(dataError);
        // FIX: En caso de error de red tampoco marcar como cargado
        self.examenesComplementariosCargados = false;
        textoError.show();
      },
      complete: function () {
        // Liberar lock siempre, independientemente del resultado
        self.examenesComplementariosCargando = false;
      },
    });
  };

  self.consultarExamenesFisicosHospitalizacion = function () {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarExamenesFisicosHospitalizacion",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          // Transformar los resultados
          let resultadosTransformados = data.Resultado.map((item) => {
            // Convertir Datos de string a objeto JSON
            let datos = JSON.parse(item.Datos);

            // Crear un objeto base con FechaHora, Persona y Observaciones
            let nuevoObjeto = {
              Id: item.Id,
              FechaHora: item.FechaHora,
              Persona: item.Persona,
              Observaciones: item.Observaciones,
              Autorizado: item.Autorizado,
            };

            // Agregar cada propiedad de Datos al nuevo objeto sin espacios ni paréntesis
            datos.forEach((dato) => {
              let nombreLimpio = dato.Nombre.replace(/[\s()]+/g, ""); // Eliminar espacios y paréntesis
              nuevoObjeto[nombreLimpio] = dato.Valor;
            });

            // Retornar el nuevo objeto transformado
            return nuevoObjeto;
          });

          console.log(resultadosTransformados); // Ver el resultado transformado
          self.examenesFisicosHospitalizacion(resultadosTransformados);
          self.actualizarTotales();
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };
  self.consultarDepositosHospitalizacion = function () {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarDepositosHospitalizacion",
      method: "POST",
      data: {
        cuentaId: $("#CuentaId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.depositosHospitalizacion(data.Resultado);
          self.actualizarTotales();
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };

  self.agregarServicio = function () {
    if (
      !self.cantidadServicioAgregar() ||
      self.cantidadServicioAgregar().trim() == ""
    ) {
      alert("Proporcione una cantidad para el servicio");
      return false;
    }
    if (!self.valorServicioSeleccionado()) {
      alert("Seleccione un precio para el servicio");
      return false;
    }
    if (confirm("¿Desea agregar este servicio?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/AgregarServicio",
        method: "POST",
        data: {
          HospitalizacionId: $("#HospitalizacionId").val(),
          ServicioId: self.servicioAgregarSeleccionado().ServicioId,
          Cantidad: self.cantidadServicioAgregar(),
          PrecioServicioId: self.valorServicioSeleccionado().PrecioServicioId, // Pasar el ID del precio seleccionado
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarServiciosHospitalizacion();
            self.cantidadServicioAgregar(0);
            mensajeEmergente("Servicio agregado");
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }

    var servicio = self.servicioAgregarSeleccionado();
    var cantidad = self.cantidadServicioAgregar();
    if (servicio && servicio.Nombre && cantidad > 0) {
      var entry = servicio.Nombre + " x " + cantidad;
      // self.concatenarOrdenMedicaTemp(entry);  // Almacena en el array temporal
    } else {
      alert("Seleccione un servicio y proporcione una cantidad válida.");
    }
  };

  //Consultar precioServicio
  // Evento cuando se selecciona un servicio
  self.servicioAgregarSeleccionado.subscribe(function (value) {
    if (value) {
      self.consultarPrecioServicio(value.ServicioId);
    }
  });

  // Método para consultar el precio del servicio
  // self.consultarPrecioServicio = function (servicioId) {
  //     showLoading();
  //     $.ajax({
  //         url: "/Hospitalizacion/ConsultarPrecioServicio",
  //         method: "POST",
  //         data: {
  //             servicioId: servicioId,
  //             codigoSeguro: $("#CodigoSeguro").val()
  //         },
  //         success: function (dataResult) {
  //             hideLoading();
  //             let data = JSON.parse(dataResult);
  //             if (data.Exitoso) {
  //                 self.valoresServicioAgregar(data.Resultado[0]);
  //                 self.valorServicioSeleccionado(null); // Resetea el precio seleccionado
  //             } else {
  //                 mensajeEmergenteError(data.Mensaje);
  //             }
  //         },
  //         error: function (dataError) {
  //             hideLoading();
  //             console.log(dataError);
  //             mensajeEmergenteError(dataError);
  //         }
  //     });
  // };
  self.consultarPrecioServicio = function (servicioId) {
    // Siempre limpiamos estado previo para evitar “valores fantasma”
    self.valoresServicioAgregar([]);
    self.valorServicioSeleccionado(null);

    var codigoSeguro = $("#CodigoSeguro").val();

    // Trazabilidad (no rompe nada)
    console.log(
      "[Servicios] consultarPrecioServicio -> servicioId:",
      servicioId,
      "codigoSeguro:",
      codigoSeguro,
    );

    $.ajax({
      url: "/Hospitalizacion/ConsultarPrecioServicio",
      type: "POST",
      dataType: "json",
      data: { servicioId: servicioId, codigoSeguro: codigoSeguro },
      success: function (data) {
        // Trazabilidad
        console.log("[Servicios] Respuesta ConsultarPrecioServicio:", data);

        if (!data || data.Exitoso !== true) {
          console.warn("[Servicios] ConsultarPrecioServicio no exitoso:", data);
          return;
        }

        // Normalizamos resultado para evitar errores si viene null/undefined
        var precios = Array.isArray(data.Resultado) ? data.Resultado : [];

        // Asignar lista completa (CORRECTO para observableArray)
        self.valoresServicioAgregar(precios);

        // Auto-selección del primer precio (cumple el objetivo)
        if (precios.length > 0) {
          self.valorServicioSeleccionado(precios[0]);
          console.log("[Servicios] Precio autoseleccionado:", precios[0]);
        } else {
          console.warn(
            "[Servicios] No hay precios para este servicio/seguro. servicioId:",
            servicioId,
            "codigoSeguro:",
            codigoSeguro,
          );
        }

        // Si Select2 está inicializado, a veces necesita refresh visual
        // (No afecta KO, solo la UI)
        try {
          $(".select2bs4").trigger("change.select2");
        } catch (e) {
          // Silencioso: no queremos romper flujo por UI
        }
      },
      error: function (xhr, status, err) {
        console.error(
          "[Servicios] Error ConsultarPrecioServicio:",
          status,
          err,
          xhr,
        );
      },
    });
  };

  self.agregarPaquete = function () {
    if (confirm("¿Desea agregar este paquete de hospitalizacion?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/AgregarPaquete",
        method: "POST",
        data: {
          HospitalizacionId: $("#HospitalizacionId").val(),
          PaqueteId: self.paqueteAgregarSeleccionado().PaqueteId,
          PacienteId: $("#PacienteId").val(),
          CuentaId: $("#CuentaId").val(),
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarPaquetesHospitalizacion();
            self.consultarPaquetesAplicados();
            self.consultarServiciosHospitalizacion();
            mensajeEmergente("Paquete agregado exitosamente");
          }
          else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };

  self.obtenerSolicitudesPorHospitalizacion = function () {
    let hospitalizacionId = $("#HospitalizacionId").val();

    // ✅ Validar que el ID de hospitalización sea válido
    if (!hospitalizacionId || isNaN(hospitalizacionId)) {
      alert("⚠ Error: ID de hospitalización inválido.");
      return;
    }

    // ✅ Llamar a la API para obtener las solicitudes de medicamentos por hospitalización
    $.ajax({
      url: `/SolicitudMedicamento/GetByHospitalizacion?hospitalizacionId=${hospitalizacionId}`,
      type: "GET",
      contentType: "application/json",
      success: function (response) {
        if (response && Array.isArray(response)) {
          // console.log("📋 Solicitudes recibidas:", response);

          // ✅ Convertir cada item en un objeto con observables y asegurarse de que `indicaciones` esté presente
          let solicitudes = response.map((s) => ({
            id: s.id,
            nombreProducto: s.nombreProducto,
            cantidad: s.cantidad,
            precio: s.precio,
            viaAdministracion: s.viaAdministracion,
            frecuenciaAdministracion: s.frecuenciaAdministracion,
            indicaciones: s.indicaciones || "Sin indicaciones", // ✅ Evitar valores undefined
            estado: ko.observable(s.estado), // 🔹 Convertir `estado` en un `ko.observable`
            fechaSolicitudFormatted: s.fechaSolicitudFormatted,
            fechaDespachoFormatted: s.fechaDespachoFormatted,
          }));

          // ✅ Asignar las solicitudes al observable
          self.solicitudesDeMedicamentos(solicitudes);
        } else {
          alert("❌ No se encontraron solicitudes para esta hospitalización.");
        }
      },
      error: function (xhr, status, error) {
        alert(
          `⚠ Error inesperado: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error
          }`,
        );
      },
    });
  };

  self.crearSolicitudMedicamento = function (solicitud) {
    let tipoProductoId;

    self.medicamentosExistentes().forEach((e) => {
      if (e.ProductoId === solicitud.ProductoId) {
        tipoProductoId = e.TipoProductoId;
      }
    });

    // ✅ Construir el objeto asegurando que no falten campos
    var solicitudCompleta = {
      HospitalizacionId: solicitud.HospitalizacionId || 0,
      ProductoId: solicitud.ProductoId || 0,
      UnidadMedidaVentaId: solicitud.UnidadMedidaVentaId || 0,
      Cantidad: solicitud.Cantidad || 0,
      Precio: solicitud.Precio || 0.0,
      Indicaciones: solicitud.Indicaciones || "",
      ViaAdministracion: solicitud.ViaAdministracion || "",
      FrecuenciaAdministracion: solicitud.FrecuenciaAdministracion || "",
      IdProductoPrecioInventario: solicitud.IdProductoPrecioInventario || 0,
      PrecioId: solicitud.PrecioId || 0,

      TipoProducto: tipoProductoId ? tipoProductoId.toString() : "1",
    };

    console.log(solicitudCompleta);

    // ✅ Enviar la solicitud a la API
    $.ajax({
      url: "/SolicitudMedicamento/Add",
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify(solicitudCompleta),
      success: function (response) {
        if (response.exitoso) {
          alert(
            `✅ ${response.mensaje}\n📅 Fecha de solicitud: ${response.fechaSolicitud}`,
          );
          // window.location.reload(); // Recargar la página para ver los cambios
        } else {
          alert(`❌ Error: ${response.mensaje}`);
        }
      },
      error: function (xhr, status, error) {
        alert(
          `⚠ Error inesperado: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error
          }`,
        );
      },
    });
  };

  self.eliminarSolicitudDeMedicamento = function (id) {
    if (!confirm(`¿Seguro que deseas eliminar la solicitud #${id}?`)) {
      return;
    }

    $.ajax({
      url: `/SolicitudMedicamento/Delete/${id}`,
      type: "DELETE",
      contentType: "application/json",
      success: function (response) {
        if (response.exitoso) {
          alert("✅ Solicitud eliminada correctamente.");
          self.solicitudesDeMedicamentos.remove(function (item) {
            return item.id === id;
          });
        } else {
          alert(`❌ Error al eliminar: ${response.mensaje}`);
        }
      },
      error: function (xhr, status, error) {
        alert(
          `⚠ Error inesperado: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error
          }`,
        );
      },
    });
  };

  self.obtenerSolicitudPorId = function (id) {
    return $.ajax({
      url: `/SolicitudMedicamento/GetById?id=${id}`,
      type: "GET",
      contentType: "application/json",
    })
      .done(function (response) {
        // console.log(`📄 Detalles de la solicitud #${id}:`, response);
      })
      .fail(function (xhr, status, error) {
        console.error(
          `⚠ Error al obtener la solicitud #${id}: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error
          }`,
        );
      });
  };

  self.registrarSolicitudesDespachadas = function () {
    let hospitalizacionId = $("#HospitalizacionId").val();
    confirmAgregar =
      "¿Desea registrar los medicamentos e insumos recibidos a la hospitalización?";
    if (confirm(confirmAgregar)) {
      // ✅ Obtener todas las solicitudes de medicamentos de la hospitalización
      $.ajax({
        url: `/SolicitudMedicamento/GetByHospitalizacion?hospitalizacionId=${hospitalizacionId}`,
        method: "GET",
        success: function (solicitudes) {
          let solicitudesPendientes = solicitudes.filter(
            (s) =>
              s.estado === "Despachado" &&
              s.esRegistroHospitalizacion === false,
          );

          if (solicitudesPendientes.length === 0) {
            alert("No hay solicitudes pendientes de registro.");
            return;
          }

          procesarSolicitud(0, solicitudesPendientes);
        },
        error: function () {
          alert(
            "Error al obtener solicitudes de medicamentos. Intente nuevamente más tarde.",
          );
        },
      });
    }
  };

  // Método para agregar medicamento
  self.agregarMedicamento = function () {
    if (
      self.cantidadMedicamentoAgregar() == null ||
      self.cantidadMedicamentoAgregar().trim() == "" ||
      isNaN(self.cantidadMedicamentoAgregar())
    ) {
      alert("Proporcione una cantidad válida para el medicamento");
      return false;
    }
    if (!self.valorMedicamentoSeleccionado()) {
      alert("Seleccione un precio para el medicamento");
      return false;
    }

    if (
      self.switchMedicamentosEquipos() == "insumos" ||
      self.switchMedicamentosEquipos() == "medicamentos"
    ) {
      let dataSolicitud = {
        HospitalizacionId: $("#HospitalizacionId").val(),
        ProductoId: self.medicamentoAgregarSeleccionado().ProductoId,
        UnidadMedidaVentaId: self.unidadVentaSeleccionadaProducto().Id,
        Cantidad: self.cantidadMedicamentoAgregar(),
        Precio: self.valorMedicamentoSeleccionado().PrecioValor,
        Indicaciones: self.indicacionesMedicamentoAgregar(),
        ViaAdministracion: self.viaAdministracion(),
        FrecuenciaAdministracion: self.frecuenciaAdministracion(),
        PrecioId: self.valorMedicamentoSeleccionado().Id,
      };

      confirmAgregar = "¿Desea agregar esta solicitud?";

      if (confirm(confirmAgregar)) {
        self.crearSolicitudMedicamento(dataSolicitud);
      }

      return;
    }

    if (self.switchMedicamentosEquipos() === "equipos") {
      confirmAgregar = "¿Desea agregar este equipo médico?";
    } else if (self.switchMedicamentosEquipos() === "insumos") {
      confirmAgregar = "¿Desea agregar este insumo?";
    } else {
      confirmAgregar = "¿Desea agregar este medicamento?";
    }
    if (confirm(confirmAgregar)) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/AgregarMedicamento",
        method: "POST",
        data: {
          HospitalizacionId: $("#HospitalizacionId").val(),
          ProductoId: self.medicamentoAgregarSeleccionado().ProductoId,
          UnidadMedidaVentaId: self.unidadVentaSeleccionadaProducto().Id,
          Cantidad: self.cantidadMedicamentoAgregar(),
          Precio: self.valorMedicamentoSeleccionado().PrecioValor,
          Indicaciones: self.indicacionesMedicamentoAgregar(),
          ViaAdministracion: self.viaAdministracion(),
          FrecuenciaAdministracion: self.frecuenciaAdministracion(),
          PrecioId: self.valorMedicamentoSeleccionado().Id,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarMedicamentosHospitalizacion();
            self.consultarProductosAplicacionHospitalizacion();
            self.cantidadMedicamentoAgregar(0);
            self.indicacionesMedicamentoAgregar("");
            mensajeEmergente("Medicamento agregado");
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
    var medicamento = self.medicamentoAgregarSeleccionado();
    var cantidad = self.cantidadMedicamentoAgregar();
    if (medicamento && medicamento.ProductoNombre && cantidad > 0) {
      var entry = medicamento.ProductoNombre + " x " + cantidad;
      // self.concatenarOrdenMedicaTemp(entry);  // Almacena en el array temporal
    } else {
      alert("Seleccione un producto y proporcione una cantidad válida.");
    }
  };

  // Consultar precios del medicamento seleccionado
  self.consultarUnidadesVentaProducto = function (producto) {
    console.log(producto); // Ver qué contiene 'producto'

    if (
      !self.medicamentoAgregarSeleccionado() ||
      !producto ||
      !producto.ProductoId
    ) {
      return; // Evita continuar si no hay un producto válido
    }

    let productoId = producto.ProductoId;

    self.unidadesVentaProducto([]);
    let registrosInventarioProducto = new Array();
    $(self.registrosInventario()).each(function (idx, registro) {
      if (registro.ProductoId == productoId) {
        registrosInventarioProducto.push(registro);
      }
    });

    let unidadesVentaIds = new Set();
    $(registrosInventarioProducto).each(function (idx, registro) {
      if (
        registro.UnidadMedidaVentaId != null &&
        registro.UnidadMedidaVentaId != undefined
      ) {
        unidadesVentaIds.add(registro.UnidadMedidaVentaId);
      }
    });

    for (let unidadVentaId of unidadesVentaIds) {
      let agregado = false;
      //let unidades = new Array();
      $(registrosInventarioProducto).each(function (idx, vl) {
        if (vl.UnidadMedidaVentaId == unidadVentaId && !agregado) {
          let unidadAgregada = {
            Id: unidadVentaId,
            UnidadMedidaVentaNombre: vl.UnidadMedidaVentaNombre,
          };
          self.unidadesVentaProducto.push(unidadAgregada);
          agregado = true;
        }
      });
    }
  };

  // Método para consultar precios del medicamento
  self.consultarMedicamentoPrecio = function (unidadSeleccionada) {
    if (unidadSeleccionada == null || unidadSeleccionada == undefined) {
      self.valoresMedicamentoAgregar([]);
      return;
    }

    if (
      !self.medicamentoAgregarSeleccionado() ||
      self.medicamentoAgregarSeleccionado() == null ||
      self.medicamentoAgregarSeleccionado() == undefined
    ) {
      mensajeEmergenteError("No hay ningun medicamento valido seleccionado");
    }
    self.valoresMedicamentoAgregar([]);
    let registrosInventarioProducto = new Array();
    $(self.registrosInventario()).each(function (idx, registro) {
      if (
        registro.ProductoId ==
        self.medicamentoAgregarSeleccionado().ProductoId &&
        registro.UnidadMedidaVentaId == unidadSeleccionada.Id
      ) {
        registrosInventarioProducto.push(registro);
      }
    });

    let preciosIds = new Set();
    $(registrosInventarioProducto).each(function (idx, registro) {
      if (registro.PrecioId != null && registro.PrecioId != undefined) {
        preciosIds.add(registro.PrecioId);
      }
    });

    for (let precioId of preciosIds) {
      let precios = new Array();
      $(registrosInventarioProducto).each(function (idx, vl) {
        if (vl.PrecioId == precioId) {
          let precioAgregado = {
            ProductoInventarioId: vl.ProductoInventarioId,
            Id: precioId,
            Precio: vl.PrecioNombre + " (Q " + vl.PrecioValor + ")",
            PrecioValor: vl.PrecioValor,
            PrecioCompra: vl.PrecioCompra,
          };
          precios.push(precioAgregado);
        }
      });
      //Ahora se debe eliminar las duplicidades de precios
      //eligiendo el precio del ultimo registro en inventario
      let productoInventarioIds = new Array();
      $(precios).each(function (idx, vl) {
        productoInventarioIds.push(vl.ProductoInventarioId);
      });
      let ultimoProductoInventarioId = productoInventarioIds[0];
      for (let i = 0; i < productoInventarioIds.length; i++) {
        if (productoInventarioIds[i] > ultimoProductoInventarioId) {
          ultimoProductoInventarioId = productoInventarioIds[i];
        }
      }
      $(precios).each(function (idx, vl) {
        if (vl.ProductoInventarioId != ultimoProductoInventarioId) {
          precios.splice(idx, 1);
        }
      });
      $(precios).each(function (idx, vl) {
        self.valoresMedicamentoAgregar.push(vl);
      });
    }
  };

  // Evento cuando se selecciona un medicamento
  self.medicamentoAgregarSeleccionado.subscribe(function (value) {
    self.consultarUnidadesVentaProducto(value);
  });
  // Evento cuando se selecciona una unidad de venta de medicamento
  self.unidadVentaSeleccionadaProducto.subscribe(function (unidadSeleccionada) {
    self.consultarMedicamentoPrecio(unidadSeleccionada);
  });
  self.aplicarProducto = function (value) {
    if (confirm("¿Desea aplicar/liberar este producto?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/AplicarProducto",
        method: "POST",
        data: {
          hospitalizacionProductoAplicacionId: value.Id,
          cuentaId: $("#CuentaId").val(),
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarMedicamentosHospitalizacion();
            self.consultarProductosAplicacionHospitalizacion();
            mensajeEmergente("Producto aplicado");
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          alert(dataError);
        },
      });
    }
  };
  self.aplicarServicio = function (value) {
    if (confirm("¿Desea aplicar este servicio al paciente?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/AplicarServicio",
        method: "POST",
        data: {
          hospitalizacionServicioId: value.Id,
          cuentaId: $("#CuentaId").val(),
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarServiciosHospitalizacion();
            mensajeEmergente(data.Mensaje);
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };

  self.agregarReceta = function () {
    var recetaId = parseInt(self.recetaIdAgregar(), 10);
    var cantidad = parseInt(self.cantidadRecetaAgregar(), 10);

    // Validaciones previas (antes del confirm)
    if (!recetaId || recetaId <= 0) {
      alert("Seleccione una dieta válida.");
      return;
    }

    if (!cantidad || cantidad <= 0) {
      alert("Seleccione una cantidad válida.");
      return;
    }

    if (!self.indicacionesRecetaAgregar()) {
      self.indicacionesRecetaAgregar(""); // asegurar no undefined
    }

    if (confirm("¿Desea agregar esta receta?")) {
      showLoading();
      self.getModelReceta();

      $.ajax({
        url: "/Receta/AgregarReceta",
        method: "POST",
        data: modelReceta,
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);

          if (data.Exitoso) {
            self.consultarReceta();
            self.consultarRecetasAplicadas();

            // Limpieza de campos
            self.recetaIdAgregar(null);
            self.nombreRecetaAgregar("");
            self.ingredientesRecetaAgregar("");
            self.cantidadRecetaAgregar("");
            self.indicacionesRecetaAgregar("");

            mensajeEmergente("Receta agregada");
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };

  self.consultarReceta = function () {
    let textoCargando = $("#texto-cargando-dietas-formuladas");
    let textoError = $("#texto-error-consultar-dietas-formuladas");

    textoCargando.show();
    textoError.hide();
    showLoading();

    $.ajax({
      url: "/Receta/ConsultarLista",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        textoCargando.hide();
        hideLoading();

        let data;
        try {
          data = JSON.parse(dataResult);
        } catch (e) {
          console.log(
            "Error parseando JSON /Receta/ConsultarLista",
            e,
            dataResult,
          );
          textoError.show();
          return;
        }

        if (data && data.Exitoso) {
          // Normalización defensiva (no cambia lógica; solo evita null/undefined en UI)
          var resultado = Array.isArray(data.Resultado) ? data.Resultado : [];

          resultado = resultado.map(function (r) {
            r = r || {};

            // Categoría: si viene vacío/null, mostrar "-"
            if (
              r.CategoriaNombre == null ||
              String(r.CategoriaNombre).trim() === ""
            ) {
              r.CategoriaNombre = "-";
            }

            // Menús: si viene null/undefined, usar []
            if (!Array.isArray(r.Menus)) {
              r.Menus = [];
            }

            // Normalizar cada menú (evita textos "undefined" en el collapse)
            r.Menus = r.Menus.map(function (m) {
              m = m || {};
              if (m.Nombre == null) m.Nombre = "";
              if (m.Ingredientes == null) m.Ingredientes = "";
              return m;
            });

            return r;
          });

          self.listaRecetas(resultado);
          self.actualizarTotales();
        } else {
          textoError.show();
        }
      },
      error: function (dataError) {
        textoCargando.hide();
        hideLoading();
        console.log(dataError);
        textoError.show();
      },
    });
  };

  self.consultarRecetasAplicadas = function () {
    showLoading();
    $.ajax({
      url: "/Receta/ConsultarRecetasAplicadas",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();

        let data;
        try {
          data = JSON.parse(dataResult);
        } catch (e) {
          console.log(
            "Error parseando JSON /Receta/ConsultarRecetasAplicadas",
            e,
            dataResult,
          );
          mensajeEmergenteError("Error al consultar recetas aplicadas.");
          return;
        }

        if (data && data.Exitoso) {
          self.recetasAplicacion(
            Array.isArray(data.Resultado) ? data.Resultado : [],
          );
        } else {
          mensajeEmergenteError(
            data && data.Mensaje
              ? data.Mensaje
              : "Error al consultar recetas aplicadas.",
          );
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError("Error al consultar recetas aplicadas.");
      },
    });
  };

  self.eliminarReceta = function (value) {
    if (confirm("¿Desea eliminar esta dieta?")) {
      showLoading();
      $.ajax({
        url: "/Receta/EliminarHospitalizacionReceta",
        method: "POST",
        data: {
          IdHospitalizacionReceta: value.Id,
          cuentaId: $("#CuentaId").val(),
        },
        success: function (dataResult) {
          hideLoading();

          let data;
          try {
            data = JSON.parse(dataResult);
          } catch (e) {
            console.log(
              "Error parseando JSON /Receta/EliminarHospitalizacionReceta",
              e,
              dataResult,
            );
            alert("Error al eliminar la dieta.");
            return;
          }

          if (data && data.Exitoso) {
            self.consultarReceta();
            self.consultarRecetasAplicadas();
            self.actualizarTotales();
            mensajeEmergente("Receta eliminada");
          } else {
            alert(
              data && data.Mensaje
                ? data.Mensaje
                : "Error al eliminar la dieta.",
            );
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          alert("Error al eliminar la dieta.");
        },
      });
    }
  };

  self.aplicarReceta = function (value) {
    if (confirm("¿Desea aplicar esta dieta?")) {
      showLoading();
      $.ajax({
        url: "/Receta/AplicarReceta",
        method: "POST",
        data: {
          IdHospitalizacionReceta: value.IdHospitalizacionReceta,
          cuentaId: $("#CuentaId").val(),
        },
        success: function (dataResult) {
          hideLoading();

          let data;
          try {
            data = JSON.parse(dataResult);
          } catch (e) {
            console.log(
              "Error parseando JSON /Receta/AplicarReceta",
              e,
              dataResult,
            );
            alert("Error al aplicar la dieta.");
            return;
          }

          if (data && data.Exitoso) {
            self.consultarRecetasAplicadas();
            self.consultarReceta();
            self.actualizarTotales();
            mensajeEmergente("Receta aplicada");
          } else {
            alert(
              data && data.Mensaje
                ? data.Mensaje
                : "Error al aplicar la dieta.",
            );
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          alert("Error al aplicar la dieta.");
        },
      });
    }
  };

  self.consultarRecetasExistentes = function () {
    let textoCargando = $("#texto-cargando-dietas");
    let textoError = $("#texto-error-consultar-dietas");

    textoCargando.show();
    textoError.hide();

    $.ajax({
      url: "/Receta/ConsultarRecetasExistentes",
      method: "POST",
      success: function (dataResult) {
        textoCargando.hide();

        let data;
        try {
          data = JSON.parse(dataResult);
        } catch (e) {
          console.log(
            "Error parseando JSON /Receta/ConsultarRecetasExistentes",
            e,
            dataResult,
          );
          textoError.show();
          return;
        }

        if (data && data.Exitoso) {
          self.recetasExistentes(
            Array.isArray(data.Resultado) ? data.Resultado : [],
          );
        } else {
          textoError.show();
        }
      },
      error: function (dataError) {
        textoCargando.hide();
        textoError.show();
        console.log(dataError);
      },
    });
  };




  self.cambiarPaginaPaquetes = function (direccion) {
    var nuevaPagina = self.paginaActualPaquetes() + direccion;
    if (nuevaPagina >= 1 && nuevaPagina <= self.totalPaginasPaquetes()) {
      self.paginaActualPaquetes(nuevaPagina);
    }
  };



  self.consultarPaquetesAplicados = function () {
    $.ajax({
      url: '/Hospitalizacion/ConsultarDetallePaquetesHospitalizacionAplicados',
      method: 'POST',
      data: { hospitalizacionId: $('#HospitalizacionId').val() },
      beforeSend: function () {
        $('#texto-cargando-paquetes-aplicados').show();
        $('#texto-error-consultar-paquetes-aplicados').hide();
      },
      success: function (resp) {
        $('#texto-cargando-paquetes-aplicados').hide();
        var data = typeof resp === 'string' ? JSON.parse(resp) : resp;
        if (data.Exitoso) {
          var lista = data.Resultado.map(function (item) {
            return {
              Id: ko.observable(item.Id),
              IdAplicado: ko.observable(item.IdAplicado),
              Tipo: ko.observable(item.Tipo),
              Codigo: ko.observable(item.Codigo),
              Nombre: ko.observable(item.Nombre),
              Descripcion: ko.observable(item.Descripcion),
              Cantidad: ko.observable(item.Cantidad),
              CantidadAplicados: ko.observable(item.CantidadAplicados),
              Aplicado: ko.observable(item.Aplicado),
              FechaHoraAplicacion: ko.observable(item.FechaHoraAplicacion),
              Persona: ko.observable(item.Persona),
              CantidadAplicar: ko.observable(1)
            };
          });
          self.listaProductosAplicacion(lista);
          self.paginaActualPaquetes(1);
          self.actualizarBadgesPaquetes();
        } else {
          $('#texto-error-consultar-paquetes-aplicados').show();
        }
      },
      error: function () {
        $('#texto-cargando-paquetes-aplicados').hide();
        $('#texto-error-consultar-paquetes-aplicados').show();
      }
    });
  };

  self.aplicarPaquete = function (item) {
    var cantidadAplicar = parseInt(item.CantidadAplicar()) || 1;
    var restante = item.Cantidad() - item.CantidadAplicados();
    if (cantidadAplicar < 1 || cantidadAplicar > restante) {
      mensajeEmergenteError('La cantidad a aplicar debe estar entre 1 y ' + restante);
      return;
    }
    showLoading();
    $.ajax({
      url: '/Hospitalizacion/AplicarDetallePaqueteHospitalizacion',
      method: 'POST',
      data: { Id: item.Id(), Cantidad: cantidadAplicar },
      success: function (resp) {
        hideLoading();
        let data = typeof resp === 'string' ? JSON.parse(resp) : resp;
        if (data.Exitoso) {
          self.consultarPaquetesAplicados();
          mensajeEmergente("Aplicación exitosa.");
        } else {
          mensajeEmergenteError(data.Mensaje || "Error desconocido al aplicar.");
        }
      },
      error: function (xhr) {
        hideLoading();
        console.error(xhr);
        let msg = xhr.responseJSON?.Mensaje || "Error de comunicación.";
        mensajeEmergenteError(msg);
      }
    });
  };


  self.abrirModalAgregarProductoPaquete = function () {
    var paquetesActivos = self.paquetesHospitalizacion().filter(function (p) { return !p.Eliminado; });
    var $selectPaquete = $('#selectPaqueteDestino');
    $selectPaquete.empty().append('<option value="">-- Seleccione paquete --</option>');
    paquetesActivos.forEach(function (p) {
      $selectPaquete.append('<option value="' + p.Id + '">' + (p.Codigo || '') + ' - ' + p.Nombre + '</option>');
    });
    if ($selectPaquete.data('select2')) $selectPaquete.select2('destroy');
    $selectPaquete.select2({ dropdownParent: $('#modalAgregarProductoPaquete'), width: '100%' });

    $('#selectProductoPaquete').empty().append('<option value="">-- Seleccione producto --</option>');
    $('#selectUnidadPaquete').empty().append('<option value="">-- Seleccione unidad --</option>');
    $('#selectPrecioPaquete').empty().append('<option value="">-- Seleccione precio --</option>');
    $('#inputCantidadProductoPaquete').val(1);
    $('#inputIndicacionesPaquete').val('');

    var tipoProductoMap = { medicamentos: 1, insumos: 2, equipos: 3 };
    var tipoId = tipoProductoMap[self.switchMedicamentosEquipos()] || 1;
    var productosUnicos = [];
    var productosMap = new Map();
    self.registrosInventario().forEach(function (vl) {
      if (vl.TipoProductoId === tipoId && !productosMap.has(vl.ProductoId)) {
        productosMap.set(vl.ProductoId, {
          ProductoId: vl.ProductoId,
          ProductoCodigo: vl.ProductoCodigo,
          ProductoNombre: vl.ProductoNombre
        });
      }
    });
    productosUnicos = Array.from(productosMap.values());
    var $productoSelect = $('#selectProductoPaquete');
    productosUnicos.forEach(function (p) {
      $productoSelect.append('<option value="' + p.ProductoId + '">' + p.ProductoCodigo + ' - ' + p.ProductoNombre + '</option>');
    });
    if ($productoSelect.data('select2')) $productoSelect.select2('destroy');
    $productoSelect.select2({ dropdownParent: $('#modalAgregarProductoPaquete'), width: '100%' });

    $('#modalAgregarProductoPaquete').dialog({
      modal: true,
      width: 700,
      title: 'Agregar Medicamento / Insumo al Paquete',
      buttons: {
        "Cancelar": function () { $(this).dialog("close"); },
        "Agregar": function () {
          self.confirmarAgregarProductoAPaquete();
          $(this).dialog("close");
        }
      }
    });
  };

  self.confirmarAgregarProductoAPaquete = function () {
    var paqueteId = $('#selectPaqueteDestino').val();
    var productoId = $('#selectProductoPaquete').val();
    var unidadId = $('#selectUnidadPaquete').val();
    var cantidad = parseInt($('#inputCantidadProductoPaquete').val(), 10);
    var precio = $('#selectPrecioPaquete').val();
    var indicaciones = $('#inputIndicacionesPaquete').val();

    if (!paqueteId) { mensajeEmergenteError("Seleccione un paquete destino."); return; }
    if (!productoId) { mensajeEmergenteError("Seleccione un producto."); return; }
    if (!unidadId) { mensajeEmergenteError("Seleccione una unidad de venta."); return; }
    if (!cantidad || cantidad <= 0) { mensajeEmergenteError("Cantidad inválida."); return; }
    if (isNaN(precio) || precio <= 0) {
      mensajeEmergenteError("Seleccione un precio válido.");
      return;
    }
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/AgregarProductoAPaqueteHospitalizacion",
      method: "POST",
      data: {
        hospitalizacionPaqueteId: paqueteId,
        productoId: productoId,
        unidadMedidaVentaId: unidadId,
        cantidad: cantidad,
        precioProducto: parseFloat(precio),
        indicaciones: indicaciones
      },
      success: function (response) {
        hideLoading();
        var data = typeof response === "string" ? JSON.parse(response) : response;
        if (data.Exitoso) {
          mensajeEmergente(data.Mensaje || "Producto agregado al paquete.");
          self.consultarPaquetesAplicados();
          self.actualizarTotales();
        } else {
          mensajeEmergenteError(data.Mensaje || "Error al agregar producto.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de conexión al agregar producto.");
      }
    });
  };


  self.eliminarDetallePaquete = function (item) {
    if (!confirm("¿Eliminar este ítem del paquete? (solo si no ha sido aplicado)")) return;
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/EliminarDetallePaqueteHospitalizacion",
      method: "POST",
      data: { detalleId: item.Id() },
      success: function (response) {
        hideLoading();
        var data = typeof response === "string" ? JSON.parse(response) : response;
        if (data.Exitoso) {
          self.consultarPaquetesAplicados();
          mensajeEmergente("Ítem eliminado.");
        } else {
          mensajeEmergenteError(data.Mensaje || "Error al eliminar.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de conexión.");
      }
    });
  };
  self.abrirModalDevolucionPaquete = function (item) {
    window._paqueteItemDevolucion = item;
    $('#selectMotivoPaqueteDevolucion').val('');
    $('#divOtroMotivoPaquete').hide();
    $('#inputOtroMotivoPaquete').val('');
    $('#modalDevolucionPaquete').dialog({
      modal: true,
      width: 500,
      title: "Devolver ítem a Pendiente",
      buttons: {
        "Cancelar": function () { $(this).dialog("close"); window._paqueteItemDevolucion = null; },
        "Confirmar": function () {
          var motivo = $('#selectMotivoPaqueteDevolucion').val();
          if (!motivo) { mensajeEmergenteError("Seleccione un motivo."); return; }
          if (motivo === "Otro") {
            motivo = $('#inputOtroMotivoPaquete').val().trim();
            if (!motivo) { mensajeEmergenteError("Escriba el motivo."); return; }
          }
          self.confirmarDevolverDetallePaquete(window._paqueteItemDevolucion, motivo);
          $(this).dialog("close");
          window._paqueteItemDevolucion = null;
        }
      }
    });
  };

  self.confirmarDevolverDetallePaquete = function (item, motivo) {
    var idParaDevolver = (item.IdAplicado && item.IdAplicado() != null)
      ? item.IdAplicado()
      : item.Id();

    showLoading();
    $.ajax({
      url: "/Hospitalizacion/DevolverDetallePaqueteHospitalizacion",
      method: "POST",
      data: { detalleId: idParaDevolver, motivoDevolucion: motivo },
      success: function (response) {
        hideLoading();
        var data = typeof response === "string" ? JSON.parse(response) : response;
        if (data.Exitoso) {
          self.consultarPaquetesAplicados();
          mensajeEmergente("Ítem devuelto a estado Pendiente.");
        } else {
          mensajeEmergenteError(data.Mensaje || "Error al devolver.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de conexión.");
      }
    });
  };
  // self.ordenesMedicas = ko.observableArray([]);
  // self.ordenesMedicasTemp = [];  // Aquí guardas los valores seleccionados temporalmente

  // self.concatenarOrdenMedicaTemp = function (entry) {
  //     self.ordenesMedicasTemp.push(entry);  // Almacena cada selección en el array temporal
  // };

  // self.cerrarModalOrdenMedica = function () {
  //     // Concatenar todos los elementos almacenados en el array temporal
  //     var ordenConcatenada = self.ordenesMedicasTemp.join(' | ');

  //     // Si hay algo que concatenar, lo añadimos al observable principal
  //     if (ordenConcatenada) {
  //         // Limpiar el observable solo cuando agregamos una nueva orden
  //         self.ordenesMedicas([]);  // Limpia todas las entradas anteriores
  //         self.ordenesMedicas.push({
  //             Fecha: new Date().toLocaleDateString(),  // Puedes ajustar esto para usar la fecha real
  //             Profesional: "Dr. Ejemplo",  // Coloca aquí el nombre del profesional real
  //             Valor: ordenConcatenada,
  //             Observaciones: ""
  //         });
  //     }

  //     // Limpiar el array temporal para el siguiente uso
  //     self.ordenesMedicasTemp = [];
  // };

  // self.concatenarOrdenMedica = function () {
  //     var resultado = [];

  //     // Agregar examen seleccionado
  //     if (self.examenAgregarSeleccionado()) {
  //         resultado.push(self.examenAgregarSeleccionado().Nombre + " x 1");
  //     }

  //     // Agregar servicio seleccionado
  //     if (self.servicioAgregarSeleccionado() && self.cantidadServicioAgregar() > 0) {
  //         resultado.push(self.servicioAgregarSeleccionado().Nombre + " x " + self.cantidadServicioAgregar());
  //     }

  //     // Agregar medicamento seleccionado
  //     if (self.medicamentoAgregarSeleccionado() && self.cantidadMedicamentoAgregar() > 0) {
  //         resultado.push(self.medicamentoAgregarSeleccionado().ProductoNombre + " x " + self.cantidadMedicamentoAgregar());
  //     }

  //     // Agregar dieta seleccionada
  //     if (self.recetaAgregarSeleccionada() && self.cantidadRecetaAgregar() > 0) {
  //         resultado.push(self.recetaAgregarSeleccionada().NombreReceta + " x " + self.cantidadRecetaAgregar());
  //     }

  //     // Combinar todo en un solo string
  //     var ordenConcatenada = resultado.join(' | ');

  //     // Agregar el resultado concatenado al array
  //     if (ordenConcatenada) {
  //         self.ordenesMedicas.push(ordenConcatenada);
  //     }
  // };

  // Método para agregar examen (corregido: envía CodigoSeguro, usa JSON, refresh y bandera examenAgregado)
  self.agregarExamen = function () {
    if (!self.examenAgregarSeleccionado() || !self.examenAgregarSeleccionado().ExamenLabClinicoId) {
      alert("Seleccione un examen.");
      return false;
    }

    if (!self.valorExamenSeleccionado() || !self.valorExamenSeleccionado().ExamenLabClinicoPrecioId) {
      alert("Seleccione un precio para el examen");
      return false;
    }

    if (!confirm("¿Desea agregar este examen?")) {
      return false;
    }

    showLoading();

    const payload = {
      HospitalizacionId: Number($("#HospitalizacionId").val()),
      PacienteId: Number($("#PacienteId").val()),
      CuentaId: Number($("#CuentaId").val()),
      CodigoSeguro: ($("#CodigoSeguro").val() || "").trim(),
      Examenes: [
        {
          ExamenLabClinicoId: self.examenAgregarSeleccionado().ExamenLabClinicoId,
          Observacion: null,
        },
      ],
      ExamenLabClinicoPrecioId: self.valorExamenSeleccionado().ExamenLabClinicoPrecioId,
    };

    $.ajax({
      url: "/Hospitalizacion/AgregarExamen",
      method: "POST",
      contentType: "application/json; charset=utf-8",
      data: JSON.stringify(payload),
      success: function (dataResult) {
        hideLoading();

        let data = dataResult;
        if (typeof dataResult === "string") {
          try {
            data = JSON.parse(dataResult);
          } catch (e) {
            mensajeEmergenteError("Respuesta inválida del servidor.");
            return;
          }
        }

        if (data && data.Exitoso) {
          if (typeof self.examenAgregado === "function") {
            self.examenAgregado(true);
          }

          self.examenesComplementariosCargados = false;
          self.consultarExamenesHospitalizacion(true);

          mensajeEmergente("Examen agregado");
        } else {
          mensajeEmergenteError((data && data.Mensaje) ? data.Mensaje : "No fue posible agregar el examen.");
        }
      },
      error: function (xhr) {
        hideLoading();
        console.log(xhr);
        const msg = (xhr && xhr.responseText) ? xhr.responseText : "Error al agregar examen.";
        mensajeEmergenteError(msg);
      },
    });

    return true;
  };

  // Método para consultar precios del examen
  self.consultarPrecioExamen = function (examenId) {
    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarPrecioExamen",
      method: "POST",
      data: {
        examenId: examenId,
        codigoSeguro: $("#CodigoSeguro").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.valoresExamenAgregar(data.Resultado);
          self.valorExamenSeleccionado(null); // Resetea el precio seleccionado
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError(dataError);
      },
    });
  };

  // Evento cuando se selecciona un examen
  self.examenAgregarSeleccionado.subscribe(function (value) {
    if (value) {
      self.consultarPrecioExamen(value.ExamenLabClinicoId);
    }
  });

  self.modificarResultadosExamen = function (value) {
    window.open(
      "/LaboratorioClinico/ModificarResultadosExamen?detalleExamenId=" +
      value.DetalleExamenId,
      "_blank",
    );
  };
  self.modificarResultadosExamenPaquete = function (examen) {
    if (examen.DetalleExamenId != null && examen.DetalleExamenId != undefined) {
      window.open(
        "/LaboratorioClinico/ModificarResultadosExamen?detalleExamenId=" +
        examen.DetalleExamenId,
        "_blank",
      );
    } else {
      mensajeEmergenteError("No hay datos de examen registrado");
    }
  };
  self.descargarResultadosExamen = function (value) {
    window.open("/CrearPDF/GenerarResultados?id=" + value.ExamenId, "_blank");
  };
  self.descargarResultadosExamenPaquete = function (value) {
    if (value.ExamenId != null && value.ExamenId != undefined) {
      window.open("/CrearPDF/GenerarResultados?id=" + value.ExamenId, "_blank");
    } else {
      mensajeEmergenteError("No hay datos de examen registrado");
    }
  };
  self.agregarDeposito = function () {
    if (confirm("¿Desea agregar este deposito?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/AgregarDeposito",
        method: "POST",
        data: {
          CuentaId: $("#CuentaId").val(),
          FormaPagoId: $("#FormaPagoId").val(),
          MonedaId: $("#MonedaId").val(),
          Valor: self.valorDepositoAgregar(),
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarDepositosHospitalizacion();
            mensajeEmergente("Deposito agregado");
            self.valorDepositoAgregar("");
            self.actualizarTotales();
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };
  self.verModalAgregarNotaMedica2 = function () {
    self.editandoNotaMedica(false);
    self.editNotaMedicaId(null);
    self.notaDiagnostico("");
    if (self.quillMedica) self.quillMedica.setText("");
    $("#mdl-agregar-nota-medica").dialog({ width: 1000 });
  };

  self.editarNotaMedica2 = function (nota) {
    if (nota.Autorizado) {
      mensajeEmergenteError("No se puede editar una nota ya autorizada.");
      return;
    }
    self.editandoNotaMedica(true);
    self.editNotaMedicaId(nota.Id);
    self.notaDiagnostico(nota.Diagnostico || "");
    $("#mdl-agregar-nota-medica").dialog({
      width: 1000,
      open: function () {
        if (self.quillMedica) {
          self.quillMedica.root.innerHTML = nota.Diagnostico || "";
        }
      },
      close: function () {
        self.editandoNotaMedica(false);
        self.editNotaMedicaId(null);
        self.notaDiagnostico("");
        if (self.quillMedica) self.quillMedica.setText("");
      }
    });
  };
  self.verModalAgregarNotaOperatoria = function () {
    var hoy = new Date().toISOString().split("T")[0];
    self.notaFechaOperacion(hoy);

    // Precargar desde ultima NotaOperatoria (fuente de verdad)
    var hospitalizacionId = $("#HospitalizacionId").val() || 0;
    var cirujanoId = $("#CitaCirujanoId").val() || "";
    var primerAyudante = $("#CitaPrimerAyudanteNombre").val() || "";
    var fallback = { cirujanoId: cirujanoId, primerAyudante: primerAyudante, segundoAyudante: "", anestesista: "", instrumentista: "", circulante: "" };

    $.ajax({
      url: "/NotaOperatoria/GetUltimaNotaOperatoria",
      method: "GET",
      data: { hospitalizacionId: hospitalizacionId },
      success: function (dataResult) {
        var data = typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        if (data.exitoso) {
          self.cargarMedicosModal({
            cirujanoNombre: data.cirujano || "",
            primerAyudante: data.primerAyudante || "",
            segundoAyudante: data.segundoAyudante || "",
            anestesista: data.anestesista || "",
            instrumentista: data.instrumentista || "",
            circulante: data.circulante || ""
          }); // destino = "agregar" por defecto
        } else {
          self.cargarMedicosModal(fallback);
        }
      },
      error: function () { self.cargarMedicosModal(fallback); }
    });

    $("#mdl-agregar-nota-operatoria").dialog({
      width: 1000,
      title: "Agregar nota operatoria",
      open: function () {
        if (window.currentActiveId) {
          // detener dictado anterior
          window.stopRecording(window.currentActiveId);
        }
        setTimeout(function () {
          self.inicializarSelect2Personal();
          // Inicializar select2 para los selects de médicos
          $("#mdl-agregar-nota-operatoria .select2-medicos-modal").select2({
            width: "100%",
            dropdownParent: $("#mdl-agregar-nota-operatoria"),
            language: { noResults: function () { return "No se encontraron resultados"; } }
          });
        }, 300);
      }
    });
  };

  self.verModalAgregarUsuario = function () {
    $("#mdl-agregar-usuarios-acceso").dialog({
      width: 1000,
    });
  };

  self.verModalAgregarChequeo = function () {
    var hoy = new Date().toISOString().split("T")[0];
    self.chk_FechaChequeo(hoy);

    var nombreCompleto = ($("#PacienteNombreAdmision").val() || "").trim();
    var partes = nombreCompleto.split(" ");
    var nombre = partes[0] || "";
    var apellido = partes.slice(1).join(" ") || "";

    var fechaNacRaw = ($("#PacienteFechaNacimiento").val() || "").trim();
    var fechaNac = "";
    if (fechaNacRaw) {
      var pf = fechaNacRaw.split("/");
      if (pf.length === 3) {
        fechaNac = pf[2] + "-" + pf[1] + "-" + pf[0];
      } else {
        fechaNac = fechaNacRaw;
      }
    }

    // Asignar nombre, apellido y fecha de nacimiento a los observables del modal
    self.chk_CI_NombreConfirma(nombre);
    self.chk_CI_ApellidoConfirma(apellido);
    self.chk_CI_FechaNacConfirma(fechaNac);
    self.chk_CP_NombrePacienteCirujano(nombre);
    self.chk_CP_ApellidoPacienteCirujano(apellido);
    self.chk_CP_FechaNacCirujano(fechaNac);

    // ===== NUEVO: Precargar el procedimiento desde el hidden input =====
    var procedimiento = ($("#CitaProcedimiento").val() || "").trim();
    self.chk_CI_Operacion(procedimiento);
    self.chk_CP_NombreCirugia(procedimiento);

    // Abrir el modal
    $("#mdl-agregar-chequeo").dialog({
      width: 900,
      title: "Lista de Chequeo Quirúrgica",
    });
  };

  self.verModalAgregarKitIngreso = function () {
    var hoy = new Date().toISOString().split("T")[0];
    self.kit_Fecha(hoy);
    self.kit_productosAgregados([]);
    self.kit_total(0);
    self.kit_kitIdActivo(null);
    self.kit_productoSeleccionado(null);
    // self.kit_unidadesVenta([]);
    // self.kit_precios([]);
    self.kit_cantidad(1);


    self.kit_NombrePaciente($("#PacienteNombreAdmision").val() || "");
    self.kit_Medico($("#MedicoAsignadoNombre").val() || "");
    self.kit_Procedimiento($("#CitaProcedimiento").val() || "");
    // Nombre genérico sugerido con fecha
    self.kit_NombreKit("Kit de Ingreso " + hoy);

    $("#mdl-agregar-kit-ingreso").dialog({
      width: 1000,
      title: "Kit de Ingreso",
    });
  };

  // self.guardarEdicionKit = function () {
  //   var kit = self.kitIngresoDetalle();
  //   if (!kit || !kit.Id) {
  //     mensajeEmergenteError("No hay kit seleccionado.");
  //     return;
  //   }

  //   // Datos actuales de la hospitalización
  //   var currentPaciente = $("#PacienteNombreAdmision").val() || "";
  //   var currentMedico = $("#MedicoAsignadoNombre").val() || "";
  //   var currentProcedimiento = $("#CitaProcedimiento").val() || "";
  //   var currentResponsable = $("#ResponsableNombre").val() || "";

  //   // 1. Actualizar encabezado del kit
  //   showLoading();
  //   $.ajax({
  //     url: "/KitIngreso/ActualizarEncabezado",  // Endpoint que debes crear en el backend
  //     method: "POST",
  //     contentType: "application/json",
  //     data: JSON.stringify({
  //       Id: kit.Id,
  //       NombreKit: ko.unwrap(kit.NombreKit),        // editable en el modal
  //       NombrePaciente: currentPaciente,
  //       Medico: currentMedico,
  //       Procedimiento: currentProcedimiento,
  //       Responsable: currentResponsable,
  //       HospitalizacionId: $("#HospitalizacionId").val()
  //     }),
  //     success: function (response) {
  //       var data = typeof response === "string" ? JSON.parse(response) : response;
  //       if (!data.exitoso) {
  //         hideLoading();
  //         mensajeEmergenteError(data.mensaje || "Error al actualizar encabezado.");
  //         return;
  //       }
  //       // 2. Actualizar los detalles (productos)
  //       var updates = [];
  //       var error = false;

  //       ko.utils.arrayForEach(kit.Detalles(), function (d) {
  //         if (d.Eliminado && d.Eliminado()) return;
  //         if (d.Utilizado() > d.Cantidad()) {
  //           mensajeEmergenteError("La cantidad utilizada de " + d.ProductoNombre + " no puede ser mayor a la cantidad entregada.");
  //           error = true;
  //           return;
  //         }
  //         updates.push({
  //           detalleId: d.Id,
  //           hospitalizacionId: $("#HospitalizacionId").val(),
  //           utilizado: d.Utilizado()
  //         });
  //       });

  //       if (error) {
  //         hideLoading();
  //         return;
  //       }

  //       var promises = updates.map(function (u) {
  //         return $.ajax({
  //           url: "/KitIngreso/GuardarUtilizadoKitDetalle",
  //           method: "POST",
  //           data: u
  //         });
  //       });

  //       $.when.apply($, promises)
  //         .done(function () {
  //           hideLoading();
  //           mensajeEmergente("Kit actualizado correctamente.");
  //           $("#mdl-detalle-kit-ingreso").modal("hide");
  //           self.consultarKitsIngreso(); // refrescar lista
  //         })
  //         .fail(function (err) {
  //           hideLoading();
  //           console.error(err);
  //           mensajeEmergenteError("Error al guardar los detalles.");
  //         });
  //     },
  //     error: function () {
  //       hideLoading();
  //       mensajeEmergenteError("Error al actualizar el encabezado del kit.");
  //     }
  //   });
  // };


  self.guardarEdicionKit = function () {
    var kit = self.kitIngresoDetalle();
    if (!kit || !kit.Id) {
      mensajeEmergenteError("No hay kit seleccionado.");
      return;
    }

    // Datos actuales de la hospitalización
    var currentPaciente = $("#PacienteNombreAdmision").val() || "";
    var currentMedico = $("#MedicoAsignadoNombre").val() || "";
    var currentProcedimiento = $("#CitaProcedimiento").val() || "";
    var currentResponsable = $("#ResponsableNombre").val() || "";

    // 1. Actualizar encabezado del kit
    showLoading();
    $.ajax({
      url: "/KitIngreso/ActualizarEncabezadoKit",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        Id: kit.Id,
        NombreKit: ko.unwrap(kit.NombreKit),
        NombrePaciente: currentPaciente,
        Medico: currentMedico,
        Procedimiento: currentProcedimiento,
        Responsable: currentResponsable
      }),
      success: function (response) {
        if (!response.exitoso) {
          hideLoading();
          mensajeEmergenteError(response.mensaje || "Error al actualizar encabezado.");
          return;
        }

        // 2. Actualizar los detalles (productos)
        var updates = [];
        var error = false;

        ko.utils.arrayForEach(kit.Detalles(), function (d) {
          if (d.Eliminado && d.Eliminado()) return;
          if (d.Utilizado() > d.Cantidad()) {
            mensajeEmergenteError("La cantidad utilizada de " + d.ProductoNombre + " no puede ser mayor a la cantidad entregada.");
            error = true;
            return;
          }
          updates.push({
            detalleId: d.Id,
            hospitalizacionId: $("#HospitalizacionId").val(),
            utilizado: d.Utilizado()
          });
        });

        if (error) {
          hideLoading();
          return;
        }

        var promises = updates.map(function (u) {
          return $.ajax({
            url: "/KitIngreso/GuardarUtilizadoKitDetalle",
            method: "POST",
            data: u
          });
        });

        $.when.apply($, promises)
          .done(function () {
            hideLoading();
            mensajeEmergente("Kit actualizado correctamente.");
            $("#mdl-detalle-kit-ingreso").modal("hide");
            self.consultarKitsIngreso(); // refrescar lista
          })
          .fail(function (err) {
            hideLoading();
            console.error(err);
            mensajeEmergenteError("Error al guardar los detalles.");
          });
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error al actualizar el encabezado del kit.");
      }
    });
  };




  // self.guardarEdicionKit = function () {
  //   var kit = self.kitIngresoDetalle();
  //   if (!kit || !kit.Id) {
  //     mensajeEmergenteError("No hay kit seleccionado.");
  //     return;
  //   }

  //   var hospitalizacionId = $("#HospitalizacionId").val();
  //   var updates = [];
  //   var error = false;

  //   ko.utils.arrayForEach(kit.Detalles(), function (d) {
  //     if (d.Eliminado && d.Eliminado()) return;

  //     if (d.Utilizado() > d.Cantidad()) {
  //       mensajeEmergenteError("La cantidad utilizada de " + d.ProductoNombre + " no puede ser mayor a la cantidad entregada.");
  //       error = true;
  //       return;
  //     }

  //     updates.push({
  //       detalleId: d.Id,
  //       hospitalizacionId: hospitalizacionId,
  //       utilizado: d.Utilizado()
  //     });
  //   });

  //   if (error) return;

  //   showLoading();
  //   var promises = updates.map(function (u) {
  //     return $.ajax({
  //       url: "/KitIngreso/GuardarUtilizadoKitDetalle",
  //       method: "POST",
  //       data: u
  //     });
  //   });

  //   $.when.apply($, promises)
  //     .done(function () {
  //       hideLoading();
  //       mensajeEmergente("Cantidades utilizadas guardadas correctamente.");
  //       $("#mdl-detalle-kit-ingreso").modal("hide");
  //     })
  //     .fail(function (err) {
  //       hideLoading();
  //       console.error(err);
  //       mensajeEmergenteError("Error al guardar cambios.");
  //     });
  // };

  self.abrirAgregarProductoKitExistente = function () {
    var kit = self.kitIngresoDetalle();
    if (!kit || !kit.Id) {
      mensajeEmergenteError("No hay kit seleccionado.");
      return;
    }

    self.kit_kitIdActivo(kit.Id);

    self.kit_productoSeleccionado(null);
    self.kit_cantidad(1);

    var modal = new bootstrap.Modal(document.getElementById("mdl-agregar-producto-kit"));
    modal.show();
  };



  // Función para agregar producto al kit actual (el que está en edición)
  self.agregarProductoAKitActual = function () {
    var kit = self.kitIngresoDetalle();
    if (!kit || !kit.Id) {
      mensajeEmergenteError("No hay kit seleccionado.");
      return;
    }
    var productoId = self.kit_productoSeleccionadoId();
    if (!productoId) {
      mensajeEmergenteError("Seleccione un producto.");
      return;
    }
    var cantidad = parseFloat(self.kit_cantidadAgregar()) || 1;
    if (cantidad <= 0) {
      mensajeEmergenteError("Cantidad inválida.");
      return;
    }

    var producto = self.kit_productosExistentes().find(p => p.ProductoId == productoId);
    if (!producto) {
      mensajeEmergenteError("Producto no encontrado.");
      return;
    }

    var nuevoDetalle = {
      KitIngresoId: kit.Id,
      ProductoId: producto.ProductoId,
      ProductoCodigo: producto.ProductoCodigo,
      ProductoNombre: producto.ProductoNombre,
      Cantidad: cantidad,
      Eliminado: false
    };

    showLoading();
    $.ajax({
      url: "/KitIngreso/AgregarProductoKitIngreso",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(nuevoDetalle),
      success: function (dataResult) {
        hideLoading();
        var data = typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        if (data.exitoso) {
          // Agregar el nuevo producto al kit actual en el frontend (sin recargar todo)
          var nuevoDetalleVM = {
            Id: 0, // temporal, el backend asignará el Id real
            KitIngresoId: kit.Id,
            ProductoId: producto.ProductoId,
            ProductoCodigo: producto.ProductoCodigo,
            ProductoNombre: producto.ProductoNombre,
            Cantidad: ko.observable(cantidad),
            Utilizado: ko.observable(0),
            Eliminado: ko.observable(false)
          };
          kit.Detalles.push(nuevoDetalleVM);
          mensajeEmergente("Producto agregado al kit.");
          self.kit_productoSeleccionadoId(null);
          self.kit_cantidadAgregar(1);
        } else {
          mensajeEmergenteError(data.mensaje || "Error al agregar producto.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de red.");
      }
    });
  };

  // ── Cargar inventario de productos ────────────────────────────────────────
  self.kit_cargarProductosInventario = function () {
    if (self.kit_productosExistentes().length > 0) return;
    $.ajax({
      url: "/KitIngreso/ConsultarProductosExistentes",
      method: "POST",
      success: function (dataResult) {
        var data = typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        if (data.exitoso) {
          self.kit_productosExistentes(data.resultado);
        } else {
          console.error("Error cargando productos:", data.mensaje);
        }
      },
      error: function (err) {
        console.error("Error en petición:", err);
      }
    });
  };
  // Subscripciones para cascada producto → unidad → precio
  self.kit_productoSeleccionado.subscribe(function (producto) {
    // self.kit_unidadesVenta([]);
    // self.kit_precios([]);
    // self.kit_unidadSeleccionada(null);
    // self.kit_precioSeleccionado(null);
    if (!producto) return;
    var ids = new Set();
    $(self.kit_registrosInventario()).each(function (idx, r) {
      if (r.ProductoId == producto.ProductoId && r.UnidadMedidaVentaId != null && !ids.has(r.UnidadMedidaVentaId)) {
        ids.add(r.UnidadMedidaVentaId);
        // self.kit_unidadesVenta.push({ Id: r.UnidadMedidaVentaId, UnidadMedidaVentaNombre: r.UnidadMedidaVentaNombre });
      }
    });
  });

  // self.kit_unidadSeleccionada.subscribe(function (unidad) {
  //   // self.kit_precios([]);
  //   self.kit_precioSeleccionado(null);
  //   if (!unidad || !self.kit_productoSeleccionado()) return;
  //   var precioIds = new Set();
  //   var maxInventarioId = {};
  //   $(self.kit_registrosInventario()).each(function (idx, r) {
  //     if (r.ProductoId == self.kit_productoSeleccionado().ProductoId && r.UnidadMedidaVentaId == unidad.Id) {
  //       if (!maxInventarioId[r.PrecioId] || r.ProductoInventarioId > maxInventarioId[r.PrecioId]) {
  //         maxInventarioId[r.PrecioId] = r.ProductoInventarioId;
  //       }
  //     }
  //   });
  //   $(self.kit_registrosInventario()).each(function (idx, r) {
  //     if (r.ProductoId == self.kit_productoSeleccionado().ProductoId
  //       && r.UnidadMedidaVentaId == unidad.Id
  //       && r.ProductoInventarioId == maxInventarioId[r.PrecioId]
  //       && !precioIds.has(r.PrecioId)) {
  //       precioIds.add(r.PrecioId);
  //       self.kit_precios.push({
  //         Id: r.PrecioId,
  //         Precio: r.PrecioNombre + " (Q " + r.PrecioValor + ")",
  //         PrecioValor: r.PrecioValor
  //       });
  //     }
  //   });
  // });

  // ── Agregar producto a la lista temporal (antes de guardar encabezado) ────
  self.agregarProductoAlKit = function () {
    var p = self.kit_productoSeleccionado();
    if (!p) {
      mensajeEmergenteError("Seleccione un producto.");
      return;
    }
    var cantidad = parseFloat(self.kit_cantidad()) || 1;

    self.kit_productosAgregados.push({
      ProductoId: p.ProductoId,
      ProductoCodigo: p.ProductoCodigo,
      ProductoNombre: p.ProductoNombre,
      Cantidad: ko.observable(cantidad),
      Utilizado: ko.observable(0),
      ValorUnitario: ko.observable(0),
      ValorSubtotal: ko.observable(0),
      ValorTotal: ko.observable(0),
      UnidadMedidaVentaId: null,
      UnidadMedidaVentaNombre: ko.observable(""),
      PrecioId: null,
      PrecioNombre: ko.observable(""),
      DescuentoPorcentaje: ko.observable(0),
      DescuentoValor: ko.observable(0),
      Eliminado: ko.observable(false)
    });

    self.actualizarTotalesKit();
    mensajeEmergente("Producto agregado al kit.");
  };

  self.actualizarTotalesKit = function () {
    var total = 0;
    $(self.kit_productosAgregados()).each(function (idx, p) {
      if (!p.Eliminado()) {
        var sub = parseFloat(p.Cantidad()) * parseFloat(p.ValorUnitario());
        p.ValorSubtotal(sub.toFixed(2));
        p.ValorTotal(sub.toFixed(2));
        total += sub;
      }
    });
    self.kit_total(total.toFixed(2));
  };

  self.quitarProductoKit = function (producto) {
    producto.Eliminado(true);
    self.actualizarTotalesKit();
  };

  // ── Guardar kit completo (encabezado + productos) ─────────────────────────
  self.guardarKitIngreso = function () {
    var productosActivos = self.kit_productosAgregados().filter(function (p) { return !p.Eliminado(); });
    if (productosActivos.length === 0) { mensajeEmergenteError("Agregue al menos un producto al kit."); return; }
    showLoading();
    var encabezado = {
      HospitalizacionId: parseInt($("#HospitalizacionId").val()),
      NombreKit: self.kit_NombreKit() || "Kit de Ingreso",
      NombrePaciente: self.kit_NombrePaciente(),
      Medico: self.kit_Medico(),
      Procedimiento: self.kit_Procedimiento(),
      Responsable: self.kit_Responsable(),
      Utilizado: self.kit_Utilizado(),
      FechaKit: self.kit_Fecha() || null,
    };
    $.ajax({
      url: "/KitIngreso/AgregarKitIngreso",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(encabezado),
      success: function (dataResult) {
        var data = JSON.parse(dataResult);
        if (!data.exitoso) { hideLoading(); mensajeEmergenteError(data.resultado || "Error al crear kit."); return; }
        var kitId = data.resultado;
        var pendientes = productosActivos.length;
        var errores = 0;
        $(productosActivos).each(function (idx, p) {
          var detalle = {
            KitIngresoId: kitId,
            ProductoId: p.ProductoId,
            ProductoCodigo: p.ProductoCodigo,
            ProductoNombre: p.ProductoNombre,
            UnidadMedidaVentaId: p.UnidadMedidaVentaId,
            UnidadMedidaVentaNombre: ko.unwrap(p.UnidadMedidaVentaNombre),
            PrecioId: p.PrecioId,
            PrecioNombre: ko.unwrap(p.PrecioNombre),
            Cantidad: parseFloat(ko.unwrap(p.Cantidad)),
            ValorUnitario: parseFloat(ko.unwrap(p.ValorUnitario)),
            ValorSubtotal: parseFloat(ko.unwrap(p.ValorSubtotal)),
            DescuentoPorcentaje: parseFloat(ko.unwrap(p.DescuentoPorcentaje)),
            DescuentoValor: parseFloat(ko.unwrap(p.DescuentoValor)),
            ValorTotal: parseFloat(ko.unwrap(p.ValorTotal)),
          };
          $.ajax({
            url: "/KitIngreso/AgregarProductoKitIngreso",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(detalle),
            success: function (r2) {
              pendientes--;
              if (pendientes === 0) {
                hideLoading();
                if (errores === 0) {
                  self.consultarKitsIngreso();
                  self.limpiarModalKitIngreso();
                  $("#mdl-agregar-kit-ingreso").dialog("close");
                  mensajeEmergente("Kit de ingreso guardado exitosamente.");
                } else {
                  mensajeEmergenteError("Kit creado pero " + errores + " producto(s) no se guardaron.");
                }
              }
            },
            error: function () { errores++; pendientes--; if (pendientes === 0) { hideLoading(); } }
          });
        });
      },
      error: function () { hideLoading(); mensajeEmergenteError("Error al guardar kit de ingreso."); }
    });
  };

  self.limpiarModalKitIngreso = function () {
    self.kit_NombreKit("Kit de Ingreso");
    self.kit_NombrePaciente(""); self.kit_Medico(""); self.kit_Procedimiento("");
    self.kit_Responsable(""); self.kit_Fecha("");
    self.kit_productosAgregados([]); self.kit_total(0);
    // self.kit_productoSeleccionado(null); self.kit_unidadesVenta([]);
    // self.kit_precios([]); self.kit_cantidad(1); self.kit_kitIdActivo(null);
  };

  // ── Consultar kits por hospitalización ───────────────────────────────────


  self.consultarKitsIngreso = function () {
    showLoading();
    var hospitalizacionId = $("#HospitalizacionId").val();
    $.ajax({
      url: "/KitIngreso/ObtenerKitsGlobales",
      method: "POST",
      data: { hospitalizacionId: hospitalizacionId },  // ← clave
      success: function (data) {
        hideLoading();

        if (!data.exitoso) {
          mensajeEmergenteError(data.mensaje || "Error al obtener kits.");
          return;
        }

        var items = data.resultado || [];
        if (items.length === 0) {
          self.kitsIngreso([]);
          return;
        }

        var primerItem = items[0];
        var esFormatoKit = primerItem.hasOwnProperty("Detalles") || primerItem.hasOwnProperty("detalles");
        var kitsVM = [];

        if (esFormatoKit) {
          items.forEach(function (kit) {
            var detallesArray = (kit.Detalles || kit.detalles || []).map(function (d) {
              return {
                Id: d.Id || d.id,
                KitIngresoId: d.KitIngresoId || d.kitIngresoId,
                ProductoId: d.ProductoId || d.productoId,
                ProductoCodigo: d.ProductoCodigo || d.productoCodigo,
                ProductoNombre: d.ProductoNombre || d.productoNombre,
                Cantidad: ko.observable(d.Cantidad || d.cantidad || 0),
                Utilizado: ko.observable(0),
                Eliminado: ko.observable(d.Eliminado || d.eliminado || false)
              };
            });
            kitsVM.push({
              Id: kit.Id || kit.id,
              FechaRegistro: kit.FechaRegistro || kit.fechaRegistro,
              FechaKit: kit.FechaKit || kit.fechaKit,
              NombreKit: ko.observable(kit.NombreKit || kit.nombreKit || "Kit de Ingreso"),
              NombrePaciente: kit.NombrePaciente || kit.nombrePaciente,
              Medico: kit.Medico || kit.medico,
              Procedimiento: kit.Procedimiento || kit.procedimiento,
              Responsable: kit.Responsable || kit.responsable,
              Detalles: ko.observableArray(detallesArray)
            });
          });
        } else {
          var grupos = new Map();
          items.forEach(function (detalle) {
            var kitId = detalle.KitIngresoId || detalle.kitIngresoId;
            if (!kitId) return;

            if (!grupos.has(kitId)) {
              grupos.set(kitId, {
                Id: kitId,
                FechaRegistro: detalle.FechaRegistro || detalle.fechaRegistro,
                FechaKit: detalle.FechaKit || detalle.fechaKit,
                NombreKit: detalle.NombreKit || detalle.nombreKit || "Kit de Ingreso",
                NombrePaciente: detalle.NombrePaciente || detalle.nombrePaciente,
                Medico: detalle.Medico || detalle.medico,
                Procedimiento: detalle.Procedimiento || detalle.procedimiento,
                Responsable: detalle.Responsable || detalle.responsable,
                Detalles: []
              });
            }
            var kit = grupos.get(kitId);
            kit.Detalles.push({
              Id: detalle.Id || detalle.id,
              KitIngresoId: kitId,
              ProductoId: detalle.ProductoId || detalle.productoId,
              ProductoCodigo: detalle.ProductoCodigo || detalle.productoCodigo,
              ProductoNombre: detalle.ProductoNombre || detalle.productoNombre,
              Cantidad: ko.observable(detalle.Cantidad || detalle.cantidad || 0),
              Utilizado: ko.observable(0),
              Eliminado: ko.observable(detalle.Eliminado || detalle.eliminado || false)
            });
          });

          for (var [_, kit] of grupos.entries()) {
            kitsVM.push({
              Id: kit.Id,
              FechaRegistro: kit.FechaRegistro,
              FechaKit: kit.FechaKit,
              NombreKit: ko.observable(kit.NombreKit),
              NombrePaciente: kit.NombrePaciente,
              Medico: kit.Medico,
              Procedimiento: kit.Procedimiento,
              Responsable: kit.Responsable,
              Detalles: ko.observableArray(kit.Detalles)
            });
          }
        }

        self.kitsIngreso(kitsVM);

        self.cargarUtilizadosParaHospitalizacion();
      },
      error: function (err) {
        hideLoading();
        console.error("Error en petición:", err);
        mensajeEmergenteError("Error de red al cargar kits.");
      }
    });
  };

  // self.consultarKitsIngreso = function () {
  //   showLoading();
  //   $.ajax({
  //     url: "/KitIngreso/ObtenerKitsGlobales",
  //     method: "POST",
  //     success: function (data) {
  //       hideLoading();
  //       console.log("📦 Respuesta ObtenerKitsGlobales:", data);

  //       if (!data.exitoso) {
  //         mensajeEmergenteError(data.mensaje || "Error al obtener kits.");
  //         return;
  //       }

  //       var items = data.resultado || [];
  //       if (items.length === 0) {
  //         self.kitsIngreso([]);
  //         return;
  //       }

  //       // DETECCIÓN DEL FORMATO
  //       var primerItem = items[0];
  //       var esFormatoKit = primerItem.hasOwnProperty("Detalles") || primerItem.hasOwnProperty("detalles");
  //       var kitsVM = [];

  //       if (esFormatoKit) {
  //         // Formato 1: Ya son objetos Kit (con Detalles anidados)
  //         items.forEach(function (kit) {
  //           var detallesArray = (kit.Detalles || kit.detalles || []).map(function (d) {
  //             return {
  //               Id: d.Id || d.id,
  //               KitIngresoId: d.KitIngresoId || d.kitIngresoId,
  //               ProductoId: d.ProductoId || d.productoId,
  //               ProductoCodigo: d.ProductoCodigo || d.productoCodigo,
  //               ProductoNombre: d.ProductoNombre || d.productoNombre,
  //               Cantidad: ko.observable(d.Cantidad || d.cantidad || 0),
  //               Utilizado: ko.observable(0),
  //               Eliminado: ko.observable(d.Eliminado || d.eliminado || false)
  //             };
  //           });
  //           kitsVM.push({
  //             Id: kit.Id || kit.id,
  //             FechaRegistro: kit.FechaRegistro || kit.fechaRegistro,
  //             FechaKit: kit.FechaKit || kit.fechaKit,
  //             NombreKit: ko.observable(kit.NombreKit || kit.nombreKit || "Kit de Ingreso"),
  //             NombrePaciente: kit.NombrePaciente || kit.nombrePaciente,
  //             Medico: kit.Medico || kit.medico,
  //             Procedimiento: kit.Procedimiento || kit.procedimiento,
  //             Responsable: kit.Responsable || kit.responsable,
  //             Detalles: ko.observableArray(detallesArray)
  //           });
  //         });
  //       } else {
  //         // Formato 2: Lista plana de detalles → hay que agrupar por kitIngresoId
  //         var grupos = new Map();
  //         items.forEach(function (detalle) {
  //           var kitId = detalle.KitIngresoId || detalle.kitIngresoId;
  //           if (!kitId) return;

  //           if (!grupos.has(kitId)) {
  //             grupos.set(kitId, {
  //               Id: kitId,
  //               FechaRegistro: detalle.FechaRegistro || detalle.fechaRegistro,
  //               FechaKit: detalle.FechaKit || detalle.fechaKit,
  //               NombreKit: detalle.NombreKit || detalle.nombreKit || "Kit de Ingreso",
  //               NombrePaciente: detalle.NombrePaciente || detalle.nombrePaciente,
  //               Medico: detalle.Medico || detalle.medico,
  //               Procedimiento: detalle.Procedimiento || detalle.procedimiento,
  //               Responsable: detalle.Responsable || detalle.responsable,
  //               Detalles: []
  //             });
  //           }
  //           var kit = grupos.get(kitId);
  //           kit.Detalles.push({
  //             Id: detalle.Id || detalle.id,
  //             KitIngresoId: kitId,
  //             ProductoId: detalle.ProductoId || detalle.productoId,
  //             ProductoCodigo: detalle.ProductoCodigo || detalle.productoCodigo,
  //             ProductoNombre: detalle.ProductoNombre || detalle.productoNombre,
  //             Cantidad: ko.observable(detalle.Cantidad || detalle.cantidad || 0),
  //             Utilizado: ko.observable(0),
  //             Eliminado: ko.observable(detalle.Eliminado || detalle.eliminado || false)
  //           });
  //         });

  //         for (var [_, kit] of grupos.entries()) {
  //           kitsVM.push({
  //             Id: kit.Id,
  //             FechaRegistro: kit.FechaRegistro,
  //             FechaKit: kit.FechaKit,
  //             NombreKit: ko.observable(kit.NombreKit),
  //             NombrePaciente: kit.NombrePaciente,
  //             Medico: kit.Medico,
  //             Procedimiento: kit.Procedimiento,
  //             Responsable: kit.Responsable,
  //             Detalles: ko.observableArray(kit.Detalles)
  //           });
  //         }
  //       }

  //       self.kitsIngreso(kitsVM);
  //       console.log("✅ Kits procesados:", self.kitsIngreso().length);

  //       // Cargar los utilizados para esta hospitalización
  //       self.cargarUtilizadosParaHospitalizacion();
  //     },
  //     error: function (err) {
  //       hideLoading();
  //       console.error("Error en petición:", err);
  //       mensajeEmergenteError("Error de red al cargar kits.");
  //     }
  //   });
  // };

  self.cargarUtilizadosParaHospitalizacion = function () {
    var hospitalizacionId = $("#HospitalizacionId").val();
    self.kitsIngreso().forEach(function (kit) {
      kit.Detalles().forEach(function (detalle) {
        $.ajax({
          url: "/KitIngreso/ObtenerUtilizadoKitDetalle",
          method: "POST",
          async: false,
          data: { detalleId: detalle.Id, hospitalizacionId: hospitalizacionId },
          success: function (res) {
            if (res.exitoso) {
              detalle.Utilizado(res.utilizado);
            }
          }
        });
      });
    });
  };
  // self.consultarKitsIngreso = function () {
  //   showLoading();
  //   $.ajax({
  //     url: "/KitIngreso/ObtenerKitsIngreso",
  //     method: "POST",
  //     data: { idHospitalizacion: $("#HospitalizacionId").val() },
  //     success: function (dataResult) {
  //       hideLoading();
  //       var data = JSON.parse(dataResult);
  //       if (data.exitoso) {
  //         var kits = (data.resultado || []).map(function (kit) {
  //           var detallesArray = (kit.Detalles || []).map(function (d) {
  //             return {
  //               Id: d.Id,
  //               KitIngresoId: d.KitIngresoId,
  //               ProductoId: d.ProductoId,
  //               ProductoCodigo: d.ProductoCodigo,
  //               ProductoNombre: d.ProductoNombre,
  //               UnidadMedidaVentaId: d.UnidadMedidaVentaId,
  //               UnidadMedidaVentaNombre: ko.observable(d.UnidadMedidaVentaNombre || ""),
  //               PrecioId: d.PrecioId,
  //               PrecioNombre: ko.observable(d.PrecioNombre || ""),
  //               Cantidad: ko.observable(d.Cantidad || 0),
  //               Utilizado: ko.observable(d.Utilizado || 0),
  //               ValorUnitario: ko.observable(d.ValorUnitario || 0),
  //               Eliminado: ko.observable(d.Eliminado || false)
  //             };
  //           });
  //           var detalles = ko.observableArray(detallesArray);
  //           return {
  //             Id: kit.Id,
  //             FechaRegistro: kit.FechaRegistro,
  //             FechaKit: kit.FechaKit,
  //             NombreKit: ko.observable(kit.NombreKit || "Kit de Ingreso"),
  //             editandoNombre: ko.observable(false),
  //             NombrePaciente: kit.NombrePaciente,
  //             Medico: kit.Medico,
  //             Procedimiento: kit.Procedimiento,
  //             Responsable: kit.Responsable,
  //             Detalles: detalles
  //           };
  //         });
  //         self.kitsIngreso(kits);
  //       } else {
  //         mensajeEmergenteError(data.mensaje || "Error al obtener kits.");
  //       }
  //     },
  //     error: function () {
  //       hideLoading();
  //       mensajeEmergenteError("Error de red.");
  //     }
  //   });
  // };

  // self.verDetalleKitIngreso = function (kit) {
  //   self.kitIngresoDetalle(kit);
  //   // Cargar productos disponibles para el select (si no están cargados)
  //   if (self.kit_productosExistentes().length === 0) {
  //     self.kit_cargarProductosInventario();
  //   }
  //   var modal = new bootstrap.Modal(document.getElementById('mdl-detalle-kit-ingreso'));
  //   modal.show();
  // };
  self.verDetalleKitIngreso = function (kit) {
    // Obtener valores actuales de la hospitalización (desde los hidden inputs)
    var currentPaciente = $("#PacienteNombreAdmision").val() || "";
    var currentMedico = $("#MedicoAsignadoNombre").val() || "";
    var currentProcedimiento = $("#CitaProcedimiento").val() || "";
    var currentResponsable = $("#ResponsableNombre").val() || "";

    // Forzar que el kit tenga los datos correctos de la hospitalización actual
    kit.NombrePaciente = currentPaciente;
    kit.Medico = currentMedico;
    kit.Procedimiento = currentProcedimiento;
    kit.Responsable = currentResponsable;

    self.kitIngresoDetalle(kit);
    // Cargar productos disponibles para el select (si no están cargados)
    if (self.kit_productosExistentes().length === 0) {
      self.kit_cargarProductosInventario();
    }
    var modal = new bootstrap.Modal(document.getElementById('mdl-detalle-kit-ingreso'));
    modal.show();
  };
  // self.clonarKitIngreso = function (kit) {
  //   var nombre = ko.unwrap(kit.NombreKit) || kit.NombrePaciente || "este kit";
  //   if (!confirm('¿Reutilizar el kit "' + nombre + '"? Se creará una copia para esta hospitalización.')) return;
  //   showLoading();
  //   $.ajax({
  //     url: "/KitIngreso/ClonarKitIngreso",
  //     method: "POST",
  //     data: {
  //       kitOrigenId: kit.Id,
  //       hospitalizacionDestinoId: $("#HospitalizacionId").val()
  //     },
  //     success: function (dataResult) {
  //       hideLoading();
  //       var data = JSON.parse(dataResult);
  //       if (data.exitoso) {
  //         self.consultarKitsIngreso();
  //         mensajeEmergente("Kit reutilizado correctamente.");
  //       } else {
  //         mensajeEmergenteError(data.mensaje || "Error al reutilizar kit.");
  //       }
  //     },
  //     error: function () {
  //       hideLoading();
  //       mensajeEmergenteError("Error de red al reutilizar kit.");
  //     }
  //   });
  // };
  self.clonarKitIngreso = function (kit) {
    if (!confirm("¿Desea reutilizar este kit en esta hospitalización? Se creará una copia.")) return;
    showLoading();
    $.ajax({
      url: "/KitIngreso/ClonarKitIngreso",
      method: "POST",
      data: { kitOrigenId: kit.Id, hospitalizacionDestinoId: $("#HospitalizacionId").val() },
      success: function (response) {
        hideLoading();
        var data = typeof response === "string" ? JSON.parse(response) : response;
        if (data.exitoso) {
          self.consultarKitsIngreso();
          mensajeEmergente("Kit clonado correctamente.");
        } else {
          mensajeEmergenteError(data.mensaje || "Error al clonar kit.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de red al clonar kit.");
      }
    });
  };

  // ── Abrir modal para agregar producto a kit ya existente ──────────────────
  self.abrirAgregarProductoKit = function (kit) {
    self.kit_kitIdActivo(kit.Id);
    self.kit_productoSeleccionado(null);
    // self.kit_unidadesVenta([]); self.kit_precios([]);
    self.kit_cantidad(1);
    var el = document.getElementById("mdl-agregar-producto-kit");
    var modal = bootstrap.Modal.getOrCreateInstance(el);
    modal.show();
  };

  self.guardarProductoEnKitExistente = function () {
    var producto = self.kit_productoSeleccionado();
    var cantidad = parseFloat(self.kit_cantidad()) || 1;
    var kitId = self.kit_kitIdActivo();

    if (!producto) {
      mensajeEmergenteError("Seleccione un producto.");
      return;
    }
    if (!kitId) {
      mensajeEmergenteError("No hay kit activo.");
      return;
    }

    var nuevoDetalle = {
      KitIngresoId: kitId,
      ProductoId: producto.ProductoId,
      ProductoCodigo: producto.ProductoCodigo,
      ProductoNombre: producto.ProductoNombre,
      UnidadMedidaVentaId: null,
      UnidadMedidaVentaNombre: "",
      PrecioId: null,
      PrecioNombre: "",
      Cantidad: cantidad,
      Utilizado: 0,
      ValorUnitario: 0,
      ValorSubtotal: 0,
      DescuentoPorcentaje: 0,
      DescuentoValor: 0,
      ValorTotal: 0,
      Eliminado: false
    };

    showLoading();
    $.ajax({
      url: "/KitIngreso/AgregarProductoKitIngreso",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(nuevoDetalle),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          var kitActual = self.kitIngresoDetalle();
          if (kitActual && kitActual.Id === kitId) {
            // Volver a cargar los detalles desde el servidor
            $.ajax({
              url: "/KitIngreso/ObtenerKitsIngreso",
              method: "POST",
              data: { idHospitalizacion: $("#HospitalizacionId").val() },
              async: false,
              success: function (refreshResult) {
                var refreshData = JSON.parse(refreshResult);
                if (refreshData.exitoso) {
                  var kitRefrescado = refreshData.resultado.find(function (k) { return k.Id === kitId; });
                  if (kitRefrescado) {
                    var detallesTransformados = (kitRefrescado.Detalles || []).map(function (d) {
                      return {
                        Id: d.Id,
                        KitIngresoId: d.KitIngresoId,
                        ProductoId: d.ProductoId,
                        ProductoCodigo: d.ProductoCodigo,
                        ProductoNombre: d.ProductoNombre,
                        UnidadMedidaVentaId: d.UnidadMedidaVentaId,
                        UnidadMedidaVentaNombre: ko.observable(d.UnidadMedidaVentaNombre || ""),
                        PrecioId: d.PrecioId,
                        PrecioNombre: ko.observable(d.PrecioNombre || ""),
                        Cantidad: ko.observable(d.Cantidad || 0),
                        Utilizado: ko.observable(d.Utilizado || 0),
                        ValorUnitario: ko.observable(d.ValorUnitario || 0),
                        Eliminado: ko.observable(d.Eliminado || false)
                      };
                    });
                    kitActual.Detalles = detallesTransformados;
                    self.kitIngresoDetalle.valueHasMutated();
                  }
                }
              }
            });
          }
          mensajeEmergente("Producto agregado al kit.");
          bootstrap.Modal.getInstance(document.getElementById("mdl-agregar-producto-kit")).hide();
        } else {
          mensajeEmergenteError(data.resultado || "Error al agregar producto.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de red al agregar producto.");
      }
    });
  };

  // ── Eliminar detalle desde el modal de detalle ────────────────────────────
  self.eliminarDetalleKit = function (detalle) {
    if (!confirm("¿Eliminar este producto del kit?")) return;
    showLoading();
    $.ajax({
      url: "/KitIngreso/EliminarDetalleKitIngreso",
      method: "POST",
      data: { detalleId: detalle.Id },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          detalle.Eliminado(true);
          var kit = self.kitIngresoDetalle();
          self.recalcularTotalKit(kit);
          mensajeEmergente("Producto eliminado del kit.");
        } else {
          mensajeEmergenteError(data.mensaje || "Error al eliminar.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de red al eliminar.");
      }
    });
  };

  self.recalcularTotalKit = function () { };


  self.verModalAgregarPreanestesico = function () {
    var hoy = new Date().toISOString().split("T")[0];
    self.pa_FechaCuestionario(hoy);

    // Datos del paciente
    self.pa_NombreCompleto($("#PacienteNombreAdmision").val() || "");
    self.pa_Edad($("#PacienteEdad").val() || "");
    self.pa_Cirujano($("#MedicoAsignadoNombre").val() || "");
    self.pa_Peso($("#PacientePeso").val() || "");
    self.pa_Estatura($("#PacienteTalla").val() || "");
    self.pa_ProcedimientoProgramado($("#CitaProcedimiento").val() || "");

    $("#mdl-agregar-preanestesico").dialog({
      width: 1100,
      title: "Cuestionario Pre-Anestésico",
    });
  };

  self.verDetalleCuestionarioPreAnest = function (item) {
    self.cuestionarioPreAnestDetalle(item);
    var modal = new bootstrap.Modal(document.getElementById("mdl-detalle-preanestesico"));
    modal.show();
  };

  self.agregarCuestionarioPreAnest = function () {
    function radio(name) {
      var el = document.querySelector('#mdl-agregar-preanestesico input[name="' + name + '"]:checked');
      return el ? el.value : "";
    }
    var payload = {
      HospitalizacionId: parseInt($("#HospitalizacionId").val()),
      NombreCompleto: self.pa_NombreCompleto(),
      RegistroMedico: self.pa_RegistroMedico(),
      Edad: self.pa_Edad(),
      FechaCuestionario: self.pa_FechaCuestionario() || null,
      Peso: parseFloat(self.pa_Peso()) || null,
      Estatura: parseFloat(self.pa_Estatura()) || null,
      FechaUltimaRegla: self.pa_FechaUltimaRegla() || null,
      FechaProcedimiento: self.pa_FechaProcedimiento() || null,
      ProcedimientoProgramado: self.pa_ProcedimientoProgramado(),
      Cirujano: self.pa_Cirujano(),
      // Antecedentes
      PA_Alergia: radio("pa_alergia"),
      PA_AlergiaCual: self.pa_AlergiaCual(),
      PA_Fuma: radio("pa_fuma"),
      PA_FumaCuanto: self.pa_FumaCuanto(),
      PA_Drogas: radio("pa_drogas"),
      PA_DrogasCuales: self.pa_DrogasCuales(),
      PA_Alcohol: radio("pa_alcohol"),
      PA_AlcoholCuanto: self.pa_AlcoholCuanto(),
      PA_Embarazo: radio("pa_embarazo"),
      PA_EmbarazoCual: self.pa_EmbarazoCual(),
      PA_Transfusion: radio("pa_transfusion"),
      PA_TransfusionCual: self.pa_TransfusionCual(),
      PA_Asma: radio("pa_asma"),
      PA_AsmaCual: self.pa_AsmaCual(),
      PA_Pulmones: radio("pa_pulmones"),
      PA_PulmonesCual: self.pa_PulmonesCual(),
      PA_Corazon: radio("pa_corazon"),
      PA_CorazonCual: self.pa_CorazonCual(),
      PA_AtaqueCardiaco: radio("pa_ataque"),
      PA_AtaqueCardiacoCual: self.pa_AtaqueCardiacoCual(),
      PA_Angina: radio("pa_angina"),
      PA_AnginaCual: self.pa_AnginaCual(),
      PA_Soplo: radio("pa_soplo"),
      PA_SoploCual: self.pa_SoploCual(),
      PA_Presion: radio("pa_presion"),
      PA_PresionCual: self.pa_PresionCual(),
      PA_Higado: radio("pa_higado"),
      PA_HigadoCual: self.pa_HigadoCual(),
      PA_Rinones: radio("pa_rinones"),
      PA_RinonesCual: self.pa_RinonesCual(),
      PA_Diabetes: radio("pa_diabetes"),
      PA_DiabetesCual: self.pa_DiabetesCual(),
      PA_Epilepsia: radio("pa_epilepsia"),
      PA_EpilepsiaCual: self.pa_EpilepsiaCual(),
      PA_Derrame: radio("pa_derrame"),
      PA_DerrameCual: self.pa_DerrameCual(),
      PA_Tiroides: radio("pa_tiroides"),
      PA_TiroidesCual: self.pa_TiroidesCual(),
      PA_Anestesico: radio("pa_anestesico"),
      PA_AnestesicoCual: self.pa_AnestesicoCual(),
      PA_AceptaTransfusion: radio("pa_acepta_trans"),
      PA_AceptaTransfusionCual: self.pa_AceptaTransfusionCual(),
      // Información adicional
      AI_Medicamentos: self.pa_AI_Medicamentos(),
      AI_Actividad: radio("pa_actividad"),
      AI_ActividadDetalle: self.pa_AI_ActividadDetalle(),
      AI_OperacionesPrevias: self.pa_AI_OperacionesPrevias(),
      AI_Comentarios: self.pa_AI_Comentarios(),
    };

    showLoading();
    $.ajax({
      url: "/CuestionarioPreAnestesico/AgregarCuestionario",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(payload),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarCuestionariosPreAnest();
          self.limpiarModalPreanestesico();
          $("#mdl-agregar-preanestesico").dialog("close");
          mensajeEmergente("Cuestionario pre-anestésico guardado exitosamente");
        } else {
          mensajeEmergenteError(data.resultado || "Error al guardar.");
        }
      },
      error: function (err) {
        hideLoading();
        mensajeEmergenteError("Error al guardar el cuestionario pre-anestésico.");
      },
    });
  };

  self.limpiarModalPreanestesico = function () {
    self.pa_NombreCompleto(""); self.pa_RegistroMedico(""); self.pa_Edad("");
    self.pa_FechaCuestionario(""); self.pa_Peso(""); self.pa_Estatura("");
    self.pa_FechaUltimaRegla(""); self.pa_FechaProcedimiento("");
    self.pa_ProcedimientoProgramado(""); self.pa_Cirujano("");
    self.pa_AlergiaCual(""); self.pa_FumaCuanto(""); self.pa_DrogasCuales(""); self.pa_AlcoholCuanto("");
    self.pa_TransfusionCual(""); self.pa_AsmaCual(""); self.pa_PulmonesCual(""); self.pa_CorazonCual("");
    self.pa_AtaqueCardiacoCual(""); self.pa_AnginaCual(""); self.pa_SoploCual(""); self.pa_PresionCual("");
    self.pa_HigadoCual(""); self.pa_RinonesCual(""); self.pa_DiabetesCual(""); self.pa_EpilepsiaCual("");
    self.pa_DerrameCual(""); self.pa_TiroidesCual(""); self.pa_AnestesicoCual(""); self.pa_AceptaTransfusionCual(""); self.pa_EmbarazoCual("");
    self.pa_AI_Medicamentos(""); self.pa_AI_ActividadDetalle("");
    self.pa_AI_OperacionesPrevias(""); self.pa_AI_Comentarios("");
    document.querySelectorAll('#mdl-agregar-preanestesico input[type=radio]').forEach(function (r) { r.checked = false; });
  };

  self.consultarCuestionariosPreAnest = function () {
    showLoading();
    $.ajax({
      url: "/CuestionarioPreAnestesico/ObtenerCuestionarios",
      method: "POST",
      data: { idHospitalizacion: $("#HospitalizacionId").val() },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          function prop(obj, name) {
            if (obj[name] !== undefined && obj[name] !== null) return obj[name];
            var lower = name.charAt(0).toLowerCase() + name.slice(1);
            return (obj[lower] !== undefined && obj[lower] !== null) ? obj[lower] : "";
          }
          var lista = (data.resultado || []).map(function (n) {
            return {
              Id: prop(n, "Id"),
              FechaRegistro: prop(n, "FechaRegistro"),
              FechaCuestionario: prop(n, "FechaCuestionario"),
              NombreCompleto: prop(n, "NombreCompleto"),
              ProcedimientoProgramado: prop(n, "ProcedimientoProgramado"),
              Cirujano: prop(n, "Cirujano"),
              Peso: prop(n, "Peso"),
              Estatura: prop(n, "Estatura"),
              FechaProcedimiento: prop(n, "FechaProcedimiento"),
              PA_Alergia: prop(n, "PA_Alergia"),
              PA_AlergiaCual: prop(n, "PA_AlergiaCual"),
              PA_Fuma: prop(n, "PA_Fuma"),
              PA_FumaCuanto: prop(n, "PA_FumaCuanto"),
              PA_Drogas: prop(n, "PA_Drogas"),
              PA_DrogasCuales: prop(n, "PA_DrogasCuales"),
              PA_Alcohol: prop(n, "PA_Alcohol"),
              PA_AlcoholCuanto: prop(n, "PA_AlcoholCuanto"),
              PA_Embarazo: prop(n, "PA_Embarazo"),
              PA_EmbarazoCual: prop(n, "PA_EmbarazoCual"),
              PA_Transfusion: prop(n, "PA_Transfusion"),
              PA_TransfusionCual: prop(n, "PA_TransfusionCual"),
              PA_Asma: prop(n, "PA_Asma"),
              PA_AsmaCual: prop(n, "PA_AsmaCual"),
              PA_Pulmones: prop(n, "PA_Pulmones"),
              PA_PulmonesCual: prop(n, "PA_PulmonesCual"),
              PA_Corazon: prop(n, "PA_Corazon"),
              PA_CorazonCual: prop(n, "PA_CorazonCual"),
              PA_AtaqueCardiaco: prop(n, "PA_AtaqueCardiaco"),
              PA_AtaqueCardiacoCual: prop(n, "PA_AtaqueCardiacoCual"),
              PA_Angina: prop(n, "PA_Angina"),
              PA_AnginaCual: prop(n, "PA_AnginaCual"),
              PA_Soplo: prop(n, "PA_Soplo"),
              PA_SoploCual: prop(n, "PA_SoploCual"),
              PA_Presion: prop(n, "PA_Presion"),
              PA_PresionCual: prop(n, "PA_PresionCual"),
              PA_Higado: prop(n, "PA_Higado"),
              PA_HigadoCual: prop(n, "PA_HigadoCual"),
              PA_Rinones: prop(n, "PA_Rinones"),
              PA_RinonesCual: prop(n, "PA_RinonesCual"),
              PA_Diabetes: prop(n, "PA_Diabetes"),
              PA_DiabetesCual: prop(n, "PA_DiabetesCual"),
              PA_Epilepsia: prop(n, "PA_Epilepsia"),
              PA_EpilepsiaCual: prop(n, "PA_EpilepsiaCual"),
              PA_Derrame: prop(n, "PA_Derrame"),
              PA_DerrameCual: prop(n, "PA_DerrameCual"),
              PA_Tiroides: prop(n, "PA_Tiroides"),
              PA_TiroidesCual: prop(n, "PA_TiroidesCual"),
              PA_Anestesico: prop(n, "PA_Anestesico"),
              PA_AnestesicoCual: prop(n, "PA_AnestesicoCual"),
              PA_AceptaTransfusion: prop(n, "PA_AceptaTransfusion"),
              PA_AceptaTransfusionCual: prop(n, "PA_AceptaTransfusionCual"),
              AI_Medicamentos: prop(n, "AI_Medicamentos"),
              AI_Actividad: prop(n, "AI_Actividad"),
              AI_ActividadDetalle: prop(n, "AI_ActividadDetalle"),
              AI_OperacionesPrevias: prop(n, "AI_OperacionesPrevias"),
              AI_Comentarios: prop(n, "AI_Comentarios"),
            };
          });
          self.cuestionariosPreAnest(lista);
        } else {
          mensajeEmergenteError(data.mensaje || "Error al obtener cuestionarios.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de red al consultar cuestionarios.");
      },
    });
  };

  self.verDetalleListaChequeo = function (item) {
    self.listaChequeoDetalle(item);
    var modal = new bootstrap.Modal(document.getElementById("mdl-detalle-chequeo"));
    modal.show();
  };

  self.agregarListaChequeo = function () {
    // ===== FORZAR DATOS ACTUALES DESDE HIDDEN INPUTS =====
    // Procedimiento (se toma del campo oculto #CitaProcedimiento)
    var procedimiento = ($("#CitaProcedimiento").val() || "").trim();
    if (procedimiento) {
      self.chk_CI_Operacion(procedimiento);
      self.chk_CP_NombreCirugia(procedimiento);
    }

    // Nombre y apellido del paciente
    var nombreCompleto = ($("#PacienteNombreAdmision").val() || "").trim();
    var partes = nombreCompleto.split(" ");
    var nombre = partes[0] || "";
    var apellido = partes.slice(1).join(" ") || "";
    if (nombre) self.chk_NombrePaciente(nombre);
    if (apellido) self.chk_ApellidoPaciente(apellido);
    if (nombre) {
      self.chk_CI_NombreConfirma(nombre);
      self.chk_CP_NombrePacienteCirujano(nombre);
    }
    if (apellido) {
      self.chk_CI_ApellidoConfirma(apellido);
      self.chk_CP_ApellidoPacienteCirujano(apellido);
    }

    // Fecha de nacimiento (convertir de DD/MM/YYYY a YYYY-MM-DD)
    var fechaNacRaw = ($("#PacienteFechaNacimiento").val() || "").trim();
    var fechaNac = "";
    if (fechaNacRaw) {
      var partesFecha = fechaNacRaw.split("/");
      if (partesFecha.length === 3) {
        fechaNac = partesFecha[2] + "-" + partesFecha[1] + "-" + partesFecha[0];
      } else {
        fechaNac = fechaNacRaw;
      }
    }
    if (fechaNac) {
      self.chk_FechaNacimiento(fechaNac);
      self.chk_CI_FechaNacConfirma(fechaNac);
      self.chk_CP_FechaNacCirujano(fechaNac);
    }

    // ===== RESTO DE LA FUNCIÓN ORIGINAL =====
    function radio(name) {
      var el = document.querySelector('#mdl-agregar-chequeo input[name="' + name + '"]:checked');
      return el ? el.value : "";
    }

    var payload = {
      HospitalizacionId: parseInt($("#HospitalizacionId").val()),
      NombrePaciente: self.chk_NombrePaciente(),
      ApellidoPaciente: self.chk_ApellidoPaciente(),
      FechaNacimiento: self.chk_FechaNacimiento() || null,
      MedicoTratante: self.chk_MedicoTratante(),
      FechaChequeo: self.chk_FechaChequeo() || null,
      HoraChequeo: self.chk_HoraChequeo(),
      CI_NombreConfirma: self.chk_CI_NombreConfirma(),
      CI_ApellidoConfirma: self.chk_CI_ApellidoConfirma(),
      CI_FechaNacConfirma: self.chk_CI_FechaNacConfirma() || null,
      CI_Consentimiento: radio("ci_consentimiento"),
      CI_Operacion: self.chk_CI_Operacion(),
      CI_LadoOperar: radio("ci_lado"),
      CI_SitioMarcado: radio("ci_marcado"),
      CI_Alergia: radio("ci_alergia"),
      CI_EvalPreanestesica: radio("ci_evalanest"),
      CI_AccesoIV: radio("ci_iv"),
      CI_EquipoAnestesia: radio("ci_equanest"),
      CI_Medicamentos: radio("ci_meds"),
      CI_Oximetro: radio("ci_oximetro"),
      CI_EquipoAspiracion: radio("ci_aspir"),
      CI_ViaAerea: radio("ci_via"),
      CP_Presentacion: radio("cp_presentacion"),
      CP_NombrePacienteCirujano: self.chk_CP_NombrePacienteCirujano(),
      CP_ApellidoPacienteCirujano: self.chk_CP_ApellidoPacienteCirujano(),
      CP_FechaNacCirujano: self.chk_CP_FechaNacCirujano() || null,
      CP_NombreCirugia: self.chk_CP_NombreCirugia(),
      CP_EventosCriticos: radio("cp_anticipa"),
      CP_TiempoDuracion: radio("cp_duracion"),
      CP_ImagenesDiagnosticas: radio("cp_imagenes"),
      CP_PerdidaSangre: radio("cp_sangre"),
      CP_Esterilidad: radio("cp_esteril"),
      CP_MaterialesAdicionales: radio("cp_materiales"),
      CP_EventosCriticosAnest: radio("cp_anticipa_anest"),
      CP_ProfilaxisAntibiotica: radio("cp_antibiotico"),
      CP_Tromboprofilaxis: radio("cp_trombo"),
      CP_ManejoDolor: radio("cp_dolor"),
      CS_NombreOperacion: self.chk_CS_NombreOperacion(),
      CS_NombreEnfermera: self.chk_CS_NombreEnfermera(),
      CS_RecuentoCompleto: radio("cs_recuento"),
      CS_EtiquetadoMuestras: radio("cs_muestras"),
      CS_RepasoPostOp: radio("cs_repaso"),
      CS_PorQue: self.chk_CS_PorQue(),
      CS_Traslado: radio("cs_traslado"),
      CS_Complicaciones: self.chk_CS_Complicaciones(),
      CS_ServicioNumCama: self.chk_CS_ServicioNumCama(),
    };

    showLoading();
    $.ajax({
      url: "/ListaChequeo/AgregarListaChequeo",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(payload),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarListasChequeo();
          self.limpiarModalChequeo();
          $("#mdl-agregar-chequeo").dialog("close");
          mensajeEmergente("Lista de chequeo guardada exitosamente");
        } else {
          mensajeEmergenteError(data.resultado || "Error al guardar.");
        }
      },
      error: function (err) {
        hideLoading();
        mensajeEmergenteError("Error al guardar la lista de chequeo.");
      },
    });
  };

  self.limpiarModalChequeo = function () {
    self.chk_NombrePaciente(""); self.chk_ApellidoPaciente(""); self.chk_FechaNacimiento("");
    self.chk_MedicoTratante(""); self.chk_FechaChequeo(""); self.chk_HoraChequeo("");
    self.chk_CI_NombreConfirma(""); self.chk_CI_ApellidoConfirma(""); self.chk_CI_FechaNacConfirma("");
    self.chk_CI_Operacion("");
    self.chk_CP_NombrePacienteCirujano(""); self.chk_CP_ApellidoPacienteCirujano(""); self.chk_CP_FechaNacCirujano(""); self.chk_CP_NombreCirugia("");
    self.chk_CS_NombreOperacion(""); self.chk_CS_NombreEnfermera("");
    self.chk_CS_PorQue(""); self.chk_CS_Complicaciones(""); self.chk_CS_ServicioNumCama("");
    document.querySelectorAll('#mdl-agregar-chequeo input[type=radio]').forEach(function (r) { r.checked = false; });
  };

  self.consultarListasChequeo = function () {
    showLoading();
    $.ajax({
      url: "/ListaChequeo/ObtenerListasChequeo",
      method: "POST",
      data: { idHospitalizacion: $("#HospitalizacionId").val() },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          function prop(obj, name) {
            if (obj[name] !== undefined && obj[name] !== null) return obj[name];
            var lower = name.charAt(0).toLowerCase() + name.slice(1);
            return (obj[lower] !== undefined && obj[lower] !== null) ? obj[lower] : "";
          }
          var lista = (data.resultado || []).map(function (n) {
            return {
              Id: prop(n, "Id"),
              FechaRegistro: prop(n, "FechaRegistro"),
              FechaChequeo: prop(n, "FechaChequeo"),
              HoraChequeo: prop(n, "HoraChequeo"),
              NombrePaciente: prop(n, "NombrePaciente"),
              ApellidoPaciente: prop(n, "ApellidoPaciente"),
              CI_Operacion: prop(n, "CI_Operacion"),
              CI_NombreConfirma: prop(n, "CI_NombreConfirma"),
              CI_ApellidoConfirma: prop(n, "CI_ApellidoConfirma"),
              CI_Consentimiento: prop(n, "CI_Consentimiento"),
              CI_LadoOperar: prop(n, "CI_LadoOperar"),
              CI_SitioMarcado: prop(n, "CI_SitioMarcado"),
              CI_Alergia: prop(n, "CI_Alergia"),
              CI_EvalPreanestesica: prop(n, "CI_EvalPreanestesica"),
              CI_AccesoIV: prop(n, "CI_AccesoIV"),
              CI_EquipoAnestesia: prop(n, "CI_EquipoAnestesia"),
              CI_Medicamentos: prop(n, "CI_Medicamentos"),
              CI_Oximetro: prop(n, "CI_Oximetro"),
              CI_EquipoAspiracion: prop(n, "CI_EquipoAspiracion"),
              CI_ViaAerea: prop(n, "CI_ViaAerea"),
              CP_Presentacion: prop(n, "CP_Presentacion"),
              CP_NombrePacienteCirujano: prop(n, "CP_NombrePacienteCirujano"),
              CP_NombreCirugia: prop(n, "CP_NombreCirugia"),
              CP_EventosCriticos: prop(n, "CP_EventosCriticos"),
              CP_TiempoDuracion: prop(n, "CP_TiempoDuracion"),
              CP_ImagenesDiagnosticas: prop(n, "CP_ImagenesDiagnosticas"),
              CP_PerdidaSangre: prop(n, "CP_PerdidaSangre"),
              CP_Esterilidad: prop(n, "CP_Esterilidad"),
              CP_MaterialesAdicionales: prop(n, "CP_MaterialesAdicionales"),
              CP_EventosCriticosAnest: prop(n, "CP_EventosCriticosAnest"),
              CP_ProfilaxisAntibiotica: prop(n, "CP_ProfilaxisAntibiotica"),
              CP_Tromboprofilaxis: prop(n, "CP_Tromboprofilaxis"),
              CP_ManejoDolor: prop(n, "CP_ManejoDolor"),
              CS_NombreOperacion: prop(n, "CS_NombreOperacion"),
              CS_NombreEnfermera: prop(n, "CS_NombreEnfermera"),
              CS_RecuentoCompleto: prop(n, "CS_RecuentoCompleto"),
              CS_EtiquetadoMuestras: prop(n, "CS_EtiquetadoMuestras"),
              CS_RepasoPostOp: prop(n, "CS_RepasoPostOp"),
              CS_PorQue: prop(n, "CS_PorQue"),
              CS_Traslado: prop(n, "CS_Traslado"),
              CS_Complicaciones: prop(n, "CS_Complicaciones"),
              CS_ServicioNumCama: prop(n, "CS_ServicioNumCama"),
              EntradaCompleta: !!(prop(n, "CI_Consentimiento")),
              PausaCompleta: !!(prop(n, "CP_Presentacion")),
              SalidaCompleta: !!(prop(n, "CS_RecuentoCompleto")),
            };
          });
          self.listasChequeo(lista);
        } else {
          mensajeEmergenteError(data.mensaje || "Error al obtener listas de chequeo.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de red al consultar listas de chequeo.");
      },
    });
  };

  self.verModalAgregarExamenFisico = function () {
    self.consultarDatosExamenFisico();
    self.observacionesExamenFisicoAgregar("");
    $("#mdl-agregar-examen-fisico").dialog({
      width: 1000,
    });
  };
  self.verModalAgregarObservacionProductosHospitalizacion = function (value) {
    var modalElement = document.getElementById(
      "mdl-agregar-observacion-productos-hospitalizacion",
    );
    var modal = new bootstrap.Modal(modalElement);
    (self.observacionesProductsHospiAplicacionId(value.Id),
      self.observacionesProductsHospiUsuarioCrea(value.PersonaCrea),
      self.consultarObservacionProductsHospi());
    modal.show();
  };
  self.verModalAgregarExcretaIncreta = function () {
    $("#mdl-agregar-increta-excreta").dialog({
      width: 1000,
    });
  };


  self.verModalAgregarNotaEnfermeria = function (turnoId) {
    // Buscar turno y setear estado
    var turnoSeleccionado = self.turnosEnfermeria().find(function (t) { return t.Id === turnoId; });
    self.turnoFirmado(turnoSeleccionado ? turnoSeleccionado.Firmado : false);
    self.idTurnoEnfermeria(turnoId);
    self.notasEnfermerias([]); // limpiar mientras carga

    // ✅ Limpiar el contenido del observable y del editor (para una nota nueva)
    self.notaDiagnostico('');
    if (quillEditor) {
      quillEditor.setText('');
    }

    showLoading();
    $.ajax({
      url: "/NotaEnfermeria/ListaNotaEnfermeria",
      method: "POST",
      data: { hospitalizacionId: $("#HospitalizacionId").val() },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          // Normalizar camelCase/PascalCase
          var todasLasNotas = (data.resultado || []).map(function (n) {
            return {
              Id: n.Id ?? n.id,
              FechaRegistro: n.FechaRegistro ?? n.fechaRegistro,
              Profesional: n.Profesional ?? n.profesional ?? "",
              Diagnostico: n.Diagnostico ?? n.diagnostico ?? "",
              Evolucion: n.Evolucion ?? n.evolucion ?? "",
              Sintomas: n.Sintomas ?? n.sintomas ?? "",
              TurnoEnfermeriaId: n.TurnoEnfermeriaId ?? n.turnoEnfermeriaId ?? null,
              Firmado: ko.observable(!!(n.Firmado ?? n.firmado)),
              FirmaRuta: n.FirmaRuta ?? n.firmaRuta ?? null,
            };
          });

          var notasFiltradas = todasLasNotas
            .filter(function (n) { return n.TurnoEnfermeriaId === turnoId; })
            .sort(function (a, b) { return new Date(a.FechaRegistro) - new Date(b.FechaRegistro); }); // asc: a - b
          self.notasEnfermerias(notasFiltradas);
        } else {
          self.notasEnfermerias([]);
          mensajeEmergenteError(data.mensaje || "Error al cargar las notas.");
        }

        $("#mdl-agregar-nota-enfermeria").dialog({
          width: 1000,
          open: function () {
            if (self.quillEditor) {
              var contenidoActual = self.notaDiagnostico();
              if (contenidoActual && contenidoActual.trim() !== "") {
                self.quillEditor.root.innerHTML = contenidoActual;
              } else {
                self.quillEditor.setText('');
              }
            }
          },
          close: function () {
            if (self.quillEditor) self.quillEditor.setText('');
            self.notaDiagnostico('');
          }
        });
      },
      error: function (err) {
        hideLoading();
        self.notasEnfermerias([]);
        console.error(err);
        mensajeEmergenteError("Error al cargar las notas del turno.");
        $("#mdl-agregar-nota-enfermeria").dialog({
          width: 1000,
          open: function () {
            if (quillEditor) quillEditor.setText('');
            self.notaDiagnostico('');
          }
        });
      }
    });
  };


  self.verModalAgregarNotaEvolucion = function () {
    $("#mdl-agregar-nota-evolucion").dialog({
      width: 1000,
    });
  };

  self.verModalAgregarControlGlucometria = function () {
    $("#mdl-agregar-control-glucometria").dialog({
      width: 1000,
    });
  };
  self.imprimirNotaMedica2 = function (notaMedica) {
    if (notaMedica && notaMedica.Id) {
      window.open(
        "/CrearPDF/HospitalizacionNotaMedica2PDF?notaMedicaId=" + notaMedica.Id,
        "_blank",
      );
    } else {
      alert("El ID de la nota médica no es válido:", notaMedica);
    }
  };
  self.imprimirNotaOperatoria = function (nota) {
    if (nota && nota.Id) {
      window.open(
        "/CrearPDF/HospitalizacionNotaOperatoria2PDF?notaOperatoriaId=" + nota.Id,
        "_blank",
      );
    } else {
      alert("El ID de la nota operatoria no es válido.");
    }
  };

  self.verDetalleNotaOperatoria = function (nota) {
    self.notaOperatoriaDetalle(nota);
    var modal = new bootstrap.Modal(document.getElementById("mdl-detalle-nota-operatoria"));
    modal.show();
  };


  self.abrirModalFirmaNotaOperatoria = function (nota) {
    self.firmaNotaOpId(nota.Id);
    self.firmaNotaOpBase64(null);
    self.firmaNotaOpExistenteUrl(null);
    self.firmaNotaOpModo("canvas");

    // Resetear UI al estado inicial: selector visible, previsualización oculta, botón deshabilitado
    var selector = document.getElementById("selectorFirmaNotaOp");
    var previsualizacion = document.getElementById("contenedorPrevFirmaNotaOp");
    var btnFirmar = document.getElementById("btnConfirmarFirmaNotaOp");
    var imgPrev = document.getElementById("imgPrevFirmaNotaOp");
    if (selector) selector.classList.remove("d-none");
    if (previsualizacion) previsualizacion.classList.add("d-none");
    if (btnFirmar) btnFirmar.setAttribute("disabled", "disabled");
    if (imgPrev) imgPrev.src = "";

    // Asegurar que el tab canvas quede activo
    var tabCanvas = document.getElementById("tabCanvasFirmaNotaOp");
    var tabArchivo = document.getElementById("tabArchivoFirmaNotaOp");
    var panelCanvas = document.getElementById("panelCanvasFirmaNotaOp");
    var panelArchivo = document.getElementById("panelArchivoFirmaNotaOp");
    if (tabCanvas) tabCanvas.classList.add("active");
    if (tabArchivo) tabArchivo.classList.remove("active");
    if (panelCanvas) panelCanvas.classList.remove("d-none");
    if (panelArchivo) panelArchivo.classList.add("d-none");

    self._firmaNotaOpModalEl = $("#mdl-firma-nota-operatoria");

    showLoading();
    $.ajax({
      url: "/NotaOperatoria/ObtenerFirmaEmpleado",
      method: "GET",
      success: function (res) {
        hideLoading();

        if (res.exitoso && res.firmaUrl) {
          self.firmaNotaOpExistenteUrl(res.firmaUrl);
          var testImg = new Image();
          testImg.onload = function () {
            self._mostrarPrevisualizacionFirmaNotaOp(res.firmaUrl);
          };
          testImg.onerror = function () {
            self.firmaNotaOpExistenteUrl(null);
          };
          testImg.src = res.firmaUrl;
        }
        self._firmaNotaOpModalEl.modal("show");
      },
      error: function () {
        hideLoading();
        self._firmaNotaOpModalEl.modal("show");
      }
    });
  };

  /** Muestra la previsualización en el modal de firma */
  self._mostrarPrevisualizacionFirmaNotaOp = function (src) {
    var imgPrev = document.getElementById("imgPrevFirmaNotaOp");
    var selector = document.getElementById("selectorFirmaNotaOp");
    var contenedor = document.getElementById("contenedorPrevFirmaNotaOp");
    var btnFirmar = document.getElementById("btnConfirmarFirmaNotaOp");

    if (imgPrev) imgPrev.src = src;
    if (selector) selector.classList.add("d-none");
    if (contenedor) contenedor.classList.remove("d-none");
    if (btnFirmar) btnFirmar.removeAttribute("disabled");
  };

  self.resetearFirmaNotaOp = function () {
    self.firmaNotaOpBase64(null);
    self.firmaNotaOpExistenteUrl(null);

    var imgPrev = document.getElementById("imgPrevFirmaNotaOp");
    var selector = document.getElementById("selectorFirmaNotaOp");
    var contenedor = document.getElementById("contenedorPrevFirmaNotaOp");
    var btnFirmar = document.getElementById("btnConfirmarFirmaNotaOp");

    if (imgPrev) { imgPrev.src = ""; imgPrev.style.display = "none"; }
    if (selector) selector.classList.remove("d-none");
    if (contenedor) contenedor.classList.add("d-none");
    if (btnFirmar) btnFirmar.setAttribute("disabled", "disabled");

    // Volver siempre al tab canvas y re-inicializarlo limpio
    if (typeof window._switchFirmaTabNotaOp === "function") {
      window._switchFirmaTabNotaOp("canvas");
    } else {
      if (typeof window.limpiarCanvasFirmaNotaOp === "function") window.limpiarCanvasFirmaNotaOp();
      if (typeof window.initCanvasFirmaNotaOp === "function") window.initCanvasFirmaNotaOp();
    }
  };

  // self.confirmarFirmaNotaOperatoria = function () {
  //   var id = self.firmaNotaOpId();
  //   var base64 = self.firmaNotaOpBase64();
  //   var existenteUrl = self.firmaNotaOpExistenteUrl();

  //   if (!base64 && !existenteUrl) {
  //     mensajeEmergenteError("Debe dibujar, subir o usar su firma antes de continuar.");
  //     return;
  //   }

  //   if (!confirm("¿Confirma la firma de esta nota operatoria? Esta acción no se puede deshacer.")) return;

  //   showLoading();
  //   $.ajax({
  //     url: "/NotaOperatoria/FirmarNotaOperatoria",
  //     method: "POST",
  //     contentType: "application/json",
  //     data: JSON.stringify({
  //       NotaOperatoriaId: id,
  //       FirmaBase64: base64 || null,
  //       FirmaExistenteUrl: existenteUrl || null,
  //     }),
  //     success: function (dataResult) {
  //       hideLoading();
  //       var data = JSON.parse(dataResult);
  //       if (data.exitoso) {
  //         var notas = self.notasOperatorias();
  //         for (var i = 0; i < notas.length; i++) {
  //           if (notas[i].Id === id) {
  //             var notaActualizada = ko.utils.extend({}, notas[i]);
  //             notaActualizada.FirmaRuta = data.firmaRuta;
  //             notaActualizada.Firmado = true;
  //             self.notasOperatorias.splice(i, 1, notaActualizada);
  //             break;
  //           }
  //         }

  //         var detalle = self.notaOperatoriaDetalle();
  //         if (detalle && detalle.Id === id) {
  //           var detalleActualizado = ko.utils.extend({}, detalle);
  //           detalleActualizado.FirmaRuta = data.firmaRuta;
  //           detalleActualizado.Firmado = true;
  //           self.notaOperatoriaDetalle(detalleActualizado);
  //         }

  //         if (self._firmaNotaOpModalEl) {
  //           self._firmaNotaOpModalEl.modal("hide");
  //           self._firmaNotaOpModalEl = null;
  //         }

  //         mensajeEmergente("Nota operatoria firmada correctamente.");
  //       } else {
  //         mensajeEmergenteError(data.resultado || "Error al firmar la nota.");
  //       }
  //     },
  //     error: function () {
  //       hideLoading();
  //       mensajeEmergenteError("Error de conexión al firmar la nota.");
  //     }
  //   });
  // };


  self.autorizarDocumento = async function (documento) {
    if (!documento.Id) {
      mensajeEmergenteError("No se puede autorizar: falta el ID.");
      return;
    }

    showLoading();
    var beginRes = await fetch("/api/WebAuthnVerify/Begin?actionLabel=Autorizar+Documento", { method: "POST" });
    if (!beginRes.ok) {
      hideLoading();
      var errData = await beginRes.json().catch(() => ({}));
      mensajeEmergenteError(errData.message || "No se pudo iniciar verificación.");
      return;
    }
    var options = await beginRes.json();
    hideLoading();

    var assertion;
    try {
      assertion = await navigator.credentials.get({
        publicKey: {
          challenge: base64UrlToBuffer(options.challenge),
          timeout: options.timeout ?? 60000,
          rpId: options.rpId,
          userVerification: options.userVerification ?? "required",
          allowCredentials: [],
        },
      });
    } catch (e) {
      mensajeEmergenteError(e.name === "NotAllowedError" ? "Verificación cancelada." : "Error al leer huella.");
      return;
    }

    // Construir el payload de huella como objeto
    var huellaPayloadObj = {
      id: assertion.id,
      rawId: bufferToBase64Url(assertion.rawId),
      type: assertion.type,
      response: {
        authenticatorData: bufferToBase64Url(assertion.response.authenticatorData),
        clientDataJSON: bufferToBase64Url(assertion.response.clientDataJSON),
        signature: bufferToBase64Url(assertion.response.signature),
        userHandle: assertion.response.userHandle ? bufferToBase64Url(assertion.response.userHandle) : null,
      },
    };

    // Convertir a string JSON
    var huellaPayloadString = JSON.stringify(huellaPayloadObj);

    var requestData = {
      DocumentoId: documento.Id,
      HuellaPayload: huellaPayloadString
    };

    showLoading();
    $.ajax({
      url: "/Hospitalizacion/AutorizarDocumento",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(requestData),
      success: function (response) {
        hideLoading();
        if (response.exitoso) {
          documento.Autorizado = true;
          self.cargarListaDocumentos(); // recargar tabla
          mensajeEmergente("Documento autorizado.");
        } else {
          mensajeEmergenteError(response.mensaje || "Error al autorizar.");
        }
      },
      error: function (xhr) {
        hideLoading();
        console.error(xhr);
        mensajeEmergenteError("Error de conexión al autorizar.");
      }
    });
  };


  self.confirmarFirmaNotaOperatoria = async function () {
    var id = self.firmaNotaOpId();
    var base64 = self.firmaNotaOpBase64();
    var existenteUrl = self.firmaNotaOpExistenteUrl();

    if (!base64 && !existenteUrl) {
      mensajeEmergenteError("Debe dibujar, subir o usar su firma antes de continuar.");
      return;
    }

    if (!confirm("¿Confirma la firma de esta nota operatoria? Esta acción no se puede deshacer.")) return;

    // ── Paso 1: Pedir challenge ─────────────────────────────────────
    showLoading();
    var beginRes = await fetch("/api/WebAuthnVerify/Begin?actionLabel=Firmar+Nota+Operatoria", {
      method: "POST"
    });

    if (!beginRes.ok) {
      hideLoading();
      var errData = await beginRes.json().catch(() => ({}));
      mensajeEmergenteError(errData.message || "No se pudo iniciar la verificación de huella.");
      return;
    }

    var options = await beginRes.json();
    hideLoading();

    // ── Paso 2: Solicitar huella al dispositivo ─────────────────────
    var assertion;
    try {
      assertion = await navigator.credentials.get({
        publicKey: {
          challenge: base64UrlToBuffer(options.challenge),
          timeout: options.timeout ?? 60000,
          rpId: options.rpId,
          userVerification: options.userVerification ?? "required",
          allowCredentials: [],  // Vacío = cualquier credencial registrada
        },
      });
    } catch (e) {
      if (e.name === "NotAllowedError") {
        mensajeEmergenteError("Verificación cancelada.");
      } else {
        mensajeEmergenteError("El dispositivo no pudo leer la huella: " + e.message);
      }
      return;
    }

    // ── Paso 3: Serializar la respuesta ─────────────────────────────
    var huellaPayload = {
      id: assertion.id,
      rawId: bufferToBase64Url(assertion.rawId),
      type: assertion.type,
      response: {
        authenticatorData: bufferToBase64Url(assertion.response.authenticatorData),
        clientDataJSON: bufferToBase64Url(assertion.response.clientDataJSON),
        signature: bufferToBase64Url(assertion.response.signature),
        userHandle: assertion.response.userHandle
          ? bufferToBase64Url(assertion.response.userHandle)
          : null,
      },
    };

    // ── Paso 4: Enviar al backend ───────────────────────────────────
    showLoading();
    $.ajax({
      url: "/NotaOperatoria/FirmarNotaOperatoria",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        model: {
          NotaOperatoriaId: id,
          FirmaBase64: base64 || null,
          FirmaExistenteUrl: existenteUrl || null,
        },
        huellaPayload: huellaPayload,
      }),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          var notas = self.notasOperatorias();
          for (var i = 0; i < notas.length; i++) {
            if (notas[i].Id === id) {
              var notaActualizada = ko.utils.extend({}, notas[i]);
              notaActualizada.FirmaRuta = data.firmaRuta;
              notaActualizada.Firmado = true;
              self.notasOperatorias.splice(i, 1, notaActualizada);
              break;
            }
          }

          var detalle = self.notaOperatoriaDetalle();
          if (detalle && detalle.Id === id) {
            var detalleActualizado = ko.utils.extend({}, detalle);
            detalleActualizado.FirmaRuta = data.firmaRuta;
            detalleActualizado.Firmado = true;
            self.notaOperatoriaDetalle(detalleActualizado);
          }

          if (self._firmaNotaOpModalEl) {
            self._firmaNotaOpModalEl.modal("hide");
            self._firmaNotaOpModalEl = null;
          }

          mensajeEmergente("Nota operatoria firmada correctamente.");
        } else {
          mensajeEmergenteError(data.resultado || "Error al firmar la nota.");
        }
      },
      error: function (dataError) {
        hideLoading();
        console.error("Error ajax:", dataError.responseText);
        mensajeEmergenteError("Error de conexión al firmar la nota.");
      },
    });
  };

  self.agregarExamenFisico = function () {
    var hospId = parseInt($("#HospitalizacionId").val());
    if (!hospId || hospId === 0) {
      mensajeEmergenteError("No se encontró el ID de hospitalización.");
      return;
    }

    var entityExamenFisico = {
      HospitalizacionId: hospId,
      DatosExamen: self.datosExamenFisicoAgregar(),
      Observaciones: self.observacionesExamenFisicoAgregar(),
    };

    showLoading();
    $.ajax({
      url: "/Hospitalizacion/AgregarExamenFisicoHospitalizacion",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        entity: entityExamenFisico,
        hospitalizacionId: hospId,
      }),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          $("#mdl-agregar-examen-fisico").dialog("close");
          self.consultarExamenesFisicosHospitalizacion();
          mensajeEmergente("Signos vitales guardados.");
        } else {
          mensajeEmergenteError(data.resultado);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.error("Error ajax:", dataError.responseText);
        mensajeEmergenteError("Error al guardar el examen físico");
      },
    });
  };

  self.autorizarExamenFisico = async function (examen) {
    // Primero verificar si el usuario actual tiene huella
    await self.checkCurrentUserFingerprint();

    let targetUserId = null;
    if (!self.currentUserHasFingerprint()) {
      // Mostrar modal de selección de usuario
      const result = await self.showSelectUserModal();
      if (!result) return; // cancelado
      targetUserId = result.userId;
    }

    // Continuar con la verificación
    await self.realizarVerificacionHuella(examen, targetUserId);
  };

  self.realizarVerificacionHuella = async function (examen, targetUserId = null) {
    showLoading();
    let url = "/api/WebAuthnVerify/Begin?actionLabel=Autorizar+Signos+Vitales";
    if (targetUserId) {
      url += `&targetUserId=${encodeURIComponent(targetUserId)}`;
    }
    const beginRes = await fetch(url, { method: "POST" });
    if (!beginRes.ok) {
      hideLoading();
      const err = await beginRes.json();
      mensajeEmergenteError(err.message || "No se pudo iniciar verificación.");
      return;
    }
    const options = await beginRes.json();
    hideLoading();

    let assertion;
    try {
      // 🔥 CAMBIO IMPORTANTE: allowCredentials = [] (vacío)
      assertion = await navigator.credentials.get({
        publicKey: {
          challenge: base64UrlToBuffer(options.challenge),
          timeout: options.timeout ?? 60000,
          rpId: options.rpId,
          userVerification: options.userVerification ?? "required",
          allowCredentials: []  // ← Permitir cualquier credencial del dispositivo
        },
      });
    } catch (e) {
      console.error("Error detallado:", e);
      let msg = e.name === "NotAllowedError"
        ? "Verificación cancelada o no se pudo leer la huella. Asegúrate de tener una huella registrada en el sistema operativo."
        : e.name === "NotSupportedError"
          ? "Tu navegador no soporta WebAuthn."
          : e.message || "Error desconocido al leer huella.";
      mensajeEmergenteError(msg);
      return;
    }

    const huellaPayload = {
      id: assertion.id,
      rawId: bufferToBase64Url(assertion.rawId),
      type: assertion.type,
      response: {
        authenticatorData: bufferToBase64Url(assertion.response.authenticatorData),
        clientDataJSON: bufferToBase64Url(assertion.response.clientDataJSON),
        signature: bufferToBase64Url(assertion.response.signature),
        userHandle: assertion.response.userHandle ? bufferToBase64Url(assertion.response.userHandle) : null,
      },
    };

    showLoading();
    $.ajax({
      url: "/Hospitalizacion/AutorizarExamenFisicoHospitalizacion",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({ examenId: examen.Id, huellaPayload: huellaPayload }),
      success: function (dataResult) {
        hideLoading();
        const data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarExamenesFisicosHospitalizacion();
          mensajeEmergente("Signos vitales autorizados correctamente.");
        } else {
          mensajeEmergenteError(data.resultado);
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error al autorizar.");
      }
    });
  };


  self.showSelectUserModal = function () {
    return new Promise((resolve) => {
      const modalHtml = `
            <div class="modal fade" id="selectUserModal" tabindex="-1">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Seleccionar usuario para autorizar</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <p>Usted no tiene huella registrada. Seleccione un médico que pueda autorizar:</p>
                            <select id="authUserSelect" class="form-select">
                                <option value="">-- Seleccionar --</option>
                                ${self.availableUsersForAuth().map(u => `<option value="${u.userId}">${u.fullName}</option>`).join('')}
                            </select>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                            <button type="button" class="btn btn-primary" id="confirmSelectUser">Continuar</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
      $('body').append(modalHtml);
      const modal = new bootstrap.Modal(document.getElementById('selectUserModal'));
      modal.show();

      $('#confirmSelectUser').off('click').on('click', function () {
        const userId = $('#authUserSelect').val();
        if (!userId) {
          mensajeEmergenteError("Debe seleccionar un usuario.");
          return;
        }
        modal.hide();
        $('#selectUserModal').remove();
        resolve({ userId: userId });
      });

      $('#selectUserModal').on('hidden.bs.modal', function () {
        $('#selectUserModal').remove();
        resolve(null);
      });
    });
  };

  self.eliminarServicio = function (value) {
    if (confirm("¿Desea eliminar este servicio?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/EliminarServicio",
        method: "POST",
        data: {
          hospitalizacionServicioId: value.Id,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarServiciosHospitalizacion();
            mensajeEmergente("Servicio eliminado");
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };
  self.consultarAmbulancias = function () {
    let hospitalizacionId = $("#HospitalizacionId").val();

    if (!hospitalizacionId) return;

    $.ajax({
      url: "/Ambulancias/ObtenerPorHospitalizacion",
      method: "GET",
      data: { hospitalizacionId: hospitalizacionId },
      success: function (data) {
        self.listaAmbulancias(data);
      },
      error: function (err) {
        console.log("Error consultando ambulancias:", err);
        mensajeEmergenteError("No se pudo cargar la lista de ambulancias.");
      },
    });
  };

  self.eliminarMedicamento = function (value) {
    if (confirm("¿Desea eliminar este producto?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/EliminarMedicamento",
        method: "POST",
        data: {
          hospitalizacionMedicamentoId: value.Id,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarMedicamentosHospitalizacion();
            self.consultarProductosAplicacionHospitalizacion();
            mensajeEmergente("Medicamento eliminado");
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };
  self.eliminarPaquete = function (value) {
    if (confirm("¿Desea eliminar este paquete?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/EliminarPaquete",
        method: "POST",
        data: {
          hospitalizacionPaqueteId: value.Id,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarPaquetesHospitalizacion();
            self.consultarPaquetesAplicados();
            mensajeEmergente("Paquete eliminado");
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };
  self.eliminarExamen = function (value) {
    if (!value || !value.Id) {
      mensajeEmergenteError("No se pudo identificar el examen a eliminar.");
      return;
    }

    if (confirm("¿Desea eliminar este examen?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/EliminarExamen",
        method: "POST",
        data: {
          hospitalizacionExamenId: value.HospitalizacionExamenId || value.Id,
        },
        success: function (dataResult) {
          hideLoading();
          let data;
          try {
            data = typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
          } catch (e) {
            mensajeEmergenteError("Respuesta inválida del servidor.");
            return;
          }

          if (data.Exitoso) {
            self.examenesComplementariosCargados = false;
            self.consultarExamenesHospitalizacion(true);
            mensajeEmergente("Examen eliminado");
          } else {
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError("Error de conexión al eliminar el examen.");
        },
      });
    }
  };
  self._recargarTabOrdenMedica = function () {
    var hospitalizacionId = $("#HospitalizacionId").val();
    var $tabOrdenMedica = $("#tab-content-orden-medica");

    if ($tabOrdenMedica.length === 0) {
      console.warn("⚠ [VM] Tab Orden Médica no encontrado en el DOM.");
      return;
    }

    console.log("🔄 [VM] Recargando contenido del tab Orden Médica...");

    $.ajax({
      url: "/Hospitalizacion/OrdenMedicaPartial",
      method: "GET",
      data: { hospitalizacionId: hospitalizacionId },
      success: function (html) {
        $tabOrdenMedica.html(html);
        console.log("✅ [VM] Tab Orden Médica recargado correctamente.");
      },
      error: function (xhr) {
        console.warn("⚠ [VM] No se pudo recargar Orden Médica (¿falta el endpoint?):", xhr.status, xhr.statusText);
        if ($tabOrdenMedica.hasClass("active") && $tabOrdenMedica.is(":visible")) {
          console.log("🔄 [VM] Aplicando fallback: re-trigger del tab Orden Médica.");
          $('a[href="#tab-content-orden-medica"]').trigger("shown.bs.tab");
        }
      },
    });
  };

  self.eliminarDeposito = function (value) {
    if (confirm("¿Desea eliminar este deposito?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/EliminarDeposito",
        method: "POST",
        data: {
          pagoId: value.Id,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            self.consultarDepositosHospitalizacion();
            mensajeEmergente("Deposito eliminado");
          } else {
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          alert(dataError);
        },
      });
    }
  };

  self.actualizarEstadia = function () {
    let periodo = $("#periodo-hospitalizacion").val();
    let fechas = periodo.split(" - ");
    if (
      confirm(
        "¿Desea actualizar las fechas de estadia de esta hospitalizacion?",
      )
    ) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/ActualizarEstadia",
        method: "POST",
        data: {
          hospitalizacionId: $("#HospitalizacionId").val(),
          periodo: periodo,
        },
        success: function (dataResult) {
          var data = JSON.parse(dataResult);
          if (data.Exitoso) {
            mensajeEmergente("Estadia actualizada");
            self.consultarRegistrosHospitalizacion();
            hideLoading();
          } else {
            hideLoading();
            mensajeEmergenteError(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          mensajeEmergenteError(dataError);
        },
      });
    }
  };

  self.verModalCambioHabitacion = function () {
    $("#mdl-cambiar-habitacion").dialog({
      width: 1000,
    });
  };
  self.cambiarHabitacion = function (value) {
    if (confirm("¿Desea realizar el cambio de habitacion?")) {
      showLoading();
      $.ajax({
        url: "/Hospitalizacion/CambiarHabitacion",
        method: "POST",
        data: {
          hospitalizacionId: $("#HospitalizacionId").val(),
          habitacionId: value.HabitacionId,
        },
        success: function (dataResult) {
          var data = JSON.parse(dataResult);
          if (data.Exitoso) {
            window.location.reload();
          } else {
            hideLoading();
            alert(data.Mensaje);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.log(dataError);
          alert(dataError);
        },
      });
    }
    alert("Cambio de habitacion");
  };

  //Nota medica
  // self.agregarNotaMedica2 = function () {
  //   var nuevaNotaMedica2 = {
  //     HistoriaProblema: self.notaHistoriaProblema(),
  //     Sintomas: self.notaSintomas(),
  //     Diagnostico: self.notaDiagnostico(),
  //     ProfesionalId: $("#ProfesionalId").val(), // Asegúrate de tener el ID del profesional
  //     HospitalizacionId: $("#HospitalizacionId").val(),
  //   };

  //   showLoading();
  //   $.ajax({
  //     url: "/NotaMedica2/AgregarNotaMedica2",
  //     method: "POST",
  //     contentType: "application/json",
  //     data: JSON.stringify(nuevaNotaMedica2),
  //     success: function (dataResult) {
  //       hideLoading();
  //       let data = JSON.parse(dataResult);
  //       if (data.exitoso) {
  //         self.consultarNotasMedicas();
  //         self.limpiarModalNotaMedica2(); // Limpiar el formulario
  //         $("#mdl-agregar-nota-medica").dialog("close");
  //         mensajeEmergente("Nota médica agregada exitosamente");
  //       } else {
  //         mensajeEmergenteError("Esto es un mensaje de error: ", data.mensaje);
  //       }
  //     },
  //     error: function (dataError) {
  //       hideLoading();

  //       alert(dataError);
  //     },
  //   });
  // };

  self.agregarNotaMedica2 = function () {
    var hospId = parseInt($("#HospitalizacionId").val());
    if (!hospId || hospId === 0) {
      mensajeEmergenteError("No se encontró el ID de hospitalización.");
      return;
    }

    // Obtener contenido del editor Quill si está disponible
    var contenido = self.notaDiagnostico();
    if (self.quillMedica && self.quillMedica.root) {
      contenido = self.quillMedica.root.innerHTML;
      self.notaDiagnostico(contenido);
    }

    // MODO EDICIÓN
    if (self.editandoNotaMedica()) {
      var notaId = self.editNotaMedicaId();
      if (!notaId) {
        mensajeEmergenteError("No se encontró el ID de la nota.");
        return;
      }
      showLoading();
      $.ajax({
        url: "/NotaMedica2/ActualizarNotaMedica2",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ Id: notaId, Diagnostico: contenido }),
        success: function (dataResult) {
          hideLoading();
          var data = JSON.parse(dataResult);
          if (data.exitoso) {
            self.consultarNotasMedicas();
            self.limpiarModalNotaMedica2();
            $("#mdl-agregar-nota-medica").dialog("close");
            mensajeEmergente("Nota médica actualizada.");
          } else {
            mensajeEmergenteError(data.resultado);
          }
        },
        error: function (dataError) {
          hideLoading();
          console.error("Error ajax:", dataError.responseText);
          mensajeEmergenteError("Error al actualizar la nota médica.");
        },
      });
      return;
    }

    // MODO AGREGAR
    var nuevaNota = {
      HistoriaProblema: self.notaHistoriaProblema(),
      Sintomas: self.notaSintomas(),
      Diagnostico: contenido,
      HospitalizacionId: hospId,
      ProfesionalId: $("#ProfesionalId").val()
    };

    showLoading();
    $.ajax({
      url: "/NotaMedica2/AgregarNotaMedica2",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({ entity: nuevaNota, hospitalizacionId: hospId }),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarNotasMedicas();
          self.limpiarModalNotaMedica2();
          $("#mdl-agregar-nota-medica").dialog("close");
          mensajeEmergente("Nota médica guardada.");
        } else {
          mensajeEmergenteError(data.resultado);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.error("Error ajax:", dataError.responseText);
        mensajeEmergenteError("Error al agregar la nota médica.");
      },
    });
  };


  self.autorizarNotaMedica2 = async function (nota) {
    if (!nota.Id) {
      mensajeEmergenteError("No se puede autorizar: falta el ID.");
      return;
    }

    showLoading();
    var beginRes = await fetch("/api/WebAuthnVerify/Begin?actionLabel=Autorizar+Nota+Medica", { method: "POST" });
    if (!beginRes.ok) {
      hideLoading();
      var errData = await beginRes.json().catch(() => ({}));
      mensajeEmergenteError(errData.message || "No se pudo iniciar verificación.");
      return;
    }
    var options = await beginRes.json();
    hideLoading();

    var assertion;
    try {
      assertion = await navigator.credentials.get({
        publicKey: {
          challenge: base64UrlToBuffer(options.challenge),
          timeout: options.timeout ?? 60000,
          rpId: options.rpId,
          userVerification: options.userVerification ?? "required",
          allowCredentials: [],
        },
      });
    } catch (e) {
      mensajeEmergenteError(e.name === "NotAllowedError" ? "Verificación cancelada." : "Error al leer huella.");
      return;
    }

    var huellaPayload = {
      id: assertion.id,
      rawId: bufferToBase64Url(assertion.rawId),
      type: assertion.type,
      response: {
        authenticatorData: bufferToBase64Url(assertion.response.authenticatorData),
        clientDataJSON: bufferToBase64Url(assertion.response.clientDataJSON),
        signature: bufferToBase64Url(assertion.response.signature),
        userHandle: assertion.response.userHandle ? bufferToBase64Url(assertion.response.userHandle) : null,
      },
    };

    showLoading();
    $.ajax({
      url: "/NotaMedica2/AutorizarNotaMedica2",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        notaMedicaId: nota.Id,
        huellaPayload: huellaPayload,
      }),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          nota.Autorizado = true;
          self.consultarNotasMedicas(); // recargar lista
          mensajeEmergente("Nota médica autorizada.");
        } else {
          mensajeEmergenteError(data.resultado);
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error al autorizar.");
      },
    });
  };

  self.limpiarModalNotaMedica2 = function () {
    self.notaHistoriaProblema("");
    self.notaSintomas("");
    self.notaDiagnostico("");
  };
  self.consultarNotasMedicas = function () {
    showLoading();
    $.ajax({
      url: "/NotaMedica2/ObtenerNotasMedicas",
      method: "POST",
      data: {
        idHospitalizacion: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        // console.log(data);

        if (data.exitoso) {
          self.notasMedicas(data.resultado);
        } else {
          alert(
            data.mensaje ||
            "Ocurrió un error inesperado al obtener las notas médicas.",
          );
        }
      },
      error: function (dataError) {
        hideLoading();
        alert("Error de red o del servidor. Intenta nuevamente.");
      },
    });
  };

  // ── Nota Operatoria ───────────────────────────────────────────────
  self.abrirEditarNotaOperatoria = function (nota) {
    // Cerrar modal de detalle
    $("#mdl-detalle-nota-operatoria").modal("hide");

    // Guardar Id directamente (no via hidden input)
    self.editNotaId(nota.Id);
    self.editNotaDiagnostico(nota.Diagnostico || "");
    // Precargar campos de texto/fecha
    self.editNotaFechaOperacion(nota.FechaOperacion
      ? nota.FechaOperacion.replace(/^(\d{2})\/(\d{2})\/(\d{4}).*/, "$3-$2-$1")
      : "");
    self.editNotaHoraComenzo(nota.HoraComenzo || "");
    self.editNotaHoraTermino(nota.HoraTermino || "");
    self.editNotaDiagnosticoPreOp(nota.DiagnosticoPreOperatorio || "");
    self.editNotaDiagnosticoPostOp(nota.DiagnosticoPostOperatorio || "");
    self.editNotaOperacionEfectuada(nota.OperacionEfectuada || "");
    self.editNotaHallazgosTransOp(nota.HallazgosTransOperatorios || "");
    self.editNotaDiagnostico(nota.Diagnostico || "");

    // Cargar listas y preseleccionar directo en observables de edicion
    self.cargarMedicosModal({
      cirujanoNombre: nota.Cirujano || "",
      primerAyudante: nota.PrimerAyudante || "",
      segundoAyudante: nota.SegundoAyudante || "",
      anestesista: nota.Anestesista || "",
      instrumentista: nota.Instrumentista || "",
      circulante: nota.Circulante || ""
    }, "editar");

    $("#mdl-editar-nota-operatoria").dialog({
      width: 1000,
      title: "Editar Nota Operatoria #" + nota.Id,
      open: function () {
        $("#diagnostico_NO_ID").val(nota.Diagnostico || "");
        $("#diagnostico_NO_ID").trigger('change');
        setTimeout(function () {
          $("#mdl-editar-nota-operatoria .select2-medicos-modal").select2({
            width: "100%",
            dropdownParent: $("#mdl-editar-nota-operatoria"),
            language: { noResults: function () { return "No se encontraron resultados"; } }
          });
        }, 300);
      }
    });
  };

  self.guardarEdicionNotaOperatoria = function () {
    var id = self.editNotaId();
    if (!id || id <= 0) {
      mensajeEmergenteError("Error: ID de nota no valido.");
      return;
    }

    // Forzar sincronización de TODOS los textareas del modal de edición
    $("#diagnosticoPreOperatorio_NO_Edit, #diagnosticoPostOperatorio_NO_Edit, #operacionEfectuada_NO_Edit, #hallazgos_NO_Edit, #descripcionGeneral_NO_Edit").each(function () {
      var $ta = $(this);
      var bind = $ta.attr("data-bind");
      if (bind) {
        var match = bind.match(/textInput:\s*(\w+)/);
        if (match && match[1] && self[match[1]] && ko.isObservable(self[match[1]])) {
          self[match[1]]($ta.val());
        }
      }
    });

    var payload = {
      Id: id,
      HospitalizacionId: parseInt($("#HospitalizacionId").val()),
      FechaOperacion: self.editNotaFechaOperacion() || null,
      HoraComenzo: self.editNotaHoraComenzo(),
      HoraTermino: self.editNotaHoraTermino(),
      Cirujano: self.editNotaCirujano(),
      PrimerAyudante: self.editNotaPrimerAyudante(),
      SegundoAyudante: self.editNotaSegundoAyudante(),
      Anestesista: self.editNotaAnestesista(),
      Instrumentista: self.editNotaInstrumentista(),
      Circulante: self.editNotaCirculante(),
      DiagnosticoPreOperatorio: self.editNotaDiagnosticoPreOp(),
      DiagnosticoPostOperatorio: self.editNotaDiagnosticoPostOp(),
      OperacionEfectuada: self.editNotaOperacionEfectuada(),
      HallazgosTransOperatorios: self.editNotaHallazgosTransOp(),
      Diagnostico: self.editNotaDiagnostico()
    };

    showLoading();
    $.ajax({
      url: "/NotaOperatoria/ActualizarNotaOperatoria",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(payload),
      success: function (dataResult) {
        hideLoading();
        var data = typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        if (data.exitoso) {
          $("#mdl-editar-nota-operatoria").dialog("close");
          // Actualizar vista inmediatamente y recargar lista
          self.consultarNotasOperatorias();
          mensajeEmergente("Nota operatoria actualizada correctamente");
        } else {
          mensajeEmergenteError(data.resultado || "Error al actualizar");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de servidor al actualizar la nota operatoria");
      }
    });
  };
  // ── EDITAR LISTA DE CHEQUEO ─────────────────────────────────────────────

  self.abrirEditarListaChequeo = function (item) {
    self.editChkId(item.Id);
    self.editChk_FechaChequeo(item.FechaChequeo || "");
    self.editChk_HoraChequeo(item.HoraChequeo || "");
    self.editChk_CI_NombreConfirma(item.CI_NombreConfirma || "");
    self.editChk_CI_ApellidoConfirma(item.CI_ApellidoConfirma || "");
    // ===== NUEVO: Forzar datos actuales del paciente =====
    // 1. Fecha de nacimiento (convertir de DD/MM/YYYY a YYYY-MM-DD)
    var fechaNacRaw = ($("#PacienteFechaNacimiento").val() || "").trim();
    var fechaNac = "";
    if (fechaNacRaw) {
      var partes = fechaNacRaw.split("/");
      if (partes.length === 3) {
        fechaNac = partes[2] + "-" + partes[1] + "-" + partes[0];
      } else {
        fechaNac = fechaNacRaw;
      }
    }
    self.editChk_CI_FechaNacConfirma(fechaNac);
    self.editChk_CI_Consentimiento(item.CI_Consentimiento || "");
    var procedimiento = ($("#CitaProcedimiento").val() || "").trim();
    self.editChk_CI_Operacion(procedimiento);
    self.editChk_CI_LadoOperar(item.CI_LadoOperar || "");
    self.editChk_CI_SitioMarcado(item.CI_SitioMarcado || "");
    self.editChk_CI_Alergia(item.CI_Alergia || "");
    self.editChk_CI_EvalPreanestesica(item.CI_EvalPreanestesica || "");
    self.editChk_CI_AccesoIV(item.CI_AccesoIV || "");
    self.editChk_CI_EquipoAnestesia(item.CI_EquipoAnestesia || "");
    self.editChk_CI_Medicamentos(item.CI_Medicamentos || "");
    self.editChk_CI_Oximetro(item.CI_Oximetro || "");
    self.editChk_CI_EquipoAspiracion(item.CI_EquipoAspiracion || "");
    self.editChk_CI_ViaAerea(item.CI_ViaAerea || "");
    self.editChk_CP_Presentacion(item.CP_Presentacion || "");
    self.editChk_CP_NombrePacienteCirujano(item.CP_NombrePacienteCirujano || "");
    self.editChk_CP_ApellidoPacienteCirujano(item.CP_ApellidoPacienteCirujano || "");
    self.editChk_CP_FechaNacCirujano(item.CP_FechaNacCirujano || "");
    self.editChk_CP_NombreCirugia(item.CP_NombreCirugia || "");
    self.editChk_CP_EventosCriticos(item.CP_EventosCriticos || "");
    self.editChk_CP_TiempoDuracion(item.CP_TiempoDuracion || "");
    self.editChk_CP_ImagenesDiagnosticas(item.CP_ImagenesDiagnosticas || "");
    self.editChk_CP_PerdidaSangre(item.CP_PerdidaSangre || "");
    self.editChk_CP_Esterilidad(item.CP_Esterilidad || "");
    self.editChk_CP_MaterialesAdicionales(item.CP_MaterialesAdicionales || "");
    self.editChk_CP_EventosCriticosAnest(item.CP_EventosCriticosAnest || "");
    self.editChk_CP_ProfilaxisAntibiotica(item.CP_ProfilaxisAntibiotica || "");
    self.editChk_CP_Tromboprofilaxis(item.CP_Tromboprofilaxis || "");
    self.editChk_CP_ManejoDolor(item.CP_ManejoDolor || "");
    self.editChk_CS_NombreOperacion(item.CS_NombreOperacion || "");
    self.editChk_CS_NombreEnfermera(item.CS_NombreEnfermera || "");
    self.editChk_CS_RecuentoCompleto(item.CS_RecuentoCompleto || "");
    self.editChk_CS_EtiquetadoMuestras(item.CS_EtiquetadoMuestras || "");
    self.editChk_CS_RepasoPostOp(item.CS_RepasoPostOp || "");
    self.editChk_CS_PorQue(item.CS_PorQue || "");
    self.editChk_CS_Traslado(item.CS_Traslado || "");
    self.editChk_CS_Complicaciones(item.CS_Complicaciones || "");
    self.editChk_CS_ServicioNumCama(item.CS_ServicioNumCama || "");

    setTimeout(function () {
      function setRadio(name, value) {
        if (!value) return;
        var el = document.querySelector('#mdl-editar-chequeo input[name="' + name + '"][value="' + value + '"]');
        if (el) el.checked = true;
      }
      setRadio("edit_ci_consentimiento", item.CI_Consentimiento);
      setRadio("edit_ci_lado", item.CI_LadoOperar);
      setRadio("edit_ci_marcado", item.CI_SitioMarcado);
      setRadio("edit_ci_alergia", item.CI_Alergia);
      setRadio("edit_ci_evalanest", item.CI_EvalPreanestesica);
      setRadio("edit_ci_iv", item.CI_AccesoIV);
      setRadio("edit_ci_equanest", item.CI_EquipoAnestesia);
      setRadio("edit_ci_meds", item.CI_Medicamentos);
      setRadio("edit_ci_oximetro", item.CI_Oximetro);
      setRadio("edit_ci_aspir", item.CI_EquipoAspiracion);
      setRadio("edit_ci_via", item.CI_ViaAerea);
      setRadio("edit_cp_presentacion", item.CP_Presentacion);
      setRadio("edit_cp_anticipa", item.CP_EventosCriticos);
      setRadio("edit_cp_duracion", item.CP_TiempoDuracion);
      setRadio("edit_cp_imagenes", item.CP_ImagenesDiagnosticas);
      setRadio("edit_cp_sangre", item.CP_PerdidaSangre);
      setRadio("edit_cp_esteril", item.CP_Esterilidad);
      setRadio("edit_cp_materiales", item.CP_MaterialesAdicionales);
      setRadio("edit_cp_anticipa_anest", item.CP_EventosCriticosAnest);
      setRadio("edit_cp_antibiotico", item.CP_ProfilaxisAntibiotica);
      setRadio("edit_cp_trombo", item.CP_Tromboprofilaxis);
      setRadio("edit_cp_dolor", item.CP_ManejoDolor);
      setRadio("edit_cs_recuento", item.CS_RecuentoCompleto);
      setRadio("edit_cs_muestras", item.CS_EtiquetadoMuestras);
      setRadio("edit_cs_repaso", item.CS_RepasoPostOp);
      setRadio("edit_cs_traslado", item.CS_Traslado);
    }, 200);

    $("#mdl-editar-chequeo").dialog({ width: 1100, title: "Editar Lista de Chequeo #" + item.Id });
  };

  self.guardarEdicionListaChequeo = function () {
    var id = self.editChkId();
    if (!id || id <= 0) { mensajeEmergenteError("ID de lista de chequeo no válido."); return; }
    function radio(name) {
      var el = document.querySelector('#mdl-editar-chequeo input[name="' + name + '"]:checked');
      return el ? el.value : "";
    }
    var payload = {
      Id: id, HospitalizacionId: parseInt($("#HospitalizacionId").val()),
      FechaChequeo: self.editChk_FechaChequeo() || null,
      HoraChequeo: self.editChk_HoraChequeo(),
      CI_NombreConfirma: self.editChk_CI_NombreConfirma(),
      CI_ApellidoConfirma: self.editChk_CI_ApellidoConfirma(),
      CI_FechaNacConfirma: self.editChk_CI_FechaNacConfirma() || null,
      CI_Consentimiento: radio("edit_ci_consentimiento"),
      CI_Operacion: self.editChk_CI_Operacion(),
      CI_LadoOperar: radio("edit_ci_lado"),
      CI_SitioMarcado: radio("edit_ci_marcado"),
      CI_Alergia: radio("edit_ci_alergia"),
      CI_EvalPreanestesica: radio("edit_ci_evalanest"),
      CI_AccesoIV: radio("edit_ci_iv"),
      CI_EquipoAnestesia: radio("edit_ci_equanest"),
      CI_Medicamentos: radio("edit_ci_meds"),
      CI_Oximetro: radio("edit_ci_oximetro"),
      CI_EquipoAspiracion: radio("edit_ci_aspir"),
      CI_ViaAerea: radio("edit_ci_via"),
      CP_Presentacion: radio("edit_cp_presentacion"),
      CP_NombrePacienteCirujano: self.editChk_CP_NombrePacienteCirujano(),
      CP_ApellidoPacienteCirujano: self.editChk_CP_ApellidoPacienteCirujano(),
      CP_FechaNacCirujano: self.editChk_CP_FechaNacCirujano() || null,
      CP_NombreCirugia: self.editChk_CP_NombreCirugia(),
      CP_EventosCriticos: radio("edit_cp_anticipa"),
      CP_TiempoDuracion: radio("edit_cp_duracion"),
      CP_ImagenesDiagnosticas: radio("edit_cp_imagenes"),
      CP_PerdidaSangre: radio("edit_cp_sangre"),
      CP_Esterilidad: radio("edit_cp_esteril"),
      CP_MaterialesAdicionales: radio("edit_cp_materiales"),
      CP_EventosCriticosAnest: radio("edit_cp_anticipa_anest"),
      CP_ProfilaxisAntibiotica: radio("edit_cp_antibiotico"),
      CP_Tromboprofilaxis: radio("edit_cp_trombo"),
      CP_ManejoDolor: radio("edit_cp_dolor"),
      CS_NombreOperacion: self.editChk_CS_NombreOperacion(),
      CS_NombreEnfermera: self.editChk_CS_NombreEnfermera(),
      CS_RecuentoCompleto: radio("edit_cs_recuento"),
      CS_EtiquetadoMuestras: radio("edit_cs_muestras"),
      CS_RepasoPostOp: radio("edit_cs_repaso"),
      CS_PorQue: self.editChk_CS_PorQue(),
      CS_Traslado: radio("edit_cs_traslado"),
      CS_Complicaciones: self.editChk_CS_Complicaciones(),
      CS_ServicioNumCama: self.editChk_CS_ServicioNumCama(),
    };
    showLoading();
    $.ajax({
      url: "/ListaChequeo/ActualizarListaChequeo",
      method: "POST", contentType: "application/json", data: JSON.stringify(payload),
      success: function (r) {
        hideLoading();
        var d = typeof r === "string" ? JSON.parse(r) : r;
        if (d.exitoso) { $("#mdl-editar-chequeo").dialog("close"); self.consultarListasChequeo(); mensajeEmergente("Lista de chequeo actualizada correctamente"); }
        else { mensajeEmergenteError(d.resultado || d.mensaje || "Error al actualizar"); }
      },
      error: function () { hideLoading(); mensajeEmergenteError("Error de servidor al actualizar la lista de chequeo"); }
    });
  };

  // ── EDITAR CUESTIONARIO PRE-ANESTÉSICO ────────────────────────────────────
  self.abrirEditarCuestionarioPreAnest = function (item) {
    self.editPaId(item.Id);

    var fechaFinal = "";
    if (item.FechaCuestionario) {
      fechaFinal = item.FechaCuestionario.split("T")[0];
    } else {
      fechaFinal = new Date().toISOString().split("T")[0];
    }
    self.editPa_FechaCuestionario(fechaFinal);

    var edadFinal = item.Edad || $("#PacienteEdad").val() || "";
    self.editPa_Edad(edadFinal);

    // Cargar valores existentes del registro (con respaldo de los hidden inputs)
    self.editPa_NombreCompleto(item.NombreCompleto || "");
    self.editPa_RegistroMedico(item.RegistroMedico || "");
    // self.editPa_Edad(item.Edad || "");
    // self.editPa_FechaCuestionario(item.FechaCuestionario || "");
    self.editPa_Peso(item.Peso || "");
    self.editPa_Estatura(item.Estatura || "");
    self.editPa_FechaUltimaRegla(item.FechaUltimaRegla || "");
    self.editPa_Cirujano(item.Cirujano || "");
    self.editPa_AlergiaCual(item.PA_AlergiaCual || "");
    self.editPa_FumaCuanto(item.PA_FumaCuanto || "");
    self.editPa_DrogasCuales(item.PA_DrogasCuales || "");
    self.editPa_AlcoholCuanto(item.PA_AlcoholCuanto || "");
    self.editPa_TransfusionCual(item.PA_TransfusionCual || "");
    self.editPa_AsmaCual(item.PA_AsmaCual || "");
    self.editPa_PulmonesCual(item.PA_PulmonesCual || "");
    self.editPa_CorazonCual(item.PA_CorazonCual || "");
    self.editPa_AtaqueCardiacoCual(item.PA_AtaqueCardiacoCual || "");
    self.editPa_AnginaCual(item.PA_AnginaCual || "");
    self.editPa_SoploCual(item.PA_SoploCual || "");
    self.editPa_PresionCual(item.PA_PresionCual || "");
    self.editPa_HigadoCual(item.PA_HigadoCual || "");
    self.editPa_RinonesCual(item.PA_RinonesCual || "");
    self.editPa_DiabetesCual(item.PA_DiabetesCual || "");
    self.editPa_EpilepsiaCual(item.PA_EpilepsiaCual || "");
    self.editPa_DerrameCual(item.PA_DerrameCual || "");
    self.editPa_TiroidesCual(item.PA_TiroidesCual || "");
    self.editPa_AnestesicoCual(item.PA_AnestesicoCual || "");
    self.editPa_AceptaTransfusionCual(item.PA_AceptaTransfusionCual || "");
    self.editPa_EmbarazoCual(item.PA_EmbarazoCual || "");
    self.editPa_AI_Medicamentos(item.AI_Medicamentos || "");
    self.editPa_AI_ActividadDetalle(item.AI_ActividadDetalle || "");
    self.editPa_AI_OperacionesPrevias(item.AI_OperacionesPrevias || "");
    self.editPa_AI_Comentarios(item.AI_Comentarios || "");
    self.editPa_ProcedimientoProgramado(item.ProcedimientoProgramado || "");

    setTimeout(function () {
      function setRadio(name, value) {
        if (!value) return;
        var el = document.querySelector('#mdl-editar-preanestesico input[name="' + name + '"][value="' + value + '"]');
        if (el) el.checked = true;
      }
      setRadio("edit_pa_alergia", item.PA_Alergia);
      setRadio("edit_pa_fuma", item.PA_Fuma);
      setRadio("edit_pa_drogas", item.PA_Drogas);
      setRadio("edit_pa_alcohol", item.PA_Alcohol);
      setRadio("edit_pa_embarazo", item.PA_Embarazo);
      setRadio("edit_pa_transfusion", item.PA_Transfusion);
      setRadio("edit_pa_asma", item.PA_Asma);
      setRadio("edit_pa_pulmones", item.PA_Pulmones);
      setRadio("edit_pa_corazon", item.PA_Corazon);
      setRadio("edit_pa_ataque", item.PA_AtaqueCardiaco);
      setRadio("edit_pa_angina", item.PA_Angina);
      setRadio("edit_pa_soplo", item.PA_Soplo);
      setRadio("edit_pa_presion", item.PA_Presion);
      setRadio("edit_pa_higado", item.PA_Higado);
      setRadio("edit_pa_rinones", item.PA_Rinones);
      setRadio("edit_pa_diabetes", item.PA_Diabetes);
      setRadio("edit_pa_epilepsia", item.PA_Epilepsia);
      setRadio("edit_pa_derrame", item.PA_Derrame);
      setRadio("edit_pa_tiroides", item.PA_Tiroides);
      setRadio("edit_pa_anestesico", item.PA_Anestesico);
      setRadio("edit_pa_acepta_trans", item.PA_AceptaTransfusion);
      setRadio("edit_pa_actividad", item.AI_Actividad);
    }, 200);

    $("#mdl-editar-preanestesico").dialog({ width: 1000, title: "Editar Cuestionario Pre-Anestésico #" + item.Id });
  };

  self.guardarEdicionCuestionarioPreAnest = function () {
    var id = self.editPaId();
    if (!id || id <= 0) { mensajeEmergenteError("ID de cuestionario no válido."); return; }
    function radio(name) {
      var el = document.querySelector('#mdl-editar-preanestesico input[name="' + name + '"]:checked');
      return el ? el.value : "";
    }
    var payload = {
      Id: id, HospitalizacionId: parseInt($("#HospitalizacionId").val()),
      NombreCompleto: self.editPa_NombreCompleto(),
      RegistroMedico: self.editPa_RegistroMedico(),
      Edad: self.editPa_Edad(),
      FechaCuestionario: self.editPa_FechaCuestionario() || null,
      Peso: parseFloat(self.editPa_Peso()) || null,
      Estatura: parseFloat(self.editPa_Estatura()) || null,
      FechaUltimaRegla: self.editPa_FechaUltimaRegla() || null,
      FechaProcedimiento: self.editPa_FechaProcedimiento() || null,
      ProcedimientoProgramado: self.editPa_ProcedimientoProgramado(),
      Cirujano: self.editPa_Cirujano(),
      PA_Alergia: radio("edit_pa_alergia"), PA_AlergiaCual: self.editPa_AlergiaCual(),
      PA_Fuma: radio("edit_pa_fuma"), PA_FumaCuanto: self.editPa_FumaCuanto(),
      PA_Drogas: radio("edit_pa_drogas"), PA_DrogasCuales: self.editPa_DrogasCuales(),
      PA_Alcohol: radio("edit_pa_alcohol"), PA_AlcoholCuanto: self.editPa_AlcoholCuanto(),
      PA_Embarazo: radio("edit_pa_embarazo"),
      PA_EmbarazoCual: self.editPa_EmbarazoCual(),
      PA_Transfusion: radio("edit_pa_transfusion"),
      PA_TransfusionCual: self.editPa_TransfusionCual(),
      PA_Asma: radio("edit_pa_asma"),
      PA_AsmaCual: self.editPa_AsmaCual(),
      PA_Pulmones: radio("edit_pa_pulmones"),
      PA_PulmonesCual: self.editPa_PulmonesCual(),
      PA_Corazon: radio("edit_pa_corazon"),
      PA_CorazonCual: self.editPa_CorazonCual(),
      PA_AtaqueCardiaco: radio("edit_pa_ataque"),
      PA_AtaqueCardiacoCual: self.editPa_AtaqueCardiacoCual(),
      PA_Angina: radio("edit_pa_angina"),
      PA_AnginaCual: self.editPa_AnginaCual(),
      PA_Soplo: radio("edit_pa_soplo"),
      PA_SoploCual: self.editPa_SoploCual(),
      PA_Presion: radio("edit_pa_presion"),
      PA_PresionCual: self.editPa_PresionCual(),
      PA_Higado: radio("edit_pa_higado"),
      PA_HigadoCual: self.editPa_HigadoCual(),
      PA_Rinones: radio("edit_pa_rinones"),
      PA_RinonesCual: self.editPa_RinonesCual(),
      PA_Diabetes: radio("edit_pa_diabetes"),
      PA_DiabetesCual: self.editPa_DiabetesCual(),
      PA_Epilepsia: radio("edit_pa_epilepsia"),
      PA_EpilepsiaCual: self.editPa_EpilepsiaCual(),
      PA_Derrame: radio("edit_pa_derrame"),
      PA_DerrameCual: self.editPa_DerrameCual(),
      PA_Tiroides: radio("edit_pa_tiroides"),
      PA_TiroidesCual: self.editPa_TiroidesCual(),
      PA_Anestesico: radio("edit_pa_anestesico"),
      PA_AnestesicoCual: self.editPa_AnestesicoCual(),
      PA_AceptaTransfusion: radio("edit_pa_acepta_trans"),
      PA_AceptaTransfusionCual: self.editPa_AceptaTransfusionCual(),
      AI_Medicamentos: self.editPa_AI_Medicamentos(),
      AI_Actividad: radio("edit_pa_actividad"),
      AI_ActividadDetalle: self.editPa_AI_ActividadDetalle(),
      AI_OperacionesPrevias: self.editPa_AI_OperacionesPrevias(),
      AI_Comentarios: self.editPa_AI_Comentarios(),
    };
    showLoading();
    $.ajax({
      url: "/CuestionarioPreAnestesico/ActualizarCuestionario",
      method: "POST", contentType: "application/json", data: JSON.stringify(payload),
      success: function (r) {
        hideLoading();
        var d = typeof r === "string" ? JSON.parse(r) : r;
        if (d.exitoso) { $("#mdl-editar-preanestesico").dialog("close"); self.consultarCuestionariosPreAnest(); mensajeEmergente("Cuestionario pre-anestésico actualizado correctamente"); }
        else { mensajeEmergenteError(d.resultado || d.mensaje || "Error al actualizar"); }
      },
      error: function () { hideLoading(); mensajeEmergenteError("Error de servidor al actualizar el cuestionario pre-anestésico"); }
    });
  };

  // self.agregarNotaOperatoria = function () {
  //   var nuevaNotaOperatoria = {
  //     Diagnostico: self.notaDiagnosticoOperatoria(),
  //     HospitalizacionId: $("#HospitalizacionId").val(),
  //     FechaOperacion: self.notaFechaOperacion(),
  //     HoraComenzo: self.notaHoraComenzo(),
  //     HoraTermino: self.notaHoraTermino(),
  //     Cirujano: self.notaCirujano(),
  //     PrimerAyudante: self.notaPrimerAyudante(),
  //     SegundoAyudante: self.notaSegundoAyudante(),
  //     Anestesista: self.notaAnestesista(),
  //     Instrumentista: self.notaInstrumentista(),
  //     Circulante: self.notaCirculante(),
  //     DiagnosticoPreOperatorio: self.notaDiagnosticoPreOp(),
  //     DiagnosticoPostOperatorio: self.notaDiagnosticoPostOp(),
  //     OperacionEfectuada: self.notaOperacionEfectuada(),
  //     HallazgosTransOperatorios: self.notaHallazgosTransOp(),
  //   };

  //   showLoading();
  //   $.ajax({
  //     url: "/NotaOperatoria/AgregarNotaOperatoria",
  //     method: "POST",
  //     contentType: "application/json",
  //     data: JSON.stringify(nuevaNotaOperatoria),
  //     success: function (dataResult) {
  //       hideLoading();
  //       let data = JSON.parse(dataResult);
  //       if (data.exitoso) {
  //         self.consultarNotasOperatorias();
  //         self.limpiarModalNotaOperatoria();
  //         $("#mdl-agregar-nota-operatoria").dialog("close");
  //         mensajeEmergente("Nota operatoria agregada exitosamente");
  //       } else {
  //         mensajeEmergenteError(data.mensaje || data.resultado);
  //       }
  //     },
  //     error: function (dataError) {
  //       hideLoading();
  //       console.error("Error ajax:", dataError.responseText);
  //       mensajeEmergenteError("Error al agregar la nota operatoria");
  //     },
  //   });
  // };




  self.agregarNotaOperatoria = function () {
    var hospId = parseInt($("#HospitalizacionId").val());
    if (!hospId || hospId === 0) {
      mensajeEmergenteError("No se encontró el ID de hospitalización.");
      return;
    }

    // Forzar sincronización de TODOS los textareas del modal de creación
    $("#diagnosticoPreOperatorio_NO, #diagnosticoPostOperatorio_NO, #operacionEfectuada_NO, #hallazgos_NO, #notaDiagnosticoOperatoria_NO").each(function () {
      var $ta = $(this);
      var bind = $ta.attr("data-bind");
      if (bind) {
        var match = bind.match(/textInput:\s*(\w+)/);
        if (match && match[1] && self[match[1]]) {
          // Forzamos el valor del input al observable
          self[match[1]]($ta.val());
        }
      }
    });

    var nuevaNota = {
      Diagnostico: self.notaDiagnosticoOperatoria(),
      HospitalizacionId: hospId,
      FechaOperacion: self.notaFechaOperacion() || null,
      HoraComenzo: self.notaHoraComenzo(),
      HoraTermino: self.notaHoraTermino(),
      Cirujano: self.notaCirujano(),
      PrimerAyudante: self.notaPrimerAyudante(),
      SegundoAyudante: self.notaSegundoAyudante(),
      Anestesista: self.notaAnestesista(),
      Instrumentista: self.notaInstrumentista(),
      Circulante: self.notaCirculante(),
      DiagnosticoPreOperatorio: self.notaDiagnosticoPreOp() || "",
      DiagnosticoPostOperatorio: self.notaDiagnosticoPostOp() || "",
      OperacionEfectuada: self.notaOperacionEfectuada() || "",
      HallazgosTransOperatorios: self.notaHallazgosTransOp() || ""
    };

    showLoading();
    $.ajax({
      url: "/NotaOperatoria/AgregarNotaOperatoria",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(nuevaNota),
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarNotasOperatorias();
          self.limpiarModalNotaOperatoria();
          $("#mdl-agregar-nota-operatoria").dialog("close");
          mensajeEmergente("Nota operatoria agregada exitosamente");
        } else {
          mensajeEmergenteError(data.resultado || "Error al guardar.");
        }
      },
      error: function (dataError) {
        hideLoading();
        mensajeEmergenteError("Error al agregar la nota operatoria.");
      }
    });
  };
  self.limpiarModalNotaOperatoria = function () {
    self.notaDiagnosticoOperatoria("");
    self.notaFechaOperacion("");
    self.notaHoraComenzo("");
    self.notaHoraTermino("");
    self.notaCirujano("");
    self.notaPrimerAyudante("");
    self.notaSegundoAyudante("");
    self.notaAnestesista("");
    self.notaInstrumentista("");
    self.notaCirculante("");
    self.notaDiagnosticoPreOp("");
    self.notaDiagnosticoPostOp("");
    self.notaOperacionEfectuada("");
    self.notaHallazgosTransOp("");
  };

  self.consultarNotasOperatorias = function () {
    showLoading();
    $.ajax({
      url: "/NotaOperatoria/ObtenerNotasOperatorias",
      method: "POST",
      data: {
        idHospitalizacion: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.exitoso) {
          // Helper: lee propiedad en PascalCase o camelCase (cubre ambos casos del servidor)
          function prop(obj, name) {
            if (obj[name] !== undefined && obj[name] !== null) return obj[name];
            var lower = name.charAt(0).toLowerCase() + name.slice(1);
            if (obj[lower] !== undefined && obj[lower] !== null) return obj[lower];
            return "";
          }

          var notas = (data.resultado || []).map(function (n) {
            return {
              Id: prop(n, "Id") || null,
              Diagnostico: prop(n, "Diagnostico"),
              Profesional: prop(n, "Profesional"),
              FechaRegistro: prop(n, "FechaRegistro"),
              HospitalizacionId: prop(n, "HospitalizacionId") || null,
              FechaOperacion: prop(n, "FechaOperacion"),
              HoraComenzo: prop(n, "HoraComenzo"),
              HoraTermino: prop(n, "HoraTermino"),
              Cirujano: prop(n, "Cirujano"),
              PrimerAyudante: prop(n, "PrimerAyudante"),
              SegundoAyudante: prop(n, "SegundoAyudante"),
              Anestesista: prop(n, "Anestesista"),
              Instrumentista: prop(n, "Instrumentista"),
              Circulante: prop(n, "Circulante"),
              DiagnosticoPreOperatorio: prop(n, "DiagnosticoPreOperatorio"),
              DiagnosticoPostOperatorio: prop(n, "DiagnosticoPostOperatorio"),
              OperacionEfectuada: prop(n, "OperacionEfectuada"),
              HallazgosTransOperatorios: prop(n, "HallazgosTransOperatorios"),
              FirmaRuta: prop(n, "FirmaRuta"),
              FechaFirma: prop(n, "FechaFirma"),
              Firmado: !!(prop(n, "FirmaRuta")),
            };
          });

          console.log("[NotaOperatoria] Primera nota normalizada:", notas[0]);
          self.notasOperatorias(notas);

          // Refrescar notaOperatoriaDetalle si corresponde a una nota del listado
          var detalleActivo = self.notaOperatoriaDetalle();
          if (detalleActivo) {
            var notaFresca = notas.find(function (n) { return n.Id === detalleActivo.Id; });
            if (notaFresca) {
              self.notaOperatoriaDetalle(notaFresca);
            }
          }
        } else {
          mensajeEmergenteError(
            data.mensaje || "Ocurrió un error al obtener las notas operatorias."
          );
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de red o del servidor. Intenta nuevamente.");
      },
    });
  };


  // Carga los selects del equipo quirúrgico en el modal indicado.
  // destino = "agregar" (default) -> usa observables notaCirujano etc.
  // destino = "editar"            -> usa observables editNotaCirujano etc.
  self.cargarMedicosModal = function (valoresCita, destino) {
    var esEditar = (destino === "editar");

    // Resuelve nombre desde valor (ID numerico o nombre directo)
    function resolverNombre(valor, lista) {
      if (!valor || valor === "") return "";
      if (/^\d+$/.test(String(valor))) {
        var emp = lista.find(function (e) { return String(e.id) === String(valor); });
        return emp ? emp.nombre : "";
      }
      return valor;
    }

    var cfg = [
      {
        obs: self.listaMedicosModal, esp: "", uni: "", setFns: [
          function (d) {
            var nombre = valoresCita.cirujanoNombre
              || (valoresCita.cirujanoId ? resolverNombre(valoresCita.cirujanoId, d) : "");
            if (esEditar) self.editNotaCirujano(nombre);
            else self.notaCirujano(nombre);
          },
          function (d) {
            var v = resolverNombre(valoresCita.primerAyudante, d);
            if (esEditar) self.editNotaPrimerAyudante(v);
            else self.notaPrimerAyudante(v);
          },
          function (d) {
            var v = resolverNombre(valoresCita.segundoAyudante, d);
            if (esEditar) self.editNotaSegundoAyudante(v);
            else self.notaSegundoAyudante(v);
          }
        ]
      },
      {
        obs: self.listaAnestesistas, esp: "22", uni: "", setFns: [
          function (d) {
            var v = resolverNombre(valoresCita.anestesista, d);
            if (esEditar) self.editNotaAnestesista(v);
            else self.notaAnestesista(v);
          }
        ]
      },
      {
        obs: self.listaInstrumentistas, esp: "", uni: "12", setFns: [
          function (d) {
            var v = resolverNombre(valoresCita.instrumentista, d);
            if (esEditar) self.editNotaInstrumentista(v);
            else self.notaInstrumentista(v);
          }
        ]
      },
      {
        obs: self.listaCirculantes, esp: "", uni: "13", setFns: [
          function (d) {
            var v = resolverNombre(valoresCita.circulante, d);
            if (esEditar) self.editNotaCirculante(v);
            else self.notaCirculante(v);
          }
        ]
      },
    ];

    cfg.forEach(function (item) {
      var params = {};
      if (item.esp !== "") params.especialidadId = item.esp;
      if (item.uni !== "") params.unidadOrgId = item.uni;

      $.ajax({
        url: "/Cita/GetEmpleadosPorTipo",
        type: "GET",
        data: params,
        success: function (data) {
          item.obs(data);
          // Asignar valor DESPUÉS de cargar las opciones — así KO puede preseleccionar
          item.setFns.forEach(function (fn) { fn(data); });
        },
        error: function () { console.error("Error al cargar empleados modal operatorio"); }
      });
    });
  };

  self.cargarPersonalEnfermeria = function (callback) {
    $.ajax({
      url: "/NotaOperatoria/ObtenerPersonalEnfermeria",
      method: "GET",
      success: function (response) {
        if (response.success) {
          self.listaPersonalEnfermeria(response.data);
          console.log("Personal de enfermería cargado:", response.data);
          if (typeof callback === "function") callback();
        } else {
          console.error("Error al cargar personal de enfermería:", response.message);
        }
      },
      error: function (xhr, status, error) {
        console.error("Error en la petición:", error);
      }
    });
  };


  self.inicializarSelect2Personal = function () {
    setTimeout(function () {
      $('.select2-personal').select2({
        width: '100%',
        placeholder: 'Seleccione un instrumentista...',
        allowClear: true,
        language: {
          noResults: function () { return "No se encontraron resultados"; }
        }
      });
    }, 200);
  };



  //Usuarios acceso

  // Método para agregar usuario con acceso
  // Método para agregar usuario con acceso
  self.agregarUsuarioAcceso = function () {
    var usuario = self.usuarioSeleccionado();
    if (!usuario) {
      alert("Seleccione un usuario.");
      return;
    }

    var nuevoAcceso = {
      HospitalizacionId: parseInt($("#HospitalizacionId").val()),
      UserId: usuario.Id,
    };
    showLoading();
    $.ajax({
      url: "/HospitalizacionUsuarioAcceso/Agregar",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(nuevoAcceso), // Aquí enviamos los datos como JSON

      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarUsuariosAcceso();
          mensajeEmergente("Usuario agregado exitosamente.");
          $("#mdl-agregar-usuarios-acceso").dialog("close");
        } else {
          alert(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al agregar el usuario.");
      },
    });
  };

  // Método para consultar usuarios con acceso
  self.consultarUsuariosAcceso = function () {
    showLoading();
    $.ajax({
      url: "/HospitalizacionUsuarioAcceso/GetAllByIdHospitalizacion",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.usuariosAcceso(data.resultado);
        } else {
          alert(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        mensajeEmergenteError("Error al consultar los usuarios con acceso.");
      },
    });
  };
  self.actualizarPermisos = function (usuarioAcceso) {
    let textoCargando = $("#texto-actualizando-permisos");
    let textoError = $("#texto-error-actualizar-permisos");
    textoCargando.show();
    textoError.hide();
    $.ajax({
      url: "/HospitalizacionUsuarioAcceso/ActualizarPermiso",
      method: "POST",
      data: {
        model: usuarioAcceso,
      },
      success: function (result) {
        textoCargando.hide();
        let data = JSON.parse(result);
        if (data.Exitoso) {
          mensajeEmergente("Permisos actualizados");
        } else {
          textoError.show();
        }
      },
      error: function (resultError) {
        textoCargando.hide();
        textoError.show();
        console.log(resultError);
      },
    });
  };

  // Método para consultar todos los usuarios disponibles
  self.consultarUsuarios = function () {
    showLoading();
    $.ajax({
      url: "/User/ConsultarUsuarios",
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.listaUsuarios(data.Resultado);
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al consultar los usuarios.");
      },
    });
  };

  self.eliminarUsuarioAcceso = function (usuario) {
    if (
      !confirm("¿Está seguro de que desea eliminar el acceso de este usuario?")
    ) {
      return;
    }

    showLoading();
    $.ajax({
      url: "/HospitalizacionUsuarioAcceso/Eliminar",
      method: "POST",
      data: { id: usuario.Id }, // Enviar los datos como parámetros de formulario
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.exitoso) {
          self.consultarUsuariosAcceso();
          mensajeEmergente("Usuario eliminado exitosamente.");
        } else {
          alert(data.mensaje);
        }
      },
      error: function (dataError) {
        hideLoading();
        console.log(dataError);
        alert("Error al eliminar el usuario.");
      },
    });
  };

  self.cancelarCambioHabitacion = function () {
    $("#mdl-cambiar-habitacion").dialog("close");
  };

  self.verEstadoCuenta = function () {
    window.open(
      "/CuentasPorCobrar/VerDetallesCuenta?cuentaId=" + $("#CuentaId").val(),
      "_blank",
    );
  };



  // ============================================================
  // FUNCIÓN PARA OBTENER EL ID DE EMERGENCIA ASOCIADO
  // ============================================================
  self.consultarIdEmergencia = function () {
    var hospId = self.hospitalizacionId();
    console.log("consultarIdEmergencia para hospitalización:", hospId);

    if (!hospId || hospId <= 0) {
      console.log("Hospitalización ID inválido");
      return;
    }

    $.ajax({
      url: '/Hospitalizacion/ObtenerEmergenciaId/' + hospId,
      type: 'GET',
      success: function (response) {
        console.log("Respuesta ObtenerEmergenciaId:", response);
        var data = JSON.parse(response);
        if (data.Exitoso && data.EmergenciaId > 0) {
          console.log("Emergencia encontrada ID:", data.EmergenciaId);
          self.emergenciaId(data.EmergenciaId);

          self.consultarElementosEmergencia();           // Carga productos, servicios, exámenes ya agregados
          self.consultarProductosExistentesEmergencia(); // Llena el select de productos
          self.consultarServiciosExistentesEmergencia(); // Llena el select de servicios
          self.consultarExamenesExistentesEmergencia();  // Llena el select de exámenes
        } else {
          console.log("No hay emergencia asociada a esta hospitalización");
          self.emergenciaId(0);
        }
      },
      error: function (err) {
        console.log("Error consultando emergencia:", err);
        self.emergenciaId(0);
      }
    });
  };


  // ============================================================
  // MÉTODOS PARA EMERGENCIAS
  // ============================================================

  self.consultarElementosEmergencia = function () {
    let emergenciaId = self.emergenciaId();
    if (!emergenciaId || emergenciaId === 0) return;

    showLoading();
    $.ajax({
      method: "POST",
      url: '/Emergencias/ConsultarElementosEmergencia',
      data: { emergenciaId: emergenciaId },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.productosEmergenciaHospi([]);
          self.serviciosEmergenciaHospi([]);
          self.examenesEmergenciaHospi([]);

          $(data.Resultado.Productos).each(function (idx, producto) {
            let cantidad = parseFloat(producto.Cantidad) || 1;
            let valorUnitario = parseFloat(producto.ValorUnitario) || 0;
            let descuentoPorcentaje = parseFloat(producto.DescuentoPorcentaje) || 0;

            let subtotal = cantidad * valorUnitario;
            let descuentoValor = subtotal * (descuentoPorcentaje / 100);
            let total = subtotal - descuentoValor;

            producto.Eliminado = ko.observable(producto.Eliminado);
            producto.Cantidad = ko.observable(producto.Cantidad);
            producto.ValorUnitario = ko.observable(producto.ValorUnitario);
            producto.DescuentoPorcentaje = ko.observable(producto.DescuentoPorcentaje);
            producto.DescuentoValor = ko.observable(descuentoValor);
            producto.ValorSubtotal = ko.observable(subtotal);
            producto.ValorTotal = ko.observable(total);

            self.productosEmergenciaHospi.push(producto);
          });

          $(data.Resultado.Servicios).each(function (idx, servicio) {
            let cantidad = parseFloat(servicio.Cantidad) || 1;
            let valorUnitario = parseFloat(servicio.ValorUnitario) || 0;

            let subtotal = cantidad * valorUnitario;

            servicio.Eliminado = ko.observable(servicio.Eliminado);
            servicio.Cantidad = servicio.Cantidad;
            servicio.ValorUnitario = valorUnitario;
            servicio.DescuentoValor = ko.observable(0);
            servicio.ValorSubtotal = ko.observable(subtotal);
            servicio.ValorTotal = ko.observable(subtotal);

            self.serviciosEmergenciaHospi.push(servicio);
          });

          $(data.Resultado.Examenes).each(function (idx, examen) {
            let cantidad = parseFloat(examen.Cantidad) || 1;
            let valorUnitario = parseFloat(examen.ValorUnitario) || 0;

            let subtotal = cantidad * valorUnitario;

            examen.Eliminado = ko.observable(examen.Eliminado);
            examen.Cantidad = examen.Cantidad;
            examen.ValorUnitario = valorUnitario;
            examen.DescuentoValor = ko.observable(0);
            examen.ValorSubtotal = ko.observable(subtotal);
            examen.ValorTotal = ko.observable(subtotal);

            self.examenesEmergenciaHospi.push(examen);
          });

          self.actualizarTotalesEmergencia();
        } else {
          mensajeEmergenteError(data.Mensaje);
        }
      },
      error: function (data) {
        hideLoading();
        mensajeEmergenteError("Error al consultar elementos de emergencia");
        console.log(data);
      }
    });
  };

  self.consultarProductosExistentesEmergencia = function () {
    $.ajax({
      method: "POST",
      url: '/Emergencias/ConsultarProductosExistentes',
      success: function (dataResult) {
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.registrosInventario(data.Resultado);
          let productoIds = new Set();
          $(self.registrosInventario()).each(function (idx, vl) {
            productoIds.add(vl.ProductoId);
          });

          let productosTemp = [];
          for (let id of productoIds) {
            let agregado = false;
            $(self.registrosInventario()).each(function (idx2, vl2) {
              if (!agregado && id == vl2.ProductoId) {
                let productoExistente = {
                  ProductoId: vl2.ProductoId,
                  ProductoCodigo: vl2.ProductoCodigo,
                  ProductoNombre: vl2.ProductoNombre,
                  ProductoNombreMostrar: vl2.ProductoNombre + " - " + vl2.ProductoActivoConcentracion
                };
                productosTemp.push(productoExistente);
                agregado = true;
              }
            });
          }
          self.productosExistentesEmergencia(productosTemp);
        }
      },
      error: function (data) {
        console.log("Error consultando productos:", data);
      }
    });
  };

  self.consultarServiciosExistentesEmergencia = function () {
    $.ajax({
      method: "POST",
      url: '/Venta/ConsultarServiciosExistentes',
      success: function (dataResult) {
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.serviciosExistentesEmergencia(data.Resultado);
        }
      },
      error: function (data) {
        console.log("Error consultando servicios:", data);
      }
    });
  };

  self.consultarExamenesExistentesEmergencia = function () {
    $.ajax({
      method: "POST",
      url: '/Venta/ConsultarExamenesExistentes',
      success: function (dataResult) {
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.examenesExistentesEmergencia(data.Resultado);
        }
      },
      error: function (data) {
        console.log("Error consultando exámenes:", data);
      }
    });
  };



  self.consultarUnidadesVentaProductoEmergencia = function (producto) {
    console.log("consultarUnidadesVentaProductoEmergencia llamado con:", producto);

    if (!producto || !producto.ProductoId) {
      console.log("Producto inválido");
      return;
    }

    self.unidadesVentaProductoEmergencia([]);
    let registrosProducto = [];

    $(self.registrosInventario()).each(function (idx, registro) {
      if (registro.ProductoId == producto.ProductoId) {
        registrosProducto.push(registro);
      }
    });

    console.log("Registros de inventario para producto:", registrosProducto);

    let unidadesVentaIds = new Set();
    $(registrosProducto).each(function (idx, registro) {
      if (registro.UnidadMedidaVentaId) {
        unidadesVentaIds.add(registro.UnidadMedidaVentaId);
      }
    });

    console.log("Unidades encontradas:", Array.from(unidadesVentaIds));

    for (let unidadId of unidadesVentaIds) {
      let agregado = false;
      $(registrosProducto).each(function (idx, vl) {
        if (vl.UnidadMedidaVentaId == unidadId && !agregado) {
          self.unidadesVentaProductoEmergencia.push({
            Id: unidadId,
            UnidadMedidaVentaNombre: vl.UnidadMedidaVentaNombre
          });
          agregado = true;
        }
      });
    }

    console.log("Unidades cargadas:", self.unidadesVentaProductoEmergencia());
  };


  // ══════════════════════════════════════════════════════════════
  //  HISTORIAL DE MEDICAMENTOS E INSUMOS
  // ══════════════════════════════════════════════════════════════
  self.verModalHistorialMedicamentos = function () {
    var tbody = document.getElementById('tablaHistorialMedicamentosBody');
    if (tbody) {
      tbody.innerHTML = '<tr><td colspan="11" class="text-center text-muted py-3">' +
        '<i class="fas fa-spinner fa-spin mr-2"></i> Cargando...</td></tr>';
    }

    // Limpiar filtros al abrir
    var selectOrigen = document.getElementById('selectFiltroOrigenMed');
    if (selectOrigen) selectOrigen.value = '';
    // Resetear botones activos a "Todos"
    document.querySelectorAll('.btn-group .btn').forEach(btn => btn.classList.remove('active'));
    var btnTodos = document.getElementById('btnFiltroTodos');
    if (btnTodos) btnTodos.classList.add('active');

    $("#mdl-historial-medicamentos").dialog({
      modal: true,
      width: 1300,
      title: "Historial de Medicamentos e Insumos",
      buttons: {
        "Cerrar": function () { $(this).dialog("close"); }
      }
    });

    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ConsultarHistorialMedicamentosHospitalizacion",
      method: "POST",
      data: { hospitalizacionId: $("#HospitalizacionId").val() },
      success: function (resp) {
        hideLoading();
        var data = (typeof resp === 'string') ? JSON.parse(resp) : resp;

        if (!data.Exitoso) {
          mensajeEmergenteError(data.Mensaje || "Error al cargar historial.");
          return;
        }

        var resultado = data.Resultado || [];
        // Llamar a la función central que asigna datos, actualiza badges, aplica filtros y pinta la tabla paginada
        if (typeof window.cargarDatosHistorial === 'function') {
          window.cargarDatosHistorial(resultado);
        } else {
          console.error("La función cargarDatosHistorial no está definida. Revisa que el script del modal se haya cargado.");
          mensajeEmergenteError("Error de inicialización del historial.");
        }
      },
      error: function (xhr) {
        hideLoading();
        console.error("Error historial medicamentos:", xhr);
        mensajeEmergenteError("No se pudo cargar el historial de medicamentos.");
      }
    });
  };

  self.consultarPreciosProductoEmergencia = function (unidadSeleccionada) {
    console.log("consultarPreciosProductoEmergencia llamado con unidad:", unidadSeleccionada);

    if (!unidadSeleccionada || !self.productoEmergenciaSeleccionado()) {
      console.log("Unidad o producto no seleccionado");
      self.preciosProductoEmergencia([]);
      return;
    }

    self.preciosProductoEmergencia([]);
    let registrosProducto = [];

    $(self.registrosInventario()).each(function (idx, registro) {
      if (registro.ProductoId == self.productoEmergenciaSeleccionado().ProductoId &&
        registro.UnidadMedidaVentaId == unidadSeleccionada.Id) {
        registrosProducto.push(registro);
      }
    });

    console.log("Registros para producto y unidad:", registrosProducto);

    let preciosIds = new Set();
    $(registrosProducto).each(function (idx, registro) {
      if (registro.PrecioId) {
        preciosIds.add(registro.PrecioId);
      }
    });

    console.log("Precios encontrados:", Array.from(preciosIds));

    for (let precioId of preciosIds) {
      let precios = [];
      $(registrosProducto).each(function (idx, vl) {
        if (vl.PrecioId == precioId) {
          precios.push({
            ProductoInventarioId: vl.ProductoInventarioId,
            Id: precioId,
            Precio: vl.PrecioNombre + " (Q " + vl.PrecioValor + ")",
            PrecioValor: vl.PrecioValor,
            PrecioCompra: vl.PrecioCompra
          });
        }
      });

      if (precios.length > 0) {
        self.preciosProductoEmergencia.push(precios[precios.length - 1]);
      }
    }

    console.log("Precios cargados:", self.preciosProductoEmergencia());
  };

  self.consultarPreciosServicioEmergencia = function () {
    if (!self.servicioEmergenciaSeleccionado()) {
      console.log("No hay servicio seleccionado");
      return;
    }

    console.log("Consultando precios para servicio ID:", self.servicioEmergenciaSeleccionado().ServicioId);

    $.ajax({
      method: "POST",
      url: '/Venta/ConsultarPreciosServicio',
      data: { servicioId: self.servicioEmergenciaSeleccionado().ServicioId },
      success: function (dataResult) {
        console.log("Respuesta de precios servicio:", dataResult);
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          console.log("Precios de servicio cargados:", data.Resultado);
          self.preciosServicioEmergencia(data.Resultado);
        } else {
          console.log("Error al cargar precios:", data.Mensaje);
        }
      },
      error: function (err) {
        console.log("Error en ajax:", err);
      }
    });
  };

  self.consultarPreciosExamenEmergencia = function () {
    if (!self.examenEmergenciaSeleccionado()) {
      console.log("No hay examen seleccionado");
      return;
    }

    console.log("Consultando precios para examen ID:", self.examenEmergenciaSeleccionado().ExamenId);

    $.ajax({
      method: "POST",
      url: '/Venta/ConsultarPreciosExamen',
      data: { examenLabClinicoId: self.examenEmergenciaSeleccionado().ExamenId },
      success: function (dataResult) {
        console.log("Respuesta de precios examen:", dataResult);
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          console.log("Precios de examen cargados:", data.Resultado);
          self.preciosExamenEmergencia(data.Resultado);
        } else {
          console.log("Error al cargar precios:", data.Mensaje);
        }
      },
      error: function (err) {
        console.log("Error en ajax:", err);
      }
    });
  };


  self.agregarProductoEmergencia = function () {
    if (!self.productoEmergenciaSeleccionado() || !self.precioSeleccionadoProductoEmergencia()) {
      alert("Seleccione producto y precio");
      return;
    }

    let cantidad = parseFloat(self.cantidadProductoEmergenciaAgregar()) || 1;
    let valorUnitario = parseFloat(self.precioSeleccionadoProductoEmergencia().PrecioValor) || 0;
    let subtotal = cantidad * valorUnitario;

    let data = {
      ProductId: self.productoEmergenciaSeleccionado().ProductoId,
      ServicioId: null,
      Cantidad: cantidad,
      PrecioValor: valorUnitario,
      Descuento: 0,
      Subtotal: subtotal,
      Total: subtotal,
      EmergencialId: self.emergenciaId(),
      ExamenLabClinicId: null,
      Eliminado: false,
      Preciold: self.precioSeleccionadoProductoEmergencia().Id,
      UnidadMedidaVentad: self.unidadVentaSeleccionadaProductoEmergencia() ? self.unidadVentaSeleccionadaProductoEmergencia().Id : null,
      DescuentoPorcentaje: 0
    };

    showLoading();
    $.ajax({
      url: '/Emergencias/AgregarProductoEmergencia',
      type: 'POST',
      contentType: 'application/json',
      data: JSON.stringify(data),
      success: function (response) {
        hideLoading();

        if (response.exitoso === true || response.Exitoso === true) {
          mensajeEmergente(response.mensaje || response.Mensaje || "Producto agregado a emergencia");

          self.cantidadProductoEmergenciaAgregar(1);
          self.productoEmergenciaSeleccionado(null);
          self.precioSeleccionadoProductoEmergencia(null);
          self.unidadVentaSeleccionadaProductoEmergencia(null);

          self.consultarElementosEmergencia();
        } else {
          mensajeEmergenteError(response.mensaje || response.Mensaje || "Error al agregar producto");
        }
      },
      error: function (xhr, status, error) {
        hideLoading();
        mensajeEmergenteError("Error de comunicación: " + error);
      }
    });
  };

  self.agregarServicioEmergenciaHospi = function () {
    if (!self.servicioEmergenciaSeleccionado() || !self.precioSeleccionadoServicioEmergencia()) {
      alert("Seleccione servicio y precio");
      return;
    }

    let cantidad = parseInt(self.cantidadServicioEmergenciaAgregar()) || 1;
    let valorUnitario = parseFloat(self.precioSeleccionadoServicioEmergencia().PrecioValor) || 0;
    let subtotal = cantidad * valorUnitario;

    let data = {
      ProductId: null,
      ServicioId: self.servicioEmergenciaSeleccionado().ServicioId,
      Cantidad: cantidad,
      PrecioValor: valorUnitario,
      Descuento: 0,
      Subtotal: subtotal,
      Total: subtotal,
      EmergencialId: self.emergenciaId(),
      ExamenLabClinicId: null,
      Eliminado: false,
      Preciold: self.precioSeleccionadoServicioEmergencia().PrecioId,
      UnidadMedidaVentad: null,
      DescuentoPorcentaje: 0
    };

    showLoading();
    $.ajax({
      url: '/Emergencias/AgregarServicioEmergencia',
      type: 'POST',
      contentType: 'application/json',
      data: JSON.stringify(data),
      success: function (response) {
        hideLoading();

        if (response.exitoso === true || response.Exitoso === true) {
          mensajeEmergente(response.mensaje || response.Mensaje || "Servicio agregado a emergencia");

          self.cantidadServicioEmergenciaAgregar(1);
          self.servicioEmergenciaSeleccionado(null);
          self.precioSeleccionadoServicioEmergencia(null);

          self.consultarElementosEmergencia();
        } else {
          mensajeEmergenteError(response.mensaje || response.Mensaje || "Error al agregar servicio");
        }
      },
      error: function (xhr, status, error) {
        hideLoading();
        mensajeEmergenteError("Error de comunicación: " + error);
      }
    });
  };

  self.agregarExamenEmergencia = function () {
    if (!self.examenEmergenciaSeleccionado() || !self.precioSeleccionadoExamenEmergencia()) {
      alert("Seleccione examen y precio");
      return;
    }

    let cantidad = parseInt(self.cantidadExamenEmergenciaAgregar()) || 1;
    let valorUnitario = parseFloat(self.precioSeleccionadoExamenEmergencia().PrecioValor) || 0;
    let subtotal = cantidad * valorUnitario;

    let data = {
      ProductId: null,
      ServicioId: null,
      Cantidad: cantidad,
      PrecioValor: valorUnitario,
      Descuento: 0,
      Subtotal: subtotal,
      Total: subtotal,
      EmergencialId: self.emergenciaId(),
      ExamenLabClinicId: self.examenEmergenciaSeleccionado().ExamenId,
      Eliminado: false,
      Preciold: self.precioSeleccionadoExamenEmergencia().PrecioId,
      UnidadMedidaVentad: null,
      DescuentoPorcentaje: 0
    };

    showLoading();
    $.ajax({
      url: '/Emergencias/AgregarExamenEmergencia',
      type: 'POST',
      contentType: 'application/json',
      data: JSON.stringify(data),
      success: function (response) {
        hideLoading();

        if (response.exitoso === true || response.Exitoso === true) {
          mensajeEmergente(response.mensaje || response.Mensaje || "Examen agregado a emergencia");

          self.cantidadExamenEmergenciaAgregar(1);
          self.examenEmergenciaSeleccionado(null);
          self.precioSeleccionadoExamenEmergencia(null);

          self.consultarElementosEmergencia();
        } else {
          mensajeEmergenteError(response.mensaje || response.Mensaje || "Error al agregar examen");
        }
      },
      error: function (xhr, status, error) {
        hideLoading();
        mensajeEmergenteError("Error de comunicación: " + error);
      }
    });
  };

  self.quitarElementoEmergencia = function (elemento, tipo) {
    if (!confirm("¿Desea eliminar este elemento?")) return;

    if (!elemento.Id) {
      console.error("Elemento sin Id:", elemento);
      mensajeEmergenteError("Error: No se puede eliminar, falta el Id del detalle");
      return;
    }

    let detalleId = elemento.Id;
    console.log("Eliminando detalleId:", detalleId);

    showLoading();
    $.ajax({
      url: '/Emergencias/EliminarDetalleEmergencia',
      type: 'POST',
      data: {
        detalleId: detalleId
      },
      success: function (response) {
        hideLoading();

        if (response.exitoso === true || response.Exitoso === true) {
          mensajeEmergente("Elemento eliminado");
          self.consultarElementosEmergencia();
        } else {
          mensajeEmergenteError(response.mensaje || response.Mensaje || "Error al eliminar");
        }
      },
      error: function (xhr, status, error) {
        hideLoading();
        console.error("Error:", error);
        mensajeEmergenteError("Error de comunicación");
      }
    });
  };


  self.actualizarTotalesEmergencia = function () {
    let totalProductos = 0;
    let totalServicios = 0;
    let totalExamenes = 0;
    let totalGeneral = 0;

    $(self.productosEmergenciaHospi()).each(function (idx, producto) {
      if (!producto.Eliminado()) {
        let total = parseFloat(producto.ValorTotal()) || 0;
        totalProductos += total;
        totalGeneral += total;
      }
    });

    $(self.serviciosEmergenciaHospi()).each(function (idx, servicio) {
      if (!servicio.Eliminado()) {
        let total = parseFloat(servicio.ValorTotal()) || 0;
        totalServicios += total;
        totalGeneral += total;
      }
    });

    $(self.examenesEmergenciaHospi()).each(function (idx, examen) {
      if (!examen.Eliminado()) {
        let total = parseFloat(examen.ValorTotal()) || 0;
        totalExamenes += total;
        totalGeneral += total;
      }
    });

    self.totalProductosEmergencia(totalProductos);
    self.totalServiciosEmergencia(totalServicios);
    self.totalExamenesEmergencia(totalExamenes);

    self.valorTotalEmergencias(totalGeneral);
    self.valorTotalEmergenciasTexto("Q. " + totalGeneral.toFixed(2));

    self.actualizarTotales();
  };

  self.productoEmergenciaSeleccionado.subscribe(function (value) {
    console.log("Producto seleccionado:", value);
    self.consultarUnidadesVentaProductoEmergencia(value);
  });

  self.unidadVentaSeleccionadaProductoEmergencia.subscribe(function (unidad) {
    console.log("Unidad seleccionada:", unidad);
    self.consultarPreciosProductoEmergencia(unidad);
  });

  self.servicioEmergenciaSeleccionado.subscribe(function (value) {
    console.log("Servicio seleccionado:", value);
    self.consultarPreciosServicioEmergencia();
  });

  self.examenEmergenciaSeleccionado.subscribe(function (value) {
    console.log(" Examen seleccionado:", value);
    self.consultarPreciosExamenEmergencia();
  });



  // Firmas para Turno de Enfermería
  self.firmaTurnoId = ko.observable(null);
  self.firmaTurnoBase64 = ko.observable(null);
  self.firmaTurnoExistenteUrl = ko.observable(null);
  self._firmaTurnoModalEl = null;


  // ── Modal Firma Nota de Enfermería ───────────────────────────────────────────
  self.abrirModalFirmaNotaEnfermeria = function (nota) {
    self.firmaEnfermeriaId(nota.Id);
    self.firmaEnfermeriaBase64(null);
    self.firmaEnfermeriaExistenteUrl(null);

    // Resetear UI
    var selector = document.getElementById("selectorFirmaNotaEnf");
    var previsualizacion = document.getElementById("contenedorPrevFirmaNotaEnf");
    var btnFirmar = document.getElementById("btnConfirmarFirmaNotaEnf");
    var imgPrev = document.getElementById("imgPrevFirmaNotaEnf");
    if (selector) selector.classList.remove("d-none");
    if (previsualizacion) previsualizacion.classList.add("d-none");
    if (btnFirmar) btnFirmar.setAttribute("disabled", "disabled");
    if (imgPrev) imgPrev.src = "";

    // Activar pestaña canvas por defecto
    var tabCanvas = document.getElementById("tabCanvasFirmaNotaEnf");
    var tabArchivo = document.getElementById("tabArchivoFirmaNotaEnf");
    var panelCanvas = document.getElementById("panelCanvasFirmaNotaEnf");
    var panelArchivo = document.getElementById("panelArchivoFirmaNotaEnf");
    if (tabCanvas) tabCanvas.classList.add("active");
    if (tabArchivo) tabArchivo.classList.remove("active");
    if (panelCanvas) panelCanvas.classList.remove("d-none");
    if (panelArchivo) panelArchivo.classList.add("d-none");

    showLoading();
    $.ajax({
      url: "/NotaEnfermeria/ObtenerFirmaEmpleado",
      method: "GET",
      success: function (res) {
        hideLoading();
        if (res.exitoso && res.firmaUrl) {
          self.firmaEnfermeriaExistenteUrl(res.firmaUrl);
          var testImg = new Image();
          testImg.onload = function () {
            self._mostrarPrevisualizacionFirmaNotaEnf(res.firmaUrl);
          };
          testImg.onerror = function () {
            self.firmaEnfermeriaExistenteUrl(null);
          };
          testImg.src = res.firmaUrl;
        }
        $("#mdl-firma-nota-enfermeria").modal("show");
      },
      error: function () {
        hideLoading();
        $("#mdl-firma-nota-enfermeria").modal("show");
      }
    });
  };

  self._mostrarPrevisualizacionFirmaNotaEnf = function (src) {
    var imgPrev = document.getElementById("imgPrevFirmaNotaEnf");
    var selector = document.getElementById("selectorFirmaNotaEnf");
    var contenedor = document.getElementById("contenedorPrevFirmaNotaEnf");
    var btnFirmar = document.getElementById("btnConfirmarFirmaNotaEnf");
    if (imgPrev) imgPrev.src = src;
    if (selector) selector.classList.add("d-none");
    if (contenedor) contenedor.classList.remove("d-none");
    if (btnFirmar) btnFirmar.removeAttribute("disabled");
  };

  self.resetearFirmaNotaEnf = function () {
    self.firmaEnfermeriaBase64(null);
    self.firmaEnfermeriaExistenteUrl(null);

    var imgPrev = document.getElementById("imgPrevFirmaNotaEnf");
    var selector = document.getElementById("selectorFirmaNotaEnf");
    var contenedor = document.getElementById("contenedorPrevFirmaNotaEnf");
    var btnFirmar = document.getElementById("btnConfirmarFirmaNotaEnf");
    if (imgPrev) { imgPrev.src = ""; imgPrev.style.display = "none"; }
    if (selector) selector.classList.remove("d-none");
    if (contenedor) contenedor.classList.add("d-none");
    if (btnFirmar) btnFirmar.setAttribute("disabled", "disabled");

    if (typeof window._switchFirmaTabNotaEnf === "function") {
      window._switchFirmaTabNotaEnf("canvas");
    } else {
      if (typeof window.limpiarCanvasFirmaNotaEnf === "function") window.limpiarCanvasFirmaNotaEnf();
      if (typeof window.initCanvasFirmaNotaEnf === "function") window.initCanvasFirmaNotaEnf();
    }
  };

  self.confirmarFirmaNotaEnfermeria = async function () {
    var id = self.firmaEnfermeriaId();
    var base64 = self.firmaEnfermeriaBase64();
    var existenteUrl = self.firmaEnfermeriaExistenteUrl();

    if (!base64 && !existenteUrl) {
      mensajeEmergenteError("Debe dibujar, subir o usar su firma antes de continuar.");
      return;
    }
    if (!confirm("¿Confirma la firma de esta nota de enfermería? Esta acción no se puede deshacer.")) return;

    // Solicitar huella
    showLoading();
    var beginRes = await fetch("/api/WebAuthnVerify/Begin?actionLabel=Firmar+Nota+Enfermeria", { method: "POST" });
    if (!beginRes.ok) {
      hideLoading();
      var errData = await beginRes.json().catch(() => ({}));
      mensajeEmergenteError(errData.message || "No se pudo iniciar la verificación de huella.");
      return;
    }
    var options = await beginRes.json();
    hideLoading();

    var assertion;
    try {
      assertion = await navigator.credentials.get({
        publicKey: {
          challenge: base64UrlToBuffer(options.challenge),
          timeout: options.timeout ?? 60000,
          rpId: options.rpId,
          userVerification: options.userVerification ?? "required",
          allowCredentials: [],
        },
      });
    } catch (e) {
      mensajeEmergenteError(e.name === "NotAllowedError" ? "Verificación cancelada." : "Error al leer huella.");
      return;
    }

    var huellaPayload = {
      id: assertion.id,
      rawId: bufferToBase64Url(assertion.rawId),
      type: assertion.type,
      response: {
        authenticatorData: bufferToBase64Url(assertion.response.authenticatorData),
        clientDataJSON: bufferToBase64Url(assertion.response.clientDataJSON),
        signature: bufferToBase64Url(assertion.response.signature),
        userHandle: assertion.response.userHandle ? bufferToBase64Url(assertion.response.userHandle) : null,
      },
    };

    showLoading();
    $.ajax({
      url: "/NotaEnfermeria/FirmarNotaEnfermeria",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        huellaPayload: huellaPayload,
        model: {
          NotaEnfermeriaId: id,
          FirmaBase64: base64 || null,
          FirmaExistenteUrl: existenteUrl || null,
        }
      }),
      success: function (dataResult) {
        hideLoading();
        if (dataResult.exitoso) {
          $("#mdl-firma-nota-enfermeria").modal("hide");
          // Refrescar las notas del turno actual
          self.consultarNotaEnfermeria();
          mensajeEmergente("Nota de enfermería firmada correctamente.");

          var notaActualizada = ko.utils.arrayFirst(self.notasEnfermerias(), function (item) {
            return item.Id === id;
          });
          if (notaActualizada) {
            notaActualizada.Firmado(true);
          }
        } else {
          mensajeEmergenteError(dataResult.resultado || "Error al firmar la nota.");
        }
      },
      error: function (xhr) {
        hideLoading();
        console.error(xhr);
        mensajeEmergenteError("Error de conexión al firmar la nota.");
      }
    });
  };

  self.abrirModalFirmaTurnoEnfermeria = function (turno) {
    self.firmaTurnoId(turno.Id);
    self.firmaTurnoBase64(null);
    self.firmaTurnoExistenteUrl(null);

    // Resetear UI del modal
    var selector = document.getElementById("selectorFirmaTurno");
    var previsualizacion = document.getElementById("contenedorPrevFirmaTurno");
    var btnFirmar = document.getElementById("btnConfirmarFirmaTurno");
    var imgPrev = document.getElementById("imgPrevFirmaTurno");
    if (selector) selector.classList.remove("d-none");
    if (previsualizacion) previsualizacion.classList.add("d-none");
    if (btnFirmar) btnFirmar.setAttribute("disabled", "disabled");
    if (imgPrev) imgPrev.src = "";

    // Activar pestaña canvas
    var tabCanvas = document.getElementById("tabCanvasFirmaTurno");
    var tabArchivo = document.getElementById("tabArchivoFirmaTurno");
    var panelCanvas = document.getElementById("panelCanvasFirmaTurno");
    var panelArchivo = document.getElementById("panelArchivoFirmaTurno");
    if (tabCanvas) tabCanvas.classList.add("active");
    if (tabArchivo) tabArchivo.classList.remove("active");
    if (panelCanvas) panelCanvas.classList.remove("d-none");
    if (panelArchivo) panelArchivo.classList.add("d-none");

    self._firmaTurnoModalEl = $("#mdl-firma-turno-enfermeria");

    showLoading();
    $.ajax({
      url: "/TurnoEnfermeria/ObtenerFirmaEmpleado",
      method: "GET",
      success: function (res) {
        hideLoading();
        if (res.exitoso && res.firmaUrl) {
          self.firmaTurnoExistenteUrl(res.firmaUrl);
          var testImg = new Image();
          testImg.onload = function () {
            self._mostrarPrevisualizacionFirmaTurno(res.firmaUrl);
          };
          testImg.onerror = function () {
            self.firmaTurnoExistenteUrl(null);
          };
          testImg.src = res.firmaUrl;
        }
        self._firmaTurnoModalEl.modal("show");
      },
      error: function () {
        hideLoading();
        self._firmaTurnoModalEl.modal("show");
      }
    });
  };

  self._mostrarPrevisualizacionFirmaTurno = function (src) {
    var imgPrev = document.getElementById("imgPrevFirmaTurno");
    var selector = document.getElementById("selectorFirmaTurno");
    var contenedor = document.getElementById("contenedorPrevFirmaTurno");
    var btnFirmar = document.getElementById("btnConfirmarFirmaTurno");

    if (imgPrev) imgPrev.src = src;
    if (selector) selector.classList.add("d-none");
    if (contenedor) contenedor.classList.remove("d-none");
    if (btnFirmar) btnFirmar.removeAttribute("disabled");
  };

  self.resetearFirmaTurno = function () {
    self.firmaTurnoBase64(null);
    self.firmaTurnoExistenteUrl(null);

    var imgPrev = document.getElementById("imgPrevFirmaTurno");
    var selector = document.getElementById("selectorFirmaTurno");
    var contenedor = document.getElementById("contenedorPrevFirmaTurno");
    var btnFirmar = document.getElementById("btnConfirmarFirmaTurno");

    if (imgPrev) { imgPrev.src = ""; imgPrev.style.display = "none"; }
    if (selector) selector.classList.remove("d-none");
    if (contenedor) contenedor.classList.add("d-none");
    if (btnFirmar) btnFirmar.setAttribute("disabled", "disabled");

    if (typeof window._switchFirmaTabTurno === "function") {
      window._switchFirmaTabTurno("canvas");
    } else {
      if (typeof window.limpiarCanvasFirmaTurno === "function") window.limpiarCanvasFirmaTurno();
      if (typeof window.initCanvasFirmaTurno === "function") window.initCanvasFirmaTurno();
    }
  };

  self.confirmarFirmaTurno = async function () {
    var id = self.firmaTurnoId();
    var base64 = self.firmaTurnoBase64();
    var existenteUrl = self.firmaTurnoExistenteUrl();

    if (!base64 && !existenteUrl) {
      mensajeEmergenteError("Debe dibujar, subir o usar su firma antes de continuar.");
      return;
    }

    if (!confirm("¿Confirma la firma de este turno de enfermería? Esta acción no se puede deshacer.")) return;

    // Solicitar huella
    showLoading();
    var beginRes = await fetch("/api/WebAuthnVerify/Begin?actionLabel=Firmar+Turno+Enfermeria", { method: "POST" });
    if (!beginRes.ok) {
      hideLoading();
      var errData = await beginRes.json().catch(() => ({}));
      mensajeEmergenteError(errData.message || "No se pudo iniciar la verificación de huella.");
      return;
    }
    var options = await beginRes.json();
    hideLoading();

    var assertion;
    try {
      assertion = await navigator.credentials.get({
        publicKey: {
          challenge: base64UrlToBuffer(options.challenge),
          timeout: options.timeout ?? 60000,
          rpId: options.rpId,
          userVerification: options.userVerification ?? "required",
          allowCredentials: [],
        },
      });
    } catch (e) {
      mensajeEmergenteError(e.name === "NotAllowedError" ? "Verificación cancelada." : "Error al leer huella.");
      return;
    }

    var huellaPayload = {
      id: assertion.id,
      rawId: bufferToBase64Url(assertion.rawId),
      type: assertion.type,
      response: {
        authenticatorData: bufferToBase64Url(assertion.response.authenticatorData),
        clientDataJSON: bufferToBase64Url(assertion.response.clientDataJSON),
        signature: bufferToBase64Url(assertion.response.signature),
        userHandle: assertion.response.userHandle ? bufferToBase64Url(assertion.response.userHandle) : null,
      },
    };

    showLoading();
    $.ajax({
      url: "/TurnoEnfermeria/FirmarTurnoEnfermeria",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        TurnoId: id,
        FirmaBase64: base64 || null,
        FirmaExistenteUrl: existenteUrl || null,
        HuellaPayload: huellaPayload,
      }),
      success: function (dataResult) {
        hideLoading();
        // Parsear si viene como string
        var res = typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        if (res.exitoso) {
          // Cerrar modal
          if (self._firmaTurnoModalEl) {
            self._firmaTurnoModalEl.modal("hide");
            self._firmaTurnoModalEl = null;
          }
          mensajeEmergente("Turno de enfermería firmado correctamente.");
          self.turnoFirmado(true);
          // Recargar desde el servidor para que la tabla refleje el cambio
          self.consultarTurnoEnfermeria();
        } else {
          mensajeEmergenteError(res.resultado || "Error al firmar el turno.");
        }
      },
      error: function (xhr) {
        hideLoading();
        console.error(xhr);
        mensajeEmergenteError("Error de conexión al firmar el turno.");
      }
    });
  };


  // Guardar los cambios de tarifa y días de un registro del historial de habitaciones
  self.guardarCambioTarifaHabitacion = function () {
    var item = self.cambioHabitacionEditando();   // registro que se está editando
    if (!item) {
      mensajeEmergenteError("No hay ningún registro seleccionado para editar.");
      return;
    }

    var id = item.Id;
    var nuevaTarifa = parseFloat(self.editTarifaCambio()) || 0;
    var nuevosDias = parseInt(self.editDiasCambio(), 10) || 1;

    if (!id) {
      mensajeEmergenteError("No se pudo identificar el registro.");
      return;
    }
    if (nuevaTarifa < 0) {
      mensajeEmergenteError("La tarifa no puede ser negativa.");
      return;
    }
    if (nuevosDias <= 0) {
      mensajeEmergenteError("El número de días debe ser mayor a cero.");
      return;
    }

    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ModificarTarifaCambioHabitacion",
      method: "POST",
      data: {
        cambioHabitacionId: id,
        nuevaTarifa: nuevaTarifa,
        nuevosDias: nuevosDias
      },
      success: function (dataResult) {
        hideLoading();
        var data = typeof dataResult === "string" ? JSON.parse(dataResult) : dataResult;
        if (data.Exitoso) {
          // Cerrar el modal
          $("#mdl-editar-cambio-habitacion").dialog("close");
          // Limpiar el registro que se estaba editando
          self.cambioHabitacionEditando(null);
          // Recargar el historial para reflejar los cambios
          self.consultarHistorialCambiosHabitacion();
          mensajeEmergente("Registro actualizado correctamente.");
        } else {
          mensajeEmergenteError(data.Mensaje || "No se pudo actualizar.");
        }
      },
      error: function (xhr) {
        hideLoading();
        console.error(xhr);
        mensajeEmergenteError("Error de conexión al modificar el registro.");
      }
    });
  };

  self.verPagosCuenta = function () {
    // Leer el estado de pago desde el campo oculto
    var pagada = $("#HospitalizacionPagada").val() === "true";

    // Construir la base de la URL
    var url = "/CuentasPorCobrar/Pagar?cuentaId=" + $("#CuentaId").val() +
      "&ResponsableNit=" + encodeURIComponent($("#ResponsableNit").val()) +
      "&ResponsableNombre=" + encodeURIComponent($("#ResponsableNombre").val()) +
      "&ResponsableDireccion=" + encodeURIComponent($("#ResponsableDireccion").val()) +
      "&ResponsableCorreo=" + encodeURIComponent($("#ResponsableCorreo").val()) +
      "&SeguroNombre=" + encodeURIComponent($("#SeguroNombre").val()) +
      "&PacienteNombreAdmision=" + encodeURIComponent($("#PacienteNombreAdmision").val()) +
      "&AdmisionId=" + $("#AdmisionId").val();

    // Agregar soloVista=true únicamente si la cuenta ya está pagada
    if (pagada) {
      url += "&soloVista=true";
    }

    window.location.href = url;
  };


  // self.pagarCuenta = function () {
  //   if (confirm("¿Desea realizar el pago de esta cuenta?")) {
  //     showLoading();
  //     window.location.href =
  //       "/CuentasPorCobrar/Pagar?cuentaId=" +
  //       $("#CuentaId").val() +
  //       "&ResponsableNit=" +
  //       $("#ResponsableNit").val() +
  //       "&ResponsableNombre=" +
  //       $("#ResponsableNombre").val() +
  //       "&ResponsableDireccion=" +
  //       $("#ResponsableDireccion").val() +
  //       "&ResponsableCorreo=" +
  //       $("#ResponsableCorreo").val() +
  //       "&SeguroNombre=" +
  //       $("#SeguroNombre").val() +
  //       "&PacienteNombreAdmision=" +
  //       $("#PacienteNombreAdmision").val() +
  //       "&AdmisionId=" +
  //       $("#AdmisionId").val();
  //   }
  // };
  self.pagarCuenta = function () {
    if (confirm("¿Desea realizar el pago de esta cuenta?")) {
      showLoading();

      // Creamos un único objeto con toda la información necesaria
      var requestData = {
        CuentaId: parseInt($("#CuentaId").val()),
        HospitalizacionId: parseInt($("#HospitalizacionId").val()),
        PacienteId: parseInt($("#PacienteId").val()),
        BodegaId: parseInt($("#BodegaId").val()),

        // Desglose de totales provenientes de los observables
        TotalesVistos: {
          Estadia: self.valorTotalRegistrosHospitalizacion(),
          Servicios: self.valorTotalServicios(),
          Paquetes: self.valorTotalPaquetes(),
          Medicamentos: self.valorTotalMedicamentos(),
          Examenes: self.valorTotalExamenes(),
          Dietas: self.valorTotalDietas(),
          Ambulancias: self.valorTotalAmbulancias(),
          Depositos: self.valorTotalDepositosHospitalizacion(),
          TotalFinal: self.totalPagar(),
        },
      };
      window.location.href =
        "/CuentasPorCobrar/Pagar?cuentaId=" +
        $("#CuentaId").val() +
        "&ResponsableNit=" +
        $("#ResponsableNit").val() +
        "&ResponsableNombre=" +
        $("#ResponsableNombre").val() +
        "&ResponsableDireccion=" +
        $("#ResponsableDireccion").val() +
        "&ResponsableCorreo=" +
        $("#ResponsableCorreo").val() +
        "&SeguroNombre=" +
        $("#SeguroNombre").val() +
        "&PacienteNombreAdmision=" +
        $("#PacienteNombreAdmision").val() +
        "&AdmisionId=" +
        $("#AdmisionId").val();

      // $.ajax({
      //   url: "/CuentasPorCobrar/ValidarTotalCuenta",
      //   method: "POST",
      //   contentType: "application/json", // Enviamos como JSON para que C# mapee el objeto
      //   data: JSON.stringify(requestData),
      //   success: function (response) {
      //     // Aquí irá la lógica de comparación
      //   },
      // });
    }
  };




  // self.actualizarTotales = function () {
  //   let totalPagar = 0;

  //   // Helper local: convierte "Q 1,250.00", "1.250,00", "1250,00", numbers, etc. a número
  //   const parseMoney = function (value) {
  //     // ✅ FIX CRÍTICO:
  //     // Si el valor viene como ko.observable, se desenvuelve primero.
  //     // Sin esto, Knockout pasa una función y el totalizador siempre da 0.
  //     if (typeof ko !== "undefined" && ko.utils && ko.utils.unwrapObservable) {
  //       value = ko.utils.unwrapObservable(value);
  //     }

  //     if (value === null || value === undefined) return 0;

  //     // Si ya es número válido
  //     if (typeof value === "number") {
  //       return isFinite(value) ? value : 0;
  //     }

  //     // Convertir a string
  //     let s = String(value).trim();
  //     if (!s) return 0;

  //     // Q / Q. / espacios
  //     s = s.replace(/Q\.?/gi, "").replace(/\s+/g, "");

  //     // dejar solo dígitos, coma, punto y signo negativo
  //     s = s.replace(/[^0-9,.\-]/g, "");
  //     if (!s) return 0;

  //     const lastComma = s.lastIndexOf(",");
  //     const lastDot = s.lastIndexOf(".");

  //     // Si tiene coma y punto: el último separador es decimal
  //     if (lastComma > -1 && lastDot > -1) {
  //       if (lastComma > lastDot) {
  //         // "1.234,56" → "1234.56"
  //         s = s.replace(/\./g, "").replace(/,/g, ".");
  //       } else {
  //         // "1,234.56" → "1234.56"
  //         s = s.replace(/,/g, "");
  //       }
  //     } else if (lastComma > -1) {
  //       // Solo coma: si parece decimal (1–2 dígitos), usar como decimal
  //       const parts = s.split(",");
  //       if (parts.length === 2 && parts[1].length > 0 && parts[1].length <= 2) {
  //         s = parts[0].replace(/,/g, "") + "." + parts[1];
  //       } else {
  //         // Coma como separador de miles
  //         s = s.replace(/,/g, "");
  //       }
  //     } else {
  //       // Solo punto o ninguno: si hay más de un punto, probablemente son miles
  //       const dotCount = (s.match(/\./g) || []).length;
  //       if (dotCount > 1) {
  //         s = s.replace(/\./g, "");
  //       }
  //     }

  //     const n = parseFloat(s);
  //     return isFinite(n) ? n : 0;
  //   };

  //   // ====== TRAZABILIDAD (persistida en KO) ======
  //   const trace = [];
  //   const formatQ = function (n) {
  //     try {
  //       return (
  //         "Q. " +
  //         (n || 0).toLocaleString("es-GT", {
  //           minimumFractionDigits: 2,
  //           maximumFractionDigits: 2,
  //         })
  //       );
  //     } catch (e) {
  //       return "Q. " + (Math.round((n || 0) * 100) / 100).toFixed(2);
  //     }
  //   };
  //   const addTrace = function (categoria, monto) {
  //     const n = typeof monto === "number" && isFinite(monto) ? monto : 0;
  //     trace.push({
  //       Orden: trace.length + 1,
  //       Categoria: categoria,
  //       Monto: n,
  //       MontoTexto: formatQ(n),
  //     });
  //     totalPagar += n;
  //   };

  //   // Hospitalización
  //   let valorTotalRegistrosHospitalizacion = 0;
  //   $(self.registrosHospitalizacion()).each(function (idx, vl) {
  //     valorTotalRegistrosHospitalizacion += parseMoney(vl && vl.Precio);
  //   });
  //   self.valorTotalRegistrosHospitalizacion(valorTotalRegistrosHospitalizacion);
  //   addTrace("Hospitalización", valorTotalRegistrosHospitalizacion);

  //   // Servicios
  //   let valorTotalServicios = 0;
  //   $(self.serviciosHospitalizacion()).each(function (idx, vl) {
  //     valorTotalServicios += parseMoney(vl && vl.Subtotal);
  //   });
  //   self.valorTotalServicios(valorTotalServicios);
  //   addTrace("Servicios", valorTotalServicios);

  //   // Paquetes (FIX REAL: soporta diferentes nombres de campo)
  //   // ====== 3. Paquetes (Código Modificado) ======
  //   let valorTotalPaquetes = 0;
  //   $(self.paquetesHospitalizacion()).each(function (idx, paquete) {
  //     if (!paquete) return;
  //     const precioRaw = (paquete.Precio != null ? paquete.Precio : null) ??
  //       (paquete.Valor != null ? paquete.Valor : null) ??
  //       (paquete.Subtotal != null ? paquete.Subtotal : null) ??
  //       (paquete.Monto != null ? paquete.Monto : null);
  //     valorTotalPaquetes += self.parseMoney(precioRaw); // Asegúrate de que parseMoney sea accesible
  //   });

  //   // --- CAMBIO AQUÍ: Forzar a número y a notificación ---
  //   valorTotalPaquetes = parseFloat(valorTotalPaquetes.toFixed(2));
  //   self.valorTotalPaquetes(valorTotalPaquetes);
  //   // Forzar la notificación a los suscriptores (útil si el valor es el mismo)
  //   self.valorTotalPaquetes.notifySubscribers(valorTotalPaquetes);
  //   // --- FIN DEL CAMBIO ---
  //   addTrace("Paquetes", valorTotalPaquetes);

  //   // Medicamentos
  //   let valorTotalMedicamentos = 0;
  //   $(self.medicamentosHospitalizacion()).each(function (idx, vl) {
  //     valorTotalMedicamentos += parseMoney(vl && vl.Subtotal);
  //   });
  //   self.valorTotalMedicamentos(valorTotalMedicamentos);
  //   addTrace("Medicamentos", valorTotalMedicamentos);

  //   // Exámenes
  //   let valorTotalExamenes = 0;
  //   $(self.examenesHospitalizacion()).each(function (idx, vl) {
  //     valorTotalExamenes += parseMoney(vl && vl.Precio);
  //   });

  //   self.valorTotalExamenes(valorTotalExamenes);

  //   self.totalExamenesComplementarios(valorTotalExamenes);
  //   self.totalExamenesComplementariosTexto(formatQ(valorTotalExamenes));

  //   addTrace("Exámenes", valorTotalExamenes);

  //   // Dietas
  //   let valorDietas = 0;

  //   // Trazabilidad (detalle por item)
  //   let dietasTrace = [];
  //   let dietasOmitidas = 0;

  //   $(self.listaRecetas()).each(function (idx, receta) {
  //     if (!receta) {
  //       dietasOmitidas++;
  //       return;
  //     }

  //     // Valores crudos (para auditoría)
  //     const rawCantidadAgregada =
  //       receta.Cantidad !== undefined && receta.Cantidad !== null
  //         ? receta.Cantidad
  //         : "";
  //     const rawPrecioUnitario =
  //       receta.PrecioVenta !== undefined && receta.PrecioVenta !== null
  //         ? receta.PrecioVenta
  //         : "";

  //     // Normalización numérica
  //     const cantidadAgregada = parseMoney(rawCantidadAgregada);
  //     const precioUnitario = parseMoney(rawPrecioUnitario);

  //     if (
  //       !cantidadAgregada ||
  //       cantidadAgregada <= 0 ||
  //       !precioUnitario ||
  //       precioUnitario <= 0
  //     ) {
  //       dietasOmitidas++;
  //       dietasTrace.push({
  //         Index: idx,
  //         HospitalizacionRecetaId: receta.Id,
  //         NombreReceta: receta.NombreReceta,
  //         CantidadAgregadaRaw: rawCantidadAgregada,
  //         CantidadAgregada: cantidadAgregada,
  //         PrecioUnitarioRaw: rawPrecioUnitario,
  //         PrecioUnitario: precioUnitario,
  //         TotalLinea: 0,
  //         Incluida: false,
  //       });
  //       return;
  //     }

  //     const totalLinea = cantidadAgregada * precioUnitario;
  //     valorDietas += totalLinea;

  //     dietasTrace.push({
  //       Index: idx,
  //       HospitalizacionRecetaId: receta.Id,
  //       NombreReceta: receta.NombreReceta,
  //       CantidadAgregadaRaw: rawCantidadAgregada,
  //       CantidadAgregada: cantidadAgregada,
  //       PrecioUnitarioRaw: rawPrecioUnitario,
  //       PrecioUnitario: precioUnitario,
  //       TotalLinea: parseFloat(totalLinea.toFixed(2)),
  //       Incluida: true,
  //     });
  //   });

  //   valorDietas = parseFloat(valorDietas.toFixed(2));
  //   self.valorTotalDietas(valorDietas);
  //   addTrace("Dietas", valorDietas);

  //   // Trazabilidad detallada (consola)
  //   try {
  //     console.groupCollapsed("TRACE Totales -> Dietas");
  //     console.table(dietasTrace);
  //     console.log(
  //       "Dietas: Total =",
  //       valorDietas,
  //       "| Items =",
  //       dietasTrace.length,
  //       "| Omitidas =",
  //       dietasOmitidas,
  //     );
  //     console.groupEnd();
  //   } catch (e) {
  //     console.log("TRACE Totales -> Dietas", {
  //       total: valorDietas,
  //       items: dietasTrace,
  //       omitidas: dietasOmitidas,
  //     });
  //   }

  //   // Ambulancias
  //   let valorTotalAmbulancias = 0;
  //   $(self.listaAmbulancias()).each(function (idx, ambulancia) {
  //     valorTotalAmbulancias += parseMoney(ambulancia && ambulancia.precio);
  //   });
  //   self.valorTotalAmbulancias(valorTotalAmbulancias);
  //   addTrace("Ambulancias", valorTotalAmbulancias);

  //   // Depósitos (resta)
  //   let valortotalDepositos = 0;
  //   $(self.depositosHospitalizacion()).each(function (idx, vl) {
  //     valortotalDepositos += parseMoney(vl && vl.Monto);
  //   });
  //   self.valorTotalDepositosHospitalizacion(valortotalDepositos);
  //   addTrace("Depósitos (resta)", -valortotalDepositos);

  //   // Total a pagar
  //   self.totalPagar(parseFloat(totalPagar.toFixed(2)));

  //   // Persistir trazabilidad para el modal (KO)
  //   self.detalleTotales(trace);
  // };
  self.actualizarTotales = function () {
    let totalPagar = 0;

    // ============================================================
    // FUNCIÓN HELPER: parseMoney (DEFINIDA DENTRO DE LA FUNCIÓN)
    // ============================================================
    const parseMoney = function (value) {
      // Si el valor viene como ko.observable, lo desempaquetamos
      if (typeof ko !== "undefined" && ko.utils && ko.utils.unwrapObservable) {
        value = ko.utils.unwrapObservable(value);
      }

      if (value === null || value === undefined) return 0;

      // Si ya es número válido
      if (typeof value === "number") {
        return isFinite(value) ? value : 0;
      }

      // Convertir a string
      let s = String(value).trim();
      if (!s) return 0;

      // Quitar Q / Q. y espacios
      s = s.replace(/Q\.?/gi, "").replace(/\s+/g, "");

      // Dejar solo dígitos, coma, punto y signo negativo
      s = s.replace(/[^0-9,.\-]/g, "");
      if (!s) return 0;

      const lastComma = s.lastIndexOf(",");
      const lastDot = s.lastIndexOf(".");

      // Si tiene coma y punto: el último separador es decimal
      if (lastComma > -1 && lastDot > -1) {
        if (lastComma > lastDot) {
          // "1.234,56" → "1234.56"
          s = s.replace(/\./g, "").replace(/,/g, ".");
        } else {
          // "1,234.56" → "1234.56"
          s = s.replace(/,/g, "");
        }
      } else if (lastComma > -1) {
        // Solo coma: si parece decimal (1–2 dígitos), usar como decimal
        const parts = s.split(",");
        if (parts.length === 2 && parts[1].length > 0 && parts[1].length <= 2) {
          s = parts[0].replace(/,/g, "") + "." + parts[1];
        } else {
          // Coma como separador de miles
          s = s.replace(/,/g, "");
        }
      } else {
        // Solo punto o ninguno: si hay más de un punto, probablemente son miles
        const dotCount = (s.match(/\./g) || []).length;
        if (dotCount > 1) {
          s = s.replace(/\./g, "");
        }
      }

      const n = parseFloat(s);
      return isFinite(n) ? n : 0;
    };

    // ============================================================
    // FUNCIONES HELPER: formatQ y addTrace
    // ============================================================
    const formatQ = function (n) {
      try {
        return (
          "Q. " +
          (n || 0).toLocaleString("es-GT", {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
          })
        );
      } catch (e) {
        return "Q. " + (Math.round((n || 0) * 100) / 100).toFixed(2);
      }
    };

    const trace = [];
    const addTrace = function (categoria, monto) {
      const n = typeof monto === "number" && isFinite(monto) ? monto : 0;
      trace.push({
        Orden: trace.length + 1,
        Categoria: categoria,
        Monto: n,
        MontoTexto: formatQ(n),
      });
      totalPagar += n;
    };

    // ============================================================
    // 1. HOSPITALIZACIÓN (registro actual + historial de cambios)
    // ============================================================
    let valorTotalRegistrosHospitalizacion = 0;
    $(self.registrosHospitalizacion()).each(function (idx, vl) {
      valorTotalRegistrosHospitalizacion += parseMoney(vl && vl.Precio);
    });
    // Sumar historial de cambios de habitación al total de estadía
    $(self.historialCambiosHabitacion()).each(function (idx, it) {
      valorTotalRegistrosHospitalizacion += parseMoney(it && it.ValorTotal);
    });
    self.valorTotalRegistrosHospitalizacion(valorTotalRegistrosHospitalizacion);
    addTrace("Hospitalización", valorTotalRegistrosHospitalizacion);

    // ============================================================
    // 2. SERVICIOS
    // ============================================================
    let valorTotalServicios = 0;
    $(self.serviciosHospitalizacion()).each(function (idx, vl) {
      valorTotalServicios += parseMoney(vl && vl.Subtotal);
    });
    self.valorTotalServicios(valorTotalServicios);
    addTrace("Servicios", valorTotalServicios);

    // ============================================================
    // 3. PAQUETES (VERSIÓN SUPER FORZADA)
    // ============================================================
    let valorTotalPaquetes = 0;
    console.log("🔍 Calculando total de paquetes...");

    // Mostrar todos los paquetes para depuración
    console.log("📋 Lista de paquetes:", self.paquetesHospitalizacion());

    $(self.paquetesHospitalizacion()).each(function (idx, paquete) {
      if (!paquete) return;

      console.log(`📦 Paquete ${idx}:`, paquete);

      // Intentar obtener el precio de diferentes maneras
      let precioPaquete = 0;

      if (paquete.Precio !== undefined && paquete.Precio !== null) {
        precioPaquete = paquete.Precio;
      } else if (paquete.Valor !== undefined && paquete.Valor !== null) {
        precioPaquete = paquete.Valor;
      } else if (paquete.Subtotal !== undefined && paquete.Subtotal !== null) {
        precioPaquete = paquete.Subtotal;
      }

      console.log(`💰 Precio encontrado: ${precioPaquete}`);
      valorTotalPaquetes += parseMoney(precioPaquete);
    });

    console.log("💰 Total paquetes calculado:", valorTotalPaquetes);

    // Forzar a número con 2 decimales
    valorTotalPaquetes = parseFloat(valorTotalPaquetes.toFixed(2));
    console.log("💰 Total formateado:", valorTotalPaquetes);

    // 🔴 FORZAR ACTUALIZACIÓN MÚLTIPLE
    self.valorTotalPaquetes(valorTotalPaquetes);
    self.valorTotalPaquetes.valueHasMutated(); // Forzar notificación de cambio
    self.valorTotalPaquetes.notifySubscribers(valorTotalPaquetes); // Forzar notificación a suscriptores

    // 🔴 FORZAR ACTUALIZACIÓN DE LA TABLA COMPLETA
    self.paquetesHospitalizacion.valueHasMutated();

    console.log("✅ Total actualizado en observable:", self.valorTotalPaquetes());

    addTrace("Paquetes", valorTotalPaquetes);

    // ============================================================
    // 4. MEDICAMENTOS
    // ============================================================
    // let valorTotalMedicamentos = 0;
    // $(self.medicamentosHospitalizacion()).each(function (idx, vl) {
    //   valorTotalMedicamentos += parseMoney(vl && vl.Subtotal);
    // });
    // self.valorTotalMedicamentos(valorTotalMedicamentos);
    // addTrace("Medicamentos", valorTotalMedicamentos);
    let valorTotalMedicamentos = 0;
    $(self.medicamentosHospitalizacion()).each(function (idx, vl) {
      valorTotalMedicamentos += parseMoney(vl && vl.Subtotal);
    });

    valorTotalMedicamentos += parseFloat((self._acumuladoInsumosDirectos || 0).toFixed(2));
    self.valorTotalMedicamentos(valorTotalMedicamentos);
    addTrace("Medicamentos", valorTotalMedicamentos);

    // ============================================================
    // 5. EXÁMENES
    // ============================================================
    let valorTotalExamenes = 0;
    $(self.examenesHospitalizacion()).each(function (idx, vl) {
      valorTotalExamenes += parseMoney(vl && vl.Precio);
    });

    self.valorTotalExamenes(valorTotalExamenes);
    self.totalExamenesComplementarios(valorTotalExamenes);
    self.totalExamenesComplementariosTexto(formatQ(valorTotalExamenes));
    addTrace("Exámenes", valorTotalExamenes);

    // ============================================================
    // 6. DIETAS
    // ============================================================
    let valorDietas = 0;
    $(self.listaRecetas()).each(function (idx, receta) {
      if (!receta) return;

      const cantidadAgregada = parseMoney(receta.Cantidad);
      const precioUnitario = parseMoney(receta.PrecioVenta);

      if (!cantidadAgregada || cantidadAgregada <= 0 || !precioUnitario || precioUnitario <= 0) {
        return;
      }

      valorDietas += cantidadAgregada * precioUnitario;
    });

    valorDietas = parseFloat(valorDietas.toFixed(2));
    self.valorTotalDietas(valorDietas);
    addTrace("Dietas", valorDietas);

    // ============================================================
    // 7. AMBULANCIAS
    // ============================================================
    let valorTotalAmbulancias = 0;
    $(self.listaAmbulancias()).each(function (idx, ambulancia) {
      valorTotalAmbulancias += parseMoney(ambulancia && ambulancia.precio);
    });
    self.valorTotalAmbulancias(valorTotalAmbulancias);
    addTrace("Ambulancias", valorTotalAmbulancias);


    // ============================================================
    // 8. EMERGENCIAS
    // ============================================================
    let valorTotalEmergencias = 0;
    console.log("Calculando total de emergencias...");

    $(self.productosEmergenciaHospi()).each(function (idx, producto) {
      if (!producto.Eliminado()) {
        console.log(`Producto ${idx}:`, producto);
        valorTotalEmergencias += parseMoney(producto.ValorTotal());
      }
    });

    $(self.serviciosEmergenciaHospi()).each(function (idx, servicio) {
      if (!servicio.Eliminado()) {
        console.log(`Servicio ${idx}:`, servicio);
        valorTotalEmergencias += parseMoney(servicio.ValorTotal());
      }
    });

    $(self.examenesEmergenciaHospi()).each(function (idx, examen) {
      if (!examen.Eliminado()) {
        console.log(`Examen ${idx}:`, examen);
        valorTotalEmergencias += parseMoney(examen.ValorTotal());
      }
    });

    console.log("Total emergencias calculado:", valorTotalEmergencias);
    self.valorTotalEmergencias(valorTotalEmergencias);
    self.valorTotalEmergenciasTexto("Q. " + valorTotalEmergencias.toFixed(2));
    self.valorTotalEmergencias.notifySubscribers(valorTotalEmergencias);
    addTrace("Emergencias", valorTotalEmergencias);


    // ============================================================
    // 9. DEPÓSITOS (RESTA)
    // ============================================================
    let valortotalDepositos = 0;
    $(self.depositosHospitalizacion()).each(function (idx, vl) {
      valortotalDepositos += parseMoney(vl && vl.Monto);
    });
    self.valorTotalDepositosHospitalizacion(valortotalDepositos);
    addTrace("Depósitos (resta)", -valortotalDepositos);

    // ============================================================
    // TOTAL A PAGAR
    // ============================================================
    self.totalPagar(parseFloat(totalPagar.toFixed(2)));

    // Persistir trazabilidad para el modal
    self.detalleTotales(trace);
  };
  self.imprimirNotasMedicasAll = function (hospitalizacionId) {
    const url = `/CrearPDF/HospitalizacionNotaMedicaPDFGetAll?hospitalizacionId=${hospitalizacionId}`;
    window.open(url, "_blank");
  };
  self.imprimirNotasOperatoriasAll = function (hospitalizacionId) {
    const url = `/CrearPDF/HospitalizacionNotaOperatoriaPDFGetAll?hospitalizacionId=${hospitalizacionId}`;
    window.open(url, "_blank");
  };
  self.verInformeDetallado = function () {
    window.open(
      "/CrearPDF/HospitalizacionInformeDetalladoPDF?hospitalizacionId=" +
      $("#HospitalizacionId").val(),
      "_blank",
    );
  };
  self.verExpedientePaciente = function () {
    window.open(
      "/CrearPDF/ExpedientePacientePDF?hospitalizacionId=" +
      $("#HospitalizacionId").val(),
      "_blank",
    );
  };
  self.verInformeGeneral = function () {
    window.open(
      "/CrearPDF/HospitalizacionInformeGeneralPDF?hospitalizacionId=" +
      $("#HospitalizacionId").val(),
      "_blank",
    );
  };
  self.getModelReceta = function () {
    modelReceta = {
      //Receta
      Id: self.recetaIdAgregar(),
      NombreReceta: self.nombreRecetaAgregar(),
      Ingredientes: self.ingredientesRecetaAgregar(),
      Indicaciones: self.indicacionesRecetaAgregar(),
      Cantidad: self.cantidadRecetaAgregar(),
      HospitalizacionId: $("#HospitalizacionId").val(),
    };
  };
  self.consultarHistorialCambiosHabitacion = function () {
    showLoading();

    self.historialCambiosHabitacion([]); // limpiar antes de cargar
    self.valorTotalRegistrosHospitalizacionHistorial(0);

    $.ajax({
      url: "/Hospitalizacion/HospitalizacionHistorialHabitacion",
      method: "POST",
      data: {
        hospitalizacionId: $("#HospitalizacionId").val(),
      },
      success: function (dataResult) {
        hideLoading();

        let data = JSON.parse(dataResult);
        if (!data || data.Exitoso !== true) {
          // Si falla, dejamos el historial vacío y total 0 (vista mostrará el ifnot)
          return;
        }

        // Normalizar lista
        let items = Array.isArray(data.Resultado) ? data.Resultado : [];

        // Formatear FechaCambio si Moment existe (en tu vista ya se usa moment)
        items = items.map(function (it) {
          // Si FechaCambio ya viene string ISO, moment lo formatea bien
          let fechaTexto = it.FechaCambio;

          try {
            if (typeof moment !== "undefined" && it.FechaCambio) {
              fechaTexto = moment(it.FechaCambio).format("DD/MM/YYYY hh:mm A");
            }
          } catch (e) {
            // Si algo falla, dejamos el valor original
          }

          return {
            Id: it.Id,
            HabitacionId: it.HabitacionId,
            Habitacion: it.Habitacion,
            Categoria: it.Categoria,
            FechaCambio: it.FechaCambio,
            FechaCambioTexto: fechaTexto,
            Dias: it.Dias,
            Tarifa: it.Tarifa,
            ValorTarifa: it.ValorTarifa,
            ValorTotal: it.ValorTotal,
          };
        });

        self.historialCambiosHabitacion(items);

        // Total (suma de ValorTotal)
        let total = 0;
        items.forEach(function (it) {
          total += parseFloat(it.ValorTotal || 0);
        });

        self.valorTotalRegistrosHospitalizacionHistorial(
          parseFloat(total.toFixed(2)),
        );
        // Recalcular estado de cuenta con el historial ya actualizado
        self.actualizarTotales();
      },
      error: function (err) {
        hideLoading();
        // Mantener consistencia: historial vacío
        self.historialCambiosHabitacion([]);
        self.valorTotalRegistrosHospitalizacionHistorial(0);
        console.log(err);
      },
    });
  };





  // ── Eliminar registro del historial de cambios de habitación ────────────
  self.eliminarCambioHabitacion = function (item) {
    if (!confirm("¿Desea eliminar este registro del historial de habitación?")) return;

    var id = item.Id;
    if (!id) {
      alert("No se pudo identificar el registro a eliminar.");
      return;
    }

    showLoading();
    $.ajax({
      url: "/Hospitalizacion/EliminarCambioHabitacion",
      method: "POST",
      data: { cambioHabitacionId: id },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          // Recargar historial completo desde el servidor para reflejar cambios reales
          self.consultarHistorialCambiosHabitacion();
        } else {
          alert("Error: " + (data.Mensaje || "No se pudo eliminar."));
        }
      },
      error: function () {
        hideLoading();
        alert("Error de conexión al eliminar el registro.");
      }
    });
  };




  self.abrirModalEditarCambioHabitacion = function (cambio) {
    // Guardar el cambio que se está editando (observable)
    self.cambioHabitacionEditando(cambio);

    // Cargar valores actuales en los campos
    self.editDiasCambio(cambio.Dias);
    self.editTarifaCambio(cambio.ValorTarifa);
    self.tarifasDisponibles([]);

    var habitacionId = cambio.HabitacionId;
    if (habitacionId) {
      $.ajax({
        url: '/Hospitalizacion/ObtenerTarifasPorHabitacion',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(habitacionId),
        success: function (response) {
          var data = JSON.parse(response);
          if (data.Exitoso) {
            var items = data.Resultado.map(function (t) {
              return {
                id: t.Id,
                valor: t.ValorTarifa,
                textoMostrar: t.NombreTarifa + ' - Q ' + t.ValorTarifa.toFixed(2)
              };
            });
            self.tarifasDisponibles(items);
          }
        },
        error: function (err) {
          console.error("Error cargando tarifas:", err);
        }
      });
    }

    // Abrir el diálogo (ya inicializado)
    $("#mdl-editar-cambio-habitacion").dialog("open");
  };

  self.abrirModalEditarCambioHabitacion = function (cambio) {
    // Guardar el cambio que se está editando (observable)
    self.cambioHabitacionEditando(cambio);

    // Cargar valores actuales en los campos
    self.editDiasCambio(cambio.Dias);
    self.editTarifaCambio(cambio.ValorTarifa);
    self.tarifasDisponibles([]);

    var habitacionId = cambio.HabitacionId;
    if (habitacionId) {
      $.ajax({
        url: '/Hospitalizacion/ObtenerTarifasPorHabitacion',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(habitacionId),
        success: function (response) {
          var data = JSON.parse(response);
          if (data.Exitoso) {
            var items = data.Resultado.map(function (t) {
              return {
                id: t.Id,
                valor: t.ValorTarifa,
                textoMostrar: t.NombreTarifa + ' - Q ' + t.ValorTarifa.toFixed(2)
              };
            });
            self.tarifasDisponibles(items);
          }
        },
        error: function (err) {
          console.error("Error cargando tarifas:", err);
        }
      });
    }

    // Abrir el diálogo (ya inicializado)
    $("#mdl-editar-cambio-habitacion").dialog("open");
  };


  self.abrirModalEditarTarifaEstancia = function () {
    var habitacionId = self.habitacionId();
    if (!habitacionId) {
      console.error("No se encontró habitaciónId");
      mensajeEmergenteError("No se pudo identificar la habitación actual.");
      return;
    }

    self.tarifasEstancia([]);
    self.nuevaTarifaEstanciaId(null);

    $.ajax({
      url: '/Hospitalizacion/ObtenerTarifasPorHabitacion',
      type: 'POST',
      contentType: 'application/json',
      data: JSON.stringify(habitacionId),
      success: function (response) {
        var data = typeof response === 'string' ? JSON.parse(response) : response;
        if (data.Exitoso) {
          var items = data.Resultado.map(function (t) {
            return {
              id: t.Id,
              nombre: t.NombreTarifa,
              valor: t.ValorTarifa,
              textoMostrar: t.NombreTarifa + ' - Q ' + t.ValorTarifa.toFixed(2)
            };
          });
          self.tarifasEstancia(items);

          // Asegurar que el diálogo esté inicializado antes de abrirlo
          if (!$("#mdl-editar-tarifa-estancia").hasClass("ui-dialog-content")) {
            $("#mdl-editar-tarifa-estancia").dialog({
              autoOpen: false,
              modal: true,
              width: 550,
              title: "Cambiar tarifa de hospitalización actual",
              buttons: {
                "Actualizar": function () {
                  detallesVm.guardarCambioTarifaEstancia();
                  $(this).dialog("close");
                },
                "Cancelar": function () {
                  $(this).dialog("close");
                }
              }
            });
          }
          $("#mdl-editar-tarifa-estancia").dialog("open");
        } else {
          alert("Error al cargar tarifas: " + data.Mensaje);
        }
      },
      error: function (err) {
        console.error("Error cargando tarifas:", err);
        alert("Error de conexión al cargar tarifas");
      }
    });
  };

  self.cargarProductosNoControlados = function (callback) {
    showLoading();
    $.ajax({
      url: "/MedicamentoNoControlado/ConsultarProductosNoControlados",
      method: "POST",
      success: function (dataResult) {
        hideLoading();
        var data = dataResult;
        console.log("Respuesta completa:", data);
        if (data.exitoso) {
          console.log("Resultado (array):", data.resultado);
          self.listaProductosNoControlados(data.resultado);
          if (callback) callback();
        } else {
          mensajeEmergenteError(data.mensaje);
          if (callback) callback();
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error al cargar productos no controlados.");
        if (callback) callback();
      }
    });
  };
  self.abrirModalAgregarMedNoControlado = function () {
    self.productoSeleccionadoNoControladoId(null);
    if (self.listaProductosNoControlados().length > 0) {
      $("#modalAgregarMedNoControlado").dialog("open");
    } else {
      self.cargarProductosNoControlados(function () {
        $("#modalAgregarMedNoControlado").dialog("open");
      });
    }
  };

  self.agregarMedicamentoNoControlado = function () {
    var idSeleccionado = self.productoSeleccionadoNoControladoId();
    console.log("ID seleccionado:", idSeleccionado);
    if (!idSeleccionado) {
      mensajeEmergenteError("Seleccione un medicamento.");
      return;
    }

    var producto = self.listaProductosNoControlados().find(function (p) {
      return p.productoId === idSeleccionado;
    });

    if (!producto) {
      mensajeEmergenteError("Producto no encontrado en la lista.");
      return;
    }

    var yaExiste = ko.utils.arrayFirst(self.medicamentosNoControlados(), function (item) {
      return item.productoId === producto.productoId;
    });

    if (yaExiste) {
      mensajeEmergenteError("Este medicamento ya está agregado.");
      return;
    }

    var nuevo = {
      productoId: producto.productoId,
      nombre: producto.productoNombre,
      unidadesIniciales: ko.observable(0),
      unidadesExtra: ko.observable(0),
      utilizado: ko.observable(0),
      descartado: ko.observable(0),
      retornadas: ko.observable(0)
    };

    self.medicamentosNoControlados.push(nuevo);
    $("#modalAgregarMedNoControlado").dialog("close");
  };

  self.eliminarMedicamentoNoControlado = function (item) {
    if (confirm("¿Desea eliminar este medicamento de la lista?")) {
      self.medicamentosNoControlados.remove(item);
    }
  };


  self.guardarCambioTarifaEstancia = function () {
    var nuevaTarifaId = self.nuevaTarifaEstanciaId();
    if (!nuevaTarifaId) {
      alert("Debe seleccionar una tarifa.");
      return;
    }

    var hospitalizacionId = $("#HospitalizacionId").val();
    if (!hospitalizacionId) {
      alert("No se encontró ID de hospitalización.");
      return;
    }

    showLoading();
    $.ajax({
      url: "/Hospitalizacion/ActualizarTarifaHospitalizacion",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({
        hospitalizacionId: parseInt(hospitalizacionId, 10),
        nuevaTarifaId: nuevaTarifaId
      }),
      success: function (response) {
        hideLoading();
        var data = typeof response === 'string' ? JSON.parse(response) : response;
        if (data.Exitoso) {
          // Recargar datos de hospitalización para actualizar la tabla
          self.consultarRegistrosHospitalizacion();
          mensajeEmergente("Tarifa actualizada correctamente.");
        } else {
          alert("Error: " + data.Mensaje);
        }
      },
      error: function (err) {
        hideLoading();
        console.error("Error al actualizar tarifa:", err);
        alert("Error de conexión.");
      }
    });
  };

  self.guardarMedicamentosNoControlados = function () {


    var dataToSave = {
      HospitalizacionId: parseInt($("#HospitalizacionId").val()),
      FechaProcedimiento: self.fechaProcedimientoNoControlados(),
      Registros: ko.toJS(self.medicamentosNoControlados()).map(function (r) {
        return {
          ProductoId: r.productoId,
          ProductoNombre: r.nombre,
          UnidadesIniciales: r.unidadesIniciales,
          UnidadesExtra: r.unidadesExtra,
          Utilizado: r.utilizado,
          Descartado: r.descartado,
          Retornadas: r.retornadas
        };
      })
    };

    showLoading();
    $.ajax({
      url: "/MedicamentoNoControlado/Guardar",
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(dataToSave),
      success: function (dataResult) {
        hideLoading();
        var data = dataResult;

        if (data.exitoso) {
          mensajeEmergente("Registro guardado correctamente.");
          self.cargarHistorialMedicamentosNoControlados();
        } else {
          mensajeEmergenteError(data.resultado || "Error al guardar.");
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de comunicación al guardar.");
      }
    });
  };


  self.subirDocumentos = function (files, progressBar, progressText, progressContainer) {
    var hospitalizacionId = $("#HospitalizacionId").val();
    if (!hospitalizacionId) {
      mensajeEmergenteError("No se encontró ID de hospitalización.");
      if (progressContainer) progressContainer.style.display = 'none';
      return;
    }

    var formData = new FormData();
    for (var i = 0; i < files.length; i++) {
      formData.append("archivos", files[i]);
    }
    formData.append("hospitalizacionId", hospitalizacionId);

    var progressInterval = setInterval(function () {
      var currentWidth = parseInt(progressBar.style.width) || 0;
      if (currentWidth < 90) {
        var newWidth = currentWidth + 10;
        progressBar.style.width = newWidth + '%';
        progressText.textContent = `Subiendo archivos... ${newWidth}%`;
      }
    }, 300);

    $.ajax({
      url: '/Hospitalizacion/SubirDocumento',
      type: 'POST',
      data: formData,
      processData: false,
      contentType: false,
      success: function (response) {
        clearInterval(progressInterval);
        progressBar.style.width = '100%';
        progressText.textContent = 'Completado. Actualizando lista...';
        setTimeout(function () {
          if (progressContainer) progressContainer.style.display = 'none';
          if (response.exitoso) {
            mensajeEmergente(response.mensaje, "success");
            // Limpiar input file
            $("#archivosDocumento").val('');
            $("#archivosSeleccionados").text('Ningún archivo seleccionado');
            // Recargar la tabla de documentos
            self.cargarListaDocumentos();
          } else {
            mensajeEmergenteError(response.mensaje || "Error al subir.");
          }
        }, 500);
      },
      error: function (xhr, status, error) {
        clearInterval(progressInterval);
        if (progressContainer) progressContainer.style.display = 'none';
        console.error("Error en subida:", error);
        mensajeEmergenteError("Error de conexión al subir archivos.");
      }
    });
  };


  self.cargarListaDocumentos = function () {
    var hospitalizacionId = $("#HospitalizacionId").val();
    if (!hospitalizacionId) return;

    showLoading();
    $.ajax({
      url: '/Hospitalizacion/ListarDocumentos',
      type: 'GET',
      data: { hospitalizacionId: hospitalizacionId },
      success: function (response) {
        hideLoading();
        if (response.exitoso) {
          var docs = response.documentos.map(function (doc) {
            return {
              NombreArchivo: doc.nombreArchivo,
              FechaSubida: doc.fechaSubida,
              TamanoFormateado: doc.tamanfoFormatado,
              UrlDescarga: doc.urlDescarga,
              Id: doc.id,
              Autorizado: doc.autorizado
            };
          });
          self.documentos(docs);
        } else {
          mensajeEmergenteError("Error al cargar documentos: " + response.mensaje);
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error de conexión al cargar documentos.");
      }
    });
  };
  self.cargarHistorialMedicamentosNoControlados = function () {
    var hospitalizacionId = $("#HospitalizacionId").val();
    if (!hospitalizacionId) return;
    showLoading();
    $.ajax({
      url: "/MedicamentoNoControlado/ObtenerHistorial",
      method: "POST",
      data: { hospitalizacionId: hospitalizacionId },
      success: function (dataResult) {
        hideLoading();
        var data = dataResult;

        if (data.exitoso) {
          self.registrosPreviosNoControlados(data.resultado);
        } else {
          console.warn("Error al cargar historial:", data.mensaje);
        }
      },
      error: function () {
        hideLoading();
        console.error("Error al cargar historial.");
      }
    });
  };

  self.verHistorialNoControlados = function () {
    self.cargarHistorialMedicamentosNoControlados();
    $("#modalHistorialMedNoControlados").dialog("open");
  };


  self.eliminarDocumento = function (documento) {
    if (!confirm("¿Eliminar este documento?")) return;
    showLoading();
    $.ajax({
      url: '/Hospitalizacion/EliminarDocumento',
      type: 'POST',
      data: { id: documento.Id },
      success: function (response) {
        hideLoading();
        if (response.exitoso) {
          self.documentos.remove(documento);
          mensajeEmergente("Documento eliminado.");
        } else {
          mensajeEmergenteError(response.mensaje);
        }
      },
      error: function () {
        hideLoading();
        mensajeEmergenteError("Error al eliminar.");
      }
    });
  };
};

var detallesVm = new HospitalizarDetallesVM();
ko.applyBindings(detallesVm);


function base64UrlToBuffer(b64) {
  var std = b64.replace(/-/g, "+").replace(/_/g, "/");
  var padded = std.padEnd(std.length + (4 - std.length % 4) % 4, "=");
  var binary = atob(padded);
  var buf = new Uint8Array(binary.length);
  for (var i = 0; i < binary.length; i++) buf[i] = binary.charCodeAt(i);
  return buf.buffer;
}

function bufferToBase64Url(buf) {
  var bytes = new Uint8Array(buf);
  var bin = "";
  for (var i = 0; i < bytes.length; i++) bin += String.fromCharCode(bytes[i]);
  return btoa(bin).replace(/\+/g, "-").replace(/\//g, "_").replace(/=/g, "");
}




$(document).ready(function () {

  var idInput = $("#HospitalizacionId").val();
  if (idInput) {
    detallesVm.hospitalizacionId(parseInt(idInput));
  }
  // ── LOTE 1: Lo que el usuario ve inmediatamente ────────────────────────────
  // actualizarTotales es local (sin AJAX), va siempre primero.
  detallesVm.actualizarTotales();

  // Datos del tab principal que carga al abrir la página
  detallesVm.consultarRegistrosHospitalizacion();
  detallesVm.consultarMedicamentosHospitalizacion();
  detallesVm.consultarExamenesHospitalizacion();

  // ── LOTE 2: Catálogos de selects (necesarios antes de abrir modales) ───────
  setTimeout(function () {
    detallesVm.consultarExamenesExistentes();
    detallesVm.consultarMedicamentosExistentes();
    detallesVm.consultarServiciosExistentes();
    detallesVm.consultarRecetasExistentes();
  }, 400);

  // ── LOTE 3: Registros adicionales de la hospitalización ───────────────────
  setTimeout(function () {
    detallesVm.consultarServiciosHospitalizacion();
    detallesVm.consultarPaquetesHospitalizacion();
    detallesVm.consultarPaquetesExistentes();
    detallesVm.consultarDepositosHospitalizacion();
    detallesVm.consultarReceta();
    detallesVm.consultarInsumosDirectosAplicados();
  }, 900);

  // ── LOTE 4: Datos de baja prioridad / historial ────────────────────────────
  setTimeout(function () {
    detallesVm.consultarUsuariosAcceso();
    detallesVm.consultarUsuarios();
    detallesVm.consultarAmbulancias();
    detallesVm.consultarDatosExamenFisico();
    detallesVm.consultarHabitacionesDisponibles();
    detallesVm.consultarPaquetesAplicados();
    detallesVm.consultarTurnoEnfermeria();
    detallesVm.consultarNotaEnfermeria();
    detallesVm.consultarNotaEvolucion();
    detallesVm.consultarHistorialCambiosHabitacion();
    detallesVm.consultarKitsIngreso();

    if (detallesVm.hospitalizacionId() > 0) {
      detallesVm.consultarIdEmergencia();
    }
  }, 1500);


  $('a[href="#tab-content-carga-documentos"]').on('shown.bs.tab', function () {
    if (detallesVm && typeof detallesVm.cargarListaDocumentos === 'function') {
      detallesVm.cargarListaDocumentos();
    }
  });

  if ($('#tab-content-carga-documentos').is(':visible')) {
    detallesVm.cargarListaDocumentos();
  }


  var fechaEntrada = moment().startOf("day");
  var fechaSalida = moment().endOf("day");

  $("#periodo-hospitalizacion").daterangepicker(
    {
      startDate: fechaEntrada,
      endDate: fechaSalida,
      timePicker: true,
      locale: {
        applyLabel: "Aplicar",
        cancelLabel: "Cancelar",
        format: "DD/MM/YYYY hh:mm A",
        daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Setiembre", "Octubre", "Noviembre", "Diciembre"],
      },
    },
    function (start, end) {
      var diffday = parseInt((end - start) / (1000 * 60 * 60 * 24));
      rangoDias = diffday > 0 ? diffday + 1 : 1;
    }
  );




  if (document.getElementById('editor')) {
    quillEditor = new Quill('#editor', {
      theme: 'snow',
      placeholder: 'Escriba la nota de enfermería aquí...',
      modules: {
        toolbar: [
          ['bold', 'italic', 'underline', 'strike'],
          ['blockquote', 'code-block'],
          [{ 'list': 'ordered' }, { 'list': 'bullet' }],
          [{ 'script': 'sub' }, { 'script': 'super' }],
          [{ 'indent': '-1' }, { 'indent': '+1' }],
          [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
          [{ 'color': [] }, { 'background': [] }],
          ['clean']
        ]
      }
    });

    // Sincronizar cambios del editor con el observable
    quillEditor.on('text-change', function () {
      if (detallesVm) {
        detallesVm.notaDiagnostico(quillEditor.root.innerHTML);
      }
    });

    detallesVm.quillEditor = quillEditor;

  }

  if (document.getElementById('editor-nota-medica')) {
    var quillMedica = new Quill('#editor-nota-medica', {
      theme: 'snow',
      placeholder: 'Escriba la nota de evolución médica aquí...',
      modules: {
        toolbar: [
          ['bold', 'italic', 'underline', 'strike'],
          ['blockquote'],
          [{ 'list': 'ordered' }, { 'list': 'bullet' }],
          [{ 'indent': '-1' }, { 'indent': '+1' }],
          [{ 'header': [1, 2, 3, false] }],
          [{ 'color': [] }],
          ['clean']
        ]
      }
    });

    quillMedica.on('text-change', function () {
      if (detallesVm) {
        detallesVm.notaDiagnostico(quillMedica.root.innerHTML);
      }
    });

    detallesVm.quillMedica = quillMedica;
  }




  $("#mdl-editar-cambio-habitacion").dialog({
    autoOpen: false,
    modal: true,
    width: 500,
    title: "Modificar tarifa / días",
    buttons: {
      "Guardar": function () {
        if (typeof detallesVm.guardarCambioTarifaHabitacion === "function") {
          detallesVm.guardarCambioTarifaHabitacion();
          $(this).dialog("close");
        } else {
          console.error("guardarCambioTarifaHabitacion no está definida");
        }
      },
      "Cancelar": function () {
        $(this).dialog("close");
      }
    }
  });  // ← Asegúrate de que no haya caracteres adicionales aquí

  $("#mdl-editar-tarifa-estancia").dialog({
    autoOpen: false,
    modal: true,
    width: 550,
    title: "Cambiar tarifa de hospitalización actual",
    buttons: {
      "Actualizar": function () {
        detallesVm.guardarCambioTarifaEstancia();
        $(this).dialog("close");
      },
      "Cancelar": function () {
        $(this).dialog("close");
      }
    }
  });

});


// $(document).ready(function () {

//   var idInput = $("#HospitalizacionId").val();
//   if (idInput) {
//     detallesVm.hospitalizacionId(parseInt(idInput));
//   }

//   detallesVm.actualizarTotales();
//   detallesVm.consultarUsuariosAcceso();
//   detallesVm.consultarUsuarios();
//   detallesVm.consultarAmbulancias();
//   detallesVm.consultarRecetasExistentes();
//   detallesVm.consultarPaquetesExistentes();
//   detallesVm.consultarServiciosExistentes();
//   detallesVm.consultarMedicamentosExistentes();
//   detallesVm.consultarExamenesExistentes();
//   detallesVm.consultarDatosExamenFisico();
//   detallesVm.consultarHabitacionesDisponibles();
//   detallesVm.consultarPaquetesHospitalizacion();
//   detallesVm.consultarRegistrosHospitalizacion();
//   detallesVm.consultarServiciosHospitalizacion();
//   detallesVm.consultarMedicamentosHospitalizacion();
//   detallesVm.consultarExamenesHospitalizacion();
//   detallesVm.consultarDepositosHospitalizacion();
//   detallesVm.consultarReceta();
//   detallesVm.consultarPaquetesAplicados();
//   detallesVm.consultarTurnoEnfermeria();
//   detallesVm.consultarNotaEnfermeria();
//   detallesVm.consultarNotaEvolucion();
//   detallesVm.consultarHistorialCambiosHabitacion();
//   detallesVm.consultarKitsIngreso();
//   detallesVm.consultarInsumosDirectosAplicados();


//   if (detallesVm.hospitalizacionId() > 0) {
//     detallesVm.consultarIdEmergencia();
//   }

//   var fechaEntrada = moment().startOf("day");
//   var fechaSalida = moment().endOf("day");

//   $("#periodo-hospitalizacion").daterangepicker(
//     {
//       startDate: fechaEntrada,
//       endDate: fechaSalida,
//       timePicker: true,
//       locale: {
//         applyLabel: "Aplicar",
//         cancelLabel: "Cancelar",
//         format: "DD/MM/YYYY hh:mm A",
//         daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
//         monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Setiembre", "Octubre", "Noviembre", "Diciembre"],
//       },
//     },
//     function (start, end) {
//       var diffday = parseInt((end - start) / (1000 * 60 * 60 * 24));
//       rangoDias = diffday > 0 ? diffday + 1 : 1;
//     },
//   );


//   $('a[href="#tab-content-carga-documentos"]').on('shown.bs.tab', function () {
//     if (detallesVm && typeof detallesVm.cargarListaDocumentos === 'function') {
//       detallesVm.cargarListaDocumentos();
//     }
//   });

//   if ($('#tab-content-carga-documentos').is(':visible')) {
//     detallesVm.cargarListaDocumentos();
//   }
// });



function irADetallesPaciente(pacienteId) {
  window.open("/Pacientes/Informacion?pacienteId=" + pacienteId, "_blank");
}

function procesarSolicitud(index, solicitudes) {
  if (index >= solicitudes.length) {
    alert("Todas las solicitudes han sido procesadas correctamente.");
    return;
  }

  let solicitud = solicitudes[index];

  // ✅ Obtener los detalles completos de la solicitud
  detallesVm
    .obtenerSolicitudPorId(solicitud.id)
    .done(function (dataSolicitud) {
      showLoading();

      console.log(dataSolicitud);
      if (!dataSolicitud || !dataSolicitud.hospitalizacionId) {
        mensajeEmergenteError(
          `Error: La solicitud #${solicitud.id} no tiene datos válidos.`,
        );
        procesarSolicitud(index + 1, solicitudes);
        return;
      }

      $.ajax({
        url: "/Hospitalizacion/AgregarMedicamento",
        method: "POST",
        data: {
          HospitalizacionId: dataSolicitud.hospitalizacionId,
          ProductoId: dataSolicitud.productoId,
          UnidadMedidaVentaId: dataSolicitud.unidadMedidaVentaId,
          Cantidad: dataSolicitud.cantidad,
          Precio: dataSolicitud.precio,
          Indicaciones: dataSolicitud.indicaciones,
          ViaAdministracion: dataSolicitud.viaAdministracion,
          FrecuenciaAdministracion: dataSolicitud.frecuenciaAdministracion,
          PrecioId: dataSolicitud.precioId,
          FechaHoraAplicacionManual: dataSolicitud.fechaHoraAplicacionManual,
        },
        success: function (dataResult) {
          hideLoading();
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            // alert(`🥳🎉 Medicamento de solicitud #${solicitud.id} registrado correctamente.`);
            $.ajax({
              url: `/SolicitudMedicamento/ActualizarRegistroHospitalizacion/${solicitud.id}`,
              method: "PUT",
              contentType: "application/json",
              success: function () {
                // alert(`🥳🎉 Solicitud #${solicitud.id} actualizada exitosamente.`);
                procesarSolicitud(index + 1, solicitudes);
              },
              error: function () {
                mensajeEmergenteError(
                  `Error al actualizar solicitud #${solicitud.id}.`,
                );
                procesarSolicitud(index + 1, solicitudes);
              },
            });
          } else {
            mensajeEmergenteError(data.Mensaje);
            procesarSolicitud(index + 1, solicitudes);
          }
        },
        error: function () {
          hideLoading();
          mensajeEmergenteError(`Error en la solicitud #${solicitud.id}.`);
          procesarSolicitud(index + 1, solicitudes);
        },
      });
    })
    .fail(function () {
      mensajeEmergenteError(
        `Error al obtener detalles de la solicitud #${solicitud.id}.`,
      );
      procesarSolicitud(index + 1, solicitudes);
    });
}