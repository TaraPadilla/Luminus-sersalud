if (typeof ko !== 'undefined' && document.querySelector('[data-bind]')) {
    var InventarioVM = function () {
        var self = this;
        self.precioEditar = ko.observable();
        self.stockEditar = ko.observable();
    };
    ko.applyBindings(new InventarioVM());
}

// =============================================
// ESTADO DE COMPARATIVA
// =============================================
var modoComparativaActivo = false;

// Objeto para guardar los valores ingresados por el usuario antes de un redibujado
var valoresGuardados = {};

// =============================================
// FUNCIONES PARA GUARDAR Y RESTAURAR INPUTS
// =============================================

/**
 * Guarda todos los valores de los inputs editables de la tabla
 * Se usa un identificador único por fila (data-id) + el nombre del campo
 */
function guardarValoresTabla() {
    valoresGuardados = {};
    $('#tabla-clinica-medicamentos tbody tr').each(function (index, row) {
        var $row = $(row);
        var rowId = $row.attr('data-id'); // ID del producto
        if (!rowId) return;

        valoresGuardados[rowId] = {
            // Campos editables
            codigoReferencia: $row.find('.codigo-referencia').val(),
            nombreProducto: $row.find('.nombre-producto').val(),
            loteProducto: $row.find('.lote-producto').val(),
            fechaRecepcionLote: $row.find('.fecha-recepcion-lote').val(),
            precioCompra: $row.find('.precio-compra-input').val(),
            fechaVencimiento: $row.find('.fecha-vencimiento').val(),
            stockIngresado: $row.find('.stock-input').val()
        };
    });
}

/**
 * Restaura los valores guardados después de un redibujado
 */
function restaurarValoresTabla() {
    $('#tabla-clinica-medicamentos tbody tr').each(function (index, row) {
        var $row = $(row);
        var rowId = $row.attr('data-id');
        if (!rowId || !valoresGuardados[rowId]) return;

        var valores = valoresGuardados[rowId];
        $row.find('.codigo-referencia').val(valores.codigoReferencia || '');
        $row.find('.nombre-producto').val(valores.nombreProducto || '');
        $row.find('.lote-producto').val(valores.loteProducto || '');
        $row.find('.fecha-recepcion-lote').val(valores.fechaRecepcionLote || '');
        $row.find('.precio-compra-input').val(valores.precioCompra || '');
        $row.find('.fecha-vencimiento').val(valores.fechaVencimiento || '');
        $row.find('.stock-input').val(valores.stockIngresado || '');
    });
    // Limpiar valores guardados para no reutilizarlos accidentalmente
    valoresGuardados = {};
}

// =============================================
// COMPARATIVA (MODO DIFERENCIAS)
// =============================================

function obtenerTablaAuditoria() {
    var $tabla = $('#tabla-clinica-medicamentos');
    if (!$.fn.DataTable || !$.fn.DataTable.isDataTable($tabla)) {
        return null;
    }
    return $tabla.DataTable();
}

