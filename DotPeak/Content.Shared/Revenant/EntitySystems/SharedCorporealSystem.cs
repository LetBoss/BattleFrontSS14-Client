// Decompiled with JetBrains decompiler
// Type: Content.Shared.Revenant.EntitySystems.SharedCorporealSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Systems;
using Content.Shared.Revenant.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Revenant.EntitySystems;

public abstract class SharedCorporealSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private MovementSpeedModifierSystem _movement;
  [Dependency]
  private SharedPhysicsSystem _physics;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CorporealComponent, ComponentStartup>(new ComponentEventHandler<CorporealComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<CorporealComponent, ComponentShutdown>(new ComponentEventHandler<CorporealComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<CorporealComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<CorporealComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefresh));
  }

  private void OnRefresh(
    EntityUid uid,
    CorporealComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    args.ModifySpeed(component.MovementSpeedDebuff, component.MovementSpeedDebuff);
  }

  public virtual void OnStartup(EntityUid uid, CorporealComponent component, ComponentStartup args)
  {
    this._appearance.SetData(uid, (Enum) RevenantVisuals.Corporeal, (object) true);
    FixturesComponent comp;
    if (this.TryComp<FixturesComponent>(uid, out comp) && comp.FixtureCount >= 1)
    {
      KeyValuePair<string, Fixture> keyValuePair = comp.Fixtures.First<KeyValuePair<string, Fixture>>();
      this._physics.SetCollisionMask(uid, keyValuePair.Key, keyValuePair.Value, 50, comp);
      this._physics.SetCollisionLayer(uid, keyValuePair.Key, keyValuePair.Value, 65, comp);
    }
    this._movement.RefreshMovementSpeedModifiers(uid);
  }

  public virtual void OnShutdown(
    EntityUid uid,
    CorporealComponent component,
    ComponentShutdown args)
  {
    this._appearance.SetData(uid, (Enum) RevenantVisuals.Corporeal, (object) false);
    FixturesComponent comp;
    if (this.TryComp<FixturesComponent>(uid, out comp) && comp.FixtureCount >= 1)
    {
      KeyValuePair<string, Fixture> keyValuePair = comp.Fixtures.First<KeyValuePair<string, Fixture>>();
      this._physics.SetCollisionMask(uid, keyValuePair.Key, keyValuePair.Value, 32 /*0x20*/, comp);
      this._physics.SetCollisionLayer(uid, keyValuePair.Key, keyValuePair.Value, 0, comp);
    }
    component.MovementSpeedDebuff = 1f;
    this._movement.RefreshMovementSpeedModifiers(uid);
  }
}
