var cuentasVM = function () {
    var self = this;
    var model = {};
    self.sucursalId = ko.observable($("SucursalId").val());
    self.bancos = ko.observableArray();
    self.bancoSeleccionado = ko.observable();

    self.cuentasBanco = ko.observableArray();
    self.cuentaSeleccionado = ko.observable();

    self.cuentasContables = ko.observableArray();
    self.cuentaContableSeleccionado = ko.observable();

    //Reabrir caja
    self.codigoEmpleadoReabrirCaja = ko.observable();
    self.cajaSeleccionadaReabrir = ko.observable();


    self.consultarBancos = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/CuentaContable/ConsultarBancos',
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.bancos(data.Resultado);
                }
                else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.consultarCuentasBanco = function (Id) {
        showLoading();
        $.ajax({
            method: "POST",
            data: {
                "Id": Id
            },
            url: '/CuentaContable/ConsultarCuentasBanco',
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.cuentasBanco(data.Resultado);

                }
                else {
                    //debugger;
                    hideLoading();
                    alert(data.Mensaje);
                    console.log("Error ajax");
                }
            },

            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });

    };

    self.consultarCuentasContables = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/CuentaContable/ConsultarCuentas',
            success: function (dataResult) {
                hideLoading();
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    self.cuentasContables(data.Resultado);

                }
                else {
                    //debugger;
                    hideLoading();
                    alert(data.Mensaje);
                    console.log("Error ajax");
                }
            },

            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });

    };

    self.bancoSeleccionado.subscribe(function (value) {
        self.consultarCuentasBanco(value.Id);
    });

    self.reabrirCaja = function () {
        if (confirm("�Desea reabrir esta caja?")) {
            showLoading();
            $.ajax({
                url: "/Caja/ReabrirCaja",
                data: {
                    cajaId: self.cajaSeleccionadaReabrir(),
                    empleadoId: self.codigoEmpleadoReabrirCaja()
                },
                success: function (data) {
                    let dataResult = JSON.parse(data);
                    if (!dataResult.Exitoso) {
                        hideLoading();
                        alert(dataResult.Mensaje);
                    } else {
                        window.location.reload();
                    }
                },
                error: function (error) {
                    hideLoading();
                    alert("Error al solicitar reapertura de caja");
                    console.log(error.statusText);
                }
            })
        }
    };
}

var cuentasVM = new cuentasVM();
ko.applyBindings(cuentasVM);

$(document).ready(function () {
    cuentasVM.consultarBancos();
    cuentasVM.consultarCuentasContables();
    $('#miFormularioIngreso').submit(function (event) {
        event.preventDefault(); // Evita el env�o por defecto del formulario

        var formData = $(this).serialize(); // Serializa los datos del formulario
        var bancoSeleccionadoValue = cuentasVM.bancoSeleccionado() ? cuentasVM.bancoSeleccionado().Id : 0;
        var cuentaSeleccionadoValue = cuentasVM.cuentaSeleccionado() ? cuentasVM.cuentaSeleccionado().CuentaId : 0;


        //Validaciones
        let montoIngreso = $("#monto-ingreso").val();
        let descripcionIngreso = $("#descripcion-ingreso").val();
        if (descripcionIngreso == undefined || montoIngreso == null || descripcionIngreso.trim() == '') {
            alert("Debe colocar la descripcion del ingreso");
        } else if (montoIngreso == undefined || montoIngreso == null || isNaN(montoIngreso) || montoIngreso <= 0) {
            alert("Debe proporcionar un monto de ingreso");
        } else if (bancoSeleccionadoValue == 0) {
            alert("Debe seleccionar un banco");
        } else if (cuentaSeleccionadoValue == 0) {
            alert("Debe seleccionar una cuenta");
        } else {

            // Agrega los valores de bancoSeleccionado y cuentaSeleccionado al formData
            formData += '&banco=' + bancoSeleccionadoValue + '&cuenta=' + cuentaSeleccionadoValue;

            showLoading();
            // Env�o AJAX
            $.ajax({
                type: 'POST',
                url: '/Caja/GuardarIngreso', // Reemplaza "Home" con el nombre de tu controlador
                data: formData,
                success: function (response) {
                    $('#miModalIngreso').modal('hide'); // Cierra el modal despu�s del env�o
                    location.reload();
                },
                error: function () {
                    hideLoading();
                    alert('Error al enviar el formulario');
                }
            });
        }
    });

    $('#miFormularioEgreso').submit(function (event) {
        event.preventDefault(); // Evita el env�o por defecto del formulario

        var formData = $(this).serialize(); // Serializa los datos del formulario
        var cuentaSeleccionadoValue = cuentasVM.cuentaContableSeleccionado() ? cuentasVM.cuentaContableSeleccionado().Id : 0;
        var cuentapuirbea = cuentasVM.cuentaContableSeleccionado();


        //Validaciones
        let montoEgreso = $("#monto-egreso").val();
        let descripcionEgreso = $("#descripcion-egreso").val();
        if (descripcionEgreso == undefined || descripcionEgreso == null || descripcionEgreso.trim() == '') {
            alert("Debe colocar la descripcion del egreso");
        } else if (montoEgreso == undefined || montoEgreso == null || isNaN(montoEgreso) || montoEgreso <= 0) {
            alert("Debe proporcionar un monto de egreso");
        } else {

            // Agrega los valores de bancoSeleccionado y cuentaSeleccionado al formData
            formData += '&cuenta=' + cuentaSeleccionadoValue;

            showLoading();
            // Env�o AJAX
            $.ajax({
                type: 'POST',
                url: '/Caja/GuardarEgreso', // Reemplaza "Home" con el nombre de tu controlador
                data: formData,
                success: function (response) {
                    $('#miModalEgreso').modal('hide'); // Cierra el modal despu�s del env�o
                    location.reload();
                },
                error: function () {
                    hideLoading();
                    alert('Error al enviar el formulario');
                }
            });
        }
    });

});