function compararStock() {
    let table = obtenerTablaAuditoria();
    if (!table) {
        mensajeEmergenteError('La tabla no está lista. Recargue la página e intente de nuevo.');
        return;
    }
    
    // Guardar valores actuales antes de cualquier redibujado
    guardarValoresTabla();
    
    showLoading();

    $.ajax({
        url: "/Productos/GetLotesExistentesInventario",
        method: "POST",
        success: function (result) {
            hideLoading();
            let data = JSON.parse(result);
            if (data.Exitoso) {
                let lotesActuales = data.Resultado;

                // Actualizar stock actual (campo disabled) y marcar diferencias
                table.rows().every(function () {
                    let row = $(this.node());
                    let stockInput = row.find('.stock-input');
                    let productoInventarioId = row.find('.producto-inventario-id');
                    let stockActualInput = row.find('.stock-actual');
                    let stockActual = 0;

                    // Obtener stock real del servidor
                    $(lotesActuales).each(function (idx, vl) {
                        if (vl.Id == productoInventarioId.val()) {
                            stockActual = vl.Stock;
                            stockActualInput.val(stockActual);
                        }
                    });

                    let stockIngresado = stockInput.val().trim();
                    if (stockIngresado === '') {
                        row.addClass('fila-oculta-comparativa');
                    } else {
                        let stockIngresadoInt = parseInt(stockIngresado, 10);
                        let stockActualInt = parseInt(stockActual, 10) || 0;
                        if (stockIngresadoInt === stockActualInt) {
                            row.addClass('fila-oculta-comparativa');
                        } else {
                            row.removeClass('fila-oculta-comparativa');
                            row.addClass('stock-diferente');
                            row.find('.stock-diff-info').show();
                        }
                    }
                });

                // Aplicar ocultamiento sin redibujar la tabla para no perder datos
                $('#tabla-clinica-medicamentos tbody tr.fila-oculta-comparativa').hide();
                $('#tabla-clinica-medicamentos tbody tr:not(.fila-oculta-comparativa)').show();

                // No llamar a table.draw(false) para no perder los valores ingresados
                // En su lugar, forzamos a DataTable a refrescar su información de paginación (sin redibujar)
                table.page.info(); // Solo para recalcular, no afecta DOM
                
                modoComparativaActivo = true;
                $('#panel-comparativa').slideDown(200);
            } else {
                mensajeEmergenteError(data.Mensaje);
            }
        },
        error: function (error) {
            hideLoading();
            mensajeEmergenteError("Error de servidor");
        }
    });
}

function regresarDeComparativa() {
    let table = obtenerTablaAuditoria();
    if (!table) return;
    
    // Guardar valores actuales antes de restaurar la vista completa
    guardarValoresTabla();
    
    // Mostrar todas las filas y quitar marcas de comparativa
    $('#tabla-clinica-medicamentos tbody tr').each(function () {
        let row = $(this);
        row.removeClass('fila-oculta-comparativa stock-diferente');
        row.find('.stock-diff-info').hide();
        row.show();
    });
    
    // No llamar a table.draw(false) para no perder datos
    // Simplemente actualizar estado interno de DataTable (no afecta el DOM)
    table.page.info();
    
    modoComparativaActivo = false;
    $('#panel-comparativa').slideUp(200);
    
    // Pequeño retraso para asegurar que cualquier redibujado automático no borre los valores
    setTimeout(function() {
        restaurarValoresTabla();
    }, 10);
}

// =============================================
// GUARDAR CON ACTUALIZACIÓN DE STOCK (RESTA)
// =============================================

function mostrarDialogoGuardarNuevo() {
    // 🔁 Cambiar mensaje: ahora se resta, no se suma
    let confirmacion = confirm("¿Desea guardar y actualizar el stock? Se RESTARÁ el stock ingresado al stock actual.");
    if (confirmacion) {
        guardarStockNuevo();
    }
}

