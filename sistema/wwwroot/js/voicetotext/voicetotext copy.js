// document.addEventListener('DOMContentLoaded', () => {
//     const apiUrl = '/api/VoiceToText';

//     if (!window.voiceToTextFields || !Array.isArray(window.voiceToTextFields)) {
//         console.error('No se han definido los campos para voz a texto');
//         return;
//     }

//     window.voiceToTextFields.forEach(fieldName => {
//         const field = document.querySelector(`[name="${fieldName}"]`);
//         if (!field) return;

//         const micButton = document.createElement('button');
//         micButton.innerHTML = `<i class="fa fa-microphone" aria-hidden="true"></i>`;
//         micButton.style.marginLeft = '5px';
//         micButton.style.border = 'none';
//         micButton.style.background = '#4CAF50';
//         micButton.style.color = 'white';
//         micButton.style.padding = '5px 10px';
//         micButton.style.borderRadius = '50%';
//         micButton.style.fontSize = '16px';
//         micButton.style.cursor = 'pointer';
//         micButton.style.transition = 'background 0.3s ease';

//         const stopButton = document.createElement('button');
//         stopButton.innerHTML = `<i class="fa fa-stop" aria-hidden="true"></i>`;
//         stopButton.style.marginLeft = '5px';
//         stopButton.style.border = 'none';
//         stopButton.style.background = '#f44336';
//         stopButton.style.color = 'white';
//         stopButton.style.padding = '5px 10px';
//         stopButton.style.borderRadius = '50%';
//         stopButton.style.fontSize = '16px';
//         stopButton.style.cursor = 'not-allowed';
//         stopButton.style.transition = 'background 0.3s ease';
//         stopButton.disabled = true;

//         field.parentElement.appendChild(micButton);
//         field.parentElement.appendChild(stopButton);

//         let mediaRecorder;
//         let audioChunks = [];
//         let blinkInterval;

//         function startBlinking() {
//             let isYellow = false;
//             blinkInterval = setInterval(() => {
//                 micButton.style.background = isYellow ? '#4CAF50' : '#FFEB3B';
//                 isYellow = !isYellow;
//             }, 500);
//         }

//         function stopBlinking() {
//             clearInterval(blinkInterval);
//             micButton.style.background = '#4CAF50';
//         }

//         micButton.addEventListener('click', async () => {
//             if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
//                 const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
//                 mediaRecorder = new MediaRecorder(stream);
//                 mediaRecorder.start();
//                 micButton.disabled = true;
//                 stopButton.disabled = false;
//                 startBlinking();

//                 mediaRecorder.addEventListener('dataavailable', event => {
//                     audioChunks.push(event.data);
//                 });

//                 mediaRecorder.addEventListener('stop', async () => {
//                     stopBlinking();
//                     const audioBlob = new Blob(audioChunks, { type: 'audio/wav' });
//                     audioChunks = [];

//                     const formData = new FormData();
//                     formData.append('audioFile', audioBlob, 'audio.wav');

//                     try {
//                         const response = await fetch(apiUrl, {
//                             method: 'POST',
//                             body: formData
//                         });

//                         if (response.ok) {
//                             const data = await response.text();
//                             field.value = data;
//                         } else {
//                             console.error('Error:', await response.text());
//                         }
//                     } catch (error) {
//                         console.error('Error:', error.message);
//                     }

//                     micButton.disabled = false;
//                     stopButton.disabled = true;
//                 });

//                 setTimeout(() => {
//                     if (mediaRecorder && mediaRecorder.state === 'recording') {
//                         mediaRecorder.stop();
//                     }
//                 }, 300000);
//             } else {
//                 console.error('Tu navegador no soporta grabación de audio.');
//             }
//         });

//         stopButton.addEventListener('click', () => {
//             if (mediaRecorder && mediaRecorder.state === 'recording') {
//                 mediaRecorder.stop();
//             }
//         });
//     });
// });
// document.addEventListener('DOMContentLoaded', () => {

// const DEEPGRAM_API_KEY = '4880d2d625df2ec21e3b1a0b6f2078398b3fadc9';
// let socket;
// let mediaRecorder;
// let audioStream;
// let currentActiveId = null;

