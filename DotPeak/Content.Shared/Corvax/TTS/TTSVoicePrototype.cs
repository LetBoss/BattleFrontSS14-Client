// Decompiled with JetBrains decompiler
// Type: Content.Shared.Corvax.TTS.TTSVoicePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Corvax.TTS;

[Prototype("ttsVoice", 1)]
public sealed class TTSVoicePrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("name", false, 1, false, false, null)]
  public string Name { get; private set; } = string.Empty;

  [DataField("sex", false, 1, true, false, null)]
  public Sex Sex { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("speaker", false, 1, true, false, null)]
  public string Speaker { get; private set; } = string.Empty;

  [DataField("roundStart", false, 1, false, false, null)]
  public bool RoundStart { get; private set; } = true;

  [DataField("sponsorOnly", false, 1, false, false, null)]
  public bool SponsorOnly { get; private set; }

  [DataField("requiresPermission", false, 1, false, false, null)]
  public string RequiresPermission { get; private set; } = string.Empty;
}
