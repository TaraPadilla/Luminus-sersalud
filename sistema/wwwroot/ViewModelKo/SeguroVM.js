var SeguroVM = function () {
    var self = this;
    var model = {};

    //Ajustar el modelo
    self.getModel = function () {
        var nombreSeguro = $("#nombreSeguro").val();
        model = {
            "Id": $("#Id").val(),
            "Nombre": nombreSeguro,
            "Direccion": $("#Direccion").val(),
            "Telefono": $("#Telefono").val(),
            "Web": $("#Web").val(),

        };
    };

    // Función para obtener todos los seguros
    self.obtenerSeguros = function () {
        $.ajax({
            method: "GET",
            url: '/Seguro/ObtenerTodos', // URL para obtener todos los seguros
            success: function (dataResult) {
                // Suponiendo que dataResult es un array de objetos seguros
                var seguros = JSON.parse(dataResult);
                console.log("SeguroVM.js: seguro: ", seguros)
                var segurosObj = {};
                // Construir un objeto con ID como clave y Nombre como valor
                seguros.forEach(function (seguro) {
                    segurosObj[seguro.Id] = seguro.Nombre;
                });
                console.log(segurosObj); // Aquí tienes un objeto con todos los seguros
                // Retornar el objeto con los seguros
                return segurosObj;
            },
            error: function (data) {
                console.log(data.error);
                alert("ERROR DE SERVIDOR: " + data.error);
            }
        });
    };

    self.registrarSeguro = function () {
        var nombreSeguro = $("#nombreSeguro").val();
        if (!nombreSeguro) {
            alert("El nombre del seguro es obligatorio.");
            return;
        }

        if (confirm("¿Desea registrar este seguro?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Seguro/Nuevo',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Seguro/Lista";
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

    self.modificarSeguro = function () {
        if (confirm("�Desea modificar este Seguro?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Seguro/ModificarSeguro',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Seguro/Lista";
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

    self.cancelarRegistroSeguro = function () {
        if (confirm("�Desea cancelar el registro del Seguro?")) {
            window.location.href = "/Seguro/Lista";
        }
    };
}

var seguroVM = new SeguroVM();
ko.applyBindings(seguroVM);

$(document).ready(function () {

});