// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.Cartridges.NewsReaderBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.MassMedia.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.CartridgeLoader.Cartridges;

[NetSerializable]
[Serializable]
public sealed class NewsReaderBoundUserInterfaceState : BoundUserInterfaceState
{
  public NewsArticle Article;
  public int TargetNum;
  public int TotalNum;
  public bool NotificationOn;

  public NewsReaderBoundUserInterfaceState(
    NewsArticle article,
    int targetNum,
    int totalNum,
    bool notificationOn)
  {
    this.Article = article;
    this.TargetNum = targetNum;
    this.TotalNum = totalNum;
    this.NotificationOn = notificationOn;
  }
}
