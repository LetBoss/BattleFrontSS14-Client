// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Explosion.PubgExplosionShakeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Camera;
using Content.Shared.Explosion.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Random;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Explosion;

public sealed class PubgExplosionShakeSystem : EntitySystem
{
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedCameraRecoilSystem _recoil;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private IRobustRandom _random;
  private const float ShakeRadius = 12f;
  private const float MaxKick = 1f;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ExplosionVisualsComponent, ComponentStartup>(new ComponentEventHandler<ExplosionVisualsComponent, ComponentStartup>((object) this, __methodptr(OnExplosion)), (Type[]) null, (Type[]) null);
  }

  private void OnExplosion(
    EntityUid uid,
    ExplosionVisualsComponent component,
    ComponentStartup args)
  {
    EntityUid? localEntity = this._player.LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    MapCoordinates epicenter = component.Epicenter;
    TransformComponent transformComponent;
    if (!this.TryComp(valueOrDefault, ref transformComponent) || MapId.op_Inequality(transformComponent.MapID, epicenter.MapId))
      return;
    float num1 = (this._transform.GetWorldPosition(valueOrDefault) - epicenter.Position).Length();
    if ((double) num1 >= 12.0)
      return;
    float num2 = (float) (1.0 - (double) num1 / 12.0);
    float num3 = 1f * num2 * num2;
    if ((double) num3 <= 0.0)
      return;
    float x = this._random.NextFloat(0.0f, 6.28318548f);
    Vector2 kickback = new Vector2(MathF.Cos(x), MathF.Sin(x)) * num3;
    this._recoil.KickCamera(valueOrDefault, kickback);
  }
}
