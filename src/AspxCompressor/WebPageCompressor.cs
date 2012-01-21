using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace AspxCompressor
{
    /// <summary>
    /// Loads web pages and compresses them, writes the output to a file
    /// </summary>
    public class WebPageCompressor
    {
        public WebPageCompressor()
        {
            InputText = "";
            OutputText = "";
        }

        private string InputText;
        private string OutputText;

        /// <summary>
        /// Visits a URL and downloads the html
        /// </summary>
        /// <param name="url"></param>
        public void Load(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

            // Pipe the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, encode);
            this.InputText = readStream.ReadToEnd();

            response.Close();
        }

        public void Compress()
        {
            AspxStringCompressor sc = new AspxStringCompressor();
            this.OutputText = sc.Compress(this.InputText);
        }

        public void Save(string outputFile)
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

        public long InputSize
        {
            get
            {
                return (long) InputText.Length;
            }
        }
    }
}
