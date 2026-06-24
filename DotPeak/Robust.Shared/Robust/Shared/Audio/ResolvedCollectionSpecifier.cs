// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.ResolvedCollectionSpecifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Audio;

[NetSerializable]
[Serializable]
public sealed class ResolvedCollectionSpecifier : 
  ResolvedSoundSpecifier,
  IEquatable<ResolvedCollectionSpecifier>
{
  public ProtoId<SoundCollectionPrototype>? Collection { get; private set; }

  public int Index { get; private set; }

  public override string ToString()
  {
    return $"ResolvedCollectionSpecifier({this.Collection}, {this.Index})";
  }

  private ResolvedCollectionSpecifier()
  {
  }

  public ResolvedCollectionSpecifier(string collection, int index)
  {
    this.Collection = (ProtoId<SoundCollectionPrototype>?) collection;
    this.Index = index;
  }

  public bool Equals(ResolvedCollectionSpecifier? other)
  {
    return this.Collection.Equals((object) (ProtoId<SoundCollectionPrototype>?) other?.Collection) && this.Index.Equals((object) other?.Index);
  }

  public override bool Equals(object? obj) => this.Equals(obj as ResolvedCollectionSpecifier);

  public override int GetHashCode()
  {
    return HashCode.Combine<ProtoId<SoundCollectionPrototype>?, int>(this.Collection, this.Index);
  }
}
