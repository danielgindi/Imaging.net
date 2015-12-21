using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.AccessControl;
using System.IO;
using System.Web.Hosting;
using System.Web;

namespace Imaging.net.Internal
{
    /// <summary>
    /// File utilities, from https://github.com/danielgindi/dg.Utilities
    /// </summary>
    internal class FileHelper
    {

        /// <summary>
        /// Creates an empty file in the TEMP folder.
        /// Note that you might want to reset the file's permissions after moving, because it has the permissions of the TEMP folder.
        /// </summary>
        /// <returns>Path to the temp file that was created</returns>
        public static string CreateEmptyTempFile()
        {
            string tempFilePath = FolderHelper.GetTempDir() + Guid.NewGuid().ToString() + @".tmp";
            FileStream fs = null;
            while (true)
            {
                try
                {
                    fs = new FileStream(tempFilePath, FileMode.CreateNew);
                    break;
                }
                catch (IOException ioex)
                {
                    Console.WriteLine(@"Utility.File.CreateEmptyTempFile - Error: {0}", ioex.ToString());
                    if (System.IO.File.Exists(tempFilePath))
                    { // File exists, make up another name
                        tempFilePath = FolderHelper.GetTempDir() + Guid.NewGuid().ToString() + @".tmp";
                    }
                    else
                    { // Another error, throw it back up
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(@"Utility.File.CreateEmptyTempFile - Error: {0}", ex.ToString());
                    break;
                }
            }
            if (fs != null)
            {
                fs.Dispose();
                fs = null;
                return tempFilePath;
            }
            return null;
        }

        /// <summary>
        /// Reset the file's permissions to it's parent folder's permissions
        /// </summary>
        /// <param name="filePath">Path to the target file</param>
        public static void ResetFilePermissionsToInherited(string filePath)
        {
            FileSecurity fileSecurity = File.GetAccessControl(filePath);
            fileSecurity.SetAccessRuleProtection(false, true);
            foreach (FileSystemAccessRule rule in fileSecurity.GetAccessRules(true, false, typeof(System.Security.Principal.NTAccount)))
            {
                fileSecurity.RemoveAccessRule(rule);
            }
            File.SetAccessControl(filePath, fileSecurity);
        }

        public static string MapPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            else if (HostingEnvironment.IsHosted)
            {
                return HostingEnvironment.MapPath(path);
            }
            else if (VirtualPathUtility.IsAppRelative(path))
            {
                string physicalPath = VirtualPathUtility.ToAbsolute(path, "/");
                physicalPath = physicalPath.Replace('/', '\\');
                physicalPath = physicalPath.Substring(1);
                physicalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, physicalPath);

                return physicalPath;
            }
            else
            {
                throw new Exception("Could not resolve non-rooted path.");
            }
        }

    }
}
