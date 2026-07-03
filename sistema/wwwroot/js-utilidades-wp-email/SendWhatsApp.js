// Marca de versión para comprobar que se está cargando el archivo correcto
console.log("SendWhatsApp.js v1.3 cargado");

// Obtiene el token de acceso para la API de WhatsApp
async function getWhatsAppToken() {
  try {
    const response = await fetch("/api/whatsapptoken");

    if (!response.ok) {
      console.error(
        "Error al obtener el token de WhatsApp. Status:",
        response.status
      );
      return null;
    }

    const token = await response.text();
    if (!token) {
      console.error("Token de WhatsApp vacío o nulo.");
      return null;
    }

    return token;
  } catch (error) {
    console.error("Excepción al obtener el token de WhatsApp:", error);
    return null;
  }
}

/**
 * Normaliza el número para enviarlo a la API de WhatsApp.
 * Reglas:
 * - Si empieza con '+', se asume internacional:
 *      - Se eliminan espacios, guiones, puntos y paréntesis.
 *      - Se quita el '+' y se envían solo dígitos.
 * - Si NO empieza con '+', se asume número local:
 *      - Se eliminan espacios, guiones, puntos y paréntesis.
 *      - Se aplica la lógica actual (502 por defecto, caso especial 3249480108).
 *
 * IMPORTANTE: WhatsApp Cloud API espera el número en formato internacional
 *             SIN el '+' (solo dígitos).
 */
function normalizarNumeroWhatsapp(celularPaciente) {
  if (!celularPaciente) {
    return null;
  }

  let raw = celularPaciente.toString().trim();
  if (!raw) {
    return null;
  }

  // Eliminar espacios, guiones, puntos y paréntesis (dejamos el '+' si existe)
  raw = raw.replace(/[\s\-\.\(\)]/g, "");

  // Si contiene letras, claramente no es un número
  if (/[a-zA-Z]/.test(raw)) {
    console.error(
      "normalizarNumeroWhatsapp: el valor contiene letras, probablemente no es un número:",
      raw
    );
    return null;
  }

  // Caso: ya viene en formato internacional, ej. +584120122517
  if (raw.startsWith("+")) {
    const soloDigitos = raw.replace(/\D/g, ""); // quitar '+'
    return soloDigitos || null;
  }

  // Desde aquí asumimos número "local"
  const soloDigitos = raw.replace(/\D/g, "");
  if (!soloDigitos) {
    return null;
  }

  if (soloDigitos === "3249480108") {
    return "57" + soloDigitos;
  }

  return "502" + soloDigitos;
}

/**
 * Envía los resultados de laboratorio vía WhatsApp.
 * @param {string} examenId        Número/ID del examen.
 * @param {string} nombrePaciente  Nombre del paciente.
 * @param {string} celularPaciente Celular del paciente.
 */
