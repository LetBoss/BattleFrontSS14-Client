// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.ExtinguishableSetCollisionWakeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public sealed class ExtinguishableSetCollisionWakeSystem : EntitySystem
{
  [Dependency]
  private CollisionWakeSystem _collisionWake;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ExtinguishableSetCollisionWakeComponent, ExtinguishedEvent>(new EntityEventRefHandler<ExtinguishableSetCollisionWakeComponent, ExtinguishedEvent>((object) this, __methodptr(HandleExtinguished)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ExtinguishableSetCollisionWakeComponent, IgnitedEvent>(new EntityEventRefHandler<ExtinguishableSetCollisionWakeComponent, IgnitedEvent>((object) this, __methodptr(HandleIgnited)), (Type[]) null, (Type[]) null);
  }

  private void HandleExtinguished(
    Entity<ExtinguishableSetCollisionWakeComponent> ent,
    ref ExtinguishedEvent args)
  {
    this._collisionWake.SetEnabled(Entity<ExtinguishableSetCollisionWakeComponent>.op_Implicit(ent), true, (CollisionWakeComponent) null);
  }

  private void HandleIgnited(
    Entity<ExtinguishableSetCollisionWakeComponent> ent,
    ref IgnitedEvent args)
  {
    this._collisionWake.SetEnabled(Entity<ExtinguishableSetCollisionWakeComponent>.op_Implicit(ent), false, (CollisionWakeComponent) null);
  }
}
