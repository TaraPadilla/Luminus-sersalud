var SucursalVM = function () {
    var self = this;
    var model = {};

    self.validateModel = function () {
        let nombre = $("#Nombre").val();
        let direccion = $("#Direccion").val();
        let horario = $("#Horario").val();
        if (nombre == undefined || nombre == null || nombre.trim() == '') {
            alert("Proporcione el nombre de la sucursal");
            return false;
        }
        if (direccion == undefined || direccion == null || direccion.trim() == '') {
            alert("Proporcione la direccion de la sucursal");
            return false;
        }
        if (horario == undefined || horario == null || horario.trim() == '') {
            alert("Proporcione el horario de la sucursal");
            return false;
        }
        return true;
    };
    self.getModel = function () {
        model = {
            "SucursalId": $("#SucursalId").val(),
            "Nombre": $("#Nombre").val(),
            "Direccion": $("#Direccion").val(),
            "Horario": $("#Horario").val()
        };
    };
    self.registrarSucursal = function () {
        if (self.validateModel()) {
            if (confirm("¿Desea registrar esta sucursal?")) {
                showLoading();
                self.getModel();
                $.ajax({
                    method: "POST",
                    url: '/Sucursales/Nueva',
                    data: model,
                    success: function (dataResult) {
                        let data = JSON.parse(dataResult);
                        if (data.Exitoso) {
                            window.location.href = "/Sucursales";
                        }
                        else {
                            hideLoading();
                            alert(data.Mensaje);
                        }
                    }
                });
            }
        }
    };
    self.editarSucursal = function () {
        if (self.validateModel()) {
            if (confirm("¿Desea editar esta sucursal?")) {
                showLoading();
                self.getModel();
                $.ajax({
                    method: "POST",
                    url: '/Sucursales/Modificar',
                    data: model,
                    success: function (dataResult) {
                        let data = JSON.parse(dataResult);
                        if (data.Exitoso) {
                            window.location.href = "/Sucursales";
                        }
                        else {
                            hideLoading();
                            alert(data.Mensaje);
                        }
                    }
                });
            }
        }
    };
    self.cancelarRegistrarSucursal = function () {
        if (confirm("¿Desea descartar los cambios?")) {
            window.location.href = "/Sucursales";
        }
    };
    self.cancelarEditarSucursal = function () {
        if (confirm("¿Desea descartar los cambios?")) {
            window.location.href = "/Sucursales";
        }
    };
}

var sucursalVm = new SucursalVM();
ko.applyBindings(sucursalVm);

$(document).ready(function () {

});