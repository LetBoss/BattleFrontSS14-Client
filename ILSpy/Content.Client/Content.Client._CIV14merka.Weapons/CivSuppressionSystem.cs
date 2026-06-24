using System;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._CIV14merka.Weapons;
using Content.Shared.Ghost;
using Content.Shared.Humanoid;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._CIV14merka.Weapons;

public sealed class CivSuppressionSystem : SharedCivSuppressionSystem
{
	[Dependency]
	private IOverlayManager _overlays;

	[Dependency]
	private IPlayerManager _player;

	private bool _subscriptionsInitialized;

	private CivSuppressionOverlay? _overlay;

	private EntityUid? _trackedEntity;

	private float _currentIntensity;

	private float _stressIntensity;

	private float _settleIntensity;

	private float _shockIntensity;

	private float _previousIntensity;

	private float _pulse;

	private float _recoveryLock;

	private float _shockMultiplier = 1f;

	private float _recoveryDelay = 0.45f;

	private CivSuppressionVisualProfile _visualProfile;

	public float CurrentIntensity => _currentIntensity;

	public float Stress => Math.Clamp(MathF.Max(_stressIntensity, _settleIntensity * 0.9f), 0f, 1f);

	public float Pulse => _pulse;

	public CivSuppressionVisualProfile VisualProfile => _visualProfile;

