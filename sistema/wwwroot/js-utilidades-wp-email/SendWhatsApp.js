console.log("SendWhatsApp.js v2.0 cargado");

/**
 * Normaliza el número para envío vía backend.
 */
function normalizarNumeroWhatsapp(celularPaciente) {
  if (!celularPaciente) return null;

  let raw = celularPaciente.toString().trim();
  if (!raw) return null;

  raw = raw.replace(/[\s\-\.\(\)]/g, "");
  if (/[a-zA-Z]/.test(raw)) return null;

  if (raw.startsWith("+")) return raw;
  if (raw.startsWith("00")) return "+" + raw.substring(2);

  if (raw === "3249480108") return "+57" + raw;
  return "+502" + raw.replace(/^0+/, "");
}

async function enviarWhatsAppBackend(endpoint, payload) {
  const response = await fetch("/api/WhatsAppMessaging/" + endpoint, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "same-origin",
    body: JSON.stringify(payload),
  });

  const result = await response.json().catch(() => null);
  return { ok: response.ok, result };
}

/**
 * Envía los resultados de laboratorio vía WhatsApp.
 */
async function enviarResultadosLaboratorioWhatsApp(
  examenId,
  nombrePaciente,
  celularPaciente
) {
  try {
    if (!examenId) {
      alert("No se encontró el número de examen.");
      return;
    }

    const celularLimpio = (celularPaciente || "").toString().trim();
    if (!celularLimpio || /[a-zA-Z]/.test(celularLimpio)) {
      alert("El paciente no tiene registrado un número de celular válido.");
      return;
    }

    const telefono = normalizarNumeroWhatsapp(celularLimpio);
    if (!telefono) {
      alert("El número de celular del paciente no es válido.");
      return;
    }

    const pdfUrlExamen = `/CrearPDF/GenerarResultados/${encodeURIComponent(examenId)}`;
    const pdfResponse = await fetch(pdfUrlExamen, { credentials: "same-origin" });
    if (!pdfResponse.ok) {
      alert("Error al generar el PDF de resultados.");
      return;
    }

    const pdfBlob = await pdfResponse.blob();
    const formData = new FormData();
    formData.append("file", pdfBlob, "Resultados_Laboratorio.pdf");

    const saveResponse = await fetch("/api/GuardarPDFWhatsapp", {
      method: "POST",
      credentials: "same-origin",
      body: formData,
    });

    if (!saveResponse.ok) {
      alert("Error al guardar el PDF para WhatsApp.");
      return;
    }

    const saveResult = await saveResponse.json().catch(() => null);
    if (!saveResult || !saveResult.success || !saveResult.url) {
      alert("Error al procesar el PDF para WhatsApp.");
      return;
    }

    const { ok, result } = await enviarWhatsAppBackend("send-template-document", {
      to: telefono,
      documentUrl: saveResult.url,
      templateName: "manantiales_resultadoslab",
      filename: "Resultados_Laboratorio.pdf",
      languageCode: "es",
    });

    if (ok) {
      alert("Mensaje enviado por WhatsApp exitosamente.");
    } else {
      alert(
        (result && result.message) ||
          "No se pudo enviar por WhatsApp. Verifique WhatsAppSettings en appsettings."
      );
    }
  } catch (error) {
    console.error("Error al enviar resultados por WhatsApp:", error);
    alert("Ocurrió un error inesperado al enviar el mensaje por WhatsApp.");
  }
}

async function enviarNotificacionHospitalizacionWhatsApp(
  hospitalizacionId,
  nombrePaciente,
  celularPaciente
) {
  try {
    const celularLimpio = (celularPaciente || "").toString().trim();
    if (!celularLimpio || /[a-zA-Z]/.test(celularLimpio)) {
      alert("El paciente no tiene un número de celular válido registrado.");
      return;
    }

    const telefono = normalizarNumeroWhatsapp(celularLimpio);
    if (!telefono) {
      alert("El número de celular no pudo ser formateado correctamente.");
      return;
    }

    const mensaje =
      "Notificación de hospitalización\nPaciente: " +
      (nombrePaciente || "") +
      "\nNo. admisión: " +
      (hospitalizacionId || "");

    const { ok, result } = await enviarWhatsAppBackend("send-text", {
      to: telefono,
      message: mensaje,
    });

    if (ok) {
      alert("Notificación de hospitalización enviada exitosamente.");
    } else {
      alert(
        (result && result.message) ||
          "Error al enviar la notificación. Verifique WhatsAppSettings."
      );
    }
  } catch (error) {
    console.error("Error inesperado:", error);
    alert("Ocurrió un error inesperado al conectar con WhatsApp.");
  }
}

async function enviarRecordatorioCitaWhatsApp(
  citaId,
  nombrePaciente,
  celularPaciente,
  fechaCita
) {
  if (!celularPaciente) {
    alert("El paciente no tiene un número de celular registrado.");
    return;
  }

  const telefono = normalizarNumeroWhatsapp(celularPaciente);
  if (!telefono) {
    alert("El número de celular del paciente no es válido.");
    return;
  }

  const { ok, result } = await enviarWhatsAppBackend("send-appointment-reminder", {
    to: telefono,
    patientName: nombrePaciente,
    appointmentDate: fechaCita,
  });

  if (ok) {
    alert("Recordatorio enviado por WhatsApp.");
  } else {
    alert(
      (result && result.message) ||
        "No se pudo enviar el recordatorio. Verifique WhatsAppSettings."
    );
  }
}
