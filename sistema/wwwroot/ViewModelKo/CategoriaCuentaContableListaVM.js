var CategoriaListaVM = function () {
    var self = this;
}

var listaVM = new CategoriaListaVM();
ko.applyBindings(listaVM);

$(document).ready(function () {
    drawDataTable("tabla-categoria");
});

function modificarCategoria(categoriaId) {
    window.location.href = "/CategoriasCuentaContable/Modificar?categoriaId=" + categoriaId;
}
function eliminarCategoria(categoriaId) {
    if (confirm("¿Desea eliminar esta categoria?")) {
        showLoading();
        $.ajax({
            url: "/CategoriasCuentaContable/EliminarCategoria",
            method: "POST",
            data: {
                categoriaId: categoriaId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.href = "/CategoriasCuentaContable/Lista";
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