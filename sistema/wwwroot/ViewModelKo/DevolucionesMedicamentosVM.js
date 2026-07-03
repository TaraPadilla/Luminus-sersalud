function DevolucionesViewModel() {
    var self = this;

    // ObservableArray con todas las devoluciones
    self.devoluciones = ko.observableArray([]);
    self.categoriasHabitacion = ko.observableArray([{ id: "", nombreCategoria: "Todas" }]);
    self.filtroCategoria = ko.observable("");

    // Computeds para filtrar y ordenar (por ahora simplemente retorna todas las devoluciones)
    self.devolucionesFiltradas = ko.computed(function () {
        return self.devoluciones();
    });

    // Ordenar por ID descendente
    self.devolucionesOrdenadas = ko.computed(function () {
        return self.devolucionesFiltradas().slice().sort(function (a, b) {
            return b.id - a.id;
        });
    });

    // Obtener categorías de habitación
    self.obtenerCategoriasHabitacion = function () {
        $.ajax({
            url: "/DevolucionMedicamento/GetAllCategoriasHabitacion",
            type: "GET",
            contentType: "application/json",
            success: function (response) {
                if (response && Array.isArray(response)) {
                    let categoriasTransformadas = response.map(function (c) {
                        return {
                            id: c.id,
                            nombreCategoria: c.nombreCategoria
                        };
                    });
                    // Mantener "Todas" en la lista y agregar las categorías desde la API
                    self.categoriasHabitacion([{ id: "", nombreCategoria: "Todas" }].concat(categoriasTransformadas));
                    self.categoriasHabitacion.valueHasMutated();
                }
            },
            error: function (xhr, status, error) {
                console.error("⚠ Error al obtener las categorías de habitación:", error);
            }
        });
    };

    // Obtener devoluciones (con posibilidad de filtrar por hospitalización y categoría)
    self.obtenerDevoluciones = function () {
        const categoriaId = self.filtroCategoria();
        const urlParams = new URLSearchParams(window.location.search);
        const hospitalizacionId = urlParams.get("hospitalizacionId");

        let url = "/DevolucionMedicamento/GetAll";
        let params = [];

        if (hospitalizacionId) {
            params.push("hospitalizacionId=" + hospitalizacionId);
        }
        if (categoriaId) {
            params.push("categoriaId=" + categoriaId);
        }

        if (params.length > 0) {
            url += "?" + params.join("&");
        }

        $.ajax({
            url: url,
            type: "GET",
            contentType: "application/json",
            success: function (response) {
                if (response && Array.isArray(response)) {
                    let devolucionesProcesadas = response.map(function (d) {
                        return {
                            id: d.id || 0,
                            hospitalizacionId: d.hospitalizacionId || "N/A",
                            hospitalizacionProductoAplicacionId: d.hospitalizacionProductoAplicacionId || "",  // ← AÑADIDO
                            nombreProducto: d.nombreProducto || "Desconocido",
                            cantidadDevuelta: d.cantidadDevuelta || 0,
                            motivoDevolucion: d.motivoDevolucion || "N/A",
                            tipoProducto: d.tipoProducto || "N/A",
                            estado: d.estado || "En espera",
                            fechaSolicitudFormatted: d.fechaSolicitudFormatted || "N/A",
                            usuarioSolicitanteNombre: d.usuarioSolicitanteNombre || "N/A",
                            fechaAprobacionFormatted: d.fechaAprobacionFormatted || "No procesada",
                            usuarioAprobadorNombre: d.usuarioAprobadorNombre || "N/A"
                        };
                    });
                    self.devoluciones(devolucionesProcesadas);

                    setTimeout(function () {
                        self.actualizarDataTable();
                    }, 500);
                } else {
                    console.warn("⚠ No se encontraron devoluciones.");
                    self.devoluciones([]);
                }
            },
            error: function (xhr, status, error) {
                alert(`⚠ Error inesperado: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error}`);
            }
        });
    };

    // Actualizar la DataTable con los datos actuales
    self.actualizarDataTable = function () {
        let data = ko.toJS(self.devoluciones());
        console.log("Datos enviados a DataTable:", data);
        if ($.fn.DataTable.isDataTable("#devolucionesTable")) {
            console.log("🛑 Destruyendo DataTable antes de actualizar...");
            $("#devolucionesTable").DataTable().destroy();
        }
        self.inicializarDataTable();
        let table = $("#devolucionesTable").DataTable();
        table.clear().rows.add(data).draw(false);
        console.log("✅ DataTable actualizado correctamente.");
    };

    // Inicializar la DataTable
    self.inicializarDataTable = function () {
        console.log("Inicializando DataTable...");
        if (!$.fn.DataTable.isDataTable("#devolucionesTable")) {
            $("#devolucionesTable").DataTable({
                paging: true,
                pageLength: 8,
                lengthMenu: [5, 8, 15, 25],
                searching: true,
                ordering: true,
                deferRender: true,
                processing: true,
                info: true,
                autoWidth: false,
                responsive: true,
                order: [[0, "desc"]],
                data: [],
                columns: [
                    { data: "id" },
                    { data: "hospitalizacionId" },
                    { data: "hospitalizacionProductoAplicacionId"},
                    { data: "nombreProducto" },
                    { data: "cantidadDevuelta" },
                    { data: "motivoDevolucion" },
                    { data: "tipoProducto" },
                    { data: "estado" },
                    { data: "fechaSolicitudFormatted" },
                    { data: "usuarioSolicitanteNombre" },
                    { data: "fechaAprobacionFormatted" },
                    { data: "usuarioAprobadorNombre" },
                    {
                        data: null,
                        orderable: false,
                        render: function (data, type, row) {
                            let aprobarBtn = `<button class="btn btn-success btn-sm" onclick="detallesVm.aprobarDevolucion(${row.id})">✅ Aprobar</button>`;
                            let rechazarBtn = `<button class="btn btn-danger btn-sm" onclick="detallesVm.rechazarDevolucion(${row.id})">❌ Rechazar</button>`;
                            if (row.estado === "En espera") {
                                return `${aprobarBtn} ${rechazarBtn}`;
                            } else if (row.estado === "Aprobada") {
                                return `<span class="badge badge-success">✅ Aprobada</span>`;
                            } else if (row.estado === "Rechazada") {
                                return `<span class="badge badge-danger">❌ Rechazada</span>`;
                            }
                        }
                    }
                ],
                language: {
                    lengthMenu: "Mostrar _MENU_ devoluciones por página",
                    zeroRecords: "No se encontraron devoluciones",
                    info: "Mostrando página _PAGE_ de _PAGES_",
                    infoEmpty: "No hay registros disponibles",
                    infoFiltered: "(filtrado de _MAX_ registros totales)",
                    search: "Buscar:",
                    paginate: {
                        first: "Primero",
                        last: "Último",
                        next: "Siguiente",
                        previous: "Anterior"
                    }
                }
            });
        }
    };

    // Función para aprobar una devolución
    self.aprobarDevolucion = function (id) {
        $.ajax({
            url: `/DevolucionMedicamento/ProcesarDevolucion/${id}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(false), // false: no es rechazada, es aprobada
            success: function (response) {
                if (response.exitoso) {
                    alert(`✅ La devolución #${id} ha sido aprobada.`);
                    self.obtenerDevoluciones();
                } else {
                    alert(`❌ Error al aprobar: ${response.mensaje}`);
                }
            },
            error: function (xhr, status, error) {
                alert(`⚠ Error inesperado: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error}`);
            }
        });
    };

    // Función para rechazar una devolución
    self.rechazarDevolucion = function (id) {
        $.ajax({
            url: `/DevolucionMedicamento/ProcesarDevolucion/${id}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(true), // true: se rechaza
            success: function (response) {
                if (response.exitoso) {
                    alert(`❌ La devolución #${id} ha sido rechazada.`);
                    self.obtenerDevoluciones();
                } else {
                    alert(`❌ Error al rechazar: ${response.mensaje}`);
                }
            },
            error: function (xhr, status, error) {
                alert(`⚠ Error inesperado: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error}`);
            }
        });
    };

    // Actualización automática de la lista cada 30 segundos
    setInterval(function () {
        self.obtenerDevoluciones();
    }, 30000);
}

var detallesVm = new DevolucionesViewModel();
ko.applyBindings(detallesVm);

$(document).ready(function () {
    console.log("🔄 Iniciando carga de devoluciones...");
    detallesVm.obtenerCategoriasHabitacion();
    detallesVm.obtenerDevoluciones();

    // Esperar a que se obtengan datos antes de inicializar DataTable
    let checkDataInterval = setInterval(() => {
        let data = ko.toJS(detallesVm.devoluciones());
        console.log("🔍 Verificando datos antes de inicializar DataTable...", data);
        if (data.length > 0) {
            console.log("✅ Datos obtenidos, inicializando DataTable...");
            detallesVm.inicializarDataTable();
            detallesVm.actualizarDataTable();
            clearInterval(checkDataInterval);
        }
    }, 1000);
});
