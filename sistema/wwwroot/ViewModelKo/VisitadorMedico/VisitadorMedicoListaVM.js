$(document).ready(function () {
    var table = $("#tabla-registro-visitadores-medicos").DataTable({
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
        }
    });

    table.buttons().container().appendTo('#tabla-registro-visitadores-medicos_wrapper .col-md-6:eq(0)');
});
