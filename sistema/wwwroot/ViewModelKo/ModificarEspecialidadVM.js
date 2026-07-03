function ModificarEspecialidadViewModel(especialidad) {
    var self = this;

    // Observables para la especialidad
    self.Nombre = ko.observable(especialidad.Nombre || '');
    self.Descripcion = ko.observable(especialidad.Descripcion || '');
    self.Codigo = ko.observable(especialidad.Codigo || '');

    // Función para modificar la especialidad existente
    self.modificarEspecialidad = function() {
        var formData = new FormData();
        
        formData.append('Especialidad.NombreEspecialidad', self.Nombre());
        formData.append('Especialidad.Descripcion', self.Descripcion());
        formData.append('Especialidad.Codigo', self.Codigo());

        fetch('/Especialidad/Modificar', {
            method: 'POST',
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                window.location.href = '/Especialidad/Lista';
            } else {
                console.error('Error al modificar la especialidad:', data.message);
            }
        })
        .catch(error => {
            console.error('Error en la solicitud:', error);
        });
    };
}

ko.applyBindings(new ModificarEspecialidadViewModel(especialidadData));
