var cuentasVM = function () {
    var self = this;
    var model = {};

    //Ajustar el modelo
    self.getModel = function () {
        model = {
            "CuentaId": $("#CuentaId").val(),
            "Nombre": $("#Nombre").val(),
            "NumeroCuenta": $("#NumeroCuenta").val(),
            "BancoId": $("#BancoId").val(),
            "TipoCuentaId": $("#TipoCuentaId").val(),
        };
    };


    self.registrarCuenta = function () {
        if (confirm("¿Desea registrar esta cuenta?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Cuentas/Nuevo',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    debugger;
                    if (data.Exitoso) {
                        window.location.href = "/Cuentas/Lista";
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

    self.modificarCuenta = function () {
        if (confirm("¿Desea modificar esta cuenta?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Cuentas/ModificarCuenta',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Cuentas/Lista";
                    }
                    else {
                        hideLoading();
                        alert(data.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    console.log(data);
                    alert(data);
                }
            });
        }
    };

    self.cancelarRegistroCuenta = function () {
        if (confirm("¿Desea cancelar el registro de la cuenta?")) {
            window.location.href = "/Cuentas/Lista";
        }
    };
}

var cuentasVM = new cuentasVM();
ko.applyBindings(cuentasVM);

$(document).ready(function () {

});