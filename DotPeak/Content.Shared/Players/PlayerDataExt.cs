// Decompiled with JetBrains decompiler
// Type: Content.Shared.Players.PlayerDataExt
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared.Players;

public static class PlayerDataExt
{
  public static ContentPlayerData? ContentData(this SessionData data)
  {
    return (ContentPlayerData) data.ContentDataUncast;
  }

  public static ContentPlayerData? ContentData(this ICommonSession session)
  {
    return session.Data.ContentData();
  }

  public static EntityUid? GetMind(this ICommonSession session) => session.Data.ContentData()?.Mind;
}
