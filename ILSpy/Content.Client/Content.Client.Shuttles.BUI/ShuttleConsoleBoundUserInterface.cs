using System;
using Content.Client.Shuttles.UI;
using Content.Shared.Shuttles.BUIStates;
using Content.Shared.Shuttles.Events;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.Shuttles.BUI;

public sealed class ShuttleConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private ShuttleConsoleWindow? _window;

	public ShuttleConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<ShuttleConsoleWindow>((BoundUserInterface)(object)this);
		_window.RequestFTL += OnFTLRequest;
		_window.RequestBeaconFTL += OnFTLBeaconRequest;
		_window.DockRequest += OnDockRequest;
		_window.UndockRequest += OnUndockRequest;
	}

	private void OnUndockRequest(NetEntity entity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new UndockRequestMessage
		{
			DockEntity = entity
		});
	}

	private void OnDockRequest(NetEntity entity, NetEntity target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new DockRequestMessage
		{
			DockEntity = entity,
			TargetDockEntity = target
		});
	}

	private void OnFTLBeaconRequest(NetEntity ent, Angle angle)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ShuttleConsoleFTLBeaconMessage
		{
			Beacon = ent,
			Angle = angle
		});
	}

	private void OnFTLRequest(MapCoordinates obj, Angle angle)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ShuttleConsoleFTLPositionMessage
		{
			Coordinates = obj,
			Angle = angle
		});
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			ShuttleConsoleWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		if (state is ShuttleBoundUserInterfaceState cState)
		{
			_window?.UpdateState(((BoundUserInterface)this).Owner, cState);
		}
	}
}
