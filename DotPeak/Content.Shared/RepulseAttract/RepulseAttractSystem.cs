// Decompiled with JetBrains decompiler
// Type: Content.Shared.RepulseAttract.RepulseAttractSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
using Content.Shared.RepulseAttract.Events;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.RepulseAttract;

public sealed class RepulseAttractSystem : EntitySystem
{
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private ThrowingSystem _throw;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedTransformSystem _xForm;
  [Dependency]
  private UseDelaySystem _delay;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private HashSet<EntityUid> _entSet = new HashSet<EntityUid>();

  public override void Initialize()
  {
    base.Initialize();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.SubscribeLocalEvent<RepulseAttractComponent, MeleeHitEvent>(new EntityEventRefHandler<RepulseAttractComponent, MeleeHitEvent>(this.OnMeleeAttempt), new Type[1]
    {
      typeof (UseDelayOnMeleeHitSystem)
    }, new Type[1]{ typeof (SharedWieldableSystem) });
    this.SubscribeLocalEvent<RepulseAttractComponent, RepulseAttractActionEvent>(new EntityEventRefHandler<RepulseAttractComponent, RepulseAttractActionEvent>(this.OnRepulseAttractAction));
  }

  private void OnMeleeAttempt(Entity<RepulseAttractComponent> ent, ref MeleeHitEvent args)
  {
    if (this._delay.IsDelayed((Entity<UseDelayComponent>) ent.Owner))
      return;
    this.TryRepulseAttract(ent, args.User);
  }

  private void OnRepulseAttractAction(
    Entity<RepulseAttractComponent> ent,
    ref RepulseAttractActionEvent args)
  {
    if (args.Handled)
      return;
    MapCoordinates mapCoordinates = this._xForm.GetMapCoordinates(args.Performer);
    args.Handled = this.TryRepulseAttract(mapCoordinates, new EntityUid?(args.Performer), ent.Comp.Speed, ent.Comp.Range, ent.Comp.Whitelist, ent.Comp.CollisionMask);
  }

  public bool TryRepulseAttract(Entity<RepulseAttractComponent> ent, EntityUid user)
  {
    return this.TryRepulseAttract(this._xForm.GetMapCoordinates(ent.Owner), new EntityUid?(user), ent.Comp.Speed, ent.Comp.Range, ent.Comp.Whitelist, ent.Comp.CollisionMask);
  }

  public bool TryRepulseAttract(
    MapCoordinates position,
    EntityUid? user,
    float speed,
    float range,
    EntityWhitelist? whitelist = null,
    CollisionGroup layer = CollisionGroup.SingularityLayer)
  {
    this._entSet.Clear();
    Vector2 position1 = position.Position;
    this._lookup.GetEntitiesInRange(position.MapId, position1, range, this._entSet, LookupFlags.Dynamic | LookupFlags.Sundries);
    foreach (EntityUid ent in this._entSet)
    {
      PhysicsComponent component;
      if (this._physicsQuery.TryGetComponent(ent, out component) && ((CollisionGroup) component.CollisionLayer & layer) == CollisionGroup.None && !this._whitelist.IsWhitelistFail(whitelist, ent))
      {
        Vector2 vector2 = this._xForm.GetWorldPosition(ent) - position1;
        if (!(vector2 == Vector2.Zero))
        {
          Vector2 direction = (double) speed < 0.0 ? -vector2 : Vector2Helpers.Normalized(vector2) * (range - vector2.Length());
          this._throw.TryThrow(ent, direction, Math.Abs(speed), user, compensateFriction: true, recoil: false);
        }
      }
    }
    return true;
  }
}
