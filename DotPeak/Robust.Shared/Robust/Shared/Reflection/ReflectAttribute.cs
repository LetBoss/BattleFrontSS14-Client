// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Reflection.ReflectAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Reflection;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ReflectAttribute : Attribute
{
  public const bool DEFAULT_DISCOVERABLE = true;

  public bool Discoverable { get; }

  public ReflectAttribute(bool discoverable) => this.Discoverable = discoverable;
}
