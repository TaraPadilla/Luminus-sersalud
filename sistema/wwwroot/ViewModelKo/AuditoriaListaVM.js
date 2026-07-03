var InventarioVM = function () {
    var self = this;
    self.precioEditar = ko.observable();
    self.stockEditar = ko.observable();
};

function eliminarAuditoria(auditoriaId) {
    if (confirm("¿Desea eliminar esta auditoria?")) {
        showLoading();
        $.ajax({
            url: "/Auditoria/Eliminar",
            method: "POST",
            data: {
                auditoriaId: auditoriaId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.href = "/Auditoria/Lista";
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

var inventarioVm = new InventarioVM();
ko.applyBindings(inventarioVm);
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