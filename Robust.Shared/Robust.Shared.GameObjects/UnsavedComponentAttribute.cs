using System;

namespace Robust.Shared.GameObjects;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class UnsavedComponentAttribute : Attribute
{
}
