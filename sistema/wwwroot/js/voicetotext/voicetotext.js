document.addEventListener("DOMContentLoaded", () => {
  const DEEPGRAM_API_KEY = "4880d2d625df2ec21e3b1a0b6f2078398b3fadc9";

  let socket;
  let mediaRecorder;
  let audioStream;
  let currentActiveId = null;
  let baseText = ""; // Solo para textareas

  const allowedIds = [
    // Textareas existentes
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
    // Editores Quill
    "editor",               // Nota de enfermería
    "editor-nota-medica",   // Nota de evolución médica
    "editor-orden-medica"   // Orden médica
  ];

  // Inyección de estilos
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

  // ─────────────────────────────────────────────────────────────────────────
  // HELPER: Escribe valor en un textarea y notifica a KnockoutJS
  // KO con "textInput" escucha: input + keypress + keyup
  // KO con "value"     escucha: change
  // → disparamos AMBOS para cubrir los dos bindings
  // ─────────────────────────────────────────────────────────────────────────
  // Dentro de voicetotext.js, reemplazar setTextareaValue y la parte de socket.onmessage

  function setTextareaValue(el, text) {
    const nativeSetter = Object.getOwnPropertyDescriptor(window.HTMLTextAreaElement.prototype, "value").set;
    nativeSetter.call(el, text);
    el.dispatchEvent(new Event("input", { bubbles: true }));
    el.dispatchEvent(new Event("change", { bubbles: true }));
    el.dispatchEvent(new Event("blur", { bubbles: true }));

    if (window.detallesVm) {
      const bindAttr = el.getAttribute("data-bind");
      if (bindAttr) {
        const matchTextInput = bindAttr.match(/textInput:\s*(\w+)/);
        const matchValue = bindAttr.match(/value:\s*(\w+)/);
        const observableName = matchTextInput?.[1] || matchValue?.[1];
        if (observableName && ko.isObservable(window.detallesVm[observableName])) {
          window.detallesVm[observableName](text);
          ko.utils.triggerEvent(el, "change");
        }
      }
    }
  }
  // Inicialización: agregar botones a cada elemento permitido
  function initDictation() {
    allowedIds.forEach((id) => {
      const el = document.getElementById(id);
      if (!el) return;
      if (document.getElementById(`start-${id}`)) return; // Evita duplicados

      const isTextarea = el.tagName.toLowerCase() === "textarea";

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

      document.getElementById(`start-${id}`).onclick = () => startRecording(id);
      document.getElementById(`stop-${id}`).onclick = () => stopRecording(id);
    });
  }

  initDictation();

  // Observer para elementos dinámicos (modales)
  // ✅ DESPUÉS — debounce de 300ms para que KO termine de bindear antes de inyectar botones
  let _dictationTimer = null;
  const observer = new MutationObserver(() => {
    clearTimeout(_dictationTimer);
    _dictationTimer = setTimeout(initDictation, 300);
  });
  observer.observe(document.body, { childList: true, subtree: true });

  // Función principal de grabación
  async function startRecording(id) {
    if (currentActiveId && currentActiveId !== id) {
      stopRecording(currentActiveId);
    }

    currentActiveId = id;
    const targetEl = document.getElementById(id);
    const sBtn = document.getElementById(`start-${id}`);
    const tBtn = document.getElementById(`stop-${id}`);
    const status = document.getElementById(`status-${id}`);

    const isTextarea = targetEl && targetEl.tagName.toLowerCase() === "textarea";
    let quill = null;

    if (!isTextarea) {
      quill = Quill.find(targetEl);
      if (!quill) {
        console.error(`No se encontró instancia Quill para el elemento ${id}`);
        stopRecording(id);
        return;
      }
    }

    if (isTextarea) {
      // Tomar el valor ACTUAL del textarea, que puede venir del observable de KO
      baseText = targetEl.value || "";
      if (baseText.trim() === "-") baseText = "";
    }

    try {
      audioStream = await navigator.mediaDevices.getUserMedia({ audio: true });

      const wsUrl = "wss://api.deepgram.com/v1/listen?model=nova-2&language=es-419&smart_format=true&interim_results=true&endpointing=300";
      socket = new WebSocket(wsUrl, ["token", DEEPGRAM_API_KEY]);

      socket.onopen = () => {
        status.innerHTML = `<div style="display:flex; align-items:center; gap:5px;"><div class="recording-pulse"></div> Grabando...</div>`;
        sBtn.style.display = "none";
        tBtn.style.display = "flex";

        if (isTextarea) {
          // Mostrar el texto base sin disparar eventos (solo visual)
          targetEl.value = baseText;
        }

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
        mediaRecorder.start(100);
      };

      socket.onmessage = (message) => {
        const received = JSON.parse(message.data);
        if (!received.channel?.alternatives?.length) return;

        const transcript = received.channel.alternatives[0].transcript;
        const activeEl = document.getElementById(currentActiveId);
        if (!activeEl) return;

        if (transcript && transcript.trim().length > 0) {
          const isTextarea = activeEl.tagName.toLowerCase() === "textarea";

          if (isTextarea) {
            const needsSpace = (baseText.length && !baseText.endsWith(" ") && !baseText.endsWith("\n")) ? " " : "";

            if (received.is_final) {
              baseText += needsSpace + transcript;
            }

            const textoFinal = received.is_final ? baseText : (baseText + needsSpace + transcript);

            // Actualizar DOM
            activeEl.value = textoFinal;
            // Disparar eventos para que Knockout reactive el binding 'value'
            activeEl.dispatchEvent(new Event("input", { bubbles: true }));
            activeEl.dispatchEvent(new Event("change", { bubbles: true }));

            // Actualizar observable de Knockout si existe (value o textInput)
            const context = ko.contextFor(activeEl);
            if (context && context.$data) {
              const bindAttr = activeEl.getAttribute('data-bind');
              if (bindAttr) {
                // Buscar tanto 'textInput:' como 'value:'
                const matchTextInput = bindAttr.match(/textInput:\s*(\w+)/);
                const matchValue = bindAttr.match(/value:\s*(\w+)/);
                const observableName = matchTextInput?.[1] || matchValue?.[1];
                if (observableName && typeof context.$rawData[observableName] === 'function') {
                  context.$rawData[observableName](textoFinal);
                }
              }
            }

            activeEl.scrollTop = activeEl.scrollHeight;

          } else if (quill && received.is_final) {
            // Lógica para Quill
            const selection = quill.getSelection();
            let index = selection ? selection.index : quill.getLength();
            quill.insertText(index, transcript + " ");
            quill.setSelection(index + transcript.length + 1);
            quill.root.dispatchEvent(new Event("input", { bubbles: true }));
            quill.root.dispatchEvent(new Event("change", { bubbles: true }));
          }
        }
      };
      socket.onclose = () => stopRecording(currentActiveId);
      socket.onerror = () => stopRecording(currentActiveId);

    } catch (err) {
      alert("Micrófono no disponible o permisos denegados. Verifique la configuración del navegador.");
      stopRecording(id);
    }
  }

  function stopRecording(id) {
    if (!id) return;
    const sBtn = document.getElementById(`start-${id}`);
    const tBtn = document.getElementById(`stop-${id}`);
    const status = document.getElementById(`status-${id}`);

    if (status) status.innerText = "";
    if (sBtn) sBtn.style.display = "flex";
    if (tBtn) tBtn.style.display = "none";

    if (mediaRecorder && mediaRecorder.state !== "inactive") mediaRecorder.stop();
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