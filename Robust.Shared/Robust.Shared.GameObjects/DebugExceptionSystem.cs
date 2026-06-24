using System;

namespace Robust.Shared.GameObjects;

public sealed class DebugExceptionSystem : EntitySystem
{
	public override void Initialize()
	{
		base.Initialize();
		SubscribeLocalEvent<DebugExceptionOnAddComponent, ComponentAdd>(OnCompAdd);
		SubscribeLocalEvent((ComponentEventHandler<DebugExceptionInitializeComponent, ComponentInit>)delegate
		{
			throw new NotSupportedException();
		}, (Type[]?)null, (Type[]?)null);
		SubscribeLocalEvent((ComponentEventHandler<DebugExceptionStartupComponent, ComponentStartup>)delegate
		{
			throw new NotSupportedException();
		}, (Type[]?)null, (Type[]?)null);
	}

	private void OnCompAdd(EntityUid uid, DebugExceptionOnAddComponent component, ComponentAdd args)
	{
		throw new NotSupportedException();
	}
}
