var HospitalizacionPaquetesDetallesVM = function () {
    let self = this;
}

var detallesVm = new HospitalizacionPaquetesDetallesVM();
ko.applyBindings(detallesVm);

$(document).ready(function () {
    drawDataTable("tabla-productos-paquete");
    drawDataTable("tabla-servicios-paquete");
    drawDataTable("tabla-laboratorios-paquete");
});