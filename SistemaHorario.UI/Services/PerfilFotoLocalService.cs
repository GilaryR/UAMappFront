using System;
using System.IO;
using System.Linq;

namespace SistemaHorario.UI.Services
{
    public class PerfilFotoLocalService
    {
        private const string NombreCarpeta = "SistemaHorarioUAM";
        private const string NombreSubcarpeta = "FotosPerfil";

        private readonly string[] _extensionesPermitidas =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp"
        };

        public string ObtenerRutaFoto(string correoInstitucional)
        {
            string rutaCarpeta = ObtenerCarpetaFotos();

            if (!Directory.Exists(rutaCarpeta))
            {
                return "/Assets/Images/ImgUsuario.png";
            }

            string nombreBase =
                ObtenerNombreArchivoSeguro(correoInstitucional);

            string? fotoEncontrada = Directory
                .GetFiles(rutaCarpeta)
                .FirstOrDefault(archivo =>
                    Path.GetFileNameWithoutExtension(archivo) == nombreBase &&
                    _extensionesPermitidas.Contains(
                        Path.GetExtension(archivo).ToLower()));

            return fotoEncontrada ?? "/Assets/Images/ImgUsuario.png";
        }

        public string GuardarFoto(
            string rutaOrigen,
            string correoInstitucional)
        {
            if (!File.Exists(rutaOrigen))
            {
                throw new FileNotFoundException(
                    "La imagen seleccionada no existe.",
                    rutaOrigen);
            }

            string extension =
                Path.GetExtension(rutaOrigen).ToLower();

            if (!_extensionesPermitidas.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Formato de imagen no permitido. Use JPG, JPEG, PNG o BMP.");
            }

            string rutaCarpeta = ObtenerCarpetaFotos();

            if (!Directory.Exists(rutaCarpeta))
            {
                Directory.CreateDirectory(rutaCarpeta);
            }

            string nombreBase =
                ObtenerNombreArchivoSeguro(correoInstitucional);

            EliminarFotosAnteriores(rutaCarpeta, nombreBase);

            string nombreArchivo = nombreBase + extension;

            string rutaDestino =
                Path.Combine(rutaCarpeta, nombreArchivo);

            File.Copy(
                rutaOrigen,
                rutaDestino,
                true
            );

            return rutaDestino;
        }

        private void EliminarFotosAnteriores(
            string rutaCarpeta,
            string nombreBase)
        {
            string[] archivos = Directory.GetFiles(rutaCarpeta);

            foreach (string archivo in archivos)
            {
                bool mismoUsuario =
                    Path.GetFileNameWithoutExtension(archivo) == nombreBase;

                bool extensionPermitida =
                    _extensionesPermitidas.Contains(
                        Path.GetExtension(archivo).ToLower());

                if (mismoUsuario && extensionPermitida)
                {
                    File.Delete(archivo);
                }
            }
        }

        private static string ObtenerCarpetaFotos()
        {
            string rutaBase =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData
                );

            return Path.Combine(
                rutaBase,
                NombreCarpeta,
                NombreSubcarpeta
            );
        }

        private static string ObtenerNombreArchivoSeguro(
            string correoInstitucional)
        {
            if (string.IsNullOrWhiteSpace(correoInstitucional))
            {
                return "usuario";
            }

            foreach (char caracter in Path.GetInvalidFileNameChars())
            {
                correoInstitucional =
                    correoInstitucional.Replace(caracter, '_');
            }

            return correoInstitucional
                .Replace("@", "_")
                .Replace(".", "_");
        }
    }
}