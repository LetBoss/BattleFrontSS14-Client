// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Auto.GunToggleableAutoFireSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged.Battery;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Auto;

public sealed class GunToggleableAutoFireSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private CMGunSystem _rmcGun;
  [Dependency]
  private RMCGunBatterySystem _rmcGunBattery;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly HashSet<Entity<XenoComponent>> _targets = new HashSet<Entity<XenoComponent>>();
  public readonly PolygonShape Shape = new PolygonShape();
  public bool Debug;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GunToggleableAutoFireComponent, GetItemActionsEvent>(new EntityEventRefHandler<GunToggleableAutoFireComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<GunToggleableAutoFireComponent, GunToggleableAutoFireActionEvent>(new EntityEventRefHandler<GunToggleableAutoFireComponent, GunToggleableAutoFireActionEvent>(this.OnAutoFireAction));
    this.SubscribeLocalEvent<GunToggleableAutoFireComponent, GunGetBatteryDrainEvent>(new EntityEventRefHandler<GunToggleableAutoFireComponent, GunGetBatteryDrainEvent>(this.OnAutoFireGetBatteryDrain));
    this.SubscribeLocalEvent<ActiveGunAutoFireComponent, ComponentRemove>(new EntityEventRefHandler<ActiveGunAutoFireComponent, ComponentRemove>(this.OnRemove));
    this.SubscribeLocalEvent<ActiveGunAutoFireComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<ActiveGunAutoFireComponent, ItemUnwieldedEvent>(this.OnDoRemove<ItemUnwieldedEvent>));
    this.SubscribeLocalEvent<ActiveGunAutoFireComponent, DroppedEvent>(new EntityEventRefHandler<ActiveGunAutoFireComponent, DroppedEvent>(this.OnDoRemove<DroppedEvent>));
    this.SubscribeLocalEvent<ActiveGunAutoFireComponent, GunUnpoweredEvent>(new EntityEventRefHandler<ActiveGunAutoFireComponent, GunUnpoweredEvent>(this.OnDoRemove<GunUnpoweredEvent>));
  }

  private void OnGetItemActions(
    Entity<GunToggleableAutoFireComponent> ent,
    ref GetItemActionsEvent args)
  {
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
    this.Dirty<GunToggleableAutoFireComponent>(ent);
  }

  private void OnAutoFireAction(
    Entity<GunToggleableAutoFireComponent> ent,
    ref GunToggleableAutoFireActionEvent args)
  {
    args.Handled = true;
    EntityUid performer = args.Performer;
    if (!this._rmcGun.HasRequiredEquippedPopup((Entity<GunRequireEquippedComponent>) ent.Owner, performer))
      return;
    if (!this._hands.IsHolding((Entity<HandsComponent>) performer, new EntityUid?((EntityUid) ent)))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-toggleable-autofire-requires-wielding", ("gun", (object) ent)), performer, new EntityUid?(performer), PopupType.MediumCaution);
    }
    else
    {
      WieldableComponent comp;
      if (this.TryComp<WieldableComponent>((EntityUid) ent, out comp) && !comp.Wielded)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-toggleable-autofire-requires-wielding", ("gun", (object) ent)), performer, new EntityUid?(performer), PopupType.MediumCaution);
      }
      else
      {
        this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(performer));
        if (this.EnsureComp<ActiveGunAutoFireComponent>((EntityUid) ent, out ActiveGunAutoFireComponent _))
        {
          this.RemCompDeferred<ActiveGunAutoFireComponent>((EntityUid) ent);
          this.AutoUpdated((Entity<GunToggleableAutoFireComponent>) ((EntityUid) ent, (GunToggleableAutoFireComponent) ent), false);
        }
        else
          this.AutoUpdated((Entity<GunToggleableAutoFireComponent>) ((EntityUid) ent, (GunToggleableAutoFireComponent) ent), true);
      }
    }
  }

  private void OnAutoFireGetBatteryDrain(
    Entity<GunToggleableAutoFireComponent> ent,
    ref GunGetBatteryDrainEvent args)
  {
    if (!this.HasComp<ActiveGunAutoFireComponent>((EntityUid) ent))
      return;
    args.Drain += ent.Comp.BatteryDrain;
  }

  private void OnRemove(Entity<ActiveGunAutoFireComponent> ent, ref ComponentRemove args)
  {
    if (this.TerminatingOrDeleted((EntityUid) ent))
      return;
    this.AutoUpdated((Entity<GunToggleableAutoFireComponent>) ent.Owner, false);
  }

  private void OnDoRemove<T>(Entity<ActiveGunAutoFireComponent> ent, ref T args)
  {
    this.RemCompDeferred<ActiveGunAutoFireComponent>((EntityUid) ent);
    this.AutoUpdated((Entity<GunToggleableAutoFireComponent>) ent.Owner, false);
  }

  private void AutoUpdated(Entity<GunToggleableAutoFireComponent?> ent, bool active)
  {
    if (!this.Resolve<GunToggleableAutoFireComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    SharedActionsSystem actions = this._actions;
    EntityUid? action1 = ent.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = active ? 1 : 0;
    actions.SetToggled(action2, num != 0);
    this._rmcGunBattery.RefreshBatteryDrain((Entity<GunDrainBatteryOnShootComponent>) ent.Owner);
  }

  public override void Update(float frameTime)
  {
    if (!this.Debug && this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveGunAutoFireComponent, GunToggleableAutoFireComponent, GunComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveGunAutoFireComponent, GunToggleableAutoFireComponent, GunComponent, TransformComponent>();
    EntityUid uid;
    ActiveGunAutoFireComponent comp1;
    GunToggleableAutoFireComponent comp2;
    GunComponent comp3;
    TransformComponent comp4;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3, out comp4))
    {
      if (!(curTime < comp1.NextFire))
      {
        comp1.NextFire = curTime + comp1.FailCooldown;
        BaseContainer container;
        if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (uid, comp4), out container) || !this._hands.IsHolding((Entity<HandsComponent>) container.Owner, new EntityUid?(uid)))
        {
          this.RemCompDeferred<ActiveGunAutoFireComponent>(uid);
        }
        else
        {
          (Vector2 WorldPosition, Angle WorldRotation) = this._transform.GetWorldPositionRotation(comp4);
          Angle angle = DirectionExtensions.ToAngle(((Angle) ref WorldRotation).GetCardinalDir());
          Vector2 vector2 = WorldPosition + ((Angle) ref angle).ToWorldVec() * (float) comp2.Range.Y / 2f;
          Box2Rotated bounds;
          // ISSUE: explicit constructor call
          ((Box2Rotated) ref bounds).\u002Ector(Box2.CenteredAround(vector2, Vector2i.op_Implicit(comp2.Range)), angle, vector2);
          Robust.Shared.Physics.Transform empty = Robust.Shared.Physics.Transform.Empty;
          this.Shape.Set(bounds);
          this._targets.Clear();
          this._entityLookup.GetEntitiesIntersecting<XenoComponent, PolygonShape>(comp4.MapID, this.Shape, empty, this._targets, LookupFlags.Uncontained);
          foreach (Entity<XenoComponent> target in this._targets)
          {
            if (!this._mobState.IsDead((EntityUid) target) && this._interaction.InRangeUnobstructed((Entity<TransformComponent>) container.Owner, (Entity<TransformComponent>) target.Owner, comp2.MaxRange, CollisionGroup.Impassable | CollisionGroup.BulletImpassable))
            {
              if (this._net.IsServer)
                this._gun.AttemptShoot(container.Owner, uid, comp3, target.Owner.ToCoordinates());
              comp1.NextFire = curTime + comp1.Cooldown;
              break;
            }
          }
        }
      }
    }
  }
}
