using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BestSellers.Deploy
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// Extends the delete method to all deletion of files and subfolders within the directory
        /// </summary>
        /// <param name="original"></param>
        /// <param name="includeFiles"></param>
        /// <returns></returns>
        static public void Delete(string path, bool includeFiles)
        {
            if (!Directory.Exists(path))
                return;

            if (includeFiles)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                DirectoryInfo[] subdirectories = directoryInfo.GetDirectories();
                foreach (DirectoryInfo subdir in subdirectories)
                {
                    Delete(subdir.FullName, true);
                }

                foreach (string filename in Directory.GetFiles(path))
                {
                    File.Delete(filename);
                }
            }

            Directory.Delete(path);
        }
    }
}