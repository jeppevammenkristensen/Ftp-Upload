using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using ConfigR;
using Dapper;
using Effortless.Net.Encryption;
using Microsoft.Win32;
using Shared.Infrastructure.Database;

namespace Shared.Configuration.Database
{
    public class Bootstrapper
    {
        public void Bootstrap()
        {
            var databaseBootstrapper = new DatabaseBootstrapper();
            databaseBootstrapper.Setup();

            var encryptionBootstrapper = new EncryptionBootstrapper();
            encryptionBootstrapper.Setup();

            var contextMenuItemBootstrapper = new ContextItemBootstrapper();
            contextMenuItemBootstrapper.Setup();
        }
    }

    public class ContextItemBootstrapper
    {
        public void Setup()
        {
            try
            {
                var folderSubkey = Registry.ClassesRoot.OpenSubKey("Directory").OpenSubKey("shell",true);

                RegistryKey customFtpSubKey = folderSubkey.OpenSubKey("CustomFtp",true);
                if (customFtpSubKey == null)
                {
                    customFtpSubKey = folderSubkey.CreateSubKey("CustomFtp");
                    customFtpSubKey.SetValue("", "Upload folder til ftp");


                    var command = customFtpSubKey.CreateSubKey("command");
                    command.SetValue("",
                        string.Format("{0} \"%L\"", Assembly.GetExecutingAssembly().Location));
                    command.Close();

                    customFtpSubKey.Close();
                }
            }
            catch (Exception e)
            {
                // We catch the exception because security can cause this not to be available
            }
        }
    }

    public class EncryptionBootstrapper
    {
        private ConnectionManager _connectionManager = new ConnectionManager();

        public void Setup()
        {
            using (var connection = _connectionManager.OpenConnectionAsync().Result)
            {
                var encryption = connection.Query<EncryptionConfiguration>("SELECT top 1 * From Encryption").First();
                Config.Global.Add(ConfigurationKeys.Encryption, encryption);
            }
        }
    }

    public class DatabaseBootstrapper
    {
        public List<SetupStep> Steps = new List<SetupStep>()
        {
            new FirstStep()
        };

        public void Setup()
        {
            Config.Global.LoadScriptFile("Config.csx", AppDomain.CurrentDomain.GetAssemblies());

            using (var connection = new SqlConnection(Config.Global.Get<string>(ConfigurationKeys.configurationConnection)))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var first = connection.Query<int>(
                        @"SELECT Count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Version'",
                        transaction: transaction).First();
                    int version = 0;
                    
                    if (first > 0)
                        version = connection.Query<int>("SELECT MAX(Version) FROM Version",transaction:transaction).First();

                    foreach (var source in Steps.Where(x => x.Version > version))
                    {
                        source.Setup(connection, transaction);
                    }

                    transaction.Commit();
                }
            }
        } 
    }

    public abstract class SetupStep
    {
        public abstract int Version { get; }
        public abstract void Setup(IDbConnection connection, IDbTransaction transaction);

        public void ExecuteSql(IDbConnection connection, IDbTransaction transaction)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format("Upload.Data.{0}.sql", Version);

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var format = reader.ReadToEnd();
                    connection.Execute(format, transaction:transaction);
                }
            }
        }
    }

    public class FirstStep : SetupStep
    {
        public override int Version
        {
            get { return 1; }
        }

        public override void Setup(IDbConnection connection, IDbTransaction transaction)
        {
            ExecuteSql(connection,transaction);
            var encryption = new EncryptionConfiguration();
            encryption.IV = Bytes.GenerateIV();
            encryption.Key = Bytes.GenerateKey();
            connection.Execute("INSERT INTO Encryption(IV,[Key]) Values(@IV,@Key)", new {encryption.IV, encryption.Key}, transaction);
            connection.Execute("INSERT INTO Version(Version) Values (@Version)", new {Version}, transaction);
        }
    }


}