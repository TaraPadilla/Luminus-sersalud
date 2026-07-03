var sumatotal = 0.00;
var total = 0.00;
var total2 = 0.00;
var total3 = 0.00;
var saldo= 0;

actualizarPreciosVenta();

/// Funciones Venta /////////
$('#boton').click(function(){
var fecha = document.getElementById('reservationtime').value;
var empleadoid = document.getElementById('empleadoselect').vaalue;

var datos = "";
$.ajax({
    method: "POST",
    data: datos,
    dataType: 'json',
    url: '/CrearPDF/VentasPdf?fecha= '+fecha+'&empleadoid='+empleadoid,
    traditional: true,
    success: function (data, state) {

    console.log(JSON.stringify(data));
    console.log(state);

    },
    error: function (data) {
        console.log(data);
    },

});


});
 
function LimpiarModal(){
    document.getElementById('montotxt').value="";
    document.getElementById('vuelto-n').innerHTML="0.00";
    saldo = total;

}


$('#buscartxt').keypress(function (e) {
    var keycode = e.which;
    if (keycode == '13') {
        var codigo = $("#buscartxt").val();

        var datos = { "codigo": codigo }

        $.ajax({
            method: "POST",
            data: datos,
            dataType: 'json',
            url: '/Productos/RetornarProducto ',
            traditional: true,
            success: function (data, state) {
                agregarATabla(data);
            },
            error: function (data) {
                console.log(data);
            },

        });

    }
});

function agregardetalle(codigo) {

    var datos = { "codigo": codigo }

    $.ajax({
        method: "POST",
        data: datos,
        dataType: 'json',
        url: '/Productos/RetornarProducto ',
        traditional: true,
        success: function (data, state) {
            agregarATabla(data);
        },
        error: function (data) {
            console.log(data);
        },

    });

}

function FiltrarProductos(codigo) {
    
    var datos = { "codigo": codigo }

    $.ajax({
        method: "GET",
        data: datos,
        dataType: "json",
        // contentType: "application/json; charset=utf-8",
        url: '/Venta/BusquedaProductos',
        traditional: true,
        success: function (data) {
            agregarALista(data);
        },
        error: function (data) {
            console.log(data);
            toastr.options = {
                "closeButton": false,
                "debug": false,
                "newestOnTop": false,
                "progressBar": true,
                "positionClass": "toast-top-center",
                "preventDuplicates": true,
                "onclick": null,
                "showDuration": "100",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "show",
                "hideMethod": "hide"
            };
            toastr.warning(data.responseJSON.message);

        },
    });
}

