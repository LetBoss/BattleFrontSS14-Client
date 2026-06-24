// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.PrototypeManagerExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Prototypes;

public static class PrototypeManagerExt
{
  [return: NotNullIfNotNull("protoId")]
  public static T? Index<T>(this IPrototypeManager prototypeManager, ProtoId<T>? protoId) where T : class, IPrototype
  {
    return protoId.HasValue ? prototypeManager.Index<T>(protoId.Value) : default (T);
  }
}
