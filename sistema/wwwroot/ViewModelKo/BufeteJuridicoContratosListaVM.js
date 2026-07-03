var BufeteJuridicoContratosListaVM = function () {
    var self = this;
};

var listaVm = new BufeteJuridicoContratosListaVM();
ko.applyBindings(listaVm);

$(document).ready(function () {
    drawDataTable('tabla-contratos');
});