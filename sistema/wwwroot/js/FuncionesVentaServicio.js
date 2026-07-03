var sumatotal = 0.00;
var total = 0.00;
var total2 = 0.00;
var total3 = 0.00;


/// Funciones Venta /////////

$('#servicioselect').on('change', function(){
   
        var id = document.getElementById('servicioselect').value;

        var datos = { "id": id }

        $.ajax({
            method: "POST",
            data: datos,
            dataType: 'json',
            url: '/Servicio/RetornarServicios',
            traditional: true,
            success: function(data, state) {
                console.log(data);
                agregarATabla(data);
            },
            error: function(data) {
                console.log(data);
            },

        });

    
});

$("#montotxt-s").on("keyup", function() {

    var monto = document.getElementById('montotxt-s').value;

    var vuelto = monto - sumatotal.toFixed(2);


    if (vuelto < 0) {
        document.getElementById('vuelto-n-s').innerHTML = "0.00";

    } else {
        document.getElementById('vuelto-n-s').innerHTML = vuelto.toFixed(2);
    }

});


function editarValorFila(r) {

    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;

    var val = r.value;

    var sig = r.parentNode.parentNode.querySelectorAll("td.precio-fila")[0].innerHTML;

    var suma2 = val * sig;

    var porc = r.parentNode.parentNode.querySelectorAll("td.porcentaje")[0].querySelectorAll("input")[0].value;

    var desc = suma2 * (parseInt(porc)/100);

    r.parentNode.parentNode.querySelectorAll("td.desc-t")[0].innerHTML=desc.toFixed(2);

    var totalfin = parseFloat(suma2) - parseFloat(desc);

    r.parentNode.parentNode.querySelectorAll("td.subtotal-t")[0].innerHTML = suma2.toFixed(2);
    r.parentNode.parentNode.querySelectorAll("td.total-t")[0].innerHTML = totalfin.toFixed(2);

    


    var lista = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.desc-t");



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


    document.getElementById('subtotal-venta-s').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-s').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-s').innerHTML = total.toFixed(2);
    document.getElementById('total-a-pagar2-s').innerHTML = total.toFixed(2);

    //sumatotal = sumatotal - a.querySelectorAll("td.precio-fila")[0].innerHTML
    // primero aseguremos si entra el cuerpo this en el parametro va va, creo que usaremos parentnode

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

    var lista = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.desc-t");



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


    document.getElementById('subtotal-venta-s').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-s').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-s').innerHTML = total.toFixed(2);
    document.getElementById('total-a-pagar2-s').innerHTML = total.toFixed(2);

    //sumatotal = sumatotal - a.querySelectorAll("td.precio-fila")[0].innerHTML
    // primero aseguremos si entra el cuerpo this en el parametro va va, creo que usaremos parentnode
    //dale


}



$("#clienteselect-s").on('change', function() {

    var nombre = { "nombre":  $("#clienteselect-s option:selected").text()}

    $.ajax({
        method: "POST",
        data: nombre,
        dataType: 'json',
        url: '/Cliente/RetornarCliente',
        traditional: true,
        success: function(data, state) {
            console.log(data);

            if(data === null)
            {
                document.getElementById("Nit-s").value = "";
                document.getElementById("nombres-s").value = nombre.nombre;
                document.getElementById("direccion-s").value = "";
                return;
            }
            
            document.getElementById("Nit-s").value = data.nit;
            document.getElementById("nombres-s").value = data.nombre;
            document.getElementById("direccion-s").value = data.direccion;

           


        },
        error: function(data) {

        },

    });
});

function agregardetalle(id){

    var datos = { "id": id }

    $.ajax({
        method: "POST",
        data: datos,
        dataType: 'json',
        url: '/Servicio/RetornarServicios ',
        traditional: true,
        success: function(data, state) {
            agregarATabla(data);

        },
        error: function(data) {
            console.log(data);
        },

    });

}

function agregarATabla(data) {
    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;


    var htmlTags = '<tr style="background-color:rgba(255,211,105,0.5);"> <td> ' + data.id + '</td>' +
        '<td> ' + data.nombreServicio + ' </td>' +
        '<td class="cantidad-fila" ><input type="text" onchange="editarValorFila(this)" style="background-color:rgba(255,211,105,0.5);" value="1" class="form-control "></td>' +
        '<td class="precio-fila"> ' + data.precio.toFixed(2) + ' </td>' +
        '<td class="porcentaje"><input type="text" value="0" class="form-control" style="background-color:rgba(255,211,105,0.5);" onchange="editarPorcentaje(this)"></td>' +
        '<td class="desc-t">0.00 </td>' +
        '<td class="subtotal-t">' + data.precio.toFixed(2) + '</td>' +
        '<td class="total-t">' + data.precio.toFixed(2) + '</td>' +
        '<td><button  type="button"  class="btn btn-danger btn-sm" onclick="eliminarFila(this)">Quitar</button></td>' +
        '<td style="display:none;" class="nuevo-detalle"> ' + data.id + ' </td></tr>'

        $('#contentdetalle-s').append(htmlTags);

    //var total= '<span>'+sumatotal.toFixed(2)+'</span>'

    var lista = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.desc-t");


    lista.forEach(function (item) {
        total = parseFloat(total) + parseFloat(item.innerText);
    });

    listasub.forEach(function (item) {
        total2 = parseFloat(total2) + parseFloat(item.innerText);
    });

    listadesc.forEach(function (item) {
        total3 = parseFloat(total3) + parseFloat(item.innerText);
    });

    // var descuento = document.getElementById('descuentoventa').value;


    document.getElementById('subtotal-venta-s').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-s').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-s').innerHTML = total.toFixed(2);
    document.getElementById('total-a-pagar2-s').innerHTML = total.toFixed(2);

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
toastr.success('Servicio agregado!');
}

