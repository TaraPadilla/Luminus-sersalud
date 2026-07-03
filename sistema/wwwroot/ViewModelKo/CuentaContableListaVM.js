var BancoListaVM = function () {
    var self = this;
}

var listaVM = new BancoListaVM();
ko.applyBindings(listaVM);

$(document).ready(function () {
    drawDataTable("tabla-cuenta");
});

function modificarCuenta(cuentaId) {
    debugger;
    window.location.href = "/CuentaContable/Modificar?cuentaId=" + cuentaId;
}
function eliminarCuenta(cuentaId) {
    if (confirm("¿Desea eliminar esta cuenta?")) {
        showLoading();
        $.ajax({
            url: "/CuentaContable/EliminarCuenta",
            method: "POST",
            data: {
                cuentaId: cuentaId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.href = "/CuentaContable/Lista";
                } else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (dataError) {
                hideLoading();
                console.log(dataError);
                alert(dataError);
            }
        });
    }
}