// const allowedIds = [
//     'ConsultaMotivo',
//     'HistoriaEnfermedadActual',
//     'HistoriaClinicaImpresionClinica',
//     'HistoriaClinicaComentario',
//     'GinecologiaConsultaMotivo',
//     'HistoriaClinicaAntecedentesMedicos',
//     'HistoriaClinicaAntecedentesQuirurgicos',
//     'HistoriaClinicaAntecedentesTraumaticos',
//     'HistoriaClinicaAntecedentesAlergicos',
//     'HistoriaClinicaAntecedentesVicios',
//     'HistoriaClinicaAntecedentesUsoActualMedicamentos',
//     'Observaciones',
//     'OrdenMedicaDescripcionHospitalizacion',
//     'NotaEvolucionHospitalizacion',
//     'NotaEnfermeriaHospitalizacion',
//     'RevisionApariencia',
//     'RevisionCabeza',
//     'RevisionOidos',
//     'RevisionCuello',
//     'RevisionTorax',
//     'RevisionAbdomen',
//     'RevisionDorso',
//     'RevisionGenitales',
//     'diagnosticoPreOperatorio_NO',
//     'diagnosticoPostOperatorio_NO',
//     'operacionEfectuada_NO',
//     'hallazgos_NO'
// ];

// const styles = `
//     .dictation-controls { display: flex; gap: 10px; margin-bottom: 8px; align-items: center; }
//     .btn-dictation { border: none; padding: 8px 15px; border-radius: 6px; cursor: pointer; font-size: 13px; font-weight: 600; display: flex; align-items: center; gap: 7px; transition: all 0.2s; }
//     .btn-start { background-color: #eef2ff; color: #4f46e5; border: 1px solid #e0e7ff; }
//     .btn-stop { background-color: #fff1f2; color: #e11d48; border: 1px solid #ffe4e6; display: none; }
//     .status-indicator { font-size: 12px; color: #6b7280; font-style: italic; }
//     .recording-pulse { width: 10px; height: 10px; background: #e11d48; border-radius: 50%; animation: pulse 1.5s infinite; }
//     @keyframes pulse { 0% { transform: scale(0.95); opacity: 1; } 70% { transform: scale(1.1); opacity: 0.7; } 100% { transform: scale(0.95); opacity: 1; } }
// `;
// const styleSheet = document.createElement("style");
// styleSheet.innerText = styles;
// document.head.appendChild(styleSheet);

// function initDictation() {
//     allowedIds.forEach(id => {
//         const el = document.getElementById(id);

//         // si es un TEXTAREA
//         if (el && el.tagName.toLowerCase() === 'textarea') {

//             if (document.getElementById(`start-${id}`)) return;

//             const container = document.createElement('div');
//             container.className = 'dictation-controls';
//             container.innerHTML = `
//                 <button type="button" class="btn-dictation btn-start" id="start-${id}">
//                     <i class="fas fa-microphone"></i> Dictar
//                 </button>
//                 <button type="button" class="btn-dictation btn-stop" id="stop-${id}">
//                     <i class="fas fa-stop"></i> Detener
//                 </button>
//                 <span class="status-indicator" id="status-${id}"></span>
//             `;

//             el.parentNode.insertBefore(container, el);

//             document.getElementById(`start-${id}`).onclick = () => startRecording(id);
//             document.getElementById(`stop-${id}`).onclick = () => stopRecording(id);
//         }
//     });
// }

// window.addEventListener('load', initDictation);
// setTimeout(initDictation, 500);

// async function startRecording(id) {
//     currentActiveId = id;
//     const targetTextArea = document.getElementById(id);
//     const sBtn = document.getElementById(`start-${id}`);
//     const tBtn = document.getElementById(`stop-${id}`);
//     const status = document.getElementById(`status-${id}`);

//     try {
//         audioStream = await navigator.mediaDevices.getUserMedia({ audio: true });
//         socket = new WebSocket('wss://api.deepgram.com/v1/listen?model=nova-2&language=es-419&smart_format=true', ['token', DEEPGRAM_API_KEY]);

