var total = 0.00;
var total2 = 0.00;
var total3 = 0.00;

actualizarPrecios();


function BuscarProductoServicio() {

    var codigo = document.getElementById('buscarcotitxt').value;
    var codcotizacion = $('input[name="radiocoti"]:checked').val();;

    console.log(codcotizacion);
    console.log(codigo);

    if (codcotizacion === "productos") {

        var datos = { "codigo": codigo }

        $.ajax({
            method: "POST",
            data: datos,
            dataType: 'json',
            url: '/Productos/RetornarProducto',
            traditional: true,
            success: function (data, state) {
                agregarATablaProducto(data);

            },
            error: function (data) {
                console.log(data);
            },

        });

    }
    else if (codcotizacion === "servicios") {
        var datos = { "id": codigo }
        console.log(datos);

        $.ajax({
            method: "POST",
            data: datos,
            dataType: 'json',
            url: '/Servicio/RetornarServicios',
            traditional: true,
            success: function (data, state) {
                agregarATablaServicio(data);
            },
            error: function (data) {
                console.log(data);
            },

        });

    }




}


function BuscarProducto() {
    var codigo = document.getElementById('buscarcotitxt').value;



    //console.log(codcotizacion);
    //console.log(codigo);

    var datos = { "codigo": codigo }

    $.ajax({
        method: "POST",
        data: datos,
        dataType: 'json',
        url: '/Productos/RetornarProductoCotizacion',
        traditional: true,
        success: function (data, state) {
            agregarATablaProducto(data);
            console.log(data);

        },
        error: function (data) {
            console.log(data);
        },

    });

}

$('input[type="radio"]').click(function () {
    if ($(this).is(':checked')) {
        console.log($(this).val());
        //   var $element = $('#buscarcotitxt');  
        //   $element.attr("class","form-control select2bs4");


        if ($(this).val() === "productos") {

            var datos = { "codigo": "" }

            $.ajax({
                method: "POST",
                data: datos,
                dataType: 'json',
                url: '/Productos/RetornarProductoLista',
                traditional: true,
                success: function (data, state) {
                    console.log(data);
                    $('#buscarcotitxt').empty();
                    data.forEach(el => {
                        var html = '<option value="' + el.codigoReferencia + '">' + el.productoYCodigoDeBarras + '</option>'
                        $('#buscarcotitxt').append(html);


                        // <select class="form-control select2bs4" style="width: 80%;" id="buscarcotitxt">
                        // @foreach (var item in Model.ListaProductos)
                        // {
                        //     <option value="@item.CodigoReferencia">@item.ProductoYCodigoDeBarras</option>    
                        // }

                        // </select>



                    });

                },
                error: function (data) {
                    console.log(data);
                },

            });


        }
        else {
            var datos = { "nombre": "" }

            $.ajax({
                method: "POST",
                data: datos,
                dataType: 'json',
                url: '/VentaServicio/BusquedaServicios',
                traditional: true,
                success: function (data, state) {
                    console.log(data);

                    $('#buscarcotitxt').empty();
                    data.forEach(el => {
                        var html = '<option value="' + el.id + '">' + el.nombreServicio + '</option>'
                        $('#buscarcotitxt').append(html);
                        //console.log(data.nombreServicio);
                    });
                    //$('#buscarcotitxt').trigger('change');

                },
                error: function (data) {
                    console.log(data);
                },

            });

        }
    }
});


