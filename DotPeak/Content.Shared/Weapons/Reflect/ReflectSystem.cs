// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Reflect.ReflectSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Localizations;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared.Weapons.Reflect;

public sealed class ReflectSystem : EntitySystem
{
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    base.Initialize();
    this.Subs.SubscribeWithRelay<ReflectComponent, ProjectileReflectAttemptEvent>(new EntityEventRefHandler<ReflectComponent, ProjectileReflectAttemptEvent>(this.OnReflectUserCollide), false);
    this.Subs.SubscribeWithRelay<ReflectComponent, HitScanReflectAttemptEvent>(new EntityEventRefHandler<ReflectComponent, HitScanReflectAttemptEvent>(this.OnReflectUserHitscan), false);
    this.SubscribeLocalEvent<ReflectComponent, ProjectileReflectAttemptEvent>(new EntityEventRefHandler<ReflectComponent, ProjectileReflectAttemptEvent>(this.OnReflectCollide));
    this.SubscribeLocalEvent<ReflectComponent, HitScanReflectAttemptEvent>(new EntityEventRefHandler<ReflectComponent, HitScanReflectAttemptEvent>(this.OnReflectHitscan));
    this.SubscribeLocalEvent<ReflectComponent, GotEquippedEvent>(new EntityEventRefHandler<ReflectComponent, GotEquippedEvent>(this.OnReflectEquipped));
    this.SubscribeLocalEvent<ReflectComponent, GotUnequippedEvent>(new EntityEventRefHandler<ReflectComponent, GotUnequippedEvent>(this.OnReflectUnequipped));
    this.SubscribeLocalEvent<ReflectComponent, GotEquippedHandEvent>(new EntityEventRefHandler<ReflectComponent, GotEquippedHandEvent>(this.OnReflectHandEquipped));
    this.SubscribeLocalEvent<ReflectComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<ReflectComponent, GotUnequippedHandEvent>(this.OnReflectHandUnequipped));
    this.SubscribeLocalEvent<ReflectComponent, ExaminedEvent>(new EntityEventRefHandler<ReflectComponent, ExaminedEvent>(this.OnExamine));
  }

  private void OnReflectUserCollide(
    Entity<ReflectComponent> ent,
    ref ProjectileReflectAttemptEvent args)
  {
    if (args.Cancelled || !ent.Comp.InRightPlace || !this.TryReflectProjectile(ent, ent.Owner, (Entity<ProjectileComponent>) args.ProjUid))
      return;
    args.Cancelled = true;
  }

  private void OnReflectUserHitscan(
    Entity<ReflectComponent> ent,
    ref HitScanReflectAttemptEvent args)
  {
    Vector2? newDirection;
    if (args.Reflected || !ent.Comp.InRightPlace || !this.TryReflectHitscan(ent, ent.Owner, args.Shooter, args.SourceItem, args.Direction, args.Reflective, out newDirection))
      return;
    args.Direction = newDirection.Value;
    args.Reflected = true;
  }

  private void OnReflectCollide(
    Entity<ReflectComponent> ent,
    ref ProjectileReflectAttemptEvent args)
  {
    if (args.Cancelled || !this.TryReflectProjectile(ent, ent.Owner, (Entity<ProjectileComponent>) args.ProjUid))
      return;
    args.Cancelled = true;
  }

  private void OnReflectHitscan(Entity<ReflectComponent> ent, ref HitScanReflectAttemptEvent args)
  {
    Vector2? newDirection;
    if (args.Reflected || !this.TryReflectHitscan(ent, ent.Owner, args.Shooter, args.SourceItem, args.Direction, args.Reflective, out newDirection))
      return;
    args.Direction = newDirection.Value;
    args.Reflected = true;
  }

  private bool TryReflectProjectile(
    Entity<ReflectComponent> reflector,
    EntityUid user,
    Entity<ProjectileComponent?> projectile)
  {
    ReflectiveComponent comp1;
    PhysicsComponent comp2;
    if (!this.TryComp<ReflectiveComponent>((EntityUid) projectile, out comp1) || (reflector.Comp.Reflects & comp1.Reflective) == ReflectType.None || !this._toggle.IsActivated((Entity<ItemToggleComponent>) reflector.Owner) || !this._random.Prob(reflector.Comp.ReflectProb) || !this.TryComp<PhysicsComponent>((EntityUid) projectile, out comp2))
      return false;
    Angle angle1 = this._random.NextAngle(Angle.op_Implicit(Angle.op_Implicit(Angle.op_UnaryNegation(reflector.Comp.Spread)) / 2.0), Angle.op_Implicit(Angle.op_Implicit(reflector.Comp.Spread) / 2.0));
    Angle angle2 = ((Angle) ref angle1).Opposite();
    Vector2 mapLinearVelocity = this._physics.GetMapLinearVelocity((EntityUid) projectile, comp2);
    Vector2 vector2_1 = mapLinearVelocity - this._physics.GetMapLinearVelocity(user);
    Vector2 vector2_2 = ((Angle) ref angle2).RotateVec(ref vector2_1) - mapLinearVelocity;
    this._physics.SetLinearVelocity((EntityUid) projectile, comp2.LinearVelocity + vector2_2, body: comp2);
    Angle localRotation = this.Transform((EntityUid) projectile).LocalRotation;
    ref Angle local1 = ref angle2;
    Vector2 vec = ((Angle) ref localRotation).ToVec();
    ref Vector2 local2 = ref vec;
    Vector2 vector2_3 = ((Angle) ref local1).RotateVec(ref local2);
    this._transform.SetLocalRotation((EntityUid) projectile, DirectionExtensions.ToAngle(vector2_3));
    this.PlayAudioAndPopup(reflector.Comp, user);
    if (this.Resolve<ProjectileComponent>((EntityUid) projectile, ref projectile.Comp, false))
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(26, 4);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" reflected ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) projectile)), "ToPrettyString(projectile)");
      logStringHandler.AppendLiteral(" from ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(projectile.Comp.Weapon), "ToPrettyString(projectile.Comp.Weapon)");
      logStringHandler.AppendLiteral(" shot by ");
      logStringHandler.AppendFormatted<EntityUid?>(projectile.Comp.Shooter, "projectile.Comp.Shooter");
      ref LogStringHandler local3 = ref logStringHandler;
      adminLogger.Add(LogType.BulletHit, LogImpact.Medium, ref local3);
      projectile.Comp.Shooter = new EntityUid?(user);
      projectile.Comp.Weapon = new EntityUid?(user);
      this.Dirty((EntityUid) projectile, (IComponent) projectile.Comp);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(11, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" reflected ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) projectile)), "ToPrettyString(projectile)");
      ref LogStringHandler local4 = ref logStringHandler;
      adminLogger.Add(LogType.BulletHit, LogImpact.Medium, ref local4);
    }
    return true;
  }

  private bool TryReflectHitscan(
    Entity<ReflectComponent> reflector,
    EntityUid user,
    EntityUid? shooter,
    EntityUid shotSource,
    Vector2 direction,
    ReflectType hitscanReflectType,
    [NotNullWhen(true)] out Vector2? newDirection)
  {
    if ((reflector.Comp.Reflects & hitscanReflectType) == ReflectType.None || !this._toggle.IsActivated((Entity<ItemToggleComponent>) reflector.Owner) || !this._random.Prob(reflector.Comp.ReflectProb))
    {
      newDirection = new Vector2?();
      return false;
    }
    this.PlayAudioAndPopup(reflector.Comp, user);
    Angle angle = this._random.NextAngle(Angle.op_Implicit(Angle.op_Implicit(Angle.op_UnaryNegation(reflector.Comp.Spread)) / 2.0), Angle.op_Implicit(Angle.op_Implicit(reflector.Comp.Spread) / 2.0));
    newDirection = new Vector2?(-((Angle) ref angle).RotateVec(ref direction));
    if (shooter.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(33, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" reflected hitscan from ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) shotSource), "ToPrettyString(shotSource)");
      logStringHandler.AppendLiteral(" shot by ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) shooter.Value), "ToPrettyString(shooter.Value)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.HitScanHit, LogImpact.Medium, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(24, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" reflected hitscan from ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) shotSource), "ToPrettyString(shotSource)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.HitScanHit, LogImpact.Medium, ref local);
    }
    return true;
  }

  private void PlayAudioAndPopup(ReflectComponent reflect, EntityUid user)
  {
    if (!this._netManager.IsServer)
      return;
    this._popup.PopupEntity(this.Loc.GetString("reflect-shot"), user);
    this._audio.PlayPvs(reflect.SoundOnReflect, user);
  }

  private void OnReflectEquipped(Entity<ReflectComponent> ent, ref GotEquippedEvent args)
  {
    ent.Comp.InRightPlace = (ent.Comp.SlotFlags & args.SlotFlags) == args.SlotFlags;
    this.Dirty<ReflectComponent>(ent);
  }

  private void OnReflectUnequipped(Entity<ReflectComponent> ent, ref GotUnequippedEvent args)
  {
    ent.Comp.InRightPlace = false;
    this.Dirty<ReflectComponent>(ent);
  }

  private void OnReflectHandEquipped(Entity<ReflectComponent> ent, ref GotEquippedHandEvent args)
  {
    ent.Comp.InRightPlace = ent.Comp.ReflectingInHands;
    this.Dirty<ReflectComponent>(ent);
  }

  private void OnReflectHandUnequipped(
    Entity<ReflectComponent> ent,
    ref GotUnequippedHandEvent args)
  {
    ent.Comp.InRightPlace = false;
    this.Dirty<ReflectComponent>(ent);
  }

  private void OnExamine(Entity<ReflectComponent> ent, ref ExaminedEvent args)
  {
    float num = MathF.Round(ent.Comp.ReflectProb * 100f, 1);
    if (!this._toggle.IsActivated((Entity<ItemToggleComponent>) ent.Owner) || (double) num == 0.0 || ent.Comp.Reflects == ReflectType.None)
      return;
    string[] strArray = ent.Comp.Reflects.ToString().Split(", ");
    List<string> list = new List<string>(strArray.Length);
    for (int index = 0; index < strArray.Length; ++index)
    {
      string str = this.Loc.GetString(("reflect-component-" + strArray[index]).ToLower());
      list.Add(str);
    }
    string str1 = ContentLocalizationManager.FormatList(list);
    args.PushMarkup(this.Loc.GetString("reflect-component-examine", ("value", (object) num), ("type", (object) str1)));
  }
}
