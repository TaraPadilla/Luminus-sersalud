/* var CuentaPagarVM = function () {
  let itemPago = 1;
  var model = {};
  var self = this;

  self.montoPagoAgregar = ko.observable();
  self.totalPagos = ko.observable(0);
  self.saldoPagos = ko.observable(0);
  self.nombreMedico = ko.observable();

  self.pagos = ko.observableArray();

  self.verInformacionPaciente = function () {
    window.open(
      "/Pacientes/Informacion?pacienteId=" + $("#PacienteId").val(),
      "_blank",
    );
  };

  self.actualizarTotales = function () {
    let totalPagos = 0;

    // Sumar los pagos ya ingresados
    $(self.pagos()).each(function (idx, pago) {
      totalPagos += parseFloat(pago.Monto) || 0;
    });

    // Total a pagar (según lo que calcula la vista)
    let pagoPaciente = parseFloat($("#pagoPaciente").val()) || 0;

    // Saldo restante (permite pago parcial)
    let saldo = Math.max(pagoPaciente - totalPagos, 0);

    self.totalPagos(totalPagos);
    self.saldoPagos(saldo.toFixed(2));
  };

  self.agregarPago = function () {
    let monto = parseFloat(self.montoPagoAgregar()) || 0;

    if (monto <= 0) {
      alert("El monto debe ser mayor a 0.");
      return;
    }

    let pago = {
      Item: itemPago,
      FormaPagoId: $("#FormaPagoId").val(),
      FormaPago: $("#FormaPagoId option:selected").text(),
      MonedaId: $("#MonedaId").val(),
      Moneda: $("#MonedaId option:selected").text(),
      Monto: monto,
      Nuevo: true,
    };

    self.pagos.push(pago);
    itemPago++;
    self.actualizarTotales();
    self.montoPagoAgregar("");
  };

  self.eliminarPago = function (value) {
    $(self.pagos()).each(function (idx, pago) {
      if (value.Item == pago.Item) {
        self.pagos.splice(idx, 1);
      }
    });
    self.actualizarTotales();
  };

  self.consultarPagos = function () {
    showLoading();
    $.ajax({
      url: "/CuentasPorCobrar/ConsultarPagos",
      method: "POST",
      data: {
        cuentaId: $("#CuentaId").val(),
      },
      success: function (dataResult) {
        hideLoading();
        var data = JSON.parse(dataResult);
        if (data.Exitoso) {
          self.pagos(data.Resultado);
          self.actualizarTotales();
        } else {
          alert(data.Mensaje);
        }
      },
      error: function (xhr) {
        hideLoading();
        console.log("❌ Error ConsultarPagos:", xhr);
        let msg = `Error ConsultarPagos\nStatus: ${xhr.status} ${xhr.statusText}\n`;
        if (xhr.responseText) msg += `Response: ${xhr.responseText}\n`;
        alert(msg);
      },
    });
  };

  // Productos: construirlos desde la tabla visible (#productos-body),
  // que incluye paquetes, ambulancia y productos (lo que el usuario ve).
  function construirProductosDesdeTabla() {
    var productos = [];

    $("#productos-body tr").each(function () {
      var fila = $(this);

      // Evitar filas vacías o de "no hay productos"
      if (fila.find("td").length >= 4) {
        var nombre = fila.find("td:nth-child(1)").text().trim();
        var cantidad = parseInt(fila.find("td:nth-child(2)").text().trim(), 10);
        var precioUnitario = parseFloat(
          fila.find("td:nth-child(3)").text().replace("Q", "").trim(),
        );
        var subtotal = parseFloat(
          fila.find("td:nth-child(4)").text().replace("Q", "").trim(),
        );

        if (!nombre) return;
        if (isNaN(cantidad)) cantidad = 1;
        if (isNaN(precioUnitario)) precioUnitario = 0;
        if (isNaN(subtotal)) subtotal = cantidad * precioUnitario;

        productos.push({
          Nombre: nombre,
          Cantidad: cantidad,
          PrecioUnitario: precioUnitario,
          Subtotal: subtotal,
        });
      }
    });

    return productos;
  }

  self.getModel = function () {
    // EmpleadoId por defecto (según tu regla)
    let empleadoId = $("#EmpleadoId").val();
    if (!empleadoId || String(empleadoId).trim() === "") {
      empleadoId = 3;
      $("#EmpleadoId").val(empleadoId);
    }

    model = {
      // Datos de factura
      NoComprobante: $("#NoComprobante").val(),
      EmpleadoId: empleadoId,

      // Datos de cuenta
      CuentaId: $("#CuentaId").val(),
      Valor: parseFloat($("#Valor").val()) || 0,
      Observaciones: $("#Observaciones").val(),

      // Datos del paciente
      PacienteId: $("#PacienteId").val(),
      PacienteNombre: $("#PacienteNombre").val(), // este viene del HiddenFor
      PacienteNit: $("#PacienteNit").val(),

      // Datos de pago
      Pagos: self.pagos(),

      // Productos actuales (lo visible)
      Productos: construirProductosDesdeTabla(),
    };
  };

  function getFormattedDateTimeForFEL() {
    const now = new Date();

    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, "0");
    const day = String(now.getDate()).padStart(2, "0");
    const hours = String(now.getHours()).padStart(2, "0");
    const minutes = String(now.getMinutes()).padStart(2, "0");
    const seconds = String(now.getSeconds()).padStart(2, "0");

    // Zona horaria fija a -06:00
    const timezone = "-06:00";

    return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}${timezone}`;
  }

  const FechaHoraEmisionLocal = getFormattedDateTimeForFEL();

  self.pagarCuenta = function () {
    if (!confirm("¿Desea registrar el pago de esta cuenta?")) {
      // Antes había: alert(data.Mensaje) (data no existe aquí)
      return;
    }

    showLoading();

    try {
      self.getModel();

      // Obtener totales desde la vista
      let totalAseguradora = parseFloat($("#totalAseguradora").val()) || 0;
      let totalNoElegibles = parseFloat($("#totalNoElegibles").val()) || 0;
      let pagoPaciente = parseFloat($("#pagoPaciente").val()) || 0;

      // Nombre para descripción: usar el nuevo ID de la vista
      let pacienteNombreAdmisionEl = document.getElementById(
        "PacienteNombreAdmision",
      );
      let pacienteNombreEl = document.getElementById("PacienteNombre");

      let pacienteNombre =
        pacienteNombreAdmisionEl && pacienteNombreAdmisionEl.value
          ? pacienteNombreAdmisionEl.value
          : pacienteNombreEl
            ? pacienteNombreEl.value
            : "";

      let admisionIdEl = document.getElementById("AdmisionId");
      let admisionId = admisionIdEl ? admisionIdEl.value : "";

      let descripcion = `Servicio de Hospitalización Paciente: ${pacienteNombre} (Admision #${admisionId})`;
      let desc = String(descripcion || "").toUpperCase();

      // Gran total: en tu lógica actual estás facturando por pagoPaciente
      let granTotal = pagoPaciente;

      const itemHospitalizacion = {
        BienOServicio: "S",
        NumeroLinea: "1",
        Cantidad: 1,
        UnidadMedida: "UND",
        Descripcion: desc,
        PrecioUnitario: granTotal,
        Precio: granTotal,
        Descuento: 0,
        Impuestos: [
          {
            NombreCorto: "IVA",
            CodigoUnidadGravable: "1",
            MontoGravable: (granTotal / 1.12).toFixed(2),
            MontoImpuesto: (granTotal - granTotal / 1.12).toFixed(2),
          },
        ],
        Total: granTotal.toFixed(2),
      };

      const requestXMLData = {
        CodigoMoneda: "GTQ",
        FechaHoraEmision: FechaHoraEmisionLocal,
        TipoDocumento: "FACT",
        Emisor: {
          AfiliacionIVA: "GEN",
          CodigoEstablecimiento: "1",
          CorreoEmisor: "recepcion@hcq.com",
          NITEmisor: "117286303",
          NombreComercial: "HOSPITAL CLINICO QUIRURGICO SANCTI SPIRITUS",
          NombreEmisor: "HOSPITAL CLINICO QUIRURGICO SANCTI SPIRITUS",
          Direccion: {
            DetalleDireccion:
              "4 CALLE 1-91 BARRIO SAN SEBASTIAN ZONA 3 SAN CRISTÓBAL VERAPAZ, ALTA VERAPAZ",
            CodigoPostal: "01011",
            Municipio: "Guatemala",
            Departamento: "Guatemala",
            Pais: "GT",
          },
        },
        Receptor: {
          CorreoReceptor:
            $("#ResponsableCorreo").val() || "sin-correo@example.com",
          IDReceptor: $("#ResponsableNit").val() || "CF",
          NombreReceptor: $("#ResponsableNombre").val() || "Consumidor Final",
          Direccion: {
            DetalleDireccion: $("#ResponsableDireccion").val() || "N/A",
            CodigoPostal: "01001",
            Municipio: "Guatemala",
            Departamento: "Guatemala",
            Pais: "GT",
          },
        },
        Frases: [
          {
            CodigoEscenario: "1",
            TipoFrase: "1",
          },
        ],
        Items: [itemHospitalizacion],
        Totales: {
          TotalImpuestos: [
            {
              NombreCorto: "IVA",
              TotalMontoImpuesto:
                itemHospitalizacion.Impuestos[0].MontoImpuesto,
            },
          ],
          GranTotal: granTotal.toFixed(2),
        },
        Adenda: "FACTURA-19",
      };

      fetch("http://3.128.79.135/Xml/GenerateXml", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(requestXMLData),
      })
        .then((response) => {
          if (!response.ok) {
            throw new Error(
              `Error al generar el XML (HTTP ${response.status})`,
            );
          }
          return response.json();
        })
        .then((data) => {
          console.log("XML generado:", data);

          return fetch("http://3.128.79.135/Xml/SendXml", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
              XmlContent: data.xmlContent,
              UsuarioFirma: "117286303",
              LlaveFirma: "3f324367093d7bef36bdf933a1e95aff",
              UsuarioApi: "117286303",
              LlaveApi: "DF34FB82B6FE81A4CC20979661FFF8F4",
            }),
          });
        })
        .then((response) => {
          if (!response.ok) {
            throw new Error(`Error al enviar el XML (HTTP ${response.status})`);
          }
          return response.json();
        })
        .then((data) => {
          if (data.uuid) {
            model.UuidFel = data.uuid;
          } else {
            console.error("Error: UUID está vacío o nulo.");
            hideLoading();
            alert(
              "El UUID no fue generado correctamente. Por favor, verifica el estado del servicio.",
            );
            return;
          }

          const pdfUrl = `http://3.128.79.135/Xml/GeneratePDF?uuid=${data.uuid}`;
          const pdfWindow = window.open(pdfUrl, "_blank");
          if (!pdfWindow) {
            console.error(
              "El navegador bloqueó la apertura de la pestaña del PDF.",
            );
            alert(
              "Permite la apertura de ventanas emergentes para mostrar el PDF.",
            );
          }

          $.ajax({
            method: "POST",
            url: "/CuentasPorCobrar/Pagar",
            data: model,
            success: function (dataResult) {
              var data = JSON.parse(dataResult);
              if (data.Exitoso) {
                window.location.href = "/CuentasPorCobrar/Pagadas";
              } else {
                hideLoading();
                alert(data.Mensaje);
              }
            },
            error: function (xhr) {
              hideLoading();
              console.log("❌ Error /CuentasPorCobrar/Pagar:", xhr);

              let msg = `Error al pagar\nStatus: ${xhr.status} ${xhr.statusText}\n`;
              if (xhr.responseText) msg += `Response: ${xhr.responseText}\n`;
              if (xhr.responseJSON)
                msg += `JSON: ${JSON.stringify(xhr.responseJSON)}\n`;

              alert(msg);
            },
          });
        })
        .catch((error) => {
          hideLoading();
          console.error("Error:", error);
          alert(
            "Hubo un error al procesar la solicitud.\n" +
              (error && error.message ? error.message : ""),
          );
        });
    } catch (e) {
      hideLoading();
      console.error("❌ Error inesperado:", e);
      alert("Error inesperado al procesar el pago.");
    }
  };
};

var cuentaPagarVm = new CuentaPagarVM();
ko.applyBindings(cuentaPagarVm);

$(document).ready(function () {
  cuentaPagarVm.consultarPagos();
  cuentaPagarVm.actualizarTotales();
  $("#tabs-datos-cuenta").tabs();
});
 */