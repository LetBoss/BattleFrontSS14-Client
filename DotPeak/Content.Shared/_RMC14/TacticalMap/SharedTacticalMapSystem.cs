// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.SharedTacticalMapSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

public abstract class SharedTacticalMapSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedUserInterfaceSystem _ui;

  public int LineLimit { get; private set; }

  public override void Initialize()
  {
    this.SubscribeLocalEvent<TacticalMapUserComponent, OpenTacticalMapActionEvent>(new EntityEventRefHandler<TacticalMapUserComponent, OpenTacticalMapActionEvent>(this.OnUserOpenAction));
    this.SubscribeLocalEvent<TacticalMapUserComponent, OpenTacMapAlertEvent>(new EntityEventRefHandler<TacticalMapUserComponent, OpenTacMapAlertEvent>(this.OnUserOpenAlert));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCTacticalMapLineLimit, (Action<int>) (v => this.LineLimit = v), true);
  }

  private void OnUserOpenAction(
    Entity<TacticalMapUserComponent> ent,
    ref OpenTacticalMapActionEvent args)
  {
    Entity<TacticalMapComponent> map;
    if (this.TryGetTacticalMap(out map))
      this.UpdateUserData(ent, (TacticalMapComponent) map);
    this.ToggleMapUI(ent);
  }

  private void OnUserOpenAlert(Entity<TacticalMapUserComponent> ent, ref OpenTacMapAlertEvent args)
  {
    Entity<TacticalMapComponent> map;
    if (this.TryGetTacticalMap(out map))
      this.UpdateUserData(ent, (TacticalMapComponent) map);
    this.ToggleMapUI(ent);
  }

  public bool TryGetTacticalMap(out Entity<TacticalMapComponent> map)
  {
    EntityUid uid;
    TacticalMapComponent comp1;
    if (this.EntityQueryEnumerator<TacticalMapComponent>().MoveNext(out uid, out comp1))
    {
      map = (Entity<TacticalMapComponent>) (uid, comp1);
      return true;
    }
    map = new Entity<TacticalMapComponent>();
    return false;
  }

  protected void UpdateMapData(Entity<TacticalMapComputerComponent> computer)
  {
    Entity<TacticalMapComponent> map;
    if (!this.TryGetTacticalMap(out map))
      return;
    this.UpdateMapData(computer, (TacticalMapComponent) map);
  }

  protected virtual void UpdateMapData(
    Entity<TacticalMapComputerComponent> computer,
    TacticalMapComponent map)
  {
    TacticalMapIncludeXenosEvent message = new TacticalMapIncludeXenosEvent();
    this.RaiseLocalEvent<TacticalMapIncludeXenosEvent>(ref message);
    if (message.Include)
    {
      computer.Comp.Blips = new Dictionary<int, TacticalMapBlip>((IDictionary<int, TacticalMapBlip>) map.MarineBlips);
      foreach (KeyValuePair<int, TacticalMapBlip> xenoBlip in map.XenoBlips)
        computer.Comp.Blips.TryAdd(xenoBlip.Key, xenoBlip.Value);
    }
    else
      computer.Comp.Blips = map.MarineBlips;
    this.Dirty<TacticalMapComputerComponent>(computer);
    TacticalMapLinesComponent mapLinesComponent = this.EnsureComp<TacticalMapLinesComponent>((EntityUid) computer);
    mapLinesComponent.MarineLines = map.MarineLines;
    this.Dirty((EntityUid) computer, (IComponent) mapLinesComponent);
  }

  public virtual void OpenComputerMap(Entity<TacticalMapComputerComponent?> computer, EntityUid user)
  {
    if (!this.Resolve<TacticalMapComputerComponent>((EntityUid) computer, ref computer.Comp, false))
      return;
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) computer.Owner, (Enum) TacticalMapComputerUi.Key, user);
    this.UpdateMapData((Entity<TacticalMapComputerComponent>) ((EntityUid) computer, computer.Comp));
  }

  public virtual void UpdateUserData(
    Entity<TacticalMapUserComponent> user,
    TacticalMapComponent map)
  {
  }

  private void ToggleMapUI(Entity<TacticalMapUserComponent> user)
  {
    if (this._ui.IsUiOpen((Entity<UserInterfaceComponent>) user.Owner, (Enum) TacticalMapUserUi.Key, (EntityUid) user))
      this._ui.CloseUi((Entity<UserInterfaceComponent>) user.Owner, (Enum) TacticalMapUserUi.Key, new EntityUid?((EntityUid) user));
    else
      this._ui.TryOpenUi((Entity<UserInterfaceComponent>) user.Owner, (Enum) TacticalMapUserUi.Key, (EntityUid) user);
  }
}
