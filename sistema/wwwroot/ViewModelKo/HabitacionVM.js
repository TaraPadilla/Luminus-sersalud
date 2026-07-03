var HabitacionVM = function () {
    var model = {};
    var self = this;
    self.categorias = ko.observableArray();
    self.categoriaSeleccionadaId = ko.observable();
    self.estados = ko.observableArray();
    self.estadoSeleccionadoId = ko.observable();

    self.consultarCategorias = function () {
        showLoading();
        $.ajax({
            url: "/Habitaciones/ConsultarCategoriasExistentes",
            method: "POST",
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.categorias(data.Resultado);
                    let categoriaIdHabitacionExistente = $("#CategoriaId").val();
                    if (categoriaIdHabitacionExistente != null
                        && categoriaIdHabitacionExistente != undefined
                        && categoriaIdHabitacionExistente.trim() != '') {
                        self.categoriaSeleccionadaId(categoriaIdHabitacionExistente);
                    }

                } else {
                    alert(data.Mensaje);
                }
            },
            error: function (dataError) {
                hideLoading();
                alert(dataError);
                console.log(dataError);
            }
        })
    };
    self.consultarEstados = function () {
        showLoading();
        $.ajax({
            url: "/Habitaciones/ConsultarEstadosExistentes",
            method: "POST",
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.estados(data.Resultado);
                    let estadoIdHabitacionExistente = $("#EstadoId").val();
                    if (estadoIdHabitacionExistente != null
                        && estadoIdHabitacionExistente != undefined
                        && estadoIdHabitacionExistente.trim() != '') {
                        self.estadoSeleccionadoId(estadoIdHabitacionExistente);
                    }

                } else {
                    alert(data.Mensaje);
                }
            },
            error: function (dataError) {
                hideLoading();
                alert(dataError);
                console.log(dataError);
            }
        })
    };

    self.getModel = function () {
        model = {
            HabitacionId: $("#HabitacionId").val(),
            NumeroNombre: $("#NumeroNombre").val(),
            CategoriaId: self.categoriaSeleccionadaId(),
            EstadoId: self.estadoSeleccionadoId(),
            CapacidadPersonas: $("#CapacidadPersonas").val(),
            NumeroCamas: $("#CapacidadPersonas").val()
        };
    };
    self.registrarHabitacion = function () {
        if (confirm("¿Desea registrar esta habitacion?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Habitaciones/Nueva',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Habitaciones/Lista";
                    }
                    else {
                        hideLoading();
                        alert(data.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    alert(data);
                    console.log(data);
                }
            });
        }
    };
    self.cancelarRegistrarHabitacion = function () {
        if (confirm("¿Desea cancelar el registro de esta habitacion?")) {
            window.location.href = "/Habitaciones/Lista";
        }
    };
    self.modificarHabitacion = function () {
        if (confirm("¿Desea modificar esta habitacion?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Habitaciones/Modificar',
                data: model,
                success: function (dataResult) {
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/Habitaciones/Lista";
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
    self.cancelarModificarHabitacion = function () {
        if (confirm("¿Desea cancelar la edicion de esta habitacion?")) {
            window.location.href = "/Habitaciones/Lista";
        }
    };
}

var habitacionVm = new HabitacionVM();
ko.applyBindings(habitacionVm);

$(document).ready(function () {
    habitacionVm.consultarCategorias();
    habitacionVm.consultarEstados();
});