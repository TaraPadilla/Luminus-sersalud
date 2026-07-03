var CuentasPorCobrarDetallesVM = function () {
    var self = this;

    self.verInformacionPaciente = function () {
        window.open("/Pacientes/Informacion?pacienteId=" + $("#PacienteId").val(), "_blank");
    };
}

var detallesVm = new CuentasPorCobrarDetallesVM();
ko.applyBindings(detallesVm);

$(document).ready(function () {
    $("#tabs-datos-cuenta").tabs();
});