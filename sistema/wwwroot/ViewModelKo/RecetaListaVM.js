var RecetasListaVM = function () {
    var self = this;
}

var listaVm = new RecetasListaVM();
ko.applyBindings(listaVm);

$(document).ready(function () {
    drawDataTable("tabla-recetas");
});

function modificarReceta(recetaId) {
    window.location.href = "/Receta/Modificar?recetaId=" + recetaId;
}
function eliminarReceta(recetaId) {
    if (confirm("¿Desea eliminar esta receta?")) {
        showLoading();
        $.ajax({
            url: "/Receta/EliminarReceta",
            method: "POST",
            data: {
                recetaId: recetaId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.href = "/Receta/Lista";
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