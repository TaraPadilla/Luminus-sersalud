var ListaVentasVM = function () {
    var self = this;

    self.formaPago = ko.observableArray();
    self.formaPagoSeleccionada = ko.observable(); // Corregido typo de "Selccionada"
    self.formaPagoId = ko.observable();

    // El objeto "value" llegará con minúsculas desde el JSON de .NET
    self.formaPagoSeleccionada.subscribe(function (value) {
        if (value) {
            self.formaPagoId(value.id); // 'id' en minúscula
        } else {
            self.formaPagoId(null);
        }
    });

    self.consultarFormaPago = function () {
        debugger;
        showLoading();
        $.ajax({
            url: "/FormasPago/ConsultarFormasPago",
            method: "GET",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            cache: false,
            success: function (dataResult) {
                hideLoading(); // Te faltaba apagar el loading en el éxito
                
                // .NET convierte las propiedades a camelCase por defecto (exitoso, resultado, mensaje)
                if (dataResult.exitoso) {  
                    self.formaPago(dataResult.resultado);
                } else {
                    alert(dataResult.mensaje);
                }
            },
            error: function (dataError) {
                hideLoading();
                console.log(dataError);
                alert("Error de comunicación con el servidor.");
            }
        });
    };
}

var detallesVm = new ListaVentasVM();
ko.applyBindings(detallesVm);

$(document).ready(function () {
    detallesVm.consultarFormaPago();
});