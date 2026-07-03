//$(function () {
//    setInterval(recargarPagina(), 15000);
//});
function terminarTurno(id) {
    var option = confirm("¿Está seguro que desea Finalizar el Turno?");

    if (option) {

        var data = { "id": parseInt(id) }

        $.ajax({
            url: "/Cita/functionterminarTurno/",
            data: data,
            type: 'POST',
            success: function (result) {
                window.location.href = '/Cita/Turnos';
            },
            error: function (error) {
                alert(error);
            }
        });
    }
}

function recargarPagina() {
    location.reload();
}


function Eliminar(id) {
    var option = confirm("¿Está seguro/a que desea eliminar este registro?");

    if (option) {

        var data = { "id": parseInt(id) }

        $.ajax({
            url: "/Cita/EliminarCita/",
            data: data,
            type: 'POST',
            success: function (result) {
                window.location.href = '/Cita/Lista';
            },
            error: function (error) {
                alert(error);
            }
        });
    }
}

function Finalizar(id) {
    var option = confirm("¿Está seguro/a que desea finalizar esta cita?");

    if (option) {

        var data = { "id": parseInt(id) }

        $.ajax({
            url: "/Cita/FinalizarCita/",
            data: data,
            type: 'POST',
            success: function (result) {
                window.location.href = '/Cita/Lista';
            },
            error: function (error) {
                alert(error);
            }
        });
    }
}