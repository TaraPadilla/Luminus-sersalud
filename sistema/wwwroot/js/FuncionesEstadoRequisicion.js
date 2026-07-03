function previsualizarArchivo(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            mostrarPrevisualizacion(e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function usarFirmaCanvas() {
    const canvas = document.getElementById('canvasFirma');
    const blank = document.createElement('canvas');
    blank.width = canvas.width;
    blank.height = canvas.height;

    if (canvas.toDataURL() === blank.toDataURL()) {
        toastr.warning("Por favor, realice su firma antes de confirmar.");
        return;
    }

    const dataUrl = canvas.toDataURL("image/png");
    mostrarPrevisualizacion(dataUrl);
}

function mostrarPrevisualizacion(src) {
    document.getElementById('imgFirmaPrev').src = src;
    document.getElementById('contenedorPrevisualizacion').classList.remove('d-none');
    document.getElementById('selectorFirma').classList.add('d-none');

    const btnFinal = document.getElementById('btnFinalizarAutorizacion');
    if (btnFinal) btnFinal.disabled = false;
}

function resetearFirma() {
    document.getElementById('imgFirmaPrev').src = "";
    document.getElementById('contenedorPrevisualizacion').classList.add('d-none');
    document.getElementById('selectorFirma').classList.remove('d-none');

    document.getElementById('inputFirma').value = "";
    limpiarCanvas('canvasFirma');

    const btnFinal = document.getElementById('btnFinalizarAutorizacion');
    if (btnFinal) btnFinal.disabled = true;
}

let isDrawing = false;
let ctx = null;

function initCanvas() {
    const canvas = document.getElementById('canvasFirma');
    if (!canvas) return;

    ctx = canvas.getContext('2d');
    ctx.strokeStyle = "#000000";
    ctx.lineWidth = 2.5;
    ctx.lineCap = "round";
    ctx.lineJoin = "round";

    const getPos = (e) => {
        const rect = canvas.getBoundingClientRect();
        const clientX = e.touches ? e.touches[0].clientX : e.clientX;
        const clientY = e.touches ? e.touches[0].clientY : e.clientY;

        return {
            x: (clientX - rect.left) * (canvas.width / rect.width),
            y: (clientY - rect.top) * (canvas.height / rect.height)
        };
    };

    const start = (e) => {
        if (e.type === 'touchstart') e.preventDefault();
        isDrawing = true;
        const pos = getPos(e);
        ctx.beginPath();
        ctx.moveTo(pos.x, pos.y);
    };

    const move = (e) => {
        if (!isDrawing) return;
        if (e.type === 'touchmove') e.preventDefault();
        const pos = getPos(e);
        ctx.lineTo(pos.x, pos.y);
        ctx.stroke();
    };

    const stop = () => {
        if (isDrawing) {
            ctx.closePath();
            isDrawing = false;
        }
    };

    canvas.onmousedown = start;
    canvas.onmousemove = move;
    canvas.onmouseup = stop;
    canvas.onmouseleave = stop;

    canvas.addEventListener("touchstart", start, { passive: false });
    canvas.addEventListener("touchmove", move, { passive: false });
    canvas.addEventListener("touchend", stop, { passive: false });
}

window.limpiarCanvas = function (idCanvas) {
    const canvas = document.getElementById(idCanvas);
    if (canvas && ctx) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        isDrawing = false;
    }
};

function Autorizar() {
    const imgFirma = document.getElementById('imgFirmaPrev').src;
    const requisicionId = $('#idRequisicionAutorizar').val();
    const tipoFirma = $('#tipoFirmaAutorizar').val();
    const nombreGerente = $('#nombreGerente').val();
    const nombreJefatura = $('#nombreJefatura').val();
    const nombreAlmacen = $('#nombreAlmacen').val();
    const nombreEntregado  = $('#nombreEntregado').val();

    if (!imgFirma || imgFirma.startsWith('window.location') || imgFirma === "") {
        toastr.warning("Por favor, capture o suba una firma.");
        return;
    }

    $.ajax({
        url: "/Requisicion/ProcesarAutorizacionFirma",
        method: "POST",
        data: {
            id: requisicionId,
            firmaBase64: imgFirma,
            tipoFirma: tipoFirma,
            nombreGerencia: nombreGerente,
            nombreJefatura: nombreJefatura,
            nombreAlmacen: nombreAlmacen,
            nombreEntregado: nombreEntregado


        },
        success: function (res) {
            if (res.success) {
                toastr.success(res.message);
                $('#modalAutorizacion').modal('hide');

                if ($.fn.DataTable.isDataTable('#tabla-requisiciones')) {
                    $('#tabla-requisiciones').DataTable().ajax.reload();
                }
            } else {
                toastr.error(res.message);
            }
        },
        error: function () {
            toastr.error("Error crítico al conectar con el servidor.");
        }
    });
}



document.addEventListener("DOMContentLoaded", function () {

    // 1. Cálculo en tiempo real mientras el usuario escribe
    document.addEventListener("input", function (e) {
        if (e.target.classList.contains("js-input-despacho")) {
            const fila = e.target.closest("tr");

            // Obtener valores (usamos parseFloat para permitir decimales)
            const solicitado = parseFloat(fila.querySelector(".js-solicitado").innerText) || 0;
            const despacho = parseFloat(e.target.value) || 0;

            // Calcular diferencia
            const diferencia = solicitado - despacho;

            // Actualizar el badge de diferencia
            const badgeDiferencia = fila.querySelector(".js-diferencia");
            badgeDiferencia.innerText = diferencia.toFixed(2).replace('.00', '');

            // Cambio de color visual si el despacho supera lo solicitado
            if (diferencia < 0) {
                badgeDiferencia.classList.replace("badge-light", "badge-danger");
            } else {
                badgeDiferencia.classList.replace("badge-danger", "badge-light");
            }
        }
    });
});


function confirmarDespachoGeneral() {
    const inputs = document.querySelectorAll(".js-input-despacho");
    const payload = [];

    // Recolectar datos de los inputs
    inputs.forEach(input => {
        payload.push({
            Id: parseInt(input.getAttribute("data-id")),
            CantidadDespachada: parseFloat(input.value) || 0,
            RequisionId: parseInt(input.getAttribute("data-req")),

        });
    });

    // Validación con Toastr si la lista está vacía
    if (payload.length === 0) {
        toastr.info("No se encontraron productos para actualizar.");
        return;
    }

    // Mantenemos la confirmación nativa como solicitaste
    if (!confirm(`¿Desea guardar los cambios para estos ${payload.length} productos?`)) {
        return;
    }

    fetch('/Requisicion/GuardarDespachoMasivo', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
        .then(response => response.json())
        .then(data => {
            if (data.exitoso) {
                toastr.success(data.message || "Cambios guardados correctamente.");


                $('#modalDetalleRequisicion').modal('hide');

                if ($.fn.DataTable.isDataTable('#tabla-requisiciones')) {
                    $('#tabla-requisiciones').DataTable().ajax.reload();
                }
                // Retrasamos la recarga un poco para que el usuario vea el mensaje

            } else {
                // Reemplazo de alert por toastr.error
                toastr.error("Error: " + data.message);
            }
        })
        .catch(error => {
            console.error("Error:", error);
            // Reemplazo de alert por toastr.error
            toastr.error("Ocurrió un error crítico al conectar con el servidor.");
        });
}


$(document).on('input', '.js-input-despacho', function () {
    const input = $(this);
    
    const valorDespacho = Number(input.val()) || 0;

    const textoSolicitado = input.closest('tr').find('.js-solicitado').text();
    const valorSolicitado = Number(textoSolicitado) || 0;
    
    const mensajeError = input.siblings('.invalid-feedback');
    const btnGuardar = $('.btn-success');

    if (valorDespacho > valorSolicitado) {
        input.addClass('is-invalid');
        mensajeError.removeClass('d-none');
        mensajeError.text("Máximo permitido: " + valorSolicitado);
    } else {
        input.removeClass('is-invalid');
        mensajeError.addClass('d-none');
    }

    const hayErrores = $('.js-input-despacho.is-invalid').length > 0;
    btnGuardar.prop('disabled', hayErrores);
});