//         socket.onopen = () => {
//             status.innerHTML = `<div style="display:flex; align-items:center; gap:5px;"><div class="recording-pulse"></div> Grabando...</div>`;
//             sBtn.style.display = 'none';
//             tBtn.style.display = 'flex';
//             if (targetTextArea.value.trim() === "-") targetTextArea.value = "";
//             mediaRecorder = new MediaRecorder(audioStream, { mimeType: 'audio/webm' });
//             mediaRecorder.ondataavailable = (e) => { if (e.data.size > 0 && socket.readyState === WebSocket.OPEN) socket.send(e.data); };
//             mediaRecorder.start(250);
//         };

//         socket.onmessage = (message) => {
//             const received = JSON.parse(message.data);
//             const transcript = received.channel.alternatives[0].transcript;
//             const activeEl = document.getElementById(currentActiveId);
//             if (activeEl && received.is_final && transcript.trim().length > 0) {
//                 let currentText = activeEl.value;
//                 const needsSpace = currentText.length > 0 && !currentText.endsWith(" ");
//                 activeEl.value = currentText + (needsSpace ? " " : "") + transcript;
//                 activeEl.scrollTop = activeEl.scrollHeight;
//             }
//         };

//         socket.onclose = () => stopRecording(currentActiveId);
//         socket.onerror = () => stopRecording(currentActiveId);
//     } catch (err) {
//         alert("Microfono no disponible");
//         stopRecording(id);
//     }
// }

// function stopRecording(id) {
//     const sBtn = document.getElementById(`start-${id}`);
//     const tBtn = document.getElementById(`stop-${id}`);
//     const status = document.getElementById(`status-${id}`);
//     if (status) status.innerText = "";
//     if (sBtn) sBtn.style.display = 'flex';
//     if (tBtn) tBtn.style.display = 'none';
//     if (mediaRecorder && mediaRecorder.state !== 'inactive') mediaRecorder.stop();
//     if (socket && socket.readyState === WebSocket.OPEN) socket.close();
//     if (audioStream) { audioStream.getTracks().forEach(t => t.stop()); audioStream = null; }
// }

// const apiUrl = '/api/VoiceToText';

// if (!window.voiceToTextFields || !Array.isArray(window.voiceToTextFields)) {
//   console.error('No se han definido los campos para voz a texto');
//   return;
// }

// window.voiceToTextFields.forEach(fieldName => {
//   const field = document.querySelector(`[name="${fieldName}"]`);
//   if (!field) return;

//   const micButton = document.createElement('button');
//   micButton.innerHTML = `<i class="fa fa-microphone" aria-hidden="true"></i>`;
//   micButton.style.marginLeft = '5px';
//   micButton.style.border = 'none';
//   micButton.style.background = '#4CAF50';
//   micButton.style.color = 'white';
//   micButton.style.padding = '5px 10px';
//   micButton.style.borderRadius = '50%';
//   micButton.style.fontSize = '16px';
//   micButton.style.cursor = 'pointer';
//   micButton.style.transition = 'background 0.3s ease';

//   const stopButton = document.createElement('button');
//   stopButton.innerHTML = `<i class="fa fa-stop" aria-hidden="true"></i>`;
//   stopButton.style.marginLeft = '5px';
//   stopButton.style.border = 'none';
//   stopButton.style.background = '#f44336';
//   stopButton.style.color = 'white';
//   stopButton.style.padding = '5px 10px';
//   stopButton.style.borderRadius = '50%';
//   stopButton.style.fontSize = '16px';
//   stopButton.style.cursor = 'not-allowed';
//   stopButton.style.transition = 'background 0.3s ease';
//   stopButton.disabled = true;

// field.parentElement.appendChild(micButton);
//   field.parentElement.appendChild(stopButton);

//   let mediaRecorder;
//   let audioChunks = [];
//   let blinkInterval;
//   let stream; // para cerrar el micro al finalizar

//   function startBlinking() {
//     let isYellow = false;
//     blinkInterval = setInterval(() => {
//       micButton.style.background = isYellow ? '#4CAF50' : '#FFEB3B';
//       isYellow = !isYellow;
//     }, 500);
//   }

//   function stopBlinking() {
//     clearInterval(blinkInterval);
//     micButton.style.background = '#4CAF50';
//   }

