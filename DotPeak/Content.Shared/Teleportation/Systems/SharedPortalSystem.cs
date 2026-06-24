// Decompiled with JetBrains decompiler
// Type: Content.Shared.Teleportation.Systems.SharedPortalSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ghost;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Teleportation.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Teleportation.Systems;

public abstract class SharedPortalSystem : EntitySystem
{
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private PullingSystem _pulling;
  [Dependency]
  private SharedPopupSystem _popup;
  private const string PortalFixture = "portalFixture";
  private const string ProjectileFixture = "projectile";
  private const int MaxRandomTeleportAttempts = 20;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PortalComponent, StartCollideEvent>(new ComponentEventRefHandler<PortalComponent, StartCollideEvent>(this.OnCollide));
    this.SubscribeLocalEvent<PortalComponent, EndCollideEvent>(new ComponentEventRefHandler<PortalComponent, EndCollideEvent>(this.OnEndCollide));
    this.SubscribeLocalEvent<PortalComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<PortalComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetVerbs));
  }

  private void OnGetVerbs(
    EntityUid uid,
    PortalComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !this.HasComp<GhostComponent>(args.User))
      return;
    LinkedEntityComponent link;
    bool disabled = !this.TryComp<LinkedEntityComponent>(uid, out link) || link.LinkedEntities.Count != 1;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Priority = 11;
    alternativeVerb.Act = (Action) (() =>
    {
      if (link == null | disabled)
        return;
      EntityUid uid1 = link.LinkedEntities.First<EntityUid>();
      this.TeleportEntity(uid, args.User, this.Transform(uid1).Coordinates, new EntityUid?(uid1), false);
    });
    alternativeVerb.Disabled = disabled;
    alternativeVerb.Text = this.Loc.GetString("portal-component-ghost-traverse");
    alternativeVerb.Message = disabled ? this.Loc.GetString("portal-component-no-linked-entities") : this.Loc.GetString("portal-component-can-ghost-traverse");
    alternativeVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/open.svg.192dpi.png"));
    verbs.Add(alternativeVerb);
  }

  private bool ShouldCollide(string ourId, string otherId, Fixture our, Fixture other)
  {
    if (!(ourId == "portalFixture"))
      return false;
    return other.Hard || otherId == "projectile";
  }

  private void OnCollide(EntityUid uid, PortalComponent component, ref StartCollideEvent args)
  {
    if (!this.ShouldCollide(args.OurFixtureId, args.OtherFixtureId, args.OurFixture, args.OtherFixture))
      return;
    EntityUid otherEntity = args.OtherEntity;
    if (this.Transform(otherEntity).Anchored)
      return;
    PullableComponent comp1;
    if (this.TryComp<PullableComponent>(otherEntity, out comp1) && comp1.BeingPulled)
      this._pulling.TryStopPull(otherEntity, comp1);
    PullerComponent comp2;
    PullableComponent comp3;
    if (this.TryComp<PullerComponent>(otherEntity, out comp2) && this.TryComp<PullableComponent>(comp2.Pulling, out comp3))
      this._pulling.TryStopPull(comp2.Pulling.Value, comp3);
    if (this.HasComp<PortalTimeoutComponent>(otherEntity))
      return;
    LinkedEntityComponent comp4;
    if (this.TryComp<LinkedEntityComponent>(uid, out comp4))
    {
      if (comp4.LinkedEntities.Count == 0)
        return;
      if (this._netMan.IsClient)
      {
        EntityUid uid1 = comp4.LinkedEntities.First<EntityUid>();
        bool flag = this.Exists(uid1);
        if (comp4.LinkedEntities.Count != 1 || !flag || flag && this.Transform(uid1).MapID == MapId.Nullspace)
          return;
      }
      EntityUid uid2 = this._random.Pick<EntityUid>((IReadOnlyCollection<EntityUid>) comp4.LinkedEntities);
      if (this.HasComp<PortalComponent>(uid2))
      {
        PortalTimeoutComponent timeoutComponent = this.EnsureComp<PortalTimeoutComponent>(otherEntity);
        timeoutComponent.EnteredPortal = new EntityUid?(uid);
        this.Dirty(otherEntity, (IComponent) timeoutComponent);
      }
      this.TeleportEntity(uid, otherEntity, this.Transform(uid2).Coordinates, new EntityUid?(uid2));
    }
    else
    {
      if (this._netMan.IsClient || !component.RandomTeleport)
        return;
      this.TeleportRandomly(uid, otherEntity, component);
    }
  }

  private void OnEndCollide(EntityUid uid, PortalComponent component, ref EndCollideEvent args)
  {
    if (!this.ShouldCollide(args.OurFixtureId, args.OtherFixtureId, args.OurFixture, args.OtherFixture))
      return;
    EntityUid otherEntity = args.OtherEntity;
    PortalTimeoutComponent comp;
    if (!this.TryComp<PortalTimeoutComponent>(otherEntity, out comp))
      return;
    EntityUid? enteredPortal = comp.EnteredPortal;
    EntityUid entityUid = uid;
    if ((enteredPortal.HasValue ? (enteredPortal.GetValueOrDefault() != entityUid ? 1 : 0) : 1) == 0)
      return;
    this.RemCompDeferred<PortalTimeoutComponent>(otherEntity);
  }

  private void TeleportEntity(
    EntityUid portal,
    EntityUid subject,
    EntityCoordinates target,
    EntityUid? targetEntity = null,
    bool playSound = true,
    PortalComponent? portalComponent = null)
  {
    if (!this.Resolve<PortalComponent>(portal, ref portalComponent))
      return;
    EntityCoordinates coordinates = this.Transform(portal).Coordinates;
    int num1 = this._transform.GetMapId(coordinates) == this._transform.GetMapId(target) ? 1 : 0;
    float distance;
    int num2;
    if (portalComponent.MaxTeleportRadius.HasValue && coordinates.TryDistance((IEntityManager) this.EntityManager, target, out distance))
    {
      double num3 = (double) distance;
      float? maxTeleportRadius = portalComponent.MaxTeleportRadius;
      double valueOrDefault = (double) maxTeleportRadius.GetValueOrDefault();
      num2 = num3 > valueOrDefault & maxTeleportRadius.HasValue ? 1 : 0;
    }
    else
      num2 = 0;
    bool flag = num2 != 0;
    if (((num1 != 0 ? 0 : (!portalComponent.CanTeleportToOtherMaps ? 1 : 0)) | (flag ? 1 : 0)) != 0)
    {
      if (!this._netMan.IsServer)
        return;
      this._popup.PopupCoordinates(this.Loc.GetString("portal-component-invalid-configuration-fizzle"), coordinates, Filter.Pvs(coordinates, entityMan: (IEntityManager) this.EntityManager), true);
      this._popup.PopupCoordinates(this.Loc.GetString("portal-component-invalid-configuration-fizzle"), target, Filter.Pvs(target, entityMan: (IEntityManager) this.EntityManager), true);
      this.QueueDel(new EntityUid?(portal));
      if (!targetEntity.HasValue)
        return;
      this.QueueDel(new EntityUid?(targetEntity.Value));
    }
    else
    {
      SoundSpecifier sound = this.CompOrNull<PortalComponent>(targetEntity)?.ArrivalSound ?? portalComponent.ArrivalSound;
      SoundSpecifier departureSound = portalComponent.DepartureSound;
      ProjectileComponent comp;
      if (this.TryComp<ProjectileComponent>(subject, out comp))
        comp.IgnoreShooter = false;
      this.LogTeleport(portal, subject, this.Transform(subject).Coordinates, target);
      this._transform.SetCoordinates(subject, target);
      if (!playSound)
        return;
      this._audio.PlayPredicted(departureSound, portal, new EntityUid?(subject));
      this._audio.PlayPredicted(sound, subject, new EntityUid?(subject));
    }
  }

  private void TeleportRandomly(EntityUid portal, EntityUid subject, PortalComponent? component = null)
  {
    if (!this.Resolve<PortalComponent>(portal, ref component))
      return;
    EntityCoordinates coordinates = this.Transform(portal).Coordinates;
    EntityCoordinates entityCoordinates = coordinates.Offset(this._random.NextVector2(component.MaxRandomRadius));
    for (int index = 0; index < 20; ++index)
    {
      Vector2 position = this._random.NextVector2(component.MaxRandomRadius);
      entityCoordinates = coordinates.Offset(position);
      if (!this._lookup.AnyEntitiesIntersecting(this._transform.ToMapCoordinates(entityCoordinates), LookupFlags.Static))
        break;
    }
    this.TeleportEntity(portal, subject, entityCoordinates);
  }

  protected virtual void LogTeleport(
    EntityUid portal,
    EntityUid subject,
    EntityCoordinates source,
    EntityCoordinates target)
  {
  }
}
