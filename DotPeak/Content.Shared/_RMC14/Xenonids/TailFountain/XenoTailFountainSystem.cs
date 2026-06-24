// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.TailFountain.XenoTailFountainSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared.ActionBlocker;
using Content.Shared.Atmos.Components;
using Content.Shared.Coordinates;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.TailFountain;

public sealed class XenoTailFountainSystem : EntitySystem
{
  [Dependency]
  private SharedRMCFlammableSystem _flame;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoTailFountainComponent, XenoTailFountainActionEvent>(new EntityEventRefHandler<XenoTailFountainComponent, XenoTailFountainActionEvent>(this.OnTailFountainAction));
  }

  private void OnTailFountainAction(
    Entity<XenoTailFountainComponent> xeno,
    ref XenoTailFountainActionEvent args)
  {
    if (args.Handled || !this._actionBlocker.CanAttack((EntityUid) xeno))
      return;
    if (xeno.Owner == args.Target)
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tail-fountain-fail-self"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    else if (!this.HasComp<MobStateComponent>(args.Target))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tail-fountain-fail"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    }
    else
    {
      args.Handled = true;
      this._flame.Extinguish((Entity<FlammableComponent>) args.Target);
      this._audio.PlayPredicted(xeno.Comp.ExtinguishSound, args.Target, new EntityUid?((EntityUid) xeno));
      this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-tail-fountain-self", ("target", (object) args.Target)), this.Loc.GetString("rmc-xeno-tail-fountain-others", ("user", (object) xeno), ("target", (object) args.Target)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
      if (this._net.IsServer)
        this.SpawnAttachedTo((string) xeno.Comp.Acid, args.Target.ToCoordinates(), rotation: new Angle());
      MeleeWeaponComponent comp;
      if (this.TryComp<MeleeWeaponComponent>((EntityUid) xeno, out comp))
      {
        if (this._timing.CurTime < comp.NextAttack)
          return;
        comp.NextAttack = this._timing.CurTime + TimeSpan.FromSeconds(1L);
        this.Dirty((EntityUid) xeno, (IComponent) comp);
      }
      MeleeAttackEvent args1 = new MeleeAttackEvent((EntityUid) xeno);
      this.RaiseLocalEvent<MeleeAttackEvent>((EntityUid) xeno, ref args1);
    }
  }
}
