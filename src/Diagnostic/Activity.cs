// ----------------------------------------------------------------------------
// <copyright file="Activity.cs" company="ABC Software Ltd">
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
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;

    /// <summary>
    /// Represent activity.
    /// </summary>
    internal class Activity : IDisposable {
        private readonly Guid parentId;
        private readonly Guid currentId;
        private bool mustDispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        /// <param name="activityId">The activity id.</param>
        /// <param name="parentId">The parent id.</param>
        protected Activity(Guid activityId, Guid parentId) {
            this.currentId = activityId;
            this.parentId = parentId;
            this.mustDispose = true;
            LogUtility.ActivityId = this.currentId;
        }

        /// <summary>
        /// Gets the ActivityId.
        /// </summary>
        /// <value>The ActivityId.</value>
        protected Guid Id {
            get { return this.currentId; }
        }

        /// <summary>
        /// Gets the parent ActivityId.
        /// </summary>
        /// <value>The parent ActivityId.</value>
        protected Guid ParentId {
            get { return this.parentId; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose() {
            if (this.mustDispose) {
                this.mustDispose = false;
                LogUtility.ActivityId = this.parentId;
            }

            GC.SuppressFinalize(this);
        }

        internal static Activity CreateActivity(Guid activityId) {
            Activity activity = null;
            if (activityId != Guid.Empty) {
                Guid prevParentId = LogUtility.ActivityId;
                if (activityId != prevParentId) {
                    activity = new Activity(activityId, prevParentId);
                }
            }

            return activity;
        }
    }
}
