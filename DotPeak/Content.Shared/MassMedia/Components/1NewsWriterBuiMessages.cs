// Decompiled with JetBrains decompiler
// Type: Content.Shared.MassMedia.Components.NewsWriterBoundUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.MassMedia.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.MassMedia.Components;

[NetSerializable]
[Serializable]
public sealed class NewsWriterBoundUserInterfaceState : BoundUserInterfaceState
{
  public readonly NewsArticle[] Articles;
  public readonly bool PublishEnabled;
  public readonly TimeSpan NextPublish;
  public readonly string DraftTitle;
  public readonly string DraftContent;

  public NewsWriterBoundUserInterfaceState(
    NewsArticle[] articles,
    bool publishEnabled,
    TimeSpan nextPublish,
    string draftTitle,
    string draftContent)
  {
    this.Articles = articles;
    this.PublishEnabled = publishEnabled;
    this.NextPublish = nextPublish;
    this.DraftTitle = draftTitle;
    this.DraftContent = draftContent;
  }
}
