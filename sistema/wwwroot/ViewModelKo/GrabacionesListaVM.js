var GrabacionesListaVM = function () {
    var self = this;
    self.grabaciones = ko.observableArray();

    self.consultarGrabaciones = function () {
        $("#div-loading").show();
        self.clearTableGrabaciones();

        $.ajax({
            method: "POST",
            url: '/Grabaciones/ConsultarGrabaciones',
            success: function (data, textStatus) {
                if (data.exitoso) {
                    self.grabaciones(data.resultado);
                    self.drawTableGrabaciones();
                    $("#div-loading").hide();
                }
                else {
                    $("#div-loading").hide();
                    alert(data.mensaje);
                }
            },
            error: function (data) {
                alert(data.error);
            }
        });
    };
    self.editarGrabacion = function (value) {
        window.location.href = "/Grabaciones/Modificar?grabacionId=" + value.id;
    };
    self.eliminarGrabacion = function (value) {
        if (confirm("¿Desea eliminar esta grabación?")) {
            $("#div-loading").show();
            window.location.href = "/Grabaciones/Eliminar?grabacionId=" + value.id;
        }
    };

    self.clearTableGrabaciones = function () {
        var table = $("#tabla-grabaciones").DataTable();
        table.clear().draw();

        $("#tabla-grabaciones").dataTable().fnDestroy();
    };
    self.drawTableGrabaciones = function () {
        $("#tabla-grabaciones").DataTable({
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

var listaPersonasVM = new GrabacionesListaVM();
ko.applyBindings(listaPersonasVM);

$(document).ready(function () {
    listaPersonasVM.consultarGrabaciones();
});