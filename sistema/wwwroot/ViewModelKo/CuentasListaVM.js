var BancoListaVM = function () {
    var self = this;
}

var listaVM = new BancoListaVM();
ko.applyBindings(listaVM);

$(document).ready(function () {
    if ($.fn.DataTable.isDataTable("#tabla-cuenta")) {
        $("#tabla-cuenta").DataTable().destroy();
    }
    drawDataTable("tabla-cuenta");
});

function modificarCuenta(cuentaId) {
    window.location.href = "/Cuentas/Modificar?cuentaId=" + cuentaId;
}
function eliminarCuenta(cuentaId) {
    if (confirm("¿Desea eliminar esta cuenta?")) {
        showLoading();
        $.ajax({
            url: "/Cuentas/EliminarCuenta",
            method: "POST",
            data: {
                cuentaId: cuentaId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.href = "/Cuentas/Lista";
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