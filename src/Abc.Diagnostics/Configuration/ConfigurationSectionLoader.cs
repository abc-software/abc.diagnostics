// ----------------------------------------------------------------------------
// <copyright file="ConfigurationSectionLoader.cs" company="ABC Software Ltd">
//    Copyright © 2015 ABC Software Ltd. All rights reserved.
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License  as published by the Free Software Foundation, either 
//    version 3 of the License, or (at your option) any later version. 
//
//    This library is distributed in the hope that it will be useful, 
//    but WITHOUT ANY WARRANTY; without even the implied warranty of 
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU 
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with the library. If not, see http://www.gnu.org/licenses/.
// </copyright>
// ----------------------------------------------------------------------------

#if !NETSTANDARD
namespace Abc.Diagnostics.Configuration {
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Load configuration section.
    /// </summary>
    /// <typeparam name="T">The configuration section type.</typeparam>
    internal class ConfigurationSectionLoader<T> where T : ConfigurationSection, new() {
#pragma warning disable S2743 // Static fields should not be used in generic types
        private static Exception configurationException;
        private static int recursionGuard = 0;
        private static object configurationLock = new object();
        private static T sectionObject = default(T);
        private static string name;
#pragma warning restore S2743 

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSectionLoader&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="sectionName">The configuration section path and name.</param>
        public ConfigurationSectionLoader(string sectionName) {
            if (string.IsNullOrEmpty(sectionName)) {
                throw new ArgumentNullException("sectionName");
            }

#pragma warning disable S3010 // Static fields should not be updated in constructors
            name = sectionName;
#pragma warning restore
        }

        /// <summary>
        /// Retrieves a specified configuration section for the current application's default configuration.
        /// </summary>
        /// <returns>
        /// The specified <see cref="T:System.Configuration.ConfigurationSection"></see> object, or null if the section does not exist.
        /// </returns>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
        public T GetSection() {
            return this.GetSection(false);
        }

        /// <summary>
        /// Retrieves a specified configuration section for the current application's default configuration.
        /// </summary>
        /// <param name="permitNull">if set to <c>true</c> permit null.</param>
        /// <returns>
        /// The specified <see cref="T:System.Configuration.ConfigurationSection"></see> object, or null if the section does not exist.
        /// </returns>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">A configuration file could not be loaded.</exception>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "For code better interpritation")]
        public T GetSection(bool permitNull) {
            if (sectionObject == null) {
                lock (configurationLock) {
                    if (sectionObject == null) {
                        T section = default(T);
                        if (configurationException == null) {
                            if (Interlocked.CompareExchange(ref recursionGuard, 1, 0) > 0) {
                                throw new ConfigurationErrorsException(ConfigSR.ConfigurationSectionGroupNotInitializedFormat(name));
                            }

                            try {
                                // Check to see if we are running on the server without loading system.web.dll
#if WEB
                                if (Thread.GetDomain().GetData(".appDomain") != null) {
                                    HttpContext context = HttpContext.Current;
                                    if (context != null) {
                                        section = (T)context.GetSection(_name);
                                    }
                                }
#endif

                                if (section == null) {
                                    section = (T)ConfigurationManager.GetSection(name);
                                }

                                if (section == null) {
                                    if (!permitNull) {
                                        throw new ConfigurationErrorsException(ConfigSR.ConfigurationSectionErrorFormat(name));
                                    }

                                    section = Activator.CreateInstance<T>();
                                }

                                sectionObject = section;

                                if (Interlocked.CompareExchange(ref recursionGuard, 2, 1) != 1) {
                                    throw new ConfigurationErrorsException(ConfigSR.ConfigurationThreadingError);
                                }
                            }
                            catch (Exception exception) {
                                // Cache the exception for perf reason
                                configurationException = exception;

                                // rethrow
                                throw new ConfigurationErrorsException(ConfigSR.ConfigurationSectionGroupError, exception);
                            }
                        }
                        else {
                            // rethrow
                            throw new ConfigurationErrorsException(ConfigSR.ConfigurationSectionGroupError, configurationException);
                        }
                    }
                }
            }

            return sectionObject;
        }

        /// <summary>
        /// Determines whether a specified configuration section is loaded.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a specified configuration section is loaded; otherwise, <c>false</c>.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "For code better interpritation")]
        public bool IsLoaded() {
            return Interlocked.CompareExchange(ref recursionGuard, 2, 2) == 2;
        }

        /// <summary>
        /// Reload configuration.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "For code better interpritation")]
        public void Reload() {
            lock (configurationLock) {
                sectionObject = null;
                ConfigurationManager.RefreshSection(name);
                Interlocked.CompareExchange(ref recursionGuard, 0, 2);
            }
        }
    }
}
#endif