	public bool Active
	{
		get
		{
			if (!(_currentIntensity > 0.01f) && !(_stressIntensity > 0.01f) && !(_settleIntensity > 0.01f))
			{
				return _pulse > 0.01f;
			}
			return true;
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		if (!_subscriptionsInitialized)
		{
			_subscriptionsInitialized = true;
			_overlay = new CivSuppressionOverlay(this);
			_overlays.AddOverlay((Overlay)(object)_overlay);
			((EntitySystem)this).SubscribeLocalEvent<LocalPlayerDetachedEvent>((EntityEventHandler<LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		if (_overlay != null && _overlays.HasOverlay(((object)_overlay).GetType()))
		{
			_overlays.RemoveOverlay((Overlay)(object)_overlay);
		}
		_overlay = null;
		_subscriptionsInitialized = false;
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).FrameUpdate(frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (!((EntitySystem)this).TerminatingOrDeleted(valueOrDefault, (MetaDataComponent)null) && CanDisplaySuppression(valueOrDefault))
			{
				float visualIntensity = GetVisualIntensity(valueOrDefault);
				GetVisualSettings(valueOrDefault, out var shockMultiplier, out var recoveryDelay, out var visualProfile);
				_shockMultiplier = shockMultiplier;
				_recoveryDelay = recoveryDelay;
				_visualProfile = visualProfile;
				localEntity = _trackedEntity;
				EntityUid val = valueOrDefault;
				if (!localEntity.HasValue || localEntity.GetValueOrDefault() != val)
				{
					_trackedEntity = valueOrDefault;
					_stressIntensity = visualIntensity * 0.16f;
					_settleIntensity = visualIntensity * 0.12f;
					_shockIntensity = 0f;
					_previousIntensity = visualIntensity;
					_currentIntensity = visualIntensity;
					_pulse = 0f;
					_recoveryLock = 0f;
					return;
				}
				GetProfileTuning(_visualProfile, out var shockFloorScale, out var shockDeltaScale, out var pulseDeltaScale, out var pulseIntensityScale, out var stressFloorScale, out var stressGainScale, out var stressDeltaScale, out var settleStressScale, out var settleIntensityScale, out var settleGainBase, out var settleGainIntensityScale, out var burstRecoveryBase, out var burstRecoveryIntensityScale, out var activeRecoveryBase, out var activeRecoveryIntensityScale, out var lockedStressDecay, out var freeStressDecay, out var lockedSettleDecay, out var freeSettleDecay, out var shockDecay, out var pulseDecay, out var stressOutputScale, out var settleOutputScale, out var shockOutputScale);
				float num = visualIntensity - _previousIntensity;
				if (num > 0.01f)
				{
					_shockIntensity = Math.Clamp(MathF.Max(_shockIntensity, visualIntensity * shockFloorScale * _shockMultiplier) + num * shockDeltaScale * _shockMultiplier, 0f, 1f);
					_pulse = Math.Clamp(_pulse + num * pulseDeltaScale * _shockMultiplier + visualIntensity * pulseIntensityScale, 0f, 1f);
					_recoveryLock = MathF.Max(_recoveryLock, _recoveryDelay * (burstRecoveryBase + visualIntensity * burstRecoveryIntensityScale));
				}
				if (visualIntensity > 0.01f)
				{
					_stressIntensity = Math.Clamp(MathF.Max(_stressIntensity, visualIntensity * stressFloorScale) + visualIntensity * frameTime * stressGainScale + MathF.Max(num, 0f) * stressDeltaScale * _shockMultiplier, 0f, 1f);
					_settleIntensity = Math.Clamp(MathF.Max(_settleIntensity, _stressIntensity * settleStressScale + visualIntensity * settleIntensityScale) + frameTime * (settleGainBase + visualIntensity * settleGainIntensityScale), 0f, 1f);
					_recoveryLock = MathF.Max(_recoveryLock, _recoveryDelay * (activeRecoveryBase + visualIntensity * activeRecoveryIntensityScale));
				}
				else
				{
					_recoveryLock = MathF.Max(0f, _recoveryLock - frameTime);
					float num2 = ((_recoveryLock > 0f) ? lockedStressDecay : freeStressDecay);
					float num3 = ((_recoveryLock > 0f) ? lockedSettleDecay : freeSettleDecay);
					_stressIntensity = MathF.Max(0f, _stressIntensity - frameTime * num2);
					_settleIntensity = MathF.Max(0f, _settleIntensity - frameTime * num3);
				}
				_shockIntensity = MathF.Max(0f, _shockIntensity - frameTime * shockDecay);
				_pulse = MathF.Max(0f, _pulse - frameTime * pulseDecay);
				_previousIntensity = visualIntensity;
				_currentIntensity = Math.Clamp(MathF.Max(MathF.Max(visualIntensity, _stressIntensity * stressOutputScale), _settleIntensity * settleOutputScale) + _shockIntensity * shockOutputScale, 0f, 1f);
				return;
			}
		}
		ResetState();
	}

	private void OnPlayerDetached(LocalPlayerDetachedEvent args)
	{
		ResetState();
	}

	private void ResetState()
	{
		_trackedEntity = null;
		_currentIntensity = 0f;
		_stressIntensity = 0f;
		_settleIntensity = 0f;
		_shockIntensity = 0f;
		_previousIntensity = 0f;
		_pulse = 0f;
		_recoveryLock = 0f;
		_shockMultiplier = 1f;
		_recoveryDelay = 0.45f;
		_visualProfile = CivSuppressionVisualProfile.IncomingFire;
	}

	private float GetVisualIntensity(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!CanDisplaySuppression(uid))
		{
			return 0f;
		}
		float num = GetCurrentIntensity(uid);
		CivSuppressedComponent civSuppressedComponent = default(CivSuppressedComponent);
		if (num <= 0f && ((EntitySystem)this).TryComp<CivSuppressedComponent>(uid, ref civSuppressedComponent) && civSuppressedComponent.Intensity > 0f)
		{
			num = civSuppressedComponent.Intensity;
		}
		return Math.Clamp(MathF.Pow(num, 0.95f) * 1.15f, 0f, 1f);
	}

	private bool CanDisplaySuppression(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<GhostComponent>(uid) || !((EntitySystem)this).HasComp<HumanoidAppearanceComponent>(uid))
		{
			return false;
		}
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (((EntitySystem)this).TryComp<CivTeamMemberComponent>(uid, ref civTeamMemberComponent))
		{
			return !civTeamMemberComponent.IsCommander;
		}
		return true;
	}

	private void GetVisualSettings(EntityUid uid, out float shockMultiplier, out float recoveryDelay, out CivSuppressionVisualProfile visualProfile)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		shockMultiplier = 1f;
		recoveryDelay = 0.45f;
		visualProfile = CivSuppressionVisualProfile.IncomingFire;
		CivSuppressedComponent civSuppressedComponent = default(CivSuppressedComponent);
		if (((EntitySystem)this).TryComp<CivSuppressedComponent>(uid, ref civSuppressedComponent))
		{
			shockMultiplier = Math.Max(0.25f, civSuppressedComponent.VisualShockMultiplier);
			recoveryDelay = Math.Max(0.05f, (float)civSuppressedComponent.VisualRecoveryDelay.TotalSeconds);
			visualProfile = civSuppressedComponent.VisualProfile;
		}
	}

