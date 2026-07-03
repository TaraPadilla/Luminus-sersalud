function FarmaciaViewModel() {
    var self = this;

    // ObservableArray con todas las solicitudes
    self.solicitudes = ko.observableArray([]);
    self.estadoSolicitud = ko.observable();
    self.categoriasHabitacion = ko.observableArray([{ id: "", nombreCategoria: "Todas" }]);
    self.filtroCategoria = ko.observable("");

    self.solicitudesFiltradas = ko.computed(function () {
        return self.solicitudes();
    });


    // Ordenar por ID descendente
    self.solicitudesOrdenadas = ko.computed(function () {
        return self.solicitudesFiltradas().slice().sort(function (a, b) {
            return b.id - a.id;
        });
    });



    self.obtenerCategoriasHabitacion = function () {
        $.ajax({
            url: "/SolicitudMedicamentoNacional/GetAllCategoriasHabitacion",
            type: "GET",
            contentType: "application/json",
            success: function (response) {
                if (response && Array.isArray(response)) {

                    let categoriasTransformadas = response.map(c => ({
                        id: c.id,
                        nombreCategoria: c.nombreCategoria
                    }));

                    // ✅ Mantener "Todas" en la lista y agregar las categorías desde la API
                    self.categoriasHabitacion([{ id: "", nombreCategoria: "Todas" }].concat(categoriasTransformadas));
                    self.categoriasHabitacion.valueHasMutated();
                }
            },
            error: function (xhr, status, error) {
                console.error("⚠ Error al obtener las categorías de habitación:", error);
            }
        });
    };
    self.busquedaManual = false;

    self.obtenerSolicitudes = function () {
        self.busquedaManual = true; // Indica que se hizo una búsqueda manual

        const categoriaId = self.filtroCategoria(); // Obtener el ID de la categoría seleccionada
        const urlParams = new URLSearchParams(window.location.search);
        const hospitalizacionId = urlParams.get("hospitalizacionId");

        let url = "/SolicitudMedicamentoNacional/GetAll";
        let params = [];

        if (hospitalizacionId) {
            params.push(`hospitalizacionId=${hospitalizacionId}`);
        }
        if (categoriaId) {
            params.push(`categoriaId=${categoriaId}`);
        }

        if (params.length > 0) {
            url += `?${params.join("&")}`;
        }

        $.ajax({
            url: url,
            type: "GET",
            contentType: "application/json",
            success: function (response) {
                if (response && Array.isArray(response)) {
                    let solicitudesEnEspera = response.map(s => ({
                        id: s.id || 0,
                        hospitalizacionId: s.hospitalizacionId || "N/A",
                        nombreProducto: s.nombreProducto || "Desconocido",
                        cantidad: s.cantidad || 0,
                        precio: s.precio || 0,
                        viaAdministracion: s.viaAdministracion ?? "N/A",
                        frecuenciaAdministracion: s.frecuenciaAdministracion ?? "N/A",
                        estado: s.estado || "Pendiente",
                        fechaSolicitudFormatted: s.fechaSolicitudFormatted || "N/A",
                        usuarioSolicitanteNombre: s.usuarioSolicitanteNombre ?? "N/A",
                        habitacionNumero: s.hospitalizacion?.habitacion?.nombreNumeroHabitacion || "Sin asignar",
                        nombrePaciente: s.hospitalizacion?.paciente?.nombre || "Paciente no asignado",
                        categoriaHabitacionId: s.hospitalizacion?.habitacion?.categoriaHabitacionId || null
                    }));

                    self.solicitudes(solicitudesEnEspera);


                    setTimeout(function () {
                        self.actualizarDataTable(); // Solo actualizar, sin reiniciar
                    }, 500);
                } else {
                    console.warn("⚠ No se encontraron solicitudes.");
                    self.solicitudes([]);
                }

                self.busquedaManual = false;
            },
            error: function (xhr, status, error) {
                alert(`⚠ Error inesperado: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error}`);
            }
        });
    };

    self.actualizarDataTable = function () {
        let data = ko.toJS(self.solicitudes()); // 🔥 Convertir observables a datos normales
        
        console.log("Datos enviados a DataTable:", data); // 🔍 Verificar antes de actualizar la tabla
    
        if ($.fn.DataTable.isDataTable("#solicitudesTable")) {
            console.log("🛑 Destruyendo DataTable antes de actualizar...");
            $("#solicitudesTable").DataTable().destroy();
        }
    
        self.inicializarDataTable(); // 🔥 Vuelve a inicializar con los datos nuevos
        
        let table = $("#solicitudesTable").DataTable();
        table.clear().rows.add(data).draw(false);
        
        console.log("✅ DataTable actualizado correctamente.");
    };
    


    self.inicializarDataTable = function () {
        console.log("Inicializando DataTable...");
        
        if (!$.fn.DataTable.isDataTable("#solicitudesTable")) {
            $("#solicitudesTable").DataTable({
                "paging": true,
                "pageLength": 8,
                "lengthMenu": [5, 8, 15, 25],
                "searching": true,
                "ordering": true,
                "deferRender": true,
                "processing": true,
                "info": true,
                "autoWidth": false,
                "responsive": true,
                "order": [[0, "desc"]],
                "data": [], // 🔥 Inicializa con un array vacío
                "columns": [
                    { "data": "id" },
                    { "data": "hospitalizacionId" },
                    { "data": "habitacionNumero" },
                    { "data": "nombrePaciente" },
                    { "data": "nombreProducto" },
                    { "data": "cantidad" },
                    { "data": "viaAdministracion" },
                    { "data": "frecuenciaAdministracion" },
                    { "data": "estado" },
                    { "data": "fechaSolicitudFormatted" },
                    { "data": "usuarioSolicitanteNombre" },
                    {
                        "data": null, 
                        "orderable": false, 
                        "render": function (data, type, row) {
                            // 🔥 Renderiza los botones dinámicamente
                            let aprobarBtn = `<button class="btn btn-success btn-sm" onclick="detallesVm.despacharSolicitud(${row.id})">✅ Aprobar</button>`;
                            let rechazarBtn = `<button class="btn btn-danger btn-sm" onclick="detallesVm.rechazarSolicitud(${row.id})">❌ Rechazar</button>`;
                            
                            if (row.estado === "En espera") {
                                return `${aprobarBtn} ${rechazarBtn}`;
                            } else if (row.estado === "Rechazado") {
                                return `<span class="badge">❌ Rechazado</span>`;
                            } else if (row.estado === "Despachado") {
                                return `<span class="badge">✅ Despachado</span>`;
                            }
                        }
                    }
                ],
                "language": {
                    "lengthMenu": "Mostrar _MENU_ solicitudes por página",
                    "zeroRecords": "No se encontraron solicitudes",
                    "info": "Mostrando página _PAGE_ de _PAGES_",
                    "infoEmpty": "No hay registros disponibles",
                    "infoFiltered": "(filtrado de _MAX_ registros totales)",
                    "search": "Buscar:",
                    "paginate": {
                        "first": "Primero",
                        "last": "Último",
                        "next": "Siguiente",
                        "previous": "Anterior"
                    }
                }
            });
        }
    };
    




    // ✅ Función para marcar una solicitud como "Despachado"
    self.despacharSolicitud = function (id) {
        $.ajax({
            url: `/SolicitudMedicamentoNacional/MarkAsDispensed/${id}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(false), // `false` significa que NO es rechazada (se despacha)
            success: function (response) {
                if (response.exitoso) {
                    // alert(`✅ Solicitud #${id} marcada como "Despachado" correctamente.`);
                    self.obtenerSolicitudes(); // Recargar la lista de solicitudes
                    self.actualizarEstadoSolicitud(id, "Despachado");
                } else {
                    alert(`❌ Error al despachar: ${response.mensaje}`);
                }
            },
            error: function (xhr, status, error) {
                alert(`⚠ Error inesperado: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error}`);
            }
        });
    };
    

    // ✅ Función para marcar una solicitud como "Rechazado"
    self.rechazarSolicitud = function (id) {
        $.ajax({
            url: `/SolicitudMedicamentoNacional/MarkAsDispensed/${id}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(true), // `true` significa que es rechazada
            success: function (response) {
                if (response.exitoso) {
                    // alert(`❌ Solicitud #${id} rechazada correctamente.`);
                    self.obtenerSolicitudes(); // Recargar la lista de solicitudes
                    self.actualizarEstadoSolicitud(id, "Rechazado");
                } else {
                    alert(`❌ Error al rechazar: ${response.mensaje}`);
                }
            },
            error: function (xhr, status, error) {
                alert(`⚠ Error inesperado: ${xhr.responseJSON ? xhr.responseJSON.Mensaje : error}`);
            }
        });
    };
    

    // ✅ Función para actualizar el estado en la tabla sin recargar la página
    self.actualizarEstadoSolicitud = function (id, nuevoEstado) {
        let solicitud = self.solicitudes().find(s => s.id === id);
        if (solicitud) {
            solicitud.estado(nuevoEstado); // `estado` es un `ko.observable`, por eso se usa `estado(nuevoEstado)`
            self.solicitudes.valueHasMutated(); // Notificar cambios a Knockout.js
        }
    };



    setInterval(function () {
        self.obtenerSolicitudes();
    }, 30000);

}

var detallesVm = new FarmaciaViewModel();
ko.applyBindings(detallesVm);

$(document).ready(function () {
    console.log("🔄 Iniciando carga de solicitudes...");

    detallesVm.obtenerCategoriasHabitacion();

    detallesVm.obtenerSolicitudes(); // Obtiene los datos antes de inicializar la tabla

    // 🔥 Esperar a que se obtengan los datos antes de inicializar DataTables
    let checkDataInterval = setInterval(() => {
        let data = ko.toJS(detallesVm.solicitudes());

        console.log("🔍 Verificando datos antes de inicializar DataTable...", data);

        if (data.length > 0) {
            console.log("✅ Datos obtenidos, inicializando DataTable...");

            detallesVm.inicializarDataTable();
            detallesVm.actualizarDataTable();

            clearInterval(checkDataInterval); // 🔥 Detener la verificación cuando los datos ya estén disponibles
        }
    }, 1000); // 🔄 Revisar cada segundo hasta que haya datos
});
