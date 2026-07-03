var CajaAperturarVM = function () {
    var self = this;
    let model = {};

    self.getModel = function () {
        model = {
            AperturarSucursalId: $("#AperturarSucursalId").val(),
            AperturarAmbienteId: $("#AperturarAmbienteId").val(),
            CajaMontoApertura: $("#CajaMontoApertura").val(),
            CajaNombrePersonalizado: $("#CajaNombrePersonalizado").val()
        };
    };


    self.validarAperturarCaja = function () {
        self.getModel();
        if (model.AperturarSucursalId == '0') {
            alert("Seleccione una sucursal");
            return false;
        }
        return true;
    };
    self.aperturarCaja = function () {

        if (self.validarAperturarCaja()) {
            if (confirm("�Desea abrir esta caja?")) {
                showLoading();
                $.ajax({
                    url: "/Caja/Aperturar",
                    method: "POST",
                    data: model,
                    success: function (dataResult) {
                        let data = JSON.parse(dataResult);
                        if (data.Exitoso) {
                            window.location.reload();
                        } else {
                            hideLoading();
                            alert(data.Mensaje);
                        }
                    },
                    error: function (dataerror) {
                        hideLoading();
                        alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
                    }
                });
            }
        }
    };

    self.aplicarFiltroSucursal = function () {
        let itemsSucursalesFiltro = $("input.check-filtro-sucursal");
        $(itemsSucursalesFiltro).each(function (idx, vl) {
            let sucursalId = vl.id.replace('filtro-sucursal-', "");
            if (vl.checked) {
                console.log($(".reg-sucursal-" + sucursalId));
                $(".reg-sucursal-" + sucursalId).show();
            } else {
                $(".reg-sucursal-" + sucursalId).hide();
            }
        });
    };
}

var cajaVm = new CajaAperturarVM();
ko.applyBindings(cajaVm);

$(document).ready(function () {
    $("#valorAperturaDefecto").val(250.00);
    $("#tabs").tabs();
    $("#reservationtime").daterangepicker(
        {
            locale: {
                format: 'YYYY/MM/DD'
            }
        });
});
