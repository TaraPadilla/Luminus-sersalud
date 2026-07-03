var PersonasListaVM = function () {
    var self = this;
    self.cuentasPagadas = ko.observableArray();

    self.consultarCuentasPagadas = function () {

        self.clearTableCuentasPagadas();

        $.ajax({
            method: "POST",
            url: '/CuentasPorCobrar/ConsultarCuentasPagadas',
            success: function (data, textStatus) {
                console.log(data);
                if (data.exitoso) {
                    self.cuentasPagadas(data.resultado);
                    self.drawTableCuentasPagadas();
                }
                else
                    alert(data.mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };

    self.clearTableCuentasPagadas = function () {
        var table = $("#tabla-cuentas-pagadas").DataTable();
        table.clear().draw();

        $("#tabla-cuentas-pagadas").dataTable().fnDestroy();
    };
    self.drawTableCuentasPagadas = function () {
        $("#tabla-cuentas-pagadas").DataTable({
            searching: true,
            ordering: true,
            paging: true,
            order: [[0, 'desc']], 
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
    };

}

var listaPersonasVM = new PersonasListaVM();
ko.applyBindings(listaPersonasVM);

$(document).ready(function () {
    listaPersonasVM.consultarCuentasPagadas();
});