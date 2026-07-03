var PacienteInformacionVM = function () {
    var self = this;
    self.nombreNuevoArchivo = ko.observable();

    //Subir fotografía
    self.actualizarFotografiaPaciente = function (rutaFotografia) {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Pacientes/ActualizarFotografiaPaciente',
            data: {
                pacienteId: $("#Id").val(),
                rutaFotografia: rutaFotografia
            },
            success: function (data) {
                if (data.exitoso) {
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.subirFotografiaPaciente = function () {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Files/SubirArchivo',
            data: {
                base64Archivo: $("#base-64-fotografia").val(),
                extension: ".jpg"
            },
            success: function (data) {
                hideLoading();
                if (data.exitoso)
                    self.actualizarFotografiaPaciente(data.url)
                else {
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };


    //Subir archivo
    self.registrarArchivoPaciente = function (rutaArchivo) {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Pacientes/RegistrarArchivo',
            data: {
                pacienteId: $("#Id").val(),
                rutaArchivo: rutaArchivo,
                nombreArchivo: self.nombreNuevoArchivo()
            },
            success: function (data) {
                if (data.exitoso) {
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.subirArchivoPaciente = function () {
       
        if (self.nombreNuevoArchivo() == undefined || self.nombreNuevoArchivo() == null) {
            alert("Asegúrese de proporcionar el nombre del archivo");
        } else {
            showLoading();
            $.ajax({
                method: "POST",
                url: '/Files/SubirArchivo',
                data: {
                    base64Archivo: $("#base-64-nuevo-archivo").val(),
                    extension: "." + document.querySelector('#nuevo-archivo').files[0].name.split('.')[1]
                },
                success: function (data) {
                    

                    hideLoading();
                    if (data.exitoso) {
                        $("#base-64-nuevo-archivo").val("");
                        self.registrarArchivoPaciente(data.url);
                    }
                    else {
                        alert(data.mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }
    };


    //Actualización de firmas
    self.actualizarFirmaRegistro = function (rutaFirma) {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Pacientes/ActualizarFirmaRegistro',
            data: {
                pacienteId: $("#Id").val(),
                rutaFirma: rutaFirma
            },
            success: function (data) {
                if (data.exitoso) {
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.subirFirmaRegistro = function () {

        //Carga de archivo
        var canvas = document.getElementById('firma-registro');
        var dataBase64 = canvas.toDataURL();

        //Subir archivo
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Files/SubirArchivo',
            data: {
                base64Archivo: dataBase64,
                extension: ".jpg"
            },
            success: function (data) {
                hideLoading();
                if (data.exitoso)
                    self.actualizarFirmaRegistro(data.url);
                else {
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.actualizarFirmaPoliticasPago = function (rutaFirma) {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Pacientes/ActualizarFirmaPoliticasPago',
            data: {
                pacienteId: $("#Id").val(),
                rutaFirma: rutaFirma
            },
            success: function (data) {
                if (data.exitoso) {
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.subirFirmaPoliticasPago = function () {

        //Carga de archivo
        var canvas = document.getElementById('firma-politicas-pago');
        var dataBase64 = canvas.toDataURL();

        //Subir archivo
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Files/SubirArchivo',
            data: {
                base64Archivo: dataBase64,
                extension: ".jpg"
            },
            success: function (data) {
                hideLoading();
                if (data.exitoso)
                    self.actualizarFirmaPoliticasPago(data.url);
                else {
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.actualizarFirmaDeclaracionConsentimiento = function (rutaFirma) {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Pacientes/ActualizarFirmaDeclaracionConsentimiento',
            data: {
                pacienteId: $("#Id").val(),
                rutaFirma: rutaFirma
            },
            success: function (data) {
                if (data.exitoso) {
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.subirFirmaDeclaracionConsentimiento = function () {

        //Carga de archivo
        var canvas = document.getElementById('firma-declaracion-consentimiento');
        var dataBase64 = canvas.toDataURL();

        //Subir archivo
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Files/SubirArchivo',
            data: {
                base64Archivo: dataBase64,
                extension: ".jpg"
            },
            success: function (data) {
                hideLoading();
                if (data.exitoso)
                    self.actualizarFirmaDeclaracionConsentimiento(data.url);
                else {
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.actualizarFirmaTarjetaCredito = function (rutaFirma) {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Pacientes/ActualizarFirmaTarjetaCredito',
            data: {
                pacienteId: $("#Id").val(),
                rutaFirma: rutaFirma
            },
            success: function (data) {
                if (data.exitoso) {
                    window.location.reload();
                } else {
                    hideLoading();
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };
    self.subirFirmaTarjetaCredito = function () {

        //Carga de archivo
        var canvas = document.getElementById('firma-tarjeta-credito');
        var dataBase64 = canvas.toDataURL();

        //Subir archivo
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Files/SubirArchivo',
            data: {
                base64Archivo: dataBase64,
                extension: ".jpg"
            },
            success: function (data) {
                hideLoading();
                if (data.exitoso)
                    self.actualizarFirmaTarjetaCredito(data.url);
                else {
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                hideLoading();
                alert(data.error);
            }
        });
    };





    self.abrirModalAgregarFirmaRegistro = function () {
        $("#mdl-firma-registro").dialog({
            resizable: false,
            minWidth: 1000,
            modal: true
        });
    };
    self.abrirModalAgregarFirmaPoliticasPago = function () {
        $("#mdl-firma-politicas-pago").dialog({
            resizable: false,
            minWidth: 1000,
            modal: true
        });
    };
    self.abrirModalAgregarFirmaDeclaracionConsentimiento = function () {
        $("#mdl-firma-declaracion-consentimiento").dialog({
            resizable: false,
            minWidth: 1000,
            modal: true
        });
    };
    self.abrirModalAgregarFirmaTarjetaCredito = function () {
        $("#mdl-firma-datos-pago").dialog({
            resizable: false,
            minWidth: 1000,
            modal: true
        });
    };
}

var informacionPacienteVm = new PacienteInformacionVM();
ko.applyBindings(informacionPacienteVm);

$(document).ready(function () {
    renderFirma("firma-registro");
    renderFirma("firma-politicas-pago");
    renderFirma("firma-declaracion-consentimiento");
    renderFirma("firma-tarjeta-credito");

    //Carga de fotografía de paciente
    const inputFile = document.querySelector('#fotografia-paciente');
    const dataBase64 = document.querySelector('#base-64-fotografia');
    async function encodeFileAsBase64URL(file) {
        return new Promise((resolve) => {
            const reader = new FileReader();
            reader.addEventListener('loadend', () => {
                resolve(reader.result);
            });
            reader.readAsDataURL(file);
        });
    };
    if (inputFile != null)
        inputFile.addEventListener('input', async (event) => {
            if (inputFile.files[0] != undefined && inputFile.files[0] != null) {
                let base64URL = await encodeFileAsBase64URL(inputFile.files[0]);
                dataBase64.setAttribute('value', base64URL);
            } else {
                dataBase64.removeAttribute('value');
            }
        });

    //Carga de archivo
    let inputFileNuevoArchivo = document.querySelector('#nuevo-archivo');
    const dataBase64NuevoArchvio = document.querySelector('#base-64-nuevo-archivo');
    async function encodeFileAsBase64URL(file) {
        return new Promise((resolve) => {
            const reader = new FileReader();
            reader.addEventListener('loadend', () => {
                resolve(reader.result);
            });
            reader.readAsDataURL(file);
        });
    };
    if (inputFileNuevoArchivo != null)
        inputFileNuevoArchivo.addEventListener('input', async (event) => {
            if (inputFileNuevoArchivo.files[0] != undefined && inputFileNuevoArchivo.files[0] != null) {
                let base64URL = await encodeFileAsBase64URL(inputFileNuevoArchivo.files[0]);
                dataBase64NuevoArchvio.setAttribute('value', base64URL);
            } else {
                dataBase64NuevoArchvio.removeAttribute('value');
            }
        });
});





//Firmas
function renderFirma(idCanvas) {
    let miCanvas = document.querySelector('#' + idCanvas);
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

    // Eventos raton
    //Empezar dibujo
    miCanvas.addEventListener('mousedown', function () {
        pintarLinea = true;
        lineas.push([]);
    }, false);
    //Dibujar linea
    miCanvas.addEventListener('mousemove', function (event) {
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
            //Guardar linea
            lineas[lineas.length - 1].push({
                x: nuevaPosicionX,
                y: nuevaPosicionY
            });
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
    }, false);
    miCanvas.addEventListener('mouseup', function () {
        pintarLinea = false;
        //Guardar linea
        lineas[lineas.length - 1].push({
            x: nuevaPosicionX,
            y: nuevaPosicionY
        });
    }, false);

    // Eventos pantallas táctiles
    //Empezar dibujo
    miCanvas.addEventListener('touchstart', function () {
        pintarLinea = true;
        lineas.push([]);
    }, false);
    //Dibujar linea
    miCanvas.addEventListener('touchmove', function (event) {
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
            lineas[lineas.length - 1].push({
                x: nuevaPosicionX,
                y: nuevaPosicionY
            });
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
    }, false);
}