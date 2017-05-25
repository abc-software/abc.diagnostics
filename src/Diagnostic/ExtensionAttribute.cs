﻿#if NET20 || NET30
namespace System.Runtime.CompilerServices {
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    internal sealed class ExtensionAttribute : Attribute {
    }
}
#endif
