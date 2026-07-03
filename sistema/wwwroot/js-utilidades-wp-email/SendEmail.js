function _parseEmailApiResponse(response) {
  return response.text().then(function (text) {
    if (!response.ok) {
      var detail = text;
      try {
        var json = JSON.parse(text);
        detail = json.error || json.message || text;
      } catch (e) { /* usar texto plano */ }
      throw new Error(detail || ("Error HTTP " + response.status));
    }
    return text;
  });
}

function enviarResultadosLaboratorioEmail(
  examenId,
  nombrePaciente,
  emailPaciente
) {
  let bodyMessage = "";

  if (!emailPaciente) {
    alert("El paciente no tiene un correo electrónico registrado.");
    return;
  }

  // 1. Obtener la plantilla HTML — ruta absoluta para funcionar desde cualquier página
  fetch("/js-utilidades-wp-email/email-plantilla-resultados-examen.html")
    .then((response) => {
      if (!response.ok) throw new Error("Plantilla no encontrada (resultados-examen): " + response.status);
      return response.text();
    })
    .then((html) => {
      // 2. Reemplazar los comodines con los datos dinámicos de la empresa
      bodyMessage = html
        .replaceAll("{{nombrePaciente}}", nombrePaciente)
        .replaceAll("{{NombreEmpresa}}", window.EmpresaConfig.Nombre)
        .replaceAll("{{LogoEmpresa}}", window.EmpresaConfig.Logo)
        .replaceAll("{{TelefonoEmpresa}}", window.EmpresaConfig.Telefono)
        .replaceAll("{{DireccionEmpresa}}", window.EmpresaConfig.Direccion);

      // 3. Obtener el PDF del resultado
      return fetch(`/CrearPDF/GenerarResultados/${examenId}`);
    })
    .then((response) => {
      if (!response.ok) throw new Error("PDF resultados falló: " + response.status);
      return response.blob();
    })
    .then((blob) => {
      // 4. Armar el correo a enviar
      const formData = new FormData();
      formData.append("Subject", "Resultados de laboratorio - " + window.EmpresaConfig.Nombre);
      formData.append("Body", bodyMessage);
      formData.append("To", emailPaciente);
      formData.append("Attachments", blob, "Resultados.pdf");

      // 5. Enviar el correo
      return fetch("/api/SendEmail", {
        method: "POST",
        body: formData,
      });
    })
    .then(_parseEmailApiResponse)
    .then(function () {
      alert("Correo enviado exitosamente.");
    })
    .catch(function (error) {
      console.error("Error en enviarResultadosLaboratorioEmail:", error.message);
      alert("Error al enviar el correo: " + error.message);
    });
}

function enviarReporteCompraEmail(compraId, proveedorCorreo) {
  if (!proveedorCorreo) {
    alert("El proveedor no tiene un correo electrónico registrado.");
    return;
  }

  // Cuerpo del mensaje construido dinámicamente inyectando window.EmpresaConfig
  let bodyMessage = `
        <div style="display: flex; align-items: center; margin-bottom: 20px;">
            <img src="${window.EmpresaConfig.Logo}" alt="Logo de ${window.EmpresaConfig.Nombre}" style="width: 200px; height: auto; margin-right: 20px;">
            <div>
                <h1 style="color: blue; font-size: 3em; text-align: center;">${window.EmpresaConfig.Nombre}</h1>
                <p style="text-align: center;"><strong>Cuidamos de ti, como persona y como paciente</strong></p>
                <p style="text-align: center;"><strong>${window.EmpresaConfig.Direccion}</strong></p>
                <p style="text-align: center;"><strong>Tel. ${window.EmpresaConfig.Telefono}</strong></p>
                <p style="text-align: center;"><strong>Horario de atención: de lunes a domingo las 24 Hrs</strong></p>
            </div>
        </div>
        <p>Estimado/a Proveedor,</p>
        <p>
            Esperamos que se encuentre bien. Adjunto a este correo encontrará la orden de compra solicitada,
            esperando su respuesta de recibido y enviarse lo antes posible.
        </p>
        <p>
            Si existiera algún inconveniente de existencia, tiempo de envío del producto por favor notificarlo
            de forma inmediata para que nosotros no tengamos ningún inconveniente en los tiempos de recepción de los productos.
        </p>
    `;

  // Obtener el reporte PDF de la compra
  fetch(`/Compra/Reporte?CompraId=${compraId}`)
    .then((response) => {
      if (!response.ok) throw new Error("PDF compra falló: " + response.status);
      return response.blob();
    })
    .then((blob) => {
      const formData = new FormData();
      formData.append("Subject", `Orden de compra - ${window.EmpresaConfig.Nombre}`);
      formData.append("Body", bodyMessage);
      formData.append("To", proveedorCorreo);
      formData.append("Attachments", blob, "Orden.pdf");

      return fetch("/api/SendEmail", {
        method: "POST",
        body: formData,
      });
    })
    .then(_parseEmailApiResponse)
    .then(function () {
      alert("Correo enviado exitosamente.");
    })
    .catch(function (error) {
      console.error("Error en enviarReporteCompraEmail:", error.message);
      alert("Error al enviar el correo: " + error.message);
    });
}

