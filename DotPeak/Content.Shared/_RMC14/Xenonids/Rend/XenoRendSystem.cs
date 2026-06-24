// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Rend.XenoRendSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Rend;

public sealed class XenoRendSystem : EntitySystem
{
  [Dependency]
  private SharedRMCActionsSystem _actions;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedInteractionSystem _interact;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAudioSystem _audio;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoRendComponent, XenoRendActionEvent>(new EntityEventRefHandler<XenoRendComponent, XenoRendActionEvent>(this.OnXenoRendAction));
  }

  private void OnXenoRendAction(Entity<XenoRendComponent> xeno, ref XenoRendActionEvent args)
  {
    if (args.Handled || !this._actions.TryUseAction((InstantActionEvent) args))
      return;
    args.Handled = true;
    this.EnsureComp<XenoSweepingComponent>((EntityUid) xeno);
    this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.HissEmote);
    foreach (Entity<MobStateComponent> entity in this._entityLookup.GetEntitiesInRange<MobStateComponent>(this._transform.GetMapCoordinates((EntityUid) xeno), xeno.Comp.Range))
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) entity) && this._interact.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) entity.Owner, xeno.Comp.Range))
      {
        FixedPoint2? total = this._damage.TryChangeDamage(new EntityUid?((EntityUid) entity), xeno.Comp.Damage, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
        FixedPoint2 zero = FixedPoint2.Zero;
        if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
        {
          Filter filter1 = Filter.Pvs((EntityUid) entity, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
          SharedColorFlashEffectSystem colorFlash = this._colorFlash;
          Color red = Color.Red;
          List<EntityUid> entities = new List<EntityUid>();
          entities.Add((EntityUid) entity);
          Filter filter2 = filter1;
          colorFlash.RaiseEffect(red, entities, filter2);
        }
        if (this._net.IsServer)
          this.SpawnAttachedTo((string) xeno.Comp.Effect, entity.Owner.ToCoordinates(), rotation: new Angle());
        this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      }
    }
  }
}
