var HistoricoProductosVM = function () {

    self.movimientosProductos = ko.observableArray();

    self.modalAbiertoFiltrarProductos = ko.observable(false);
    self.modalAbiertoFiltrarTipoProducto = ko.observable(false);
    self.showModalFiltrarProductos = function () {
        if (!self.modalAbiertoFiltrarProductos()) {

            //Consultar productos
            drawDataTable("tabla-filtrar-productos");
            self.modalFiltrarProductos();
            self.modalAbiertoFiltrarProductos(true);
        } else {
            self.modalFiltrarProductos();
        }
    };
    self.modalFiltrarProductos = function () {
        $("#mdl-filtrar-productos").dialog({
            modal: true,
            width: 800
        });
    }
    self.showModalFiltrarTipoProducto = function () {
        if (!self.modalAbiertoFiltrarTipoProducto()) {

            //Consultar productos
            drawDataTable("tabla-filtrar-tipo-producto");
            self.modalFiltrarTipoProductos();
            self.modalAbiertoFiltrarTipoProducto(true);
        } else {
            self.modalFiltrarTipoProductos();
        }
    };
    self.modalFiltrarTipoProductos = function () {
        $("#mdl-filtrar-tipo-producto").dialog({
            modal: true,
            width: 500
        });
    }

    self.consultarHistorico = function () {
        showLoading();
        $.ajax({
            url: "/Productos/ConsultarHistoricoProductos",
            method: "POST",
            success: function (result) {
                hideLoading();
                let data = JSON.parse(result);
                if (data.Exitoso) {
                    self.movimientosProductos(data.Resultado);
                }
            }
        });
    };

    self.generarReportePDF = function () {
        window.open("/CrearPDF/HistoricoProductosPDF?productosIds=34,45,23,12", "_blank");
    };
    self.generarReporteExcel = function () {
        window.open("/Productos/HistoricoProductosExcel", "_blank");
    };
}

var historicoVm = new HistoricoProductosVM();
ko.applyBindings(historicoVm);

$(function () {

});