// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Weapons.CivSuppressionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Teams;
using Content.Shared._CIV14merka.Weapons;
using Content.Shared.Ghost;
using Content.Shared.Humanoid;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
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

  public float CurrentIntensity => this._currentIntensity;

  public float Stress
  {
    get => Math.Clamp(MathF.Max(this._stressIntensity, this._settleIntensity * 0.9f), 0.0f, 1f);
  }

  public float Pulse => this._pulse;

  public CivSuppressionVisualProfile VisualProfile => this._visualProfile;

  public bool Active
  {
    get
    {
      return (double) this._currentIntensity > 0.0099999997764825821 || (double) this._stressIntensity > 0.0099999997764825821 || (double) this._settleIntensity > 0.0099999997764825821 || (double) this._pulse > 0.0099999997764825821;
    }
  }

  public override void Initialize()
  {
    base.Initialize();
    if (this._subscriptionsInitialized)
      return;
    this._subscriptionsInitialized = true;
    this._overlay = new CivSuppressionOverlay(this);
    this._overlays.AddOverlay((Overlay) this._overlay);
    this.SubscribeLocalEvent<LocalPlayerDetachedEvent>(new EntityEventHandler<LocalPlayerDetachedEvent>(this.OnPlayerDetached), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlay != null && this._overlays.HasOverlay(((object) this._overlay).GetType()))
      this._overlays.RemoveOverlay((Overlay) this._overlay);
    this._overlay = (CivSuppressionOverlay) null;
    this._subscriptionsInitialized = false;
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (localEntity.HasValue)
    {
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      if (!this.TerminatingOrDeleted(valueOrDefault, (MetaDataComponent) null) && this.CanDisplaySuppression(valueOrDefault))
      {
        float visualIntensity = this.GetVisualIntensity(valueOrDefault);
        float shockMultiplier;
        float recoveryDelay;
        CivSuppressionVisualProfile visualProfile;
        this.GetVisualSettings(valueOrDefault, out shockMultiplier, out recoveryDelay, out visualProfile);
        this._shockMultiplier = shockMultiplier;
        this._recoveryDelay = recoveryDelay;
        this._visualProfile = visualProfile;
        EntityUid? trackedEntity = this._trackedEntity;
        EntityUid entityUid = valueOrDefault;
        if ((trackedEntity.HasValue ? (EntityUid.op_Inequality(trackedEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0)
        {
          this._trackedEntity = new EntityUid?(valueOrDefault);
          this._stressIntensity = visualIntensity * 0.16f;
          this._settleIntensity = visualIntensity * 0.12f;
          this._shockIntensity = 0.0f;
          this._previousIntensity = visualIntensity;
          this._currentIntensity = visualIntensity;
          this._pulse = 0.0f;
          this._recoveryLock = 0.0f;
          return;
        }
        float shockFloorScale;
        float shockDeltaScale;
        float pulseDeltaScale;
        float pulseIntensityScale;
        float stressFloorScale;
        float stressGainScale;
        float stressDeltaScale;
        float settleStressScale;
        float settleIntensityScale;
        float settleGainBase;
        float settleGainIntensityScale;
        float burstRecoveryBase;
        float burstRecoveryIntensityScale;
        float activeRecoveryBase;
        float activeRecoveryIntensityScale;
        float lockedStressDecay;
        float freeStressDecay;
        float lockedSettleDecay;
        float freeSettleDecay;
        float shockDecay;
        float pulseDecay;
        float stressOutputScale;
        float settleOutputScale;
        float shockOutputScale;
        CivSuppressionSystem.GetProfileTuning(this._visualProfile, out shockFloorScale, out shockDeltaScale, out pulseDeltaScale, out pulseIntensityScale, out stressFloorScale, out stressGainScale, out stressDeltaScale, out settleStressScale, out settleIntensityScale, out settleGainBase, out settleGainIntensityScale, out burstRecoveryBase, out burstRecoveryIntensityScale, out activeRecoveryBase, out activeRecoveryIntensityScale, out lockedStressDecay, out freeStressDecay, out lockedSettleDecay, out freeSettleDecay, out shockDecay, out pulseDecay, out stressOutputScale, out settleOutputScale, out shockOutputScale);
        float x = visualIntensity - this._previousIntensity;
        if ((double) x > 0.0099999997764825821)
        {
          this._shockIntensity = Math.Clamp(MathF.Max(this._shockIntensity, visualIntensity * shockFloorScale * this._shockMultiplier) + x * shockDeltaScale * this._shockMultiplier, 0.0f, 1f);
          this._pulse = Math.Clamp((float) ((double) this._pulse + (double) x * (double) pulseDeltaScale * (double) this._shockMultiplier + (double) visualIntensity * (double) pulseIntensityScale), 0.0f, 1f);
          this._recoveryLock = MathF.Max(this._recoveryLock, this._recoveryDelay * (burstRecoveryBase + visualIntensity * burstRecoveryIntensityScale));
        }
        if ((double) visualIntensity > 0.0099999997764825821)
        {
          this._stressIntensity = Math.Clamp((float) ((double) MathF.Max(this._stressIntensity, visualIntensity * stressFloorScale) + (double) visualIntensity * (double) frameTime * (double) stressGainScale + (double) MathF.Max(x, 0.0f) * (double) stressDeltaScale * (double) this._shockMultiplier), 0.0f, 1f);
          this._settleIntensity = Math.Clamp(MathF.Max(this._settleIntensity, (float) ((double) this._stressIntensity * (double) settleStressScale + (double) visualIntensity * (double) settleIntensityScale)) + frameTime * (settleGainBase + visualIntensity * settleGainIntensityScale), 0.0f, 1f);
          this._recoveryLock = MathF.Max(this._recoveryLock, this._recoveryDelay * (activeRecoveryBase + visualIntensity * activeRecoveryIntensityScale));
        }
        else
        {
          this._recoveryLock = MathF.Max(0.0f, this._recoveryLock - frameTime);
          float num1 = (double) this._recoveryLock > 0.0 ? lockedStressDecay : freeStressDecay;
          float num2 = (double) this._recoveryLock > 0.0 ? lockedSettleDecay : freeSettleDecay;
          this._stressIntensity = MathF.Max(0.0f, this._stressIntensity - frameTime * num1);
          this._settleIntensity = MathF.Max(0.0f, this._settleIntensity - frameTime * num2);
        }
        this._shockIntensity = MathF.Max(0.0f, this._shockIntensity - frameTime * shockDecay);
        this._pulse = MathF.Max(0.0f, this._pulse - frameTime * pulseDecay);
        this._previousIntensity = visualIntensity;
        this._currentIntensity = Math.Clamp(MathF.Max(MathF.Max(visualIntensity, this._stressIntensity * stressOutputScale), this._settleIntensity * settleOutputScale) + this._shockIntensity * shockOutputScale, 0.0f, 1f);
        return;
      }
    }
    this.ResetState();
  }

  private void OnPlayerDetached(LocalPlayerDetachedEvent args) => this.ResetState();

  private void ResetState()
  {
    this._trackedEntity = new EntityUid?();
    this._currentIntensity = 0.0f;
    this._stressIntensity = 0.0f;
    this._settleIntensity = 0.0f;
    this._shockIntensity = 0.0f;
    this._previousIntensity = 0.0f;
    this._pulse = 0.0f;
    this._recoveryLock = 0.0f;
    this._shockMultiplier = 1f;
    this._recoveryDelay = 0.45f;
    this._visualProfile = CivSuppressionVisualProfile.IncomingFire;
  }

  private float GetVisualIntensity(EntityUid uid)
  {
    if (!this.CanDisplaySuppression(uid))
      return 0.0f;
    float x = this.GetCurrentIntensity(uid);
    CivSuppressedComponent suppressedComponent;
    if ((double) x <= 0.0 && this.TryComp<CivSuppressedComponent>(uid, ref suppressedComponent) && (double) suppressedComponent.Intensity > 0.0)
      x = suppressedComponent.Intensity;
    return Math.Clamp(MathF.Pow(x, 0.95f) * 1.15f, 0.0f, 1f);
  }

  private bool CanDisplaySuppression(EntityUid uid)
  {
    if (this.HasComp<GhostComponent>(uid) || !this.HasComp<HumanoidAppearanceComponent>(uid))
      return false;
    CivTeamMemberComponent teamMemberComponent;
    return !this.TryComp<CivTeamMemberComponent>(uid, ref teamMemberComponent) || !teamMemberComponent.IsCommander;
  }

  private void GetVisualSettings(
    EntityUid uid,
    out float shockMultiplier,
    out float recoveryDelay,
    out CivSuppressionVisualProfile visualProfile)
  {
    shockMultiplier = 1f;
    recoveryDelay = 0.45f;
    visualProfile = CivSuppressionVisualProfile.IncomingFire;
    CivSuppressedComponent suppressedComponent;
    if (!this.TryComp<CivSuppressedComponent>(uid, ref suppressedComponent))
      return;
    shockMultiplier = Math.Max(0.25f, suppressedComponent.VisualShockMultiplier);
    recoveryDelay = Math.Max(0.05f, (float) suppressedComponent.VisualRecoveryDelay.TotalSeconds);
    visualProfile = suppressedComponent.VisualProfile;
  }

  private static void GetProfileTuning(
    CivSuppressionVisualProfile profile,
    out float shockFloorScale,
    out float shockDeltaScale,
    out float pulseDeltaScale,
    out float pulseIntensityScale,
    out float stressFloorScale,
    out float stressGainScale,
    out float stressDeltaScale,
    out float settleStressScale,
    out float settleIntensityScale,
    out float settleGainBase,
    out float settleGainIntensityScale,
    out float burstRecoveryBase,
    out float burstRecoveryIntensityScale,
    out float activeRecoveryBase,
    out float activeRecoveryIntensityScale,
    out float lockedStressDecay,
    out float freeStressDecay,
    out float lockedSettleDecay,
    out float freeSettleDecay,
    out float shockDecay,
    out float pulseDecay,
    out float stressOutputScale,
    out float settleOutputScale,
    out float shockOutputScale)
  {
    if (profile != CivSuppressionVisualProfile.Explosion)
    {
      if (profile == CivSuppressionVisualProfile.Mortar)
      {
        shockFloorScale = 0.3f;
        shockDeltaScale = 1.85f;
        pulseDeltaScale = 3.3f;
        pulseIntensityScale = 0.085f;
        stressFloorScale = 0.18f;
        stressGainScale = 0.058f;
        stressDeltaScale = 0.2f;
        settleStressScale = 0.22f;
        settleIntensityScale = 0.05f;
        settleGainBase = 3f / 500f;
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
      }
      else
      {
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
      }
    }
    else
    {
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
    }
  }
}