//El parametro checked indica si la sucursal esta seleccionada
//para ver
function aplicarFiltrosIngresosSucursal(sucursalId, checked) {

    if (checked) {
        $(".ingresos-reg-sucursal-" + sucursalId).show();

        //Se aplica el filtro por ambientes
        let itemsAmbientesFiltro = $("input.ingresos-filtro-ambiente");
        $(itemsAmbientesFiltro).each(function (idx, vl) {
            let ambienteId = vl.id.replace('ingresos-filtro-ambiente-', "");
            if (vl.checked) {
                $(".ingresos-reg-sucursal-" + sucursalId + ".ingresos-reg-ambiente-" + ambienteId).show();
                $(".ingresos-valor-sucursal-" + sucursalId + ".ingresos-valor-ambiente-" + ambienteId).addClass('ingresos-sumar');
            } else {
                $(".ingresos-reg-sucursal-" + sucursalId + ".ingresos-reg-ambiente-" + ambienteId).hide();
                $(".ingresos-valor-sucursal-" + sucursalId + ".ingresos-valor-ambiente-" + ambienteId).removeClass('ingresos-sumar');
            }
        });

    } else {
        $(".ingresos-reg-sucursal-" + sucursalId).hide();
        $(".ingresos-valor-sucursal-" + sucursalId).removeClass('ingresos-sumar')
    }
}
function aplicarFiltrosIngresos() {
    if ($("#SucursalId").val() == '') {
        let itemsIngresosFiltro = $("input.ingresos-filtro-sucursal");
        $(itemsIngresosFiltro).each(function (idx, vl) {
            let sucursalId = vl.id.replace('ingresos-filtro-sucursal-', "");
            aplicarFiltrosIngresosSucursal(sucursalId, vl.checked);
        });
    } else {
        aplicarFiltrosIngresosSucursal($("#SucursalId").val(), true);
    }

    //Sumar totales
    let totalIngresos = 0;

    let valores = $(".ingresos-sumar");
    $(valores).each(function (idx, vl) {
        totalIngresos += parseFloat(vl.innerText);
    });

    $("#ingresos-total").text(totalIngresos);

}

//El parametro checked indica si la sucursal esta seleccionada
//para ver
function aplicarFiltrosEgresosSucursal(sucursalId, checked) {

    if (checked) {
        $(".egresos-reg-sucursal-" + sucursalId).show();

        //Se aplica el filtro por ambientes
        let itemsAmbientesFiltro = $("input.egresos-filtro-ambiente");
        $(itemsAmbientesFiltro).each(function (idx, vl) {
            let ambienteId = vl.id.replace('egresos-filtro-ambiente-', "");
            if (vl.checked) {
                $(".egresos-reg-sucursal-" + sucursalId + ".egresos-reg-ambiente-" + ambienteId).show();
                $(".egresos-valor-sucursal-" + sucursalId + ".egresos-valor-ambiente-" + ambienteId).addClass('egresos-sumar');
            } else {
                $(".egresos-reg-sucursal-" + sucursalId + ".egresos-reg-ambiente-" + ambienteId).hide();
                $(".egresos-valor-sucursal-" + sucursalId + ".egresos-valor-ambiente-" + ambienteId).removeClass('egresos-sumar');
            }
        });

    } else {
        $(".egresos-reg-sucursal-" + sucursalId).hide();
        $(".egresos-valor-sucursal-" + sucursalId).removeClass('egresos-sumar')
    }
}
function aplicarFiltrosEgresos() {
    if ($("#SucursalId").val() == '') {
        let itemsEgresosFiltro = $("input.egresos-filtro-sucursal");
        $(itemsEgresosFiltro).each(function (idx, vl) {
            let sucursalId = vl.id.replace('egresos-filtro-sucursal-', "");
            aplicarFiltrosEgresosSucursal(sucursalId, vl.checked);
        });
    } else {
        aplicarFiltrosEgresosSucursal($("#SucursalId").val(), true);
    }

    //Sumar totales
    let totalEgresos = 0;

    let valores = $(".egresos-sumar");
    $(valores).each(function (idx, vl) {
        totalEgresos += parseFloat(vl.innerText);
    });

    $("#egresos-total").text(totalEgresos);

}

