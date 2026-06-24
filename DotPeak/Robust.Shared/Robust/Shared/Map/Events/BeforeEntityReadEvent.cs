// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Events.BeforeEntityReadEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Map.Events;

public sealed class BeforeEntityReadEvent
{
  public readonly HashSet<string> DeletedPrototypes = new HashSet<string>();
  public readonly Dictionary<string, string> RenamedPrototypes = new Dictionary<string, string>();
}
