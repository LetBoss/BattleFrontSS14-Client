// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.FightOrFlight.XenoFightOrFlightSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Energy;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.FightOrFlight;

public sealed class XenoFightOrFlightSystem : EntitySystem
{
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private XenoEnergySystem _energy;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private StatusEffectsSystem _status;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;
  private readonly HashSet<Entity<XenoComponent>> _xenos = new HashSet<Entity<XenoComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoFightOrFlightComponent, XenoFightOrFlightActionEvent>(new EntityEventRefHandler<XenoFightOrFlightComponent, XenoFightOrFlightActionEvent>(this.OnFightOrFlightAction));
  }

  private void OnFightOrFlightAction(
    Entity<XenoFightOrFlightComponent> xeno,
    ref XenoFightOrFlightActionEvent args)
  {
    XenoEnergyComponent comp;
    if (args.Handled || !this._rmcActions.TryUseAction((InstantActionEvent) args) || !this.TryComp<XenoEnergyComponent>((EntityUid) xeno, out comp))
      return;
    args.Handled = true;
    this._audio.PlayPredicted(xeno.Comp.RoarSound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    bool flag = this._energy.HasEnergy((Entity<XenoEnergyComponent>) (xeno.Owner, comp), xeno.Comp.FuryThreshold);
    this._xenos.Clear();
    this._entityLookup.GetEntitiesInRange<XenoComponent>(xeno.Owner.ToCoordinates(), flag ? (float) xeno.Comp.HighRange : (float) xeno.Comp.LowRange, this._xenos);
    if (this._net.IsServer)
      this.SpawnAttachedTo((string) (flag ? xeno.Comp.RoarEffect : xeno.Comp.WeakRoarEffect), xeno.Owner.ToCoordinates(), rotation: new Angle());
    foreach (Entity<XenoComponent> xeno1 in this._xenos)
    {
      if (this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) xeno1.Owner))
      {
        foreach (ProtoId<StatusEffectPrototype> key in xeno.Comp.AilmentsRemove)
          this._status.TryRemoveStatusEffect((EntityUid) xeno1, (string) key);
        this.EntityManager.RemoveComponents((EntityUid) xeno1, xeno.Comp.ComponentsRemove);
        this._jitter.DoJitter((EntityUid) xeno1, xeno.Comp.Jitter, true, 80f, 8f, true);
        if (this._net.IsServer)
        {
          this.SpawnAttachedTo((string) xeno.Comp.HealEffect, xeno1.Owner.ToCoordinates(), rotation: new Angle());
          this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-fof-effect"), (EntityUid) xeno1, (EntityUid) xeno1, PopupType.SmallCaution);
        }
      }
    }
  }
}
