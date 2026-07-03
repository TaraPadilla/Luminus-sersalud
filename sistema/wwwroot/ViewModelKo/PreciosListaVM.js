var PreciosListaVM = function () {
    var self = this;
    self.precios = ko.observableArray();

    self.consultarPrecios = function () {
        showLoading();
        clearDataTable("tabla-precios");

        $.ajax({
            method: "POST",
            url: '/Precios/ConsultarPrecios',
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.precios(data.Resultado);
                    drawDataTable("tabla-precios");
                    hideLoading();
                }
                else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (dataError) {
                alert(dataError);
            }
        });
    };
    self.editarPrecio = function (value) {
        window.location.href = "/Precios/Modificar?precioId=" + value.PrecioId;
    };
    self.eliminarPrecio = function (value) {
        if (confirm("¿Desea eliminar este precio?")) {
            showLoading();
            window.location.href = "/Precios/Eliminar?precioId=" + value.PrecioId;
        }
    };
}

var listaPreciosVM = new PreciosListaVM();
ko.applyBindings(listaPreciosVM);

$(document).ready(function () {
    listaPreciosVM.consultarPrecios();
});