var DevolucionVM = function () {
    var self = this;

    self.productosDisponibles = ko.observableArray();
    self.productosSolicitados = ko.observableArray();

    self.productosTrasladados = self.productosSolicitados;

    self.searchQuery = ko.observable("");
    self.currentPage = ko.observable(1);
    self.pageSize = ko.observable(9);

    self.proximoDevolucion = ko.observable("");

    self.departamento = ko.observable("");
    self.unidad = ko.observable("");
    self.firmaUrl = ko.observable("");

    self.proveedores = ko.observableArray([]);
    self.proveedorSeleccionado = ko.observable("");
    self.proveedorTelefono = ko.observable("");
    self.proveedorDireccion = ko.observable("");

    self.proveedorSeleccionado.subscribe(function (id) {
        var proveedor = ko.utils.arrayFirst(self.proveedores(), function (p) {
            return String(p.Id) === String(id);
        });
        self.proveedorTelefono(proveedor ? (proveedor.Telefono || "") : "");
        self.proveedorDireccion(proveedor ? (proveedor.Direccion || "") : "");
    });

    let model = {};
    let itemProducto = 1;

    // =========================
    // Utilidades
    // =========================
    self.normalizarProductoSolicitado = function (item) {
        if (!ko.isObservable(item.CantidadTrasladada)) {
            item.CantidadTrasladada = ko.observable(item.CantidadTrasladada != null ? item.CantidadTrasladada : 0);
        }
        if (!ko.isObservable(item.CantidadDespachada)) {
            item.CantidadDespachada = ko.observable(item.CantidadDespachada != null ? item.CantidadDespachada : null);
        }

        item.errorStock = ko.observable(false);

        item.CantidadTrasladada.subscribe(function (nuevaCantidad) {
            var stockDisponible = Number(item.CantidadExistente || 0);
            var cantidadPedida = Number(nuevaCantidad || 0);
            item.errorStock(cantidadPedida > stockDisponible);
        });

        if (typeof item.Diferencia === "undefined") {
            item.Diferencia = ko.pureComputed(function () {
                var solicitado = Number(item.CantidadTrasladada());
                var despachadoRaw = item.CantidadDespachada();
                if (despachadoRaw === null || despachadoRaw === undefined || despachadoRaw === "") return null;
                var despachado = Number(despachadoRaw);
                if (isNaN(despachado)) return null;
                return solicitado - despachado;
            });
        }

        return item;
    };

    // =========================
    // Paginación / filtro
    // =========================
    self.filteredProductosSolicitados = ko.computed(function () {
        let filter = (self.searchQuery() || "").toLowerCase();
        let sorted = self.productosSolicitados().slice().sort(function (a, b) {
            let fa = a.FechaTraslado ? new Date(a.FechaTraslado).getTime() : 0;
            let fb = b.FechaTraslado ? new Date(b.FechaTraslado).getTime() : 0;
            return fb - fa;
        });
        if (!filter) return sorted;
        return ko.utils.arrayFilter(sorted, function (item) {
            return (item.ProductoNombre || "").toLowerCase().includes(filter);
        });
    });

    self.filteredProductosTrasladados = self.filteredProductosSolicitados;

    self.paginatedProductosSolicitados = ko.computed(function () {
        let startIndex = (self.currentPage() - 1) * self.pageSize();
        return self.filteredProductosSolicitados().slice(startIndex, startIndex + self.pageSize());
    });

    self.paginatedProductosTrasladados = self.paginatedProductosSolicitados;

    self.totalPages = ko.computed(function () {
        return Math.ceil(self.filteredProductosSolicitados().length / self.pageSize());
    });

    self.nextPage = function () {
        if (self.currentPage() < self.totalPages()) self.currentPage(self.currentPage() + 1);
    };

    self.prevPage = function () {
        if (self.currentPage() > 1) self.currentPage(self.currentPage() - 1);
    };

    // =========================
    // Consultas al servidor
    // =========================
    self.consultarProductosDisponibles = function () {
        showLoading();
        clearDataTable("tabla-traslado");
        self.productosDisponibles([]);

        $.ajax({
            url: "/Devolucion/ConsultarProductosInventario",
            method: "POST",
            data: { bodegaOrigenId: $("#BodegaOrigenId").val() },
            success: function (dataResult) {
                hideLoading();
                let data = (typeof dataResult === "string") ? JSON.parse(dataResult) : dataResult;
                if (data.Exitoso) {
                    $(data.Resultado).each(function (idx, vl) {
                        vl.CantidadTrasladada = ko.observable(0);
                        self.productosDisponibles.push(vl);
                    });
                    drawDataTable("tabla-traslado");
                } else {
                    alert(data.Mensaje);
                }
            },
            error: function (dataerror) {
                hideLoading();
                alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
            }
        });
    };

    self.consultarUltimoRegistro = function () {
        $.ajax({
            url: "/Devolucion/ConsultarUltimoRegistro",
            method: "GET",
            success: function (dataResult) {
                let data = (typeof dataResult === "string") ? JSON.parse(dataResult) : dataResult;
                if (data.Exitoso) {
                    self.proximoDevolucion(data.Resultado.ProximoDevolucion);

                    var anio = new Date().getFullYear();
                    var proximo = data.Resultado.ProximoDevolucion || 1; 
                    var numero = String(proximo).padStart(3, '0');
                    $("#NumeroOficio").val(numero + "-" + anio);
                } else {
                    var anio = new Date().getFullYear();
                    $("#NumeroOficio").val("001-" + anio);
                }
            },
            error: function (dataerror) {
                var anio = new Date().getFullYear();
                $("#NumeroOficio").val("001-" + anio);
                console.error("Error al consultar último registro:", dataerror);
            }
        });
    };
    self.consultarProveedores = function () {
        $.ajax({
            url: "/Devolucion/ConsultarProveedores",
            method: "GET",
            success: function (dataResult) {
                var data = (typeof dataResult === "string") ? JSON.parse(dataResult) : dataResult;
                if (data.Exitoso) {
                    self.proveedores(data.Resultado);
                } else {
                    console.error("Error al consultar proveedores:", data.Mensaje);
                }
            },
            error: function (dataerror) {
                console.error("Error al consultar proveedores:", dataerror);
            }
        });
    };

    self.consultarDataUsuario = function () {
        $.ajax({
            url: "/Devolucion/ConsultarDataUsuario",
            method: "POST",
            success: function (dataResult) {
                let data = (typeof dataResult === "string") ? JSON.parse(dataResult) : dataResult;
                if (data.Exitoso && data.Data) {
                    self.departamento(data.Data.DepartamentoNombre);
                    self.unidad(data.Data.UnidadNombre);
                    self.firmaUrl(data.Data.Firma ? data.Data.Firma : "");
                } else {
                    console.warn("La consulta fue exitosa pero no trajo datos:", data.Mensaje);
                }
            },
            error: function (dataerror) {
                console.error("Error al consultar la data del usuario:", dataerror);
            }
        });
    };

    // =========================
    // Firma
    // =========================
    self.guardarFirmaDigital = function () {
        const canvas = document.getElementById('canvasFirma');
        if (!canvas) {
            alert("No se encontró el área de firma.");
            return;
        }
        const dataUrl = canvas.toDataURL("image/png");
        self.actualizarFirmaEnBD(dataUrl);
    };

    self.actualizarFirmaEnBD = function (base64Image) {
        showLoading();
        $.ajax({
            url: "/Devolucion/ActualizarFirmaUsuario",
            method: "POST",
            data: { firmaBase64: base64Image },
            success: function (data) {
                hideLoading();
                if (data.success) {
                    self.firmaUrl(data.rutaFirma);
                    toastr.success("Firma actualizada correctamente.");
                } else {
                    alert("Error al guardar: " + (data.message || "Error desconocido"));
                }
            },
            error: function (err) {
                hideLoading();
                alert("Ocurrió un error crítico al comunicarse con el servidor.");
            }
        });
    };

    self.previsualizarFirma = function (data, event) {
        var archivo = event.target.files[0];
        if (archivo) {
            var lector = new FileReader();
            lector.onload = function (e) {
                self.actualizarFirmaEnBD(e.target.result);
            };
            lector.readAsDataURL(archivo);
        }
    };

    // =========================
    // Acciones de productos
    // =========================
    self.anniadirProductoSolicitud = function (producto) {
        let agregado = false;

        self.productosSolicitados().forEach(function (vl) {
            if (vl.ProductoInventarioId === producto.ProductoInventarioId) {
                let actual = Number(vl.CantidadTrasladada());
                let nueva = actual + 1;
                if (producto.CantidadExistente != null && nueva > producto.CantidadExistente) {
                    mensajeEmergente("Cantidad no válida");
                } else {
                    vl.CantidadTrasladada(nueva);
                }
                agregado = true;
            }
        });

        if (!agregado) {
            producto.Item = itemProducto++;
            producto.CantidadTrasladada = ko.observable(1);
            producto.CantidadDespachada = ko.observable(null);
            producto.Nuevo = true;
            producto.FechaTraslado = new Date().toISOString();
            self.normalizarProductoSolicitado(producto);
            self.productosSolicitados.push(producto);
        }
    };

    self.anniadirProductoTraslado = self.anniadirProductoSolicitud;

    self.quitarProductoSolicitud = function (producto) {
        self.productosSolicitados.remove(producto);
    };

    self.quitarProductoTraslado = self.quitarProductoSolicitud;

    // =========================
    // Barcode modal
    // =========================
    self.escapeHtml = function (str) {
        str = (str === null || str === undefined) ? "" : String(str);
        return str.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#039;");
    };

    self.val = function (x) {
        return ko.isObservable(x) ? x() : x;
    };

    self.getProductoIdTexto = function (p) {
        if (typeof p.ProductoId !== "undefined" && p.ProductoId !== null && p.ProductoId !== "")
            return String(p.ProductoId);
        if (typeof p.ProductoInventarioId !== "undefined" && p.ProductoInventarioId !== null)
            return String(p.ProductoInventarioId);
        return "";
    };

    self.getSerialBarcode = function (p) {
        var id = self.getProductoIdTexto(p);
        var nombre = (p.ProductoNombre || "").trim();
        var desp = self.val(p.CantidadDespachada);
        var despTxt = (desp === null || desp === undefined || desp === "") ? "" : String(desp);
        return id + "|" + nombre + "|" + despTxt;
    };

    self.dibujarCodigoEnModal = function (producto) {
        var serial = self.getSerialBarcode(producto);
        var $modal = $('#modalBarcode');
        $modal.appendTo("body");
        $modal.modal('show');
        $modal.on('shown.bs.modal', function () {
            try {
                JsBarcode("#barcodeModalElement", serial, {
                    format: "CODE128", displayValue: false,
                    lineColor: "#000", background: "#fff",
                    height: 80, width: 2, margin: 10
                });
                $("#barcodeTextLabel").text(serial);
                var svg = document.getElementById("barcodeModalElement");
                if (svg) svg.setAttribute("preserveAspectRatio", "xMidYMid meet");
            } catch (e) {
                console.error("Error al dibujar:", e);
            }
            $modal.off('shown.bs.modal');
        });
    };

    // =========================
    // Validación
    // =========================
    self.validateModel = function () {
        if (!self.proveedorSeleccionado() || self.proveedorSeleccionado() === "") {
            alert("Seleccione un proveedor");
            return false;
        }
        var $origen = $("#BodegaOrigenId");
        if ($origen.find("option[value!='']").length === 0) {
            alert("No hay bodegas de origen configuradas. Verifique sucursales y bodegas en el sistema.");
            return false;
        }
        var bodegaOrigenId = $origen.val();
        if (bodegaOrigenId === null || bodegaOrigenId === undefined || String(bodegaOrigenId).trim() === "") {
            alert("Seleccione una bodega de origen");
            return false;
        }
        if (self.productosSolicitados().length === 0) {
            alert("Debe agregar al menos un producto a devolver");
            return false;
        }
        return true;
    };

    // =========================
    // Payload
    // =========================
    self.getModel = function () {
        model = {
            RequisicionId: $("#RequisicionId").val() || null,
            EstadoRequisicion: $("#EstadoRequisicion").val(),

            Direccion: $("#Direccion").val(),
            Departamento: $("#Departamento").val(),
            UnidadSeccion: $("#UnidadSeccion").val(),
            Otros: $("#Otros").val(),

            NumeroDevolucion: $("#NumeroDevolucion").val() || null,
            FechaSolicitud: $("#FechaSolicitud").val() || null,

            ProveedorId: self.proveedorSeleccionado() || null,

            NombreSolicitante: $("#NombreSolicitante").val(),
            BodegaOrigenId: $("#BodegaOrigenId").val(),
            Observaciones: $("#Observaciones").val(),
            NumeroOficio: parseInt($("#NumeroOficio").val().split('-')[0]) || null

        };
    };

    self.buildPostData = function () {
        self.getModel();

        var postData = {};
        Object.keys(model).forEach(function (k) {
            postData[k] = model[k];
        });

        var productos = ko.toJS(self.productosSolicitados()) || [];
        for (var i = 0; i < productos.length; i++) {
            var p = productos[i] || {};

            var cantDevuelta = p.CantidadTrasladada;
            if (cantDevuelta === "" || cantDevuelta === undefined || cantDevuelta === null) cantDevuelta = 0;

            postData["Productos[" + i + "].ProductoInventarioId"] = p.ProductoInventarioId;
            postData["Productos[" + i + "].CantidadTrasladada"] = cantDevuelta;
        }

        return postData;
    };

    // =========================
    // Guardar devolución
    // =========================
    self.guardarDevolucion = function () {
        if (!self.validateModel()) return;
        if (!confirm("¿Desea registrar esta devolución?")) return;

        showLoading();

        var postData = self.buildPostData();

        $.ajax({
            url: "/Devolucion/Guardar",
            method: "POST",
            data: postData,
            success: function (dataResult) {
                var data = (typeof dataResult === "string") ? JSON.parse(dataResult) : dataResult;
                if (data.Exitoso) {
                    toastr.success("Devolución registrada correctamente.");
                    setTimeout(function () {
                        window.location.href = "/Devolucion/Lista";
                    }, 1500);
                } else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (dataerror) {
                hideLoading();
                alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
            }
        });
    };

    // Alias defensivos
    self.guardarRequisicion = self.guardarDevolucion;
    self.guardarTraslado = self.guardarDevolucion;

    self.cancelarRegistroRequisicion = function () {
        if (confirm("¿Desea cancelar el registro?")) {
            window.location.href = "/Devolucion/Lista";
        }
    };
    self.cancelarRegistroTraslado = self.cancelarRegistroRequisicion;
};


var requisicionVm = new DevolucionVM();
ko.applyBindings(requisicionVm);


$(document).ready(function () {

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });

    $('.collapse').collapse();

    requisicionVm.consultarProductosDisponibles();
    requisicionVm.consultarUltimoRegistro();
    requisicionVm.consultarDataUsuario();
    requisicionVm.consultarProveedores();

    $("#BodegaOrigenId").change(function () {
        requisicionVm.consultarProductosDisponibles();
    });
});


