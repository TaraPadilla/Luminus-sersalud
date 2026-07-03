var BancoVM = function () {
    var self = this;
    var model = {};

    //Ajustar el modelo
    self.getModel = function () {
        model = {
            "Id": $("#Id").val(),
            "Nombre": $("#Nombre").val(),
            "Direccion": $("#Direccion").val()
        };
    };


    self.registrarBanco = function () {
        if (confirm("¿Desea registrar este banco?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Banco/Nuevo',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    debugger;
                    if (data.Exitoso) {
                        window.location.href = "/Banco/Lista";
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

    self.modificarBanco = function () {
        if (confirm("¿Desea modificar este Banco?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Banco/ModificarBanco',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Banco/Lista";
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

    self.cancelarRegistroBanco = function () {
        if (confirm("¿Desea cancelar el registro del banco?")) {
            window.location.href = "/Banco/Lista";
        }
    };
}

var bancoVM = new BancoVM();
ko.applyBindings(bancoVM);

$(document).ready(function () {

});