('#buscarcotitxt').keypress(function (e) {
    var keycode = e.which;
    if (keycode == '13') {
        var codigo = document.getElementById('buscarcotitxt').value;

        var codcotizacion = $('input[name="radiocoti"]:checked').val();;

        console.log(codcotizacion);
        console.log(codigo);

        if (codcotizacion === "productos") {

            var datos = { "codigo": codigo }

            $.ajax({
                method: "POST",
                data: datos,
                dataType: 'json',
                url: '/Productos/RetornarProductoCotizacion',
                traditional: true,
                success: function (data, state) {
                    agregarATablaProducto(data);

                },
                error: function (data) {
                    console.log(data);
                },

            });

        }
        else if (codcotizacion === "servicios") {
            var datos = { "id": codigo }
            console.log(datos);

            $.ajax({
                method: "POST",
                data: datos,
                dataType: 'json',
                url: '/Servicio/RetornarServicios',
                traditional: true,
                success: function (data, state) {
                    console.log(data);
                    agregarATablaServicio(data);
                },
                error: function (data) {
                    console.log(data);
                },

            });

        }


    }
});

function agregarATablaProducto(data) {
    //total= total + data.precio;
    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;


    var htmlTags = '<tr style="background-color:rgba(255,211,105,0.5);"> ' +
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

    $('#contentdetallecot').append(htmlTags);


    var lista = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.desc-t");


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


    document.getElementById('subtotal-venta-cot').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-cot').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-cot').innerHTML = total.toFixed(2);


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

function agregarATablaServicio(data) {
    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;


    var htmlTags = '<tr style="background-color:rgba(255,211,105,0.5);">' +
        '<td> ' + data.nombreServicio + ' </td>' +
        '<td class="cantidad-fila" ><input type="text" onchange="editarValorFila(this)" style="background-color:rgba(255,211,105,0.5);" value="1" class="form-control "></td>' +
        '<td class="precio-fila">' +
        '<select name=""  class="form-control " style="background-color:rgba(255,211,105,0.5);" >' +
        '<option value="' + data.precio.toFixed(2) + '">Precio - Q' + data.precio.toFixed(2) + '</option>' +
        '</select>' +
        '</td>' +
        '<td class="porcentaje"><input type="text" value="0" class="form-control" style="background-color:rgba(255,211,105,0.5);" onchange="editarPorcentaje(this)"></td>' +
        '<td class="desc-t">0.00 </td>' +
        '<td class="subtotal-t">' + data.precio.toFixed(2) + '</td>' +
        '<td class="total-t">' + data.precio.toFixed(2) + '</td>' +
        '<td><button  type="button"  class="btn btn-danger btn-sm" onclick="eliminarFila(this)">Quitar</button></td>' +
        '<td style="display:none;" class="nuevo-detalle"> ' + data.id + ' </td></tr>'

    $('#contentdetallecot').append(htmlTags);

    //var total= '<span>'+sumatotal.toFixed(2)+'</span>'

    var lista = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.desc-t");


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


    document.getElementById('subtotal-venta-cot').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-cot').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-cot').innerHTML = total.toFixed(2);


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

function editarValorFila(r) {

    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;

    var val = r.value;

    console.log(val);

    var sig = r.parentNode.parentNode.querySelectorAll("td.precio-fila")[0].querySelectorAll("select")[0].value;

    var suma2 = val * sig;

    var porc = r.parentNode.parentNode.querySelectorAll("td.porcentaje")[0].querySelectorAll("input")[0].value

    var desc = suma2 * (parseInt(porc) / 100);

    r.parentNode.parentNode.querySelectorAll("td.desc-t")[0].innerHTML = desc.toFixed(2);

    var totalfin = parseFloat(suma2) - parseFloat(desc);

    r.parentNode.parentNode.querySelectorAll("td.subtotal-t")[0].innerHTML = suma2.toFixed(2);
    r.parentNode.parentNode.querySelectorAll("td.total-t")[0].innerHTML = totalfin.toFixed(2);




    var lista = document.getElementById('contentdetallecot').querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetallecot').querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetallecot').querySelectorAll("td.desc-t");



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


    document.getElementById('subtotal-venta-cot').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-cot').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-cot').innerHTML = total.toFixed(2);

    //sumatotal = sumatotal - a.querySelectorAll("td.precio-fila")[0].innerHTML
    // primero aseguremos si entra el cuerpo this en el parametro va va, creo que usaremos parentnode
    //dale


}

