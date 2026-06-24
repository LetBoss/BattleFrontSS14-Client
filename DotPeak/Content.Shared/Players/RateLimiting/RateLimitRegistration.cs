// Decompiled with JetBrains decompiler
// Type: Content.Shared.Players.RateLimiting.RateLimitRegistration
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Shared.Players.RateLimiting;

public sealed class RateLimitRegistration(
  CVarDef<float> cVarLimitPeriodLength,
  CVarDef<int> cVarLimitCount,
  Action<ICommonSession>? playerLimitedAction,
  CVarDef<int>? cVarAdminAnnounceDelay = null,
  Action<ICommonSession>? adminAnnounceAction = null,
  LogType adminLogType = LogType.RateLimited)
{
  public readonly CVarDef<float> CVarLimitPeriodLength = cVarLimitPeriodLength;
  public readonly CVarDef<int> CVarLimitCount = cVarLimitCount;
  public readonly Action<ICommonSession>? PlayerLimitedAction = playerLimitedAction;
  public readonly CVarDef<int>? CVarAdminAnnounceDelay = cVarAdminAnnounceDelay;
  public readonly Action<ICommonSession>? AdminAnnounceAction = adminAnnounceAction;
  public readonly LogType AdminLogType = adminLogType;
}
