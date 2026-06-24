using System;
using Content.Shared.MouseRotator;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.MouseRotator;

public sealed class MouseRotatorSystem : SharedMouseRotatorSystem
{
	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Update(float frameTime)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		base.Update(frameTime);
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		ScreenCoordinates mouseScreenPosition = _input.MouseScreenPosition;
		if (!((ScreenCoordinates)(ref mouseScreenPosition)).IsValid)
		{
			return;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		MouseRotatorComponent mouseRotatorComponent = default(MouseRotatorComponent);
		if (!localEntity.HasValue || !((EntitySystem)this).TryComp<MouseRotatorComponent>(localEntity, ref mouseRotatorComponent))
		{
			return;
		}
		TransformComponent val = ((EntitySystem)this).Transform(localEntity.Value);
		ScreenCoordinates mouseScreenPosition2 = _input.MouseScreenPosition;
		MapCoordinates val2 = _eye.PixelToMap(mouseScreenPosition2);
		if (val2.MapId == MapId.Nullspace)
		{
			return;
		}
		Angle val3 = DirectionExtensions.ToWorldAngle(val2.Position - _transform.GetMapCoordinates(localEntity.Value, val).Position);
		Angle worldRotation = _transform.GetWorldRotation(val);
		Angle val4;
		if (mouseRotatorComponent.Simple4DirMode)
		{
			Angle rotation = _eye.CurrentEye.Rotation;
			val4 = val3 + rotation;
			Direction cardinalDir = ((Angle)(ref val4)).GetCardinalDir();
			val4 = worldRotation + rotation;
			if (cardinalDir != ((Angle)(ref val4)).GetCardinalDir())
			{
				Angle val5 = DirectionExtensions.ToAngle(cardinalDir) - rotation;
				if (Angle.op_Implicit(val5) >= Math.PI)
				{
					val5 -= Angle.op_Implicit(Math.PI * 2.0);
				}
				else if (Angle.op_Implicit(val5) < -Math.PI)
				{
					val5 += Angle.op_Implicit(Math.PI * 2.0);
				}
				((EntitySystem)this).RaisePredictiveEvent<RequestMouseRotatorRotationEvent>(new RequestMouseRotatorRotationEvent
				{
					Rotation = val5
				});
			}
		}
		else
		{
			if (Math.Abs(Angle.ShortestDistance(ref val3, ref worldRotation).Theta) < mouseRotatorComponent.AngleTolerance.Theta)
			{
				return;
			}
			if (mouseRotatorComponent.GoalRotation.HasValue)
			{
				val4 = mouseRotatorComponent.GoalRotation.Value;
				if (Math.Abs(Angle.ShortestDistance(ref val3, ref val4).Theta) < mouseRotatorComponent.AngleTolerance.Theta)
				{
					return;
				}
			}
			((EntitySystem)this).RaisePredictiveEvent<RequestMouseRotatorRotationEvent>(new RequestMouseRotatorRotationEvent
			{
				Rotation = val3
			});
		}
	}
}
