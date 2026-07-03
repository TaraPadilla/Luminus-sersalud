var VacunaVM = function () {
    var self = this;


    self.getModel = function () {
        model = {
            "Id": $("#Id").val(),
            "Nombre": $("#Nombre").val(),
            "Preparacion": $("#Preparacion").val() 

        }


    } 
    self.modificarVacuna = function(){
        if (confirm("¿Desea editar esta Vacuna?")) {
            showLoading();
            self.getModel();
            console.log(model);
            $.ajax({
                method: "POST",
                url: '/Vacuna/ModificarVacuna',
                data: model,
                success: function (data, textStatus) {
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        window.location.href = "/Vacuna/ListaVacunas";
                    }
                    else {
                        hideLoading();
                        alert(dataResult.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }

    }

    self.registrarVacuna = function () {
        if (confirm("¿Desea registrar esta Vacuna?")) {
            showLoading();
            self.getModel();
            console.log(model);
            $.ajax({
                method: "POST",
                url: '/Vacuna/CrearVacuna',
                data: model,
                success: function (data, textStatus) {
                    var dataResult = JSON.parse(data);
                    if (dataResult.Exitoso) {
                        window.location.href = "/Vacuna/ListaVacunas";
                    }
                    else {
                        hideLoading();
                        alert(dataResult.Mensaje);
                    }
                },
                error: function (data) {
                    hideLoading();
                    alert(data.error);
                }
            });
        }

    }


}

var vacunaVM = new VacunaVM();
ko.applyBindings(vacunaVM);

$(document).ready(function () {



});
