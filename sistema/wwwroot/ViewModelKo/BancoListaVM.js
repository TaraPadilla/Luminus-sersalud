var BancoListaVM = function () {
    var self = this;
}

var listaVM = new BancoListaVM();
ko.applyBindings(listaVM);

$(document).ready(function () {
    drawDataTable("tabla-bancos");
});

function modificarBanco(bancoId) {
    window.location.href = "/Banco/Modificar?bancoId=" + bancoId;
}
function eliminarBanco(bancoId) {
    if (confirm("¿Desea eliminar este banco?")) {
        showLoading();
        $.ajax({
            url: "/Banco/EliminarBanco",
            method: "POST",
            data: {
                bancoId: bancoId
            },
            success: function (dataResult) {
                var data = JSON.parse(dataResult);
                if (data.Exitoso) {
                    window.location.href = "/Banco/Lista";
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