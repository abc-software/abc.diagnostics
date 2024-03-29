﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------

#if NET20 || NET30 || NET35 || NET40
namespace Diagnostic {
#else
namespace Abc.Diagnostics {
#endif
    using System;
    using System.Reflection;

	///<summary>
	/// Provides access to resource key names for the SR resource file.
	///</summary>
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class SR {
		private static global::System.Resources.ResourceManager resourceManager;
		private static global::System.Globalization.CultureInfo resourceCulture;

		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal SR() {
		}

		/// <summary>
		/// Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceManager, null)) {
#if NET20 || NET30 || NET35 || NET40
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Diagnostic.SR", typeof(SR).Assembly);
#elif NETSTANDARD
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Abc.Diagnostics.SR", typeof(SR).GetTypeInfo().Assembly);
#else
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Abc.Diagnostics.SR", typeof(SR).Assembly);
#endif
					resourceManager = temp;
				}
				return resourceManager;
			}
		}
        
		/// <summary>
		/// Overrides the current thread's CurrentUICulture property for all
		/// resource lookups using this strongly typed resource class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture {
			get {
				return resourceCulture;
			}
			set {
				resourceCulture = value;
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Activity boundary..
		/// </summary>
		internal static string ActivityBoundary {
			get {
				return ResourceManager.GetString("ActivityBoundary", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to AuthenticationType.
		/// </summary>
		internal static string ExtraInformation_AuthenticationType {
			get {
				return ResourceManager.GetString("ExtraInformation_AuthenticationType", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to ExceptionMessage.
		/// </summary>
		internal static string ExtraInformation_ExceptionMessage {
			get {
				return ResourceManager.GetString("ExtraInformation_ExceptionMessage", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to ExceptionString.
		/// </summary>
		internal static string ExtraInformation_ExceptionString {
			get {
				return ResourceManager.GetString("ExtraInformation_ExceptionString", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to ExceptionType.
		/// </summary>
		internal static string ExtraInformation_ExceptionType {
			get {
				return ResourceManager.GetString("ExtraInformation_ExceptionType", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to IdentityName.
		/// </summary>
		internal static string ExtraInformation_IdentityName {
			get {
				return ResourceManager.GetString("ExtraInformation_IdentityName", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to IsAuthenticated.
		/// </summary>
		internal static string ExtraInformation_IsAuthenticated {
			get {
				return ResourceManager.GetString("ExtraInformation_IsAuthenticated", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to NativeErrorCode.
		/// </summary>
		internal static string ExtraInformation_NativeErrorCode {
			get {
				return ResourceManager.GetString("ExtraInformation_NativeErrorCode", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Unable to read system property. Error message: {0}..
		/// </summary>
		internal static string ExtraInformation_PropertyError {
			get {
				return ResourceManager.GetString("ExtraInformation_PropertyError", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to at.
		/// </summary>
		internal static string ExtraInformation_SchemaHelperAtString {
			get {
				return ResourceManager.GetString("ExtraInformation_SchemaHelperAtString", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to in {0}: line {1}.
		/// </summary>
		internal static string ExtraInformation_SchemaHelperLine {
			get {
				return ResourceManager.GetString("ExtraInformation_SchemaHelperLine", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to &lt;UnknownType&gt;.
		/// </summary>
		internal static string ExtraInformation_SchemaHelperUnknownType {
			get {
				return ResourceManager.GetString("ExtraInformation_SchemaHelperUnknownType", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to StackTrace.
		/// </summary>
		internal static string ExtraInformation_StackTrace {
			get {
				return ResourceManager.GetString("ExtraInformation_StackTrace", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Unable to process stack trace..
		/// </summary>
		internal static string ExtraInformation_StackTraceException {
			get {
				return ResourceManager.GetString("ExtraInformation_StackTraceException", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Insufficient privilege to generate stack trace..
		/// </summary>
		internal static string ExtraInformation_StackTraceSecurityException {
			get {
				return ResourceManager.GetString("ExtraInformation_StackTraceSecurityException", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Throwing an exception..
		/// </summary>
		internal static string ThrowingException {
			get {
				return ResourceManager.GetString("ThrowingException", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Trace.
		/// </summary>
		internal static string TraceAsTraceSource {
			get {
				return ResourceManager.GetString("TraceAsTraceSource", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Audit.
		/// </summary>
		internal static string TraceAudit {
			get {
				return ResourceManager.GetString("TraceAudit", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to AppDomain unloading..
		/// </summary>
		internal static string TraceCodeAppDomainUnload {
			get {
				return ResourceManager.GetString("TraceCodeAppDomainUnload", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to End Trace: Method &apos;{0}&apos; at {1} ticks (elapsed time: {2} seconds).
		/// </summary>
		internal static string TraceEndMessage {
			get {
				return ResourceManager.GetString("TraceEndMessage", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Handling an exception..
		/// </summary>
		internal static string TraceHandledException {
			get {
				return ResourceManager.GetString("TraceHandledException", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Start Trace: Method &apos;{0}&apos; at {1} ticks.
		/// </summary>
		internal static string TraceStartMessage {
			get {
				return ResourceManager.GetString("TraceStartMessage", resourceCulture);
			}
		}

		/// <summary>
		/// Looks up a localized string similar to Unhandled exception..
		/// </summary>
		internal static string UnhandledException {
			get {
				return ResourceManager.GetString("UnhandledException", resourceCulture);
			}
		}

	}
} 

