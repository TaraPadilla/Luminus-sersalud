var ComprasListaTodasVM = function () {
    var self = this;
}

var listaComprasVm = new ComprasListaTodasVM();
ko.applyBindings(listaComprasVm);

$(document).ready(function () {
    drawDataTable("tabla-compras");
});