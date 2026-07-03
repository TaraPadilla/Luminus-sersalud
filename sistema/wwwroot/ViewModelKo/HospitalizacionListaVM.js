var HospitalizacionListaVM = function () {
    var self = this;
    self.hospitalizaciones = ko.observableArray();

    self.consultarHospitalizaciones = function (status) {
        let textoCargando = $("#texto-cargando-hospitalizaciones");
        let textoError = $("#texto-error-consultar-hospitalizaciones");
        textoCargando.show();
        textoError.hide();
        clearDataTable('tabla-hospitalizaciones');
        $.ajax({
            url: "/Hospitalizacion/ConsultarListaHospitalizaciones",
            method: "POST",
            data: { status: status },
            success: function (result) {
                textoCargando.hide();
                let data = JSON.parse(result);
                if (data.Exitoso) {
                    self.hospitalizaciones(data.Resultado);
                    drawDataTable('tabla-hospitalizaciones');
                } else {
                    textoError.show();
                }
            },
            error: function (resultError) {
                textoCargando.hide();
                textoError.show();
            }
        });
    };
}

var hospListaVm = new HospitalizacionListaVM();
ko.applyBindings(hospListaVm);

$(document).ready(function () {
    var status = window.location.href.includes("ListaEnCurso") ? 1 : 2;
    hospListaVm.consultarHospitalizaciones(status);
});
