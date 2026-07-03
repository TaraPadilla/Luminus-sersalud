var PacienteCambiarFaseVM = function () {
    var self = this;
    self.faseNueva = ko.observable();
    self.verRequisitosMantenimiento1 = ko.observable(false);
    self.aceptaTerminosMantenimiento1 = ko.observable(false);

    self.faseNueva.subscribe(function (value) {
        switch (value) {
            case "adelgazamiento":
                self.verRequisitosMantenimiento1(false);
                break;
            case "mantenimiento1":
                self.verRequisitosMantenimiento1(true);
                break;
            case "mantenimiento2":
                self.verRequisitosMantenimiento1(false);
                break;
            case "mantenimiento3":
                self.verRequisitosMantenimiento1(false);
                break;
        }
    });

    self.guardarCambioFase = function () {

        $.ajax({
            method: "POST",
            url: '/Pacientes/CambiarFase',
            data: {
                "PacienteId": $("#PacienteId").val(),
                "PacienteFaseTratamientoActual": $("#PacienteFaseTratamientoActual").val(),
                "FaseTratamientoNueva": self.faseNueva(),
                "FechaCambioFase": $("#FechaCambioFase").val(),
                "PesoAlIniciar": $("#PesoAlIniciar").val()
            },
            success: function (data) {
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
}

var cambioVM = new PacienteCambiarFaseVM();
ko.applyBindings(cambioVM);

$(document).ready(function () {

});