//El parametro checked indica si la sucursal esta seleccionada
//para ver
function aplicarFiltrosComprasSucursal(sucursalId, checked) {

    if (checked) {
        $(".compras-reg-sucursal-" + sucursalId).show();

        //Se aplica el filtro por ambientes
        let itemsAmbientesFiltro = $("input.compras-filtro-ambiente");
        $(itemsAmbientesFiltro).each(function (idx, vl) {
            let ambienteId = vl.id.replace('compras-filtro-ambiente-', "");
            if (vl.checked) {
                $(".compras-reg-sucursal-" + sucursalId + ".compras-reg-ambiente-" + ambienteId).show();
                $(".compras-valor-sucursal-" + sucursalId + ".compras-valor-ambiente-" + ambienteId).addClass('compras-sumar');
            } else {
                $(".compras-reg-sucursal-" + sucursalId + ".compras-reg-ambiente-" + ambienteId).hide();
                $(".compras-valor-sucursal-" + sucursalId + ".compras-valor-ambiente-" + ambienteId).removeClass('compras-sumar');
            }
        });

    } else {
        $(".compras-reg-sucursal-" + sucursalId).hide();
        $(".compras-valor-sucursal-" + sucursalId).removeClass('compras-sumar')
    }
}
function aplicarFiltrosCompras() {
    if ($("#SucursalId").val() == '') {
        let itemsComprasFiltro = $("input.compras-filtro-sucursal");
        $(itemsComprasFiltro).each(function (idx, vl) {
            let sucursalId = vl.id.replace('compras-filtro-sucursal-', "");
            aplicarFiltrosComprasSucursal(sucursalId, vl.checked);
        });
    } else {
        aplicarFiltrosComprasSucursal($("#SucursalId").val(), true);
    }

    //Sumar totales
    let totalCompras = 0;

    let valores = $(".compras-sumar");
    $(valores).each(function (idx, vl) {
        totalCompras += parseFloat(vl.innerText);
    });

    $("#compras-total").text(totalCompras);

}

//El parametro checked indica si la sucursal esta seleccionada
//para ver
// function aplicarFiltrosVentasSucursal(sucursalId, checked) {

//     if (checked) {
//         $(".ventas-reg-sucursal-" + sucursalId).show();

//         //Se aplica el filtro por ambientes
//         let itemsAmbientesFiltro = $("input.ventas-filtro-ambiente");
//         $(itemsAmbientesFiltro).each(function (idx, vl) {
//             let ambienteId = vl.id.replace('ventas-filtro-ambiente-', "");
//             if (vl.checked) {
//                 $(".ventas-reg-sucursal-" + sucursalId + ".ventas-reg-ambiente-" + ambienteId).show();
//                 $(".ventas-valor-sucursal-" + sucursalId + ".ventas-valor-ambiente-" + ambienteId).addClass('ventas-sumar');
//             } else {
//                 $(".ventas-reg-sucursal-" + sucursalId + ".ventas-reg-ambiente-" + ambienteId).hide();
//                 $(".ventas-valor-sucursal-" + sucursalId + ".ventas-valor-ambiente-" + ambienteId).removeClass('ventas-sumar');
//             }
//         });

//     } else {
//         $(".ventas-reg-sucursal-" + sucursalId).hide();
//         $(".ventas-valor-sucursal-" + sucursalId).removeClass('ventas-sumar')
//     }
// }

