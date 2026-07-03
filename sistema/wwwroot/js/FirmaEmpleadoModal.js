(function () {
    "use strict";

    var _canvas = null;
    var _ctx = null;
    var _drawing = false;
    var _onPointerDown = null;
    var _onPointerMove = null;
    var _onPointerUp = null;

    function _getPos(e, canvas) {
        var r = canvas.getBoundingClientRect();
        var scaleX = canvas.width / r.width;
        var scaleY = canvas.height / r.height;
        var clientX = e.clientX !== undefined ? e.clientX : e.touches[0].clientX;
        var clientY = e.clientY !== undefined ? e.clientY : e.touches[0].clientY;
        return { x: (clientX - r.left) * scaleX, y: (clientY - r.top) * scaleY };
    }

    function _resizeCanvas(canvas) {
        var rect = canvas.getBoundingClientRect();
        var w = Math.max(1, Math.floor(rect.width) || 600);
        var h = Math.max(1, Math.floor(rect.height) || 200);
        if (canvas.width !== w || canvas.height !== h) {
            canvas.width = w;
            canvas.height = h;
        }
    }

    window.initCanvasFirmaEmpleado = function () {
        _canvas = document.getElementById("canvasFirmaEmpleado");
        if (!_canvas) return;

        var panelCanvas = document.getElementById("panelCanvasFirmaEmpleado");
        if (panelCanvas && panelCanvas.classList.contains("d-none")) return;

        _resizeCanvas(_canvas);

        if (_onPointerDown) {
            _canvas.removeEventListener("pointerdown", _onPointerDown);
            _canvas.removeEventListener("pointermove", _onPointerMove);
            _canvas.removeEventListener("pointerup", _onPointerUp);
            _canvas.removeEventListener("pointerout", _onPointerUp);
        }

        _ctx = _canvas.getContext("2d");
        _ctx.strokeStyle = "#1a1a2e";
        _ctx.lineWidth = 2.2;
        _ctx.lineCap = "round";
        _ctx.lineJoin = "round";

        _onPointerDown = function (e) {
            e.preventDefault();
            _drawing = true;
            var pos = _getPos(e, _canvas);
            _ctx.beginPath();
            _ctx.moveTo(pos.x, pos.y);
            if (_canvas.setPointerCapture) _canvas.setPointerCapture(e.pointerId);
        };
        _onPointerMove = function (e) {
            if (!_drawing) return;
            e.preventDefault();
            var pos = _getPos(e, _canvas);
            _ctx.lineTo(pos.x, pos.y);
            _ctx.stroke();
        };
        _onPointerUp = function () {
            if (_drawing) {
                _ctx.closePath();
                _drawing = false;
            }
        };

        _canvas.addEventListener("pointerdown", _onPointerDown);
        _canvas.addEventListener("pointermove", _onPointerMove);
        _canvas.addEventListener("pointerup", _onPointerUp);
        _canvas.addEventListener("pointerout", _onPointerUp);
    };

    window.limpiarCanvasFirmaEmpleado = function () {
        if (_canvas && _ctx) {
            _ctx.clearRect(0, 0, _canvas.width, _canvas.height);
            _ctx.beginPath();
            _drawing = false;
        }
    };

    window.usarCanvasFirmaEmpleado = function () {
        if (!_canvas || !_ctx) {
            window.initCanvasFirmaEmpleado();
            _canvas = document.getElementById("canvasFirmaEmpleado");
            if (!_canvas || !_ctx) return;
        }

        var px = _ctx.getImageData(0, 0, _canvas.width, _canvas.height).data;
        var hasContent = false;
        for (var i = 3; i < px.length; i += 4) {
            if (px[i] > 0) {
                hasContent = true;
                break;
            }
        }
        if (!hasContent) {
            alert("El area de firma esta vacia. Por favor dibuje su firma.");
            return;
        }

        var dataUrl = _canvas.toDataURL("image/png");
        if (window.empleadoVm) window.empleadoVm.setFirmaTemporal(dataUrl, null);
    };

    window.previsualizarArchivoFirmaEmpleado = function (input) {
        var archivo = input.files[0];
        if (!archivo) return;
        var label = input.nextElementSibling;
        if (label) label.innerText = archivo.name;
        var reader = new FileReader();
        reader.onload = function (e) {
            if (window.empleadoVm) window.empleadoVm.setFirmaTemporal(e.target.result, archivo);
        };
        reader.readAsDataURL(archivo);
    };

    window._switchFirmaTabEmpleado = function (modo) {
        var panelCanvas = document.getElementById("panelCanvasFirmaEmpleado");
        var panelArchivo = document.getElementById("panelArchivoFirmaEmpleado");
        var tabCanvas = document.getElementById("tabCanvasFirmaEmpleado");
        var tabArchivo = document.getElementById("tabArchivoFirmaEmpleado");
        if (!panelCanvas || !panelArchivo) return;

        if (modo === "canvas") {
            panelCanvas.classList.remove("d-none");
            panelArchivo.classList.add("d-none");
            if (tabCanvas) tabCanvas.classList.add("active");
            if (tabArchivo) tabArchivo.classList.remove("active");
            requestAnimationFrame(function () {
                window.initCanvasFirmaEmpleado();
            });
        } else {
            panelCanvas.classList.add("d-none");
            panelArchivo.classList.remove("d-none");
            if (tabCanvas) tabCanvas.classList.remove("active");
            if (tabArchivo) tabArchivo.classList.add("active");
        }
    };

    window.resetearFirmaEmpleadoUI = function () {
        var selector = document.getElementById("selectorFirmaEmpleado");
        var contenedorPrev = document.getElementById("contenedorPrevFirmaEmpleado");
        var btnConfirmar = document.getElementById("btnConfirmarFirmaEmpleado");
        var imgPrev = document.getElementById("imgPrevFirmaEmpleado");

        if (selector) selector.classList.remove("d-none");
        if (contenedorPrev) contenedorPrev.classList.add("d-none");
        if (btnConfirmar) btnConfirmar.disabled = true;
        if (imgPrev) imgPrev.src = "";

        window.limpiarCanvasFirmaEmpleado();

        var input = document.getElementById("inputArchivoFirma");
        if (input) input.value = "";
        var label = document.querySelector('label[for="inputArchivoFirma"]');
        if (label) label.innerText = "Elegir archivo...";

        if (window.empleadoVm) {
            window.empleadoVm.FirmaTemporalBase64(null);
            window.empleadoVm.FirmaTemporalFile(null);
        }

        requestAnimationFrame(function () {
            window._switchFirmaTabEmpleado("canvas");
        });
    };

    window.abrirModalFirmaEmpleado = function () {
        var overlay = document.getElementById("mdl-firma-empleado");
        if (!overlay) return;
        overlay.style.display = "flex";

        var contenedorPrev = document.getElementById("contenedorPrevFirmaEmpleado");
        var previewVisible = contenedorPrev && !contenedorPrev.classList.contains("d-none");

        requestAnimationFrame(function () {
            if (!previewVisible) {
                window._switchFirmaTabEmpleado("canvas");
            }
        });
    };

    window.cerrarModalFirmaEmpleado = function () {
        var overlay = document.getElementById("mdl-firma-empleado");
        if (overlay) overlay.style.display = "none";
    };
})();
