// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.TacticalMap.TacMapXenoAlertSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.TacticalMap;

public sealed class TacMapXenoAlertSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<TacMapXenoAlertComponent, MapInitEvent>(new EntityEventRefHandler<TacMapXenoAlertComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<TacMapXenoAlertComponent, ComponentRemove>(new EntityEventRefHandler<TacMapXenoAlertComponent, ComponentRemove>(this.OnRemove));
  }

  private void OnMapInit(Entity<TacMapXenoAlertComponent> ent, ref MapInitEvent args)
  {
    this._alerts.ShowAlert((EntityUid) ent, ent.Comp.Alert);
  }

  private void OnRemove(Entity<TacMapXenoAlertComponent> ent, ref ComponentRemove args)
  {
    this._alerts.ClearAlert((EntityUid) ent, ent.Comp.Alert);
  }
}
