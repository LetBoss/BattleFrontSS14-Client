// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.VVListPathOptions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Robust.Shared.ViewVariables;

[NetSerializable]
[Serializable]
public readonly struct VVListPathOptions
{
  public VVAccess MinimumAccess { get; init; }

  public bool ListIndexers { get; init; }

  public int RemoteListLength { get; init; }

  public VVListPathOptions()
  {
    this.MinimumAccess = VVAccess.ReadOnly;
    this.ListIndexers = true;
    this.RemoteListLength = 500;
  }
}