function aplicarFiltrosVentasSucursal(sucursalId, checked) {
    if (checked) {
        $(".ventas-reg-sucursal-" + sucursalId).show();

        //Se aplica el filtro por ambientes (incluyendo Emergencia)
        let itemsAmbientesFiltro = $("input.ventas-filtro-ambiente");
        $(itemsAmbientesFiltro).each(function (idx, vl) {
            let ambienteId = vl.id.replace('ventas-filtro-ambiente-', "");
            
            if (vl.checked) {
                // Para el filtro de ambiente normal
                $(".ventas-reg-sucursal-" + sucursalId + ".ventas-reg-ambiente-" + ambienteId).show();
                $(".ventas-valor-sucursal-" + sucursalId + ".ventas-valor-ambiente-" + ambienteId).addClass('ventas-sumar');
                
                // Para el filtro de emergencia
                if (ambienteId === 'emergencia') {
                    $(".ventas-reg-sucursal-" + sucursalId + ".ventas-reg-ambiente-emergencia").show();
                    $(".ventas-valor-sucursal-" + sucursalId + ".ventas-valor-ambiente-emergencia").addClass('ventas-sumar');
                }
            } else {
                // Para el filtro de ambiente normal
                $(".ventas-reg-sucursal-" + sucursalId + ".ventas-reg-ambiente-" + ambienteId).hide();
                $(".ventas-valor-sucursal-" + sucursalId + ".ventas-valor-ambiente-" + ambienteId).removeClass('ventas-sumar');
                
                // Para el filtro de emergencia
                if (ambienteId === 'emergencia') {
                    $(".ventas-reg-sucursal-" + sucursalId + ".ventas-reg-ambiente-emergencia").hide();
                    $(".ventas-valor-sucursal-" + sucursalId + ".ventas-valor-ambiente-emergencia").removeClass('ventas-sumar');
                }
            }
        });
    } else {
        $(".ventas-reg-sucursal-" + sucursalId).hide();
        $(".ventas-valor-sucursal-" + sucursalId).removeClass('ventas-sumar');
    }
}
function aplicarFiltrosVentas() {
    if ($("#SucursalId").val() == '') {
        let itemsVentasFiltro = $("input.ventas-filtro-sucursal");
        $(itemsVentasFiltro).each(function (idx, vl) {
            let sucursalId = vl.id.replace('ventas-filtro-sucursal-', "");
            aplicarFiltrosVentasSucursal(sucursalId, vl.checked);
        });
    } else {
        aplicarFiltrosVentasSucursal($("#SucursalId").val(), true);
    }

    //Sumar totales
    let totalVentas = 0;

    let valores = $(".ventas-sumar");
    $(valores).each(function (idx, vl) {
        totalVentas += parseFloat(vl.innerText);
    });

    $("#ventas-total").text(totalVentas);

}

//El parametro checked indica si la sucursal esta seleccionada
//para ver
// function aplicarFiltrosMontosSucursal(sucursalId, checked) {

//     if (checked) {
//         $(".montos-reg-sucursal-" + sucursalId).show();

