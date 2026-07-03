var VisitadorMedicoAddUpdateVM = function () {
    var self = this;
    var model = {};

    // Validar que ningún campo esté vacío
    self.validarModelo = function () {
        if (!$("#NombreVisitador").val().trim()) {
            alert("El campo 'Nombre del Visitador' es obligatorio.");
            return false;
        }
        if (!$("#ContactoVisitador").val().trim()) {
            alert("El campo 'Contacto del Visitador' es obligatorio.");
            return false;
        }
        if (!$("#NombreFarmaceutica").val().trim()) {
            alert("El campo 'Nombre de la Farmacéutica' es obligatorio.");
            return false;
        }
        if (!$("#ContactoFarmaceutica").val().trim()) {
            alert("El campo 'Contacto de la Farmacéutica' es obligatorio.");
            return false;
        }
        var urlCatalogo = $("#UrlCatalogo").val().trim();
        if (urlCatalogo && !self.esUrlCatalogoValida(urlCatalogo)) {
            alert("La URL del catálogo no es válida. Use un enlace completo, por ejemplo: https://www.ejemplo.com/catalogo");
            return false;
        }
        return true;
    };

    self.esUrlCatalogoValida = function (rawUrl) {
        if (!rawUrl) {
            return false;
        }

        var url = rawUrl.trim();
        if (!/^https?:\/\//i.test(url)) {
            url = "https://" + url;
        }

        try {
            var parsed = new URL(url);
            if (parsed.protocol !== "http:" && parsed.protocol !== "https:") {
                return false;
            }

            var host = (parsed.hostname || "").toLowerCase();
            return host.length > 0 && host !== "0.0.0.0" && host !== "0.0.0.1";
        } catch (e) {
            return false;
        }
    };

    // Obtener los datos del formulario
    self.getModel = function () {
        model = {
            "NombreVisitador": $("#NombreVisitador").val().trim(),
            "ContactoVisitador": $("#ContactoVisitador").val().trim(),
            "NombreFarmaceutica": $("#NombreFarmaceutica").val().trim(),
            "ContactoFarmaceutica": $("#ContactoFarmaceutica").val().trim(),
            "Observaciones": $("#Observaciones").val().trim(),
            "UrlCatalogo": $("#UrlCatalogo").val().trim()
        };
    };

    // Registrar un nuevo visitador médico
    self.registrarVisitadorMedico = function () {
        if (!self.validarModelo()) {
            return; // Detiene la ejecución si algún campo es inválido
        }

        if (confirm("¿Desea registrar este visitador médico?")) {
            showLoading();
            self.getModel();
            $.ajax({
                method: "POST",
                url: '/VisitadorMedico/Nuevo',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/VisitadorMedico/Lista";
                    } else {
                        hideLoading();
                        alert(data.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    console.log(data.error);
                    alert("ERROR DE SERVIDOR: " + data.error);
                }
            });
        }
    };

    // Modificar un visitador médico existente
    self.modificarVisitadorMedico = function () {
        if (!self.validarModelo()) {
            return; // Detiene la ejecución si algún campo es inválido
        }

        if (confirm("¿Desea guardar los cambios en este visitador médico?")) {
            showLoading();
            self.getModel();

            // Obtener el parámetro VisitadorMedicoId de la URL
            const urlParams = new URLSearchParams(window.location.search);
            const visitadorMedicoId = urlParams.get('VisitadorMedicoId'); // Extrae el valor del parámetro
            model.Id = visitadorMedicoId; // Asigna el valor al modelo

            $.ajax({
                method: "POST",
                url: '/VisitadorMedico/Modificar',
                data: model,
                success: function (dataResult) {
                    let data = JSON.parse(dataResult);
                    if (data.Exitoso) {
                        window.location.href = "/VisitadorMedico/Lista";
                    } else {
                        hideLoading();
                        alert(data.Mensaje);
                        $("#btnGuardar").prop("disabled", false); // Reactiva el botón
                    }
                },
                error: function (data) {
                    hideLoading();
                    alert("ERROR DE SERVIDOR: " + data.error);
                    $("#btnGuardar").prop("disabled", false); // Reactiva el botón
                }
            });
        }
    };
};

var visitadorMedicoAddUpdateVM = new VisitadorMedicoAddUpdateVM();
ko.applyBindings(visitadorMedicoAddUpdateVM);

$(document).ready(function () {});
