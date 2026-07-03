var EmergenciasListaVM = function () {
    var self = this;

    self.emergencias = ko.observableArray();

    self.consultarEmergencias = function () {
        let textoCargando = $("#texto-cargando-emergencias");
        let textoError = $("#texto-error-cargando-emergencias");
        textoCargando.show();
        textoError.hide();
        clearDataTable("tabla-emergencias");
        $.ajax({
            url: "/Emergencias/ConsultarListaEmergencias",
            method: "POST",
            data: {
                ingresadas: $("#Ingresadas").val() == "True"
                    || $("#Ingresadas").val() == "true"
            },
            success: function (result) {
                textoCargando.hide();
                let data = JSON.parse(result);
                if (data.Exitoso) {

                    $(data.Resultado).each(function (idx, emergencia) {
                        if (emergencia.EmergenciaValorTotal != null) {
                            // let valor = parseFloat(emergencia.EmergenciaValorTotal);
                            let valor = parseFloat(emergencia.EmergenciaValorTotal.replace(',', '.'));

                            if (!isNaN(valor)) {
                                emergencia.EmergenciaValorTotal = valor.toFixed(2);
                            }
                        }
                    });

                    self.emergencias(data.Resultado);
                    drawDataTable("tabla-emergencias");

                    var table = $('#tabla-emergencias').DataTable();
                    table.order([0, 'desc']).draw();
                } else {
                    textoError.show();
                    console.log(data.Mensaje);
                    mensajeEmergenteError(data.Mensaje);
                }
            }, error: function (errorData) {
                textoCargando.hide();
                textoError.show();
                console.log(errorData);
            }
        });
    };
}

var listaVM = new EmergenciasListaVM();
ko.applyBindings(listaVM);

$(function () {
    listaVM.consultarEmergencias();
});