function guardarStockNuevo() {
    let table = obtenerTablaAuditoria();
    if (!table) {
        mensajeEmergenteError('La tabla no está lista. Recargue la página e intente de nuevo.');
        return;
    }
    let productos = [];
    let errores = [];

    // Recorrer todas las filas de la tabla
    table.rows().every(function () {
        let row = $(this.node());
        let stockIngresado = row.find('.stock-input').val().trim();
        let stockActual = parseInt(row.find('.stock-actual').val(), 10) || 0;
        let idProducto = row.attr('data-id');
        let nombreProducto = row.find('.nombre-producto').val().trim();

        // Validación previa en cliente
        if (stockIngresado !== "") {
            let stockIngresadoInt = parseInt(stockIngresado, 10);
            if (isNaN(stockIngresadoInt) || stockIngresadoInt < 0) {
                errores.push(`El stock ingresado para "${nombreProducto || idProducto}" no es válido.`);
            } else if (stockIngresadoInt > stockActual) {
                errores.push(`No se pueden restar ${stockIngresadoInt} unidades de "${nombreProducto || idProducto}". Stock actual: ${stockActual}.`);
            }
        }
    });

    // Si hay errores de validación, mostrarlos y detener el envío
    if (errores.length > 0) {
        mensajeEmergenteError(errores.join("\n"));
        return;
    }

    // Recolectar datos para enviar al servidor
    table.rows().every(function () {
        let row = $(this.node());
        let stockIngresado = row.find('.stock-input').val().trim();
        let precioCostoInput = row.find('.precio-compra-input').val().trim();
        let precioCosto = precioCostoInput !== "" ? parseFloat(precioCostoInput) : 0;

        let idProducto = row.attr('data-id');
        let idProductoInventario = row.attr('data-idProductoInventario');
        let codigoReferencia = row.find('.codigo-referencia').val().trim();
        let nombreProducto = row.find('.nombre-producto').val().trim();
        let fechaVencimiento = row.find('.fecha-vencimiento').val().trim();
        let lote = row.find('.lote-producto').val().trim();
        let fechaRecepcionLote = row.find('.fecha-recepcion-lote').val().trim();

        let stockIngresadoInt = null;
        if (stockIngresado !== "") {
            stockIngresadoInt = parseInt(stockIngresado, 10);
        }

        productos.push({
            idProducto: parseInt(idProducto, 10),
            stockIngresado: stockIngresadoInt,
            idProductoInventario: parseInt(idProductoInventario, 10),
            codigoReferencia: codigoReferencia,
            nombreProducto: nombreProducto,
            fechaVencimiento: fechaVencimiento,
            loteProducto: lote,
            fechaRecepcionLote: fechaRecepcionLote,
            precioCosto: precioCosto
        });
    });

    if (productos.length > 0) {
        actualizarStockEnServidorNuevo(productos);
    }
}

function actualizarStockEnServidorNuevo(productos) {
    showLoading();
    $.ajax({
        type: "POST",
        url: "/Auditoria/ActualizarStock",
        data: JSON.stringify(productos),
        contentType: "application/json",
        success: function (data) {
            hideLoading();
            // Mostrar mensaje de éxito si el servidor lo devuelve
            if (data && data.Mensaje) {
                mensajeEmergenteExito(data.Mensaje);
            }
            window.location.href = "/Auditoria/Lista";
        },
        error: function (xhr, status, error) {
            hideLoading();
            // 🔧 Mejor manejo de errores: mostrar el mensaje devuelto por el servidor
            let errorMsg = "Error al guardar la auditoría. Por favor intente de nuevo.";
            if (xhr.responseJSON && xhr.responseJSON.Mensaje) {
                errorMsg = xhr.responseJSON.Mensaje;
            } else if (xhr.responseText) {
                try {
                    let json = JSON.parse(xhr.responseText);
                    if (json.Mensaje) errorMsg = json.Mensaje;
                } catch (e) {}
            }
            mensajeEmergenteError(errorMsg);
        }
    });
}

// =============================================
// GUARDAR SIN ACTUALIZAR STOCK
// =============================================

function mostrarDialogoGuardarNuevoSinStock() {
    let confirmacion = confirm("¿Desea guardar la auditoría sin actualizar el stock?");
    if (confirmacion) {
        guardarStockNuevoSinStock();
    }
}

function guardarStockNuevoSinStock() {
    let table = obtenerTablaAuditoria();
    if (!table) {
        mensajeEmergenteError('La tabla no está lista. Recargue la página e intente de nuevo.');
        return;
    }
    let productos = [];

    table.rows().every(function () {
        let row = $(this.node());
        let stockIngresado = row.find('.stock-input').val().trim();
        let stockIngresadoInt = stockIngresado === "" ? 0 : parseInt(stockIngresado, 10) || 0;

        let precioCostoInput = row.find('.precio-compra-input').val().trim();
        let precioCosto = precioCostoInput !== "" ? parseFloat(precioCostoInput) : 0;

        let idProducto = row.attr('data-id');
        let idProductoInventario = row.attr('data-idProductoInventario');
        let codigoReferencia = row.find('.codigo-referencia').val().trim();
        let nombreProducto = row.find('.nombre-producto').val().trim();
        let fechaVencimiento = row.find('.fecha-vencimiento').val().trim();
        let lote = row.find('.lote-producto').val().trim();
        let fechaRecepcionLote = row.find('.fecha-recepcion-lote').val().trim();

        productos.push({
            idProducto: parseInt(idProducto, 10),
            stockIngresado: stockIngresadoInt,
            idProductoInventario: parseInt(idProductoInventario, 10),
            codigoReferencia: codigoReferencia,
            nombreProducto: nombreProducto,
            fechaVencimiento: fechaVencimiento,
            loteProducto: lote,
            fechaRecepcionLote: fechaRecepcionLote,
            precioCosto: precioCosto
        });
    });

    if (productos.length > 0) {
        actualizarStockEnServidorNuevoSinStock(productos);
    }
}

