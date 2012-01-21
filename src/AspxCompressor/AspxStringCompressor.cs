using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AspxCompressor
{
    public class AspxStringCompressor
    {
        #region Constructor

        public AspxStringCompressor()
        {
        }

        #endregion Constructor

        #region Private Variables

        private static Regex specialAreaHideRegex = new Regex(@"(<pre>.*?</pre>)|(<textarea[^<>]*?/>)|(<textarea[^/]*?>.*?</textarea>)|(<script[^<>]*?/>)|(<script[^/]*?>.*?</script>)", RegexOptions.Singleline | RegexOptions.Compiled);
        
        private static Regex commentReg = new Regex(@"<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
//        private static Regex whitespaceReg = new Regex(@"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}", RegexOptions.Compiled);

        private static Regex whitespaceTrimReg = new Regex(@"^\s+|\s+$", RegexOptions.Compiled | RegexOptions.Multiline);
        private static Regex whitespaceMultipleEmptyLines = new Regex(@"\n{2,}", RegexOptions.Compiled);
        private static Regex whitespaceBetweenTagsReg = new Regex(@"(?<=[>])\s{3,}(?=[<])", RegexOptions.Compiled);

        private static Regex htmlPairReg = new Regex(@"(?<=<(/script|div|/div|li|ul|/ul|hr|table|tbody|th|/th|tr|/tr|/td|/p|br /|p /|p)[^>]*>)\s+", RegexOptions.Compiled);

        private static Regex javascriptAreaRegex = new Regex(@"(<script[^/]*?>)(.*?)(</script>)", RegexOptions.Singleline | RegexOptions.Compiled);
        private static Regex specialAreaUnhideRegex = new Regex(@"<specialarea />", RegexOptions.Compiled);

        private Queue<string> specialAreas;

        #endregion Private Variables

        #region Public Methods

        public string Compress(string inputText)
        {
            string outputText = inputText;

            // Pull out special areas
            specialAreas = new Queue<string>();

            outputText = specialAreaHideRegex.Replace(outputText, new MatchEvaluator(SpecialAreaHider));

            // Strip HTML comments
            outputText = commentReg.Replace(outputText, String.Empty);

            // Strip whitespace
            outputText = whitespaceTrimReg.Replace(outputText, String.Empty);
            outputText = whitespaceMultipleEmptyLines.Replace(outputText, "\n");
            outputText = whitespaceBetweenTagsReg.Replace(outputText, String.Empty);

            // Strip whitespace between pairs of html that are often found together and do not need whitespace
            // TODO: This could be probably find more whitespace to strip
            outputText = htmlPairReg.Replace(outputText, String.Empty);

            // Replace special areas
            outputText = specialAreaUnhideRegex.Replace(outputText, new MatchEvaluator(SpecialAreaUnhider));

            // Minimize Javascript areas
            //outputText = javascriptAreaRegex.Replace(outputText, new MatchEvaluator(JavaScriptShrinker));

            return outputText;
        }

        #endregion Public Methods

        #region Private Methods

        private string SpecialAreaHider(Match m)
        {
            specialAreas.Enqueue(m.Groups[0].Value);
            return "<specialarea />";
        }

        private string SpecialAreaUnhider(Match m)
        {
            return specialAreas.Dequeue();
        }

        private string JavaScriptShrinker(Match m)
        {
            JavaScriptMinifier jsmin = new JavaScriptMinifier();
            return m.Groups[1].Value + jsmin.Minify(m.Groups[2].Value) + m.Groups[3].Value;
        }

        #endregion Private Methods
    }
}
