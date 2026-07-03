function ListaChequeoVM() {
    var self = this;

    var initialData = JSON.parse(document.getElementById('viewModelData').textContent);
    var d = initialData;

    self.pacienteNombre = ko.observable(d.pacienteNombre || '');
    self.pacienteFechaNacimiento = ko.observable(d.pacienteFechaNacimiento || '');

    self.listaChequeo = {
        // Encabezado
        fechaChequeo: ko.observable(d.fechaChequeo || moment().format('YYYY-MM-DD')),
        horaChequeo: ko.observable(d.horaChequeo || moment().format('HH:mm')),

        // ENTRADA - Paciente
        ci_nombreConfirma: ko.observable(d.ci_nombreConfirma || ''),
        ci_apellidoConfirma: ko.observable(d.ci_apellidoConfirma || ''),
        ci_fechaNacConfirma: ko.observable(d.ci_fechaNacConfirma || ''),
        ci_consentimiento: ko.observable(d.ci_consentimiento || ''),
        ci_operacion: ko.observable(d.ci_operacion || ''),
        ci_ladoOperar: ko.observable(d.ci_ladoOperar || ''),
        ci_sitioMarcado: ko.observable(d.ci_sitioMarcado || ''),
        ci_alergia: ko.observable(d.ci_alergia || ''),

        // ENTRADA - Anestesiólogo
        ci_evalPreanestesica: ko.observable(d.ci_evalPreanestesica || ''),
        ci_accesoIV: ko.observable(d.ci_accesoIV || ''),
        ci_equipoAnestesia: ko.observable(d.ci_equipoAnestesia || ''),
        ci_medicamentos: ko.observable(d.ci_medicamentos || ''),
        ci_oximetro: ko.observable(d.ci_oximetro || ''),
        ci_equipoAspiracion: ko.observable(d.ci_equipoAspiracion || ''),
        ci_viaAerea: ko.observable(d.ci_viaAerea || ''),

        // PAUSA - Cirujano
        cp_presentacion: ko.observable(d.cp_presentacion || ''),
        cp_nombrePacienteCirujano: ko.observable(d.cp_nombrePacienteCirujano || ''),
        cp_nombreCirugia: ko.observable(d.cp_nombreCirugia || ''),
        cp_eventosCriticos: ko.observable(d.cp_eventosCriticos || ''),
        cp_tiempoDuracion: ko.observable(d.cp_tiempoDuracion || ''),
        cp_imagenesDiagnosticas: ko.observable(d.cp_imagenesDiagnosticas || ''),
        cp_perdidaSangre: ko.observable(d.cp_perdidaSangre || ''),

        // PAUSA - Instrumentista
        cp_esterilidad: ko.observable(d.cp_esterilidad || ''),
        cp_materialesAdicionales: ko.observable(d.cp_materialesAdicionales || ''),

        // PAUSA - Anestesiólogo
        cp_eventosCriticosAnest: ko.observable(d.cp_eventosCriticosAnest || ''),
        cp_profilaxisAntibiotica: ko.observable(d.cp_profilaxisAntibiotica || ''),
        cp_tromboprofilaxis: ko.observable(d.cp_tromboprofilaxis || ''),
        cp_manejoDolor: ko.observable(d.cp_manejoDolor || ''),

        // SALIDA - Enfermera
        cs_nombreOperacion: ko.observable(d.cs_nombreOperacion || ''),
        cs_nombreEnfermera: ko.observable(d.cs_nombreEnfermera || ''),
        cs_recuentoCompleto: ko.observable(d.cs_recuentoCompleto || ''),
        cs_etiquetadoMuestras: ko.observable(d.cs_etiquetadoMuestras || ''),

        // SALIDA - Recuperación
        cs_repasoPostOp: ko.observable(d.cs_repasoPostOp || ''),
        cs_porQue: ko.observable(d.cs_porQue || ''),
        cs_traslado: ko.observable(d.cs_traslado || ''),
        cs_complicaciones: ko.observable(d.cs_complicaciones || ''),
        cs_servicioNumCama: ko.observable(d.cs_servicioNumCama || ''),
    };

    // ── Al cargar: precargar desde el ViewModel del padre si existe ──
    (function precargarDesdeVentanaPadre() {
        try {
            var padre = window.opener && window.opener.hospitalizarVm;
            if (!padre || !padre.listaChequeo) return;

            var lc = padre.listaChequeo;
            var slc = self.listaChequeo;

            if (lc.fechaChequeo()) slc.fechaChequeo(lc.fechaChequeo());
            if (lc.horaChequeo()) slc.horaChequeo(lc.horaChequeo());
            if (lc.ci_nombreConfirma()) slc.ci_nombreConfirma(lc.ci_nombreConfirma());
            if (lc.ci_apellidoConfirma()) slc.ci_apellidoConfirma(lc.ci_apellidoConfirma());
            if (lc.ci_fechaNacConfirma()) slc.ci_fechaNacConfirma(lc.ci_fechaNacConfirma());
            if (lc.ci_consentimiento()) slc.ci_consentimiento(lc.ci_consentimiento());
            // if (lc.ci_operacion())              slc.ci_operacion(lc.ci_operacion());
            if (lc.ci_operacion()) slc.ci_operacion(lc.ci_operacion());
            if (lc.cp_nombreCirugia()) slc.cp_nombreCirugia(lc.cp_nombreCirugia());
            if (lc.ci_ladoOperar()) slc.ci_ladoOperar(lc.ci_ladoOperar());
            if (lc.ci_sitioMarcado()) slc.ci_sitioMarcado(lc.ci_sitioMarcado());
            if (lc.ci_alergia()) slc.ci_alergia(lc.ci_alergia());
            if (lc.ci_evalPreanestesica()) slc.ci_evalPreanestesica(lc.ci_evalPreanestesica());
            if (lc.ci_accesoIV()) slc.ci_accesoIV(lc.ci_accesoIV());
            if (lc.ci_equipoAnestesia()) slc.ci_equipoAnestesia(lc.ci_equipoAnestesia());
            if (lc.ci_medicamentos()) slc.ci_medicamentos(lc.ci_medicamentos());
            if (lc.ci_oximetro()) slc.ci_oximetro(lc.ci_oximetro());
            if (lc.ci_equipoAspiracion()) slc.ci_equipoAspiracion(lc.ci_equipoAspiracion());
            if (lc.ci_viaAerea()) slc.ci_viaAerea(lc.ci_viaAerea());
            if (lc.cp_presentacion()) slc.cp_presentacion(lc.cp_presentacion());
            if (lc.cp_nombrePacienteCirujano()) slc.cp_nombrePacienteCirujano(lc.cp_nombrePacienteCirujano());
            if (lc.cp_nombreCirugia()) slc.cp_nombreCirugia(lc.cp_nombreCirugia());
            if (lc.cp_eventosCriticos()) slc.cp_eventosCriticos(lc.cp_eventosCriticos());
            if (lc.cp_tiempoDuracion()) slc.cp_tiempoDuracion(lc.cp_tiempoDuracion());
            if (lc.cp_imagenesDiagnosticas()) slc.cp_imagenesDiagnosticas(lc.cp_imagenesDiagnosticas());
            if (lc.cp_perdidaSangre()) slc.cp_perdidaSangre(lc.cp_perdidaSangre());
            if (lc.cp_esterilidad()) slc.cp_esterilidad(lc.cp_esterilidad());
            if (lc.cp_materialesAdicionales()) slc.cp_materialesAdicionales(lc.cp_materialesAdicionales());
            if (lc.cp_eventosCriticosAnest()) slc.cp_eventosCriticosAnest(lc.cp_eventosCriticosAnest());
            if (lc.cp_profilaxisAntibiotica()) slc.cp_profilaxisAntibiotica(lc.cp_profilaxisAntibiotica());
            if (lc.cp_tromboprofilaxis()) slc.cp_tromboprofilaxis(lc.cp_tromboprofilaxis());
            if (lc.cp_manejoDolor()) slc.cp_manejoDolor(lc.cp_manejoDolor());
            if (lc.cs_nombreOperacion()) slc.cs_nombreOperacion(lc.cs_nombreOperacion());
            if (lc.cs_nombreEnfermera()) slc.cs_nombreEnfermera(lc.cs_nombreEnfermera());
            if (lc.cs_recuentoCompleto()) slc.cs_recuentoCompleto(lc.cs_recuentoCompleto());
            if (lc.cs_etiquetadoMuestras()) slc.cs_etiquetadoMuestras(lc.cs_etiquetadoMuestras());
            if (lc.cs_repasoPostOp()) slc.cs_repasoPostOp(lc.cs_repasoPostOp());
            if (lc.cs_porQue()) slc.cs_porQue(lc.cs_porQue());
            if (lc.cs_traslado()) slc.cs_traslado(lc.cs_traslado());
            if (lc.cs_complicaciones()) slc.cs_complicaciones(lc.cs_complicaciones());
            if (lc.cs_servicioNumCama()) slc.cs_servicioNumCama(lc.cs_servicioNumCama());

        } catch (e) {
            console.warn('No se pudo precargar desde ventana padre:', e);
        }
    })();

    // ── Guardar: escribe directamente en el ViewModel del padre ──
    self.guardando = ko.observable(false);

    self.guardarListaChequeo = function () {
        if (self.guardando()) return;

        try {
            var padre = window.opener && window.opener.hospitalizarVm;
            if (!padre || !padre.listaChequeo) {
                alert('No se encontró la ventana del paso a paso. Por favor no cierre la ventana principal.');
                return;
            }

            self.guardando(true);

            var lc = padre.listaChequeo;
            var slc = self.listaChequeo;

            lc.fechaChequeo(slc.fechaChequeo());
            lc.horaChequeo(slc.horaChequeo());
            lc.ci_nombreConfirma(slc.ci_nombreConfirma());
            lc.ci_apellidoConfirma(slc.ci_apellidoConfirma());
            lc.ci_fechaNacConfirma(slc.ci_fechaNacConfirma());
            lc.ci_consentimiento(slc.ci_consentimiento());
            lc.ci_operacion(slc.ci_operacion());
            lc.ci_ladoOperar(slc.ci_ladoOperar());
            lc.ci_sitioMarcado(slc.ci_sitioMarcado());
            lc.ci_alergia(slc.ci_alergia());
            lc.ci_evalPreanestesica(slc.ci_evalPreanestesica());
            lc.ci_accesoIV(slc.ci_accesoIV());
            lc.ci_equipoAnestesia(slc.ci_equipoAnestesia());
            lc.ci_medicamentos(slc.ci_medicamentos());
            lc.ci_oximetro(slc.ci_oximetro());
            lc.ci_equipoAspiracion(slc.ci_equipoAspiracion());
            lc.ci_viaAerea(slc.ci_viaAerea());
            lc.cp_presentacion(slc.cp_presentacion());
            lc.cp_nombrePacienteCirujano(slc.cp_nombrePacienteCirujano());
            lc.cp_nombreCirugia(slc.cp_nombreCirugia());
            lc.cp_eventosCriticos(slc.cp_eventosCriticos());
            lc.cp_tiempoDuracion(slc.cp_tiempoDuracion());
            lc.cp_imagenesDiagnosticas(slc.cp_imagenesDiagnosticas());
            lc.cp_perdidaSangre(slc.cp_perdidaSangre());
            lc.cp_esterilidad(slc.cp_esterilidad());
            lc.cp_materialesAdicionales(slc.cp_materialesAdicionales());
            lc.cp_eventosCriticosAnest(slc.cp_eventosCriticosAnest());
            lc.cp_profilaxisAntibiotica(slc.cp_profilaxisAntibiotica());
            lc.cp_tromboprofilaxis(slc.cp_tromboprofilaxis());
            lc.cp_manejoDolor(slc.cp_manejoDolor());
            lc.cs_nombreOperacion(slc.cs_nombreOperacion());
            lc.cs_nombreEnfermera(slc.cs_nombreEnfermera());
            lc.cs_recuentoCompleto(slc.cs_recuentoCompleto());
            lc.cs_etiquetadoMuestras(slc.cs_etiquetadoMuestras());
            lc.cs_repasoPostOp(slc.cs_repasoPostOp());
            lc.cs_porQue(slc.cs_porQue());
            lc.cs_traslado(slc.cs_traslado());
            lc.cs_complicaciones(slc.cs_complicaciones());
            lc.cs_servicioNumCama(slc.cs_servicioNumCama());

            console.log('Lista de chequeo transferida al paso a paso correctamente.');
            alert('Lista de chequeo guardada. Puede cerrar esta ventana.');
            window.close();

        } catch (e) {
            console.error('Error al transferir lista de chequeo al padre:', e);
            alert('Error al guardar la lista de chequeo. Intente de nuevo.');
        } finally {
            self.guardando(false);
        }
    };
}

var listaChequeoVm = new ListaChequeoVM();
ko.applyBindings(listaChequeoVm);