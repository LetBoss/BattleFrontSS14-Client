// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.GlobalMap.CivGlobalMapAlertSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._CIV14merka.GlobalMap;

public sealed class CivGlobalMapAlertSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<CivGlobalMapAlertComponent, MapInitEvent>(new EntityEventRefHandler<CivGlobalMapAlertComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<CivGlobalMapAlertComponent, ComponentRemove>(new EntityEventRefHandler<CivGlobalMapAlertComponent, ComponentRemove>(this.OnRemove));
  }

  private void OnMapInit(Entity<CivGlobalMapAlertComponent> ent, ref MapInitEvent args)
  {
    this._alerts.ShowAlert((EntityUid) ent, ent.Comp.Alert);
  }

  private void OnRemove(Entity<CivGlobalMapAlertComponent> ent, ref ComponentRemove args)
  {
    this._alerts.ClearAlert((EntityUid) ent, ent.Comp.Alert);
  }
}
