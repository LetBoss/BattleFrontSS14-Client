using System;
using System.Numerics;
using Content.Shared.Gravity;
using Content.Shared.Movement.Components;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Movement.Systems;

public sealed class SharedJumpAbilitySystem : EntitySystem
{
	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedGravitySystem _gravity;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<JumpAbilityComponent, GravityJumpEvent>((EntityEventRefHandler<JumpAbilityComponent, GravityJumpEvent>)OnGravityJump, (Type[])null, (Type[])null);
	}

	private void OnGravityJump(Entity<JumpAbilityComponent> entity, ref GravityJumpEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		if (!_gravity.IsWeightless(args.Performer))
		{
			TransformComponent obj = ((EntitySystem)this).Transform(args.Performer);
			Angle localRotation = obj.LocalRotation;
			Vector2 throwing = ((Angle)(ref localRotation)).ToWorldVec() * entity.Comp.JumpDistance;
			EntityCoordinates coordinates = obj.Coordinates;
			EntityCoordinates direction = ((EntityCoordinates)(ref coordinates)).Offset(throwing);
			_throwing.TryThrow(args.Performer, direction, entity.Comp.JumpThrowSpeed);
			_audio.PlayPredicted(entity.Comp.JumpSound, args.Performer, (EntityUid?)args.Performer, (AudioParams?)null);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}
}
