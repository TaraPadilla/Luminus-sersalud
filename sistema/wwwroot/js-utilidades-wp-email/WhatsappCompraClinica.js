async function getWhatsAppToken() {
    try {
        const response = await fetch('/api/whatsapptoken');
        if (response.ok) {
            const token = await response.text();
            return token; // Devuelve el token
        } else {
            console.error('Error al obtener el token de WhatsApp');
            return null;
        }
    } catch (error) {
        console.error('Error al obtener el token de WhatsApp:', error);
        return null;
    }
}

async function enviarReporteCompraWhatsapp(compraId) {
    const token = await getWhatsAppToken(); // Esperar a que se obtenga el token
    var celularProveedor = document.getElementById('celularProveedor-' + compraId).value;

    if (!celularProveedor) {
        alert('El proveedor no tiene un número de celular registrado.');
        return;
    }

    var formattedPhoneNumber = `502${celularProveedor}`;

    if (celularProveedor == "3249480108") {
        var formattedPhoneNumber = `57${celularProveedor}`;

    }
    // Generar el PDF
    const pdfResponse = await fetch(`/Compra/Reporte?CompraId=${compraId}`);
    if (!pdfResponse.ok) {
        console.error('Error al generar el PDF.', await pdfResponse.text());
        alert('Error al generar el PDF.');
        return;
    }
    const pdfBlob = await pdfResponse.blob();

    // Guardar el PDF en el servidor
    const formData = new FormData();
    formData.append('file', pdfBlob, 'Orden_Compra.pdf');

    console.log('Enviando PDF al servidor:', formData);

    const saveResponse = await fetch('/api/GuardarPDFWhatsapp', {
        method: 'POST',
        body: formData
    });
    const saveResult = await saveResponse.json();

    console.log('Respuesta del servidor al guardar PDF:', saveResult);

    if (!saveResponse.ok || !saveResult.success) {
        alert('Error al guardar el PDF.');
        return;
    }

    const pdfUrl = saveResult.url;

    // Enviar por WhatsApp
    const messageBody = {
        messaging_product: 'whatsapp',
        to: formattedPhoneNumber,
        type: 'template',
        template: {
            name: 'manantiales_compras', // Cambia la plantilla a usar
            language: { code: 'es' },
            components: [
                {
                    type: 'header',
                    parameters: [
                        {
                            type: 'document',
                            document: {
                                link: pdfUrl,
                                //link: 'http://3.12.154.4/pdf/resultados/Resultados_Laboratorio.pdf', //Utilizado para pruebas
                                filename: 'Orden_Compra.pdf'
                            }
                        }
                    ]
                }
            ]
        }
    };

    console.log('Enviando mensaje por WhatsApp:', messageBody);

    const whatsappResponse = await fetch('https://graph.facebook.com/v20.0/407063625823063/messages', {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(messageBody)
    });

    const whatsappResult = await whatsappResponse.json();

    console.log('Respuesta del servidor al enviar mensaje por WhatsApp:', whatsappResult);

    if (whatsappResponse.ok) {
        alert('Mensaje enviado por WhatsApp exitosamente.');
    } else {
        alert('Error al enviar el mensaje por WhatsApp.');
    }
}
