function NuevoEspecialidadViewModel() {
    var self = this;

    // Observables compartidos para Especialidad
    self.Nombre = ko.observable('');
    self.Descripcion = ko.observable('');
    self.Codigo = ko.observable('');

    // Función para agregar una nueva especialidad
    self.agregarEspecialidad = function() {
        var formData = new FormData();

        // Agregar campos obligatorios
        formData.append('Especialidad.NombreEspecialidad', self.Nombre());
        formData.append('Especialidad.Descripcion', self.Descripcion());
        formData.append('Especialidad.Codigo', self.Codigo());

        fetch('/Especialidad/Nuevo', {
            method: 'POST',
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                window.location.href = '/Especialidad/Lista';
            } else {
                console.error('Error al guardar la especialidad:', data.message);
            }
        })
        .catch(error => {
            console.error('Error AAA en la solicitud:', error);
        });
    };
}

ko.applyBindings(new NuevoEspecialidadViewModel());
