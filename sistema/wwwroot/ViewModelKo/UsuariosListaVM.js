var UsuariosListaVM = function () {
    var self = this;
}

var usuariosVm = new UsuariosListaVM();
ko.applyBindings(usuariosVm);

$(function () {
    drawDataTable("tabla-usuarios");
});