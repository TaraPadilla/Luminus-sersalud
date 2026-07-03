using System;

namespace Database.Shared.Models
{
    public class WebAuthnRegistrationToken
    {
        public int Id { get; set; } 
        
        // ID del usuario al que pertenece este código (ej. el empleado)
        public string UserId { get; set; } 
        
        // El código único y aleatorio que generaremos y pondremos en el QR
        public string Token { get; set; } 
        
        // Fecha y hora en la que se generó
        public DateTime CreatedAt { get; set; } 
        
        // Fecha y hora límite (ej. 15 minutos después de crearlo)
        public DateTime ExpiresAt { get; set; } 
        
        // Bandera para saber si ya se usó y evitar que un mismo QR sirva dos veces
        public bool IsUsed { get; set; } 

        // Opcional: Si tienes una relación directa con tu modelo de Identity User
        // public virtual User User { get; set; }
    }
}