function FiltrarServicios(nombre){

    var datos = { "nombre": nombre }

    $.ajax({
        method: "POST",
        data: datos,
        dataType: "json",
        url: '/VentaServicio/BusquedaServicios',
        traditional: true,
        success: function(data, state) {
          
            console.log(data);
            agregarALista(data);

        },
        error: function(data) {
            console.log(data);
        },

    });

}

function agregarALista(data) {
  
    $('#listafiltro-s').empty(); // esto si funciona va? simon
    

    data.forEach(function(item){
       

        var htmlTags =  '<article class="list--item" onclick=" agregardetalle('+item.id+')">'+
        '<figure>'+
        '<img src="/Images/imagenotavailable.png" alt="@item.Descripcion" width="100">' +
        '<header>'+
        '<h2>'+item.nombreServicio+'</h2>'+
            '</header>'+
            '<figcaption>'+
                '<h2>Q '+item.precio.toFixed(2)+'</h2>'+
            '</figcaption>'+
         '</figure>'+
        '</article>'


        $('#listafiltro-s').append(htmlTags);


    });
}

function eliminarFila(r, precio) {
    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;

    var i = r.parentNode.parentNode.rowIndex;
    document.getElementById("tableventaservicio").deleteRow(i);

    // var a = r.parentNode.parentNode;

    // sumatotal = sumatotal - a.querySelectorAll("td.total-t")[0].innerHTML


    // document.getElementById('precio-total-a-pagar-s').innerHTML = sumatotal.toFixed(2);

    // document.getElementById('total-a-pagar2-s').innerHTML = sumatotal.toFixed(2);

    var lista = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetalle-s').parentNode.querySelectorAll("td.desc-t");


    lista.forEach(function (item) {
        total = parseFloat(total) + parseFloat(item.innerHTML);
    });

    listasub.forEach(function (item) {
        total2 = parseFloat(total2) + parseFloat(item.innerHTML);
    });

    listadesc.forEach(function (item) {
        total3 = parseFloat(total3) + parseFloat(item.innerHTML);
    });

    // var descuento = document.getElementById('descuentoventa').value;


    document.getElementById('subtotal-venta-s').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-s').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-s').innerHTML = total.toFixed(2);
    document.getElementById('total-a-pagar2-s').innerHTML = total.toFixed(2);


    //var total= '<span>'+sumatotal.toFixed(2)+'</span>'
}

function GuardarVentaServicio() {

    var detalleVentaServicio = new Array();
   

    $.each($("#tableventaservicio tbody tr"), function() {

        detalleVentaServicio.push({
            "ServicioId": parseInt($(this).find("td").eq(9).html()),
            "Cantidad": parseInt(this.parentNode.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value) ,
            "Precio": parseFloat($(this).find("td").eq(3).html()),
            "Descuento": parseFloat($(this).find("td").eq(5).html()),
            "Subtotal": parseFloat($(this).find("td").eq(6).html()),
            "Total": parseFloat($(this).find("td").eq(7).html()) 
        });
    });


    var datos = {
        "VentaServicio": {
            "Nombres":  $("#clienteselect-s option:selected").text(),
            "NoComprobante": document.getElementById('nocomprobante-s').value,
            // "ClienteId": document.getElementById('clienteselect').html(),
            "Nit": document.getElementById('Nit-s').value,
            "Direccion": document.getElementById('direccion-s').value,
            "FormaPago": document.getElementById('selectformapago-s').value,
            "EmpleadoId": parseInt(document.getElementById('empleadoid').value),
        },
        "DetalleServicios": detalleVentaServicio,
    };
 
    console.log(datos);
    

    $.ajax({
        method: "POST",
        data: JSON.stringify(datos),
        "dataType": "json",
        "contentType": "application/json",
        url: '/VentaServicio/GuardarVentaServicio',
        traditional: true,
        success: function(data) {
            // // si tiene exito, reiniciemos la pag
            // pero marquemos que tuvo exito
        // window.open('/CrearPDF/ReciboVentaPdf/'+data,'_blank');
            window.location.href = '/VentaServicio/Nuevo/';
            
        },
        error: function(data) {
            alert(data.responseJSON.message);
        }
    });
}

function ModificarVentaServicio(id) {
    //usar camelCase para variables ok
    var ids_nuevos = new Array();
    var ids_insertados = new Array();
    var detalleVenta = new Array();

    $.each($("#tableventaservicio tbody tr td.idProd"), function() {

        ids_insertados.push({
            "Ids": $(this).html(),
        });
    });

    $.each($("#tableventaservicio tbody tr td.nuevo-detalle").parent(), function() {

        detalleVenta.push({
            "ServicioId": $(this).find("td").eq(9).html(),
            "Cantidad": this.parentNode.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value,
            "Precio": $(this).find("td").eq(3).html(),
            "Descuento": $(this).find("td").eq(5).html(),
            "Subtotal": $(this).find("td").eq(6).html(),
            "Total": $(this).find("td").eq(7).html(),

        });
    });

    console.log(ids_insertados);
    console.log(detalleVenta);

    var datos = {
        "encabezado": {
            "Id": id,
            "Nombres": document.getElementById('nombres').value,
            "NoComprobante": document.getElementById('nocomprobante').value,
            "ClienteId": document.getElementById('clienteselect').value,
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
        url: '/VentaServicio/ModificarVentaServicio',
        traditional: true,
        success: function(data, textStatus) {
            // si tiene exito, reiniciemos la pag
            // pero marquemos que tuvo exito
           // alert(data);
           window.location.href='/VentaServicio/Lista/';
        },
        error: function(data) {
            alert(data.error);
        }
    });



}