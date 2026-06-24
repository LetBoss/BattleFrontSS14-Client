using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Eye.Blinding.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class EyeClosingSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private BlindableSystem _blindableSystem;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private ISharedPlayerManager _playerManager;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EyeClosingComponent, MapInitEvent>((EntityEventRefHandler<EyeClosingComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeClosingComponent, ComponentShutdown>((EntityEventRefHandler<EyeClosingComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeClosingComponent, ToggleEyesActionEvent>((EntityEventRefHandler<EyeClosingComponent, ToggleEyesActionEvent>)OnToggleAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeClosingComponent, CanSeeAttemptEvent>((EntityEventRefHandler<EyeClosingComponent, CanSeeAttemptEvent>)OnTrySee, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EyeClosingComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<EyeClosingComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<EyeClosingComponent> eyelids, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		_actionsSystem.AddAction(Entity<EyeClosingComponent>.op_Implicit(eyelids), ref eyelids.Comp.EyeToggleActionEntity, eyelids.Comp.EyeToggleAction);
		((EntitySystem)this).Dirty<EyeClosingComponent>(eyelids, (MetaDataComponent)null);
	}

	private void OnShutdown(Entity<EyeClosingComponent> eyelids, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(eyelids.Owner);
		EntityUid? eyeToggleActionEntity = eyelids.Comp.EyeToggleActionEntity;
		actionsSystem.RemoveAction(performer, eyeToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(eyeToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SetEyelids(Entity<EyeClosingComponent>.op_Implicit((eyelids.Owner, eyelids.Comp)), value: false);
	}

	private void OnToggleAction(Entity<EyeClosingComponent> eyelids, ref ToggleEyesActionEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			SetEyelids(Entity<EyeClosingComponent>.op_Implicit((eyelids.Owner, eyelids.Comp)), !eyelids.Comp.EyesClosed);
		}
	}

	private void OnHandleState(Entity<EyeClosingComponent> eyelids, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		DoAudioFeedback(Entity<EyeClosingComponent>.op_Implicit((eyelids.Owner, eyelids.Comp)), eyelids.Comp.EyesClosed);
	}

	private void OnTrySee(Entity<EyeClosingComponent> eyelids, ref CanSeeAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (eyelids.Comp.EyesClosed)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	public bool AreEyesClosed(Entity<EyeClosingComponent?> eyelids)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EyeClosingComponent>(Entity<EyeClosingComponent>.op_Implicit(eyelids), ref eyelids.Comp, false))
		{
			return eyelids.Comp.EyesClosed;
		}
		return false;
	}

	public void SetEyelids(Entity<EyeClosingComponent?> eyelids, bool value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EyeClosingComponent>(Entity<EyeClosingComponent>.op_Implicit(eyelids), ref eyelids.Comp, true) && eyelids.Comp.EyesClosed != value)
		{
			eyelids.Comp.EyesClosed = value;
			((EntitySystem)this).Dirty<EyeClosingComponent>(eyelids, (MetaDataComponent)null);
			if (eyelids.Comp.EyeToggleActionEntity.HasValue)
			{
				SharedActionsSystem actionsSystem = _actionsSystem;
				EntityUid? eyeToggleActionEntity = eyelids.Comp.EyeToggleActionEntity;
				actionsSystem.SetToggled(eyeToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(eyeToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), eyelids.Comp.EyesClosed);
			}
			_blindableSystem.UpdateIsBlind(Entity<BlindableComponent>.op_Implicit(eyelids.Owner));
			DoAudioFeedback(eyelids, eyelids.Comp.EyesClosed);
		}
	}

	public void DoAudioFeedback(Entity<EyeClosingComponent?> eyelids, bool eyelidTarget)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EyeClosingComponent>(Entity<EyeClosingComponent>.op_Implicit(eyelids), ref eyelids.Comp, true) && _net.IsClient && _timing.IsFirstTimePredicted && eyelids.Comp.PreviousEyelidPosition != eyelidTarget)
		{
			eyelids.Comp.PreviousEyelidPosition = eyelidTarget;
			ICommonSession session = default(ICommonSession);
			if (_playerManager.TryGetSessionByEntity(Entity<EyeClosingComponent>.op_Implicit(eyelids), ref session))
			{
				_audio.PlayGlobal(eyelidTarget ? eyelids.Comp.EyeCloseSound : eyelids.Comp.EyeOpenSound, session, (AudioParams?)null);
			}
		}
	}

	public void UpdateEyesClosable(Entity<BlindableComponent?> blindable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BlindableComponent>(Entity<BlindableComponent>.op_Implicit(blindable), ref blindable.Comp, false))
		{
			return;
		}
		GetBlurEvent ev = new GetBlurEvent(blindable.Comp.EyeDamage);
		((EntitySystem)this).RaiseLocalEvent<GetBlurEvent>(blindable.Owner, ev, false);
		EyeClosingComponent eyelids = default(EyeClosingComponent);
		if (!((EntitySystem)this).TryComp<EyeClosingComponent>(Entity<BlindableComponent>.op_Implicit(blindable), ref eyelids) || eyelids.NaturallyCreated)
		{
			if (ev.Blur < 6f || ev.Blur >= (float)blindable.Comp.MaxDamage)
			{
				((EntitySystem)this).RemCompDeferred<EyeClosingComponent>(Entity<BlindableComponent>.op_Implicit(blindable));
				return;
			}
			((EntitySystem)this).EnsureComp<EyeClosingComponent>(Entity<BlindableComponent>.op_Implicit(blindable)).NaturallyCreated = true;
			((EntitySystem)this).Dirty<BlindableComponent>(blindable, (MetaDataComponent)null);
		}
	}
}
