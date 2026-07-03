var HabitacionesListaVM = function () {
    var self = this;
}

var habitacionesListaVm = new HabitacionesListaVM();
ko.applyBindings(habitacionesListaVm);

$(document).ready(function () {
    drawDataTable("tabla-habitaciones");
});

function modificarHabitacion(habitacionId) {
    window.location.href = "/Habitaciones/Modificar?habitacionId=" + habitacionId;
}
function eliminarHabitacion(habitacionId) {
    if (confirm("¿Desea eliminar esta habitacion?")) {
        showLoading();
        $.ajax({
            url: "/Habitaciones/Eliminar",
            method: "POST",
            data: {
                habitacionId: habitacionId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (dataError) {
                hideLoading();
                alert(dataError);
                console.log(dataError);
            }
        });
    }
}