function agregarALista(data) {
    $('#listafiltro').empty(); 
    console.log("aa");

    data.forEach(function (item) {

        var temp = "";

        if(item.imagen == null || item.imagen == "")
        {
            temp = "/Images/480x.jpg";
        }
        else 
        {
            temp = String(item.imagen);
            
        }

        if(item.tipoProductoId === 10){
            var presentacionProducto = (item.presentacion == null) ? " --- " : (item.presentacion);
            var viaAdministracion = (item.viadmin == null) ? " --- " : (item.viadmin);
            var grupoTerapeutico = (item.grupoT == null) ? " --- " : (item.grupoT);
            var laboratorio = (item.lab == null) ? " --- " : (item.lab);

            var htmlTags = 
                        '<div class="col-md-4 pb-4" onclick=" agregardetalle(' + "'" + item.codigoReferencia + "'" + ')">' +
                            '<figure class="card card-img-top border-grey border-lighten-2" itemprop="associatedMedia" itemscope itemtype="http://schema.org/ImageObject">' +
                                '<a data-size="480x360">' +
                                '<img class="gallery-thumbnail card-img-top" src="'+ temp +'" alt="@item.Descripcion" itemprop="thumbnail"/>' +
                                '</a>'+
                                '<div class="card-body px-2">' +
                                    '<div>' +
                                    '<span class="float-right" style="background:red; color: white; padding: 0.45em; font-weight: 900;">'+ item.stock +'</span>'+
                                    '<span class="card-title float-left">'+item.nombreProducto+'</span>'+
                                    '</div>'+
                                    '<br>'+
                                    '<br>'+
                                    '<h6 class="card-text" style="font-size: 13px;">' + item.activoYConcentracion + '</h6>'+
                                    '<h6 class="card-text" style="font-size: 13px;">'+ presentacionProducto +' | '+ viaAdministracion + ' | ' + grupoTerapeutico +' | '+ laboratorio +'</h6>'+
                                    '<h6 class="card-text" style="font-size: 13px;">Q. '+ parseFloat(item.precio_5).toFixed(2)  +'</h6>'+
                                    '<h6 class="card-text" style="font-size: 13px;"> Dosis: '+item.dosis+'</h6>'+
                                '</div>'+
                            '</figure>'+
                        '</div>';


            $('#listafiltro').append(htmlTags);

        } 
        else if(item.tipoProductoId === 11){

            var categoria = (item.categoria == null) ? " --- " : (item.categoria);
            var marca = (item.marca == null) ? " --- " : (item.marca);
            var grupo = (item.grupo == null) ? " --- " : (item.grupo);

            var htmlTags = 
            '<div class="col-md-4 pb-4" onclick=" agregardetalle(' + "'" + item.codigoReferencia + "'" + ')">' +
                '<figure class="card card-img-top border-grey border-lighten-2" itemprop="associatedMedia" itemscope itemtype="http://schema.org/ImageObject">' +
                    '<a data-size="480x360">' +
                    '<img class="gallery-thumbnail card-img-top" src="'+ temp +'" alt="@item.Descripcion" itemprop="thumbnail"/>' +
                    '</a>'+
                    '<div class="card-body px-2">' +
                        '<div>' +
                        '<span class="float-right" style="background:red; color: white; padding: 0.45em; font-weight: 900;">'+ item.stock +'</span>'+
                        '<span class="card-title float-left">'+item.nombreProducto+'</span>'+
                        '</div>'+
                        '<br>'+
                        '<br>'+
                        '<h6 class="card-text" style="font-size: 13px;">'+ categoria +' | '+ grupo + ' | ' + marca + '</h6>'+
                        '<h6 class="card-text" style="font-size: 13px;">Q. '+ parseFloat(item.precio_5).toFixed(2)  +'</h6>'+
                    '</div>'+
                '</figure>'+
            '</div>';

            $('#listafiltro').append(htmlTags);
        }

    });

}


$(".cantidad-fila").on("click", function() {

    var monto = document.getElementsByClassName('cantidad-fila').value;
    var total = monto * sumatotal;
    
    document.getElementById('precio-total-a-pagar').innerHTML = total.toFixed(2);

    document.getElementById('total-a-pagar2').innerHTML = sumatotal.toFixed(2);



    // if (vuelto < 0) {
    //     document.getElementById('vuelto-n').innerHTML = "0.00";

    // } else {
    //     document.getElementById('vuelto-n').innerHTML = vuelto.toFixed(2);
    // }

});

$("#descuentoventa").on('change', function () {

    var descuento = document.getElementById('descuentoventa').value;

    total = document.getElementById('precio-total-a-pagar').value;
    sumatotal = parseFloat(total) - parseFloat(descuento);

    console.log(total);
    console.log(descuento);
    console.log(sumatotal);

    document.getElementById('precio-total-a-pagar').innerHTML = sumatotal.toFixed(2);
    document.getElementById('total-a-pagar2').innerHTML = sumatotal.toFixed(2);


})


