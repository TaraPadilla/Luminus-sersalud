var CuentaContableVM = function () {
    var self = this;
    var model = {};

    self.bancos = ko.observableArray();
    self.bancoSeleccionado = ko.observable();

    self.cuentasBanco = ko.observableArray();
    self.cuentaSeleccionado = ko.observable();

    self.categorias = ko.observableArray();
    self.categoriaSeleccionada = ko.observable();

    self.getModel = function () {
        var cuentaId = self.cuentaSeleccionado() ? self.cuentaSeleccionado().CuentaId : 0;
        var bancoId = self.bancoSeleccionado() ? self.bancoSeleccionado().Id : 0;
        var categoriaId = self.categoriaSeleccionada() ? self.categoriaSeleccionada().Id : 0;

        model = {
            "Id": $("#CuentaId").val(),
            "NombreCuenta": $("#NombreCuenta").val(),
            "Especificaciones": $("#Especificaciones").val(),
            "BancoId": bancoId,
            "CuentaId": cuentaId,
            "NomenclaturaId": $("#NomenclaturaId").val(),
            "CategoriaCuentaId": categoriaId,
        };
    };

    self.consultarCategorias = function () {
        $.ajax({
            method: "POST",
            url: '/CuentaContable/ConsultarCategorias',
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.categorias(data.Resultado);
                    self.seleccionarCategoriaInicial();
                } else {
                    alert(data.Mensaje);
                }
            },
            error: function (data) {
                alert(data.responseText || "Error al consultar categorias.");
            }
        });
    };

    self.seleccionarCategoriaInicial = function () {
        var categoriaId = parseInt($("#CategoriaCuentaId").val(), 10);
        if (!categoriaId) {
            return;
        }

        var categoria = ko.utils.arrayFirst(self.categorias(), function (item) {
            return item.Id === categoriaId;
        });

        if (categoria) {
            self.categoriaSeleccionada(categoria);
        }
    };

    self.consultarBancos = function () {
        if (typeof showLoading === "function") {
            showLoading();
        }

        $.ajax({
            method: "POST",
            url: '/CuentaContable/ConsultarBancos',
            success: function (dataResult) {
                if (typeof hideLoading === "function") {
                    hideLoading();
                }

                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.bancos(data.Resultado);
                    self.seleccionarBancoInicial();
                } else {
                    alert(data.Mensaje);
                }
            },
            error: function (data) {
                if (typeof hideLoading === "function") {
                    hideLoading();
                }
                alert(data.responseText || "Error al consultar bancos.");
            }
        });
    };

    self.seleccionarBancoInicial = function () {
        var bancoId = parseInt($("#BancoId").val(), 10);
        if (!bancoId) {
            return;
        }

        var banco = ko.utils.arrayFirst(self.bancos(), function (item) {
            return item.Id === bancoId;
        });

        if (banco) {
            self.bancoSeleccionado(banco);
        }
    };

    self.consultarCuentasBanco = function (bancoId) {
        if (!bancoId) {
            self.cuentasBanco([]);
            self.cuentaSeleccionado(undefined);
            return;
        }

        if (typeof showLoading === "function") {
            showLoading();
        }

        $.ajax({
            method: "POST",
            data: {
                "Id": bancoId
            },
            url: '/CuentaContable/ConsultarCuentasBanco',
            success: function (dataResult) {
                if (typeof hideLoading === "function") {
                    hideLoading();
                }

                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.cuentasBanco(data.Resultado || []);
                    self.seleccionarCuentaInicial();
                } else {
                    alert(data.Mensaje);
                }
            },
            error: function (data) {
                if (typeof hideLoading === "function") {
                    hideLoading();
                }
                alert(data.responseText || "Error al consultar cuentas del banco.");
            }
        });
    };

    self.seleccionarCuentaInicial = function () {
        var cuentaId = parseInt($("#CuentaId").val(), 10);
        if (!cuentaId) {
            self.cuentaSeleccionado(undefined);
            return;
        }

        var cuenta = ko.utils.arrayFirst(self.cuentasBanco(), function (item) {
            return item.CuentaId === cuentaId;
        });

        self.cuentaSeleccionado(cuenta || undefined);
    };

    self.registrarCuenta = function () {
        if (confirm("¿Desea registrar esta cuenta?")) {
            if (typeof showLoading === "function") {
                showLoading();
            }

            self.getModel();
            $.ajax({
                method: "POST",
                url: '/CuentaContable/Nuevo',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/CuentaContable/Lista";
                    } else {
                        if (typeof hideLoading === "function") {
                            hideLoading();
                        }
                        alert(data.Mensaje);
                    }
                },
                error: function (data) {
                    if (typeof hideLoading === "function") {
                        hideLoading();
                    }
                    alert("ERROR DE SERVIDOR: " + (data.responseText || data.statusText));
                }
            });
        }
    };

    self.modificarCuenta = function () {
        if (confirm("¿Desea modificar esta cuenta?")) {
            if (typeof showLoading === "function") {
                showLoading();
            }

            self.getModel();
            $.ajax({
                method: "POST",
                url: '/CuentaContable/ModificarCuenta',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/CuentaContable/Lista";
                    } else {
                        if (typeof hideLoading === "function") {
                            hideLoading();
                        }
                        alert(data.Mensaje);
                    }
                },
                error: function (data) {
                    if (typeof hideLoading === "function") {
                        hideLoading();
                    }
                    alert(data.responseText || data.statusText);
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
        if (!value || !value.Id) {
            self.cuentasBanco([]);
            self.cuentaSeleccionado(undefined);
            return;
        }

        self.consultarCuentasBanco(value.Id);
    });
};

var cuentaContableVM = new CuentaContableVM();
ko.applyBindings(cuentaContableVM);

$(document).ready(function () {
    cuentaContableVM.consultarCategorias();
    cuentaContableVM.consultarBancos();
});