// =======================================================
// Enviar Información de Hospitalización (Con 3 Adjuntos)
// =======================================================
function enviarHospitalizacionEmail(
  hospitalizacionId,
  nombrePaciente,
  emailPaciente,
  idPaciente,
  idHabitacion,
  citaId
) {
  // RETORNO de promesa resuelta si no hay correo para que no rompa el flujo
  if (!emailPaciente || emailPaciente.trim() === "") {
    alert("El paciente no tiene un correo electrónico registrado.");
    return Promise.resolve();
  }

  const urlPlantilla = "/js-utilidades-wp-email/email-plantilla-hospitalizacion.html";
  const urlConsentimiento = `/CrearPDF/GenerarPDFConsentimientoHospi?idPaciente=${idPaciente}&idHabitacion=${idHabitacion}&idHospi=0&citaId=${citaId}`;
  const urlAutorizacion = `/CrearPDF/GenerarPDFHospiReports?idPaciente=${idPaciente}&idHabitacion=${idHabitacion}&idHospi=${hospitalizacionId}&report=1&citaId=${citaId}`;
  const urlAsignacion = `/CrearPDF/GenerarPDFHospiReports?idPaciente=${idPaciente}&idHabitacion=${idHabitacion}&idHospi=${hospitalizacionId}&report=2&citaId=${citaId}`;

  return Promise.all([
    fetch(urlPlantilla).then((res) => {
      if (!res.ok) throw new Error("Plantilla no encontrada");
      return res.text();
    }),
    fetch(urlConsentimiento).then((res) => {
      if (!res.ok) throw new Error("PDF Consentimiento falló");
      return res.blob();
    }),
    fetch(urlAutorizacion).then((res) => {
      if (!res.ok) throw new Error("PDF Autorización falló");
      return res.blob();
    }),
    fetch(urlAsignacion).then((res) => {
      if (!res.ok) throw new Error("PDF Asignación falló");
      return res.blob();
    }),
  ])
    .then(([htmlPlantilla, blobConsentimiento, blobAutorizacion, blobAsignacion]) => {
      let bodyMessage = htmlPlantilla
        .replaceAll("{{nombrePaciente}}", nombrePaciente)
        .replaceAll("{{NombreEmpresa}}", window.EmpresaConfig.Nombre)
        .replaceAll("{{LogoEmpresa}}", window.EmpresaConfig.Logo)
        .replaceAll("{{TelefonoEmpresa}}", window.EmpresaConfig.Telefono)
        .replaceAll("{{DireccionEmpresa}}", window.EmpresaConfig.Direccion);

      const formData = new FormData();
      formData.append("Subject", "Confirmación de Registro - " + window.EmpresaConfig.Nombre);
      formData.append("Body", bodyMessage);
      formData.append("To", emailPaciente);
      formData.append("Attachments", blobConsentimiento, "Consentimiento.pdf");
      formData.append("Attachments", blobAutorizacion, "Autorizacion_Sala.pdf");
      formData.append("Attachments", blobAsignacion, "Asignacion_Anestesiologo.pdf");

      return fetch("/api/SendEmail", {
        method: "POST",
        body: formData,
      });
    })
    .then(_parseEmailApiResponse)
    .then(function () {
      console.log("Correo enviado a: " + emailPaciente);
      if (typeof toastr !== "undefined") {
        toastr.success("Correo enviado a " + emailPaciente);
      }
    })
    .catch(function (error) {
      console.error("Error al enviar correo a " + emailPaciente + ":", error.message);
      if (typeof toastr !== "undefined") {
        toastr.error("No se pudo enviar el correo: " + error.message);
      }
      return Promise.resolve();
    });
}


