var RepositorioCarpetasVM = function () {
    var self = this;

    self.carpetas = ko.observableArray();

    self.consultarCarpetas = function () {
        showLoading();
        clearDataTable("tabla-carpetas");
        var carpetaSociedades = {
            Item: 1,
            Id: 1,
            NombreCarpeta: "Sociedades",
            Ruta: "/Archivos/Sociedades"
        };
        var carpetaMarcas = {
            Item: 2,
            Id: 2,
            NombreCarpeta: "Marcas",
            Ruta: "/Archivos/Marcas"
        };
        var carpetaContratos = {
            Item: 3,
            Id: 3,
            NombreCarpeta: "Contratos",
            Ruta: "/Archivos/Contratos"
        };
        self.carpetas.push(carpetaSociedades);
        self.carpetas.push(carpetaMarcas);
        self.carpetas.push(carpetaContratos);
        drawDataTable("tabla-carpetas");
        hideLoading();
    };
};

var repositorioCarpetasVm = new RepositorioCarpetasVM();
ko.applyBindings(repositorioCarpetasVm);

$(document).ready(function () {
    repositorioCarpetasVm.consultarCarpetas();
});