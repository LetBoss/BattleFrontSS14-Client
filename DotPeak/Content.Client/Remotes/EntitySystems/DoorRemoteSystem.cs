// Decompiled with JetBrains decompiler
// Type: Content.Client.Remotes.EntitySystems.DoorRemoteSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items;
using Content.Client.Remote.UI;
using Content.Shared.Remotes.Components;
using Content.Shared.Remotes.EntitySystems;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Remotes.EntitySystems;

public sealed class DoorRemoteSystem : SharedDoorRemoteSystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.Subs.ItemStatus<DoorRemoteComponent>((Func<Entity<DoorRemoteComponent>, Control>) (ent => (Control) new DoorRemoteStatusControl(ent)));
  }
}