function actualizarPrecios() {

    var lista = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.desc-t");


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


    document.getElementById('subtotal-venta-cot').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-cot').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-cot').innerHTML = total.toFixed(2);


}

function editarPorcentaje(r) {

    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;

    var val = r.value;



    var sub = r.parentNode.parentNode.querySelectorAll("td.subtotal-t")[0].innerHTML;

    var desc = parseFloat(sub) * (parseFloat(val) / 100);

    r.parentNode.parentNode.querySelectorAll("td.desc-t")[0].innerHTML = desc.toFixed(2);

    var totalfin = sub - desc;

    console.log(val);
    console.log(sub);
    console.log(desc);
    console.log(totalfin);

    r.parentNode.parentNode.querySelectorAll("td.total-t")[0].innerHTML = totalfin.toFixed(2);

    var lista = document.getElementById('contentdetallecot').querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetallecot').querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetallecot').querySelectorAll("td.desc-t");



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


    document.getElementById('subtotal-venta-cot').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-cot').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-cot').innerHTML = total.toFixed(2);

    //sumatotal = sumatotal - a.querySelectorAll("td.precio-fila")[0].innerHTML
    // primero aseguremos si entra el cuerpo this en el parametro va va, creo que usaremos parentnode
    //dale


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



    var lista = document.getElementById('contentdetallecot').querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetallecot').querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetallecot').querySelectorAll("td.desc-t");

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

    // var descuento = document.getElementById('descuentoventa').value;


    document.getElementById('subtotal-venta-cot').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-cot').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-cot').innerHTML = total.toFixed(2);

    console.log(precio);
    console.log(cantidad);
    // console.log(lista);
    // console.log("Se realizaron los cambios");

}

function eliminarFila(r) {
    total = 0.00;
    total2 = 0.00;
    total3 = 0.00;


    var i = r.parentNode.parentNode.rowIndex;
    document.getElementById("tableventa").deleteRow(i);


    var lista = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.total-t");
    var listasub = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.subtotal-t");
    var listadesc = document.getElementById('contentdetallecot').parentNode.querySelectorAll("td.desc-t");


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


    document.getElementById('subtotal-venta-cot').innerHTML = total2.toFixed(2);
    document.getElementById('descuento-venta-cot').innerHTML = total3.toFixed(2);
    document.getElementById('precio-total-a-pagar-cot').innerHTML = total.toFixed(2);
    //var total= '<span>'+sumatotal.toFixed(2)+'</span>'
}


function GuardarCotizacion() {
    //usar camelCase para variables ok
    var ids_nuevos = new Array();
    var ids_insertados = new Array();
    var detalleVenta = new Array();

    $.each($("#tableventa tbody tr"), function () {

        detalleVenta.push({
            "Producto": $(this).find("td").eq(8).html(),
            "Cantidad": this.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value,
            "Precio": this.querySelectorAll("td.precio-fila")[0].querySelectorAll("select")[0].value,
            "Descuento": $(this).find("td").eq(4).html(),
            "Subtotal": $(this).find("td").eq(5).html(),
            "Total": $(this).find("td").eq(6).html(),

        });
    });


    // console.log(ids_insertados);
    // console.log(ids_nuevos);
    // console.log(detalle);

    var datos = {
        "encabezado": {

            "NoComprobante": document.getElementById('diasvalidos').value,
            "Cliente": document.getElementById('clientecoti').value,
            "Nit": document.getElementById('nitcoti').value,
            "Direccion": document.getElementById('direccioncoti').value,
            "Empleado": document.getElementById('empleadoidcoti').value,

        },
        //listas
        // "nuevos": ids_insertados,
        "detalle": detalleVenta,


    };

    console.log(datos);



    // $.each($("#tableventa-c tbody tr"), function() {

    //     detalleCompra.push({
    //         "ProductoId": $(this).find("td").eq(6).html(),
    //         "Cantidad": $(this).find("td").eq(2).html(),
    //         "PrecioCosto": $(this).find("td").eq(3).html(),
    //         "Total": $(this).find("td").eq(4).html()
    //     });
    // });


    $.ajax({
        method: "POST",
        data: JSON.stringify(datos),
        "dataType": "json",
        "contentType": "application/json",
        url: '/Cotizacion/GuardarCotizacion',
        traditional: true,
        success: function (data) {
            // si tiene exito, reiniciemos la pag
            // pero marquemos que tuvo exito
            console.log(data);
            location.reload();
            //window.location.href = '/CrearPdf/Cotizacion/'+data;


        },
        error: function (data) {
            alert(data.error);
            console.log(data.error);
        }
    });



}