	private static void GetProfileTuning(CivSuppressionVisualProfile profile, out float shockFloorScale, out float shockDeltaScale, out float pulseDeltaScale, out float pulseIntensityScale, out float stressFloorScale, out float stressGainScale, out float stressDeltaScale, out float settleStressScale, out float settleIntensityScale, out float settleGainBase, out float settleGainIntensityScale, out float burstRecoveryBase, out float burstRecoveryIntensityScale, out float activeRecoveryBase, out float activeRecoveryIntensityScale, out float lockedStressDecay, out float freeStressDecay, out float lockedSettleDecay, out float freeSettleDecay, out float shockDecay, out float pulseDecay, out float stressOutputScale, out float settleOutputScale, out float shockOutputScale)
	{
		switch (profile)
		{
		case CivSuppressionVisualProfile.Explosion:
			shockFloorScale = 0.38f;
			shockDeltaScale = 2.45f;
			pulseDeltaScale = 4.1f;
			pulseIntensityScale = 0.11f;
			stressFloorScale = 0.12f;
			stressGainScale = 0.038f;
			stressDeltaScale = 0.12f;
			settleStressScale = 0.16f;
			settleIntensityScale = 0.03f;
			settleGainBase = 0.004f;
			settleGainIntensityScale = 0.01f;
			burstRecoveryBase = 0.06f;
			burstRecoveryIntensityScale = 0.14f;
			activeRecoveryBase = 0.04f;
			activeRecoveryIntensityScale = 0.09f;
			lockedStressDecay = 0.58f;
			freeStressDecay = 1.28f;
			lockedSettleDecay = 0.42f;
			freeSettleDecay = 0.98f;
			shockDecay = 2.9f;
			pulseDecay = 2.35f;
			stressOutputScale = 0.42f;
			settleOutputScale = 0.2f;
			shockOutputScale = 0.24f;
			break;
		case CivSuppressionVisualProfile.Mortar:
			shockFloorScale = 0.3f;
			shockDeltaScale = 1.85f;
			pulseDeltaScale = 3.3f;
			pulseIntensityScale = 0.085f;
			stressFloorScale = 0.18f;
			stressGainScale = 0.058f;
			stressDeltaScale = 0.2f;
			settleStressScale = 0.22f;
			settleIntensityScale = 0.05f;
			settleGainBase = 0.006f;
			settleGainIntensityScale = 0.016f;
			burstRecoveryBase = 0.1f;
			burstRecoveryIntensityScale = 0.16f;
			activeRecoveryBase = 0.08f;
			activeRecoveryIntensityScale = 0.12f;
			lockedStressDecay = 0.4f;
			freeStressDecay = 0.86f;
			lockedSettleDecay = 0.28f;
			freeSettleDecay = 0.64f;
			shockDecay = 2.05f;
			pulseDecay = 1.72f;
			stressOutputScale = 0.58f;
			settleOutputScale = 0.32f;
			shockOutputScale = 0.14f;
			break;
		default:
			shockFloorScale = 0.18f;
			shockDeltaScale = 1.18f;
			pulseDeltaScale = 2.15f;
			pulseIntensityScale = 0.035f;
			stressFloorScale = 0.18f;
			stressGainScale = 0.075f;
			stressDeltaScale = 0.18f;
			settleStressScale = 0.28f;
			settleIntensityScale = 0.08f;
			settleGainBase = 0.008f;
			settleGainIntensityScale = 0.022f;
			burstRecoveryBase = 0.18f;
			burstRecoveryIntensityScale = 0.22f;
			activeRecoveryBase = 0.12f;
			activeRecoveryIntensityScale = 0.18f;
			lockedStressDecay = 0.28f;
			freeStressDecay = 0.62f;
			lockedSettleDecay = 0.2f;
			freeSettleDecay = 0.46f;
			shockDecay = 1.65f;
			pulseDecay = 1.45f;
			stressOutputScale = 0.7f;
			settleOutputScale = 0.48f;
			shockOutputScale = 0.08f;
			break;
		}
	}
}
