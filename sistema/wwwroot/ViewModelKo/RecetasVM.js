var RecetaVM = function () {
    var self = this;
    var model = {};

    //Ajustar el modelo
    self.getModel = function () {
        model = {
            "Id": $("#Id").val(),
            "NombreReceta": $("#NombreReceta").val(),
            "Ingredientes": $("#Ingredientes").val(),
            "Cantidad": $("#Cantidad").val(),
            "Indicaciones": $("#Indicaciones").val(),

        };
    };


    self.registrarReceta = function () {
        if (confirm("¿Desea registrar esta receta?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Receta/Nuevo',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Receta/Lista";
                    }
                    else {
                        hideLoading();
                        alert(data.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    console.log(data.error);
                    alert("ERROR DE SERVIDOR: " + data.error);
                }
            });
        }
    };

    self.modificarReceta = function () {
        if (confirm("¿Desea modificar esta receta?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Receta/ModificarReceta',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Receta/Lista";
                    }
                    else {
                        hideLoading();
                        alert(data.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    console.log(data);
                    alert(data);
                }
            });
        }
    };

    self.cancelarRegistroServicio = function () {
        if (confirm("¿Desea cancelar el registro de la receta?")) {
            window.location.href = "/Receta/Lista";
        }
    };
}

var recetaVM = new RecetaVM();
ko.applyBindings(recetaVM);

$(document).ready(function () {

});