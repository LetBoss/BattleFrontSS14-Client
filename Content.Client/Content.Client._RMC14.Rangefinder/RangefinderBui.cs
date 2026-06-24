using System;
using Content.Client.Message;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Rangefinder;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Rangefinder;

public sealed class RangefinderBui : BoundUserInterface
{
	private RangefinderWindow? _window;

	private readonly AreaSystem _area;

	private readonly SharedTransformSystem _transform;

	public RangefinderBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_area = base.EntMan.System<AreaSystem>();
		_transform = base.EntMan.System<SharedTransformSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RangefinderWindow>((BoundUserInterface)(object)this);
		_window.Header.SetMarkupPermissive(Loc.GetString("rmc-rangefinder-header"));
		Refresh();
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		RangefinderWindow window = _window;
		RangefinderComponent rangefinderComponent = default(RangefinderComponent);
		if (window != null && ((BaseWindow)window).IsOpen && base.EntMan.TryGetComponent<RangefinderComponent>(((BoundUserInterface)this).Owner, ref rangefinderComponent))
		{
			Vector2i? lastTarget = rangefinderComponent.LastTarget;
			if (lastTarget.HasValue)
			{
				Vector2i valueOrDefault = lastTarget.GetValueOrDefault();
				string markup = Loc.GetString("rmc-rangefinder-longitude", new(string, object)[1] { ("x", valueOrDefault.X) });
				_window.Longitude.SetMarkupPermissive(markup);
				markup = Loc.GetString("rmc-rangefinder-latitude", new(string, object)[1] { ("y", valueOrDefault.Y) });
				_window.Latitude.SetMarkupPermissive(markup);
			}
			((Control)_window.BottomContainer).DisposeAllChildren();
			MapCoordinates? lastCoords = rangefinderComponent.LastCoords;
			if (lastCoords.HasValue)
			{
				MapCoordinates valueOrDefault2 = lastCoords.GetValueOrDefault();
				EntityCoordinates coordinates = _transform.ToCoordinates(valueOrDefault2);
				((Control)_window.BottomContainer).AddChild((Control)(object)AddRow("Supply Drop", _area.CanSupplyDrop(valueOrDefault2)));
				((Control)_window.BottomContainer).AddChild((Control)(object)AddRow("Mortar", _area.CanMortarFire(coordinates)));
				((Control)_window.BottomContainer).AddChild((Control)(object)AddRow("Close Air Support", _area.CanCAS(coordinates)));
				((Control)_window.BottomContainer).AddChild((Control)(object)AddRow("Orbital Bombardment", _area.CanOrbitalBombard(coordinates, out var _)));
			}
		}
	}

	private BoxContainer AddRow(string text, bool allowed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		RichTextLabel val2 = new RichTextLabel();
		val2.SetMarkup((allowed ? "[color=green]✓[/color]" : "[color=red]X[/color]") + " " + text);
		((Control)val).AddChild((Control)(object)val2);
		return val;
	}
}