async function enviarResultadosLaboratorioWhatsApp(
  examenId,
  nombrePaciente,
  celularPaciente
) {
  try {
    console.log("Llamada a enviarResultadosLaboratorioWhatsApp (v1.3):", {
      examenId,
      nombrePaciente,
      celularPaciente,
    });

    if (!examenId) {
      alert("No se encontró el número de examen.");
      console.error("ExamenId inválido:", examenId);
      return;
    }

    const celularLimpio = (celularPaciente || "").toString().trim();
    if (!celularLimpio) {
      alert("El paciente no tiene registrado un número de celular.");
      console.warn("Celular del paciente vacío o nulo para examen:", examenId);
      return;
    }

    // Chequeo extra: si el "celular" trae letras, probablemente estamos recibiendo el nombre
    if (/[a-zA-Z]/.test(celularLimpio)) {
      console.error(
        "El parámetro celularPaciente parece ser un nombre, no un número:",
        celularLimpio
      );
      alert(
        "Error interno: el número de celular del paciente no es válido (parece un nombre)."
      );
      return;
    }

    const token = await getWhatsAppToken();
    if (!token) {
      alert(
        "No fue posible obtener el token de WhatsApp. Intente nuevamente más tarde."
      );
      return;
    }

    const formattedPhoneNumber = normalizarNumeroWhatsapp(celularLimpio);
    if (!formattedPhoneNumber) {
      alert("El número de celular del paciente no es válido.");
      console.error(
        "No se pudo normalizar el número de celular:",
        celularPaciente
      );
      return;
    }

    console.log("Número de WhatsApp formateado:", formattedPhoneNumber);

    // 1) Generar PDF de resultados
    const pdfUrlExamen = `/CrearPDF/GenerarResultados/${encodeURIComponent(
      examenId
    )}`;
    const pdfResponse = await fetch(pdfUrlExamen);

    if (!pdfResponse.ok) {
      const errorText = await pdfResponse.text().catch(() => "");
      console.error(
        "Error al generar el PDF. Status:",
        pdfResponse.status,
        "Detalle:",
        errorText
      );
      alert("Error al generar el PDF de resultados.");
      return;
    }

    const pdfBlob = await pdfResponse.blob();

    // 2) Enviar el PDF al backend para guardarlo y obtener una URL pública
    const formData = new FormData();
    formData.append("file", pdfBlob, "Resultados_Laboratorio.pdf");

    console.log("Enviando PDF al servidor:", formData);

    const saveResponse = await fetch("/api/GuardarPDFWhatsapp", {
      method: "POST",
      body: formData,
    });

    if (!saveResponse.ok) {
      const errorText = await saveResponse.text().catch(() => "");
      console.error(
        "Error al guardar el PDF. Status:",
        saveResponse.status,
        "Detalle:",
        errorText
      );
      alert("Error al guardar el PDF para WhatsApp.");
      return;
    }

    const saveResult = await saveResponse.json().catch((e) => {
      console.error("Error al parsear respuesta de GuardarPDFWhatsapp:", e);
      return null;
    });

    console.log("Respuesta del servidor al guardar PDF:", saveResult);

    if (!saveResult || !saveResult.success || !saveResult.url) {
      console.error("Respuesta inválida de GuardarPDFWhatsapp:", saveResult);
      alert("Error al procesar el PDF para WhatsApp.");
      return;
    }

    const pdfUrl = saveResult.url;
    console.log("PDF guardado para WhatsApp en URL:", pdfUrl);

    // 3) Construir el cuerpo del mensaje de WhatsApp
    const messageBody = {
      messaging_product: "whatsapp",
      to: formattedPhoneNumber,
      type: "template",
      template: {
        name: "manantiales_resultadoslab",
        language: {
          code: "es",
        },
        components: [
          {
            type: "header",
            parameters: [
              {
                type: "document",
                document: {
                  link: pdfUrl,
                  filename: "Resultados_Laboratorio.pdf",
                },
              },
            ],
          },
        ],
      },
    };

    console.log("Enviando mensaje por WhatsApp:", messageBody);

    const whatsappResponse = await fetch(
      "https://graph.facebook.com/v20.0/407063625823063/messages",
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(messageBody),
      }
    );

    const whatsappResult = await whatsappResponse.json().catch((e) => {
      console.error("Error al parsear respuesta de WhatsApp:", e);
      return null;
    });

    console.log(
      "Respuesta del servidor al enviar mensaje por WhatsApp:",
      whatsappResult
    );

    // Validación más estricta de la respuesta
    if (!whatsappResponse.ok || !whatsappResult) {
      console.error(
        "Error HTTP al enviar mensaje por WhatsApp. Status:",
        whatsappResponse.status,
        "Respuesta:",
        whatsappResult
      );
      alert("Error al enviar el mensaje por WhatsApp.");
      return;
    }

    if (
      Array.isArray(whatsappResult.messages) &&
      whatsappResult.messages.length > 0
    ) {
      // Éxito real a nivel de API (mensaje creado)
      alert("Mensaje enviado por WhatsApp exitosamente.");
    } else if (whatsappResult.error) {
      console.error(
        "Error lógico en respuesta de WhatsApp:",
        whatsappResult.error
      );
      alert("Error al enviar el mensaje por WhatsApp.");
    } else {
      console.warn(
        "Respuesta de WhatsApp sin messages ni error claro:",
        whatsappResult
      );
      alert("Se recibió una respuesta inesperada de WhatsApp. Revisa la consola.");
    }
  } catch (error) {
    console.error("Error inesperado al enviar resultados por WhatsApp:", error);
    alert("Ocurrió un error inesperado al enviar el mensaje por WhatsApp.");
  }
}
/**
 * Envía una notificación de hospitalización vía WhatsApp (Solo texto/plantilla).
 * @param {string} hospitalizacionId Número o ID de la hospitalización.
 * @param {string} nombrePaciente    Nombre del paciente.
 * @param {string} celularPaciente   Celular del paciente.
 */
async function enviarNotificacionHospitalizacionWhatsApp(hospitalizacionId, nombrePaciente, celularPaciente) {
    try {
        console.log("Iniciando envío de notificación de hospitalización:", { hospitalizacionId, nombrePaciente, celularPaciente });

        // 1. Validaciones básicas
        const celularLimpio = (celularPaciente || "").toString().trim();
        if (!celularLimpio || /[a-zA-Z]/.test(celularLimpio)) {
            alert("El paciente no tiene un número de celular válido registrado.");
            return;
        }

        // 2. Obtener Token y normalizar número (usando tus funciones existentes)
        const token = await getWhatsAppToken();
        if (!token) {
            alert("No fue posible obtener el token de WhatsApp. Intente nuevamente.");
            return;
        }

        const formattedPhoneNumber = normalizarNumeroWhatsapp(celularLimpio);
        if (!formattedPhoneNumber) {
            alert("El número de celular no pudo ser formateado correctamente.");
            return;
        }

        // 3. Construir el cuerpo del mensaje de WhatsApp (Sin PDF)
        const messageBody = {
            messaging_product: "whatsapp",
            to: formattedPhoneNumber,
            type: "template",
            template: {
                // AQUÍ DEBES PONER EL NOMBRE DE TU PLANTILLA APROBADA EN META PARA HOSPITALIZACIÓN
                name: "manantiales_resultadoslab", 
                language: {
                    code: "es"
                },
                // Si tu plantilla tiene variables (ej. {{1}} para el nombre), se envían así. 
                // Si no tiene variables, puedes borrar la sección "components".
                components: [
                    {
                        type: "body",
                        parameters: [
                            {
                                type: "text",
                                text: nombrePaciente
                            },
                            {
                                type: "text",
                                text: hospitalizacionId
                            }
                        ]
                    }
                ]
            }
        };

        // 4. Enviar la petición a la API de Meta
        // Nota: Asegúrate de que el ID del teléfono (407063625823063) sea el correcto
        const whatsappResponse = await fetch("https://graph.facebook.com/v20.0/407063625823063/messages", {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify(messageBody)
        });

        const whatsappResult = await whatsappResponse.json();

        // 5. Manejo de la respuesta
        if (whatsappResponse.ok) {
            alert("Notificación de hospitalización enviada exitosamente.");
        } else {
            console.error("Error de WhatsApp:", whatsappResult);
            alert("Error al enviar la notificación. Revisa la consola para más detalles.");
        }

    } catch (error) {
        console.error("Error inesperado:", error);
        alert("Ocurrió un error inesperado al conectar con WhatsApp.");
    }
}