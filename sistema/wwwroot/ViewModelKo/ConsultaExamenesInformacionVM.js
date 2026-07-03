
var ConsultaExamenesInformacionVM = function () {
    
    var self = this;
    self.nombreNuevoArchivo = ko.observable();

    self.registrarArchivoExamen = function (rutaArchivo) {
        showLoading();
        $.ajax({
            method: "POST",
            url: '/Consultas/RegistrarArchivo',
            data: {
                consultaId: $("#Id").val(),
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
            error: function (jqXHR, textStatus, errorThrown) {
                hideLoading();
                alert("Error al registrar el archivo: " + textStatus);
            }
        });
    };

    self.subirArchivoExamen = function () {
        if (!self.nombreNuevoArchivo()) {
            alert("Asegúrese de proporcionar el nombre del archivo");
        } else {
            showLoading();
            var fileInput = document.querySelector('#nuevo-archivo');
            var file = fileInput.files[0];
            if (file) {
                var reader = new FileReader();
                reader.onload = function (event) {
                    var base64String = event.target.result.split(',')[1];
                    $.ajax({
                        method: "POST",
                        url: '/Files/SubirArchivo',
                        data: {
                            base64Archivo: base64String,
                            extension: file.name.split('.').pop()
                        },
                        success: function (data) {
                            hideLoading();
                            if (data.exitoso) {
                                self.registrarArchivoExamen(data.url);
                            } else {
                                alert(data.mensaje);
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            hideLoading();
                            alert("Error al subir el archivo: " + textStatus);
                        }
                    });
                };
                reader.readAsDataURL(file);
            }
        }
    };
};

var consultaExamenesInformacionVM = new ConsultaExamenesInformacionVM();
ko.applyBindings(consultaExamenesInformacionVM);

