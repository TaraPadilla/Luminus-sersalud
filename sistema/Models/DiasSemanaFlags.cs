using System;

namespace sistema.Models
{
    [Flags]
    public enum DiasSemanaFlags
    {
        Ninguno   = 0,
        Lunes     = 1 << 0, // 1
        Martes    = 1 << 1, // 2
        Miercoles = 1 << 2, // 4
        Jueves    = 1 << 3, // 8
        Viernes   = 1 << 4, // 16
        Sabado    = 1 << 5, // 32
        Domingo   = 1 << 6  // 64
    }
}
