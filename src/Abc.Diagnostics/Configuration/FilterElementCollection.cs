// ----------------------------------------------------------------------------
// <copyright file="FilterElementCollection.cs" company="ABC Software Ltd">
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
    using System.Configuration;

    /// <summary>
    /// The filter elements collection.
    /// </summary>
    /// <seealso cref="System.Configuration.ConfigurationElementCollection" />
    public class FilterElementCollection : ConfigurationElementCollection {
        /// <summary>
        /// Adds the specified <see cref="FilterElement"/> to the <see cref="ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="filter">The <see cref="FilterElement"/> to add.</param>
        public void Add(FilterElement filter) {
            base.BaseAdd(filter);
        }

        /// <summary>
        /// Removes the specified <see cref="FilterElement"/> from the <see cref="ConfigurationElementCollection"/>.
        /// </summary>
        /// <param name="filter">The <see cref="FilterElement"/> to remove.</param>
        public void Remove(FilterElement filter) {
            base.BaseRemove(this.GetElementKey(filter));
        }

        /// <inheritdoc/>
        public override ConfigurationElementCollectionType CollectionType {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <inheritdoc/>
        protected override ConfigurationElement CreateNewElement() {
            return new FilterElement();
        }

        /// <inheritdoc/>
        protected override object GetElementKey(ConfigurationElement element) {
            return ((FilterElement)element).TypeName;
        }

        /// <inheritdoc/>
        protected override string ElementName {
            get { return "filter"; }
        }
    }
}
#endif