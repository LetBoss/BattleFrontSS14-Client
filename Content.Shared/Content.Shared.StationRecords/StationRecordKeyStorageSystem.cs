using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.StationRecords;

public sealed class StationRecordKeyStorageSystem : EntitySystem
{
	[Dependency]
	private SharedStationRecordsSystem _records;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StationRecordKeyStorageComponent, ComponentGetState>((ComponentEventRefHandler<StationRecordKeyStorageComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationRecordKeyStorageComponent, ComponentHandleState>((ComponentEventRefHandler<StationRecordKeyStorageComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnGetState(EntityUid uid, StationRecordKeyStorageComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new StationRecordKeyStorageComponentState(_records.Convert(component.Key));
	}

	private void OnHandleState(EntityUid uid, StationRecordKeyStorageComponent component, ref ComponentHandleState args)
	{
		if (((ComponentHandleState)(ref args)).Current is StationRecordKeyStorageComponentState state)
		{
			component.Key = _records.Convert(state.Key);
		}
	}

	public void AssignKey(EntityUid uid, StationRecordKey key, StationRecordKeyStorageComponent? keyStorage = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StationRecordKeyStorageComponent>(uid, ref keyStorage, true))
		{
			keyStorage.Key = key;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)keyStorage, (MetaDataComponent)null);
		}
	}

	public StationRecordKey? RemoveKey(EntityUid uid, StationRecordKeyStorageComponent? keyStorage = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StationRecordKeyStorageComponent>(uid, ref keyStorage, true) || !keyStorage.Key.HasValue)
		{
			return null;
		}
		StationRecordKey? key = keyStorage.Key;
		keyStorage.Key = null;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)keyStorage, (MetaDataComponent)null);
		return key;
	}

	public bool CheckKey(EntityUid uid, StationRecordKeyStorageComponent? keyStorage = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StationRecordKeyStorageComponent>(uid, ref keyStorage, true))
		{
			return false;
		}
		return keyStorage.Key.HasValue;
	}
}
