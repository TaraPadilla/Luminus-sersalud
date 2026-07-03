var HospitalizacionPaquetesDetallesVM = function () {
    let self = this;
}

var detallesVm = new HospitalizacionPaquetesDetallesVM();
ko.applyBindings(detallesVm);

function mostrarDialogoGuardar() {
    let confirmacion = confirm("¿Desea guardar los cambios?");
    if (confirmacion) {
        guardarStock();
    }
}
function cancelarRegistroAuditoria() {
    if (confirm("¿Desea volver al listado de las auditorias?")) {
        window.location.href = "/Auditoria/Lista";
    }
}
function cancelarModificacionAuditoria() {
    if (confirm("¿Desea cancelar la modificacion de la auditoria?")) {
        window.location.href = "/Auditoria/Lista";
    }
}


function guardarStock() {
    let table = $('#tabla-clinica-medicamentos').DataTable();
    let productosActualizados = new Map();
    table.rows().every(function () {
        let row = $(this.node());
        let stockInput = row.find('.stock-input');
        let stockIngresado = stockInput.val().trim();

        let idProducto = row.attr('data-id');

        if (stockIngresado !== "") {
            productosActualizados.set(idProducto, stockIngresado);
        }
    });
    if (productosActualizados.size > 0) {
        actualizarStockEnServidor(productosActualizados);
    }
}

function obtenerValorParametro(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
};

function actualizarStockEnServidor(productosActualizados) {
    var auditoriaId = obtenerValorParametro('AuditoriaId');
    console.log(auditoriaId);
    let datosAEnviar = {
        auditoriaId : auditoriaId,
        productos: Object.fromEntries(productosActualizados) // Asumiendo que productosActualizados es un Map o un objeto similar
    };
    $.ajax({
        type: "POST",
        url: "/Auditoria/ModificarStock",
        data: JSON.stringify(datosAEnviar), 
        contentType: "application/json",
        success: function (data) {
            console.log("Stock actualizado correctamente.");
            window.location.href = "/Auditoria/Lista";
        },
        error: function (xhr, status, error) {
            console.error("Error al actualizar el stock:", error);
        }
    });
}

$(document).ready(function () {
    var table = $("#tabla-clinica-medicamentos").DataTable({
        dom: 'Bfrtip',
        buttons: [
            'copy', 'excel', 'pdf'
        ],
        searching: true,
        ordering: true,
        paging: true,
        pageLength: 10,

        language: {
            search: "Buscar: ",
            lengthMenu: "Mostrar _MENU_ registros por página",
            zeroRecords: "No hay registros para mostrar",
            info: "Mostrando página _PAGE_ de _PAGES_",
            infoEmpty: "",
            infoFiltered: "(filtrado de _MAX_ registros totales)",
            paginate: {
                first: "Primero",
                last: "Último",
                previous: "Anterior",
                next: "Siguiente"
            }
        },
        //columnDefs: [
        //    {
        //        width: "150px",
        //        targets: [3]
        //    }
        //]
    });
    new $.fn.dataTable.Buttons(table, {
        buttons: [
            'copy', 'excel', 'pdf'
        ]
    });

});

