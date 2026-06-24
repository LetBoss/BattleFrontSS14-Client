// Decompiled with JetBrains decompiler
// Type: Content.Client.Camera.CameraRecoilSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Camera;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Camera;

public sealed class CameraRecoilSystem : SharedCameraRecoilSystem
{
  [Dependency]
  private IConfigurationManager _configManager;
  private float _intensity;
  private float _pubgIntensity = 1f;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CameraKickEvent>(new EntityEventHandler<CameraKickEvent>(this.OnCameraKick), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._configManager, CCVars.ScreenShakeIntensity, new Action<float>(this.OnCvarChanged), true);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._configManager, CCVars.PubgShootingScreenShakeIntensity, new Action<float>(this.OnPubgCvarChanged), true);
  }

  private void OnCvarChanged(float value) => this._intensity = value;

  private void OnPubgCvarChanged(float value) => this._pubgIntensity = Math.Clamp(value, 0.0f, 1f);

  private void OnCameraKick(CameraKickEvent ev)
  {
    this.KickCamera(this.GetEntity(ev.NetEntity), ev.Recoil, (CameraRecoilComponent) null);
  }

  public override void KickCamera(EntityUid uid, Vector2 recoil, CameraRecoilComponent? component = null)
  {
    float num1 = this._intensity * this._pubgIntensity;
    if ((double) num1 <= 0.0 || !this.Resolve<CameraRecoilComponent>(uid, ref component, false))
      return;
    recoil *= num1;
    float num2 = component.CurrentKick.Length() / 1f;
    component.CurrentKick += recoil * (1f - num2);
    if ((double) component.CurrentKick.Length() > 1.0)
      component.CurrentKick = Vector2Helpers.Normalized(component.CurrentKick) * 1f;
    component.LastKickTime = 0.0f;
  }
}
