var ClinicaInsumosVM = function () {
    var self = this;
    self.precioEditar = ko.observable();
};

var clinicaListaVm = new ClinicaInsumosVM();
ko.applyBindings(clinicaListaVm);

$(document).ready(function () {
    $("#tabla-clinica-insumos").DataTable({
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
        },
        columnDefs: [
            {
                width: "150px",
                targets: [3]
            }
        ]
    });
});

function editarPrecioProducto(precioId, precioValor) {
    medicamentosVm.precioEditar(precioValor);
    $("#mdl-modificar-precio").dialog({
        modal: true,
        buttons: [
            {
                text: "Cancelar",
                class: "btn btn-danger",
                click: function () {
                    $(this).dialog('close');
                }
            },
            {
                text: "Modificar",
                class: "btn btn-success",
                click: function () {
                    if (confirm("Desea modificar este precio?")) {
                        showLoading();
                        $.ajax({
                            url: "/Productos/ModificarPrecio",
                            method: "POST",
                            data: {
                                productoInventarioPrecioId: precioId,
                                productoInventarioPrecioValor: medicamentosVm.precioEditar()
                            },
                            success: function (dataResult) {
                                let data = JSON.parse(dataResult);
                                if (data.Exitoso) {
                                    mensajeEmergente("Precio modificado");
                                    window.location.reload();
                                } else {
                                    hideLoading();
                                    alert(data.Mensaje);
                                }
                            }
                        })
                    }
                }
            }
        ]
    });
}