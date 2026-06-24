// Decompiled with JetBrains decompiler
// Type: Content.Shared.Speech.SpeechVerbPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Speech;

[Prototype(null, 1)]
public sealed class SpeechVerbPrototype : IPrototype
{
  [DataField("speechVerbStrings", false, 1, true, false, null)]
  public List<string> SpeechVerbStrings;
  [DataField("bold", false, 1, false, false, null)]
  public bool Bold;
  [DataField("fontSize", false, 1, false, false, null)]
  public int FontSize = 12;
  [DataField("fontId", false, 1, false, false, null)]
  public string FontId = "Default";
  [DataField("priority", false, 1, false, false, null)]
  public int Priority;
  [DataField(null, false, 1, true, false, null)]
  public LocId Name = (LocId) string.Empty;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
