// Decompiled with JetBrains decompiler
// Type: Content.Client.Players.RateLimiting.PlayerRateLimitManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Players.RateLimiting;
using Robust.Shared.Player;

#nullable enable
namespace Content.Client.Players.RateLimiting;

public sealed class PlayerRateLimitManager : SharedPlayerRateLimitManager
{
  public override RateLimitStatus CountAction(ICommonSession player, string key)
  {
    return RateLimitStatus.Allowed;
  }

  public override void Register(string key, RateLimitRegistration registration)
  {
  }

  public override void Initialize()
  {
  }
}
