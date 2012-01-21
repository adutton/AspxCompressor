using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AspxCompressor
{
    public class FileCompressor
    {
        public FileCompressor()
        {
            InputText = "";
            OutputText = "";
        }

        private string InputText;
        private string OutputText;
        private string InputFile;

        public bool IsCompressed = false;

        public void Load(string inputFile)
        {
            if (!File.Exists(inputFile))
            { 
                throw new FileNotFoundException("Input file " + inputFile + " does not exist.");
            }

            // Read input
            TextReader input = File.OpenText(inputFile);
            this.InputText = input.ReadToEnd();
            input.Close();

            this.InputFile = inputFile;
        }

        public void Compress()
        {
            switch (InputFile.Substring(InputFile.LastIndexOf(".")).ToLower())
            {
                case ".aspx":
                case ".ascx":
                case ".master":
                case ".htm":
                case ".html":

                    AspxStringCompressor sc = new AspxStringCompressor();
                    this.OutputText = sc.Compress(this.InputText);
                    IsCompressed = true;
                    break;

                case ".js":
                    JavaScriptMinifier jsmin = new JavaScriptMinifier();
                    this.OutputText = jsmin.Minify(this.InputText);
                    IsCompressed = true;
                    break;

                case ".css":
                    this.OutputText = CssCompressor.Compress(this.InputText);
                    IsCompressed = true;
                    break;

                default:
                    IsCompressed = false;
                    break;
            }

        }

        public void Save(string outputFile)
        {
            if (IsCompressed)
            {
                // Write output
                try
                {
                    TextWriter output = File.CreateText(outputFile);
                    output.Write(this.OutputText);
                    output.Close();
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine("Could not write output file: " + outputFile + ".  Exception: " + ex.Message);
                }
            }
            else
            {
                // Just copy the file
                File.Copy(this.InputFile, outputFile, true);
            }
        }
    }
}
