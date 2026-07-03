var ConsultasListaVM = function () {
    var self = this;

    self.drawTableConsultas = function () {
        $("#tabla-consultas").DataTable({
            searching: true,
            ordering: true,
            order: [[0, 'desc']],
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

var consultaListaVm = new ConsultasListaVM();
ko.applyBindings(consultaListaVm);

$(function () {
    contraerMenu();
    consultaListaVm.drawTableConsultas();
    //$("#fechacita").on("change", function () {
    //    showLoading();
    //    window.location.href = "/Cita/CalendarioLineal?buscar=" +
    //        $("#fechacita").val()
    //        + "&sucursalId=" + $("#SucursalId").val()
    //        + "&empleadoId=" + $("#EmpleadoId").val();
    //});
    //$("#SucursalId").on("change", function () {
    //    showLoading();
    //    window.location.href = "/Cita/CalendarioLineal?buscar=" +
    //        $("#fechacita").val()
    //        + "&sucursalId=" + $("#SucursalId").val()
    //        + "&empleadoId=" + $("#EmpleadoId").val();
    //});
    $("#EmpleadoId").on("change", function () {
        showLoading();
        window.location.href = "/Consultas/Index?buscar=" +
            $("#fechacita").val()
            //+ "&sucursalId=" + $("#SucursalId").val()
            + "&empleadoId=" + $("#EmpleadoId").val() 
            + "&EspecialidadId=" + $("#EspecialidadId").val();
    });
    $("#EspecialidadId").on("change", function () {
        showLoading();
        window.location.href = "/Consultas/Index?buscar=" +
            $("#fechacita").val()
            /*+ "&sucursalId=" + $("#SucursalId").val()*/
            + "&empleadoId=" + $("#EmpleadoId").val() 
            + "&EspecialidadId=" + $("#EspecialidadId").val();
    });
});