function editarValorFila(r) {

    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;

    var val = r.value;

    console.log(val);

    var sig = r.parentNode.parentNode.querySelectorAll("td.precio-fila")[0].querySelectorAll("select")[0].value;

    var suma2 = val * sig;

    var porc = r.parentNode.parentNode.querySelectorAll("td.porcentaje")[0].querySelectorAll("input")[0].value

    var desc = suma2 * (parseInt(porc)/100);

    r.parentNode.parentNode.querySelectorAll("td.desc-t")[0].innerHTML=desc.toFixed(2);

    var totalfin = parseFloat(suma2) - parseFloat(desc);

    r.parentNode.parentNode.querySelectorAll("td.subtotal-t")[0].innerHTML = suma2.toFixed(2);
    r.parentNode.parentNode.querySelectorAll("td.total-t")[0].innerHTML = totalfin.toFixed(2);

    


    actualizarPreciosVenta();


}

// function vueltoenvio(){

//     var monto = document.getElementById('montotxt').value;

//     var vuelto = monto - total;
//     console.log(vuelto);
//     console.log(total);

//     if (vuelto <0){

//         vuelto = 0.00;
//     }

//     document.getElementById('vuelto-n').innerHTML = vuelto.toFixed(2);

// }

function actualizarPreciosVenta(){

    var lista = document.getElementById('contentdetalle').querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetalle').querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetalle').querySelectorAll("td.desc-t");



    //console.log(lista);

    lista.forEach(function (item) {
        total = parseFloat(total) + parseFloat(item.innerText);
    });

    listasub.forEach(function (item) {
        total2 = parseFloat(total2) + parseFloat(item.innerText);
    });

    listadesc.forEach(function (item) {
        total3 = parseFloat(total3) + parseFloat(item.innerText);
    });

    // console.log(val);
    //console.log(sig);
    // console.log(suma2);

    saldo = total;

    document.getElementById('subtotal-venta').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar').innerHTML = total.toFixed(2);
    document.getElementById('total-a-pagar2').innerHTML = total.toFixed(2);
    document.getElementById('total-saldo-venta').innerHTML = saldo.toFixed(2);

}



function editarPorcentaje(r) {

    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;

    var val = r.value;

    var sub = r.parentNode.parentNode.querySelectorAll("td.subtotal-t")[0].innerHTML;

    var desc = parseFloat(sub) * (parseFloat(val)/100);

   r.parentNode.parentNode.querySelectorAll("td.desc-t")[0].innerHTML=desc.toFixed(2);

    var totalfin = sub - desc;

    console.log(val);
    console.log(sub);
    console.log(desc);
    console.log(totalfin);

    r.parentNode.parentNode.querySelectorAll("td.total-t")[0].innerHTML = totalfin.toFixed(2);

    actualizarPreciosVenta();


}


function escogerPrecio(r) {

    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;

    var precio = r.value;

    var cantidad = r.parentNode.parentNode.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value;

    var suma2 = parseInt(cantidad) * parseFloat(precio);

    //var descuento = r.parentNode.parentNode.querySelectorAll("td.desc-t")[0].innerHTML;
    var porc = r.parentNode.parentNode.querySelectorAll("td.porcentaje")[0].querySelectorAll("input")[0].value;

    var desc = suma2 * (parseInt(porc) / 100);

    r.parentNode.parentNode.querySelectorAll("td.desc-t")[0].innerHTML = desc.toFixed(2);

    var totalfin = parseFloat(suma2) - parseFloat(desc);

    r.parentNode.parentNode.querySelectorAll("td.subtotal-t")[0].innerHTML = suma2.toFixed(2);
    r.parentNode.parentNode.querySelectorAll("td.total-t")[0].innerHTML = totalfin.toFixed(2);

    actualizarPreciosVenta();
}


$("#montotxt").on("keyup", function () {

    var monto = document.getElementById('montotxt').value;

    var vuelto = monto - total;
    saldo = total - monto;

    

    if (vuelto <0){

        vuelto = 0.00;
    }

    if (saldo <0)
    {
        saldo=0;
    }

    console.log(saldo);
    document.getElementById('vuelto-n').innerHTML = vuelto.toFixed(2);
    document.getElementById('total-saldo-venta').innerHTML = saldo.toFixed(2);

});