function ModificarCotizacion(id) {
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
            "Producto": $(this).find("td").eq(8).html(),
            "Cantidad": this.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value,
            "Precio": this.querySelectorAll("td.precio-fila")[0].querySelectorAll("select")[0].value,
            "Descuento": $(this).find("td").eq(4).html(),
            "Subtotal": $(this).find("td").eq(5).html(),
            "Total": $(this).find("td").eq(6).html(),
        });
    });



    console.log(ids_insertados);
    console.log(detalleVenta);




    var datos = {
        "encabezado": {
            "Id": id,
            "NoComprobante": document.getElementById('diasvalidos').value,
            "Cliente": document.getElementById('clientecoti').value,
            "Nit": document.getElementById('nitcoti').value,
            "Direccion": document.getElementById('direccioncoti').value,
            "Empleado": document.getElementById('empleadoidcoti').value,
        },
        //listas
        "nuevos": ids_insertados,
        "detalle": detalleVenta,


    };




    // $.each($("#tableventa-c tbody tr"), function() {

    //     detalleCompra.push({
    //         "ProductoId": $(this).find("td").eq(6).html(),
    //         "Cantidad": $(this).find("td").eq(2).html(),
    //         "PrecioCosto": $(this).find("td").eq(3).html(),
    //         "Total": $(this).find("td").eq(4).html()
    //     });
    // });



    $.ajax({
        method: "POST",
        data: JSON.stringify(datos),
        "dataType": "json",
        "contentType": "application/json",
        url: '/Cotizacion/ModificarCotizacion',
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

function ConfirmarCotizacion(id) {
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
            "Producto": $(this).find("td").eq(8).html(),
            "Cantidad": this.querySelectorAll("td.cantidad-fila")[0].querySelectorAll("input")[0].value,
            "Precio": this.querySelectorAll("td.precio-fila")[0].querySelectorAll("select")[0].value,
            "Descuento": $(this).find("td").eq(4).html(),
            "Subtotal": $(this).find("td").eq(5).html(),
            "Total": $(this).find("td").eq(6).html(),
        });
    });



    //console.log(ids_insertados);
    //console.log(detalleVenta);




    var datos = {
        "encabezado": {
            "Id": id,
            "NoComprobante": document.getElementById('diasvalidos').value,
            "Cliente": document.getElementById('clientecoti').value,
            "Nit": document.getElementById('nitcoti').value,
            "Direccion": document.getElementById('direccioncoti').value,
            "Empleado": document.getElementById('empleadoidcoti').value,
        },
        //listas
        "nuevos": ids_insertados,
        "detalle": detalleVenta,


    };

    console.log(datos);


    // $.each($("#tableventa-c tbody tr"), function() {

    //     detalleCompra.push({
    //         "ProductoId": $(this).find("td").eq(6).html(),
    //         "Cantidad": $(this).find("td").eq(2).html(),
    //         "PrecioCosto": $(this).find("td").eq(3).html(),
    //         "Total": $(this).find("td").eq(4).html()
    //     });
    // });



    $.ajax({
        method: "POST",
        data: JSON.stringify(datos),
        "dataType": "json",
        "contentType": "application/json",
        url: '/Cotizacion/ConfirmarCotizacion',
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