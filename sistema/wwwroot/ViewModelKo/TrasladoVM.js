var TrasladoVM = function () {
  var self = this;
  self.productosDisponibles = ko.observableArray();
  self.productosTrasladados = ko.observableArray();
  let model = {};
  let itemProductoTraslado = 1;

  self.currentPage = ko.observable(1);
  self.pageSize = ko.observable(9);

  self.searchQuery = ko.observable("");

  self.filteredProductosTrasladados = ko.computed(function () {
    let filter = self.searchQuery().toLowerCase();
    let sortedArray = self.productosTrasladados().slice().sort(function (a, b) {
      return (
        new Date(b.FechaTraslado).getTime() -
        new Date(a.FechaTraslado).getTime()
      );
    });

    if (!filter) {
      return sortedArray;
    } else {
      return ko.utils.arrayFilter(sortedArray, function (item) {
        return (item.ProductoNombre || "").toLowerCase().includes(filter);
      });
    }
  });

  self.paginatedProductosTrasladados = ko.computed(function () {
    let startIndex = (self.currentPage() - 1) * self.pageSize();
    return self
      .filteredProductosTrasladados()
      .slice(startIndex, startIndex + self.pageSize());
  });

  self.totalPages = ko.computed(function () {
    return Math.ceil(
      self.filteredProductosTrasladados().length / self.pageSize()
    );
  });

  self.nextPage = function () {
    if (self.currentPage() < self.totalPages()) {
      self.currentPage(self.currentPage() + 1);
    }
  };

  self.prevPage = function () {
    if (self.currentPage() > 1) {
      self.currentPage(self.currentPage() - 1);
    }
  };

  self.consultarProductosDisponibles = function () {
    showLoading();
    clearDataTable("tabla-traslado");
    self.productosDisponibles([]);
    $.ajax({
      url: "/Traslados/ConsultarProductosInventario",
      method: "POST",
      data: {
        bodegaOrigenId: $("#BodegaOrigenId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          $(data.Resultado).each(function (idx, vl) {
            vl.CantidadTrasladada = ko.observable(0);
            self.productosDisponibles.push(vl);
          });
          drawDataTable("tabla-traslado");
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (dataerror) {
        hideLoading();
        alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
      },
    });
  };

  self.ordenarProductosTrasladados = function () {
    let sortedArray = self.productosTrasladados().slice().sort(function (a, b) {
      return (
        new Date(b.FechaTraslado).getTime() -
        new Date(a.FechaTraslado).getTime()
      );
    });

    self.productosTrasladados(sortedArray); // Reasigna el array para que Knockout detecte el cambio
  };

  self.anniadirProductoTraslado = function (producto) {
    let agregado = false;
    self.productosTrasladados().forEach(function (vlTraslado) {
      if (vlTraslado.ProductoInventarioId === producto.ProductoInventarioId) {
        let cantidadActual = Number(vlTraslado.CantidadTrasladada());
        let cantidadNueva = cantidadActual + 1;

        if (cantidadNueva > producto.CantidadExistente) {
          mensajeEmergente("Cantidad no válida");
        } else {
          vlTraslado.CantidadTrasladada(cantidadNueva);
        }
        agregado = true;
      }
    });

    if (!agregado) {
      producto.Item = itemProductoTraslado;
      producto.CantidadTrasladada = ko.observable(1);
      producto.Nuevo = true;
      producto.FechaTraslado = new Date().toISOString();
      self.productosTrasladados.push(producto);
      itemProductoTraslado++;

      self.ordenarProductosTrasladados(); // Ordenar después de agregar
    }
  };

  self.quitarProductoTraslado = function (producto) {
    self.productosTrasladados.remove(producto);
    self.ordenarProductosTrasladados(); // Ordenar después de quitar
  };

  function bodegaSeleccionada(selector) {
    var v = $(selector).val();
    return v !== null && v !== undefined && String(v).trim() !== "";
  }

  self.validateModel = function () {
    var $origen = $("#BodegaOrigenId");
    if ($origen.find("option[value!='']").length === 0) {
      alert("No hay bodegas de origen configuradas. Verifique sucursales y bodegas en el sistema.");
      return false;
    }
    if (!bodegaSeleccionada($origen)) {
      alert("Seleccione una bodega de origen");
      return false;
    }

    if (!bodegaSeleccionada("#BodegaDestinoId")) {
      alert("Seleccione una bodega de destino");
      return false;
    }

    return true;
  };

  self.getModel = function () {
    model = {
      TrasladoId: $("#TrasladoId").val(),
      BodegaOrigenId: $("#BodegaOrigenId").val(),
      BodegaDestinoId: $("#BodegaDestinoId").val(),
      Productos: ko.toJS(self.productosTrasladados()).map((p) => ({
        ProductoInventarioId: p.ProductoInventarioId,
        CantidadTrasladada: p.CantidadTrasladada,
        FechaTraslado: p.FechaTraslado,
      })),
      Observaciones: $("#Observaciones").val(),
    };
  };

  self.cancelarRegistroTraslado = function () {
    if (confirm("�Desea cancelar el registro de este traslado?")) {
      window.location.href = "/Traslados/Lista";
    }
  };

  self.guardarTraslado = function () {
    if (self.validateModel()) {
      if (confirm("�Desea registrar este traslado?")) {
        showLoading();
        self.getModel();
        $.ajax({
          url: "/Traslados/GuardarTraslado",
          method: "POST",
          data: model,
          success: function (dataResult) {
            let data = JSON.parse(dataResult);
            if (data.Exitoso) {
              window.location.href = "/Traslados/Lista";
            } else {
              hideLoading();
              alert(data.Mensaje);
            }
          },
          error: function (dataerror) {
            hideLoading();
            alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
          },
        });
      }
    }
  };

  self.cancelarEditarTraslado = function () {
    if (confirm("�Desea cancelar la edicion de este traslado?")) {
      window.location.href = "/Traslados/Lista";
    }
  };

  self.editarTraslado = function () {
    if (self.validateModel()) {
      if (confirm("�Desea editar este traslado?")) {
        showLoading();
        self.getModel();
        $.ajax({
          url: "/Traslados/EditarTraslado",
          method: "POST",
          data: model,
          success: function (dataResult) {
            let data = JSON.parse(dataResult);
            if (data.Exitoso) {
              window.location.href = "/Traslados/Lista";
            } else {
              hideLoading();
              alert(data.Mensaje);
            }
          },
          error: function (dataerror) {
            hideLoading();
            alert("ERROR DE LLAMADO ASINCRONO: " + dataerror);
          },
        });
      }
    }
  };

  self.consultarProductosTrasladados = function () {
    let trasladoId = $("#TrasladoId").val();
    showLoading();
    $.ajax({
      url: "/Traslados/ConsultarProductosTrasladados",
      method: "POST",
      data: { trasladoId: trasladoId },
      success: function (dataResult) {
        hideLoading();
        let data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.productosTrasladados([]);
          data.Resultado.forEach(function (vl) {
            vl.CantidadTrasladada = ko.observable(vl.CantidadTrasladada);
            vl.Item = itemProductoTraslado++;
            self.productosTrasladados.push(vl);
          });

          self.ordenarProductosTrasladados(); // Ordenar después de cargar
        } else {
          alert(data.Mensaje);
        }
      },
    });
  };

  self.aceptarTraslado = function () {
    if (confirm("�Desea aceptar este traslado?")) {
      showLoading();
      self.getModel();
      $.ajax({
        url: "/Traslados/AceptarTraslado",
        method: "POST",
        data: {
          id: $("#TrasladoId").val(),
        },
        success: function (dataResult) {
          let data = JSON.parse(dataResult);
          if (data.Exitoso) {
            window.location.href = "/Traslados/Lista";
          } else {
            hideLoading();
            alert(data.Mensaje);
          }
        },
      });
    }
  };

  self.verProductosEnCompras = function () {
    let productos = ko.toJS(self.productosTrasladados());

    if (productos.length === 0) {
      alert("No hay productos trasladados para enviar.");
      return;
    }

    // Tomar la bodega de destino seleccionada en el formulario
    let bodegaDestinoId = $("#BodegaDestinoId").val();

    if (!bodegaDestinoId) {
      alert(
        "Debe seleccionar la ubicación de destino antes de ver las cotizaciones."
      );
      return;
    }

    let form = $("<form>", {
      action: "/ComprasCotizaciones/MostrarProductos",
      method: "POST",
    });

    let input = $("<input>", {
      type: "hidden",
      name: "productos",
      value: JSON.stringify(
        productos.map((p) => ({
          ProductoInventarioId: p.ProductoInventarioId,
          CantidadTrasladada: p.CantidadTrasladada,
        }))
      ),
    });

    // Enviar también la bodega destino
    let inputBodegaDestino = $("<input>", {
      type: "hidden",
      name: "bodegaDestinoId",
      value: bodegaDestinoId,
    });

    form.append(input);
    form.append(inputBodegaDestino);
    $("body").append(form);
    form.submit();
  };
};

var trasladoVm = new TrasladoVM();
ko.applyBindings(trasladoVm);

$(document).ready(function () {
  $("#BodegaOrigenId").change(function () {
    trasladoVm.consultarProductosDisponibles();
  });

  var bodegaOrigenInicial = $("#BodegaOrigenId").val();
  if (bodegaOrigenInicial) {
    trasladoVm.consultarProductosDisponibles();
  }

  //Consultar productos (Editar traslado)
  let trasladoId = $("#TrasladoId").val();
  if (
    trasladoId != undefined &&
    trasladoId != null &&
    trasladoId.trim() != ""
  ) {
    trasladoVm.consultarProductosTrasladados();
  }
});
