// ----------------------------------------------------------------------------
// <copyright file="NotificationFilterAssembler.cs" company="ABC Software Ltd">
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

#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic.Configuration {
#else
namespace Abc.Diagnostics.Configuration {
#endif
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
    using Microsoft.Practices.ObjectBuilder2;

    /// <summary>
    /// This type supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
    /// Represents the process to build a <see cref="CategoryFilter"/> described by a <see cref="CategoryFilterData"/> configuration object.
    /// </summary>
    /// <remarks>This type is linked to the <see cref="CategoryFilterData"/> type and it is used by the <see cref="LogFilterCustomFactory"/> 
    /// to build the specific <see cref="ILogFilter"/> object represented by the configuration object.
    /// </remarks>
    public class NotificationFilterAssembler : IAssembler<ILogFilter, LogFilterData> {
        /// <summary>
        /// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// Builds a <see cref="CategoryFilter"/> based on an instance of <see cref="CategoryFilterData"/>.
        /// </summary>
        /// <seealso cref="LogFilterCustomFactory"/>
        /// <param name="context">The <see cref="IBuilderContext"/> that represents the current building process.</param>
        /// <param name="objectConfiguration">The configuration object that describes the object to build. Must be an instance of <see cref="LogFilterData"/>.</param>
        /// <param name="configurationSource">The source for configuration objects.</param>
        /// <param name="reflectionCache">The cache to use retrieving reflection information.</param>
        /// <returns>A fully initialized instance of <see cref="CategoryFilter"/>.</returns>
        public ILogFilter Assemble(IBuilderContext context, LogFilterData objectConfiguration, IConfigurationSource configurationSource, ConfigurationReflectionCache reflectionCache) {
            NotificationFilterData castedObjectConfiguration = (NotificationFilterData)objectConfiguration;

            ICollection<string> groupFilters = new List<string>();
            foreach (GroupFilterEntry entry in castedObjectConfiguration.GroupFilters) {
                groupFilters.Add(entry.Name);
            }

            ILogFilter createdObject
                = new Filters.NotificationFilter(
                    castedObjectConfiguration.Name,
                    castedObjectConfiguration.AuthorityId,
                    groupFilters);

            return createdObject;
        }
    }
}
