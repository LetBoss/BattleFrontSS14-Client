// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.StoreAfterFailedInteractSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Storage.Components;
using Content.Shared.Storage.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public sealed class StoreAfterFailedInteractSystem : EntitySystem
{
  [Dependency]
  private SharedStorageSystem _storage;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StoreAfterFailedInteractComponent, StorageInsertFailedEvent>(new EntityEventRefHandler<StoreAfterFailedInteractComponent, StorageInsertFailedEvent>(this.OnStorageInsertFailed));
  }

  private void OnStorageInsertFailed(
    Entity<StoreAfterFailedInteractComponent> ent,
    ref StorageInsertFailedEvent args)
  {
    this._storage.PlayerInsertHeldEntity(args.Storage, args.Player);
  }
}
