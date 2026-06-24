// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Laser.GunToggleableLaserSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Laser;

public sealed class GunToggleableLaserSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedActionsSystem _actions;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GunToggleableLaserComponent, GetItemActionsEvent>(new EntityEventRefHandler<GunToggleableLaserComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<GunToggleableLaserComponent, GunToggleLaserActionEvent>(new EntityEventRefHandler<GunToggleableLaserComponent, GunToggleLaserActionEvent>(this.OnToggleLaser));
  }

  private void OnGetItemActions(
    Entity<GunToggleableLaserComponent> ent,
    ref GetItemActionsEvent args)
  {
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
    this.Dirty(ent.Owner, (IComponent) ent.Comp);
  }

  private void OnToggleLaser(
    Entity<GunToggleableLaserComponent> ent,
    ref GunToggleLaserActionEvent args)
  {
    if (args.Handled || !this.ToggleLaser(ent, args.Performer))
      return;
    args.Handled = true;
  }

  private bool ToggleLaser(Entity<GunToggleableLaserComponent> ent, EntityUid user)
  {
    this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(user));
    ent.Comp.Active = !ent.Comp.Active;
    if (ent.Comp.Settings.Count == 0)
      return false;
    ref int local = ref ent.Comp.Setting;
    ++local;
    if (local >= ent.Comp.Settings.Count)
      local = 0;
    GunToggleableLaserSetting setting = ent.Comp.Settings[local];
    EntityUid? action = ent.Comp.Action;
    if (action.HasValue)
      this._actions.SetIcon((Entity<ActionComponent>) action.GetValueOrDefault(), (SpriteSpecifier) setting.Icon);
    this.Dirty<GunToggleableLaserComponent>(ent);
    return true;
  }
}
