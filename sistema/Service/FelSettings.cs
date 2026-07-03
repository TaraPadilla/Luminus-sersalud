namespace farmamest
{
    public class FelSettings
    {
        public string BaseUrl { get; set; }
        public string UsuarioFirma { get; set; }
        public string LlaveFirma { get; set; }
        public string UsuarioApi { get; set; }
        public string LlaveApi { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
    }
}