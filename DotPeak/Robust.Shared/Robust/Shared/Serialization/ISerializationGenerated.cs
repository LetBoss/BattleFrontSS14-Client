// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.ISerializationGenerated
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization.Manager;
using System;

#nullable enable
namespace Robust.Shared.Serialization;

public interface ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null);
}
