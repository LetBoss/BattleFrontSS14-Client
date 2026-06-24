using System;
using Content.Client.UserInterface.Systems.DamageOverlays.Overlays;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Traits.Assorted;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.UserInterface.Systems.DamageOverlays;

public sealed class DamageOverlayUiController : UIController
{
	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[UISystemDependency]
	private readonly MobThresholdSystem _mobThresholdSystem;

	private DamageOverlay _overlay;

	public override void Initialize()
	{
		_overlay = new DamageOverlay();
		((UIController)this).SubscribeLocalEvent<LocalPlayerAttachedEvent>((EntityEventHandler<LocalPlayerAttachedEvent>)OnPlayerAttach, (Type[])null, (Type[])null);
		((UIController)this).SubscribeLocalEvent<LocalPlayerDetachedEvent>((EntityEventHandler<LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((UIController)this).SubscribeLocalEvent<MobStateChangedEvent>((EntityEventHandler<MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((UIController)this).SubscribeLocalEvent<MobThresholdChecked>((EntityEventRefHandler<MobThresholdChecked>)OnThresholdCheck, (Type[])null, (Type[])null);
	}

	private void OnPlayerAttach(LocalPlayerAttachedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ClearOverlay();
		MobStateComponent mobStateComponent = default(MobStateComponent);
		if (base.EntityManager.TryGetComponent<MobStateComponent>(args.Entity, ref mobStateComponent))
		{
			if (mobStateComponent.CurrentState != MobState.Dead)
			{
				UpdateOverlays(args.Entity, mobStateComponent);
			}
			_overlayManager.AddOverlay((Overlay)(object)_overlay);
		}
	}

	private void OnPlayerDetached(LocalPlayerDetachedEvent args)
	{
		_overlayManager.RemoveOverlay((Overlay)(object)_overlay);
		ClearOverlay();
	}

	private void OnMobStateChanged(MobStateChangedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(target != localEntity.GetValueOrDefault()))
		{
			UpdateOverlays(args.Target, args.Component);
		}
	}

	private void OnThresholdCheck(ref MobThresholdChecked args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(target != localEntity.GetValueOrDefault()))
		{
			UpdateOverlays(args.Target, args.MobState, args.Damageable, args.Threshold);
		}
	}

	private void ClearOverlay()
	{
		_overlay.DeadLevel = 0f;
		_overlay.CritLevel = 0f;
		_overlay.PainLevel = 0f;
		_overlay.OxygenLevel = 0f;
	}

	private void UpdateOverlays(EntityUid entity, MobStateComponent? mobState, DamageableComponent? damageable = null, MobThresholdsComponent? thresholds = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if ((mobState == null && !base.EntityManager.TryGetComponent<MobStateComponent>(entity, ref mobState)) || (thresholds == null && !base.EntityManager.TryGetComponent<MobThresholdsComponent>(entity, ref thresholds)) || (damageable == null && !base.EntityManager.TryGetComponent<DamageableComponent>(entity, ref damageable)) || !_mobThresholdSystem.TryGetIncapThreshold(entity, out var threshold, thresholds))
		{
			return;
		}
		if (!thresholds.ShowOverlays)
		{
			ClearOverlay();
			return;
		}
		FixedPoint2 value = threshold.Value;
		_overlay.State = mobState.CurrentState;
		switch (mobState.CurrentState)
		{
		case MobState.Alive:
		{
			FixedPoint2 fixedPoint = 0;
			_overlay.PainLevel = 0f;
			if (!base.EntityManager.HasComponent<PainNumbnessComponent>(entity))
			{
				foreach (ProtoId<DamageGroupPrototype> painDamageGroup in damageable.PainDamageGroups)
				{
					damageable.DamagePerGroup.TryGetValue(ProtoId<DamageGroupPrototype>.op_Implicit(painDamageGroup), out var value2);
					fixedPoint += value2;
				}
				_overlay.PainLevel = FixedPoint2.Min(1f, fixedPoint / value).Float();
				if (_overlay.PainLevel < 0.05f)
				{
					_overlay.PainLevel = 0f;
				}
			}
			if (damageable.DamagePerGroup.TryGetValue("Airloss", out var value3))
			{
				_overlay.OxygenLevel = FixedPoint2.Min(1f, value3 / value).Float();
			}
			_overlay.CritLevel = 0f;
			_overlay.DeadLevel = 0f;
			break;
		}
		case MobState.Critical:
		{
			if (_mobThresholdSystem.TryGetDeadPercentage(entity, FixedPoint2.Max(0.0, damageable.TotalDamage), out var percentage))
			{
				_overlay.CritLevel = percentage.Value.Float();
				_overlay.PainLevel = 0f;
				_overlay.DeadLevel = 0f;
			}
			break;
		}
		case MobState.Dead:
			_overlay.PainLevel = 0f;
			_overlay.CritLevel = 0f;
			break;
		}
	}
}
