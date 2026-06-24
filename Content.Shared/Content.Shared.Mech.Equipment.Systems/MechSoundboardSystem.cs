using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Timing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Mech.Equipment.Systems;

public sealed class MechSoundboardSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private UseDelaySystem _useDelay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MechSoundboardComponent, MechEquipmentUiStateReadyEvent>((ComponentEventHandler<MechSoundboardComponent, MechEquipmentUiStateReadyEvent>)OnUiStateReady, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechSoundboardComponent, MechEquipmentUiMessageRelayEvent>((ComponentEventHandler<MechSoundboardComponent, MechEquipmentUiMessageRelayEvent>)OnSoundboardMessage, (Type[])null, (Type[])null);
	}

	private void OnUiStateReady(EntityUid uid, MechSoundboardComponent comp, MechEquipmentUiStateReadyEvent args)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<string> sounds = comp.Sounds.Select((SoundCollectionSpecifier sound) => sound.Collection);
		MechSoundboardUiState state = new MechSoundboardUiState
		{
			Sounds = sounds.ToList()
		};
		args.States.Add(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null), (BoundUserInterfaceState)(object)state);
	}

	private void OnSoundboardMessage(EntityUid uid, MechSoundboardComponent comp, MechEquipmentUiMessageRelayEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		MechEquipmentComponent equipment = default(MechEquipmentComponent);
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (args.Message is MechSoundboardPlayMessage msg && ((EntitySystem)this).TryComp<MechEquipmentComponent>(uid, ref equipment) && equipment.EquipmentOwner.HasValue && msg.Sound < comp.Sounds.Count && (!((EntitySystem)this).TryComp<UseDelayComponent>(uid, ref useDelay) || _useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((uid, useDelay)), checkDelayed: true)))
		{
			_audio.PlayPvs((SoundSpecifier)(object)comp.Sounds[msg.Sound], uid, (AudioParams?)null);
		}
	}
}
