// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.Prototypes.EmoteSoundsPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chat.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class EmoteSoundsPrototype : IPrototype, IInheritingPrototype
{
  [DataField("sound", false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public SoundSpecifier? FallbackSound;
  [DataField("params", false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public AudioParams? GeneralParams;
  [DataField(null, false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public Dictionary<ProtoId<EmotePrototype>, SoundSpecifier> Sounds = new Dictionary<ProtoId<EmotePrototype>, SoundSpecifier>();

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<EmoteSoundsPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [AbstractDataField(1)]
  [NeverPushInheritance]
  public bool Abstract { get; private set; }
}
