///-----------------------------------------------------------------
///   File:         CloudLocator.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 16:59:02
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Base.Locator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Defines the <see cref="CloudLocator"/>
    /// </summary>
    public class CloudLocator
    {
        /// <summary>
        /// Initializes static members of the <see cref="CloudLocator"/> class.
        /// </summary>
        static CloudLocator()
        {
            DeviceName = System.Net.Dns.GetHostName();
        }

        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public static string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the function that is called to request the running cloud.
        /// </summary>
        public static Func<ICloud> GetCloud { get; set; }

        /// <summary>
        /// Gets or sets the function that is called to request the correct config for the specific cloud.
        /// </summary>
        public static Func<IConfig> GetConfig { get; set; }

        /// <summary>
        /// Locates the cloud via assembly reflection.
        /// </summary>
        /// <param name="customPath">The customPath<see cref="string"/></param>
        public static void Locate(string customPath = null)
        {
            string binPath = customPath ?? Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            foreach (string dll in Directory.GetFiles(binPath, "Nanomite.Server.*.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(p => p.FullName.Contains(Path.GetFileNameWithoutExtension(dll)));
                    if (asm == null)
                    {
                        asm = Assembly.LoadFile(dll);
                    }

                    foreach (var type in asm.GetTypes())
                    {
                        if (typeof(ICloud).IsAssignableFrom(type))
                        {
                            dynamic instance = Activator.CreateInstance(type);
                            MethodInfo toInvoke = type.GetMethod(nameof(ICloud.Register));
                            toInvoke.Invoke(instance, null);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            throw new Exception(string.Format("Failed to load cloud instacnce at {0}, make sure that the neccessary cloud assembly is located inside the cloud directory.", binPath));
        }
    }
}
