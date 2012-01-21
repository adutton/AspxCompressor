using System;

namespace AspxCompressor
{
    public static class CssCompressor
    {
        public static string Compress(string css)
        {
            return Compress(css, 0);
        }

        public static string Compress(string css, int columnWidth)
        {
            string compressedCss = null;

            string yuiCompressedCss = YUICompressor.Compress(css, columnWidth);
            string michaelAshsRegexEnhancementsCompressedCss = MichaelAshRegexCompressor.Compress(css, columnWidth);
            compressedCss = yuiCompressedCss.Length < michaelAshsRegexEnhancementsCompressedCss.Length ? yuiCompressedCss : michaelAshsRegexEnhancementsCompressedCss;

            // Only return compressed CSS if it is smaller
            if (css.Length > compressedCss.Length)
                return compressedCss;

            return css;
        }
    }
}