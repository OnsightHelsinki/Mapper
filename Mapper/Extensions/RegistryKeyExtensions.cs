using System;
using Microsoft.Win32;

namespace Mapper.Extensions
{
    public static class RegistryKeyExtensions
    {
            public static RegistryKey GetOrCreateSubKey(this RegistryKey registryKey, string parentKeyLocation,
                string key, bool writable)
            {
                var keyLocation = $@"{parentKeyLocation}\{key}";

                var foundRegistryKey = registryKey.OpenSubKey(keyLocation, writable);

                return foundRegistryKey ?? registryKey.CreateSubKey(parentKeyLocation, key);
            }

            public static RegistryKey CreateSubKey(this RegistryKey registryKey, string parentKeyLocation, string key)
            {
                var parentKey = registryKey.OpenSubKey(parentKeyLocation, true);
                if (parentKey == null) { throw new NullReferenceException($"Missing parent key: {parentKeyLocation}"); }

                var createdKey = parentKey.CreateSubKey(key);
                if (createdKey == null) { throw new Exception($"Key not created: {key}"); }

                return createdKey;
            }
    }
}
