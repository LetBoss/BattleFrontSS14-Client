using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Shared.Atmos;
using Content.Shared.Input;
using Content.Shared.Pinpointer;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Timing;

namespace Content.Client.Pinpointer.UI;

[Virtual]
public class NavMapControl : MapGridControl
{
	[Dependency]
	private IResourceCache _cache;

	private readonly SharedTransformSystem _transformSystem;

	private readonly SharedNavMapSystem _navMapSystem;

	public EntityUid? Owner;

	public EntityUid? MapUid;

	public Dictionary<EntityCoordinates, (bool Visible, Color Color)> TrackedCoordinates = new Dictionary<EntityCoordinates, (bool, Color)>();

	public Dictionary<NetEntity, NavMapBlip> TrackedEntities = new Dictionary<NetEntity, NavMapBlip>();

	public List<(Vector2, Vector2)> TileLines = new List<(Vector2, Vector2)>();

	public List<(Vector2, Vector2)> TileRects = new List<(Vector2, Vector2)>();

	public List<(Vector2[], Color)> TilePolygons = new List<(Vector2[], Color)>();

	public List<NavMapRegionOverlay> RegionOverlays = new List<NavMapRegionOverlay>();

	public Color WallColor = new Color((byte)102, (byte)217, (byte)102, byte.MaxValue);

	public Color TileColor = new Color((byte)30, (byte)67, (byte)30, byte.MaxValue);

	protected float UpdateTime = 1f;

	protected float MaxSelectableDistance = 10f;

	protected float MinDragDistance = 5f;

	protected static float MinDisplayedRange = 8f;

	protected static float MaxDisplayedRange = 128f;

	protected static float DefaultDisplayedRange = 48f;

	protected float MinmapScaleModifier = 0.075f;

	protected float FullWallInstep = 0.165f;

	protected float ThinWallThickness = 0.165f;

	protected float ThinDoorThickness = 0.3f;

	private float _updateTimer = 1f;

	private Dictionary<Color, Color> _sRGBLookUp = new Dictionary<Color, Color>();

	protected Color BackgroundColor;

	protected float BackgroundOpacity = 0.9f;

	private int _targetFontsize = 8;

	private Dictionary<Vector2i, Vector2i> _horizLines = new Dictionary<Vector2i, Vector2i>();

	private Dictionary<Vector2i, Vector2i> _horizLinesReversed = new Dictionary<Vector2i, Vector2i>();

	private Dictionary<Vector2i, Vector2i> _vertLines = new Dictionary<Vector2i, Vector2i>();

	private Dictionary<Vector2i, Vector2i> _vertLinesReversed = new Dictionary<Vector2i, Vector2i>();

	private NavMapComponent? _navMap;

	private MapGridComponent? _grid;

	private TransformComponent? _xform;

	private PhysicsComponent? _physics;

	private FixturesComponent? _fixtures;

	private readonly Label _zoom = new Label
	{
		VerticalAlignment = (VAlignment)1,
		HorizontalExpand = true,
		Margin = new Thickness(8f, 8f)
	};

	private readonly Button _recenter = new Button
	{
		Text = Loc.GetString("navmap-recenter"),
		VerticalAlignment = (VAlignment)1,
		HorizontalAlignment = (HAlignment)3,
		HorizontalExpand = true,
		Margin = new Thickness(8f, 4f),
		Disabled = true
	};

	private readonly CheckBox _beacons = new CheckBox
	{
		Text = Loc.GetString("navmap-toggle-beacons"),
		VerticalAlignment = (VAlignment)2,
		HorizontalAlignment = (HAlignment)2,
		HorizontalExpand = true,
		Margin = new Thickness(4f, 0f),
		Pressed = true
	};

	protected override bool Draggable => true;

	public event Action<NetEntity?>? TrackedEntitySelectedAction;

	public event Action<DrawingHandleScreen>? PostWallDrawingAction;

