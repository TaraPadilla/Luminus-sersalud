async function enviarReporteCompraWhatsapp(compraId) {
  try {
    var celularProveedor = document.getElementById("celularProveedor-" + compraId).value;

    if (!celularProveedor) {
      alert("El proveedor no tiene un número de celular registrado.");
      return;
    }

    const telefono = normalizarNumeroWhatsapp(celularProveedor);
    if (!telefono) {
      alert("El número de celular del proveedor no es válido.");
      return;
    }

    const pdfResponse = await fetch(`/Compra/Reporte?CompraId=${compraId}`, {
      credentials: "same-origin",
    });
    if (!pdfResponse.ok) {
      alert("Error al generar el PDF.");
      return;
    }
    const pdfBlob = await pdfResponse.blob();

    const formData = new FormData();
    formData.append("file", pdfBlob, "Orden_Compra.pdf");

    const saveResponse = await fetch("/api/GuardarPDFWhatsapp", {
      method: "POST",
      credentials: "same-origin",
      body: formData,
    });
    const saveResult = await saveResponse.json().catch(() => null);

    if (!saveResponse.ok || !saveResult || !saveResult.success || !saveResult.url) {
      alert("Error al guardar el PDF.");
      return;
    }

    const { ok, result } = await enviarWhatsAppBackend("send-template-document", {
      to: telefono,
      documentUrl: saveResult.url,
      templateName: "manantiales_compras",
      filename: "Orden_Compra.pdf",
      languageCode: "es",
    });

    if (ok) {
      alert("Mensaje enviado por WhatsApp exitosamente.");
    } else {
      alert(
        (result && result.message) ||
          "Error al enviar el mensaje por WhatsApp. Verifique WhatsAppSettings."
      );
    }
  } catch (error) {
    console.error("Error enviando reporte de compra por WhatsApp:", error);
    alert("Ocurrió un error inesperado al enviar por WhatsApp.");
  }
}
