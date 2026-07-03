let isDrawingDevol = false;
let ctxDevol = null;
let firmaBase64Devol = null;   

window.initCanvasDevol = function () {
    const canvas = document.getElementById('canvasFirmaDevol');
    if (!canvas) return;

    ctxDevol = canvas.getContext('2d');
    ctxDevol.strokeStyle = '#000000';
    ctxDevol.lineWidth   = 2;
    ctxDevol.lineCap     = 'round';
    ctxDevol.lineJoin    = 'round';

    canvas.onmousedown = (e) => {
        isDrawingDevol = true;
        ctxDevol.beginPath();
        const r = canvas.getBoundingClientRect();
        ctxDevol.moveTo(e.clientX - r.left, e.clientY - r.top);
    };

    canvas.onmousemove = (e) => {
        if (!isDrawingDevol) return;
        const r = canvas.getBoundingClientRect();
        ctxDevol.lineTo(e.clientX - r.left, e.clientY - r.top);
        ctxDevol.stroke();
    };

    window.onmouseup = () => { if (isDrawingDevol) { ctxDevol.closePath(); isDrawingDevol = false; } };

    // Touch
    canvas.addEventListener('touchstart', (e) => {
        e.preventDefault();
        canvas.dispatchEvent(new MouseEvent('mousedown', { clientX: e.touches[0].clientX, clientY: e.touches[0].clientY }));
    }, false);

    canvas.addEventListener('touchmove', (e) => {
        e.preventDefault();
        canvas.dispatchEvent(new MouseEvent('mousemove', { clientX: e.touches[0].clientX, clientY: e.touches[0].clientY }));
    }, false);

    canvas.addEventListener('touchend', () => { isDrawingDevol = false; }, false);
};

window.limpiarCanvasDevol = function () {
    const canvas = document.getElementById('canvasFirmaDevol');
    if (canvas && ctxDevol) {
        ctxDevol.clearRect(0, 0, canvas.width, canvas.height);
        ctxDevol.beginPath();
    }
};

window.usarFirmaCanvasDevol = function () {
    const canvas = document.getElementById('canvasFirmaDevol');
    if (!canvas) return;
    const dataUrl = canvas.toDataURL('image/png');
    mostrarPrevisualizacionDevol(dataUrl);
};

window.previsualizarArchivoDevol = function (input) {
    const archivo = input.files[0];
    if (!archivo) return;
    const reader = new FileReader();
    reader.onload = (e) => mostrarPrevisualizacionDevol(e.target.result);
    reader.readAsDataURL(archivo);
};

function mostrarPrevisualizacionDevol(base64OrUrl) {
    firmaBase64Devol = base64OrUrl;

    const srcImagen = (base64OrUrl.startsWith('/') || base64OrUrl.startsWith('http'))
        ? base64OrUrl
        : base64OrUrl;

    document.getElementById('imgFirmaPrevDevol').src = srcImagen;
    document.getElementById('contenedorPrevisualizacionDevol').classList.remove('d-none');
    document.getElementById('selectorFirmaDevol').classList.add('d-none');
    document.getElementById('btnFinalizarAutorizacionDevol').removeAttribute('disabled');
}

window.resetearFirmaDevol = function () {
    firmaBase64Devol = null;
    document.getElementById('imgFirmaPrevDevol').src = '';
    document.getElementById('contenedorPrevisualizacionDevol').classList.add('d-none');
    document.getElementById('selectorFirmaDevol').classList.remove('d-none');
    document.getElementById('btnFinalizarAutorizacionDevol').setAttribute('disabled', 'disabled');
};

window.AutorizarDevolucion = function () {
    if (!firmaBase64Devol) {
        alert('Debe seleccionar o dibujar una firma antes de continuar.');
        return;
    }

    const id = document.getElementById('idDevolucionAutorizar')?.value;

    if (!id) {
        alert('Faltan datos de la devolución. Recargue la página e intente de nuevo.');
        return;
    }

    if (!confirm('¿Confirma la autorización de esta devolución?')) return;

    $.ajax({
        url: '/Devolucion/ProcesarAutorizacionFirma',
        method: 'POST',
        data: {
            id:          id,
            firmaBase64: firmaBase64Devol,
            tipoFirma:   'autorizacion'
        },
        success: function (res) {
            if (res.success) {
                $('#modalAutorizacionDevolucion').modal('hide');
                toastr.success(res.message || 'Autorización guardada correctamente.');
                if (window.tablaDevolucion) window.tablaDevolucion.ajax.reload(null, false);
            } else {
                toastr.error(res.message || 'Error al procesar la autorización.');
            }
        },
        error: function () {
            toastr.error('Error de conexión al procesar la autorización.');
        }
    });
};