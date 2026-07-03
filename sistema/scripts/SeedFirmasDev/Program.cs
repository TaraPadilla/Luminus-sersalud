using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Npgsql;

// Dev-only helper: creates sample signature PNGs and backfills missing DB paths.
// Run: dotnet run --project Scripts/SeedFirmasDev/SeedFirmasDev.csproj

var contentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
var firmasDir = Path.Combine(contentRoot, "wwwroot", "Firmas");
Directory.CreateDirectory(firmasDir);

string CreateSig(string name, Color ink, string label)
{
    var file = Path.Combine(firmasDir, name + ".png");
    using var bmp = new Bitmap(320, 100);
    using var g = Graphics.FromImage(bmp);
    g.SmoothingMode = SmoothingMode.AntiAlias;
    g.Clear(Color.White);
    using var pen = new Pen(ink, 2.5f);
    g.DrawBezier(pen, 10, 55, 40, 15, 80, 85, 120, 45);
    g.DrawBezier(pen, 120, 45, 160, 10, 200, 80, 250, 40);
    g.DrawBezier(pen, 250, 40, 270, 30, 290, 50, 305, 42);
    using var font = new Font("Segoe Script", 16f, FontStyle.Italic);
    using var brush = new SolidBrush(Color.FromArgb(30, 30, 30));
    g.DrawString(label, font, brush, 12f, 62f);
    bmp.Save(file, ImageFormat.Png);
    return "/Firmas/" + name + ".png";
}

var paths = new Dictionary<string, string>
{
    ["medico"] = CreateSig("firma_medico_tratante", Color.FromArgb(30, 64, 175), "Dr. Medico Tratante"),
    ["autorizador"] = CreateSig("firma_autorizador", Color.FromArgb(15, 118, 110), "Autorizador Clinico"),
    ["enfermeria"] = CreateSig("firma_enfermeria", Color.FromArgb(180, 83, 9), "Enfermeria"),
    ["paciente"] = CreateSig("firma_paciente", Color.FromArgb(127, 29, 29), "Paciente"),
    ["responsable"] = CreateSig("firma_responsable", Color.FromArgb(88, 28, 135), "Responsable"),
};

var connStr = Environment.GetEnvironmentVariable("FARMAOWL_CONNECTION")
    ?? "Host=localhost;Port=5432;Database=sersalud;Username=postgres;Password=postgres;SSL Mode=Prefer";

await using var conn = new NpgsqlConnection(connStr);
await conn.OpenAsync();

async Task<int> Run(string sql, string path)
{
    await using var cmd = new NpgsqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("p", path);
    return await cmd.ExecuteNonQueryAsync();
}

Console.WriteLine("Empleados sin firma -> " + await Run(
    @"UPDATE ""Empleados"" SET ""FirmaEmpleado"" = @p WHERE ""FirmaEmpleado"" IS NULL OR TRIM(""FirmaEmpleado"") = ''",
    paths["autorizador"]));

Console.WriteLine("Consentimiento paciente -> " + await Run(
    @"UPDATE ""ConsentimientoHospi"" SET ""URLFirmaPaciente"" = @p WHERE ""URLFirmaPaciente"" IS NULL OR TRIM(""URLFirmaPaciente"") = ''",
    paths["paciente"]));

Console.WriteLine("Consentimiento responsable -> " + await Run(
    @"UPDATE ""ConsentimientoHospi"" SET ""URLFirmaResponsable"" = @p WHERE ""URLFirmaResponsable"" IS NULL OR TRIM(""URLFirmaResponsable"") = ''",
    paths["responsable"]));

Console.WriteLine("NotaOperatoria sin ruta -> " + await Run(
    @"UPDATE ""NotaOperatoria"" SET ""FirmaRuta"" = @p WHERE ""FirmaRuta"" IS NULL OR TRIM(""FirmaRuta"") = ''",
    paths["medico"]));

try
{
    Console.WriteLine("NotaEnfermeria2 firmadas sin ruta -> " + await Run(
        @"UPDATE ""NotaEnfermeria2"" SET ""FirmaRuta"" = @p WHERE ""Firmado"" = true AND (""FirmaRuta"" IS NULL OR TRIM(""FirmaRuta"") = '')",
        paths["enfermeria"]));
}
catch (Exception ex)
{
    Console.WriteLine("NotaEnfermeria2 skip: " + ex.Message);
}

Console.WriteLine("Done. Files in " + firmasDir);
