var CategoriaHabitacionVM = function () {
    let itemTarifa = 1;
    let model = {};
    var self = this;
    self.tarifas = ko.observableArray();

    self.agregarTarifa = function () {
        let tarifa = {
            Item: itemTarifa,
            Id: null,
            Nombre: "",
            Lunes: false,
            Martes: false,
            Miercoles: false,
            Jueves: false,
            Viernes: false,
            Sabado: false,
            Domingo: false,
            FechaEspecial: false,
            Fecha: null,
            Valor: 0,
            Nueva: true
        };
        self.tarifas.push(tarifa);
        itemTarifa++;
    };
    self.quitarTarifa = function (value) {
        $(self.tarifas()).each(function (idx, tarifa) {
            if (value.Item == tarifa.Item) {
                self.tarifas.splice(idx, 1);
            }
        })
    };

    self.getModel = function () {
        model = {
            CategoriaId: $("#CategoriaId").val(),
            Nombre: $("#Nombre").val(),
            Tarifas: self.tarifas()
        };
    };
    self.registrarCategoria = function () {
        if (confirm("¿Desea registrar esta categoria?")) {
            showLoading();
            self.getModel();
            $.ajax({
                url: "/Habitaciones/NuevaCategoria",
                method: "POST",
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Habitaciones/ListaCategorias";
                    } else {
                        alert(data.Mensaje);
                    }
                },
                error: function (dataError) {
                    hideLoading();
                    console.log(dataError);
                }
            })
        }
    };
    self.modificarCategoria = function () {
        if (confirm("¿Desea modificar esta categoria?")) {
            showLoading();
            self.getModel();
            $.ajax({
                url: "/Habitaciones/ModificarCategoria",
                method: "POST",
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Habitaciones/ListaCategorias";
                    } else {
                        alert(data.Mensaje);
                    }
                },
                error: function (dataError) {
                    hideLoading();
                    console.log(dataError);
                }
            });
        }
    };
    self.consultarTarifas = function () {
        if ($("#CategoriaId").val() != undefined
            && $("#CategoriaId").val() != null
            && $("#CategoriaId").val().trim() != '') {
            showLoading();
            $.ajax({
                url: "/Habitaciones/ConsultarTarifasCategoria",
                method: "POST",
                data: {
                    categoriaId: parseInt($("#CategoriaId").val())
                },
                success: function (dataResult) {
                    hideLoading();
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        self.tarifas(data.Resultado);
                    } else {
                        alert(data.Mensaje);
                    }
                },
                error: function (dataError) {
                    hideLoading();
                    console.log(dataError);
                }
            });
        }
    };
}

var categoriaVm = new CategoriaHabitacionVM();
ko.applyBindings(categoriaVm);

$(document).ready(function () {
    categoriaVm.consultarTarifas();
});