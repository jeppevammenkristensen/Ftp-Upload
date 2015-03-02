using System.IO;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Files
{
    public class FileCopier
    {
        public static async Task CopyFromOneFolderToAnother(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();

            await Task.Run(() =>
            {
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(destDirName, file.Name);

                    file.CopyTo(temppath, true);
                }
            });

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    await CopyFromOneFolderToAnother(subdir.FullName, temppath, true);
                }
            }
        }
    }
}