$("#clienteselect").on('change', function () {

    var nombre = { "nombre": $("#clienteselect option:selected").text() }
    if(this.value == "--- CF ---"){
        $("#Nit").val("CF");
        $("#direccion").val("CF");
    }
    else {

    $.ajax({
        method: "POST",
        data: nombre,
        dataType: 'json',
        url: '/Cliente/RetornarCliente',
        traditional: true,
        success: function (data, state) {
            console.log(data);

            if (data === null) {
                document.getElementById("Nit").value = "";
                document.getElementById("nombres").value = nombre.nombre;
                document.getElementById("direccion").value = "";
                return;
            }

            document.getElementById("Nit").value = data.nit;
            // document.getElementById("nombres").value = data.nombre;
            document.getElementById("direccion").value = data.direccion;




        },
        error: function (data) {

        },

    });
}

});

function agregarATabla(data) {
    //total= total + data.precio;
    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;


    var htmlTags = '<tr style="background-color:rgba(255,211,105,0.5);"> <td> ' + data.codigoReferencia + '</td>' +
        '<td> ' + data.nombreProducto + ' </td>' +
        '<td class="cantidad-fila" ><input type="text" onchange="editarValorFila(this)" style="background-color:rgba(255,211,105,0.5);" value="1" class="form-control "></td>' +
        '<td class="precio-fila">' +
        '<select name=""  class="form-control selectpreciosventa" style="background-color:rgba(255,211,105,0.5);" onchange="escogerPrecio(this)">' +
        // '<option value="' + data.precio.toFixed(2) + '">Precio al Publico - Q' + data.precio.toFixed(2) + '</option>' +
        // '<option value="' + data.precio_2.toFixed(2) + '">Precio mayorista - Q' + data.precio_2.toFixed(2) + '</option>' +
        // '<option value="' + data.precio_3.toFixed(2) + '">Precio de fardo - Q' + data.precio_3.toFixed(2) + '</option>' +
        // '<option value="' + data.precio_4.toFixed(2) + '">Precio docena - Q' + data.precio_4.toFixed(2) + '</option>' +
      
        '<option value="' + data.precio_5.toFixed(2) + '"> Precio público - Q' + data.precio_5.toFixed(2) + '</option>' +
        '<option value="0"> Insumo - Q' + parseFloat(0.00).toFixed(2) + '</option>' +
        // '<option value="' + data.precio_6.toFixed(2) + '">Precio caja - Q' + data.precio_6.toFixed(2) + '</option>' +
        '</select>' +
        '</td>' +
        '<td class="porcentaje"><input type="text" value="0" class="form-control" style="background-color:rgba(255,211,105,0.5);" onchange="editarPorcentaje(this)"></td>' +
        '<td class="desc-t">0.00</td>' +
        '<td class="subtotal-t">' + data.precio_5.toFixed(2) + '</td>' +
        '<td class="total-t">' + data.precio_5.toFixed(2) + '</td>' +
        '<td><button  type="button"  class="btn btn-danger btn-sm" onclick="eliminarFila(this)">Quitar</button></td>' +
        '<td style="display:none;" class="nuevo-detalle"> ' + data.id + ' </td></tr>'


    //var total= '<span>'+sumatotal.toFixed(2)+'</span>'

    $('#contentdetalle').append(htmlTags);


    actualizarPreciosVenta();

    // Es para que muestre un mensaje de alerta al tener exito en buscar el producto y agregar.
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-center",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": "100",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "show",
        "hideMethod": "hide"
    };
    toastr.success('Producto agregado!');
}

