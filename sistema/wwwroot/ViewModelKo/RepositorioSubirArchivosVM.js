var RepositorioSubirArchivosVM = function () {
    var self = this;

    self.archivos = ko.observableArray();


};

var subirArchivoVm = new RepositorioSubirArchivosVM();
ko.applyBindings(subirArchivoVm);

$(document).ready(function () {
    //subirArchivoVm.drawTableArchivos();
});