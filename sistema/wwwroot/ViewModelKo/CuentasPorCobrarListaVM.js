var CuentasPorCobrarListaVM = function () {
    var self = this;
    self.cuentasPorCobrar = ko.observableArray();
    self.proveedores = ko.observableArray();

    self.mostrarTablaCuentasPendientes = ko.observable(true);
    self.mostrarTablaProveedores = ko.observable(false);

    self.mostrarCuentasPendientes = function () {
        self.mostrarTablaCuentasPendientes(true);
        self.mostrarTablaProveedores(false);
    };

    self.mostrarProveedores = function () {
        self.mostrarTablaCuentasPendientes(false);
        self.mostrarTablaProveedores(true);
    };

    self.consultarCuentasPorCobrar = function () {

        self.clearTableCuentasPorCobrar();

        $.ajax({
            method: "POST",
            url: '/CuentasPorCobrar/ConsultarCuentasPorCobrar',
            success: function (data, textStatus) {
                if (data.exitoso) {

                    self.cuentasPorCobrar(data.resultado);
                    self.drawTableCuentasPorCobrar();
                }
                else
                    alert(data.mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };

    self.consultarProveedores = function () {

        self.clearTableProveedores();

        $.ajax({
            method: "POST",
            url: '/Compra/ListaComprados',
            success: function (data, textStatus) {
                if (data.exitoso) {
                    console.log(data.resultado);
                    self.proveedores(data.resultado);
                    self.drawTableProveedores();
                }
                else
                    alert(data.mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };



    self.formatDate = function (date) {
        return date ? moment(date).format('ll') : '-';
    };

    self.modificarCuenta = function (value) {
        window.location.href = "/CuentasPorCobrar/Modificar?cuentaId=" + value.id;
    };

    self.clearTableCuentasPorCobrar = function () {
        var table = $("#tabla-cuentas-por-cobrar").DataTable();
        table.clear().draw();

        $("#tabla-cuentas-por-cobrar").dataTable().fnDestroy();
    };
    self.drawTableCuentasPorCobrar = function () {
        $("#tabla-cuentas-por-cobrar").DataTable({
            searching: true,
            ordering: true,
            paging: true,
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
    self.clearTableProveedores = function () {
        var table = $("#tabla-proveedores").DataTable();
        table.clear().draw();

        $("#tabla-proveedores").dataTable().fnDestroy();
    };
    self.drawTableProveedores = function () {
        $("#tabla-proveedores").DataTable({
            searching: true,
            ordering: false,
            paging: true,
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

    self.verDetallesCuenta = function (value) {
        window.location.href = "/CuentasPorCobrar/VerDetallesCuenta?cuentaId=" + value.id;
    };
    self.pagarCuenta = function (value) {
        window.location.href = "/CuentasPorCobrar/Pagar?cuentaId=" + value.id;
    };
    self.verDetallesProveedores = function (value) {
        window.location.href = "/Compra/Modificar?compraId=" + value.id;
    };

}

var listaPersonasVM = new CuentasPorCobrarListaVM();
ko.applyBindings(listaPersonasVM);

$(function () {
    listaPersonasVM.consultarCuentasPorCobrar();
    listaPersonasVM.consultarProveedores();
});