//         //Se aplica el filtro por ambientes
//         let itemsAmbientesFiltro = $("input.montos-filtro-ambiente");
//         $(itemsAmbientesFiltro).each(function (idx, vl) {
//             let ambienteId = vl.id.replace('montos-filtro-ambiente-', "");
//             if (vl.checked) {
//                 $(".montos-reg-sucursal-" + sucursalId + ".montos-reg-ambiente-" + ambienteId).show();
//                 $(".montos-valor-efectivo-sucursal-" + sucursalId + ".montos-valor-efectivo-ambiente-" + ambienteId).addClass('montos-efectivo-sumar');
//                 $(".montos-valor-visa-sucursal-" + sucursalId + ".montos-valor-visa-ambiente-" + ambienteId).addClass('montos-visa-sumar');
//                 $(".montos-valor-mastercard-sucursal-" + sucursalId + ".montos-valor-mastercard-ambiente-" + ambienteId).addClass('montos-mastercard-sumar');
//                 $(".montos-valor-cheques-sucursal-" + sucursalId + ".montos-valor-cheques-ambiente-" + ambienteId).addClass('montos-cheques-sumar');
//                 $(".montos-valor-transferencia-sucursal-" + sucursalId + ".montos-valor-transferencia-ambiente-" + ambienteId).addClass('montos-valor-sumar');
//                 $(".montos-valor-visalink-sucursal-" + sucursalId + ".montos-valor-visalink-ambiente-" + ambienteId).addClass('montos-visalink-sumar');
//                 $(".montos-valor-visanet-sucursal-" + sucursalId + ".montos-valor-visanet-ambiente-" + ambienteId).addClass('montos-visanet-sumar');
//                 $(".montos-valor-credito-sucursal-" + sucursalId + ".montos-valor-credito-ambiente-" + ambienteId).addClass('montos-credito-sumar');
//                 $(".montos-valor-seguro-sucursal-" + sucursalId + ".montos-valor-seguro-ambiente-" + ambienteId).addClass('montos-seguro-sumar');
//                 $(".montos-valor-sucursal-" + sucursalId + ".montos-valor-ambiente-" + ambienteId).addClass('montos-sumar');
//             } else {
//                 $(".montos-reg-sucursal-" + sucursalId + ".montos-reg-ambiente-" + ambienteId).hide();
//                 $(".montos-valor-efectivo-sucursal-" + sucursalId + ".montos-valor-efectivo-ambiente-" + ambienteId).removeClass('montos-efectivo-sumar');
//                 $(".montos-valor-visa-sucursal-" + sucursalId + ".montos-valor-visa-ambiente-" + ambienteId).removeClass('montos-visa-sumar');
//                 $(".montos-valor-mastercard-sucursal-" + sucursalId + ".montos-valor-mastercard-ambiente-" + ambienteId).removeClass('montos-mastercard-sumar');
//                 $(".montos-valor-cheques-sucursal-" + sucursalId + ".montos-valor-cheques-ambiente-" + ambienteId).removeClass('montos-cheques-sumar');
//                 $(".montos-valor-transferencia-sucursal-" + sucursalId + ".montos-valor-transferencia-ambiente-" + ambienteId).removeClass('montos-valor-sumar');
//                 $(".montos-valor-visalink-sucursal-" + sucursalId + ".montos-valor-visalink-ambiente-" + ambienteId).removeClass('montos-visalink-sumar');
//                 $(".montos-valor-visanet-sucursal-" + sucursalId + ".montos-valor-visanet-ambiente-" + ambienteId).removeClass('montos-visanet-sumar');
//                 $(".montos-valor-credito-sucursal-" + sucursalId + ".montos-valor-credito-ambiente-" + ambienteId).removeClass('montos-credito-sumar');
//                 $(".montos-valor-seguro-sucursal-" + sucursalId + ".montos-valor-seguro-ambiente-" + ambienteId).removeClass('montos-seguro-sumar');
//                 $(".montos-valor-sucursal-" + sucursalId + ".montos-valor-ambiente-" + ambienteId).removeClass('montos-sumar');
//             }
//         });

//     } else {
//         $(".montos-reg-sucursal-" + sucursalId).hide();
//         $(".montos-valor-efectivo-sucursal-" + sucursalId).removeClass('montos-efectivo-sumar')
//         $(".montos-valor-visa-sucursal-" + sucursalId).removeClass('montos-visa-sumar')
//         $(".montos-valor-mastercard-sucursal-" + sucursalId).removeClass('montos-mastercard-sumar')
//         $(".montos-valor-cheques-sucursal-" + sucursalId).removeClass('montos-cheques-sumar')
//         $(".montos-valor-transferencia-sucursal-" + sucursalId).removeClass('montos-transferencia-sumar')
//         $(".montos-valor-visalink-sucursal-" + sucursalId).removeClass('montos-visalink-sumar')
//         $(".montos-valor-visanet-sucursal-" + sucursalId).removeClass('montos-visanet-sumar')
//         $(".montos-valor-credito-sucursal-" + sucursalId).removeClass('montos-credito-sumar')
//         $(".montos-valor-seguro-sucursal-" + sucursalId).removeClass('montos-seguro-sumar')
//         $(".montos-valor-sucursal-" + sucursalId).removeClass('montos-sumar')
//     }
// }



