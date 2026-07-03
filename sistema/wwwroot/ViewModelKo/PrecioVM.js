var PrecioVM = function () {
    var self = this;
    var model = {};

    self.getModel = function () {
        model = {
            "PrecioId": $("#PrecioId").val(),
            "NombrePrecio": $("#NombrePrecio").val(),
        };
    };
    self.registrarPrecio = function () {
        if (confirm("¿Desea registrar este precio?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Precios/Nuevo',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Precios/Lista";
                    }
                    else {
                        hideLoading();
                        alert(data.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    console.log(data.error);
                    alert("ERROR DE SERVIDOR: " + data.error);
                }
            });
        }
    };
    self.editarPrecio = function () {
        if (confirm("¿Desea editar este precio?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Precios/Modificar',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Precios/Lista";
                    }
                    else {
                        hideLoading();
                        alert(data.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    console.log(data.error);
                    alert("ERROR DE SERVIDOR: " + data.error);
                }
            });
        }
    };
    //self.cancelarRegistrarGrabacion = function () {
    //    if (confirm("¿Desea cancelar el registro de esta grabación?")) {
    //        window.location.href = "/Grabaciones/Lista";
    //    }
    //};
    //self.cancelarEditarGrabacion = function () {
    //    if (confirm("¿Desea cancelar la edición de esta grabación?")) {
    //        window.location.href = "/Grabaciones/Lista";
    //    }
    //};
}

var precioVm = new PrecioVM();
ko.applyBindings(precioVm);

$(document).ready(function () {

});