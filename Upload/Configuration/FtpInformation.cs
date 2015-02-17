using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Upload.Configuration
{
    public class FtpInformation
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }


        public string Name { get; set; }

        private static FtpInformation QuickCopy()
        {
            return new FtpInformation()
            {
                Password = "",
                Server =  "",
                UserName = "",
                Name = "CustomName",
                Path = "/my/customlocation",
            };
        }

        public string Path { get; set; }

        public List<string> GetPropertiesMissingInitialization()
        {
            var tester = new List<Tuple<string, Func<string>>>()
            {
                new Tuple<string, Func<string>>("Server", () => Server),
                new Tuple<string, Func<string>>("UserName", () => UserName),
                new Tuple<string, Func<string>>("Password", () => Password),
                new Tuple<string, Func<string>>("Path", () => Path)
            };


            return tester
                .Where(x => string.IsNullOrWhiteSpace(x.Item2()))
                .Select(x => x.Item1)
                .ToList();
        }
    }
    
    

}