	public NavMapControl()
		: base(MinDisplayedRange, MaxDisplayedRange, DefaultDisplayedRange)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected O, but got Unknown
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Expected O, but got Unknown
		//IL_02db: Expected O, but got Unknown
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Expected O, but got Unknown
		//IL_0332: Expected O, but got Unknown
		IoCManager.InjectDependencies<NavMapControl>(this);
		_transformSystem = EntManager.System<SharedTransformSystem>();
		_navMapSystem = EntManager.System<SharedNavMapSystem>();
		BackgroundColor = Color.FromSrgb(((Color)(ref TileColor)).WithAlpha(BackgroundOpacity));
		((Control)this).RectClipContent = true;
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		PanelContainer val = new PanelContainer
		{
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = ((Color)(ref StyleNano.ButtonColorContext)).WithAlpha(1f),
				BorderColor = StyleNano.PanelDark
			},
			VerticalExpand = false,
			HorizontalExpand = true,
			SetWidth = 650f
		};
		OrderedChildCollection children = ((Control)val).Children;
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		((Control)val2).Children.Add((Control)(object)_zoom);
		((Control)val2).Children.Add((Control)(object)_beacons);
		((Control)val2).Children.Add((Control)(object)_recenter);
		children.Add((Control)val2);
		PanelContainer val3 = val;
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true
		};
		((Control)val4).Children.Add((Control)(object)val3);
		((Control)val4).Children.Add(new Control
		{
			Name = "DrawingControl",
			VerticalExpand = true,
			Margin = new Thickness(5f, 5f)
		});
		BoxContainer val5 = val4;
		((Control)this).AddChild((Control)(object)val5);
		((Control)val3).Measure(Vector2Helpers.Infinity);
		((BaseButton)_recenter).OnPressed += delegate
		{
			Recentering = true;
		};
		ForceNavMapUpdate();
	}

	public void ForceNavMapUpdate()
	{
		EntManager.TryGetComponent<NavMapComponent>(MapUid, ref _navMap);
		EntManager.TryGetComponent<MapGridComponent>(MapUid, ref _grid);
		EntManager.TryGetComponent<TransformComponent>(MapUid, ref _xform);
		EntManager.TryGetComponent<PhysicsComponent>(MapUid, ref _physics);
		EntManager.TryGetComponent<FixturesComponent>(MapUid, ref _fixtures);
		UpdateNavMap();
	}

	public void CenterToCoordinates(EntityCoordinates coordinates)
	{
		if (_physics != null)
		{
			Offset = new Vector2(((EntityCoordinates)(ref coordinates)).X, ((EntityCoordinates)(ref coordinates)).Y) - _physics.LocalCenter;
		}
		((BaseButton)_recenter).Disabled = false;
	}

	protected override void KeyBindUp(GUIBoundKeyEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		base.KeyBindUp(args);
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIClick)
		{
			if (this.TrackedEntitySelectedAction == null || _xform == null || _physics == null || TrackedEntities.Count == 0 || (StartDragPosition - ((BoundKeyEventArgs)args).PointerLocation.Position).Length() > MinDragDistance)
			{
				return;
			}
			Vector2 vector = Offset + _physics.LocalCenter;
			Vector2 vector2 = (((BoundKeyEventArgs)args).PointerLocation.Position - Vector2i.op_Implicit(((Control)this).GlobalPixelPosition) - base.MidPointVector) / base.MinimapScale;
			Vector2 vector3 = Vector2.Transform(new Vector2(vector2.X, 0f - vector2.Y) + vector, _transformSystem.GetWorldMatrix(_xform));
			NetEntity value = NetEntity.Invalid;
			float num = float.PositiveInfinity;
			foreach (var (val2, navMapBlip2) in TrackedEntities)
			{
				if (navMapBlip2.Selectable)
				{
					float num2 = (_transformSystem.ToMapCoordinates(navMapBlip2.Coordinates, true).Position - vector3).Length();
					if (!(num < num2) && !(num2 * base.MinimapScale > MaxSelectableDistance))
					{
						value = val2;
						num = num2;
					}
				}
			}
			if (!(num > MaxSelectableDistance) && ((NetEntity)(ref value)).IsValid())
			{
				this.TrackedEntitySelectedAction(value);
			}
		}
		else if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIRightClick)
		{
			this.TrackedEntitySelectedAction?.Invoke(null);
		}
		else if (((BoundKeyEventArgs)args).Function == ContentKeyFunctions.ExamineEntity)
		{
			((BaseButton)_beacons).Pressed = !((BaseButton)_beacons).Pressed;
		}
	}

	protected override void MouseMove(GUIMouseMoveEventArgs args)
	{
		base.MouseMove(args);
		if (Offset != Vector2.Zero)
		{
			((BaseButton)_recenter).Disabled = false;
		}
		else
		{
			((BaseButton)_recenter).Disabled = true;
		}
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Expected O, but got Unknown
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(handle);
		EntManager.TryGetComponent<NavMapComponent>(MapUid, ref _navMap);
		EntManager.TryGetComponent<MapGridComponent>(MapUid, ref _grid);
		EntManager.TryGetComponent<TransformComponent>(MapUid, ref _xform);
		EntManager.TryGetComponent<PhysicsComponent>(MapUid, ref _physics);
		EntManager.TryGetComponent<FixturesComponent>(MapUid, ref _fixtures);
		if (_navMap == null || _grid == null || _xform == null)
		{
			return;
		}
		((BaseButton)_recenter).Disabled = DrawRecenter();
		_zoom.Text = Loc.GetString("navmap-zoom", new(string, object)[1] { ("value", $"{DefaultDisplayedRange / WorldRange:0.0}") });
		Vector2 offset = Offset;
		if (_physics != null)
		{
			offset += _physics.LocalCenter;
		}
		Vector2 vector = new Vector2(offset.X, 0f - offset.Y);
		if (!_sRGBLookUp.TryGetValue(WallColor, out var value))
		{
			value = Color.ToSrgb(WallColor);
			_sRGBLookUp[WallColor] = value;
		}
		if (TilePolygons.Any())
		{
			Span<Vector2> span = new Vector2[8];
			foreach (var tilePolygon in TilePolygons)
			{
				Vector2[] item = tilePolygon.Item1;
				Color item2 = tilePolygon.Item2;
				for (int i = 0; i < item.Length; i++)
				{
					Vector2 vector2 = item[i] - offset;
					span[i] = ScalePosition(new Vector2(vector2.X, 0f - vector2.Y));
				}
				((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)2, (ReadOnlySpan<Vector2>)span.Slice(0, item.Length), item2);
			}
		}
		if (_grid != null)
		{
			UIBox2 val = default(UIBox2);
			foreach (NavMapRegionOverlay regionOverlay in RegionOverlays)
			{
				foreach (var gridCoord in regionOverlay.GridCoords)
				{
					Vector2 vector3 = ScalePosition(new Vector2(gridCoord.Item1.X, -gridCoord.Item1.Y) - new Vector2(offset.X, 0f - offset.Y));
					Vector2 vector4 = ScalePosition(new Vector2(gridCoord.Item2.X + _grid.TileSize, -gridCoord.Item2.Y - _grid.TileSize) - new Vector2(offset.X, 0f - offset.Y));
					((UIBox2)(ref val))._002Ector(vector3, vector4);
					handle.DrawRect(val, regionOverlay.Color, true);
				}
			}
		}
		if (TileLines.Any())
		{
			ValueList<Vector2> val2 = default(ValueList<Vector2>);
			val2._002Ector(TileLines.Count * 2);
			foreach (var tileLine in TileLines)
			{
				Vector2 item3 = tileLine.Item1;
				Vector2 item4 = tileLine.Item2;
				Vector2 vector5 = ScalePosition(item3 - vector);
				Vector2 vector6 = ScalePosition(item4 - vector);
				val2.Add(vector5);
				val2.Add(vector6);
			}
			if (val2.Count > 0)
			{
				((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)4, (ReadOnlySpan<Vector2>)val2.Span, value);
			}
		}
		if (TileRects.Any())
		{
			ValueList<Vector2> val3 = default(ValueList<Vector2>);
			val3._002Ector(TileRects.Count * 8);
			foreach (var tileRect in TileRects)
			{
				Vector2 item5 = tileRect.Item1;
				Vector2 item6 = tileRect.Item2;
				Vector2 vector7 = ScalePosition(item5 - vector);
				Vector2 vector8 = ScalePosition(item6 - vector);
				Vector2 vector9 = new Vector2(vector8.X, vector7.Y);
				Vector2 vector10 = new Vector2(vector7.X, vector8.Y);
				val3.Add(vector7);
				val3.Add(vector9);
				val3.Add(vector9);
				val3.Add(vector8);
				val3.Add(vector8);
				val3.Add(vector10);
				val3.Add(vector10);
				val3.Add(vector7);
			}
			if (val3.Count > 0)
			{
				((DrawingHandleBase)handle).DrawPrimitives((DrawPrimitiveTopology)4, (ReadOnlySpan<Vector2>)val3.Span, value);
			}
		}
		if (this.PostWallDrawingAction != null)
		{
			this.PostWallDrawingAction(handle);
		}
		TimeSpan realTime = Timing.RealTime;
		float num = 1f;
		bool flag = realTime.TotalSeconds % (double)num > (double)(num / 2f);
		foreach (var (val5, tuple2) in TrackedCoordinates)
		{
			if (flag && tuple2.Item1)
			{
				MapCoordinates val6 = _transformSystem.ToMapCoordinates(val5, true);
				if (val6.MapId != MapId.Nullspace)
				{
					Vector2 vector11 = Vector2.Transform(val6.Position, _transformSystem.GetInvWorldMatrix(_xform)) - offset;
					vector11 = ScalePosition(new Vector2(vector11.X, 0f - vector11.Y));
					((DrawingHandleBase)handle).DrawCircle(vector11, float.Sqrt(base.MinimapScale) * 2f, tuple2.Item2, true);
				}
			}
		}
		foreach (NavMapBlip value3 in TrackedEntities.Values)
		{
			if ((!value3.Blinks || flag) && value3.Texture != null)
			{
				MapCoordinates val7 = _transformSystem.ToMapCoordinates(value3.Coordinates, true);
				if (val7.MapId != MapId.Nullspace)
				{
					Vector2 vector12 = Vector2.Transform(val7.Position, _transformSystem.GetInvWorldMatrix(_xform)) - offset;
					vector12 = ScalePosition(new Vector2(vector12.X, 0f - vector12.Y));
					float num2 = MinmapScaleModifier * float.Sqrt(base.MinimapScale);
					Vector2 vector13 = new Vector2(num2 * value3.Scale * (float)value3.Texture.Width, num2 * value3.Scale * (float)value3.Texture.Height);
					handle.DrawTextureRect(value3.Texture, new UIBox2(vector12 - vector13, vector12 + vector13), (Color?)value3.Color);
				}
			}
		}
		if (!((BaseButton)_beacons).Pressed)
		{
			return;
		}
		Vector2 vector14 = new Vector2(5f, 3f);
		int num3 = (int)Math.Round(1f / WorldRange * DefaultDisplayedRange * ((Control)this).UIScale * (float)_targetFontsize, 0);
		VectorFont val8 = new VectorFont(_cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), num3);
		foreach (SharedNavMapSystem.NavMapBeacon value4 in _navMap.Beacons.Values)
		{
			Vector2 vector15 = value4.Position - offset;
			Vector2 value2 = vector15;
			value2.Y = 0f - vector15.Y;
			vector15 = ScalePosition(value2);
			Vector2 dimensions = handle.GetDimensions((Font)(object)val8, (ReadOnlySpan<char>)value4.Text, 1f);
			handle.DrawRect(new UIBox2(vector15 - dimensions / 2f - vector14, vector15 + dimensions / 2f + vector14), BackgroundColor, true);
			handle.DrawString((Font)(object)val8, vector15 - dimensions / 2f, value4.Text, value4.Color);
		}
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		_updateTimer += ((FrameEventArgs)(ref args)).DeltaSeconds;
		if (_updateTimer >= UpdateTime)
		{
			_updateTimer -= UpdateTime;
			UpdateNavMap();
		}
	}

	protected virtual void UpdateNavMap()
	{
		TilePolygons.Clear();
		TileLines.Clear();
		TileRects.Clear();
		UpdateNavMapFloorTiles();
		UpdateNavMapWallLines();
		UpdateNavMapAirlocks();
	}

	private void UpdateNavMapFloorTiles()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (_fixtures == null)
		{
			return;
		}
		Vector2[] array = new Vector2[8];
		foreach (Fixture value in _fixtures.Fixtures.Values)
		{
			IPhysShape shape = value.Shape;
			PolygonShape val = (PolygonShape)(object)((shape is PolygonShape) ? shape : null);
			if (val != null)
			{
				for (int i = 0; i < val.VertexCount; i++)
				{
					Vector2 vector = val.Vertices[i];
					array[i] = new Vector2(MathF.Round(vector.X), MathF.Round(vector.Y));
				}
				TilePolygons.Add((array[..val.VertexCount], TileColor));
			}
		}
	}

	private void UpdateNavMapWallLines()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		if (_navMap == null || _grid == null)
		{
			return;
		}
		_horizLines.Clear();
		_horizLinesReversed.Clear();
		_vertLines.Clear();
		_vertLinesReversed.Clear();
		Vector2i key;
		foreach (KeyValuePair<Vector2i, NavMapChunk> chunk in _navMap.Chunks)
		{
			chunk.Deconstruct(out key, out var value);
			Vector2i val = key;
			NavMapChunk navMapChunk = value;
			for (int i = 0; i < 64; i++)
			{
				int num = navMapChunk.TileData[i] & 0xF0;
				if (num == 0)
				{
					continue;
				}
				num >>= 4;
				Vector2i tileFromIndex = SharedNavMapSystem.GetTileFromIndex(i);
				Vector2i val2 = (navMapChunk.Origin * 8 + tileFromIndex) * (int)_grid.TileSize;
				if (num != 15)
				{
					AddRectForThinWall(num, val2);
					continue;
				}
				key = val2;
				key.Y = -val2.Y;
				val2 = key;
				int num2 = 0;
				NavMapChunk value2;
				if (tileFromIndex.Y != 7)
				{
					num2 = navMapChunk.TileData[i + 1];
				}
				else if (_navMap.Chunks.TryGetValue(val + Vector2i.Up, out value2))
				{
					num2 = value2.TileData[i + 1 - 8];
				}
				if ((num2 & 0x20) == 0)
				{
					AddOrUpdateNavMapLine(val2 + new Vector2i(0, -_grid.TileSize), val2 + new Vector2i((int)_grid.TileSize, -_grid.TileSize), _horizLines, _horizLinesReversed);
				}
				num2 = 0;
				if (tileFromIndex.X != 7)
				{
					num2 = navMapChunk.TileData[i + 8];
				}
				else if (_navMap.Chunks.TryGetValue(val + Vector2i.Right, out value2))
				{
					num2 = value2.TileData[i + 8 - 64];
				}
				if ((num2 & 0x80) == 0)
				{
					AddOrUpdateNavMapLine(val2 + new Vector2i((int)_grid.TileSize, -_grid.TileSize), val2 + new Vector2i((int)_grid.TileSize, 0), _vertLines, _vertLinesReversed);
				}
				num2 = 0;
				if (tileFromIndex.Y != 0)
				{
					num2 = navMapChunk.TileData[i - 1];
				}
				else if (_navMap.Chunks.TryGetValue(val + Vector2i.Down, out value2))
				{
					num2 = value2.TileData[i - 1 + 8];
				}
				if ((num2 & 0x10) == 0)
				{
					AddOrUpdateNavMapLine(val2, val2 + new Vector2i((int)_grid.TileSize, 0), _horizLines, _horizLinesReversed);
				}
				num2 = 0;
				if (tileFromIndex.X != 0)
				{
					num2 = navMapChunk.TileData[i - 8];
				}
				else if (_navMap.Chunks.TryGetValue(val + Vector2i.Left, out value2))
				{
					num2 = value2.TileData[i - 8 + 64];
				}
				if ((num2 & 0x40) == 0)
				{
					AddOrUpdateNavMapLine(val2 + new Vector2i(0, -_grid.TileSize), val2, _vertLines, _vertLinesReversed);
				}
				TileLines.Add((Vector2i.op_Implicit(val2) + new Vector2(0f, -_grid.TileSize), Vector2i.op_Implicit(val2) + new Vector2((int)_grid.TileSize, 0f)));
			}
		}
		Vector2i key2;
		foreach (KeyValuePair<Vector2i, Vector2i> horizLine in _horizLines)
		{
			horizLine.Deconstruct(out key, out key2);
			Vector2i val3 = key;
			Vector2i val4 = key2;
			TileLines.Add((Vector2i.op_Implicit(val3), Vector2i.op_Implicit(val4)));
		}
		foreach (KeyValuePair<Vector2i, Vector2i> vertLine in _vertLines)
		{
			vertLine.Deconstruct(out key2, out key);
			Vector2i val5 = key2;
			Vector2i val6 = key;
			TileLines.Add((Vector2i.op_Implicit(val5), Vector2i.op_Implicit(val6)));
		}
	}

	private void UpdateNavMapAirlocks()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (_navMap == null || _grid == null)
		{
			return;
		}
		foreach (NavMapChunk value in _navMap.Chunks.Values)
		{
			for (int i = 0; i < 64; i++)
			{
				int num = value.TileData[i] & 0xF00;
				if (num != 0)
				{
					num >>= 8;
					Vector2i tileFromIndex = SharedNavMapSystem.GetTileFromIndex(i);
					Vector2i val = (value.Origin * 8 + tileFromIndex) * (int)_grid.TileSize;
					if (num != 15)
					{
						AddRectForThinAirlock(num, val);
						continue;
					}
					TileRects.Add((new Vector2((float)val.X + FullWallInstep, (float)(-val.Y) - FullWallInstep), new Vector2((float)val.X - FullWallInstep + 1f, (float)(-val.Y) + FullWallInstep - 1f)));
					TileLines.Add((new Vector2((float)val.X + 0.5f, (float)(-val.Y) - FullWallInstep), new Vector2((float)val.X + 0.5f, (float)(-val.Y) + FullWallInstep - 1f)));
				}
			}
		}
	}

	private void AddRectForThinWall(int tileData, Vector2i tile)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = new Vector2(-0.5f, 0.5f - ThinWallThickness);
		Vector2 vector2 = new Vector2(0.5f, 0.5f);
		for (int i = 0; i < 4; i++)
		{
			int num = 1 << i;
			if ((tileData & num) != 0)
			{
				Vector2 vector3 = new Vector2((float)tile.X + 0.5f, (float)(-tile.Y) - 0.5f);
				Angle val = -((AtmosDirection)num).ToAngle();
				TileRects.Add((((Angle)(ref val)).RotateVec(ref vector) + vector3, ((Angle)(ref val)).RotateVec(ref vector2) + vector3));
			}
		}
	}

	private void AddRectForThinAirlock(int tileData, Vector2i tile)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = new Vector2(-0.5f + FullWallInstep, 0.5f - FullWallInstep - ThinDoorThickness);
		Vector2 vector2 = new Vector2(0.5f - FullWallInstep, 0.5f - FullWallInstep);
		Vector2 vector3 = new Vector2(0f, 0.5f - FullWallInstep - ThinDoorThickness);
		Vector2 vector4 = new Vector2(0f, 0.5f - FullWallInstep);
		for (int i = 0; i < 4; i++)
		{
			int num = 1 << i;
			if ((tileData & num) != 0)
			{
				Vector2 vector5 = new Vector2((float)tile.X + 0.5f, (float)(-tile.Y) - 0.5f);
				Angle val = -((AtmosDirection)num).ToAngle();
				TileRects.Add((((Angle)(ref val)).RotateVec(ref vector) + vector5, ((Angle)(ref val)).RotateVec(ref vector2) + vector5));
				TileLines.Add((((Angle)(ref val)).RotateVec(ref vector3) + vector5, ((Angle)(ref val)).RotateVec(ref vector4) + vector5));
			}
		}
	}

	protected void AddOrUpdateNavMapLine(Vector2i origin, Vector2i terminus, Dictionary<Vector2i, Vector2i> lookup, Dictionary<Vector2i, Vector2i> lookupReversed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (origin == terminus)
		{
			return;
		}
		Vector2i value2;
		if (lookup.Remove(terminus, out var value))
		{
			if (lookupReversed.Remove(origin, out value2))
			{
				lookup[value2] = value;
				lookupReversed[value] = value2;
			}
			else
			{
				lookup[origin] = value;
				lookupReversed[value] = origin;
			}
		}
		else if (lookupReversed.Remove(origin, out value2))
		{
			lookup[value2] = terminus;
			lookupReversed[terminus] = value2;
		}
		else
		{
			lookup.Add(origin, terminus);
			lookupReversed.Add(terminus, origin);
		}
	}

	protected Vector2 GetOffset()
	{
		Vector2 offset = Offset;
		PhysicsComponent? physics = _physics;
		return offset + ((physics != null) ? physics.LocalCenter : default(Vector2));
	}
}
