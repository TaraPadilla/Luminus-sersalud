var HabitacionesCategoriasListaVM = function () {
    var self = this;

}


var listaVm = new HabitacionesCategoriasListaVM();
ko.applyBindings(listaVm);

showLoading();

$(document).ready(function () {
    drawDataTable("tabla-categorias");
    hideLoading();
});

function eliminarCategoria(categoriaId) {
    if (confirm("¿Desea eliminar esta categoría?")) {
        showLoading();
        $.ajax({
            url: "/Habitaciones/EliminarCategoria",
            method: "POST",
            data: {
                categoriaId: categoriaId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.href = "/Habitaciones/ListaCategorias";
                } else {
                    hideLoading();
                    alert(data.Mensaje);
                }
            },
            error: function (dataError) {
                hideLoading();
                console.log(dataError);
                alert(dataError);
            }
        });
    }
}