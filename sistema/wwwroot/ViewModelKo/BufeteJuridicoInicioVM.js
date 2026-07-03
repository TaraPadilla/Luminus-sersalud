var BufeteJuridicoInicioVM = function () {
    var self = this;

    self.redirigirContratos = function () {
        window.location.href = "/BufeteJuridicoContratos/Lista";
    };
    self.redirigirMarcas = function () {
        window.location.href = "/BufeteJuridicoMarcas/Lista";
    };
    self.redirigirSociedades = function () {
        window.location.href = "/BufeteJuridicoSociedades/Lista";
    };
};

var inicioVm = new BufeteJuridicoInicioVM();
ko.applyBindings(inicioVm);

$(document).ready(function () {

});