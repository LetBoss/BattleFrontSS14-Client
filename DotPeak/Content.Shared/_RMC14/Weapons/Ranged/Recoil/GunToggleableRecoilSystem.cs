// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Recoil.GunToggleableRecoilSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged.Battery;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Recoil;

public sealed class GunToggleableRecoilSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private RMCGunBatterySystem _gunBattery;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GunToggleableRecoilComponent, GetItemActionsEvent>(new EntityEventRefHandler<GunToggleableRecoilComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<GunToggleableRecoilComponent, GunToggleRecoilActionEvent>(new EntityEventRefHandler<GunToggleableRecoilComponent, GunToggleRecoilActionEvent>(this.OnToggleRecoil));
    this.SubscribeLocalEvent<GunToggleableRecoilComponent, GunGetBatteryDrainEvent>(new EntityEventRefHandler<GunToggleableRecoilComponent, GunGetBatteryDrainEvent>(this.OnGetBatteryDrain));
    this.SubscribeLocalEvent<GunToggleableRecoilComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunToggleableRecoilComponent, GunRefreshModifiersEvent>(this.OnRefreshModifiers));
    this.SubscribeLocalEvent<GunToggleableRecoilComponent, GunUnpoweredEvent>(new EntityEventRefHandler<GunToggleableRecoilComponent, GunUnpoweredEvent>(this.OnGunUnpowered));
  }

  private void OnGetItemActions(
    Entity<GunToggleableRecoilComponent> ent,
    ref GetItemActionsEvent args)
  {
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
    this.Dirty<GunToggleableRecoilComponent>(ent);
  }

  private void OnToggleRecoil(
    Entity<GunToggleableRecoilComponent> ent,
    ref GunToggleRecoilActionEvent args)
  {
    args.Handled = true;
    ent.Comp.Active = !ent.Comp.Active;
    this.ActiveChanged(ent, new EntityUid?(args.Performer));
  }

  private void OnGetBatteryDrain(
    Entity<GunToggleableRecoilComponent> ent,
    ref GunGetBatteryDrainEvent args)
  {
    if (!ent.Comp.Active)
      return;
    args.Drain += ent.Comp.BatteryDrain;
  }

  private void OnRefreshModifiers(
    Entity<GunToggleableRecoilComponent> ent,
    ref GunRefreshModifiersEvent args)
  {
    if (!ent.Comp.Active)
      return;
    args.MinAngle = Angle.Zero;
    args.MaxAngle = Angle.Zero;
    args.CameraRecoilScalar = 0.0f;
  }

  private void OnGunUnpowered(Entity<GunToggleableRecoilComponent> ent, ref GunUnpoweredEvent args)
  {
    if (!ent.Comp.Active)
      return;
    ent.Comp.Active = false;
    this.ActiveChanged(ent, new EntityUid?());
  }

  private void ActiveChanged(Entity<GunToggleableRecoilComponent> ent, EntityUid? user)
  {
    this.Dirty<GunToggleableRecoilComponent>(ent);
    SharedActionsSystem actions = this._actions;
    EntityUid? action1 = ent.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = ent.Comp.Active ? 1 : 0;
    actions.SetToggled(action2, num != 0);
    this._gunBattery.RefreshBatteryDrain((Entity<GunDrainBatteryOnShootComponent>) ent.Owner);
    this._gun.RefreshModifiers((Entity<GunComponent>) ent.Owner);
    this._audio.PlayPredicted(ent.Comp.ToggleSound, ent.Owner, user);
    if (!user.HasValue)
      return;
    this._popup.PopupClient(ent.Comp.Active ? this.Loc.GetString("rmc-toggleable-recoil-compensation-on", ("gun", (object) ent.Owner)) : this.Loc.GetString("rmc-toggleable-recoil-compensation-off", ("gun", (object) ent.Owner)), user.Value, new EntityUid?(user.Value), PopupType.Large);
  }
}
