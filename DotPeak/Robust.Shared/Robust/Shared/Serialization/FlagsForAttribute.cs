// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.FlagsForAttribute
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Serialization;

[AttributeUsage(AttributeTargets.Enum, AllowMultiple = true, Inherited = false)]
public sealed class FlagsForAttribute : Attribute
{
  private readonly Type _tag;

  public Type Tag => this._tag;

  public FlagsForAttribute(Type tag) => this._tag = tag;
}
