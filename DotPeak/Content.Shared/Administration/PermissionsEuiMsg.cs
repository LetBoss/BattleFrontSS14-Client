// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.PermissionsEuiMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Administration;

public static class PermissionsEuiMsg
{
  [NetSerializable]
  [Serializable]
  public sealed class AddAdmin : EuiMessageBase
  {
    public string UserNameOrId = string.Empty;
    public string? Title;
    public AdminFlags PosFlags;
    public AdminFlags NegFlags;
    public int? RankId;
    public bool Suspended;
  }

  [NetSerializable]
  [Serializable]
  public sealed class RemoveAdmin : EuiMessageBase
  {
    public NetUserId UserId;
  }

  [NetSerializable]
  [Serializable]
  public sealed class UpdateAdmin : EuiMessageBase
  {
    public NetUserId UserId;
    public string? Title;
    public AdminFlags PosFlags;
    public AdminFlags NegFlags;
    public int? RankId;
    public bool Suspended;
  }

  [NetSerializable]
  [Serializable]
  public sealed class AddAdminRank : EuiMessageBase
  {
    public string Name = string.Empty;
    public AdminFlags Flags;
  }

  [NetSerializable]
  [Serializable]
  public sealed class RemoveAdminRank : EuiMessageBase
  {
    public int Id;
  }

  [NetSerializable]
  [Serializable]
  public sealed class UpdateAdminRank : EuiMessageBase
  {
    public int Id;
    public string Name = string.Empty;
    public AdminFlags Flags;
  }
}
