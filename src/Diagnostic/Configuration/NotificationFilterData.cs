// ----------------------------------------------------------------------------
// <copyright file="NotificationFilterData.cs" company="ABC Software Ltd">
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
    using System.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

    /// <summary>
    /// Represents the configuration for a notification filter.
    /// </summary>
    [Assembler(typeof(NotificationFilterAssembler))]
    //// [ContainerPolicyCreator(typeof(NotificationFilterPolicyCreator))]
    public class NotificationFilterData : LogFilterData {
        private const string NotificationAuthorityIdProperty = "authorityId";
        private const string NotificationGroupsProperty = "groupFilters";

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationFilterData"/> class.
        /// </summary>
        public NotificationFilterData() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationFilterData"/> class.
        /// </summary>
        /// <param name="authorityId">The authorityId to filter.</param>
        /// <param name="groupFilters">The collection of group names to filter.</param>
        public NotificationFilterData(int authorityId, NamedElementCollection<GroupFilterEntry> groupFilters)
            : this("notification", authorityId, groupFilters) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationFilterData"/> class.
        /// </summary>
        /// <param name="name">The filter name.</param>
        /// <param name="authorityId">The authorityId to filter.</param>
        /// <param name="groupFilters">The collection of group names to filter.</param>
        public NotificationFilterData(string name, int authorityId, NamedElementCollection<GroupFilterEntry> groupFilters)
            : base(name, typeof(Filters.NotificationFilter)) {
            this.GroupFilters = groupFilters;
            this.AuthorityId = authorityId;
        }

        /// <summary>
        /// Gets or sets the authorityId value for messages to be processed.  Messages with a authority id
        /// not equal are dropped immediately on the client.
        /// </summary>
        [ConfigurationProperty(NotificationAuthorityIdProperty, DefaultValue = -1)]
        public int AuthorityId {
            get {
                return (int)this[NotificationAuthorityIdProperty];
            }

            set {
                this[NotificationAuthorityIdProperty] = value;
            }
        }

        /// <summary>
        /// Gets collection of <see cref="GroupFilterEntry"/>.
        /// </summary>
        [ConfigurationProperty(NotificationGroupsProperty)]
        public NamedElementCollection<GroupFilterEntry> GroupFilters {
            get {
                return (NamedElementCollection<GroupFilterEntry>)base[NotificationGroupsProperty];
            }

            private set {
                base[NotificationGroupsProperty] = value;
            }
        }
    }
}

