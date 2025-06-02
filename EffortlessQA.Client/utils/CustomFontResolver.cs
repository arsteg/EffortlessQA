using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PdfSharpCore.Fonts;

namespace EffortlessQA.Client.utils
{
    public class CustomFontResolver : IFontResolver
    {
        private readonly string _basePath;

        public CustomFontResolver(IWebAssemblyHostEnvironment hostEnvironment)
        {
            _basePath = hostEnvironment.BaseAddress; // Gets the app's base path
        }

        public string DefaultFontName => "DejaVuSans";

        public byte[] GetFont(string faceName)
        {
            try
            {
                if (
                    faceName.Equals("DejaVuSans", StringComparison.OrdinalIgnoreCase)
                    || faceName.Equals("Helvetica", StringComparison.OrdinalIgnoreCase)
                )
                {
                    // Use relative path from wwwroot
                    var fontPath = "fonts/DejaVuSans.ttf";
                    var fullPath = Path.Combine(_basePath, fontPath).TrimStart('/');
                    using (var client = new HttpClient())
                    {
                        var fontBytes = client.GetByteArrayAsync(fullPath).GetAwaiter().GetResult();
                        return fontBytes;
                    }
                }
                throw new NotSupportedException($"Font '{faceName}' is not supported.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load font '{faceName}': {ex.Message}", ex);
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            try
            {
                if (
                    familyName.Equals("DejaVuSans", StringComparison.OrdinalIgnoreCase)
                    || familyName.Equals("Helvetica", StringComparison.OrdinalIgnoreCase)
                )
                {
                    return new FontResolverInfo("DejaVuSans");
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to resolve typeface '{familyName}': {ex.Message}", ex);
            }
        }
    }
}
