var CategoriaVM = function () {
    var self = this;
    var model = {};

    //Ajustar el modelo
    self.getModel = function () {
        model = {
            "Id": $("#Id").val(),
            "Nombre": $("#Nombre").val(),
            "Especificacion": $("#Especificacion").val()
        };
    };


    self.registrarCategoria = function () {
        if (confirm("¿Desea registrar esta categoria?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/CategoriasCuentaContable/Nuevo',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/CategoriasCuentaContable/Lista"; 
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

    self.modificarCategoria = function () {
        if (confirm("¿Desea modificar esta categoria?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/CategoriasCuentaContable/ModificarCategoria',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/CategoriasCuentaContable/Lista";
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

    self.cancelarRegistroCategoria = function () {
        if (confirm("¿Desea cancelar el registro del banco?")) {
            window.location.href = "/CategoriasCuentaContable/Lista";
        }
    };
}

var categoria = new CategoriaVM();
ko.applyBindings(categoria);

$(document).ready(function () {

});