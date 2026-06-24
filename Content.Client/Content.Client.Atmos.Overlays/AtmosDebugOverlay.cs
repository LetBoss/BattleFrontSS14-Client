using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Content.Client.Atmos.EntitySystems;
using Content.Client.Resources;
using Content.Shared.Atmos;
using Content.Shared.Atmos.EntitySystems;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.Overlays;

public sealed class AtmosDebugOverlay : Overlay
{
	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private IResourceCache _cache;

	private readonly SharedTransformSystem _transform;

	private readonly AtmosDebugOverlaySystem _system;

	private readonly SharedMapSystem _map;

	private readonly Font _font;

	private List<(Entity<MapGridComponent>, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage)> _grids = new List<(Entity<MapGridComponent>, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage)>();

	public override OverlaySpace Space => (OverlaySpace)6;

	internal AtmosDebugOverlay(AtmosDebugOverlaySystem system)
	{
		IoCManager.InjectDependencies<AtmosDebugOverlay>(this);
		_system = system;
		_transform = _entManager.System<SharedTransformSystem>();
		_map = _entManager.System<SharedMapSystem>();
		_font = _cache.GetFont("/Fonts/NotoSans/NotoSans-Regular.ttf", 12);
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if ((int)args.Space == 2)
		{
			DrawTooltip(in args);
			return;
		}
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		GetGrids(args.MapId, args.WorldBounds);
		Matrix3x2 worldMatrix;
		foreach (var grid in _grids)
		{
			Entity<MapGridComponent> item = grid.Item1;
			SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage item2 = grid.Item2;
			worldMatrix = _transform.GetWorldMatrix(Entity<MapGridComponent>.op_Implicit(item));
			((DrawingHandleBase)worldHandle).SetTransform(ref worldMatrix);
			DrawData(item2, worldHandle);
		}
		worldMatrix = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref worldMatrix);
	}

	private void DrawData(SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage msg, DrawingHandleWorld handle)
	{
		SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData?[] overlayData = msg.OverlayData;
		for (int i = 0; i < overlayData.Length; i++)
		{
			SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData? atmosDebugOverlayData = overlayData[i];
			if (atmosDebugOverlayData.HasValue)
			{
				DrawGridTile(atmosDebugOverlayData.Value, handle);
			}
		}
	}

	private void DrawGridTile(SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data, DrawingHandleWorld handle)
	{
		DrawFill(data, handle);
		DrawBlocked(data, handle);
	}

	private void DrawFill(SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data, DrawingHandleWorld handle)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		Vector2 indices = data.Indices;
		float num = (GetFillData(data) - _system.CfgBase) / _system.CfgScale;
		Color val = ((!_system.CfgCBM) ? ((num < 0.5f) ? Color.InterpolateBetween(Color.Red, Color.LimeGreen, num * 2f) : Color.InterpolateBetween(Color.LimeGreen, Color.Blue, (num - 0.5f) * 2f)) : Color.InterpolateBetween(Color.Black, Color.White, num));
		val = ((Color)(ref val)).WithAlpha(0.75f);
		handle.DrawRect(Box2.FromDimensions(new Vector2(indices.X, indices.Y), new Vector2(1f, 1f)), val, true);
	}

	private float GetFillData(SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data)
	{
		if (data.Moles == null)
		{
			return 0f;
		}
		switch (_system.CfgMode)
		{
		case AtmosDebugOverlayMode.TotalMoles:
		{
			float num = 0f;
			float[] moles = data.Moles;
			foreach (float num2 in moles)
			{
				num += num2;
			}
			return num;
		}
		case AtmosDebugOverlayMode.GasMoles:
			return data.Moles[_system.CfgSpecificGas];
		default:
			return data.Temperature;
		}
	}

	private void DrawBlocked(SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data, DrawingHandleWorld handle)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 indices = data.Indices;
		Vector2 vector = indices + 0.5f * Vector2.One;
		CheckAndShowBlockDir(data, handle, AtmosDirection.North, vector);
		CheckAndShowBlockDir(data, handle, AtmosDirection.South, vector);
		CheckAndShowBlockDir(data, handle, AtmosDirection.East, vector);
		CheckAndShowBlockDir(data, handle, AtmosDirection.West, vector);
		if (data.PressureDirection != AtmosDirection.Invalid)
		{
			DrawPressureDirection(handle, data.PressureDirection, vector, Color.Blue);
		}
		else if (data.LastPressureDirection != AtmosDirection.Invalid)
		{
			DrawPressureDirection(handle, data.LastPressureDirection, vector, Color.LightGray);
		}
		int? inExcitedGroup = data.InExcitedGroup;
		if (inExcitedGroup.HasValue)
		{
			int valueOrDefault = inExcitedGroup.GetValueOrDefault();
			Vector2 vector2 = indices;
			Vector2 vector3 = indices + new Vector2(1f, 1f);
			Vector2 vector4 = indices + new Vector2(0f, 1f);
			Vector2 vector5 = indices + new Vector2(1f, 0f);
			Color val = Color.White;
			val = ((Color)(ref val)).WithRed((float)(valueOrDefault & 0xF));
			val = ((Color)(ref val)).WithGreen((float)((valueOrDefault & 0xF0) >> 4));
			Color val2 = ((Color)(ref val)).WithBlue((float)((valueOrDefault & 0xF00) >> 8));
			((DrawingHandleBase)handle).DrawLine(vector2, vector3, val2);
			((DrawingHandleBase)handle).DrawLine(vector4, vector5, val2);
		}
		if (data.IsSpace)
		{
			((DrawingHandleBase)handle).DrawCircle(vector, 0.15f, Color.Yellow, true);
		}
		if (data.MapAtmosphere)
		{
			((DrawingHandleBase)handle).DrawCircle(vector, 0.1f, Color.Orange, true);
		}
		if (data.NoGrid)
		{
			((DrawingHandleBase)handle).DrawCircle(vector, 0.05f, Color.Black, true);
		}
	}

	private void CheckAndShowBlockDir(SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data, DrawingHandleWorld handle, AtmosDirection dir, Vector2 tileCentre)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (data.BlockDirection.HasFlag(dir))
		{
			Angle val = dir.ToAngle() - Angle.FromDegrees(90.0);
			Vector2 vector = ((Angle)(ref val)).ToVec() * 0.45f;
			Vector2 vector2 = new Vector2(vector.Y, 0f - vector.X);
			Vector2 vector3 = tileCentre + vector - vector2;
			Vector2 vector4 = tileCentre + vector + vector2;
			((DrawingHandleBase)handle).DrawLine(vector3, vector4, Color.Azure);
		}
	}

	private void DrawPressureDirection(DrawingHandleWorld handle, AtmosDirection d, Vector2 center, Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Angle val = d.ToAngle() - Angle.FromDegrees(90.0);
		Vector2 vector = ((Angle)(ref val)).ToVec() * 0.4f;
		((DrawingHandleBase)handle).DrawLine(center, center + vector, color);
	}

	private void DrawTooltip(in OverlayDrawArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		ScreenCoordinates mouseScreenPosition = _input.MouseScreenPosition;
		if (!((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
		{
			return;
		}
		Control obj = _ui.MouseGetControl(mouseScreenPosition);
		IViewportControl val = (IViewportControl)(object)((obj is IViewportControl) ? obj : null);
		if (val == null)
		{
			return;
		}
		MapCoordinates val2 = val.PixelToMap(mouseScreenPosition.Position);
		Box2 val3 = Box2.CenteredAround(val2.Position, 3f * Vector2.One);
		GetGrids(val2.MapId, new Box2Rotated(val3));
		foreach (var grid in _grids)
		{
			Entity<MapGridComponent> item = grid.Item1;
			SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage item2 = grid.Item2;
			Vector2i val4 = _map.WorldToTile(Entity<MapGridComponent>.op_Implicit(item), Entity<MapGridComponent>.op_Implicit(item), val2.Position);
			SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData?[] overlayData = item2.OverlayData;
			for (int i = 0; i < overlayData.Length; i++)
			{
				SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData? atmosDebugOverlayData = overlayData[i];
				if (atmosDebugOverlayData?.Indices == Vector2i.op_Implicit(val4))
				{
					DrawTooltip(screenHandle, mouseScreenPosition.Position, atmosDebugOverlayData.Value);
					return;
				}
			}
		}
	}

	private void DrawTooltip(DrawingHandleScreen handle, Vector2 pos, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayData data)
	{
		int lineHeight = _font.GetLineHeight(1f);
		Vector2 vector = new Vector2(0f, lineHeight);
		string text = ((data.Moles == null) ? "No Air" : data.Moles.Sum().ToString(CultureInfo.InvariantCulture));
		handle.DrawString(_font, pos, "Moles: " + text);
		pos += vector;
		handle.DrawString(_font, pos, $"Temp: {data.Temperature}");
		pos += vector;
		handle.DrawString(_font, pos, "Excited: " + (data.InExcitedGroup?.ToString() ?? "None"));
		pos += vector;
		handle.DrawString(_font, pos, $"Space: {data.IsSpace}");
		pos += vector;
		handle.DrawString(_font, pos, $"Map: {data.MapAtmosphere}");
		pos += vector;
		handle.DrawString(_font, pos, $"NoGrid: {data.NoGrid}");
		pos += vector;
		handle.DrawString(_font, pos, $"Immutable: {data.Immutable}");
	}

	private void GetGrids(MapId mapId, Box2Rotated box)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_grids.Clear();
		_mapManager.FindGridsIntersecting<List<(Entity<MapGridComponent>, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage)>>(mapId, box, ref _grids, (GridCallback<List<(Entity<MapGridComponent>, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage)>>)delegate(EntityUid uid, MapGridComponent grid, ref List<(Entity<MapGridComponent>, SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage)> state)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (_system.TileData.TryGetValue(uid, out SharedAtmosDebugOverlaySystem.AtmosDebugOverlayMessage value))
			{
				state.Add((Entity<MapGridComponent>.op_Implicit((uid, grid)), value));
			}
			return true;
		}, false, true);
	}
}
