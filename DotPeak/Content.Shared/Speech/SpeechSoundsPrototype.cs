// Decompiled with JetBrains decompiler
// Type: Content.Shared.Speech.SpeechSoundsPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Speech;

[Prototype(null, 1)]
public sealed class SpeechSoundsPrototype : IPrototype
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("variation", false, 1, false, false, null)]
  public float Variation { get; set; } = 0.1f;

  [DataField("saySound", false, 1, false, false, null)]
  public SoundSpecifier SaySound { get; set; } = (SoundSpecifier) new SoundPathSpecifier("/Audio/Voice/Talk/speak_2.ogg");

  [DataField("askSound", false, 1, false, false, null)]
  public SoundSpecifier AskSound { get; set; } = (SoundSpecifier) new SoundPathSpecifier("/Audio/Voice/Talk/speak_2_ask.ogg");

  [DataField("exclaimSound", false, 1, false, false, null)]
  public SoundSpecifier ExclaimSound { get; set; } = (SoundSpecifier) new SoundPathSpecifier("/Audio/Voice/Talk/speak_2_exclaim.ogg");
}
