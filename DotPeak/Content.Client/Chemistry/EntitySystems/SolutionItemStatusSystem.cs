// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.EntitySystems.SolutionItemStatusSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Chemistry.Components;
using Content.Client.Chemistry.UI;
using Content.Client.Items;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Chemistry.EntitySystems;

public sealed class SolutionItemStatusSystem : EntitySystem
{
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainerSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    this.Subs.ItemStatus<SolutionItemStatusComponent>((Func<Entity<SolutionItemStatusComponent>, Control>) (entity => (Control) new SolutionStatusControl(entity, (IEntityManager) this.EntityManager, this._solutionContainerSystem)));
  }
}
