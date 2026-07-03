var InventarioVM = function () {
    var self = this;
    self.precioEditar = ko.observable();
    self.stockEditar = ko.observable();
    self.fechaRecepcionEditar = ko.observable();
    self.fechaVencimientoEditar = ko.observable();
    self.loteEditar = ko.observable();

};

var inventarioVm = new InventarioVM();
ko.applyBindings(inventarioVm);

$(function () {
    var table = $("#tabla-clinica-medicamentos").DataTable({
        dom: 'Bfrtip',
        buttons: [
            'copy', 'excel', 'pdf'
        ],
        searching: true,
        ordering: true,
        paging: true,

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
        },
    });
    new $.fn.dataTable.Buttons(table, {
        buttons: [
            'copy', 'excel', 'pdf'
        ]
    });
});

function reiniciarStockProducto(productoId, ambienteId) {
    if (confirm("¿Desea reiniciar el stock de este producto? Esto eliminará los inventarios actuales y establecerá el stock de todas las equivalencias en 1.")) {
        // Capturar los valores de los precios desde los inputs ocultos
        var precioNormal = document.getElementById("precioNormal_" + productoId)?.value || 0;
        var precioInterno = document.getElementById("precioInterno_" + productoId)?.value || 0;
        var precioVIP = document.getElementById("precioVIP_" + productoId)?.value || 0;
        var stockMinimo = document.getElementById("stockMinimo_" + productoId)?.value || 1; // Si no se encuentra el stock mínimo, se asigna 1

        // Capturar el valor del lote directamente desde el span
        var lote = document.getElementById("lote_" + productoId)?.value || 'No registra';

        // Capturar las fechas de recepción de lote y vencimiento desde los inputs ocultos
        var fechaRecepcionLote = document.getElementById("fechaRecepcionLote_" + productoId)?.value;

        // Si la fecha de recepción no está definida, asignar la fecha de hoy
        var hoy = new Date();
        var fechaHoyString = hoy.toISOString().split('T')[0]; // Obtener la fecha de hoy en formato YYYY-MM-DD
        fechaRecepcionLote = fechaRecepcionLote || fechaHoyString;

        // Si la fecha de vencimiento no está definida, asignar la fecha de hoy + 1 año
        var fechaVencimiento = document.getElementById("fechaVencimiento_" + productoId)?.value;
        if (!fechaVencimiento) {
            var fechaVencimientoDate = new Date(hoy);
            fechaVencimientoDate.setFullYear(hoy.getFullYear() + 1); // Sumar 1 año a la fecha de hoy
            fechaVencimiento = fechaVencimientoDate.toISOString().split('T')[0]; // Convertir a formato YYYY-MM-DD
        }
        showLoading(); // Mostrar indicador de carga si tienes uno
        $.ajax({
            url: "/Productos/ReiniciarStock",
            method: "POST",
            data: {
                productoId: productoId,
                ambienteId: ambienteId, // Pasar el ambienteId al controlador
                precioNormal: precioNormal, // Pasar precioNormal al controlador
                precioInterno: precioInterno, // Pasar precioInterno al controlador
                precioVIP: precioVIP, // Pasar precioVIP al controlador
                stockMinimo: stockMinimo, // Pasar stockMinimo al controlador
                lote: lote, // Pasar lote al controlador
                fechaRecepcionLote: fechaRecepcionLote, // Pasar fechaRecepcionLote al controlador
                fechaVencimiento: fechaVencimiento // Pasar fechaVencimiento al controlador
            },
            success: function (dataResult) {
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    mensajeEmergente(data.Mensaje); // Mostrar un mensaje de éxito si es necesario
                    window.location.reload(); // Recargar la página para reflejar los cambios
                } else {
                    hideLoading(); // Ocultar el indicador de carga si ocurre un error
                    alert(data.Mensaje); // Mostrar el mensaje de error
                }
            },
            error: function () {
                hideLoading(); // Ocultar el indicador de carga si ocurre un error de red
                alert("Error al intentar reiniciar el stock. Intente de nuevo.");
            }
        });
    }
}


