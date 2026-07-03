var SucursalesListaVM = function () {
    var self = this;
}

var sucursalesVm = new SucursalesListaVM();
ko.applyBindings(sucursalesVm);

$(document).ready(function () {
    drawDataTable("tabla-sucursales");
});

function eliminarSucursal(sucursalId) {
    if (confirm("¿Desea eliminar esta sucursal?")) {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Sucursales/Eliminar',
            data: {
                sucursalId: sucursalId
            },
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