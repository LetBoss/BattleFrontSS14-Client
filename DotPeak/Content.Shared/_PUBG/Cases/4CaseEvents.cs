// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Cases.CaseOpenResultMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Cases;

[NetSerializable]
[Serializable]
public sealed class CaseOpenResultMessage : EntityEventArgs
{
  public string CaseId { get; }

  public int WinningIndex { get; }

  public List<CaseReelCellInfo> Cells { get; }

  public CaseRewardInfo Reward { get; }

  public CaseOpenResultMessage(
    string caseId,
    int winningIndex,
    List<CaseReelCellInfo> cells,
    CaseRewardInfo reward)
  {
    this.CaseId = caseId;
    this.WinningIndex = winningIndex;
    this.Cells = cells;
    this.Reward = reward;
  }
}
