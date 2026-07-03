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
            CategoriaId: parseInt($("#CategoriaId").val(), 10) || 0,
            Nombre: $("#Nombre").val(),
            Tarifas: ko.toJS(self.tarifas)
        };
    };
    self.registrarCategoria = function () {
        if (confirm("Desea registrar esta categoria?")) {
            showLoading();
            self.getModel();
            $.ajax({
                url: "/Habitaciones/NuevaCategoria",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(model),
                processData: false,
                success: function (dataResult) {
                    hideLoading();
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
                    alert("Error al registrar la categoria.");
                }
            })
        }
    };
    self.modificarCategoria = function () {
        if (confirm("Desea modificar esta categoria?")) {
            showLoading();
            self.getModel();
            $.ajax({
                url: "/Habitaciones/ModificarCategoria",
                method: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(model),
                processData: false,
                success: function (dataResult) {
                    hideLoading();
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
                    alert("Error al modificar la categoria. Verifique que cada tarifa tenga nombre y valor.");
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
                    categoriaId: parseInt($("#CategoriaId").val(), 10)
                },
                success: function (dataResult) {
                    hideLoading();
                    var data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        var item = 1;
                        var tarifas = (data.Resultado || []).map(function (t) {
                            t.Item = item++;
                            return t;
                        });
                        self.tarifas(tarifas);
                        itemTarifa = item;
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

(function () {
    function wireHabitacionCategoriaButtons() {
        $(document).off("click.habitacionCat", "[data-habitacion-cat-accion]").on("click.habitacionCat", "[data-habitacion-cat-accion]", function (e) {
            e.preventDefault();
            var accion = $(this).attr("data-habitacion-cat-accion");
            var vm = window.categoriaHabitacionVm;
            if (!vm) return;
            if (accion === "guardar-nuevo" && typeof vm.registrarCategoria === "function") vm.registrarCategoria();
            else if (accion === "guardar-editar" && typeof vm.modificarCategoria === "function") vm.modificarCategoria();
            else if (accion === "agregar-tarifa" && typeof vm.agregarTarifa === "function") vm.agregarTarifa();
        });
    }

    function initCategoriaHabitacionVm() {
        if (typeof ko === "undefined") {
            console.error("Knockout no esta cargado. Verifique el orden de scripts en el layout.");
            return;
        }
        window.categoriaHabitacionVm = new CategoriaHabitacionVM();
        var root = document.getElementById("habitacion-categoria-ko-root");
        if (root) {
            ko.applyBindings(window.categoriaHabitacionVm, root);
        } else {
            ko.applyBindings(window.categoriaHabitacionVm);
        }
        wireHabitacionCategoriaButtons();
        window.categoriaHabitacionVm.consultarTarifas();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initCategoriaHabitacionVm);
    } else {
        initCategoriaHabitacionVm();
    }
})();
