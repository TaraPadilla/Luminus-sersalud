var PersonasListaVM = function () {
    var self = this;
}

var listaPersonasVM = new PersonasListaVM();
ko.applyBindings(listaPersonasVM);

$(function () {
    drawDataTable("tabla-pacientes");
});