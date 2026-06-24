using System;

namespace Robust.Shared.ContentPack;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class SkipIfSandboxedAttribute : Attribute
{
}
