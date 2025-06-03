namespace CoursesManager.Helpers
{
    public static class MimeHelper
    {
        public static string GetMimeType(byte[] fileBytes)
        {
            if (fileBytes == null || fileBytes.Length < 12) return "application/octet-stream";

            // JPEG: FF D8 FF
            if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 && fileBytes[2] == 0xFF) return "image/jpeg";

            // PNG: 89 50 4E 47
            if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47) return "image/png";

            // GIF: 47 49 46 38
            if (fileBytes[0] == 0x47 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x38) return "image/gif";

            // BMP: 42 4D
            if (fileBytes[0] == 0x42 && fileBytes[1] == 0x4D) return "image/bmp";

            // TIFF (little endian): 49 49 2A 00
            if (fileBytes[0] == 0x49 && fileBytes[1] == 0x49 && fileBytes[2] == 0x2A && fileBytes[3] == 0x00) return "image/tiff";

            // TIFF (big endian): 4D 4D 00 2A
            if (fileBytes[0] == 0x4D && fileBytes[1] == 0x4D && fileBytes[2] == 0x00 && fileBytes[3] == 0x2A) return "image/tiff";

            // WebP: 52 49 46 46 00 00 00 00 57 45 42 50
            if (fileBytes[0] == 0x52 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x46 && // "RIFF"
                fileBytes[8] == 0x57 && fileBytes[9] == 0x45 && fileBytes[10] == 0x42 && fileBytes[11] == 0x50) // "WEBP"
                return "image/webp";

            return "application/octet-stream";
        }

    }
}