// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Cases.CaseRewardInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Cases;

[NetSerializable]
[Serializable]
public sealed class CaseRewardInfo
{
  public CaseRewardKind Kind { get; }

  public string? ItemId { get; }

  public int? Amount { get; }

  public bool IsDuplicateRecipe { get; }

  public int? DuplicateCompensationScrap { get; }

  public CaseRewardInfo(
    CaseRewardKind kind,
    string? itemId = null,
    int? amount = null,
    bool isDuplicateRecipe = false,
    int? duplicateCompensationScrap = null)
  {
    this.Kind = kind;
    this.ItemId = itemId;
    this.Amount = amount;
    this.IsDuplicateRecipe = isDuplicateRecipe;
    this.DuplicateCompensationScrap = duplicateCompensationScrap;
  }
}
