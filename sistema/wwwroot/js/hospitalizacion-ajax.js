/**
 * Helpers compartidos para peticiones AJAX en Hospitalización / Orden Médica.
 * Normaliza respuestas JSON (string u objeto, PascalCase/camelCase).
 */
(function (window) {
    'use strict';

    function parseHospJson(response) {
        if (response == null) return null;
        if (typeof response === 'string') {
            var trimmed = response.trim();
            if (!trimmed) return null;
            if (trimmed.charAt(0) === '<') {
                console.error('Respuesta HTML en lugar de JSON (¿sesión expirada?)');
                return null;
            }
            try {
                return JSON.parse(trimmed);
            } catch (e) {
                console.error('JSON inválido:', e, trimmed.substring(0, 200));
                return null;
            }
        }
        return response;
    }

    function hospExitoso(data) {
        return !!(data && (data.Exitoso === true || data.exitoso === true));
    }

    function hospResultado(data) {
        if (!data) return [];
        var r = data.Resultado !== undefined ? data.Resultado : data.resultado;
        return Array.isArray(r) ? r : [];
    }

    function hospMensaje(data, fallback) {
        if (!data) return fallback || 'Error desconocido';
        return data.Mensaje || data.mensaje || fallback || 'Error desconocido';
    }

    function ensureArray(value) {
        return Array.isArray(value) ? value : [];
    }

    function hospProp(obj) {
        if (!obj) return undefined;
        for (var i = 1; i < arguments.length; i++) {
            var key = arguments[i];
            if (obj[key] !== undefined && obj[key] !== null) return obj[key];
        }
        return undefined;
    }

    function getHospitalizacionId() {
        var $ = window.jQuery;
        if ($) {
            var id = parseInt($('#HospitalizacionId').val(), 10);
            if (id > 0) return id;
        }
        if (typeof hospitalizacionIdGlobal !== 'undefined') {
            id = parseInt(hospitalizacionIdGlobal, 10);
            if (id > 0) return id;
        }
        return 0;
    }

    function ensureDataTables(callback) {
        var $ = window.jQuery;
        if ($ && typeof $.fn.DataTable === 'function') {
            callback();
            return;
        }
        var existing = document.querySelector('script[src*="jquery.dataTables"]');
        if (existing) {
            existing.addEventListener('load', callback, { once: true });
            return;
        }
        if (!document.querySelector('link[href*="jquery.dataTables"]')) {
            var css = document.createElement('link');
            css.rel = 'stylesheet';
            css.href = 'https://cdn.datatables.net/1.13.4/css/jquery.dataTables.min.css';
            document.head.appendChild(css);
        }
        var script = document.createElement('script');
        script.src = 'https://cdn.datatables.net/1.13.4/js/jquery.dataTables.min.js';
        script.onload = callback;
        document.body.appendChild(script);
    }

    function initDataTableSafe(selector, options) {
        ensureDataTables(function () {
            var $ = window.jQuery;
            if (!$ || typeof $.fn.DataTable !== 'function') return;
            var $tabla = $(selector);
            if (!$tabla.length) return;
            if ($.fn.DataTable.isDataTable($tabla)) {
                $tabla.DataTable().destroy();
            }
            $tabla.DataTable(options || {});
        });
    }

    window.HospAjax = {
        parseHospJson: parseHospJson,
        hospExitoso: hospExitoso,
        hospResultado: hospResultado,
        hospMensaje: hospMensaje,
        ensureArray: ensureArray,
        hospProp: hospProp,
        getHospitalizacionId: getHospitalizacionId,
        ensureDataTables: ensureDataTables,
        initDataTableSafe: initDataTableSafe
    };
})(window);