function GuardarEnvio() {

    var detalleVenta = new Array();


    $.each($("#tableventa tbody tr"), function () {

        detalleVenta.push({
            
            "ProductoId": $(this).find("td").eq(9).html(),
            "Cantidad": this.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value,
            "Precio": this.querySelectorAll("td.precio-fila")[0].querySelectorAll("select")[0].value,
            "Descuento": $(this).find("td").eq(5).html(),
            "Subtotal": $(this).find("td").eq(6).html(),
            "Total": $(this).find("td").eq(7).html(),
        });
    });


    var datos = {
        "datosenvio": {

           // "NombrePiloto": document.getElementById('nombrepiloto').value,
            "Ruta": document.getElementById('rutaselect').value,
            "fecha": document.getElementById("reservationtime").value,
            "DireccionExacta": document.getElementById('direccionexacta').value,
            "Nombre": $(".selectcliente option:selected").text(),
            "NoComprobante": document.getElementById('nocomprobante').value,
            "Nit": document.getElementById('Nit').value,
            "EmpleadoId": document.getElementById('empleadoid').value,
            "UserId": document.getElementById('usuarioselect').value,

        },
        "detalle": detalleVenta,



    };

    console.log(datos);


    $.ajax({
        method: "POST",
        data: JSON.stringify(datos),
        "dataType": "json",
        "contentType": "application/json",
        url: '/Venta/GuardarEnvio',
        traditional: true,
        success: function (data) {
            // si tiene exito, reiniciemos la pag
            // pero marquemos que tuvo exito
            console.log(data);
            window.location.href = '/Venta/Nuevo/';

        },
        error: function (data) {
            alert(data.error);
        }
    });




}

function ModificarEnvio(id) {
    //usar camelCase para variables ok
    var ids_nuevos = new Array();
    var ids_insertados = new Array();
    var detalleVenta = new Array();


    $.each($("#tableventa tbody tr td.idProd"), function () {

        ids_insertados.push({
            "Ids": $(this).html(),
        });
    });
    datos
    $.each($("#tableventa tbody tr td.nuevo-detalle").parent(), function () {

        detalleVenta.push({
            "ProductoId": $(this).find("td").eq(9).html(),
            "Cantidad": this.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value,
            "Precio": this.querySelectorAll("td.precio-fila")[0].querySelectorAll("select")[0].value,
            "Descuento": $(this).find("td").eq(5).html(),
            "Subtotal": $(this).find("td").eq(6).html(),
            "Total": $(this).find("td").eq(7).html(),
        });
    });


    var datos = {
        "datosenvio": {
            "Id": id,
            //"NombrePiloto": document.getElementById('nombrepiloto').value,
            "Ruta": document.getElementById('rutaselect').value,
            "fecha": document.getElementById("reservationtime").value,
            "DireccionExacta": document.getElementById('direccionexacta').value,
            "Nombre": $(".selectcliente option:selected").text(),
            "NoComprobante": document.getElementById('nocomprobante').value,
            "Nit": document.getElementById('Nit').value,
            "EmpleadoId": document.getElementById('empleadoid').value,
            "UserId": document.getElementById('usuarioselect').value,

        },
        //listas
        "nuevos": ids_insertados,
        "detalle": detalleVenta,


    };



    $.ajax({
        method: "POST",
        data: JSON.stringify(datos),
        "dataType": "json",
        "contentType": "application/json",
        url: '/Envio/ModificarEnvio',
        traditional: true,
        success: function (data, textStatus) {
            // si tiene exito, reiniciemos la pag
            // pero marquemos que tuvo exito
            // alert(data);
            location.reload();
        },
        error: function (data) {
            alert(data.error);
        }
    });

}


function eliminarFila(r) {
    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;


    var i = r.parentNode.parentNode.rowIndex;
    document.getElementById("tableventa").deleteRow(i);

   
    actualizarPreciosVenta();
}



