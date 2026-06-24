// Decompiled with JetBrains decompiler
// Type: Content.Shared.Players.RateLimiting.SharedPlayerRateLimitManager
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Player;

#nullable enable
namespace Content.Shared.Players.RateLimiting;

public abstract class SharedPlayerRateLimitManager
{
  public abstract RateLimitStatus CountAction(ICommonSession player, string key);

  public abstract void Register(string key, RateLimitRegistration registration);

  public abstract void Initialize();
}