function aplicarFiltrosMontosSucursal(sucursalId, checked) {

    if (checked) {
        $(".montos-reg-sucursal-" + sucursalId).show();

        //Se aplica el filtro por ambientes
        let itemsAmbientesFiltro = $("input.montos-filtro-ambiente");
        $(itemsAmbientesFiltro).each(function (idx, vl) {
            let ambienteId = vl.id.replace('montos-filtro-ambiente-', "");
            if (vl.checked) {
                $(".montos-reg-sucursal-" + sucursalId + ".montos-reg-ambiente-" + ambienteId).show();
                $(".montos-valor-efectivo-sucursal-" + sucursalId + ".montos-valor-efectivo-ambiente-" + ambienteId).addClass('montos-efectivo-sumar');
                $(".montos-valor-visa-sucursal-" + sucursalId + ".montos-valor-visa-ambiente-" + ambienteId).addClass('montos-visa-sumar');
                $(".montos-valor-mastercard-sucursal-" + sucursalId + ".montos-valor-mastercard-ambiente-" + ambienteId).addClass('montos-mastercard-sumar');
                $(".montos-valor-cheques-sucursal-" + sucursalId + ".montos-valor-cheques-ambiente-" + ambienteId).addClass('montos-cheques-sumar');
                $(".montos-valor-transferencia-sucursal-" + sucursalId + ".montos-valor-transferencia-ambiente-" + ambienteId).addClass('montos-valor-sumar');
                $(".montos-valor-visalink-sucursal-" + sucursalId + ".montos-valor-visalink-ambiente-" + ambienteId).addClass('montos-visalink-sumar');
                $(".montos-valor-visanet-sucursal-" + sucursalId + ".montos-valor-visanet-ambiente-" + ambienteId).addClass('montos-visanet-sumar');
                $(".montos-valor-credito-sucursal-" + sucursalId + ".montos-valor-credito-ambiente-" + ambienteId).addClass('montos-credito-sumar');
                $(".montos-valor-seguro-sucursal-" + sucursalId + ".montos-valor-seguro-ambiente-" + ambienteId).addClass('montos-seguro-sumar');
                $(".montos-valor-sucursal-" + sucursalId + ".montos-valor-ambiente-" + ambienteId).addClass('montos-sumar');
            } else {
                $(".montos-reg-sucursal-" + sucursalId + ".montos-reg-ambiente-" + ambienteId).hide();
                $(".montos-valor-efectivo-sucursal-" + sucursalId + ".montos-valor-efectivo-ambiente-" + ambienteId).removeClass('montos-efectivo-sumar');
                $(".montos-valor-visa-sucursal-" + sucursalId + ".montos-valor-visa-ambiente-" + ambienteId).removeClass('montos-visa-sumar');
                $(".montos-valor-mastercard-sucursal-" + sucursalId + ".montos-valor-mastercard-ambiente-" + ambienteId).removeClass('montos-mastercard-sumar');
                $(".montos-valor-cheques-sucursal-" + sucursalId + ".montos-valor-cheques-ambiente-" + ambienteId).removeClass('montos-cheques-sumar');
                $(".montos-valor-transferencia-sucursal-" + sucursalId + ".montos-valor-transferencia-ambiente-" + ambienteId).removeClass('montos-valor-sumar');
                $(".montos-valor-visalink-sucursal-" + sucursalId + ".montos-valor-visalink-ambiente-" + ambienteId).removeClass('montos-visalink-sumar');
                $(".montos-valor-visanet-sucursal-" + sucursalId + ".montos-valor-visanet-ambiente-" + ambienteId).removeClass('montos-visanet-sumar');
                $(".montos-valor-credito-sucursal-" + sucursalId + ".montos-valor-credito-ambiente-" + ambienteId).removeClass('montos-credito-sumar');
                $(".montos-valor-seguro-sucursal-" + sucursalId + ".montos-valor-seguro-ambiente-" + ambienteId).removeClass('montos-seguro-sumar');
                $(".montos-valor-sucursal-" + sucursalId + ".montos-valor-ambiente-" + ambienteId).removeClass('montos-sumar');
            }
        });

    } else {
        $(".montos-reg-sucursal-" + sucursalId).hide();
        $(".montos-valor-efectivo-sucursal-" + sucursalId).removeClass('montos-efectivo-sumar')
        $(".montos-valor-visa-sucursal-" + sucursalId).removeClass('montos-visa-sumar')
        $(".montos-valor-mastercard-sucursal-" + sucursalId).removeClass('montos-mastercard-sumar')
        $(".montos-valor-cheques-sucursal-" + sucursalId).removeClass('montos-cheques-sumar')
        $(".montos-valor-transferencia-sucursal-" + sucursalId).removeClass('montos-transferencia-sumar')
        $(".montos-valor-visalink-sucursal-" + sucursalId).removeClass('montos-visalink-sumar')
        $(".montos-valor-visanet-sucursal-" + sucursalId).removeClass('montos-visanet-sumar')
        $(".montos-valor-credito-sucursal-" + sucursalId).removeClass('montos-credito-sumar')
        $(".montos-valor-seguro-sucursal-" + sucursalId).removeClass('montos-seguro-sumar')
        $(".montos-valor-sucursal-" + sucursalId).removeClass('montos-sumar')
    }
}



