// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Access.IdModificationConsoleUISystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Marines.Access;
using Content.Shared._RMC14.UserInterface;
using Robust.Client.Timing;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._RMC14.Access;

public sealed class IdModificationConsoleUISystem : EntitySystem
{
  [Dependency]
  private RMCUserInterfaceSystem _rmcUI;
  [Dependency]
  private IClientGameTiming _timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IdModificationConsoleComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<IdModificationConsoleComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IdModificationConsoleComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<IdModificationConsoleComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnInserted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IdModificationConsoleComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<IdModificationConsoleComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnState(
    Entity<IdModificationConsoleComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    if (GameTick.op_Inequality(((IGameTiming) this._timing).CurTick, this._timing.LastRealTick))
      return;
    this.RefreshUIs(ent);
  }

  private void OnInserted(
    Entity<IdModificationConsoleComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    this.RefreshUIs(ent);
  }

  private void OnRemoved(
    Entity<IdModificationConsoleComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    this.RefreshUIs(ent);
  }

  private void RefreshUIs(Entity<IdModificationConsoleComponent> ent)
  {
    this._rmcUI.RefreshUIs<IdModificationConsoleBui>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner));
  }
}
