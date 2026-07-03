using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Database.Shared.Models
{
    public class WebAuthnCredential
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string DescriptorId { get; set; } 
        public string PublicKey { get; set; }   
        public uint SignatureCounter { get; set; }
    }

}