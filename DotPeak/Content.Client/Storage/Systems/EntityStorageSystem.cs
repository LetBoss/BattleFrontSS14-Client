// Decompiled with JetBrains decompiler
// Type: Content.Client.Storage.Systems.EntityStorageSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Storage.Components;
using Content.Shared.Destructible;
using Content.Shared.Foldable;
using Content.Shared.Interaction;
using Content.Shared.Lock;
using Content.Shared.Movement.Events;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.Storage.Systems;

public sealed class EntityStorageSystem : SharedEntityStorageSystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, EntityUnpausedEvent>(new ComponentEventHandler<EntityStorageComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpausedEvent)), (Type[]) null, (Type[]) null);
    EntityStorageSystem entityStorageSystem1 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, ComponentInit>(new ComponentEventHandler<EntityStorageComponent, ComponentInit>((object) entityStorageSystem1, __vmethodptr(entityStorageSystem1, OnComponentInit)), (Type[]) null, (Type[]) null);
    EntityStorageSystem entityStorageSystem2 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, ComponentStartup>(new ComponentEventHandler<EntityStorageComponent, ComponentStartup>((object) entityStorageSystem2, __vmethodptr(entityStorageSystem2, OnComponentStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, ActivateInWorldEvent>(new ComponentEventHandler<EntityStorageComponent, ActivateInWorldEvent>((object) this, __methodptr(OnInteract)), (Type[]) null, new Type[1]
    {
      typeof (LockSystem)
    });
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, LockToggleAttemptEvent>(new ComponentEventRefHandler<EntityStorageComponent, LockToggleAttemptEvent>((object) this, __methodptr(OnLockToggleAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, DestructionEventArgs>(new ComponentEventHandler<EntityStorageComponent, DestructionEventArgs>((object) this, __methodptr(OnDestruction)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<EntityStorageComponent, GetVerbsEvent<InteractionVerb>>((object) this, __methodptr(AddToggleOpenVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, ContainerRelayMovementEntityEvent>(new ComponentEventRefHandler<EntityStorageComponent, ContainerRelayMovementEntityEvent>((object) this, __methodptr(OnRelayMovement)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, FoldAttemptEvent>(new ComponentEventRefHandler<EntityStorageComponent, FoldAttemptEvent>((object) this, __methodptr(OnFoldAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, ComponentGetState>(new ComponentEventRefHandler<EntityStorageComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityStorageComponent, ComponentHandleState>(new ComponentEventRefHandler<EntityStorageComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  public override bool ResolveStorage(EntityUid uid, [NotNullWhen(true)] ref SharedEntityStorageComponent? component)
  {
    if (component != null)
      return true;
    EntityStorageComponent storageComponent;
    this.TryComp<EntityStorageComponent>(uid, ref storageComponent);
    component = (SharedEntityStorageComponent) storageComponent;
    return component != null;
  }
}
