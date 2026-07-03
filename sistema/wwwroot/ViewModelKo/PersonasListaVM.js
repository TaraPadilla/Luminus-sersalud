var PersonasListaVM = function () {
    var self = this;
    self.personas = ko.observableArray();

    self.consultarPersonas = function () {
        self.clearTablePersonas();
        $.ajax({
            method: "POST",
            url: '/Personas/ConsultarPersonas',
            success: function (data, textStatus) {
                if (data.exitoso) {

                    var cuentasProcesadas = data.resultado.map(function (item) {
                        return {
                            id: item.id,
                            nombre: item.nombre ? item.nombre : '-',
                            telefono: item.telefono ? item.telefono : '-',
                            email: item.email ? item.email : '-',
                            redSocial: item.tipoRedSocial ? item.tipoRedSocial.nombreRedSocial : '-',
                            sexo: item.sexo ? item.sexo.descripcionSexo : '-',
                            fechaContacto: moment(item.fechaContacto).format('ll'),
                            paciente: !item.paciente ? 'No' : 'Sí'
                        };
                    });
                    console.log(cuentasProcesadas);
                    self.personas(cuentasProcesadas);
                    self.drawTablePersonas();
                }
                else
                    alert(data.mensaje);
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };
    self.editarPersona = function (value) {
        window.location.href = "/Personas/Modificar/" + value.id;
    };
    self.convertirPaciente = function (value) {
        if (confirm("¿Desea registrar a esta persona como paciente?")) {
            $.ajax({
                method: "POST",
                data: {
                    id: value.id
                },
                url: '/Personas/ConvertirPaciente',
                success: function (data, textStatus) {
                    if (data.exitoso) {
                        window.location.href = "/Personas/Lista";
                    }
                    else
                        alert(data.mensaje);
                },
                error: function (data) {
                    alert(data.error);
                }
            });
        }
    };
    self.eliminarPersona = function (value) {
        if (confirm("¿Desea eliminar esta persona?")) {
            $.ajax({
                method: "POST",
                data: {
                    id: value.id
                },
                url: '/Personas/Eliminar',
                success: function (data, textStatus) {
                    if (data.exitoso) {
                        window.location.href = "/Personas/Lista";
                    }
                    else
                        alert(data.mensaje);
                },
                error: function (data) {
                    alert(data.error);
                }
            });
        }
    };
    self.clearTablePersonas = function () {
        var table = $("#tabla-personas").DataTable();
        table.clear().draw();

        $("#tabla-personas").dataTable().fnDestroy();
    };
    self.drawTablePersonas = function () {
        $("#tabla-personas").DataTable({
            searching: true,
            ordering: true,
            paging: true,
            language: {
                search: "Buscar: ",
                lengthMenu: "Mostrar _MENU_ registros por página",
                zeroRecords: "No hay registros para mostrar",
                info: "Mostrando página _PAGE_ de _PAGES_",
                infoEmpty: "",
                infoFiltered: "(filtrado de _MAX_ registros totales)",
                paginate: {
                    first: "Primero",
                    last: "Último",
                    previous: "Anterior",
                    next: "Siguiente"
                }
            }
        });
    };

}

var listaPersonasVM = new PersonasListaVM();
ko.applyBindings(listaPersonasVM);

$(document).ready(function () {
    listaPersonasVM.consultarPersonas();
});