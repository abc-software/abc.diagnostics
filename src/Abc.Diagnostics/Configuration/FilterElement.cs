// ----------------------------------------------------------------------------
// <copyright file="FilterElement.cs" company="ABC Software Ltd">
//    Copyright © 2018 ABC Software Ltd. All rights reserved.
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
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Xml;

    /// <summary>
    /// The <see cref="RoutedLogWriter"/> filter element.
    /// </summary>
    /// <seealso cref="System.Configuration.ConfigurationElement" />
    public class FilterElement : ConfigurationElement, ILogWriterConfigration {
        private readonly ConfigurationProperty propTypeName = new ConfigurationProperty("type", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired);
        private readonly ConfigurationProperty propInitData = new ConfigurationProperty("initializeData", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
        private readonly ConfigurationProperty propCategories = new ConfigurationProperty("categories", typeof(CommaDelimitedStringCollectionConverter), null, new CommaDelimitedStringCollectionConverter(), null, ConfigurationPropertyOptions.IsRequired);

        private readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private Dictionary<string, string> attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterElement"/> class.
        /// </summary>
        public FilterElement() {
            this.properties.Add(this.propTypeName);
            this.properties.Add(this.propInitData);
            this.properties.Add(this.propCategories);
        }

        /// <summary>
        /// Gets or sets the categories.
        /// </summary>
        /// <value>The categories.</value>
        [TypeConverter(typeof(CommaDelimitedStringCollectionConverter))]
        [ConfigurationProperty("categories", IsRequired = true, IsDefaultCollection = false)]
        public CommaDelimitedStringCollection Categories {
            get { return (CommaDelimitedStringCollection)base[this.propCategories]; }
            set { base[this.propCategories] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        [ConfigurationProperty("type", IsRequired = true, DefaultValue = "", IsKey = true)]
        public virtual string TypeName {
            get { return (string)base[this.propTypeName]; }
            set { base[this.propTypeName] = value; }
        }

        /// <summary>
        /// Gets or sets the initialize data.
        /// </summary>
        /// <value>The initialize data.</value>
        [ConfigurationProperty("initializeData", DefaultValue = "")]
        public string InitData {
            get { return (string)base[this.propInitData]; }
            set { base[this.propInitData] = value; }
        }

        /// <summary>
        /// Gets the custom trace listener attributes defined in the application configuration file.
        /// </summary>
        public Dictionary<string, string> Attributes {
            get {
                if (this.attributes == null) {
                    this.attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }

                return this.attributes;
            }
        }

        /// <summary>
        /// Gets the collection of properties.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Configuration.ConfigurationPropertyCollection"/> of properties for the element.
        /// </returns>
        protected override ConfigurationPropertyCollection Properties {
            get { return this.properties; }
        }

        /// <summary>
        /// Gets a value indicating whether an unknown attribute is encountered during deserialization.
        /// </summary>
        /// <param name="name">The name of the unrecognized attribute.</param>
        /// <param name="value">The value of the unrecognized attribute.</param>
        /// <returns>
        /// <c>true</c> when an unknown attribute is encountered while deserializing; otherwise, <c>false</c>.
        /// </returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value) {
            this.Attributes.Add(name, value);
            return true;
        }

        /// <summary>
        /// Writes the contents of this configuration element to the configuration file when implemented in a derived class.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> that writes to the configuration file.</param>
        /// <param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false.</param>
        /// <returns>
        /// true if any data was actually serialized; otherwise, false.
        /// </returns>
        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey) {
            return base.SerializeElement(writer, serializeCollectionKey || (this.attributes != null && this.attributes.Count > 0));
        }

        /// <summary>
        /// Called before serialization.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> that will be used to serialize the <see cref="T:System.Configuration.ConfigurationElement" />.</param>
        protected override void PreSerialize(XmlWriter writer) {
            if (this.attributes != null && writer != null) {
                foreach (var item in this.attributes) {
                    if (item.Value != null) {
                        writer.WriteAttributeString(item.Key, item.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Modifies the <see cref="T:System.Configuration.ConfigurationElement" /> object to remove all values that should not be saved.
        /// </summary>
        /// <param name="sourceElement">A <see cref="T:System.Configuration.ConfigurationElement" /> at the current level containing a merged view of the properties.</param>
        /// <param name="parentElement">The parent <see cref="T:System.Configuration.ConfigurationElement" />, or null if this is the top level.</param>
        /// <param name="saveMode">A <see cref="T:System.Configuration.ConfigurationSaveMode" /> that determines which property values to include.</param>
        protected override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode) {
            base.Unmerge(sourceElement, parentElement, saveMode);
            var element = sourceElement as FilterElement;
            if (element != null && element.attributes != null) {
                this.attributes = element.attributes;
            }
        }
    }
}
#endif