using System;
using Shared.Configuration.ConfigR;

namespace Shared.Configuration
{
    public static class UploadConfig
    {
        private static IConfigurationManager _global = null;

        public static IConfigurationManager Global
        {
            get
            {
                if (_global == null)
                    throw new InvalidOperationException("The global configuration has not been configured. Call Initialize to initalize it");
                return _global;
            }
        }

        public static void SetConfigurationManager(IConfigurationManager configurationManager, params string[] args)
        {
            if (configurationManager is IInitializableConfigurationManager)
            {
               ((IInitializableConfigurationManager)configurationManager).Initalize(args);
            }

            _global = configurationManager;
        }
    }
}