function aplicarFiltrosMontos() {
    if ($("#SucursalId").val() == '') {
        let itemsMontosFiltro = $("input.montos-filtro-sucursal");
        $(itemsMontosFiltro).each(function (idx, vl) {
            let sucursalId = vl.id.replace('montos-filtro-sucursal-', "");
            aplicarFiltrosMontosSucursal(sucursalId, vl.checked);
        });
    } else {
        aplicarFiltrosMontosSucursal($("#SucursalId").val(), true);
    }

    //Sumar totales
    let totalMontos = 0;
    let valores = $(".montos-sumar");
    $(valores).each(function (idx, vl) {
        totalMontos += parseFloat(vl.innerText);
    });
    $("#montos-total").text(totalMontos);

    let totalMontosEfectivo = 0;
    let valoresEfectivo = $(".montos-efectivo-sumar");
    $(valoresEfectivo).each(function (idx, vl) {
        totalMontosEfectivo += parseFloat(vl.innerText);
    });
    $("#montos-efectivo-total").text(totalMontosEfectivo);

    let totalMontosVisa = 0;
    let valoresVisa = $(".montos-visa-sumar");
    $(valoresVisa).each(function (idx, vl) {
        totalMontosVisa += parseFloat(vl.innerText);
    });
    $("#montos-visa-total").text(totalMontosVisa);

    let totalMontosMastercard = 0;
    let valoresMastercard = $(".montos-mastercard-sumar");
    $(valoresMastercard).each(function (idx, vl) {
        totalMontosMastercard += parseFloat(vl.innerText);
    });
    $("#montos-mastercard-total").text(totalMontosMastercard);

    let totalMontosCheques = 0;
    let valoresCheques = $(".montos-cheques-sumar");
    $(valoresCheques).each(function (idx, vl) {
        totalMontosCheques += parseFloat(vl.innerText);
    });
    $("#montos-cheques-total").text(totalMontosCheques);

    let totalMontosTransferencia = 0;
    let valoresTransferencia = $(".montos-transferencia-sumar");
    $(valoresTransferencia).each(function (idx, vl) {
        totalMontosTransferencia += parseFloat(vl.innerText);
    });
    $("#montos-transferencia-total").text(totalMontosTransferencia);

    let totalMontosVisalink = 0;
    let valoresVisalink = $(".montos-visalink-sumar");
    $(valoresVisalink).each(function (idx, vl) {
        totalMontosVisalink += parseFloat(vl.innerText);
    });
    $("#montos-visalink-total").text(totalMontosVisalink);

    let totalMontosSeguro = 0;
    let valoresSeguro = $(".montos-seguro-sumar");
    $(valoresSeguro).each(function (idx, vl) {
        totalMontosSeguro += parseFloat(vl.innerText);
    });
    $("#montos-seguro-total").text(totalMontosSeguro);

    let totalMontosVisanet = 0;
    let valoresVisanet = $(".montos-visanet-sumar");
    $(valoresVisanet).each(function (idx, vl) {
        totalMontosVisanet += parseFloat(vl.innerText);
    });
    $("#montos-visanet-total").text(totalMontosVisanet);

    let totalMontosCredito = 0;
    let valoresCredito = $(".montos-credito-sumar");
    $(valoresCredito).each(function (idx, vl) {
        totalMontosCredito += parseFloat(vl.innerText);
    });
    $("#montos-credito-total").text(totalMontosCredito);

}

