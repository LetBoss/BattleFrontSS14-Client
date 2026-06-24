// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.StoreOnCollideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Lock;
using Content.Shared.Storage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

internal sealed class StoreOnCollideSystem : EntitySystem
{
  [Dependency]
  private SharedEntityStorageSystem _storage;
  [Dependency]
  private LockSystem _lock;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private IGameTiming _gameTiming;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StoreOnCollideComponent, StartCollideEvent>(new EntityEventRefHandler<StoreOnCollideComponent, StartCollideEvent>(this.OnCollide));
    this.SubscribeLocalEvent<StoreOnCollideComponent, StorageAfterOpenEvent>(new EntityEventRefHandler<StoreOnCollideComponent, StorageAfterOpenEvent>(this.AfterOpen));
  }

  private void OnCollide(Entity<StoreOnCollideComponent> ent, ref StartCollideEvent args)
  {
    this.TryStoreTarget(ent, args.OtherEntity);
    this.TryLockStorage(ent);
  }

  private void AfterOpen(Entity<StoreOnCollideComponent> ent, ref StorageAfterOpenEvent args)
  {
    StoreOnCollideComponent comp = ent.Comp;
    if (comp == null || !comp.DisableWhenFirstOpened || comp.Disabled)
      return;
    comp.Disabled = true;
  }

  private void TryStoreTarget(Entity<StoreOnCollideComponent> ent, EntityUid target)
  {
    EntityUid owner = ent.Owner;
    StoreOnCollideComponent comp = ent.Comp;
    if (this._netMan.IsClient || this._gameTiming.ApplyingState || ent.Comp.Disabled || owner == target || this.Transform(target).Anchored || this._storage.IsOpen(owner) || this._whitelist.IsWhitelistFail(comp.Whitelist, target))
      return;
    this._storage.Insert(target, owner);
  }

  private void TryLockStorage(Entity<StoreOnCollideComponent> ent)
  {
    EntityUid owner = ent.Owner;
    StoreOnCollideComponent comp = ent.Comp;
    if (this._netMan.IsClient || this._gameTiming.ApplyingState || ent.Comp.Disabled || !comp.LockOnCollide || this._lock.IsLocked((Entity<LockComponent>) owner))
      return;
    this._lock.Lock(owner, new EntityUid?(owner));
  }
}
