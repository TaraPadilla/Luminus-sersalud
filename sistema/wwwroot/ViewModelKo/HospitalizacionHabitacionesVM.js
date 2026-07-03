var HabitacionesVM = function () {
    var self = this;
}

var habitacionesVm = new HabitacionesVM();
ko.applyBindings(habitacionesVm);

$(document).ready(function () {
    drawDataTable("tabla-habitaciones-hospitalizacion");
});

function registrarHospitalizacion(habitacionId) {
    window.location.href = "/Hospitalizacion/Hospitalizar?habitacionId=" + habitacionId;
}

function detallesHospitalizacion(hospitalizacionId, citaId) {
    if (!hospitalizacionId || hospitalizacionId <= 0) {
        alert("No se encontró una hospitalización activa para esta habitación.");
        return;
    }
    var url = "/Hospitalizacion/Detalles?hospitalizacionId=" + hospitalizacionId;
    if (citaId && citaId > 0) {
        url += "&citaId=" + citaId;
    }
    window.location.href = url;
}

function finalizarHospitalizacion(hospitalizacionId) {
    if (confirm("¿Desea finalizar esta hospitalizacion?")) {
        showLoading();
        $.ajax({
            url: "/Hospitalizacion/FinalizarHospitalizacion",
            method: "POST",
            data: {
                hospitalizacionId: hospitalizacionId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    if (data.Pagada) {
                        window.location.reload();
                    } else {
                        // window.location.href = "/CuentasPorCobrar/Pagar?cuentaId=" + data.CuentaId;
                        window.location.reload();
                    }
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