//El parametro checked indica si la sucursal esta seleccionada
//para ver
function aplicarFiltrosSubcajasSucursal(sucursalId, checked) {

    if (checked) {
        $(".subcajas-reg-sucursal-" + sucursalId).show();

        //Se aplica el filtro por ambientes
        let itemsAmbientesFiltro = $("input.subcajas-filtro-ambiente");
        $(itemsAmbientesFiltro).each(function (idx, vl) {
            let ambienteId = vl.id.replace('subcajas-filtro-ambiente-', "");
            if (vl.checked) {
                $(".subcajas-reg-sucursal-" + sucursalId + ".subcajas-reg-ambiente-" + ambienteId).show();
                $(".subcajas-ingresos-valor-sucursal-" + sucursalId + ".subcajas-ingresos-valor-ambiente-" + ambienteId).addClass('subcajas-ingresos-sumar');
                $(".subcajas-gastos-valor-sucursal-" + sucursalId + ".subcajas-gastos-valor-ambiente-" + ambienteId).addClass('subcajas-gastos-sumar');
                $(".subcajas-montos-apertura-valor-sucursal-" + sucursalId + ".subcajas-montos-apertura-valor-ambiente-" + ambienteId).addClass('subcajas-montos-apertura-sumar');
                $(".subcajas-totales-cierre-valor-sucursal-" + sucursalId + ".subcajas-totales-cierre-valor-ambiente-" + ambienteId).addClass('subcajas-totales-cierre-sumar');
            } else {
                $(".subcajas-reg-sucursal-" + sucursalId + ".subcajas-reg-ambiente-" + ambienteId).hide();
                $(".subcajas-ingresos-valor-sucursal-" + sucursalId + ".subcajas-ingresos-valor-ambiente-" + ambienteId).removeClass('subcajas-ingresos-sumar');
                $(".subcajas-gastos-valor-sucursal-" + sucursalId + ".subcajas-gastos-valor-ambiente-" + ambienteId).removeClass('subcajas-gastos-sumar');
                $(".subcajas-montos-apertura-valor-sucursal-" + sucursalId + ".subcajas-montos-apertura-valor-ambiente-" + ambienteId).removeClass('subcajas-montos-apertura-sumar');
                $(".subcajas-totales-cierre-valor-sucursal-" + sucursalId + ".subcajas-totales-cierre-valor-ambiente-" + ambienteId).removeClass('subcajas-totales-cierre-sumar');
            }
        });

    } else {
        $(".subcajas-reg-sucursal-" + sucursalId).hide();
        $(".subcajas-ingresos-valor-sucursal-" + sucursalId).removeClass('subcajas-ingresos-sumar')
        $(".subcajas-gastos-valor-sucursal-" + sucursalId).removeClass('subcajas-gastos-sumar')
        $(".subcajas-montos-apertura-valor-sucursal-" + sucursalId).removeClass('subcajas-montos-apertura-sumar')
        $(".subcajas-totales-cierre-valor-sucursal-" + sucursalId).removeClass('subcajas-totales-cierre-sumar')
    }
}
function aplicarFiltrosSubcajas() {
    if ($("#SucursalId").val() == '') {
        let itemsSubcajasFiltro = $("input.subcajas-filtro-sucursal");
        $(itemsSubcajasFiltro).each(function (idx, vl) {
            let sucursalId = vl.id.replace('subcajas-filtro-sucursal-', "");
            aplicarFiltrosSubcajasSucursal(sucursalId, vl.checked);
        });
    } else {
        aplicarFiltrosSubcajasSucursal($("#SucursalId").val(), true);
    }

    //Sumar totales
    //Ingresos
    let totalIngresosSubcajas = 0;

    let valoresIngresos = $(".subcajas-ingresos-sumar");
    $(valoresIngresos).each(function (idx, vl) {
        totalIngresosSubcajas += parseFloat(vl.innerText);
    });

    $("#subcajas-ingresos-total").text(totalIngresosSubcajas);

    //Gastos
    let totalGastosSubcajas = 0;

    let valoresGastos = $(".subcajas-gastos-sumar");
    $(valoresGastos).each(function (idx, vl) {
        totalGastosSubcajas += parseFloat(vl.innerText);
    });

    $("#subcajas-gastos-total").text(totalGastosSubcajas);

    //Montos de apertura
    let totalMontosAperturaSubcajas = 0;

    let valoresMontosApertura = $(".subcajas-montos-apertura-sumar");
    $(valoresMontosApertura).each(function (idx, vl) {
        totalMontosAperturaSubcajas += parseFloat(vl.innerText);
    });

    $("#subcajas-montos-apertura-total").text(totalMontosAperturaSubcajas);

    //Totales de cierre
    let totalTotalesCierreSubcajas = 0;

    let valoresTotalesCierre = $(".subcajas-totales-cierre-sumar");
    $(valoresTotalesCierre).each(function (idx, vl) {
        totalTotalesCierreSubcajas += parseFloat(vl.innerText);
    });

    $("#subcajas-totales-cierre-total").text(totalTotalesCierreSubcajas);

}

//Eliminar detalle (ingresos y egresos)
function eliminarDetalle(id) {
    var datos = { "detalleId": id };

    if (confirm('�Est� seguro/a que desea eliminar este registro? Los cambios no se podr�n revertir.')) {
        showLoading();
        $.ajax({
            url: '/Caja/EliminarDetalle',
            method: "POST",
            data: datos,
            traditional: true,
            success: function (data, textStatus) {
                window.location.reload();
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    }
}

function seleccionarCajaReabrir(cajaId) {
    cuentasVM.cajaSeleccionadaReabrir(cajaId);
}

function cerrarCaja(cajaId) {
    if (confirm("�Desea cerrar esta caja?")) {
        showLoading();
        $.ajax({
            url: "/Caja/Cerrar",
            data: {
                cajaId: cajaId
            },
            success: function (dataResult) {
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (error) {
                hideLoading();
                alert("Error: Verirfique su conexion o contacte con el administrador");
                console.log(error);
            }
        });
    }
}