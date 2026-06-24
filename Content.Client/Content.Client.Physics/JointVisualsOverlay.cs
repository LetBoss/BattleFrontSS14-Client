using System.Numerics;
using Content.Shared.Physics;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Physics;

public sealed class JointVisualsOverlay : Overlay
{
	private IEntityManager _entManager;

	public override OverlaySpace Space => (OverlaySpace)8;

	public JointVisualsOverlay(IEntityManager entManager)
	{
		_entManager = entManager;
	}

	protected override void Draw(in OverlayDrawArgs args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		DrawingHandleWorld worldHandle = ((OverlayDrawArgs)(ref args)).WorldHandle;
		SpriteSystem val = _entManager.System<SpriteSystem>();
		SharedTransformSystem val2 = _entManager.System<SharedTransformSystem>();
		EntityQueryEnumerator<JointVisualsComponent, TransformComponent> val3 = _entManager.EntityQueryEnumerator<JointVisualsComponent, TransformComponent>();
		EntityQuery<TransformComponent> entityQuery = _entManager.GetEntityQuery<TransformComponent>();
		DrawingHandleBase drawingHandle = args.DrawingHandle;
		Matrix3x2 identity = Matrix3x2.Identity;
		drawingHandle.SetTransform(ref identity);
		JointVisualsComponent jointVisualsComponent = default(JointVisualsComponent);
		TransformComponent val4 = default(TransformComponent);
		TransformComponent val5 = default(TransformComponent);
		Box2 val10 = default(Box2);
		Box2Rotated val11 = default(Box2Rotated);
		while (val3.MoveNext(ref jointVisualsComponent, ref val4))
		{
			if (!(val4.MapID != args.MapId))
			{
				EntityUid? entity = _entManager.GetEntity(jointVisualsComponent.Target);
				if (entityQuery.TryGetComponent(entity, ref val5) && !(val4.MapID != val5.MapID))
				{
					Texture val6 = val.Frame0(jointVisualsComponent.Sprite);
					float num = (float)val6.Width / 32f;
					EntityCoordinates val7 = val4.Coordinates;
					EntityCoordinates val8 = val5.Coordinates;
					Angle localRotation = val4.LocalRotation;
					Angle localRotation2 = val5.LocalRotation;
					val7 = ((EntityCoordinates)(ref val7)).Offset(((Angle)(ref localRotation)).RotateVec(ref jointVisualsComponent.OffsetA));
					val8 = ((EntityCoordinates)(ref val8)).Offset(((Angle)(ref localRotation2)).RotateVec(ref jointVisualsComponent.OffsetB));
					Vector2 position = val2.ToMapCoordinates(val7, true).Position;
					Vector2 position2 = val2.ToMapCoordinates(val8, true).Position;
					Vector2 vector = position2 - position;
					float num2 = vector.Length();
					Vector2 vector2 = vector / 2f + position;
					Angle val9 = DirectionExtensions.ToWorldAngle(position2 - position);
					((Box2)(ref val10))._002Ector((0f - num) / 2f, (0f - num2) / 2f, num / 2f, num2 / 2f);
					((Box2Rotated)(ref val11))._002Ector(((Box2)(ref val10)).Translated(vector2), val9, vector2);
					worldHandle.DrawTextureRect(val6, ref val11, (Color?)null);
				}
			}
		}
	}
}