function editarFechaRecepcionLoteProducto(productoId, productoInventarioId, fechaRecepcionLote) {
    // Establecer el valor actual de la fecha de recepción de lote para editar
    inventarioVm.fechaRecepcionEditar(fechaRecepcionLote);

    // Abre el modal para editar la fecha de recepción de lote
    $("#mdl-modificar-fecha-recepcion").dialog({
        modal: true,
        buttons: [
            {
                text: "Cancelar",
                class: "btn btn-danger",
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: "Modificar",
                class: "btn btn-success",
                click: function () {
                    if (confirm("¿Desea modificar la fecha de recepción de lote?")) {
                        showLoading();
                        $.ajax({
                            url: "/Productos/ModificarFechaRecepcionLote",
                            method: "POST",
                            data: {
                                productoId: productoId,
                                productoInventarioId: productoInventarioId,
                                fechaRecepcionLote: inventarioVm.fechaRecepcionEditar()  // Valor editado
                            },
                            success: function (dataResult) {
                                console.log(dataResult);  // Ver la respuesta en la consola
                                let data = dataResult;
                                if (data.Exitoso) {
                                    mensajeEmergente("Fecha de recepción de lote modificada!");

                                    // Actualizar la fecha en el DOM sin recargar la página
                                    let fechaElement = document.querySelector(`#fechaRecepcionLote_${productoInventarioId}`);
                                    if (fechaElement) {
                                        fechaElement.innerText = inventarioVm.fechaRecepcionEditar(); // Actualiza la fecha en el DOM
                                    }

                                    hideLoading();
                                    $("#mdl-modificar-fecha-recepcion").dialog('close');
                                } else {
                                    hideLoading();
                                    window.location.reload();
                                }
                            },
                            error: function () {
                                hideLoading();
                                alert("Error al modificar la fecha de recepción de lote. Intente de nuevo.");
                            }
                        });
                    }
                }
            }
        ]
    });
}



function editarLoteProducto(productoId, productoInventarioId, lote) {
    // Establecer el valor actual del lote para editar
    inventarioVm.loteEditar(lote);

    // Abre el modal para editar el lote
    $("#mdl-modificar-lote").dialog({
        modal: true,
        buttons: [
            {
                text: "Cancelar",
                class: "btn btn-danger",
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: "Modificar",
                class: "btn btn-success",
                click: function () {
                    if (confirm("¿Desea modificar el número de lote?")) {
                        showLoading();
                        $.ajax({
                            url: "/Productos/ModificarLote",
                            method: "POST",
                            data: {
                                productoId: productoId,
                                productoInventarioId: productoInventarioId,
                                lote: inventarioVm.loteEditar()  // Valor editado
                            },
                            success: function (dataResult) {
                                console.log(dataResult);  // Ver la respuesta en la consola
                                let data = dataResult;
                                if (data.Exitoso) {
                                    mensajeEmergente("Número de lote modificado!");

                                    // Actualizar el lote en el DOM sin recargar la página
                                    let loteElement = document.querySelector(`#lote_${productoInventarioId}`);
                                    if (loteElement) {
                                        loteElement.innerText = inventarioVm.loteEditar(); // Actualiza el lote en el DOM
                                    }

                                    hideLoading();
                                    $("#mdl-modificar-lote").dialog('close');
                                } else {
                                    hideLoading();
                                    window.location.reload();
                                }
                            },
                            error: function () {
                                hideLoading();
                                alert("Error al modificar el número de lote. Intente de nuevo.");
                            }
                        });
                    }
                }
            }
        ]
    });
}


function editarFechaVencimientoProducto(productoId, productoInventarioId, fechaVencimiento) {
    // Establecer el valor actual de la fecha para editar
    inventarioVm.fechaVencimientoEditar(fechaVencimiento);

    // Abre el modal para editar la fecha de vencimiento
    $("#mdl-modificar-fecha-vencimiento").dialog({
        modal: true,
        buttons: [
            {
                text: "Cancelar",
                class: "btn btn-danger",
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: "Modificar",
                class: "btn btn-success",
                click: function () {
                    if (confirm("¿Desea modificar la fecha de vencimiento?")) {
                        showLoading();
                        $.ajax({
                            url: "/Productos/ModificarFechaVencimiento",
                            method: "POST",
                            data: {
                                productoId: productoId,
                                productoInventarioId: productoInventarioId,
                                fechaVencimiento: inventarioVm.fechaVencimientoEditar()  // Valor editado
                            },
                            success: function (dataResult) {
                                console.log(dataResult);  // Esto te permitirá ver la respuesta del servidor en la consola
                                let data = dataResult;  
                                if (data.Exitoso) {
                                    mensajeEmergente("Fecha de vencimiento modificada!");
                            
                                    // Actualizar la fecha en el DOM sin recargar la página
                                    let fechaElement = document.querySelector(`#fechaVencimiento_${productoInventarioId}`);
                                    if (fechaElement) {
                                        fechaElement.innerText = inventarioVm.fechaVencimientoEditar(); // Actualiza la fecha en el DOM
                                    }
                            
                                    hideLoading();
                                    $("#mdl-modificar-fecha-vencimiento").dialog('close');
                                } else {
                                    hideLoading();
                                    window.location.reload();
                                }
                            },
                            error: function () {
                                hideLoading();
                                alert("Error al modificar la fecha de vencimiento. Intente de nuevo.");
                            }
                        });
                    }
                }
            }
        ]
    });
}


