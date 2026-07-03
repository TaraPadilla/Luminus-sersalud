// Contexto (dónde estamos)
// - Paso 1/2: movimos Requisicion.cs y RequisicionEstado.cs a sistema.Models y ajustamos usings.
// - Paso 3: RequisicionViewModel ya usa RequisicionId y EstadoRequisicion; la vista Nuevo.cshtml ya usa esos campos.
// - Paso 4 (este archivo): ajustar KO para que deje de hablar de "Traslado" (payload/hidden fields) y envíe el
//   encabezado + firmas + bodegas + observaciones + productos. Sin tocar aún BD/repository nuevo.

var RequisicionVM = function () {
    var self = this;

    self.productosDisponibles = ko.observableArray();

    // Mantengo el nombre "productosSolicitados" internamente...
    self.productosSolicitados = ko.observableArray();

    // Alias (defensivo) por si existe algún binding viejo
    self.productosTrasladados = self.productosSolicitados;

    self.searchQuery = ko.observable("");

    self.currentPage = ko.observable(1);
    self.pageSize = ko.observable(9);

    self.proximoRequisicion = ko.observable("");
    self.proximoNumeroOrden = ko.observable("");

    self.departamento = ko.observable("");
    self.unidad = ko.observable("");

    self.firmaUrl = ko.observable("");

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

            if (cantidadPedida > stockDisponible) {
                item.errorStock(true); 
            } else {
                item.errorStock(false);
            }
        });

        if (typeof item.Diferencia === "undefined") {
            item.Diferencia = ko.pureComputed(function () {
                var solicitado = Number(item.CantidadTrasladada());
                var despachadoRaw = item.CantidadDespachada();

                if (despachadoRaw === null || despachadoRaw === undefined || despachadoRaw === "") {
                    return null;
                }

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

        // Mejor práctica KO: slice() para no mutar el observable.
        let sortedArray = self.productosSolicitados().slice().sort(function (a, b) {
            let fa = a.FechaTraslado ? new Date(a.FechaTraslado).getTime() : 0;
            let fb = b.FechaTraslado ? new Date(b.FechaTraslado).getTime() : 0;
            return fb - fa;
        });

        if (!filter) return sortedArray;

        return ko.utils.arrayFilter(sortedArray, function (item) {
            let nombre = (item.ProductoNombre || "").toLowerCase();
            return nombre.includes(filter);
        });
    });

    // Alias defensivo (vista vieja)
    self.filteredProductosTrasladados = self.filteredProductosSolicitados;

    self.paginatedProductosSolicitados = ko.computed(function () {
        let startIndex = (self.currentPage() - 1) * self.pageSize();
        return self.filteredProductosSolicitados().slice(startIndex, startIndex + self.pageSize());
    });

    // Alias defensivo (vista vieja)
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
    // Consultas
    // =========================
    self.consultarProductosDisponibles = function () {
        showLoading();
        clearDataTable("tabla-traslado");
        self.productosDisponibles([]);

        $.ajax({
            url: "/Requisicion/ConsultarProductosInventario",
            method: "POST",
            data: {
                bodegaOrigenId: $("#BodegaOrigenId").val()
            },
            success: function (dataResult) {
                hideLoading();
                let data = JSON.parse(dataResult);

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
            url: "/Requisicion/ConsultarUltimoRegistro",
            method: "GET",
            success: function (dataResult) {
                let data = JSON.parse(dataResult);

                if (data.Exitoso) {
                    self.proximoNumeroOrden(data.Resultado.ProximoNumeroOrden);
                    self.proximoRequisicion(data.Resultado.ProximoRequisicion);
                }
            },
            error: function (dataerror) {
                console.error("Error al consultar último registro:", dataerror);
            }
        });
    };

    self.guardarFirmaDigital = function () {
        const canvas = document.getElementById('canvasFirma');
        const dataUrl = canvas.toDataURL("image/png");
        self.actualizarFirmaEnBD(dataUrl);
    };

    self.actualizarFirmaEnBD = function (base64Image) {
        showLoading();
        $.ajax({
            url: "/Requisicion/ActualizarFirmaUsuario",
            method: "POST",
            data: {
                firmaBase64: base64Image
            },
            success: function (data) {
                hideLoading();
                
                if (data.success) {
                    self.firmaUrl(data.rutaFirma);

                    if (typeof self.metodoFirmaSeleccionado === "function") {
                        self.metodoFirmaSeleccionado(origen);
                    }

                    toastr.success("Firma actualizada correctamente.");
                } else {
                    alert("Error al guardar: " + (data.message || "Error desconocido"));
                }
            },
            error: function (err) {
                hideLoading();
                console.error("Error en la petición AJAX:", err);
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


    self.consultarDataUsuario = function () {
        $.ajax({
            url: "/Requisicion/ConsultarDataUsuario",
            method: "POST",
            success: function (dataResult) {
                let data = (typeof dataResult === "string") ? JSON.parse(dataResult) : dataResult;

                if (data.Exitoso && data.Data) {
                    self.departamento(data.Data.DepartamentoNombre);
                    self.unidad(data.Data.UnidadNombre);

                    if (data.Data.Firma) {
                        self.firmaUrl(data.Data.Firma);
                    } else {
                        self.firmaUrl("");
                    }
                } else {
                    console.warn("La consulta fue exitosa pero no trajo datos o falló:", data.Mensaje);
                }
            },
            error: function (dataerror) {
                console.error("Error al consultar la data del usuario:", dataerror);
            }
        });
    };

    // =========================
    // Acciones UI
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
            // Nota: mantenemos "FechaTraslado" por compatibilidad con el modelo actual del backend (aún no migrado).
            producto.FechaTraslado = new Date().toISOString();

            self.normalizarProductoSolicitado(producto);
            self.productosSolicitados.push(producto);
        }
    };

    self.quitarProductoSolicitud = function (producto) {
        self.productosSolicitados.remove(producto);
    };

    // Alias defensivos (por si queda algún binding viejo en KO)
    self.anniadirProductoTraslado = self.anniadirProductoSolicitud;
    self.quitarProductoTraslado = self.quitarProductoSolicitud;

    // ======================================================================
    // IMPRESIÓN: helpers comunes (ID, serial, escape)
    // ======================================================================
    self.getTagTexto = function (p) {
        return (typeof p.TagRfid !== 'undefined' ? p.TagRfid :
            (typeof p.tag_rfid !== 'undefined' ? p.tag_rfid :
                (typeof p.Etiqueta !== 'undefined' ? p.Etiqueta : '')));
    };

    self.val = function (x) {
        return ko.isObservable(x) ? x() : x;
    };

    self.escapeHtml = function (str) {
        str = (str === null || str === undefined) ? "" : String(str);
        return str
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    };

    self.getProductoIdTexto = function (p) {
        if (typeof p.ProductoId !== "undefined" && p.ProductoId !== null && p.ProductoId !== "")
            return String(p.ProductoId);

        if (typeof p.ProductoInventarioId !== "undefined" && p.ProductoInventarioId !== null && p.ProductoInventarioId !== "")
            return String(p.ProductoInventarioId);

        return "";
    };

    // Serial = ID|NOMBRE|CANTIDAD_DESPACHADA
    self.getSerialBarcode = function (p) {
        var id = self.getProductoIdTexto(p);
        var nombre = (p.ProductoNombre || "").trim();
        var desp = self.val(p.CantidadDespachada);
        var despTxt = (desp === null || desp === undefined || desp === "") ? "" : String(desp);
        return id + "|" + nombre + "|" + despTxt;
    };

    // ======================================================================
    // MODO 1: Tarjeta completa + código al final (2 columnas)
    // ======================================================================
    self.renderProductoPrintable = function (p, idx) {
        self.normalizarProductoSolicitado(p);

        var id = self.escapeHtml(self.getProductoIdTexto(p));
        var nombre = self.escapeHtml(p.ProductoNombre || "");
        var unidad = self.escapeHtml(p.UnidadMedidaVentaNombre || "");
        var stock = self.escapeHtml((typeof p.CantidadExistente !== 'undefined' ? p.CantidadExistente : ""));
        var tag = self.escapeHtml(self.getTagTexto(p) || "");
        var solicitado = self.escapeHtml(self.val(p.CantidadTrasladada));
        var despachado = self.escapeHtml(self.val(p.CantidadDespachada));
        var diferencia = "";
        if (typeof p.Diferencia !== "undefined" && typeof p.Diferencia === "function") {
            var d = p.Diferencia();
            diferencia = (d === null || d === undefined) ? "—" : self.escapeHtml(d);
        } else {
            diferencia = "—";
        }

        var serial = self.getSerialBarcode(p);
        var serialEsc = self.escapeHtml(serial);
        var titulo = (idx !== null && idx !== undefined) ? ("Producto #" + (idx + 1)) : "Producto";

        return `
          <div class="card">
            <div class="title">${titulo}</div>

            <div class="kv"><div class="k">Id</div><div class="v">${id}</div></div>
            <div class="kv"><div class="k">Producto</div><div class="v">${nombre}</div></div>
            <div class="kv"><div class="k">Unidad</div><div class="v">${unidad}</div></div>
            <div class="kv"><div class="k">Stock</div><div class="v">${stock}</div></div>
            <div class="kv"><div class="k">Tag</div><div class="v mono">${tag}</div></div>
            <div class="kv"><div class="k">Solicitado</div><div class="v">${solicitado}</div></div>
            <div class="kv"><div class="k">Despachado</div><div class="v">${despachado === "" ? "—" : despachado}</div></div>
            <div class="kv"><div class="k">Diferencia</div><div class="v">${diferencia}</div></div>

            <div class="barcodeWrap">
              <svg class="barcode" data-serial="${serialEsc}"></svg>
              <div class="serial mono">${serialEsc}</div>
            </div>
          </div>
        `;
    };

    self.printHtml = function (title, bodyHtml) {
        var w = window.open("", "_blank");
        if (!w) {
            alert("No se pudo abrir la ventana de impresión (pop-up bloqueado).");
            return;
        }

        var jsBarcodeCdn = "https://cdn.jsdelivr.net/npm/jsbarcode@3.11.6/dist/JsBarcode.all.min.js";

        var html = `
        <html>
          <head>
            <title>${self.escapeHtml(title)}</title>
            <meta charset="utf-8" />
            <style>
              @page { size: A4; margin: 10mm; }
              body { font-family: Arial, sans-serif; margin: 0; }

              .sheet {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 8mm;
                align-content: start;
              }

              .card {
                border: 1px solid #ddd;
                border-radius: 8px;
                padding: 8px 10px;
                page-break-inside: avoid;
              }

              .title { font-weight: 700; margin-bottom: 6px; font-size: 12px; }

              .kv { display: flex; margin: 2px 0; font-size: 11px; line-height: 1.2; }
              .k { width: 78px; color: #555; }
              .v { flex: 1; min-width: 0; word-break: break-word; }
              .mono { font-family: Consolas, monospace; }

              .barcodeWrap { margin-top: 8px; padding-top: 6px; border-top: 1px dashed #ddd; }
              .barcode { width: 100%; height: 18mm; }
              .serial { margin-top: 2mm; font-size: 9pt; text-align: center; word-break: break-word; line-height: 1.1; }
            </style>
          </head>
          <body>
            <div class="sheet">
              ${bodyHtml}
            </div>

            <script src="${jsBarcodeCdn}"></script>
            <script>
              (function () {
                function generar() {
                  var svgs = document.querySelectorAll('svg.barcode');
                  svgs.forEach(function (svg) {
                    var val = svg.getAttribute('data-serial') || '';
                    JsBarcode(svg, val, { format: "CODE128", displayValue: false, margin: 0, height: 52 });
                  });
                  window.print();
                }
                if (document.readyState === "complete") generar();
                else window.onload = generar;
              })();
            </script>
          </body>
        </html>
        `;

        w.document.open();
        w.document.write(html);
        w.document.close();
    };

    self.imprimirProducto = function (producto) {
        var card = self.renderProductoPrintable(producto, null);
        self.printHtml("Impresión de producto", card);
    };

    self.imprimirTodos = function () {
        var arr = self.productosSolicitados();
        if (!arr || arr.length === 0) {
            alert("No hay productos solicitados para imprimir.");
            return;
        }

        var lista = self.filteredProductosSolicitados();
        var htmlCards = "";
        lista.forEach(function (p, idx) {
            htmlCards += self.renderProductoPrintable(p, idx);
        });

        self.printHtml("Impresión de productos (" + lista.length + ")", htmlCards);
    };

    // ======================================================================
    // MODO 2: SOLO CÓDIGO (barcode + serial) (2 columnas, más compacto)
    // ======================================================================
    self.renderCodigoOnly = function (p) {
        self.normalizarProductoSolicitado(p);

        var serial = self.getSerialBarcode(p);
        var serialEsc = self.escapeHtml(serial);

        return `
          <div class="codeCard">
            <svg class="barcode" data-serial="${serialEsc}"></svg>
            <div class="serial mono">${serialEsc}</div>
          </div>
        `;
    };

    self.printCodigosHtml = function (title, bodyHtml) {
        var w = window.open("", "_blank");
        if (!w) {
            alert("No se pudo abrir la ventana de impresión (pop-up bloqueado).");
            return;
        }

        var jsBarcodeCdn = "https://cdn.jsdelivr.net/npm/jsbarcode@3.11.6/dist/JsBarcode.all.min.js";

        var html = `
        <html>
          <head>
            <title>${self.escapeHtml(title)}</title>
            <meta charset="utf-8" />
            <style>
              @page { size: A4; margin: 10mm; }
              body { font-family: Arial, sans-serif; margin: 0; }

              .sheet {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 8mm;
                align-content: start;
              }

              .codeCard {
                border: 1px solid #ddd;
                border-radius: 8px;
                padding: 8px 10px;
                page-break-inside: avoid;
              }

              .barcode { width: 100%; height: 18mm; }
              .serial {
                margin-top: 2mm;
                font-size: 9pt;
                text-align: center;
                word-break: break-word;
                line-height: 1.1;
              }
              .mono { font-family: Consolas, monospace; }
            </style>
          </head>
          <body>
            <div class="sheet">
              ${bodyHtml}
            </div>

            <script src="${jsBarcodeCdn}"></script>
            <script>
              (function () {
                function generar() {
                  var svgs = document.querySelectorAll('svg.barcode');
                  svgs.forEach(function (svg) {
                    var val = svg.getAttribute('data-serial') || '';
                    JsBarcode(svg, val, { format: "CODE128", displayValue: false, margin: 0, height: 52 });
                  });
                  window.print();
                }
                if (document.readyState === "complete") generar();
                else window.onload = generar;
              })();
            </script>
          </body>
        </html>
        `;

        w.document.open();
        w.document.write(html);
        w.document.close();
    };

    self.imprimirCodigoProducto = function (producto) {
        var code = self.renderCodigoOnly(producto);
        self.printCodigosHtml("Impresión SOLO código", code);
    };


    self.dibujarCodigoEnModal = function (producto) {
        var serial = self.getSerialBarcode(producto); //

        var $modal = $('#modalBarcode');
        $modal.appendTo("body");

        $modal.modal('show');

        $modal.on('shown.bs.modal', function () {
            try {
                JsBarcode("#barcodeModalElement", serial, {
                    format: "CODE128",
                    displayValue: false,
                    lineColor: "#000",
                    background: "#fff",
                    height: 80,
                    width: 2,
                    margin: 10
                });

                $("#barcodeTextLabel").text(serial);

                var svg = document.getElementById("barcodeModalElement");
                if (svg) {
                    svg.setAttribute("preserveAspectRatio", "xMidYMid meet");
                }
            } catch (e) {
                console.error("Error al dibujar:", e);
            }

            $modal.off('shown.bs.modal');
        });
    };

    self.imprimirCodigosTodos = function () {
        var arr = self.productosSolicitados();
        if (!arr || arr.length === 0) {
            alert("No hay productos solicitados para imprimir.");
            return;
        }

        var lista = self.filteredProductosSolicitados();
        var htmlCodes = "";
        lista.forEach(function (p) {
            htmlCodes += self.renderCodigoOnly(p);
        });

        self.printCodigosHtml("Impresión SOLO códigos (" + lista.length + ")", htmlCodes);
    };

    // =========================
    // Validación mínima
    // =========================
    self.validateModel = function () {
        let bodegaOrigenId = $("#BodegaOrigenId").val();
        if (!bodegaOrigenId || bodegaOrigenId.trim() === "") {
            alert("Seleccione una bodega de origen");
            return false;
        }

        let bodegaDestinoId = $("#BodegaDestinoId").val();
        if (!bodegaDestinoId || bodegaDestinoId.trim() === "") {
            alert("Seleccione una bodega de destino");
            return false;
        }

        return true;
    };

    // =========================
    // Model (payload) - actualizado a Requisición
    // =========================
    self.getModel = function () {
        model = {
            RequisicionId: $("#RequisicionId").val() || null,
            EstadoRequisicion: $("#EstadoRequisicion").val(),

            // Encabezado / firmas (lo que hoy está en la vista)
            Direccion: $("#Direccion").val(),
            Departamento: $("#Departamento").val(),
            UnidadSeccion: $("#UnidadSeccion").val(),
            Otros: $("#Otros").val(),

            NumeroRequisicion: $("#NumeroRequisicion").val() || null,
            NumeroOrden: $("#NumeroOrden").val() || null,
            FechaSolicitud: $("#FechaSolicitud").val() || null,

            NombreSolicitante: $("#NombreSolicitante").val(),
            NombreJefaturaCoordinador: $("#NombreJefaturaCoordinador").val(),
            NombreGerenteAdministrativo: $("#NombreGerenteAdministrativo").val(),
            NombreEncargadoAlmacen: $("#NombreEncargadoAlmacen").val(),
            NombreReceptorFinal: $("#NombreReceptorFinal").val(),

            // Ubicaciones / obs
            BodegaOrigenId: $("#BodegaOrigenId").val(),
            BodegaDestinoId: $("#BodegaDestinoId").val(),
            Observaciones: $("#Observaciones").val()
        };
    };

    // ============================================================
    // Data para POST (binder-friendly): Productos[i].Campo
    // ============================================================
    self.buildPostData = function () {
        self.getModel();

        var postData = {};
        Object.keys(model).forEach(function (k) {
            postData[k] = model[k];
        });

        var productos = ko.toJS(self.productosSolicitados()) || [];
        for (var i = 0; i < productos.length; i++) {
            var p = productos[i] || {};

            var invId = p.ProductoInventarioId;

            // CantidadTrasladada = solicitado
            var cantSolicitada = p.CantidadTrasladada;
            if (cantSolicitada === "" || cantSolicitada === undefined || cantSolicitada === null) cantSolicitada = 0;

            // CantidadDespachada puede ser null/"" (no obligatorio en creación)
            var cantDespachada = p.CantidadDespachada;
            if (cantDespachada === "" || cantDespachada === undefined) cantDespachada = null;

            postData["Productos[" + i + "].ProductoInventarioId"] = invId;
            postData["Productos[" + i + "].CantidadTrasladada"] = cantSolicitada;
            postData["Productos[" + i + "].CantidadDespachada"] = cantDespachada;

            // Compatibilidad (si el backend/VM aún la tiene)
            if (typeof p.FechaTraslado !== "undefined") {
                postData["Productos[" + i + "].FechaTraslado"] = p.FechaTraslado;
            }
        }

        return postData;
    };

    // =========================
    // Guardar
    // =========================
    self.guardarRequisicion = function () {
        if (!self.validateModel()) return;
        if (!confirm("¿Desea registrar esta requisición?")) return;

        showLoading();

        var postData = self.buildPostData();

        $.ajax({
            url: "/Requisicion/GuardarRequisicion",
            method: "POST",
            data: postData,
            success: function (dataResult) {
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    const idGenerado = data.RequisicionId;
                    const urlPdf = `/CrearPDF/generarPDFRequisicionDespacho?requisicionId=${idGenerado}`;
                    window.open(urlPdf, '_blank');

                    window.location.href = "/Requisicion/Lista";
                } else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            // success: function (dataResult) {
            //     let data = JSON.parse(dataResult);
            //     if (data.Exitoso) {
            //         window.location.href = "/Requisicion/Lista";
            //     } else {
            //         hideLoading();
            //         alert(data.Mensaje);
            //     }
            // },
            error: function (dataerror) {
                hideLoading();
                alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
            }
        });
    };

    // Alias defensivo
    self.guardarTraslado = self.guardarRequisicion;

    // =========================
    // Cancelar (renombrado, con alias defensivo)
    // =========================
    self.cancelarRegistroRequisicion = function () {
        if (confirm("¿Desea cancelar el registro?")) {
            window.location.href = "/Requisicion/Lista";
        }
    };

    self.cancelarRegistroTraslado = self.cancelarRegistroRequisicion;
};


var requisicionVm = new RequisicionVM();
ko.applyBindings(requisicionVm);


$(document).ready(function () {

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    })

    $('.collapse').collapse()


    requisicionVm.consultarProductosDisponibles();

    requisicionVm.consultarUltimoRegistro();

    requisicionVm.consultarDataUsuario();


    $("#BodegaOrigenId").change(function () {
        requisicionVm.consultarProductosDisponibles();
    });

});



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

    // --- EVENTOS DE MOUSE ---
    canvas.onmousedown = (e) => {
        isDrawing = true;
        ctx.beginPath();
        const rect = canvas.getBoundingClientRect();
        const x = e.clientX - rect.left;
        const y = e.clientY - rect.top;

        ctx.moveTo(x, y);
    };

    canvas.onmousemove = (e) => {
        if (isDrawing) {
            const rect = canvas.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            ctx.lineTo(x, y); 
            ctx.stroke();     
        }
    };

    window.onmouseup = () => {
        if (isDrawing) {
            ctx.closePath();
            isDrawing = false;
        }
    };

    // --- EVENTOS TÁCTILES (MÓVILES) ---
    canvas.addEventListener("touchstart", function (e) {
        e.preventDefault();
        var touch = e.touches[0];
        var rect = canvas.getBoundingClientRect();
        var mouseEvent = new MouseEvent("mousedown", {
            clientX: touch.clientX,
            clientY: touch.clientY
        });
        canvas.dispatchEvent(mouseEvent);
    }, false);

    canvas.addEventListener("touchmove", function (e) {
        e.preventDefault();
        var touch = e.touches[0];
        var rect = canvas.getBoundingClientRect();
        var mouseEvent = new MouseEvent("mousemove", {
            clientX: touch.clientX,
            clientY: touch.clientY
        });
        canvas.dispatchEvent(mouseEvent);
    }, false);

    canvas.addEventListener("touchend", function (e) {
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