// =========================
// Canvas firma
// =========================
let isDrawing = false;
let ctx = null;

function initCanvas() {
    const canvas = document.getElementById('canvasFirma');
    if (!canvas) return;

    ctx = canvas.getContext('2d');
    ctx.strokeStyle = "#000000";
    ctx.lineWidth = 2;
    ctx.lineCap = "round";
    ctx.lineJoin = "round";

    canvas.onmousedown = (e) => {
        isDrawing = true;
        ctx.beginPath();
        const rect = canvas.getBoundingClientRect();
        ctx.moveTo(e.clientX - rect.left, e.clientY - rect.top);
    };

    canvas.onmousemove = (e) => {
        if (isDrawing) {
            const rect = canvas.getBoundingClientRect();
            ctx.lineTo(e.clientX - rect.left, e.clientY - rect.top);
            ctx.stroke();
        }
    };

    window.onmouseup = () => {
        if (isDrawing) { ctx.closePath(); isDrawing = false; }
    };

    canvas.addEventListener("touchstart", function (e) {
        e.preventDefault();
        var touch = e.touches[0];
        canvas.dispatchEvent(new MouseEvent("mousedown", { clientX: touch.clientX, clientY: touch.clientY }));
    }, false);

    canvas.addEventListener("touchmove", function (e) {
        e.preventDefault();
        var touch = e.touches[0];
        canvas.dispatchEvent(new MouseEvent("mousemove", { clientX: touch.clientX, clientY: touch.clientY }));
    }, false);

    canvas.addEventListener("touchend", function () {
        isDrawing = false;
    }, false);
}

window.limpiarCanvas = function (idCanvas) {
    const canvas = document.getElementById(idCanvas);
    if (canvas && ctx) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.beginPath();
        isDrawing = false;
    }
};