export async function confirmarConHuella(actionLabel = '') {
  // ── Paso 1: Pedir el challenge al servidor ──────────────────────
  const label = encodeURIComponent(actionLabel);
  const beginRes = await fetch(`/api/WebAuthnVerify/Begin?actionLabel=${label}`, {
    method: 'POST'
  });

  if (!beginRes.ok) {
    const err = await beginRes.json().catch(() => ({}));
    return {
      ok: false,
      mensaje: err.message || 'No se pudo iniciar la verificación de huella.',
      errorCode: err.errorCode || 'InternalError'
    };
  }

  const options = await beginRes.json();

  // ── Paso 2: Construir opciones para el dispositivo ──────────────
  const publicKeyOptions = {
    challenge: base64UrlToBuffer(options.challenge),
    timeout: options.timeout ?? 60000,
    rpId: options.rpId,
    userVerification: options.userVerification ?? 'required',
    allowCredentials: (options.allowCredentials ?? []).map(c => ({
      type: 'public-key',
      id: base64UrlToBuffer(c.id),
      ...(c.transports ? { transports: c.transports } : {})
    }))
  };

  // ── Paso 3: Activar la huella en el dispositivo ─────────────────
  let assertion;
  try {
    assertion = await navigator.credentials.get({ publicKey: publicKeyOptions });
  } catch (e) {
    if (e.name === 'NotAllowedError') {
      return { ok: false, mensaje: 'Verificación cancelada.', errorCode: 'UserCancelled' };
    }
    return { ok: false, mensaje: 'El dispositivo no pudo leer la huella.', errorCode: 'DeviceError' };
  }

  // ── Paso 4: Serializar respuesta del dispositivo ────────────────
  // Este payload se incluye en el request del módulo junto con los datos del formulario
  const payload = {
    id: assertion.id,
    rawId: bufferToBase64Url(assertion.rawId),
    type: assertion.type,
    response: {
      authenticatorData: bufferToBase64Url(assertion.response.authenticatorData),
      clientDataJSON:    bufferToBase64Url(assertion.response.clientDataJSON),
      signature:         bufferToBase64Url(assertion.response.signature),
      userHandle: assertion.response.userHandle
        ? bufferToBase64Url(assertion.response.userHandle)
        : null
    }
  };

  return { ok: true, payload };
}

// ── Conversiones base64 ─────────────────────────────────────────

function base64UrlToBuffer(b64) {
  const std = b64.replace(/-/g, '+').replace(/_/g, '/');
  const padded = std.padEnd(std.length + (4 - std.length % 4) % 4, '=');
  const binary = atob(padded);
  const buf = new Uint8Array(binary.length);
  for (let i = 0; i < binary.length; i++) buf[i] = binary.charCodeAt(i);
  return buf.buffer;
}

function bufferToBase64Url(buf) {
  const bytes = new Uint8Array(buf);
  let bin = '';
  for (const b of bytes) bin += String.fromCharCode(b);
  return btoa(bin).replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
}
