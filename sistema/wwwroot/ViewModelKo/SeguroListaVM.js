var SeguroListaVM = function () {
    var self = this;
}

var listaVM = new SeguroListaVM();
ko.applyBindings(listaVM);

$(document).ready(function () {
    drawDataTable("tabla-seguros");
});

function inventarioMedicamentosSeguro(seguroId) {
    window.location.href = `/Productos/Inventario?AmbienteId=6&TipoProductoId=1&BodegaId=14&SeguroId=${seguroId}`;
}

function inventarioInsumosSeguro(seguroId) {
    window.location.href = `/Productos/Inventario?AmbienteId=6&TipoProductoId=2&BodegaId=14&SeguroId=${seguroId}`;
}

function modificarSeguro(seguroId) {
    window.location.href = "/Seguro/Modificar?seguroId=" + seguroId;
}
function eliminarSeguro(seguroId) {
    if (confirm("¿Desea eliminar este seguro?")) {
        showLoading();
        $.ajax({
            url: "/Seguro/EliminarSeguro",
            method: "POST",
            data: {
                seguroId: seguroId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.href = "/Seguro/Lista";
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