using System;
using System.Numerics;
using Content.Shared.FixedPoint;
using Content.Shared.Fluids;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Fluids;

public sealed class PuddleOverlay : Overlay
{
	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IEntitySystemManager _entitySystemManager;

	private readonly PuddleDebugOverlaySystem _debugOverlaySystem;

	private readonly SharedTransformSystem _transformSystem;

	private readonly Color _heavyPuddle = new Color((byte)0, byte.MaxValue, byte.MaxValue, (byte)50);

	private readonly Color _mediumPuddle = new Color((byte)0, (byte)150, byte.MaxValue, (byte)50);

	private readonly Color _lightPuddle = new Color((byte)0, (byte)50, byte.MaxValue, (byte)50);

	private readonly Font _font;

	public override OverlaySpace Space => (OverlaySpace)6;

	public PuddleOverlay()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		IoCManager.InjectDependencies<PuddleOverlay>(this);
		_debugOverlaySystem = _entitySystemManager.GetEntitySystem<PuddleDebugOverlaySystem>();
		IResourceCache val = IoCManager.Resolve<IResourceCache>();
		_font = (Font)new VectorFont(val.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf", true), 8);
		_transformSystem = _entityManager.System<SharedTransformSystem>();
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		OverlaySpace space = args.Space;
		if ((int)space != 2)
		{
			if ((int)space == 4)
			{
				DrawWorld(in args);
			}
		}
		else
		{
			DrawScreen(in args);
		}
	}

	private void DrawWorld(in OverlayDrawArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		EntityQuery<TransformComponent> entityQuery = _entityManager.GetEntityQuery<TransformComponent>();
		MapGridComponent val = default(MapGridComponent);
		foreach (EntityUid key in _debugOverlaySystem.TileData.Keys)
		{
			if (!_entityManager.TryGetComponent<MapGridComponent>(key, ref val))
			{
				continue;
			}
			TransformComponent component = entityQuery.GetComponent(key);
			ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = _transformSystem.GetWorldPositionRotationMatrixWithInv(component, entityQuery);
			Matrix3x2 item = worldPositionRotationMatrixWithInv.Item3;
			Box2 val2 = Matrix3Helpers.TransformBox(worldPositionRotationMatrixWithInv.Item4, ref args.WorldBounds);
			Box2 val3 = ((Box2)(ref val2)).Enlarged((float)(val.TileSize * 2));
			((DrawingHandleBase)worldHandle).SetTransform(ref item);
			PuddleDebugOverlayData[] data = _debugOverlaySystem.GetData(key);
			for (int i = 0; i < data.Length; i++)
			{
				PuddleDebugOverlayData puddleDebugOverlayData = data[i];
				Vector2 vector = (Vector2i.op_Implicit(puddleDebugOverlayData.Pos) + Vector2Helpers.Half) * (int)val.TileSize;
				if (((Box2)(ref val3)).Contains(vector, true))
				{
					Box2 val4 = ((Box2)(ref Box2.UnitCentered)).Translated(vector);
					worldHandle.DrawRect(val4, Color.Blue, false);
					worldHandle.DrawRect(val4, ColorMap(puddleDebugOverlayData.CurrentVolume), true);
				}
			}
		}
		Matrix3x2 identity = Matrix3x2.Identity;
		((DrawingHandleBase)worldHandle).SetTransform(ref identity);
	}

	private void DrawScreen(in OverlayDrawArgs args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleScreen screenHandle = ((OverlayDrawArgs)(ref args)).ScreenHandle;
		EntityQuery<TransformComponent> entityQuery = _entityManager.GetEntityQuery<TransformComponent>();
		MapGridComponent val = default(MapGridComponent);
		foreach (EntityUid key in _debugOverlaySystem.TileData.Keys)
		{
			if (!_entityManager.TryGetComponent<MapGridComponent>(key, ref val))
			{
				continue;
			}
			TransformComponent component = entityQuery.GetComponent(key);
			ValueTuple<Vector2, Angle, Matrix3x2, Matrix3x2> worldPositionRotationMatrixWithInv = _transformSystem.GetWorldPositionRotationMatrixWithInv(component, entityQuery);
			Matrix3x2 item = worldPositionRotationMatrixWithInv.Item3;
			Box2 val2 = Matrix3Helpers.TransformBox(worldPositionRotationMatrixWithInv.Item4, ref args.WorldBounds);
			Box2 val3 = ((Box2)(ref val2)).Enlarged((float)(val.TileSize * 2));
			PuddleDebugOverlayData[] data = _debugOverlaySystem.GetData(key);
			for (int i = 0; i < data.Length; i++)
			{
				PuddleDebugOverlayData puddleDebugOverlayData = data[i];
				Vector2 vector = (Vector2i.op_Implicit(puddleDebugOverlayData.Pos) + Vector2Helpers.Half) * (int)val.TileSize;
				if (((Box2)(ref val3)).Contains(vector, true))
				{
					Vector2 vector2 = _eyeManager.WorldToScreen(Vector2.Transform(vector, item));
					screenHandle.DrawString(_font, vector2, puddleDebugOverlayData.CurrentVolume.ToString(), Color.White);
				}
			}
		}
	}

	private Color ColorMap(FixedPoint2 intensity)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 fixedPoint = 1 - intensity / FixedPoint2.New(20f);
		if (!(fixedPoint < 0.5f))
		{
			return Color.InterpolateBetween(_lightPuddle, _mediumPuddle, (fixedPoint.Float() - 0.5f) * 2f);
		}
		return Color.InterpolateBetween(_mediumPuddle, _heavyPuddle, fixedPoint.Float() * 2f);
	}
}
