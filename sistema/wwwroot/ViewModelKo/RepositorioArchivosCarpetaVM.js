var RepositorioArchivosCarpetaVM = function () {
    var self = this;
};

var repoArchivosCarpetaVm = new RepositorioArchivosCarpetaVM();
ko.applyBindings(repoArchivosCarpetaVm);

$(document).ready(function () {
    drawDataTable("tabla-archivos");
});