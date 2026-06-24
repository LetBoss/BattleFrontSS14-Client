// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.Hypospray.RMCHypospraySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items;
using Content.Shared._RMC14.Chemistry;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Medical.Hypospray;

public sealed class RMCHypospraySystem : RMCSharedHypospraySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.Subs.ItemStatus<RMCHyposprayComponent>((Func<Entity<RMCHyposprayComponent>, Control>) (ent => (Control) new RMCHyposprayStatusControl(ent, this._solution, this._container)));
  }
}
