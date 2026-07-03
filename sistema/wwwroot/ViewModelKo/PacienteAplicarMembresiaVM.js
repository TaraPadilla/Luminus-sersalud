var PacienteAplicarMembresiaVM = function () {

    var model = {};
    var self = this;

    self.getModel = function () {
        model = {
            "PacienteId": $("#PacienteId").val()
        };
    };

    self.guardarMembresia = function () {
        if (confirm("¿Desea registrar la membresia?")) {
            $("#div-loading").show();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/Pacientes/AplicarMembresia',
                data: model,
                success: function (data, textStatus) {
                    if (data.exitoso) {
                        window.open("/CrearPDF/PacientesCompromisoMembresiaPDF?pacienteId=" + model.PacienteId, "Membresía PlusVida", "width=1000, height=500")
                        window.location.href = "/Pacientes/Lista";
                    }
                    else {
                        $("#div-loading").hide();
                        alert(data.mensaje);
                    }
                },
                error: function (data) {
                    $("#div-loading").hide();
                    alert(data.error);
                }
            });
        }
    };
}

var aplicarMembresiaVM = new PacienteAplicarMembresiaVM();
ko.applyBindings(aplicarMembresiaVM);

$(document).ready(function () {
    //======================================================================
    // VARIABLES
    //======================================================================
    let miCanvas = document.querySelector('#firma');
    let lineas = [];
    let correccionX = 0;
    let correccionY = 0;
    let pintarLinea = false;
    // Marca el nuevo punto
    let nuevaPosicionX = 0;
    let nuevaPosicionY = 0;

    let posicion = miCanvas.getBoundingClientRect()
    correccionX = posicion.x;
    correccionY = posicion.y;

    miCanvas.width = 900;
    miCanvas.height = 300;

    //======================================================================
    // FUNCIONES
    //======================================================================

    /**
     * Funcion que empieza a dibujar la linea
     */
    function empezarDibujo() {
        pintarLinea = true;
        lineas.push([]);
    };

    /**
     * Funcion que guarda la posicion de la nueva línea
     */
    function guardarLinea() {
        lineas[lineas.length - 1].push({
            x: nuevaPosicionX,
            y: nuevaPosicionY
        });
    }

    /**
     * Funcion dibuja la linea
     */
    function dibujarLinea(event) {
        event.preventDefault();
        if (pintarLinea) {
            let ctx = miCanvas.getContext('2d')
            // Estilos de linea
            ctx.lineJoin = ctx.lineCap = 'round';
            ctx.lineWidth = 3;
            // Color de la linea
            ctx.strokeStyle = '#000000';
            // Marca el nuevo punto
            if (event.changedTouches == undefined) {
                // Versión ratón
                nuevaPosicionX = event.layerX;
                nuevaPosicionY = event.layerY;
            } else {
                // Versión touch, pantalla tactil
                nuevaPosicionX = event.changedTouches[0].pageX - correccionX;
                nuevaPosicionY = event.changedTouches[0].pageY - correccionY;
            }
            // Guarda la linea
            guardarLinea();
            // Redibuja todas las lineas guardadas
            ctx.beginPath();
            lineas.forEach(function (segmento) {
                ctx.moveTo(segmento[0].x, segmento[0].y);
                segmento.forEach(function (punto, index) {
                    ctx.lineTo(punto.x, punto.y);
                });
            });
            ctx.stroke();
        }
    }

    /**
     * Funcion que deja de dibujar la linea
     */
    function pararDibujar() {
        pintarLinea = false;
        guardarLinea();
    }

    //======================================================================
    // EVENTOS
    //======================================================================

    // Eventos raton
    miCanvas.addEventListener('mousedown', empezarDibujo, false);
    miCanvas.addEventListener('mousemove', dibujarLinea, false);
    miCanvas.addEventListener('mouseup', pararDibujar, false);

    // Eventos pantallas táctiles
    miCanvas.addEventListener('touchstart', empezarDibujo, false);
    miCanvas.addEventListener('touchmove', dibujarLinea, false);
});