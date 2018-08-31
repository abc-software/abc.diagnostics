// ----------------------------------------------------------------------------
// <copyright file="LogWriterFactory.cs" company="ABC Software Ltd">
//    Copyright © 2017 ABC Software Ltd. All rights reserved.
//
//    This library is free software; you can redistribute it and/or.
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
#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic.Configuration {
#else
namespace Abc.Diagnostics.Configuration {
#endif
    using System;
    using System.Configuration;
    using System.Reflection;

    /// <summary>
    /// The <see cref="ILogWriter"/> factory.
    /// </summary>
    internal static class LogWriterFactory {
        /// <summary>
        /// Cretes the log writer.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns><see cref="ILogWriter"/> instance.</returns>
        /// <exception cref="ConfigurationErrorsException">If cannot create <see cref="ILogWriter"/>.</exception>
        public static ILogWriter CreteLogWriter(ILogWriterConfigration configuration) {
            Type c;

            // If we have custom ILogWriter class
            string typeName = configuration.TypeName;
            if (!string.IsNullOrEmpty(typeName)) {
                c = Type.GetType(typeName);
            }
            else {
                // Detect Automaticaly
                if (EntrLib50LogWriter.Available) {
                    c = typeof(EntrLib50LogWriter);
                }
                else if (EntrLib40LogWriter.Available) {
                    c = typeof(EntrLib40LogWriter);
                }
                else {
                    c = typeof(DefaultLogWriter);
                }
            }

            if (c == null) {
                throw new ConfigurationErrorsException(Abc.Diagnostics.Configuration.ConfigSR.ConfigurationCouldNotFindTypeFormat(typeName));
            }

            if (!typeof(ILogWriter).IsAssignableFrom(c)) {
                throw new ConfigurationErrorsException(Abc.Diagnostics.Configuration.ConfigSR.ConfigurationIncorrectBaseTypeFormat(typeName, typeof(ILogWriter).FullName));
            }

            string initializeData = configuration.InitData;
            if (!string.IsNullOrEmpty(initializeData)) {
                ConstructorInfo constructor2 = c.GetConstructor(new Type[] { typeof(string) });
                if (constructor2 == null) {
                    throw new ConfigurationErrorsException(Abc.Diagnostics.Configuration.ConfigSR.ConfigCouldNotGetConstructorFormat(typeName));
                }

                return (ILogWriter)constructor2.Invoke(new object[] { initializeData });
            }

            ConstructorInfo constructor = c.GetConstructor(new Type[0]);
            if (constructor == null) {
                throw new ConfigurationErrorsException(Abc.Diagnostics.Configuration.ConfigSR.ConfigCouldNotGetConstructorFormat(typeName));
            }

            var runtimeObject = (ILogWriter)constructor.Invoke(new object[0]);

            // Support cutom attributes
            var runtimeObjectWithAttrbutes = runtimeObject as ILogWriterCustomAttributes;
            if (runtimeObjectWithAttrbutes != null) {
                var supportedAttributes = runtimeObjectWithAttrbutes.GetSupportedAttributes();

                foreach (string key in configuration.Attributes.Keys) {
                    bool flag = false;
                    if (supportedAttributes != null) {
                        foreach (var item in supportedAttributes) {
                            if (item == key) {
                                flag = true;
                                break;
                            }
                        }
                    }

                    if (!flag) {
                        throw new ConfigurationErrorsException(Abc.Diagnostics.Configuration.ConfigSR.ConfigAttributeNotSupportedFormat(key, typeName));
                    }
                }

                runtimeObjectWithAttrbutes.SetAttributes(configuration.Attributes);
            }

            return runtimeObject;
        }
    }
}
#endif