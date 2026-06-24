// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.TypeParsers.Prototype`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Robust.Shared.Toolshed.TypeParsers;

[Obsolete("Use ProtoId<T> or EntProtoId, or the prototype directly")]
public readonly record struct Prototype<T>(T Value) : IAsType<string> where T : class, IPrototype
{
  public ProtoId<T> Id => (ProtoId<T>) this.Value.ID;

  public string AsType() => this.Value.ID;
}
