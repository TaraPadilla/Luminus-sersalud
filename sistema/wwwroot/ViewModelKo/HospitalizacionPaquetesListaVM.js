var HospitalizacionPaquetesListaVM = function () {
    var self = this;
    self.pacientesExistentes = ko.observableArray();
    self.pacienteSeleccionadoImprimirCotizacion = ko.observable();

    self.pacienteSeleccionadoImprimirCotizacion.subscribe(function (paciente) {
        console.log(paciente);
    });
}

var paquetesVm = new HospitalizacionPaquetesListaVM();
ko.applyBindings(paquetesVm);

$(document).ready(function () {
    drawDataTable("tabla-paquetes");
});

function imprimirPaquete(paqueteId) {
    showLoading();
    $.ajax({
        url: "/Hospitalizacion/ConsultarPacientes",
        method: "POST",
        success: function (result) {
            hideLoading();
            let data = JSON.parse(result);
            if (!data.Exitoso) {
                mensajeEmergenteError("Error al consultar pacientes existentes");
                return;
            }

            let $select = $("#select-paciente-cotizacion");
            if ($select.hasClass("select2-hidden-accessible")) {
                $select.select2("destroy");
            }

            // Limpiar y poblar el select con los pacientes recibidos
            $select.empty().append('<option value="">-- Seleccione un paciente --</option>');
            $.each(data.Resultado, function (idx, paciente) {
                $select.append(
                    $("<option>").val(paciente.Id).text(paciente.NombreConDpi)
                );
            });

            // Mostrar el modal
            let modalImprimir = $("#mdl-imprimir-paquete").dialog({
                modal: true,
                width: 450,
                height: 500,
                open: function () {
                    $select.select2({
                        dropdownParent: $("#mdl-imprimir-paquete"),
                        placeholder: "Buscar paciente...",
                        allowClear: true,
                        width: "100%"
                    });
                },
                buttons: [
                    {
                        text: "Imprimir",
                        class: "btn btn-success",
                        click: function () {
                            let pacienteId = $select.val();
                            if (!pacienteId) {
                                mensajeEmergenteError("Seleccione un paciente");
                                return;
                            }
                            window.open(
                                "/CrearPDF/PaqueteCotizacionPDF?paqueteId=" + paqueteId
                                + "&pacienteId=" + pacienteId,
                                "_blank"
                            );
                        }
                    },
                    {
                        text: "Cerrar",
                        class: "btn btn-danger",
                        click: function () {
                            modalImprimir.dialog("close");
                        }
                    }
                ]
            });
        },
        error: function (resultError) {
            hideLoading();
            mensajeEmergenteError("Error de conexion");
            console.log(resultError);
        }
    });
}
function eliminarPaquete(paqueteId) {
    if (confirm("�desea eliminar este paquete?")) {
        showLoading();
        $.ajax({
            url: "/HospitalizacionPaquetes/Eliminar",
            method: "POST",
            data: {
                paqueteId: paqueteId
            },
            success: function (dataResult) {
                let data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.href = "/HospitalizacionPaquetes/Lista";
                } else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (dataError) {
                hideLoading();
                alert("Error de llamado al servidor: " + dataError.error);
                console.log(dataError);
            }
        });
    }
}