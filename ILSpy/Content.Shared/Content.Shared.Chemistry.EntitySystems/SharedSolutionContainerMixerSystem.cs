using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared.Chemistry.EntitySystems;

public abstract class SharedSolutionContainerMixerSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SolutionContainerMixerComponent, ActivateInWorldEvent>((EntityEventRefHandler<SolutionContainerMixerComponent, ActivateInWorldEvent>)OnActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionContainerMixerComponent, ContainerIsRemovingAttemptEvent>((EntityEventRefHandler<SolutionContainerMixerComponent, ContainerIsRemovingAttemptEvent>)OnRemoveAttempt, (Type[])null, (Type[])null);
	}

	private void OnActivateInWorld(Entity<SolutionContainerMixerComponent> entity, ref ActivateInWorldEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && args.Complex)
		{
			TryStartMix(entity, args.User);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnRemoveAttempt(Entity<SolutionContainerMixerComponent> ent, ref ContainerIsRemovingAttemptEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (((ContainerAttemptEventBase)args).Container.ID == ent.Comp.ContainerId && ent.Comp.Mixing)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	protected virtual bool HasPower(Entity<SolutionContainerMixerComponent> entity)
	{
		return true;
	}

	public void TryStartMix(Entity<SolutionContainerMixerComponent> entity, EntityUid? user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionContainerMixerComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		SolutionContainerMixerComponent solutionContainerMixerComponent = default(SolutionContainerMixerComponent);
		val.Deconstruct(ref val2, ref solutionContainerMixerComponent);
		EntityUid uid = val2;
		SolutionContainerMixerComponent comp = solutionContainerMixerComponent;
		if (comp.Mixing)
		{
			return;
		}
		if (!HasPower(entity))
		{
			if (user.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("solution-container-mixer-no-power"), Entity<SolutionContainerMixerComponent>.op_Implicit(entity), user.Value);
			}
			return;
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(uid, comp.ContainerId, ref container, (ContainerManagerComponent)null) || container.Count == 0)
		{
			if (user.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("solution-container-mixer-popup-nothing-to-mix"), Entity<SolutionContainerMixerComponent>.op_Implicit(entity), user.Value);
			}
			return;
		}
		comp.Mixing = true;
		if (_net.IsServer)
		{
			SharedAudioSystem audio = _audio;
			SoundSpecifier? mixingSound = comp.MixingSound;
			EntityUid val3 = Entity<SolutionContainerMixerComponent>.op_Implicit(entity);
			SoundSpecifier? mixingSound2 = comp.MixingSound;
			AudioParams? val4;
			if (mixingSound2 == null)
			{
				val4 = null;
			}
			else
			{
				AudioParams val5 = mixingSound2.Params;
				val4 = ((AudioParams)(ref val5)).WithLoop(true);
			}
			(EntityUid, AudioComponent)? tuple = audio.PlayPvs(mixingSound, val3, val4);
			comp.MixingSoundEntity = (tuple.HasValue ? new Entity<AudioComponent>?(Entity<AudioComponent>.op_Implicit(tuple.GetValueOrDefault())) : ((Entity<AudioComponent>?)null));
		}
		comp.MixTimeEnd = _timing.CurTime + comp.MixDuration;
		_appearance.SetData(Entity<SolutionContainerMixerComponent>.op_Implicit(entity), (Enum)SolutionContainerMixerVisuals.Mixing, (object)true, (AppearanceComponent)null);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	public void StopMix(Entity<SolutionContainerMixerComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionContainerMixerComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		SolutionContainerMixerComponent solutionContainerMixerComponent = default(SolutionContainerMixerComponent);
		val.Deconstruct(ref val2, ref solutionContainerMixerComponent);
		EntityUid uid = val2;
		SolutionContainerMixerComponent comp = solutionContainerMixerComponent;
		if (comp.Mixing)
		{
			SharedAudioSystem audio = _audio;
			Entity<AudioComponent>? mixingSoundEntity = comp.MixingSoundEntity;
			audio.Stop(mixingSoundEntity.HasValue ? new EntityUid?(Entity<AudioComponent>.op_Implicit(mixingSoundEntity.GetValueOrDefault())) : ((EntityUid?)null), (AudioComponent)null);
			_appearance.SetData(Entity<SolutionContainerMixerComponent>.op_Implicit(entity), (Enum)SolutionContainerMixerVisuals.Mixing, (object)false, (AppearanceComponent)null);
			comp.Mixing = false;
			comp.MixingSoundEntity = null;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	public void FinishMix(Entity<SolutionContainerMixerComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionContainerMixerComponent> val = entity;
		EntityUid val2 = default(EntityUid);
		SolutionContainerMixerComponent solutionContainerMixerComponent = default(SolutionContainerMixerComponent);
		val.Deconstruct(ref val2, ref solutionContainerMixerComponent);
		EntityUid uid = val2;
		SolutionContainerMixerComponent comp = solutionContainerMixerComponent;
		if (!comp.Mixing)
		{
			return;
		}
		StopMix(entity);
		ReactionMixerComponent reactionMixer = default(ReactionMixerComponent);
		BaseContainer container = default(BaseContainer);
		if (!((EntitySystem)this).TryComp<ReactionMixerComponent>(Entity<SolutionContainerMixerComponent>.op_Implicit(entity), ref reactionMixer) || !_container.TryGetContainer(uid, comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		foreach (EntityUid ent in container.ContainedEntities)
		{
			if (_solution.TryGetFitsInDispenser(Entity<FitsInDispenserComponent, SolutionContainerManagerComponent>.op_Implicit(ent), out Entity<SolutionComponent>? soln, out Solution _))
			{
				_solution.UpdateChemicals(soln.Value, needsReactionsProcessing: true, reactionMixer);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<SolutionContainerMixerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SolutionContainerMixerComponent>();
		EntityUid uid = default(EntityUid);
		SolutionContainerMixerComponent comp = default(SolutionContainerMixerComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (comp.Mixing && !(_timing.CurTime < comp.MixTimeEnd))
			{
				FinishMix(Entity<SolutionContainerMixerComponent>.op_Implicit((uid, comp)));
			}
		}
	}
}
