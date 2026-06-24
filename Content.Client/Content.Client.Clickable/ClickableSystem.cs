using System;
using System.Numerics;
using Content.Client.Sprite;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Clickable;

public sealed class ClickableSystem : EntitySystem
{
	[Dependency]
	private IClickMapManager _clickMapManager;

	[Dependency]
	private SharedTransformSystem _transforms;

	[Dependency]
	private SpriteSystem _sprites;

	private EntityQuery<ClickableComponent> _clickableQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	private EntityQuery<FadingSpriteComponent> _fadingSpriteQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_clickableQuery = ((EntitySystem)this).GetEntityQuery<ClickableComponent>();
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
		_fadingSpriteQuery = ((EntitySystem)this).GetEntityQuery<FadingSpriteComponent>();
	}

	public bool CheckClick(Entity<ClickableComponent?, SpriteComponent, TransformComponent?, FadingSpriteComponent?> entity, Vector2 worldPos, IEye eye, bool excludeFaded, out int drawDepth, out uint renderOrder, out float bottom)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		if (!_clickableQuery.Resolve(entity.Owner, ref entity.Comp1, false))
		{
			drawDepth = 0;
			renderOrder = 0u;
			bottom = 0f;
			return false;
		}
		if (!_xformQuery.Resolve(entity.Owner, ref entity.Comp3, true))
		{
			drawDepth = 0;
			renderOrder = 0u;
			bottom = 0f;
			return false;
		}
		if (excludeFaded && _fadingSpriteQuery.Resolve(entity.Owner, ref entity.Comp4, false))
		{
			drawDepth = 0;
			renderOrder = 0u;
			bottom = 0f;
			return false;
		}
		SpriteComponent comp = entity.Comp2;
		TransformComponent comp2 = entity.Comp3;
		if (!comp.Visible)
		{
			drawDepth = 0;
			renderOrder = 0u;
			bottom = 0f;
			return false;
		}
		drawDepth = comp.DrawDepth;
		renderOrder = comp.RenderOrder;
		ValueTuple<Vector2, Angle> worldPositionRotation = _transforms.GetWorldPositionRotation(comp2);
		Vector2 item = worldPositionRotation.Item1;
		Angle item2 = worldPositionRotation.Item2;
		Box2Rotated val = _sprites.CalculateBounds(Entity<SpriteComponent>.op_Implicit((entity.Owner, comp)), item, item2, eye.Rotation);
		bottom = Matrix3Helpers.TransformBox(Matrix3Helpers.CreateRotation(Angle.op_Implicit(eye.Rotation)), ref val).Bottom;
		Matrix3x2.Invert(comp.LocalMatrix, out var result);
		Angle val2 = item2 + eye.Rotation;
		val2 = ((Angle)(ref val2)).Reduced();
		Angle val3 = ((Angle)(ref val2)).FlipPositive();
		Angle val4 = (comp.SnapCardinals ? DirectionExtensions.ToAngle(((Angle)(ref val3)).GetCardinalDir()) : Angle.Zero);
		val2 = (comp.NoRotation ? (-eye.Rotation) : (item2 - val4));
		Matrix3x2 matrix = Matrix3Helpers.CreateInverseTransform(ref item, ref val2);
		Vector2 vector = Vector2.Transform(Vector2.Transform(worldPos, matrix), result);
		if (CheckDirBound(Entity<ClickableComponent, SpriteComponent>.op_Implicit((entity.Owner, entity.Comp1, entity.Comp2)), val3, vector))
		{
			return true;
		}
		State val6 = default(State);
		Matrix3x2 matrix2 = default(Matrix3x2);
		foreach (ISpriteLayer allLayer in comp.AllLayers)
		{
			Layer val5 = (Layer)(object)((allLayer is Layer) ? allLayer : null);
			if (val5 == null || !_sprites.IsVisible(val5))
			{
				continue;
			}
			if (val5.Texture != null)
			{
				Vector2i pos = (Vector2i)(vector * 32f * new Vector2(1f, -1f) + val5.Texture.Size / 2f);
				if (_clickMapManager.IsOccluding(val5.Texture, pos))
				{
					return true;
				}
			}
			RSI actualRsi = val5.ActualRsi;
			if (actualRsi != null && actualRsi.TryGetState(val5.State, ref val6))
			{
				RsiDirection val7 = Layer.GetDirection(val6.RsiDirections, val3);
				val5.GetLayerDrawMatrix(val7, ref matrix2);
				Matrix3x2.Invert(matrix2, out var result2);
				Vector2i pos2 = (Vector2i)(Vector2.Transform(vector, result2) * 32f * new Vector2(1f, -1f) + val6.Size / 2f);
				if (comp.EnableDirectionOverride)
				{
					val7 = DirExt.Convert(comp.DirectionOverride, val6.RsiDirections);
				}
				val7 = DirExt.OffsetRsiDir(val7, val5.DirOffset);
				if (_clickMapManager.IsOccluding(val5.ActualRsi, val5.State, val7, val5.AnimationFrame, pos2))
				{
					return true;
				}
			}
		}
		drawDepth = 0;
		renderOrder = 0u;
		bottom = 0f;
		return false;
	}

	public bool CheckDirBound(Entity<ClickableComponent, SpriteComponent> entity, Angle relativeRotation, Vector2 localPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected I4, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		ClickableComponent comp = entity.Comp1;
		SpriteComponent comp2 = entity.Comp2;
		if (comp.Bounds == null)
		{
			return false;
		}
		Direction cardinalDir = ((Angle)(ref relativeRotation)).GetCardinalDir();
		Vector2 vector;
		if (!comp2.NoRotation)
		{
			Angle val = DirectionExtensions.ToAngle(cardinalDir);
			vector = ((Angle)(ref val)).RotateVec(ref localPos);
		}
		else
		{
			vector = localPos;
		}
		Vector2 vector2 = vector;
		if (((Box2)(ref comp.Bounds.All)).Contains(vector2, true))
		{
			return true;
		}
		Direction val2 = (comp2.EnableDirectionOverride ? comp2.DirectionOverride : cardinalDir);
		Box2 val3 = (Box2)((int)val2 switch
		{
			2 => comp.Bounds.East, 
			4 => comp.Bounds.North, 
			0 => comp.Bounds.South, 
			6 => comp.Bounds.West, 
			_ => throw new InvalidOperationException(), 
		});
		return ((Box2)(ref val3)).Contains(vector2, true);
	}
}
