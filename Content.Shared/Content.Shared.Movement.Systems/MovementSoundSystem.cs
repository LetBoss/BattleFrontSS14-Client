using System;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Movement.Systems;

public sealed class MovementSoundSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MovementSoundComponent, MoveInputEvent>((EntityEventRefHandler<MovementSoundComponent, MoveInputEvent>)OnMoveInput, (Type[])null, (Type[])null);
	}

	private void OnMoveInput(Entity<MovementSoundComponent> ent, ref MoveInputEvent args)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.IsFirstTimePredicted)
		{
			return;
		}
		bool num = (SharedMoverController.GetNormalizedMovement(args.OldMovement) & MoveButtons.AnyDirection) != 0;
		bool moving = (SharedMoverController.GetNormalizedMovement(args.Entity.Comp.HeldMoveButtons) & MoveButtons.AnyDirection) != 0;
		if (num != moving)
		{
			if (moving)
			{
				ent.Comp.SoundEntity = _audio.PlayPredicted(ent.Comp.Sound, ent.Owner, (EntityUid?)ent.Owner, (AudioParams?)null)?.Item1;
			}
			else
			{
				ent.Comp.SoundEntity = _audio.Stop(ent.Comp.SoundEntity, (AudioComponent)null);
			}
		}
	}
}