// =======================================================
// Enviar Alerta de Inventario
// =======================================================
function enviarAlertaInventarioEmail(emailDestino) {
  let bodyMessage = "";

  if (!emailDestino) {
    alert("No se ha proporcionado un correo de destino para la alerta.");
    return;
  }

  // 1. Obtener la plantilla — ruta absoluta
  fetch("/js-utilidades-wp-email/email-plantilla-producto-vencimiento.html")
    .then((response) => {
      if (!response.ok) throw new Error("Plantilla vencimiento no encontrada: " + response.status);
      return response.text();
    })
    .then((html) => {
      // 2. Reemplazar los comodines
      bodyMessage = html
        .replaceAll("{{NombreEmpresa}}", window.EmpresaConfig.Nombre)
        .replaceAll("{{LogoEmpresa}}", window.EmpresaConfig.Logo);

      // 3. Obtener el PDF del reporte de inventario
      return fetch(`/Inventario/GenerarReporteVencimiento`);
    })
    .then((response) => {
      if (!response.ok) throw new Error("PDF inventario falló: " + response.status);
      return response.blob();
    })
    .then((blob) => {
      // 4. Armar el correo
      const formData = new FormData();
      formData.append("Subject", "Alerta de Inventario: Productos Vencidos - " + window.EmpresaConfig.Nombre);
      formData.append("Body", bodyMessage);
      formData.append("To", emailDestino);
      formData.append("Attachments", blob, "Reporte_Inventario.pdf");

      // 5. Enviar el correo
      return fetch("/api/SendEmail", {
        method: "POST",
        body: formData,
      });
    })
    .then(_parseEmailApiResponse)
    .then(function () {
      alert("Alerta de inventario enviada exitosamente.");
    })
    .catch(function (error) {
      console.error("Error en enviarAlertaInventarioEmail:", error.message);
      alert("Error al enviar la alerta de inventario: " + error.message);
    });
}




function enviarConfirmacionHospitalizacion(pacienteNombre, emailDestino, hospitalizacionId) {
  console.log("Iniciando envío para:", pacienteNombre, "ID:", hospitalizacionId);
  console.log("Email:", emailDestino);
  // 1. Obtener la plantilla
  fetch("/js-utilidades-wp-email/email-plantilla-resultados-examen.html")
    .then(response => {
      if (!response.ok) throw new Error("No se encontró la plantilla de correo");
      return response.text();
    })
    .then(html => {
      // USAR LOS PARÁMETROS QUE RECIBE LA FUNCIÓN
      let bodyMessage = html
        .replaceAll("{{nombrePaciente}}", pacienteNombre)
        .replaceAll("{{NombreEmpresa}}", "sersalud")
        .replaceAll("{{LogoEmpresa}}", "https://i.ibb.co/7d5gLVBC/LOGO-SERSALUD.jpg");

      // 2. Generar el PDF usando el ID recibido
      return fetch(`/CrearPDF/GenerarHospitalizacion/${hospitalizacionId}`) // Verifica que esta ruta sea la correcta en tu C#
        .then(res => {
          if (!res.ok) throw new Error("Error al generar PDF");
          return res.blob();
        })
        .then(blob => {
          const formData = new FormData();
          formData.append("Subject", "Registro de Hospitalización - " + pacienteNombre);
          formData.append("Body", bodyMessage);
          formData.append("To", emailDestino);
          // formData.append("Attachment", blob, "Hospitalizacion.pdf");

          return fetch("/api/SendEmail", { method: "POST", body: formData });
        });
    })
    .then(_parseEmailApiResponse)
    .then(function () {
      if (typeof mensajeEmergente === "function") {
        mensajeEmergente("Correo enviado con éxito");
      }
    })
    .catch(function (err) {
      console.error("Error detallado en SendEmail:", err);
      if (typeof mensajeEmergenteError === "function") {
        mensajeEmergenteError(err.message || "Error al enviar correo");
      }
    });
}