// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.UserInterface.RMCUserInterfaceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.UserInterface;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.UserInterface;

public sealed class RMCUserInterfaceSystem : EntitySystem
{
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  private readonly List<(Entity<UserInterfaceComponent?> Ent, Action<Entity<UserInterfaceComponent?>, RMCUserInterfaceSystem> Act)> _toRefresh = new List<(Entity<UserInterfaceComponent>, Action<Entity<UserInterfaceComponent>, RMCUserInterfaceSystem>)>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<ActivatableUIBlacklistComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<ActivatableUIBlacklistComponent, ActivatableUIOpenAttemptEvent>(this.OnUIBlacklistAttempt));
    this.SubscribeLocalEvent<UserBlacklistActivatableUIComponent, UserOpenActivatableUIAttemptEvent>(new EntityEventRefHandler<UserBlacklistActivatableUIComponent, UserOpenActivatableUIAttemptEvent>(this.OnUIBlacklistUserAttempt));
  }

  private void OnUIBlacklistAttempt(
    Entity<ActivatableUIBlacklistComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled || this.CanOpenUI((Entity<ActivatableUIBlacklistComponent>) ((EntityUid) ent, (ActivatableUIBlacklistComponent) ent), (Entity<UserBlacklistActivatableUIComponent>) args.User, (Enum) null))
      return;
    args.Cancel();
  }

  private void OnUIBlacklistUserAttempt(
    Entity<UserBlacklistActivatableUIComponent> ent,
    ref UserOpenActivatableUIAttemptEvent args)
  {
    ActivatableUIComponent comp;
    if (args.Cancelled || !this.TryComp<ActivatableUIComponent>(args.Target, out comp))
      return;
    Enum key = comp.Key;
    if (key == null || !ent.Comp.Keys.Contains(key))
      return;
    args.Cancel();
  }

  public bool CanOpenUI(
    Entity<ActivatableUIBlacklistComponent?> ent,
    Entity<UserBlacklistActivatableUIComponent?> user,
    Enum? key)
  {
    return (!this.Resolve<ActivatableUIBlacklistComponent>((EntityUid) ent, ref ent.Comp, false) || !this._whitelist.IsBlacklistPass(ent.Comp.Blacklist, (EntityUid) user)) && (key == null || !this.Resolve<UserBlacklistActivatableUIComponent>((EntityUid) user, ref user.Comp, false) || !user.Comp.Keys.Contains(key));
  }

  public void RefreshUIs<T>(Entity<UserInterfaceComponent?> uiEnt1) where T : BoundUserInterface, IRefreshableBui
  {
    this._toRefresh.Add((uiEnt1, (Action<Entity<UserInterfaceComponent>, RMCUserInterfaceSystem>) ((uiEnt2, system) =>
    {
      try
      {
        if (system.TerminatingOrDeleted((EntityUid) uiEnt2) || !system.Resolve<UserInterfaceComponent>((EntityUid) uiEnt2, ref uiEnt2.Comp))
          return;
        foreach (BoundUserInterface boundUserInterface in uiEnt2.Comp.ClientOpenInterfaces.Values)
        {
          if (boundUserInterface is T obj2)
            obj2.Refresh();
        }
      }
      catch (Exception ex)
      {
        system.Log.Error($"Error refreshing {nameof (T)}\n{ex}");
      }
    })));
  }

  public void TryBui<T>(Entity<UserInterfaceComponent?> ent, Action<T> action) where T : BoundUserInterface
  {
    try
    {
      if (!this.Resolve<UserInterfaceComponent>((EntityUid) ent, ref ent.Comp, false))
        return;
      foreach (BoundUserInterface boundUserInterface in ent.Comp.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is T obj)
          action(obj);
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error getting {nameof (T)}:\n{ex}");
    }
  }

  public override void Update(float frameTime)
  {
    try
    {
      foreach ((Entity<UserInterfaceComponent> Ent, Action<Entity<UserInterfaceComponent>, RMCUserInterfaceSystem> Act) tuple in this._toRefresh)
        tuple.Act(tuple.Ent, this);
    }
    finally
    {
      this._toRefresh.Clear();
    }
  }
}
