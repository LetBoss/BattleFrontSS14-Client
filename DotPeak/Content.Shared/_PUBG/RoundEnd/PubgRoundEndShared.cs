// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.RoundEnd.PubgInstanceRoundEndMessageEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.RoundEnd;

[NetSerializable]
[Serializable]
public sealed class PubgInstanceRoundEndMessageEvent : EntityEventArgs
{
  public int GameInstanceId { get; }

  public string Title { get; }

  public string RoundEndText { get; }

  public List<PubgRoundEndPartyEntry> PartyEntries { get; }

  public PubgInstanceRoundEndMessageEvent(
    int gameInstanceId,
    string title,
    string roundEndText,
    List<PubgRoundEndPartyEntry>? partyEntries = null)
  {
    this.GameInstanceId = gameInstanceId;
    this.Title = title;
    this.RoundEndText = roundEndText;
    this.PartyEntries = partyEntries ?? new List<PubgRoundEndPartyEntry>();
  }
}
