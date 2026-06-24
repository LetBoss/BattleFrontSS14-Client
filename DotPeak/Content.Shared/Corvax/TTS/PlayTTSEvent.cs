// Decompiled with JetBrains decompiler
// Type: Content.Shared.Corvax.TTS.PlayTTSEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Corvax.TTS;

[NetSerializable]
[Serializable]
public sealed class PlayTTSEvent : EntityEventArgs
{
  public byte[] Data { get; }

  public NetEntity? SourceUid { get; }

  public bool IsWhisper { get; }

  public bool IsRadio { get; }

  public bool IsLexiconSound { get; }

  public string LanguageId { get; }

  public PlayTTSEvent(
    byte[] data,
    NetEntity? sourceUid = null,
    bool isWhisper = false,
    bool isRadio = false,
    bool isSoundLexicon = false,
    string languageId = "")
  {
    this.Data = data;
    this.SourceUid = sourceUid;
    this.IsWhisper = isWhisper;
    this.IsRadio = isRadio;
    this.IsLexiconSound = isSoundLexicon;
    this.LanguageId = languageId;
  }
}