function GuardarVentaFarmacia() {

    var detalleVenta = new Array();
   

    $.each($("#tableventa tbody tr"), function () {
        detalleVenta.push({
            "ProductoId": parseInt($(this).find("td").eq(9).html()),
            "Cantidad": parseInt(this.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value),
            "Precio": parseFloat(this.querySelectorAll("td.precio-fila")[0].querySelectorAll("select")[0].value),
            "Descuento": parseFloat($(this).find("td").eq(5).html()),
            "Subtotal": parseFloat($(this).find("td").eq(6).html()),
            "Total": parseFloat($(this).find("td").eq(7).html()) 
        });
    });

   // var pago = document.getElementById('formapagoselect').value;
    

    var datos = {
        "encabezado": {
            // formapagoselect-venta
            "Nombres": $("#clienteselect option:selected").text(),
            "NoComprobante": document.getElementById('nocomprobante').value,
            // "ClienteId": document.getElementById('clienteselect').value,
            "Nit": document.getElementById('Nit').value,
            "Direccion": document.getElementById('direccion').value,
            "FormaPago": parseInt(document.getElementById('formapagoselect-venta').value),
            "Monto": parseFloat(document.getElementById('montotxt').value),
            "Vuelto" :  parseFloat(document.getElementById('vuelto-n').innerText), 
            "EmpleadoId": parseInt(document.getElementById('empleadoid').value),
        },

        "detalle": detalleVenta,
    }

    console.log(datos);

//    var data = { "datos": datos }

   if(saldo==0){

    $.ajax({
        method: "POST",
        data: JSON.stringify(datos),
        "dataType": "json",
        "contentType": "application/json",
        url: '/Venta/GuardarVentaFarmacia' ,
        traditional: true,
        success: function (data) {
            window.open('/CrearPDF/ReciboVentaPdf/'+ data,'_blank');
            window.location.href = '/Venta/Nuevo/';
        },
        error: function (data) {
            alert(data.responseJSON.messsage);
        }
    });

   }
   else{
    alert("Error, Hay Saldo Pendiente");
   }

}



function ModificarVenta(id) {
    //usar camelCase para variables ok
    var ids_nuevos = new Array();
    var ids_insertados = new Array();
    var detalleVenta = new Array();
    // var noComprobante = document.getElementById('nocomprobante').value;
    // var proveedor = document.getElementById('proveedorselect').value;
    // var nombreVendedor = document.getElementById('nombrevendedor').value;
    // var fechaLimite = document.getElementById('fechalimite').value;
    // var fechaRecepcion = document.getElementById('fecharecepcion').value;

    // $.each($("#tableventa-c tbody tr td.idProd"), function() {
    //     ids_insertados.push($(this).html());
    // });

    // $.each($("#tableventa-c tbody tr td.nuevo-detalle"), function() {
    //     ids_nuevos.push({
    //         "Ids": $(this).html(),
    //     })

    // });

    $.each($("#tableventa tbody tr td.idProd"), function () {

        ids_insertados.push({
            "Ids": $(this).html(),
        });
    });

    $.each($("#tableventa tbody tr td.nuevo-detalle").parent(), function () {

        detalleVenta.push({
            "ProductoId": $(this).find("td").eq(9).html(),
            "Cantidad":this.parentNode.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value,
            "Precio": this.parentNode.querySelectorAll("td.precio-fila")[0].querySelectorAll("select")[0].value,
            "Descuento": $(this).find("td").eq(5).html(),
            "Subtotal": $(this).find("td").eq(6).html(),
            "Total": $(this).find("td").eq(7).html()
        });
    });



    console.log(ids_insertados);
    console.log(detalleVenta);

   


    var datos = {
        "encabezado": {
            "Id": id,
            "Nombres": document.getElementById('nombres').value,
            "NoComprobante": document.getElementById('nocomprobante').value,
            "ClienteId": document.getElementById('clienteselect-m').value,
            "Nit": document.getElementById('Nit').value,
            "Direccion": document.getElementById('direccion').value,
            "FormaPago": document.getElementById('selectformapago').value,
            "EmpleadoId": document.getElementById('empleadoid').value,
        },
        //listas
        "nuevos": ids_insertados,
        "detalle": detalleVenta,


    };


    $.ajax({
        method: "POST",
        data: JSON.stringify(datos),
        "dataType": "json",
        "contentType": "application/json",
        url: '/Venta/ModificarVenta',
        traditional: true,
        success: function (data, textStatus) {
            // si tiene exito, reiniciemos la pag
            // pero marquemos que tuvo exito
            // alert(data);
            location.reload();
        },
        error: function (data) {
            alert(data.error);
        }
    });

}