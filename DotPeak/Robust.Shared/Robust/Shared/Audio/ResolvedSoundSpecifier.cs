// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.ResolvedSoundSpecifier
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Audio;

[NetSerializable]
[Serializable]
public abstract class ResolvedSoundSpecifier
{
  [Obsolete("String literals for sounds are deprecated, use a SoundSpecifier or ResolvedSoundSpecifier as appropriate instead")]
  public static implicit operator ResolvedSoundSpecifier(string s)
  {
    return (ResolvedSoundSpecifier) new ResolvedPathSpecifier(s);
  }

  [Obsolete("String literals for sounds are deprecated, use a SoundSpecifier or ResolvedSoundSpecifier as appropriate instead")]
  public static implicit operator ResolvedSoundSpecifier(ResPath s)
  {
    return (ResolvedSoundSpecifier) new ResolvedPathSpecifier(s);
  }

  public static bool IsNullOrEmpty(ResolvedSoundSpecifier? s)
  {
    switch (s)
    {
      case null:
        return true;
      case ResolvedPathSpecifier resolvedPathSpecifier:
        return resolvedPathSpecifier.Path.ToString() == "";
      case ResolvedCollectionSpecifier collectionSpecifier:
        ProtoId<SoundCollectionPrototype>? collection = collectionSpecifier.Collection;
        return string.IsNullOrEmpty(collection.HasValue ? (string) collection.GetValueOrDefault() : (string) null);
      default:
        throw new ArgumentOutOfRangeException(nameof (s), (object) s, "argument is not a ResolvedPathSpecifier or a ResolvedCollectionSpecifier");
    }
  }
}
