var LaboratorioClinicoListaExamenesVM = function () {
    var self = this;
}

var listaVm = new LaboratorioClinicoListaExamenesVM();
ko.applyBindings(listaVm);

$(function () {
    drawDataTable("tabla-examenes");

    $("#catexamenId").on('change', function () {
        let categoriaSeleccionadaId = $(this).val();
        let registros = $(".registro-examen");
        $(registros).each(function (idx, vl) {
            if (categoriaSeleccionadaId == 0) {
                $(vl).show();
            } else {
                if ($(vl).hasClass('registro-categoria-' + categoriaSeleccionadaId)) {
                    $(vl).show();
                } else {
                    $(vl).hide();
                }
            }
        });
    });
});