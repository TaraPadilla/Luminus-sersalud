$(function () {
    var infoedit = document.getElementsByClassName('infoedit');
    [].forEach.call(infoedit, function (el) {
        el.style.display = 'none';
    });
    var infoeditexam = document.getElementsByClassName('infoeditexam');
    [].forEach.call(infoeditexam, function (el) {
        el.style.display = 'none';
    });
    var infoeditginefile = document.getElementsByClassName('infoeditginefile');
    [].forEach.call(infoeditginefile, function (el) {
        el.style.display = 'none';
    });
    var infoeditneufile = document.getElementsByClassName('infoeditneufile');
    [].forEach.call(infoeditneufile, function (el) {
        el.style.display = 'none';
    });
    //@ts-ignore
    $("#productname").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Consultas/Search',
                data: { "term": request.term },
                dataType: "json",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }));
                }
            });
        },
        minLength: 3
    });

    //lOS CONTROLES TIPO INPUT SE CONVIERTEN EN TEXTO
    //ESTO PERMITE UTILIZAR MENOS HTML Y QUE SEA MAS ORDENADA
    //LA VISTA DE INFORMACION CONSULTA Y EDITAR CONSULTA
    let controlesDatos = document.getElementsByClassName("dato-consulta");
    $(controlesDatos).each(function (idx, control) {
        var input = control;
        var span = document.createElement('p');
        span.textContent = input.value;
        input.parentNode.replaceChild(span, input);
    });

});
var edit = function () {
    var infoedit = document.getElementsByClassName('infoconsulta');
    [].forEach.call(infoedit, function (el) {
        el.style.display = 'none';
    });
    var edit = document.getElementsByClassName('infoedit');
    [].forEach.call(edit, function (el) {
        el.style.display = 'inline';
    });
};
var editexam = function () {
    var infoconsultaexafisico = document.getElementsByClassName('infoconsultaexafisico');
    [].forEach.call(infoconsultaexafisico, function (el) {
        el.style.display = 'none';
    });
    var infoeditexam = document.getElementsByClassName('infoeditexam');
    [].forEach.call(infoeditexam, function (el) {
        el.style.display = 'inline';
    });
};
var editginefile = function () {
    var infoeditginefile = document.getElementsByClassName('infoeditginefile');
    [].forEach.call(infoeditginefile, function (el) {
        el.style.display = 'inline';
    });
    var infoginefile = document.getElementsByClassName('infoginefile');
    [].forEach.call(infoginefile, function (el) {
        el.style.display = 'none';
    });
};
var editneufile = function () {
    var infoeditneufile = document.getElementsByClassName('infoeditneufile');
    [].forEach.call(infoeditneufile, function (el) {
        el.style.display = 'inline';
    });
    var infoneufile = document.getElementsByClassName('infoneufile');
    [].forEach.call(infoneufile, function (el) {
        el.style.display = 'none';
    });
};
var canceledit = function () {
    var infoedit = document.getElementsByClassName('infoconsulta');
    [].forEach.call(infoedit, function (el) {
        el.style.display = 'inline';
    });
    var edit = document.getElementsByClassName('infoedit');
    [].forEach.call(edit, function (el) {
        el.style.display = 'none';
    });
};
var canceleditexam = function () {
    var infoconsultaexafisico = document.getElementsByClassName('infoconsultaexafisico');
    [].forEach.call(infoconsultaexafisico, function (el) {
        el.style.display = 'inline';
    });
    var infoeditexam = document.getElementsByClassName('infoeditexam');
    [].forEach.call(infoeditexam, function (el) {
        el.style.display = 'none';
    });
};
var canceleditginefile = function () {
    var infoeditginefile = document.getElementsByClassName('infoeditginefile');
    [].forEach.call(infoeditginefile, function (el) {
        el.style.display = 'none';
    });
    var infoginefile = document.getElementsByClassName('infoginefile');
    [].forEach.call(infoginefile, function (el) {
        el.style.display = 'inline';
    });
};
var canceleditneufile = function () {
    var infoeditneufile = document.getElementsByClassName('infoeditneufile');
    [].forEach.call(infoeditneufile, function (el) {
        el.style.display = 'none';
    });
    var infoneufile = document.getElementsByClassName('infoneufile');
    [].forEach.call(infoneufile, function (el) {
        el.style.display = 'inline';
    });
};
var submitForm = function (formId) {
    var form = document.getElementById(formId);
    form.submit();
};
//var addprescription = function () {
//    var productname = document.getElementById('productname');
//    var productqty = document.getElementById('productqty');
//    var productin = document.getElementById('productin');
//    var prescriptiontbody = document.getElementById('Prescriptions');
//    if (productname.value.length > 0 && productqty.value.length > 0 && productin.value.length > 0) {
//        var cont = 0;
//        var postoedit = 0;
//        //@ts-ignore
//        for (var i = 0; i < prescriptiontbody.rows.length; i++) {
//            //@ts-ignore
//            var row = prescriptiontbody.rows[i];
//            if (row.cells[1].innerHTML === productname.value) {
//                cont = cont + 1;
//                postoedit = i;
//            }
//        }
//        if (cont == 0) {
//            var id = String(prescriptiontbody.childElementCount + 1);
//            var buttons = '<button class="btn btn-warning" onclick="editprescription(' + id + ')" ><i class="fas fa-edit"></i></button>'
//                + '&nbsp &nbsp'
//                + '<button onclick="deleteprescription(' + id + ')" class="btn btn-danger"><i class="fas fa-trash"></i></button>';
//            var tr = document.createElement('tr');
//            var ctd = document.createElement('td');
//            var pntd = document.createElement('td');
//            var pitd = document.createElement('td');
//            var pqtd = document.createElement('td');
//            var pactions = document.createElement('td');
//            var b = document.createElement('b');
//            tr.setAttribute('id', id);
//            b.innerHTML = id;
//            pntd.innerHTML = productname.value;
//            pqtd.innerHTML = productqty.value;
//            pitd.innerHTML = productin.value;
//            pactions.innerHTML = buttons;
//            ctd.appendChild(b);
//            tr.appendChild(ctd);
//            tr.appendChild(pntd);
//            tr.appendChild(pqtd);
//            tr.appendChild(pitd);
//            tr.appendChild(pactions);
//            console.log(tr);
//            prescriptiontbody.appendChild(tr);
//        }
//        else {
//            //@ts-ignore
//            prescriptiontbody.rows[postoedit].cells[2].innerHTML = productqty.value;
//            //@ts-ignore
//            prescriptiontbody.rows[postoedit].cells[3].innerHTML = productin.value;
//        }
//        productname.value = '';
//        productqty.value = '';
//        productin.value = '';
//    }
//    if (document.getElementById('generateprespdf').hasAttribute('disabled')) {
//        document.getElementById('generateprespdf').removeAttribute('disabled');
//    }
//};
//var editprescription = function (editid) {
//    var productname = document.getElementById('productname');
//    var productqty = document.getElementById('productqty');
//    var productin = document.getElementById('productin');
//    var trparent = document.getElementById(editid);
//    var tr = trparent.childNodes;
//    //@ts-ignore
//    productname.value = tr[1].innerHTML;
//    //@ts-ignore
//    productqty.value = tr[2].innerHTML;
//    //@ts-ignore
//    productin.value = tr[3].innerHTML;
//};
//var deleteprescription = function (deleteid) {
//    document.getElementById(deleteid).remove();
//};
//var savePrescription = function () {
//    var clientName = document.getElementById('Consulta_Citas_Paciente_Nombre');
//    var clientAge = document.getElementById('Consulta_Citas_Paciente_Edad');
//    var clientAddress = document.getElementById('Consulta_Citas_Paciente_Direccion');
//    var ConsultaId = document.getElementById('Consulta_Id');
//    var prescriptiontbody = document.getElementById('Prescriptions');
//    var Prescriptions = new Array();
//    var model = {};
//    //@ts-ignore
//    for (var i = 0; i < prescriptiontbody.rows.length; i++) {
//        //@ts-ignore
//        var row = prescriptiontbody.rows[i];
//        Prescriptions.push({
//            Item: parseInt(row.cells[0].firstChild.innerHTML),
//            Medicine: row.cells[1].innerHTML,
//            Qty: row.cells[2].innerHTML,
//            Indications: row.cells[3].innerHTML
//        });
//    }
//    //@ts-ignore
//    model.Nombre = clientName.value;
//    //@ts-ignore
//    model.Edad = clientAge.value.trim() == '' || clientAge.value == undefined ? 0 : parseInt(clientAge.value);
//    //@ts-ignore
//    model.Direccion = clientAddress.value;
//    //@ts-ignore
//    model.Prescriptions = Prescriptions;
//    //@ts-ignore
//    model.ConsultaId = parseInt(ConsultaId.value);
//    console.log(Prescriptions);
//    console.log(JSON.stringify(model));
//    $.ajax({
//        method: "POST",
//        url: '/Consultas/GuardarPrescripcion',
//        data: JSON.stringify(model),
//        contentType: "application/json",
//        dataType: "json",
//        success: function (data) {
//            window.open('/CrearPDF/PrescripcionPDF?prescripcionId=' + data, "_blank");
//        }
//    });
//};
//# sourceMappingURL=infoconsulta.js.map

//function actualizarVista() {
//    //Inicia Cálculo de IMC
//    var peso = 0.0;
//    var estatura = 0.0;
//    try { peso = parseFloat($("#Peso").val()) } catch { peso = 0.0 }
//    try { estatura = parseFloat($("#Estatura").val()) } catch { estatura = 0.0 }
//    var imc = 0.0;
//    if (estatura > 0 && peso > 0) {
//        imc = peso / (estatura * estatura);
//        $("#IMC").val(imc.toString() + " kg/m2");
//    } else {
//        $("#IMC").val("");
//    }
//    //Finaliza cálculo de IMC
//}