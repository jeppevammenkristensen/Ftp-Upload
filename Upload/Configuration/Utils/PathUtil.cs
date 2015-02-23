using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Upload.Configuration.Utils
{
    public static class ConfigurationPathUtil
    {
        private static readonly string[] _paths = new[]
        {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FtpUploader"};

        public static string GetEnsuredPathConfigurationPath(IEnumerable<string> paths)
        {
            var pathArray = paths.ToArray();
            var currentPath = pathArray.First();
            EnsureSinglePath(currentPath);

            foreach (var path in pathArray.Skip(1))
            {
                currentPath = Path.Combine(currentPath, path);
                Directory.CreateDirectory(currentPath);
            }

            return currentPath;
        }

        private static void EnsureSinglePath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string CopyEnsuredFile(string additionalPath,string fileName)
        {
            var folderPath = GetEnsuredPathConfigurationPath(
                _paths.Concat(additionalPath.Split(new string[] {"\\"}, StringSplitOptions.RemoveEmptyEntries)));

            var filePath = Path.Combine(folderPath, fileName);

            if (!File.Exists(filePath))
                File.Move(GetStartPath(additionalPath, fileName),filePath);

            return filePath;
        }

        private static string GetStartPath(string additionalPath, string fileName)
        {
            var simplePath = Path.Combine(additionalPath, fileName);
            if (File.Exists(simplePath))
                return simplePath;

            // ClickOnce path. Who said it had to be easy
            return Path.Combine((string) AppDomain.CurrentDomain.GetData("DataDirectory"), simplePath);

        }
    }
}