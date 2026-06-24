// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.ResolvedPathSpecifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Audio;

[NetSerializable]
[Serializable]
public sealed class ResolvedPathSpecifier : ResolvedSoundSpecifier, IEquatable<ResolvedPathSpecifier>
{
  public ResPath Path { get; private set; }

  public override string ToString() => $"ResolvedPathSpecifier({this.Path})";

  private ResolvedPathSpecifier()
  {
  }

  public ResolvedPathSpecifier(ResPath path) => this.Path = path;

  public ResolvedPathSpecifier(string path)
    : this(new ResPath(path))
  {
  }

  public bool Equals(ResolvedPathSpecifier? other) => this.Path.Equals((object) other?.Path);

  public override bool Equals(object? obj) => this.Equals(obj as ResolvedPathSpecifier);

  public override int GetHashCode() => this.Path.GetHashCode();
}
