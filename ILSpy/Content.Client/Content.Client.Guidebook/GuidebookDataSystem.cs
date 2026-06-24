using System;
using Content.Shared.Guidebook;
using Robust.Shared.GameObjects;

namespace Content.Client.Guidebook;

public sealed class GuidebookDataSystem : EntitySystem
{
	private GuidebookData? _data;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<UpdateGuidebookDataEvent>((EntityEventHandler<UpdateGuidebookDataEvent>)OnServerUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RequestGuidebookDataEvent());
	}

	private void OnServerUpdated(UpdateGuidebookDataEvent args)
	{
		_data = args.Data;
		_data.Freeze();
	}

	public bool TryGetValue(string prototype, string component, string field, out object? value)
	{
		if (_data == null)
		{
			value = null;
			return false;
		}
		return _data.TryGetValue(prototype, component, field, out value);
	}
}
