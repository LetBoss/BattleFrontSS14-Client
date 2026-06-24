// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Admin.NameRestriction.PubgAdminNameRestrictionState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Admin.NameRestriction;

[NetSerializable]
[Serializable]
public sealed class PubgAdminNameRestrictionState(
  string playerCkey,
  bool isRestricted,
  bool canEdit,
  string? changedByCkey,
  DateTime? changedAtUtc) : EuiStateBase
{
  public readonly string PlayerCkey = playerCkey;
  public readonly bool IsRestricted = isRestricted;
  public readonly bool CanEdit = canEdit;
  public readonly string? ChangedByCkey = changedByCkey;
  public readonly DateTime? ChangedAtUtc = changedAtUtc;
}
