// Decompiled with JetBrains decompiler
// Type: Content.Shared.MassMedia.Systems.NewsArticle
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.MassMedia.Systems;

[NetSerializable]
[Serializable]
public struct NewsArticle
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string Title;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string Content;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string? Author;
  [Robust.Shared.ViewVariables.ViewVariables]
  public ICollection<(NetEntity, uint)>? AuthorStationRecordKeyIds;
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan ShareTime;
}
