var CuentaModificarVM = function () {
    var self = this;

    //Consultas dashboard
    self.modificarCuenta = function () {

        if (confirm("¿Desea modificar los datos de la cuenta?")) {
            $.ajax({
                method: "POST",
                url: '/CuentasPorCobrar/Modificar',
                data: {
                    "CuentaId": $("#CuentaId").val(),
                    "Valor": $("#Valor").val(),
                    "Observaciones": $("#Observaciones").val()
                },
                success: function (data, textStatus) {
                    if (data.exitoso) {
                        window.location.href = "/CuentasPorCobrar/Pendientes";
                    }
                    else
                        alert(data.mensaje);
                },
                error: function (data) {
                    alert(data.error);
                }
            });
        }
    };
}

var modificarVm = new CuentaModificarVM();
ko.applyBindings(modificarVm);

$(document).ready(function () {

});