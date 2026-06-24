// Decompiled with JetBrains decompiler
// Type: Content.Shared.Players.ContentPlayerData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.GameTicking;
using Content.Shared.Mind;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Shared.Players;

public sealed class ContentPlayerData
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public NetUserId UserId { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string Name { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (SharedMindSystem), typeof (SharedGameTicker)})]
  public EntityUid? Mind { get; set; }

  public bool Stealthed { get; set; }

  public ContentPlayerData(NetUserId userId, string name)
  {
    this.UserId = userId;
    this.Name = name;
  }
}
