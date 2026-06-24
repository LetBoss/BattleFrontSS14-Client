// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.LocId
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Localization;

[NetSerializable]
[Serializable]
public readonly record struct LocId(string Id) : IEquatable<string>, IComparable<LocId>
{
  public static implicit operator string(LocId locId) => locId.Id;

  public static implicit operator LocId(string id) => new LocId(id);

  public static implicit operator LocId?(string? id)
  {
    return id != null ? new LocId?(new LocId(id)) : new LocId?();
  }

  public bool Equals(string? other) => this.Id == other;

  public int CompareTo(LocId other) => string.Compare(this.Id, other.Id, StringComparison.Ordinal);

  public override string ToString() => this.Id ?? string.Empty;
}
