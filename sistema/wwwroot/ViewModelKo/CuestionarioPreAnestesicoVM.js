function CuestionarioPreAnestesicoVM() {
    var self = this;

    var initialData = JSON.parse(document.getElementById('viewModelData').textContent);
    var d = initialData;

    self.pacienteRegistro        = ko.observable(d.pacienteRegistro        || '');
    self.pacienteNombre          = ko.observable(d.pacienteNombre          || '');
    self.pacienteEdad            = ko.observable(d.pacienteEdad            || '');
    self.fechaCuestionario       = ko.observable(d.fechaCuestionario       || moment().format('YYYY-MM-DD'));
    self.peso                    = ko.observable(d.peso                    || '');
    self.estatura                = ko.observable(d.estatura                || '');
    self.fechaUltimaRegla        = ko.observable(d.fechaUltimaRegla        || '');
    self.fechaProcedimiento      = ko.observable(d.fechaProcedimiento      || '');
    self.procedimientoProgramado = ko.observable(d.procedimientoProgramado || '');
    self.cirujano                = ko.observable(d.cirujano                || '');

    // ── Antecedentes ──
    self.pa_alergia           = ko.observable(d.pa_alergia           || 'NO');
    self.pa_alergiaCual       = ko.observable(d.pa_alergiaCual       || '');
    self.pa_fuma              = ko.observable(d.pa_fuma              || 'NO');
    self.pa_fumaCuanto        = ko.observable(d.pa_fumaCuanto        || '');
    self.pa_drogas            = ko.observable(d.pa_drogas            || 'NO');
    self.pa_drogasCuales      = ko.observable(d.pa_drogasCuales      || '');
    self.pa_alcohol           = ko.observable(d.pa_alcohol           || 'NO');
    self.pa_alcoholCuanto     = ko.observable(d.pa_alcoholCuanto     || '');
    self.pa_embarazo          = ko.observable(d.pa_embarazo          || 'NO');
    self.pa_transfusion       = ko.observable(d.pa_transfusion       || 'NO');
    self.pa_asma              = ko.observable(d.pa_asma              || 'NO');
    self.pa_pulmones          = ko.observable(d.pa_pulmones          || 'NO');
    self.pa_corazon           = ko.observable(d.pa_corazon           || 'NO');
    self.pa_ataqueCardiaco    = ko.observable(d.pa_ataqueCardiaco    || 'NO');
    self.pa_angina            = ko.observable(d.pa_angina            || 'NO');
    self.pa_soplo             = ko.observable(d.pa_soplo             || 'NO');
    self.pa_presion           = ko.observable(d.pa_presion           || 'NO');
    self.pa_higado            = ko.observable(d.pa_higado            || 'NO');
    self.pa_rinones           = ko.observable(d.pa_rinones           || 'NO');
    self.pa_diabetes          = ko.observable(d.pa_diabetes          || 'NO');
    self.pa_epilepsia         = ko.observable(d.pa_epilepsia         || 'NO');
    self.pa_derrame           = ko.observable(d.pa_derrame           || 'NO');
    self.pa_tiroides          = ko.observable(d.pa_tiroides          || 'NO');
    self.pa_anestesico        = ko.observable(d.pa_anestesico        || 'NO');
    self.pa_aceptaTransfusion = ko.observable(d.pa_aceptaTransfusion || 'SI');

    // ── Información Adicional ──
    self.ai_medicamentos       = ko.observable(d.ai_medicamentos       || '');
    self.ai_actividad          = ko.observable(d.ai_actividad          || 'NO');
    self.ai_actividadDetalle   = ko.observable(d.ai_actividadDetalle   || '');
    self.ai_operacionesPrevias = ko.observable(d.ai_operacionesPrevias || '');
    self.ai_comentarios        = ko.observable(d.ai_comentarios        || '');

    // ── Al cargar: si hay una ventana padre con hospitalizarVm, precargar sus datos ──
    (function precargarDesdeVentanaPadre() {
        try {
            var padre = window.opener && window.opener.hospitalizarVm;
            if (!padre || !padre.cuestionario) return;

            var c = padre.cuestionario;
            // Solo precargar campos que el padre ya tenga con valor
            if (c.pacienteRegistro())        self.pacienteRegistro(c.pacienteRegistro());
            if (c.pacienteNombre())          self.pacienteNombre(c.pacienteNombre());
            if (c.pacienteEdad())            self.pacienteEdad(c.pacienteEdad());
            if (c.fechaCuestionario())       self.fechaCuestionario(c.fechaCuestionario());
            if (c.peso())                    self.peso(c.peso());
            if (c.estatura())                self.estatura(c.estatura());
            if (c.fechaUltimaRegla())        self.fechaUltimaRegla(c.fechaUltimaRegla());
            if (c.fechaProcedimiento())      self.fechaProcedimiento(c.fechaProcedimiento());
            if (c.procedimientoProgramado()) self.procedimientoProgramado(c.procedimientoProgramado());
            if (c.cirujano())                self.cirujano(c.cirujano());
            if (c.pa_alergia())              self.pa_alergia(c.pa_alergia());
            if (c.pa_alergiaCual())          self.pa_alergiaCual(c.pa_alergiaCual());
            if (c.pa_fuma())                 self.pa_fuma(c.pa_fuma());
            if (c.pa_fumaCuanto())           self.pa_fumaCuanto(c.pa_fumaCuanto());
            if (c.pa_drogas())               self.pa_drogas(c.pa_drogas());
            if (c.pa_drogasCuales())         self.pa_drogasCuales(c.pa_drogasCuales());
            if (c.pa_alcohol())              self.pa_alcohol(c.pa_alcohol());
            if (c.pa_alcoholCuanto())        self.pa_alcoholCuanto(c.pa_alcoholCuanto());
            if (c.pa_embarazo())             self.pa_embarazo(c.pa_embarazo());
            if (c.pa_transfusion())          self.pa_transfusion(c.pa_transfusion());
            if (c.pa_asma())                 self.pa_asma(c.pa_asma());
            if (c.pa_pulmones())             self.pa_pulmones(c.pa_pulmones());
            if (c.pa_corazon())              self.pa_corazon(c.pa_corazon());
            if (c.pa_ataqueCardiaco())       self.pa_ataqueCardiaco(c.pa_ataqueCardiaco());
            if (c.pa_angina())               self.pa_angina(c.pa_angina());
            if (c.pa_soplo())                self.pa_soplo(c.pa_soplo());
            if (c.pa_presion())              self.pa_presion(c.pa_presion());
            if (c.pa_higado())               self.pa_higado(c.pa_higado());
            if (c.pa_rinones())              self.pa_rinones(c.pa_rinones());
            if (c.pa_diabetes())             self.pa_diabetes(c.pa_diabetes());
            if (c.pa_epilepsia())            self.pa_epilepsia(c.pa_epilepsia());
            if (c.pa_derrame())              self.pa_derrame(c.pa_derrame());
            if (c.pa_tiroides())             self.pa_tiroides(c.pa_tiroides());
            if (c.pa_anestesico())           self.pa_anestesico(c.pa_anestesico());
            if (c.pa_aceptaTransfusion())    self.pa_aceptaTransfusion(c.pa_aceptaTransfusion());
            if (c.ai_medicamentos())         self.ai_medicamentos(c.ai_medicamentos());
            if (c.ai_actividad())            self.ai_actividad(c.ai_actividad());
            if (c.ai_actividadDetalle())     self.ai_actividadDetalle(c.ai_actividadDetalle());
            if (c.ai_operacionesPrevias())   self.ai_operacionesPrevias(c.ai_operacionesPrevias());
            if (c.ai_comentarios())          self.ai_comentarios(c.ai_comentarios());
        } catch (e) {
            console.warn('No se pudo precargar desde ventana padre:', e);
        }
    })();

    // ── Guardar: escribe directamente en el ViewModel del padre ──
    self.guardando = ko.observable(false);

    self.guardarCuestionario = function () {
        if (self.guardando()) return;

        try {
            var padre = window.opener && window.opener.hospitalizarVm;
            if (!padre || !padre.cuestionario) {
                alert('No se encontró la ventana del paso a paso. Por favor no cierre la ventana principal.');
                return;
            }

            self.guardando(true);

            var c = padre.cuestionario;
            c.pacienteRegistro(self.pacienteRegistro());
            c.pacienteNombre(self.pacienteNombre());
            c.pacienteEdad(self.pacienteEdad());
            c.fechaCuestionario(self.fechaCuestionario());
            c.peso(self.peso());
            c.estatura(self.estatura());
            c.fechaUltimaRegla(self.fechaUltimaRegla());
            c.fechaProcedimiento(self.fechaProcedimiento());
            c.procedimientoProgramado(self.procedimientoProgramado());
            c.cirujano(self.cirujano());
            c.pa_alergia(self.pa_alergia());
            c.pa_alergiaCual(self.pa_alergiaCual());
            c.pa_fuma(self.pa_fuma());
            c.pa_fumaCuanto(self.pa_fumaCuanto());
            c.pa_drogas(self.pa_drogas());
            c.pa_drogasCuales(self.pa_drogasCuales());
            c.pa_alcohol(self.pa_alcohol());
            c.pa_alcoholCuanto(self.pa_alcoholCuanto());
            c.pa_embarazo(self.pa_embarazo());
            c.pa_transfusion(self.pa_transfusion());
            c.pa_asma(self.pa_asma());
            c.pa_pulmones(self.pa_pulmones());
            c.pa_corazon(self.pa_corazon());
            c.pa_ataqueCardiaco(self.pa_ataqueCardiaco());
            c.pa_angina(self.pa_angina());
            c.pa_soplo(self.pa_soplo());
            c.pa_presion(self.pa_presion());
            c.pa_higado(self.pa_higado());
            c.pa_rinones(self.pa_rinones());
            c.pa_diabetes(self.pa_diabetes());
            c.pa_epilepsia(self.pa_epilepsia());
            c.pa_derrame(self.pa_derrame());
            c.pa_tiroides(self.pa_tiroides());
            c.pa_anestesico(self.pa_anestesico());
            c.pa_aceptaTransfusion(self.pa_aceptaTransfusion());
            c.ai_medicamentos(self.ai_medicamentos());
            c.ai_actividad(self.ai_actividad());
            c.ai_actividadDetalle(self.ai_actividadDetalle());
            c.ai_operacionesPrevias(self.ai_operacionesPrevias());
            c.ai_comentarios(self.ai_comentarios());

            console.log('Cuestionario transferido al paso a paso correctamente.');
            alert('Cuestionario guardado. Puede cerrar esta ventana.');
            window.close();

        } catch (e) {
            console.error('Error al transferir cuestionario al padre:', e);
            alert('Error al guardar el cuestionario. Intente de nuevo.');
        } finally {
            self.guardando(false);
        }
    };
}

var vm = new CuestionarioPreAnestesicoVM();
ko.applyBindings(vm);