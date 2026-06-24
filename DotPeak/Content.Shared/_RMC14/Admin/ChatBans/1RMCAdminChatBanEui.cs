// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Admin.ChatBans.RMCAdminChatBanAddMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._RMC14.Admin.ChatBans;

[NetSerializable]
[Serializable]
public sealed class RMCAdminChatBanAddMsg(
  string target,
  ChatType type,
  TimeSpan? duration,
  string reason) : EuiMessageBase
{
  public readonly string Target = target;
  public readonly ChatType Type = type;
  public readonly TimeSpan? Duration = duration;
  public readonly string Reason = reason;
}