function editarPrecioProducto(inventarioPrecio, precioValor, precioId, productoInventarioId) {
    // console.log("Editar precio iniciado");
    // console.log("Parámetros recibidos:", { inventarioPrecio, precioValor, precioId, productoInventarioId });

    // Actualizar el valor del precio a editar en el ViewModel
    inventarioVm.precioEditar(parseFloat(precioValor).toFixed(2));
    // console.log("ViewModel actualizado con precioEditar:", inventarioVm.precioEditar());

    // Configuración del diálogo
    $("#mdl-modificar-precio").dialog({
        modal: true,
        buttons: [
            {
                text: "Cancelar",
                class: "btn btn-danger",
                click: function () {
                    // console.log("Diálogo cerrado por el usuario (Cancelar)");
                    $(this).dialog('close');
                }
            },
            {
                text: "Modificar",
                class: "btn btn-success",
                click: function () {
                    // console.log("Botón 'Modificar' presionado");

                    if (confirm("¿Desea modificar este precio?")) {
                        // console.log("Confirmación de modificación aceptada");
                        showLoading();
                        // console.log("Enviando solicitud AJAX para modificar precio...");

                        // Solicitud AJAX
                        $.ajax({
                            url: "/Productos/ModificarPrecio",
                            method: "POST",
                            data: {
                                productoInventarioPrecioId: inventarioPrecio,
                                productoInventarioPrecioValor: inventarioVm.precioEditar(),
                                precioId: precioId,
                                productoInventarioId: productoInventarioId
                            },
                            success: function (dataResult) {
                                // console.log("Respuesta recibida del servidor:", dataResult);
                            
                                let data;
                                try {
                                    data = JSON.parse(dataResult);
                                    // console.log("Datos parseados:", data);
                                } catch (error) {
                                    console.error("Error al parsear la respuesta del servidor:", error);
                                    hideLoading();
                                    alert("Error inesperado al procesar la respuesta del servidor.");
                                    return;
                                }
                            
                                if (data.Exitoso) {
                                    // console.log("Modificación exitosa. Actualizando DOM...");
                                    mensajeEmergente("Precio modificado!");
                            
                                    // Actualizar los elementos visibles
                                    let nombreElement = document.getElementById(`precioNombre_${productoInventarioId}`);
                                    let valorElement = document.getElementById(`precioValor_${productoInventarioId}`);
                            
                                    if (nombreElement && valorElement) {
                                        // console.log("Actualizando elementos visibles en el DOM...");
                                        nombreElement.textContent = data.NombrePrecio || "Sin Nombre";
                                        valorElement.textContent = inventarioVm.precioEditar();
                                    } else {
                                        console.warn("No se encontraron elementos para actualizar el precio dinámicamente.");
                                    }
                            
                                    hideLoading();
                                    $("#mdl-modificar-precio").dialog('close');
                                    // console.log("Diálogo cerrado tras la modificación exitosa.");
                                } else {
                                    console.warn("Error reportado por el servidor:", data.Mensaje);
                                    hideLoading();
                                    alert(data.Mensaje);
                                }
                            },
                            error: function () {
                                console.error("Error en la solicitud AJAX.");
                                hideLoading();
                                alert("Error al modificar el precio. Intente de nuevo.");
                            }
                        });
                    } else {
                        // console.log("Confirmación de modificación cancelada por el usuario");
                    }
                }
            }
        ]
    });
    // console.log("Diálogo configurado y mostrado.");
}



function eliminarRegistroInventario(registroId) {
    if (confirm("¿Desea eliminar este registro?")) {
        showLoading();
        $.ajax({
            url: "/Productos/InventarioProductoEliminarRegistroInventario",
            method: "POST",
            data: {
                registroId: registroId
            },
            success: function (dataResult) {
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            }
        })
    }
}

