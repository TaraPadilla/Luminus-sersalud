var sumatotal = 0.00;
var total = 0.00;
var total2 = 0.00;
var total3 = 0.00;
var saldo = 0;

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
        method: "POST",
        data: datos,
        dataType: "json",
        url: '/Venta/BusquedaProductos',
        traditional: true,
        success: function (data, state) {

            console.log(data);
            agregarALista(data);

        },
        error: function (data) {
            console.log(data);
        },

    });

}

function agregarALista(data) {

    $('#listafiltro').empty(); // esto si funciona va? simon


    data.forEach(function (item) {


        var htmlTags = '<article class="list--item" onclick=" agregardetalle(' + "'" + item.codigoReferencia + "'" + ')">' +
            '<figure>' +
            '<img src="/assets/images/nodisponible.png" alt="@item.Descripcion">' +
            '<header>' +
            '<div class="floater">' +
            +item.stock +
            '</div>' +
            '<h2>' + item.nombreProducto + ' | ' + item.codigoReferencia + '</h2>' +
            '</header>' +
            '<figcaption>' +
            '<h2>Q ' + item.precio.toFixed() + '</h2>' +
            '</figcaption>' +
            '</figure>' +
            '</article>'


        $('#listafiltro').append(htmlTags);


    });


}


$(".cantidad-fila").on("click", function() {

    var monto = document.getElementsByClassName('cantidad-fila').value;

    var total = monto * sumatotal;


    document.getElementById('precio-total-a-pagar').innerHTML = total.toFixed(2);

    


   

});

$("#descuentoventa").on('change', function () {

    var descuento = document.getElementById('descuentoventa').value;

    total = document.getElementById('precio-total-a-pagar').value;
    sumatotal = parseFloat(total) - parseFloat(descuento);

    console.log(total);
    console.log(descuento);
    console.log(sumatotal);

    document.getElementById('precio-total-a-pagar').innerHTML = sumatotal.toFixed(2);
   


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

    


    var lista = document.getElementById('contentdetalle').querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetalle').querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetalle').querySelectorAll("td.desc-t");



    console.log(lista);

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
    console.log(sig);
    // console.log(suma2);


    document.getElementById('subtotal-venta').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar').innerHTML = total.toFixed(2);
    

  


}

$("#montoenviotxt").on("keyup", function () {

    var monto = document.getElementById('montoenviotxt').value;

    var vuelto = monto - total;
    saldo = total - monto;
    console.log(vuelto);
    console.log(total);

    if (vuelto <0){

        vuelto = 0;
    }

    if (saldo <0)
    {
        saldo=0;
    }

    document.getElementById('vuelto-envio').innerHTML = vuelto.toFixed(2);
    document.getElementById('total-saldo-envio').innerHTML = saldo.toFixed(2);



});

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
    document.getElementById('total-a-pagar-envio').innerHTML = total.toFixed(2);
   document.getElementById('total-saldo-envio').innerHTML = total.toFixed(2);

    

  

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

    var lista = document.getElementById('contentdetalle').querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetalle').querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetalle').querySelectorAll("td.desc-t");



    //console.log(lista);

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

function LimpiarModal(){
    document.getElementById('montoenviotxt').value="";
    document.getElementById('vuelto-envio').innerHTML="0.00";
    saldo = total;

}





$("#clienteselect").on('change', function () {

    var nombre = { "nombre": $("#clienteselect option:selected").text() }

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
            document.getElementById("nombres").value = data.nombre;
            document.getElementById("direccion").value = data.direccion;




        },
        error: function (data) {

        },

    });
});

// $("#usuarioselect").on('change', function () {

//     console.log("Usuariooo");
// });

// $("#rutaselect").on('change', function () {

//     console.log("Rutaa");
// });

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
        '<option value="' + data.precio.toFixed(2) + '">Precio al Publico - Q' + data.precio.toFixed(2) + '</option>' +
        '<option value="' + data.precio_2.toFixed(2) + '">Precio mayorista - Q' + data.precio_2.toFixed(2) + '</option>' +
        '<option value="' + data.precio_3.toFixed(2) + '">Precio de fardo - Q' + data.precio_3.toFixed(2) + '</option>' +
        '<option value="' + data.precio_4.toFixed(2) + '">Precio cliente especial - Q' + data.precio_4.toFixed(2) + '</option>' +
        '<option value="' + data.precio_5.toFixed(2) + '">Precio cuenta clave - Q' + data.precio_5.toFixed(2) + '</option>' +
        '<option value="' + data.precio_6.toFixed(2) + '">Precio modificable - Q' + data.precio_6.toFixed(2) + '</option>' +
        '</select>' +
        '</td>' +
        '<td class="porcentaje"><input type="text" value="0" class="form-control" style="background-color:rgba(255,211,105,0.5);" onchange="editarPorcentaje(this)"></td>' +
        '<td class="desc-t">0.00</td>' +
        '<td class="subtotal-t">' + data.precio.toFixed(2) + '</td>' +
        '<td class="total-t">' + data.precio.toFixed(2) + '</td>' +
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


function RecibirEnvio(id){

    var pago = document.getElementById('formapagoselect').value;
    var monto = total;


    var datos = {
        
            // "Id": id,
            // "FormaPago" : pago,
            // "Monto" : monto,

    
     

    };

   // var datos = { "id": id }

   if(saldo == 0)
   {

    $.ajax({
        method: "POST",
        data: datos,
        "dataType": "json",
        "contentType": "application/json",
        url: '/Envio/RecibirPedido?Id='+id+'&FormaPago='+pago+'&Monto='+monto,
        traditional: true,
        success: function (data, textStatus) {
            // si tiene exito, reiniciemos la pag
            // pero marquemos que tuvo exito
            //alert(data);
            console.log(data);
           location.reload();
            
        },
        error: function (data) {
            alert(data.error);
        }
    });

   }
   else{
       alert("Error, Hay saldo Pendiente");
   }

    


}





function eliminarFila(r) {
    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;


    var i = r.parentNode.parentNode.rowIndex;
    document.getElementById("tableventa").deleteRow(i);

   
    actualizarPreciosVenta();
   
}



