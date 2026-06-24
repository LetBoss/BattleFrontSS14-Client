using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared.Chasm;

public sealed class ChasmSystem : EntitySystem
{
	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ActionBlockerSystem _blocker;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ChasmComponent, StepTriggeredOffEvent>((ComponentEventRefHandler<ChasmComponent, StepTriggeredOffEvent>)OnStepTriggered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChasmComponent, StepTriggerAttemptEvent>((ComponentEventRefHandler<ChasmComponent, StepTriggerAttemptEvent>)OnStepTriggerAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChasmFallingComponent, UpdateCanMoveEvent>((ComponentEventHandler<ChasmFallingComponent, UpdateCanMoveEvent>)OnUpdateCanMove, (Type[])null, (Type[])null);
	}

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<ChasmFallingComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ChasmFallingComponent>();
		EntityUid uid = default(EntityUid);
		ChasmFallingComponent chasm = default(ChasmFallingComponent);
		MobStateComponent comp = default(MobStateComponent);
		while (query.MoveNext(ref uid, ref chasm))
		{
			if (!(_timing.CurTime < chasm.NextDeletionTime))
			{
				if (((EntitySystem)this).TryComp<MobStateComponent>(uid, ref comp))
				{
					_mobState.ChangeMobState(uid, MobState.Dead, comp);
				}
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
		}
	}

	private void OnStepTriggered(EntityUid uid, ChasmComponent component, ref StepTriggeredOffEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<ChasmFallingComponent>(args.Tripper))
		{
			StartFalling(uid, component, args.Tripper);
		}
	}

	public void StartFalling(EntityUid chasm, ChasmComponent component, EntityUid tripper, bool playSound = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		ChasmFallingComponent falling = ((EntitySystem)this).AddComp<ChasmFallingComponent>(tripper);
		falling.NextDeletionTime = _timing.CurTime + falling.DeletionTime;
		_blocker.UpdateCanMove(tripper);
		if (playSound)
		{
			_audio.PlayPredicted(component.FallingSound, chasm, (EntityUid?)tripper, (AudioParams?)null);
		}
	}

	private void OnStepTriggerAttempt(EntityUid uid, ChasmComponent component, ref StepTriggerAttemptEvent args)
	{
		args.Continue = true;
	}

	private void OnUpdateCanMove(EntityUid uid, ChasmFallingComponent component, UpdateCanMoveEvent args)
	{
		((CancellableEntityEventArgs)args).Cancel();
	}
}