function editarStockProducto(productoId, productoInventarioId, stock) {
    console.log('Producto ID:', productoId);
    console.log('Producto Inventario ID:', productoInventarioId);
    console.log('Stock:', stock);

    // Verifica si ambos IDs están presentes
    if (!productoId && !productoInventarioId) {
        alert('Producto ID o Producto Inventario ID es nulo o indefinido.');
        return;
    }

    console.log('Abriendo el diálogo para modificar stock');
    inventarioVm.stockEditar(stock);
    $("#mdl-modificar-stock").dialog({
        modal: true,
        buttons: [
            {
                text: "Cancelar",
                class: "btn btn-danger",
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: "Modificar",
                class: "btn btn-success",
                click: function () {
                    if (confirm("¿Desea modificar este stock?")) {
                        showLoading();
                        $.ajax({
                            url: "/Productos/ModificarStock",
                            method: "POST",
                            data: {
                                productoId: productoId,
                                productoInventarioId: productoInventarioId,
                                stock: inventarioVm.stockEditar(),
                                crearRegistroInventario: productoInventarioId == null || productoInventarioId == undefined,
                                ambienteId: $("#AmbienteId").val()
                            },
                            success: function (dataResult) {
                                let data = JSON.parse(dataResult);
                                if (data.Exitoso) {
                                    mensajeEmergente("Stock modificado!!");

                                    // Actualizar el stock directamente en el DOM sin recargar la página
                                    let stockElement = document.querySelector(`#stockProducto_${productoId}`);
                                    if (stockElement) {
                                        stockElement.innerText = inventarioVm.stockEditar(); // Actualiza el valor en el DOM
                                    }

                                    hideLoading();
                                    $("#mdl-modificar-stock").dialog('close');
                                } else {
                                    hideLoading();
                                    alert(data.Mensaje);
                                }
                            },
                            error: function () {
                                hideLoading();
                                alert("Error al modificar el stock. Intente de nuevo.");
                            }
                        });
                    }
                }
            }
        ]
    });
}



function eliminarStockProducto(productoId, productoInventarioId) {
    // Confirmación antes de realizar la eliminación
    if (confirm("¿Desea eliminar este registro? Esto establecerá el stock en 0.")) {
        showLoading();
        $.ajax({
            url: "/Productos/ModificarStock",
            method: "POST",
            data: {
                productoId: productoId,
                productoInventarioId: productoInventarioId,
                stock: 0,
                crearRegistroInventario: false, // no creará un registro de inventario nuevo
                ambienteId: $("#AmbienteId").val()
            },
            success: function (dataResult) {
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    mensajeEmergente("Stock eliminado (establecido en 0)!");
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            }
        });
    }
}


function editarStockMinimoProducto(productoId, productoInventarioId, stock) {
    // Verificar el valor de stock recibido
    console.log("Stock mínimo inicial:", stock);

    // Asignar el valor al observable de Knockout
    inventarioVm.stockEditar(stock);

    // Abrir el diálogo de modificación
    $("#mdl-modificar-stock-minimo").dialog({
        modal: true,
        buttons: [
            {
                text: "Cancelar",
                class: "btn btn-danger",
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: "Modificar",
                class: "btn btn-success",
                click: function () {
                    if (confirm("¿Desea modificar este stock mínimo?")) {
                        showLoading();

                        // Log para verificar el valor de stock antes de enviarlo
                        console.log("Stock mínimo a enviar:", inventarioVm.stockEditar());

                        $.ajax({
                            url: "/Productos/ModificarStockMinimo",
                            method: "POST",
                            data: {
                                productoId: productoId,
                                productoInventarioId: productoInventarioId,
                                stockMinimo: inventarioVm.stockEditar(),  // Valor actualizado de stock
                                crearRegistroInventario: productoId != null && productoId != undefined,
                                ambienteId: $("#AmbienteId").val()
                            },
                            success: function (dataResult) {
                                let data = JSON.parse(dataResult);
                                if (data.Exitoso) {
                                    mensajeEmergente("¡Stock mínimo modificado!");

                                    // Actualizar el stock mínimo en el DOM sin recargar la página
                                    let stockMinimoElement = document.querySelector(`#stockMinimo_${productoInventarioId}`);
                                    if (stockMinimoElement) {
                                        stockMinimoElement.innerText = `Stock mínimo: ${inventarioVm.stockEditar()}`;
                                    }

                                    hideLoading();
                                    $("#mdl-modificar-stock-minimo").dialog('close');
                                } else {
                                    hideLoading();
                                    alert(data.Mensaje);
                                }
                            },
                            error: function () {
                                hideLoading();
                                alert("Error al modificar el stock mínimo. Intente de nuevo.");
                            }
                        });
                    }
                }
            }
        ]
    });
}



function toggleCollapse(button, targetId) {
    var target = document.querySelector(targetId);
    var isExpanded = button.getAttribute('aria-expanded') === 'true';
    button.setAttribute('aria-expanded', !isExpanded);
    if (isExpanded) {
        target.classList.remove('show');
        button.innerHTML = "+";
    } else {
        target.classList.add('show');
        button.innerHTML = "-";
    }
}