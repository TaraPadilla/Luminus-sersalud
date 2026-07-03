var PersonasListaVM = function () {
    var itemArchivosPaciente = 1;
    var itemBeneficiarioEps = 1;
    var itemAlergiaNueva = 1;

    var model = {};
    var self = this;
    self.nombreNuevoBeneficiario = ko.observable();
    self.nitNuevoBeneficiario = ko.observable();
    self.tipoNuevoBeneficiario = ko.observable();
    self.nombreAlergiaNueva = ko.observable();//Variable para guardar alergias
    self.vacunas = ko.observableArray();
    self.alergiasNuevas = ko.observableArray();//Variable para las nuevas alergias
    self.antecedentesPersonales = ko.observableArray();
    self.antecedentesFamiliares = ko.observableArray();
    self.preguntasRegistro = ko.observableArray();
    self.beneficiariosEpss = ko.observableArray();
    self.urlFotografiaPaciente = ko.observable();
    self.urlFirmaRegistro = ko.observable();
    self.urlFirmaPoliticas = ko.observable();
    self.urlFirmaConsentimiento = ko.observable();
    self.urlFirmaTarjetaCredito = ko.observable();
    self.archivos = ko.observableArray();
    self.archivosSubidos = ko.observableArray();

    //Archivos
    self.agregarArchivo = function () {
        var archivo = {
            Item: itemArchivosPaciente,
            Nombre: "",
            IdInputFile: "archivo-paciente-" + itemArchivosPaciente
        };
        itemArchivosPaciente++;
        self.archivos.push(archivo);
    };
    self.quitarArchivo = function (value) {
        $(self.archivos()).each(function (idxArchivo, archivo) {
            if (value.Item == archivo.Item) {
                self.archivos.splice(idxArchivo, 1);
            }
        });
    };

    
    

    //Alergia Nueva
    self.agregarAlergia = function () {
        if (self.nombreAlergiaNueva() == undefined || self.nombreAlergiaNueva() == null || self.nombreAlergiaNueva().trim() == '') {
            alert("debe escribir  un nombre de alergia");
            return false;
        }
        //Agregar una nueva alergia en la tabla de javaScritp 
        //Se a�ade el paciente id para porder enviarlo en el ajax
        var alergiaNueva = {
            Item: itemAlergiaNueva,
            NombreAlergia: self.nombreAlergiaNueva(),
            Estado: "Si",
            PacienteId: pacienteId
        };
        self.alergiasNuevas.push(alergiaNueva);
        self.nombreAlergiaNueva('');
        self.agregarAlergiaRara(alergiaNueva);
        itemAlergiaNueva++;
    };

    //Agrgar nuevas alergias ala base de datos
    self.agregarAlergiaRara = function (alergiaNueva) {
        /*self.clearTableVacunas();*/
        debugger;
        console.log(alergiaNueva)
        $.ajax({
            method: "POST",
            url: '/Pacientes/AgregarAlergiaRara',
            data: alergiaNueva,
            traditional: true,
            success: function (data, textStatus) {
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {

                    console.log(dataResult.Exitoso);
                }
                else
                    alert(dataResult.Mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };

    //Beneficiarios EPSS
    self.agregarBeneficiarioEpss = function () {
        if (self.nitNuevoBeneficiario() == undefined || self.nitNuevoBeneficiario() == null || self.nombreNuevoBeneficiario().trim() == '') {
            alert("Escriba un NIT de beneficiario");
            return false;
        }
        if (self.nombreNuevoBeneficiario() == undefined || self.nombreNuevoBeneficiario() == null || self.nombreNuevoBeneficiario().trim() == '') {
            alert("Escriba un nombre de beneficiario");
            return false;
        }
        var beneficiario = new Object();
        beneficiario.Item = itemBeneficiarioEps;
        beneficiario.Name = self.nombreNuevoBeneficiario();
        beneficiario.Tipo = self.tipoNuevoBeneficiario();
        beneficiario.Nit = self.nitNuevoBeneficiario();
        self.beneficiariosEpss.push(beneficiario);
        self.nombreNuevoBeneficiario('');
        self.nitNuevoBeneficiario('');
        itemBeneficiarioEps++;
    };
    self.quitarBeneficiarioEpss = function (value) {
        console.log(value);
        $(self.beneficiariosEpss()).each(function (idxBeneficiario, beneficiario) {
            if (value.Item == beneficiario.Item) {
                self.beneficiariosEpss.splice(idxBeneficiario, 1);
            }
        });
    };

    self.getModel = function () {
       
        model = {
           
            "PacienteId": $("#PacienteId").val(),

            "FechaRegistro": $("#FechaRegistro").val(),

            "Nombre": $("#Nombre").val(),
            "Alias": $("#Alias").val(),
            "SexoId": $("#SexoId").val(),
            "FechaNacimiento": $("#FechaNacimiento").val(),
            "Edad": $("#Edad").val(),
            "Peso": $("#Peso").val(),
            "NombreEncargado": $("#NombreEncargado").val(),
            "DPIEncargado": $("#DPIEncargado").val(),
            "TipoDeSangre": $("#TipoDeSangre").val(),
            "Telefono": $("#Telefono").val(),
            "Celular": $("#Celular").val(),
            "Email": $("#Email").val(),
            "Contrasennia": $("#Contrasennia").val(),
            "Nit": $("#Nit").val(),
            "Dpi": $("#Dpi").val(),
            "Direccion": $("#Direccion").val(),
            "no_IGGS": $("#no_IGGS").val(),
            "esta_Afiliado": $("#esta_Afiliado").val(),
            "SeguroEpssId": $("#SeguroEpssId").val() ?? null,
            "CodigoEPS": $("#CodigoEPS").val(),
            "Religion": $("#Religion").val(),
            "Ocupacion": $("#Ocupacion").val(),
            "EstadoCivil": $("#EstadoCivil").val(),
            "ContactoEmergencia": $("#ContactoEmergencia").val(),
            "NumeroContactoEmergencia": $("#NumeroContactoEmergencia").val(),
            "NombreContactoEmergencia": $("#NombreContactoEmergencia").val(),
            "Nacionalidad": $("#Nacionalidad").val(),
            "PaisResidencia": $("#PaisResidencia").val(),
            "DepartamentoResidencia": $("#DepartamentoResidencia").val(),
            "MunicipioResidencia": $("#MunicipioResidencia").val(),
            "TipoPacienteId": $("#TipoPacienteId").val(),
            "ModalidadAtencion": $("#ModalidadAtencion").val(),

            //Informaci�n de nacimiento
            "PesoAlNacer": $("#PesoAlNacer").val(),
            "Talla": $("#Talla").val(),
            "CircunferenciaCefalica": $("#CircunferenciaCefalica").val(),
            "TipoParto": $("#TipoParto").val(),

            //Antecedentes personales y patologicos observaciones
            "AntecedentesPersonalesObservaciones": $("#AntecedentesPersonalesObservaciones").val(),
            "AntecedentesPersonalesOtros": $("#AntecedentesPersonalesOtros").val(),

            //Alergias
            "AlergiaAnestesiaLocal": $("#AlergiaAnestesiaLocal").val(),
            "AlergiaAspirina": $("#AlergiaAspirina").val(),
            "AlergiaPenicilina": $("#AlergiaPenicilina").val(),
            "AlergiaBarbiturios": $("#AlergiaBarbiturios").val(),
            "AlergiaSulfas": $("#AlergiaSulfas").val(),
            "AlergiaCodeina": $("#AlergiaCodeina").val(),
            "AlergiaMetales": $("#AlergiaMetales").val(),
            "AlergiaLatex": $("#AlergiaLatex").val(),
            "AlergiaYodo": $("#AlergiaYodo").val(),
            "AlergiaPolen": $("#AlergiaPolen").val(),
            "AlergiaAnimales": $("#AlergiaAnimales").val(),
            "AlergiaAlimentos": $("#AlergiaAlimentos").val(),
            "AlergiaOtros": $("#AlergiaOtros").val(),
            "AlergiaOtrosDescripcion": $("#AlergiaOtrosDescripcion").val(),

            //Informacion medica
            "MedicaUsaLentesContacto": $("#MedicaUsaLentesContacto").val(),
            "MedicaUsaLentesContactoDescripcion": $("#MedicaUsaLentesContactoDescripcion").val(),
            "MedicaArticulacionesArtificiales": $("#MedicaArticulacionesArtificiales").val(),
            "MedicaArticulacionesArtificialesFecha": $("#MedicaArticulacionesArtificialesFecha").val(),
            "MedicaArticulacionesArtificialesComplicaciones": $("#MedicaArticulacionesArtificialesComplicaciones").val(),
            "MedicaTomaAlendronato": $("#MedicaTomaAlendronato").val(),
            "MedicaTomaAlendronatoFecha": $("#MedicaTomaAlendronatoFecha").val(),
            "MedicaTratamientoDolorHuesos": $("#MedicaTratamientoDolorHuesos").val(),
            "MedicaTratamientoDolorHuesosFechaInicio": $("#MedicaTratamientoDolorHuesosFechaInicio").val(),
            "MedicaTratamientoDolorHuesosDescripcionCaso": $("#MedicaTratamientoDolorHuesosDescripcionCaso").val(),
            "MedicaSustanciasReguladorasDrogas": $("#MedicaSustanciasReguladorasDrogas").val(),
            "MedicaSustanciasReguladorasDrogasFecha": $("#MedicaSustanciasReguladorasDrogasFecha").val(),
            "MedicaUsaTabaco": $("#MedicaUsaTabaco").val(),
            "MedicaBebidasAlcoholicas": $("#MedicaBebidasAlcoholicas").val(),
            "MedicaBebidasAlcoholicasDescripcion": $("#MedicaBebidasAlcoholicasDescripcion").val(),

            //Informaci�n dental
            "DentalSangradoCepillar": $("#DentalSangradoCepillar").val(),
            "DentalDolorFrio": $("#DentalDolorFrio").val(),
            "DentalDolorPresionar": $("#DentalDolorPresionar").val(),
            "DentalObjetosAtorados": $("#DentalObjetosAtorados").val(),
            "DentalBocaSeca": $("#DentalBocaSeca").val(),
            "DentalTratamientoPeriondal": $("#DentalTratamientoPeriondal").val(),
            "DentalTratamientoOrtodoncia": $("#DentalTratamientoOrtodoncia").val(),
            "DentalProblemasTratamientoDental": $("#DentalProblemasTratamientoDental").val(),
            "DentalProblemasTratamientoDentalDescripcion": $("#DentalProblemasTratamientoDentalDescripcion").val(),
            "DentalFluoradaAguaDomicilio": $("#DentalFluoradaAguaDomicilio").val(),
            "DentalBebeAguaFiltrada": $("#DentalBebeAguaFiltrada").val(),
            "DentalDolorOidos": $("#DentalDolorOidos").val(),
            "DentalMolestiaRuidoAlto": $("#DentalMolestiaRuidoAlto").val(),
            "DentalMolestiaRuidoAltoDescripcion": $("#DentalMolestiaRuidoAltoDescripcion").val(),
            "DentalBrumismo": $("#DentalBrumismo").val(),
            "DentalLesiones": $("#DentalLesiones").val(),
            "DentalLesionesDescripcion": $("#DentalLesionesDescripcion").val(),
            "DentalDentaduraPlacas": $("#DentalDentaduraPlacas").val(),
            "DentalDentaduraPlacasDescripcion": $("#DentalDentaduraPlacasDescripcion").val(),
            "DentalActividadesRecreacion": $("#DentalActividadesRecreacion").val(),
            "DentalActividadesRecreacionDescripcion": $("#DentalActividadesRecreacionDescripcion").val(),
            "DentalLesionesCabeza": $("#DentalLesionesCabeza").val(),
            "DentalLesionesCabezaDescripcion": $("#DentalLesionesCabezaDescripcion").val(),

            //An�lisis facial
            "FacialPatron": $("#FacialPatron").val(),
            "FacialPatronObservaciones": $("#FacialPatronObservaciones").val(),
            "FacialPerfil": $("#FacialPerfil").val(),
            "FacialPerfilObservaciones": $("#FacialPerfilObservaciones").val(),
            "FacialAsimetria": $("#FacialAsimetria").val(),
            "FacialAsimetriaObservaciones": $("#FacialAsimetriaObservaciones").val(),
            "FacialAlturaFacialEquilibrada": $("#FacialAlturaFacialEquilibrada").val(),
            "FacialAlturaFacialEquilibradaObservaciones": $("#FacialAlturaFacialEquilibradaObservaciones").val(),
            "FacialAnchoFacialEquilibrada": $("#FacialAnchoFacialEquilibrada").val(),
            "FacialAnchoFacialEquilibradaObservaciones": $("#FacialAnchoFacialEquilibradaObservaciones").val(),
            "FacialPerfilMaxilar": $("#FacialPerfilMaxilar").val(),
            "FacialPerfilMaxilarObservaciones": $("#FacialPerfilMaxilarObservaciones").val(),
            "FacialPerfilMandibular": $("#FacialPerfilMandibular").val(),
            "FacialPerfilMandibularObservaciones": $("#FacialPerfilMandibularObservaciones").val(),
            "FacialSurcoLabialMenton": $("#FacialSurcoLabialMenton").val(),
            "FacialSurcoLabialMentonObservaciones": $("#FacialSurcoLabialMentonObservaciones").val(),
            "FacialLabiosReposo": $("#FacialLabiosReposo").val(),

            //An�lisis funcional
            "FuncionalActividadComisurial": $("#FuncionalActividadComisurial").val(),
            "FuncionalActividadLingual": $("#FuncionalActividadLingual").val(),
            "FuncionalLabioSuperior": $("#FuncionalLabioSuperior").val(),
            "FuncionalLabioInferior": $("#FuncionalLabioInferior").val(),
            "FuncionalMasetero": $("#FuncionalMasetero").val(),
            "FuncionalMentoniano": $("#FuncionalMentoniano").val(),
            "FuncionalRespiracion": $("#FuncionalRespiracion").val(),
            "FuncionalDeglucion": $("#FuncionalDeglucion").val(),

            //Patr�n facial
            "PatronFacial": $("#PatronFacial").val(),
            "CaracteristicaPatronFacial": $("#CaracteristicaPatronFacial").val(),

            //MEDICOS
            //Antecedentes
            "AntecedentesMedicos": $("#AntecedentesMedicos").val(),
            "AntecedentesQuirurgicos": $("#AntecedentesQuirurgicos").val(),
            "AntecedentesTraumaticos": $("#AntecedentesTraumaticos").val(),
            "AntecedentesAlergias": $("#AntecedentesAlergias").val(),
            "AntecedentesVicios": $("#AntecedentesVicios").val(),
            "AntecedentesMedicamentos": $("#AntecedentesMedicamentos").val(),


            //Pediatricos
            "NombrePadre": $("#NombrePadre").val(),
            "NombreMadre": $("#NombreMadre").val(),

            //Informaci�n de la madre
            "MadreFechaNacimiento": $("#MadreFechaNacimiento").val(),
            "MadreEmbarazos": $("#MadreEmbarazos").val(),
            "MadrePartosNormales": $("#MadrePartosNormales").val(),
            "MadreCesareas": $("#MadreCesareas").val(),
            "MadreAbortos": $("#MadreAbortos").val(),
            "MadreHijosMuertos": $("#MadreHijosMuertos").val(),
            "MadreComplicaciones": $("#MadreComplicaciones").val(),

            //Historia medica
            "HistoriaMedicoPersonal": $("#HistoriaMedicoPersonal").val(),
            "HistoriaTelefonoMedico": $("#HistoriaTelefonoMedico").val(),
            "HistoriaEspecialidadMedico": $("#HistoriaEspecialidadMedico").val(),
            "HistoriaTratamientoMedico": $("#HistoriaTratamientoMedico").val(),
            "HistoriaSangraMuchoCortarse": $("#HistoriaSangraMuchoCortarse").val(),
            "HistoriaHospitalizado": $("#HistoriaHospitalizado").val(),
            "HistoriaOperado": $("#HistoriaOperado").val(),
            "HistoriaAlergiaMedicina": $("#HistoriaAlergiaMedicina").val(),
            "HistoriaAlergiaComida": $("#HistoriaAlergiaComida").val(),
            "HistoriaAlergiaOtros": $("#HistoriaAlergiaOtros").val(),
            "HistoriaProblemaEmocional": $("#HistoriaProblemaEmocional").val(),
            "HistoriaObservaciones": $("#HistoriaObservaciones").val(),

            //Datos de IGSS
            "IgssTipoAfiliacion": $("#IgssTipoAfiliacion").val(),
            "IgssNumeroAfiliacion": $("#IgssNumeroAfiliacion").val(),
            "IgssCantidadDependientes": $("#IgssCantidadDependientes").val(),
            "IgssParentescoDependientes": $("#IgssParentescoDependientes").val(),

            //Politicas de pago
            "PoliticasPagoAceptaTerminos": $("#PoliticasPagoAceptaTerminos").val(),

            //Datos de pago
            "NumeroTarjetaCredito": $("#NumeroTarjetaCredito").val(),

            //Antecedentes
            "AntecedentesPersonalesViewModel": self.antecedentesPersonales(),

            //Vacunas
            "VacunasPacienteViewModel": self.vacunas(),

            //Beneficiarios EPSS
            "BeneficiariosEpssPacientesViewModel": self.beneficiariosEpss(),

            //Antecedentes familiares
            "PatologiasPacienteViewModel": self.antecedentesFamiliares(),

            //Preguntas registro
            "PreguntasRegistroPacienteViewModel": self.preguntasRegistro(),

            //Datos Ginecologicos PRUEBA TECNICA ESTUARDO
            "CicloMenstGine": $("#CicloMenstGine").val(),
            "ETSGine": $("#ETSGine").val(),
            "VIHGine": $("#VIHGine").val(),
            "GrupoFactorGine": $("#GrupoFactorGine").val(),
            "TorchGine": $("#TorchGine").val(),
            "InicioVidaSexualGine": $("#InicioVidaSexualGine").val(),
            "ParejasSexGine": $("#ParejasSexGine").val(),
            "ObesidadGine": $("#ObesidadGine").val(),
            "DesnutricionGine": $("#DesnutricionGine").val(),
            "QGine": $("#QGine").val(),
            "PGine": $("#PGine").val(),
            "ABGine": $("#ABGine").val(),
            "CGine": $("#CGine").val(),
            "FURGine": $("#FURGine").val(),
            "MuerteNeoGine": $("#MuerteNeoGine").val(),
            "FPPGine": $("#FPPGine").val(),
            "HVGine": $("#HVGine").val(),
            "MuerteGine": $("#MuerteGine").val(),
            "ControlPrenatalGine": $("#ControlPrenatalGine").val(),
            "ComadronaGine": $("#ComadronaGine").val(),
            "NoControlesGine": $("#NoControlesGine").val(),

            // Datos mama
            "AbdomenObstetricoGine": $("#AbdomenObstetricoGine").val(),
            "UteroGravioGine": $("#UteroGravioGine").val(),
            "FCBGine": $("#FCBGine").val(),
            "AUGine": $("#AUGine").val(),
            "PresentacionLeopoldGine": $("#PresentacionLeopoldGine").val(),
            "OtrasGine": $("#OtrasGine").val(),
            "ActividadUterinaGine": $("#ActividadUterinaGine").val(),
            "MovimientoFetalPercetibleGine": $("#MovimientoFetalPercetibleGine").val(),
            "EspecifiqueGine": $("#EspecifiqueGine").val(),
            "TactoVaginalGine": $("#TactoVaginalGine").val(),
            "DGine": $("#DGine").val(),
            "CMSGine": $("#CMSGine").val(),
            "BPorcientoGine": $("#BPorcientoGine").val(),
            "AltiutudGine": $("#AltiutudGine").val(),
            "VariedadPosicionGine": $("#VariedadPosicionGine").val(),
            "MembranasOvularesGine": $("#MembranasOvularesGine").val(),
            "LiquidoAmnioticoGine": $("#LiquidoAmnioticoGine").val(),
            "Especifique2Gine": $("#Especifique2Gine").val(),
            "PelvisGine": $("#PelvisGine").val()

        };
    };

    self.registrarPaciente = function () {
        if (confirm("�Desea registrar este nuevo paciente?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Pacientes/Nuevo',
                data: model,
                success: function (data) {
                    if (data.exitoso) {
                        window.location.href = "/Pacientes/Informacion?pacienteId=" + data.pacienteId;
                    }
                    else {
                        $("#div-loading").hide();
                        alert(data.mensaje);
                    }
                },
                error: function (data) {
                    $("#div-loading").hide();
                    alert(data.error);
                }
            });
        }
    };


    self.cancelarRegistroPaciente = function () {
        if (confirm("�Desea cancelar el registro del paciente?")) {
            window.location.href = "/Pacientes/Lista";
        }
    };
    self.editarPaciente = function () {
        debugger;
        self.getModel();
        $.ajax({
            method: "POST",
            url: '/Pacientes/Modificar',
            data: model,
            success: function (data, textStatus) {
                if (data.exitoso) {
                    window.location.href = "/Pacientes/Lista";
                }
                else
                    alert(data.mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };
    
    self.cancelarEdicionPaciente = function () {
        if (confirm("�Desea cancelar la edici�n del paciente?")) {
            window.location.href = "/Pacientes/Lista";
        }
    };

    self.consultarVacunas = function () {
        self.clearTableVacunas();

        $.ajax({
            method: "POST",
            url: '/Pacientes/ConsultarVacunas',
            data: {
                pacienteId: pacienteId == '' ? null : pacienteId
            },
            traditional: true,
            success: function (data, textStatus) {
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.vacunas(dataResult.Resultado);
                    self.drawTableVacunas();
                }
                else
                    alert(dataResult.Mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };

    //Metodo para consultar las nuevas alergias
    self.consultarAlergiasNuevas = function () {
        self.clearTableAlergiasNuevas();
        $.ajax({
            method: "POST",
            url: '/Pacientes/ConsultarAlergiasRaras',
            data: {
                pacienteId: pacienteId == '' ? null : pacienteId
            },
            traditional: true,
            success: function (data, textStatus) {
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.alergiasNuevas(dataResult.Resultado);
                    self.drawTableAlergiasNuevas();
                }
                else
                    alert(dataResult.Mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };
    self.consultarAntecedentesPersonales = function () {
        self.clearTableAntecedentesPersonales();

        $.ajax({
            method: "POST",
            url: '/Pacientes/ConsultarAntecedentesPersonales',
            data: {
                pacienteId: pacienteId == '' ? null : pacienteId
            },
            traditional: true,
            success: function (data, textStatus) {
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.antecedentesPersonales(dataResult.Resultado);
                    self.drawTableAntecedentesPersonales();
                }
                else
                    alert(dataResult.Mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };
    self.consultarBeneficiariosEpss = function () {

        if (pacienteId != '') {

            $.ajax({
                method: "POST",
                url: '/Pacientes/ConsultarPersonasSeguro',
                data: {
                    pacienteId: pacienteId
                },
                traditional: true,
                success: function (data, textStatus) {
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        self.beneficiariosEpss(dataResult.Resultado);
                    }
                    else
                        alert(dataResult.Mensaje);
                },
                error: function (data) {
                    alert(data.error);
                }
            });
        }

    };
    self.consultarAntecedentesFamiliares = function () {
        self.clearTableAntecedentesFamiliares();

        $.ajax({
            method: "POST",
            url: '/Pacientes/ConsultarAntecedentesFamiliares',
            data: {
                pacienteId: pacienteId == '' ? null : pacienteId
            },
            traditional: true,
            success: function (data, textStatus) {
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.antecedentesFamiliares(dataResult.Resultado);
                    self.drawTableAntecedentesFamiliares();
                }
                else
                    alert(dataResult.Mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };
    self.consultarPreguntasRegistro = function () {
        self.clearTablePreguntasRegistro();

        $.ajax({
            method: "POST",
            url: '/Pacientes/ConsultarPreguntasRegistro',
            data: {
                pacienteId: pacienteId == '' ? null : pacienteId
            },
            traditional: true,
            success: function (data, textStatus) {
                var dataResult = JSON.parse(data);
                if (dataResult.Exitoso) {
                    self.preguntasRegistro(dataResult.Resultado);
                    self.drawTablePreguntasRegistro();
                }
                else
                    alert(dataResult.Mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };
    //Destruir y crear tabla alergias nuevas
    self.clearTableAlergiasNuevas = function () {
        var table = $("#tabla-alergias-nuevas").DataTable();
        table.clear().draw();

        $("#tabla-alergias-nuevas").dataTable().fnDestroy();
    };
    self.drawTableAlergiasNuevas = function () {
        $("#tabla-alergias-nuevas").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por p&aacute;gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };
    

    self.clearTableVacunas = function () {
        var table = $("#tabla-vacunas").DataTable();
        table.clear().draw();

        $("#tabla-vacunas").dataTable().fnDestroy();
    };
    self.drawTableVacunas = function () {
        $("#tabla-vacunas").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por p&aacute;gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };
    self.clearTableAntecedentesPersonales = function () {
        var table = $("#tabla-antecedentes-personales").DataTable();
        table.clear().draw();

        $("#tabla-antecedentes-personales").dataTable().fnDestroy();
    };
    self.drawTableAntecedentesPersonales = function () {
        $("#tabla-antecedentes-personales").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por p&aacute;gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };
    self.clearTableAntecedentesFamiliares = function () {
        var table = $("#tabla-antecedentes-familiares").DataTable();
        table.clear().draw();

        $("#tabla-antecedentes-familiares").dataTable().fnDestroy();
    };
    self.drawTableAntecedentesFamiliares = function () {
        $("#tabla-antecedentes-familiares").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                //search: "Buscar: ",
                //lengthMenu: "Mostrar _MENU_ registros por p�gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                //info: "Mostrando p�gina _PAGE_ de _PAGES_",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };
    self.clearTablePreguntasRegistro = function () {
        var table = $("#tabla-preguntas-registro").DataTable();
        table.clear().draw();

        $("#tabla-preguntas-registro").dataTable().fnDestroy();
    };
    self.drawTablePreguntasRegistro = function () {
        $("#tabla-preguntas-registro").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por p�gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };
    self.drawTableAnalisisFacial = function () {
        $("#tabla-analisis-facial").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por p&aacute;gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                //info: "Mostrando p&aacute;gina _PAGE_ de _PAGES_",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };
    self.drawTableAlergias = function () {
        $("#tabla-alergias").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por p&aacute;gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };

   
    self.drawTableInformacionMedica = function () {
        $("#tabla-informacion-medica").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por p&aacute;gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };
    self.drawTableInformacionDental = function () {
        $("#tabla-informacion-dental").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por p&aacute;gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };
    self.drawTableAnalisisFuncional = function () {
        $("#tabla-analisis-funcional").DataTable({
            searching: false,
            ordering: false,
            paging: false,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por p&aacute;gina",
                zeroRecords: "No hay registros para mostrar",
                info: "",
                //info: "Mostrando p&aacute;gina _PAGE_ de _PAGES_",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "�ltimo",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };
}

var listaPersonasVM = new PersonasListaVM();
ko.applyBindings(listaPersonasVM);

$(document).ready(function () {
    $("#tabs").tabs();

    listaPersonasVM.consultarVacunas();
    listaPersonasVM.consultarAlergiasNuevas(); //Consultar las alergias inusuales
    listaPersonasVM.consultarAntecedentesPersonales();
    listaPersonasVM.consultarAntecedentesFamiliares();
    listaPersonasVM.consultarPreguntasRegistro();
    listaPersonasVM.consultarBeneficiariosEpss();
    listaPersonasVM.drawTableAnalisisFacial();
    listaPersonasVM.drawTableAlergias();
    listaPersonasVM.drawTableInformacionMedica();
    listaPersonasVM.drawTableInformacionDental();
    listaPersonasVM.drawTableAnalisisFuncional();
   
});

function getEdad() {
    let hoy = new Date();
    let fechaNacimiento = new Date($("#FechaNacimiento").val());
    let edad = hoy.getFullYear() - fechaNacimiento.getFullYear();
    let diferenciaMeses = hoy.getMonth() - fechaNacimiento.getMonth();
    if (
        diferenciaMeses < 0 ||
        (diferenciaMeses === 0 && hoy.getDate() < fechaNacimiento.getDate())
    ) {
        edad--;
    }
    $("#Edad").val(edad);
}