//   micButton.addEventListener('click', async () => {
//     if (!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)) {
//       console.error('Tu navegador no soporta grabación de audio.');
//       return;
//     }

//     // Preferir opus/webm si el navegador lo soporta
//     const preferredMime = MediaRecorder.isTypeSupported('audio/webm;codecs=opus')
//       ? 'audio/webm;codecs=opus'
//       : (MediaRecorder.isTypeSupported('audio/ogg;codecs=opus') ? 'audio/ogg;codecs=opus' : '');

//     stream = await navigator.mediaDevices.getUserMedia({ audio: true });
//     mediaRecorder = new MediaRecorder(stream, preferredMime ? { mimeType: preferredMime } : undefined);

//     audioChunks = [];
//     mediaRecorder.start();

//     micButton.disabled = true;
//     stopButton.disabled = false;
//     stopButton.style.cursor = 'pointer';
//     startBlinking();

//     mediaRecorder.addEventListener('dataavailable', event => {
//       if (event.data && event.data.size > 0) audioChunks.push(event.data);
//     });

//     mediaRecorder.addEventListener('stop', async () => {
//       stopBlinking();

//       // Cerrar el micro (importante)
//       if (stream) stream.getTracks().forEach(t => t.stop());

//       // Usa el tipo real que grabó el MediaRecorder
//       const blobType = mediaRecorder.mimeType || (audioChunks[0] && audioChunks[0].type) || 'audio/webm';
//       const audioBlob = new Blob(audioChunks, { type: blobType });
//       audioChunks = [];

//       const ext = blobType.includes('webm') ? 'webm' : (blobType.includes('ogg') ? 'ogg' : 'dat');

//       const formData = new FormData();
//       formData.append('audioFile', audioBlob, `audio.${ext}`);

//       try {
//         const response = await fetch(apiUrl, { method: 'POST', body: formData });

//         if (response.ok) {
//           const data = await response.text();
//           field.value = data;
//         } else {
//           console.error('Error:', await response.text());
//         }
//       } catch (error) {
//         console.error('Error:', error.message);
//       }

//       micButton.disabled = false;
//       stopButton.disabled = true;
//       stopButton.style.cursor = 'not-allowed';
//     });

//     setTimeout(() => {
//       if (mediaRecorder && mediaRecorder.state === 'recording') {
//         mediaRecorder.stop();
//       }
//     }, 300000);
//   });

//   stopButton.addEventListener('click', () => {
//     if (mediaRecorder && mediaRecorder.state === 'recording') {
//       mediaRecorder.stop();
//     }
//   });
// });
// });

