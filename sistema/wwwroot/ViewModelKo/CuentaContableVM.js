var cuentasVM = function () {
    var self = this;
    var model = {};
    self.bancos = ko.observableArray();
    self.bancoSeleccionado = ko.observable();

    self.cuentasBanco = ko.observableArray();
    self.cuentaSeleccionado = ko.observable();

    //Ajustar el modelo
    self.getModel = function () {
        var cuentaId = self.cuentaSeleccionado() ? self.cuentaSeleccionado().CuentaId : 0;

        model = {
            "Id": $("#CuentaId").val(),
            "NombreCuenta": $("#NombreCuenta").val(),
            "Especificaciones": $("#Especificaciones").val(),
            "BancoId": self.bancoSeleccionado().Id,
            "CuentaId": cuentaId,
            "NomenclaturaId": $("#NomenclaturaId").val(),
            "CategoriaCuentaId": $("#CategoriaCuentaId").val(),
        };
    };

    self.consultarBancos = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/CuentaContable/ConsultarBancos',
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.bancos(data.Resultado);
                }
                else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.consultarCuentasBanco = function (Id) {
        showLoading();
        $.ajax({
            method: "POST",
            data: {
                "Id": Id
            },
            url: '/CuentaContable/ConsultarCuentasBanco',
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.cuentasBanco(data.Resultado);

                }
                else {
                    //debugger;
                    hideLoading();
                    alert(data.Mensaje);
                    console.log("Error ajax");
                }
            },

            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });

    };
    self.registrarCuenta = function () {
        if (confirm("¿Desea registrar esta cuenta?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/CuentaContable/Nuevo',
                data: model,
                success: function (dataResult) {
                    debugger;
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/CuentaContable/Lista";
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
            debugger;
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/CuentaContable/ModificarCuenta',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/CuentaContable/Lista";
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
            window.location.href = "/CuentaContable/Lista";
        }
    };
    self.bancoSeleccionado.subscribe(function (value) {
        self.consultarCuentasBanco(value.Id);
    });
}

var cuentasVM = new cuentasVM();
ko.applyBindings(cuentasVM);

$(document).ready(function () {
    cuentasVM.consultarBancos();
});