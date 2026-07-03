// IDs ya mostrados en esta sesión para no repetir
    var notificacionesMostradas = new Set();

    // =============================================
    // Consultar notificaciones no leídas cada 30s
    // =============================================
    function consultarNotificaciones() {
        $.ajax({
            url: '/SolicitudMedicamento/GetNotificacionesNoLeidas',
            method: 'GET',
            success: function (notifs) {
                if (!notifs || notifs.length === 0) return;

                notifs.forEach(function (n) {
                    if (!notificacionesMostradas.has(n.id)) {
                        notificacionesMostradas.add(n.id);
                        mostrarNotificacion(n);
                    }
                });
            },
            error: function () {
                console.warn('[NOTIF] Error al consultar notificaciones.');
            }
        });
    }

    // =============================================
    // Crear y mostrar la tarjeta de notificación
    // =============================================
    function mostrarNotificacion(n) {
        var hora = n.hora ? '<div class="notif-hora text-muted small">' + n.hora + '</div>' : '';
        var card = $(
            '<div class="notif-card" data-id="' + n.id + '">' +
                '<button class="notif-cerrar" onclick="cerrarNotificacion(' + n.id + ')">✕</button>' +
                '<div class="notif-titulo">' + n.titulo + '</div>' +
                hora +
                '<div class="notif-mensaje">' + n.mensaje + '</div>' +
                '<div class="notif-barra"></div>' +
            '</div>'
        );

        $('#notif-container').append(card);

        // Sonido suave de alerta
        try {
            var ctx = new (window.AudioContext || window.webkitAudioContext)();
            var osc = ctx.createOscillator();
            var gain = ctx.createGain();
            osc.connect(gain);
            gain.connect(ctx.destination);
            osc.frequency.value = 660;
            gain.gain.setValueAtTime(0.3, ctx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.5);
            osc.start(ctx.currentTime);
            osc.stop(ctx.currentTime + 0.5);
        } catch (e) { /* silenciar si el navegador no lo soporta */ }

        // Auto-cerrar después de 15 segundos
        setTimeout(function () {
            cerrarNotificacion(n.id);
        }, 15000);
    }

    // =============================================
    // Cerrar notificación y marcarla como leída
    // =============================================
    function cerrarNotificacion(id) {
        var card = $('.notif-card[data-id="' + id + '"]');
        if (card.length === 0) return;

        card.addClass('notif-saliendo');

        setTimeout(function () {
            card.remove();
        }, 300);

        // Marcar como leída en la BD
        $.ajax({
            url: '/SolicitudMedicamento/MarcarNotificacionLeida',
            method: 'POST',
            data: { id: id }
        });
    }

    // =============================================
    // Iniciar polling al cargar la página
    // =============================================
    $(document).ready(function () {
        // Consulta inmediata al entrar
        consultarNotificaciones();

        // Luego cada 30 segundos
        setInterval(consultarNotificaciones, 30000);
    });