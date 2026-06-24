// Decompiled with JetBrains decompiler
// Type: Content.Shared.StationRecords.StationRecordKeyStorageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.StationRecords;

public sealed class StationRecordKeyStorageSystem : EntitySystem
{
  [Dependency]
  private SharedStationRecordsSystem _records;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StationRecordKeyStorageComponent, ComponentGetState>(new ComponentEventRefHandler<StationRecordKeyStorageComponent, ComponentGetState>(this.OnGetState));
    this.SubscribeLocalEvent<StationRecordKeyStorageComponent, ComponentHandleState>(new ComponentEventRefHandler<StationRecordKeyStorageComponent, ComponentHandleState>(this.OnHandleState));
  }

  private void OnGetState(
    EntityUid uid,
    StationRecordKeyStorageComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new StationRecordKeyStorageComponentState(this._records.Convert(component.Key));
  }

  private void OnHandleState(
    EntityUid uid,
    StationRecordKeyStorageComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is StationRecordKeyStorageComponentState current))
      return;
    component.Key = this._records.Convert(current.Key);
  }

  public void AssignKey(
    EntityUid uid,
    StationRecordKey key,
    StationRecordKeyStorageComponent? keyStorage = null)
  {
    if (!this.Resolve<StationRecordKeyStorageComponent>(uid, ref keyStorage))
      return;
    keyStorage.Key = new StationRecordKey?(key);
    this.Dirty(uid, (IComponent) keyStorage);
  }

  public StationRecordKey? RemoveKey(EntityUid uid, StationRecordKeyStorageComponent? keyStorage = null)
  {
    if (!this.Resolve<StationRecordKeyStorageComponent>(uid, ref keyStorage) || !keyStorage.Key.HasValue)
      return new StationRecordKey?();
    StationRecordKey? key = keyStorage.Key;
    keyStorage.Key = new StationRecordKey?();
    this.Dirty(uid, (IComponent) keyStorage);
    return key;
  }

  public bool CheckKey(EntityUid uid, StationRecordKeyStorageComponent? keyStorage = null)
  {
    return this.Resolve<StationRecordKeyStorageComponent>(uid, ref keyStorage) && keyStorage.Key.HasValue;
  }
}
