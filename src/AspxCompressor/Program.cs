using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AspxCompressor
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessParameters(args);
        }

        private static void ProcessParameters(string[] args)
        {   
            if (args.Length == 0 || args[0] == "/?")
            {
                PrintUsage();
                return;
            }

            if (args[0].StartsWith("http"))
            {
                // Process single web page
                ProcessSingleWebPage(args[0], args[1]);
                return;
            }

            if (File.Exists(args[0]))
            {
                // Process single file
                ProcessSingleFile(args[0], args[1]);
                return;
            }

            if (Directory.Exists(args[0]))
            {
                // Process path
                ProcessSingleDirectory(args[0], args[1]);
                return;
            }
        }

        private static void ProcessSingleFile(string inputPath, string outputPath)
        {
            FileCompressor f = new FileCompressor();
            f.Load(inputPath);
            f.Compress();
            f.Save(outputPath);

            PrintFileStatistics(inputPath, outputPath);
        }

        private static void ProcessSingleWebPage(string url, string outputPath)
        {
            // TODO: Handle binary types plus JS, CSS, etc
            WebPageCompressor f = new WebPageCompressor();
            f.Load(url);
            f.Compress();
            f.Save(outputPath);

            PrintWebPageStatistics(url, f.InputSize, outputPath);
        }


        private static void ProcessSingleDirectory(string directory, string outputPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            DirectoryInfo[] subdirectories = directoryInfo.GetDirectories();
            foreach (DirectoryInfo subdir in subdirectories)
            {
                ProcessSingleDirectory(subdir.FullName, outputPath + "\\" + subdir.Name);
            }

            FileInfo[] files = directoryInfo.GetFiles("*.*");

            foreach (FileInfo f in files)
            {
                ProcessSingleFile(f.FullName, outputPath + "\\" + f.Name);
            }
        }

        private static void PrintFileStatistics(string inputPath, string outputPath)
        {
            FileInfo inInfo = new FileInfo(inputPath);
            FileInfo outInfo = new FileInfo(outputPath);

            string currentDir = Directory.GetCurrentDirectory();
            string fileName = inInfo.FullName.Substring(currentDir.Length);

            long inSize = inInfo.Length;
            long outSize = outInfo.Length;
            double ratio = 100.0 * ((double) (inSize - outSize) / (double) inSize);

            if (outSize == inSize)
            {
                Console.WriteLine("Copied file " + fileName);
            }
            else
            {
                Console.WriteLine("Compressed file " + fileName + ":");
                Console.WriteLine(String.Format("\tOriginal: {0}, Compressed: {1}, Saved: {2:0}%", inSize, outSize, ratio));
            }
        }

        private static void PrintWebPageStatistics(string url, long inSize, string outputPath)
        {
            FileInfo outInfo = new FileInfo(outputPath);

            string currentDir = Directory.GetCurrentDirectory();

            long outSize = outInfo.Length;
            double ratio = 100.0 * ((double)(inSize - outSize) / (double)inSize);

            Console.WriteLine("Compressed page " + url + ":");
            Console.WriteLine(String.Format("\tOriginal: {0}, Compressed: {1}, Saved: {2:0}%", inSize, outSize, ratio));
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage:\n\n");
            Console.WriteLine("\tAspxCompressor <input filename> <output filename>");
            Console.WriteLine("\tAspxCompressor <input folder> <output folder>");
            Console.WriteLine("\tAspxCompressor <input url> <output folder>");
            Console.WriteLine("");
            //MessageBox.Show("test");

        }
    }
}