function actualizarStockEnServidorNuevoSinStock(productos) {
    showLoading();
    $.ajax({
        type: "POST",
        url: "/Auditoria/NuevoSinActualizarStock",
        data: JSON.stringify(productos),
        contentType: "application/json",
        success: function (data) {
            hideLoading();
            if (data && data.Mensaje) {
                mensajeEmergenteExito(data.Mensaje);
            }
            window.location.href = "/Auditoria/Lista";
        },
        error: function (xhr, status, error) {
            hideLoading();
            let errorMsg = "Error al guardar la auditoría. Por favor intente de nuevo.";
            if (xhr.responseJSON && xhr.responseJSON.Mensaje) {
                errorMsg = xhr.responseJSON.Mensaje;
            } else if (xhr.responseText) {
                try {
                    let json = JSON.parse(xhr.responseText);
                    if (json.Mensaje) errorMsg = json.Mensaje;
                } catch (e) {}
            }
            mensajeEmergenteError(errorMsg);
        }
    });
}

// =============================================
// CANCELAR
// =============================================

function cancelarRegistroAuditoria() {
    if (confirm("¿Desea cancelar el registro de la auditoría? Se perderán los datos no guardados.")) {
        window.location.href = "/Auditoria/Lista";
    }
}

// =============================================
// HELPERS DE TABLA
// =============================================

function actualizarCodigoReferencia(inputContent, productoId) {
    let codigo = inputContent.val();
    let inputsCodigo = $(".codigo-referencia-producto-" + productoId);
    $(inputsCodigo).each(function (idx, input) {
        $(input).val(codigo);
    });
}

function actualizarNombre(inputContent, productoId) {
    let nombre = inputContent.val();
    let inputsNombre = $(".nombre-producto-" + productoId);
    $(inputsNombre).each(function (idx, input) {
        $(input).val(nombre);
    });
}

// =============================================
// INIT DATATABLE
// =============================================

$(function () {
    if (!$.fn.DataTable) {
        console.error('DataTables no está disponible en Auditoría/Nuevo.');
        return;
    }

    var $tabla = $("#tabla-clinica-medicamentos");
    if ($.fn.DataTable.isDataTable($tabla)) {
        $tabla.DataTable().destroy();
    }

    var table = $tabla.DataTable({
        dom: 'frtip',
        searching: true,
        ordering: true,
        paging: true,
        pageLength: 10,
        language: {
            search: "Buscar: ",
            lengthMenu: "Mostrar _MENU_ registros por página",
            zeroRecords: "No hay registros para mostrar",
            info: "Mostrando página _PAGE_ de _PAGES_",
            infoEmpty: "",
            infoFiltered: "(filtrado de _MAX_ registros totales)",
            paginate: {
                first: "Primero",
                last: "Último",
                previous: "Anterior",
                next: "Siguiente"
            }
        }
    });

    // Actualizar contador de llenado en tiempo real
    $('#tabla-clinica-medicamentos').on('input', '.stock-input', function () {
        actualizarContadorLlenado();
    });

    function actualizarContadorLlenado() {
        let total = 0;
        let llenados = 0;
        table.rows().every(function () {
            let val = $(this.node()).find('.stock-input').val().trim();
            total++;
            if (val !== '') llenados++;
        });
        $('#label-total').text(llenados + ' / ' + total + ' ingresados');
    }
});