// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Systems.ActionGunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class ActionGunSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedGunSystem _gun;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ActionGunComponent, MapInitEvent>(new EntityEventRefHandler<ActionGunComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ActionGunComponent, ComponentShutdown>(new EntityEventRefHandler<ActionGunComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<ActionGunComponent, ActionGunShootEvent>(new EntityEventRefHandler<ActionGunComponent, ActionGunShootEvent>(this.OnShoot));
  }

  private void OnMapInit(Entity<ActionGunComponent> ent, ref MapInitEvent args)
  {
    if (string.IsNullOrEmpty((string) ent.Comp.Action))
      return;
    this._actions.AddAction((EntityUid) ent, ref ent.Comp.ActionEntity, (string) ent.Comp.Action);
    ent.Comp.Gun = new EntityUid?(this.Spawn((string) ent.Comp.GunProto));
  }

  private void OnShutdown(Entity<ActionGunComponent> ent, ref ComponentShutdown args)
  {
    EntityUid? gun = ent.Comp.Gun;
    if (!gun.HasValue)
      return;
    this.QueueDel(new EntityUid?(gun.GetValueOrDefault()));
  }

  private void OnShoot(Entity<ActionGunComponent> ent, ref ActionGunShootEvent args)
  {
    GunComponent comp;
    if (!this.TryComp<GunComponent>(ent.Comp.Gun, out comp))
      return;
    this._gun.AttemptShoot((EntityUid) ent, ent.Comp.Gun.Value, comp, args.Target);
  }
}
