using System;
using Content.Shared.Pinpointer;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Pinpointer.UI;

public sealed class StationMapBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private StationMapWindow? _window;

	public StationMapBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		EntityUid? val = null;
		TransformComponent val2 = default(TransformComponent);
		if (base.EntMan.TryGetComponent<TransformComponent>(((BoundUserInterface)this).Owner, ref val2))
		{
			val = val2.GridUid;
		}
		_window = BoundUserInterfaceExt.CreateWindow<StationMapWindow>((BoundUserInterface)(object)this);
		_window.Title = base.EntMan.GetComponent<MetaDataComponent>(((BoundUserInterface)this).Owner).EntityName;
		string stationName = string.Empty;
		MetaDataComponent val3 = default(MetaDataComponent);
		if (base.EntMan.TryGetComponent<MetaDataComponent>(val, ref val3))
		{
			stationName = val3.EntityName;
		}
		StationMapComponent stationMapComponent = default(StationMapComponent);
		if (base.EntMan.TryGetComponent<StationMapComponent>(((BoundUserInterface)this).Owner, ref stationMapComponent) && stationMapComponent.ShowLocation)
		{
			_window.Set(stationName, val, ((BoundUserInterface)this).Owner);
		}
		else
		{
			_window.Set(stationName, val, null);
		}
	}
}