document.addEventListener("DOMContentLoaded", () => {
  // ADVERTENCIA: Exponer la API KEY en el frontend es altamente inseguro en producción.
  // Se recomienda mover la autenticación a un servidor backend cuando sea posible.
  const DEEPGRAM_API_KEY = "4880d2d625df2ec21e3b1a0b6f2078398b3fadc9";

  let socket;
  let mediaRecorder;
  let audioStream;
  let currentActiveId = null;
  let baseText = ""; // Almacena el texto consolidado/finalizado

  const allowedIds = [
    "ConsultaMotivo",
    "HistoriaEnfermedadActual",
    "HistoriaClinicaImpresionClinica",
    "HistoriaClinicaComentario",
    "GinecologiaConsultaMotivo",
    "HistoriaClinicaAntecedentesMedicos",
    "HistoriaClinicaAntecedentesQuirurgicos",
    "HistoriaClinicaAntecedentesTraumaticos",
    "HistoriaClinicaAntecedentesAlergicos",
    "HistoriaClinicaAntecedentesVicios",
    "HistoriaClinicaAntecedentesUsoActualMedicamentos",
    "Observaciones",
    "OrdenMedicaDescripcionHospitalizacion",
    "NotaEvolucionHospitalizacion",
    "NotaEnfermeriaHospitalizacion",
    "NotaEnfermeriaModal",
    "RevisionApariencia",
    "RevisionCabeza",
    "RevisionOidos",
    "RevisionCuello",
    "RevisionTorax",
    "RevisionAbdomen",
    "RevisionDorso",
    "RevisionGenitales",
    "diagnosticoPreOperatorio_NO",
    "diagnosticoPostOperatorio_NO",
    "operacionEfectuada_NO",
    "hallazgos_NO",
    "diagnosticoPreOperatorio_NO_Edit",
    "diagnosticoPostOperatorio_NO_Edit",
    "operacionEfectuada_NO_Edit",
    "hallazgos_NO_Edit",
    "descripcionGeneral_NO_Edit",
  ];

  // 1. Inyección de Estilos
  const styles = `
        .dictation-controls { display: flex; gap: 10px; margin-bottom: 8px; align-items: center; }
        .btn-dictation { border: none; padding: 8px 15px; border-radius: 6px; cursor: pointer; font-size: 13px; font-weight: 600; display: flex; align-items: center; gap: 7px; transition: all 0.2s; }
        .btn-start { background-color: #eef2ff; color: #4f46e5; border: 1px solid #e0e7ff; }
        .btn-stop { background-color: #fff1f2; color: #e11d48; border: 1px solid #ffe4e6; display: none; }
        .status-indicator { font-size: 12px; color: #6b7280; font-style: italic; }
        .recording-pulse { width: 10px; height: 10px; background: #e11d48; border-radius: 50%; animation: pulse 1.5s infinite; }
        @keyframes pulse { 0% { transform: scale(0.95); opacity: 1; } 70% { transform: scale(1.1); opacity: 0.7; } 100% { transform: scale(0.95); opacity: 1; } }
    `;
  const styleSheet = document.createElement("style");
  styleSheet.innerText = styles;
  document.head.appendChild(styleSheet);

  // 2. Inicialización Robusta (Soporta textareas creados dinámicamente)
  function initDictation() {
    allowedIds.forEach((id) => {
      const el = document.getElementById(id);

      if (el && el.tagName.toLowerCase() === "textarea") {
        if (document.getElementById(`start-${id}`)) return; // Evita duplicar botones

        const container = document.createElement("div");
        container.className = "dictation-controls";
        container.innerHTML = `
                    <button type="button" class="btn-dictation btn-start" id="start-${id}">
                        <i class="fas fa-microphone"></i> Dictar
                    </button>
                    <button type="button" class="btn-dictation btn-stop" id="stop-${id}">
                        <i class="fas fa-stop"></i> Detener
                    </button>
                    <span class="status-indicator" id="status-${id}"></span>
                `;

        el.parentNode.insertBefore(container, el);

        document.getElementById(`start-${id}`).onclick = () =>
          startRecording(id);
        document.getElementById(`stop-${id}`).onclick = () => stopRecording(id);
      }
    });
  }

  // Ejecutar inicialización inicial
  initDictation();

  // Usar MutationObserver reemplaza los setTimeout inseguros.
  // Observa si aparecen nuevos textareas en el DOM posteriormente.
  const observer = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      if (mutation.addedNodes.length) initDictation();
    });
  });
  observer.observe(document.body, { childList: true, subtree: true });

  // 3. Función de Grabación Optimizada (Baja latencia y resultados parciales)
  async function startRecording(id) {
    // Prevenir que se abran dos micrófonos al mismo tiempo
    if (currentActiveId && currentActiveId !== id) {
      stopRecording(currentActiveId);
    }

    currentActiveId = id;
    const targetTextArea = document.getElementById(id);
    const sBtn = document.getElementById(`start-${id}`);
    const tBtn = document.getElementById(`stop-${id}`);
    const status = document.getElementById(`status-${id}`);

    baseText = targetTextArea.value;
    if (baseText.trim() === "-") baseText = "";

    try {
      audioStream = await navigator.mediaDevices.getUserMedia({ audio: true });

      // Endpoint optimizado: interim_results=true y endpointing=300
      const wsUrl =
        "wss://api.deepgram.com/v1/listen?model=nova-2&language=es-419&smart_format=true&interim_results=true&endpointing=300";
      socket = new WebSocket(wsUrl, ["token", DEEPGRAM_API_KEY]);

      socket.onopen = () => {
        status.innerHTML = `<div style="display:flex; align-items:center; gap:5px;"><div class="recording-pulse"></div> Grabando...</div>`;
        sBtn.style.display = "none";
        tBtn.style.display = "flex";
        targetTextArea.value = baseText;

        // Comprobación de compatibilidad de formato de audio
        let options = {};
        if (MediaRecorder.isTypeSupported("audio/webm")) {
          options = { mimeType: "audio/webm" };
        }

        mediaRecorder = new MediaRecorder(audioStream, options);
        mediaRecorder.ondataavailable = (e) => {
          if (e.data.size > 0 && socket.readyState === WebSocket.OPEN) {
            socket.send(e.data);
          }
        };

        // Reducción del timeslice a 100ms para inyectar audio más rápido al socket
        mediaRecorder.start(100);
      };

      // socket.onmessage = (message) => {
      //   const received = JSON.parse(message.data);

      //   if (
      //     !received.channel ||
      //     !received.channel.alternatives ||
      //     received.channel.alternatives.length === 0
      //   )
      //     return;

      //   const transcript = received.channel.alternatives[0].transcript;
      //   const activeEl = document.getElementById(currentActiveId);

      //   if (activeEl && transcript.trim().length > 0) {
      //     const needsSpace =
      //       baseText.length > 0 &&
      //       !baseText.endsWith(" ") &&
      //       !baseText.endsWith("\n")
      //         ? " "
      //         : "";

      //     if (received.is_final) {
      //       // Consolidar el texto finalizado
      //       baseText += needsSpace + transcript;
      //       activeEl.value = baseText;
      //     } else {
      //       // Mostrar visualmente el texto mientras se está hablando (no consolidado)
      //       activeEl.value = baseText + needsSpace + transcript;
      //     }

      //     activeEl.scrollTop = activeEl.scrollHeight;
      //   }
      // };

      socket.onmessage = (message) => {
    const received = JSON.parse(message.data);
    if (!received.channel || !received.channel.alternatives || received.channel.alternatives.length === 0) return;

    const transcript = received.channel.alternatives[0].transcript;
    const activeEl = document.getElementById(currentActiveId);

    if (activeEl && transcript && transcript.trim().length > 0) {
        // Determinar si se necesita un espacio antes de agregar el nuevo texto
        const needsSpace = (baseText.length > 0 && !baseText.endsWith(' ') && !baseText.endsWith('\n')) ? ' ' : '';

        if (received.is_final) {
            // Texto final: se consolida en baseText
            baseText += needsSpace + transcript;
            activeEl.value = baseText;

            // Actualizar directamente el observable de Knockout (si existe)
            if (window.detallesVm && window.detallesVm.notaDiagnostico) {
                window.detallesVm.notaDiagnostico(baseText);
            }
        } else {
            // Texto parcial (interino): se muestra pero no se consolida
            const partialText = baseText + needsSpace + transcript;
            activeEl.value = partialText;

            // También actualizar el observable con el texto parcial (opcional, pero mejora UX)
            if (window.detallesVm && window.detallesVm.notaDiagnostico) {
                window.detallesVm.notaDiagnostico(partialText);
            }
        }

        // Disparar evento input para cualquier otro listener (incluyendo posibles validaciones)
        activeEl.dispatchEvent(new Event('input', { bubbles: true }));

        // Desplazar el scroll al final
        activeEl.scrollTop = activeEl.scrollHeight;
    }
};


      socket.onclose = () => stopRecording(currentActiveId);
      socket.onerror = () => stopRecording(currentActiveId);
    } catch (err) {
      alert(
        "Micrófono no disponible o permisos denegados. Verifique la configuración del navegador.",
      );
      stopRecording(id);
    }
  }

  // 4. Función de Detención
  function stopRecording(id) {
    if (!id) return;
    const sBtn = document.getElementById(`start-${id}`);
    const tBtn = document.getElementById(`stop-${id}`);
    const status = document.getElementById(`status-${id}`);

    if (status) status.innerText = "";
    if (sBtn) sBtn.style.display = "flex";
    if (tBtn) tBtn.style.display = "none";

    if (mediaRecorder && mediaRecorder.state !== "inactive")
      mediaRecorder.stop();
    if (socket && socket.readyState === WebSocket.OPEN) socket.close();
    if (audioStream) {
      audioStream.getTracks().forEach((track) => track.stop());
      audioStream = null;
    }

    if (currentActiveId === id) {
      currentActiveId = null;
    }
  }
});
