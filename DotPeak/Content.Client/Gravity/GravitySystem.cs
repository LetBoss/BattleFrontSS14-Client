// Decompiled with JetBrains decompiler
// Type: Content.Client.Gravity.GravitySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Camera;
using Content.Shared.Gravity;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Gravity;

public sealed class GravitySystem : SharedGravitySystem
{
  [Dependency]
  private AppearanceSystem _appearanceSystem;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedCameraRecoilSystem _sharedCameraRecoil;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SharedGravityGeneratorComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<SharedGravityGeneratorComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
    this.InitializeShake();
  }

  private void OnAppearanceChange(
    EntityUid uid,
    SharedGravityGeneratorComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    PowerChargeStatus key;
    string str;
    if (((SharedAppearanceSystem) this._appearanceSystem).TryGetData<PowerChargeStatus>(uid, (Enum) PowerChargeVisuals.State, ref key, args.Component) && comp.SpriteMap.TryGetValue(key, out str))
    {
      int num = this._sprite.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) GravityGeneratorVisualLayers.Base);
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(str));
    }
    float num1;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<float>(uid, (Enum) PowerChargeVisuals.Charge, ref num1, args.Component))
      return;
    int num2 = this._sprite.LayerMapGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) GravityGeneratorVisualLayers.Core);
    if ((double) num1 >= 0.20000000298023224)
    {
      if ((double) num1 >= 0.20000000298023224)
      {
        if ((double) num1 >= 0.40000000596046448)
        {
          if ((double) num1 >= 0.60000002384185791)
          {
            if ((double) num1 < 0.800000011920929)
            {
              this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, true);
              this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, RSI.StateId.op_Implicit(comp.CoreActivatingState));
              return;
            }
          }
          else
          {
            this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, true);
            this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, RSI.StateId.op_Implicit(comp.CoreIdleState));
            return;
          }
        }
        else
        {
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, true);
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, RSI.StateId.op_Implicit(comp.CoreStartupState));
          return;
        }
      }
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, true);
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, RSI.StateId.op_Implicit(comp.CoreActivatedState));
    }
    else
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, false);
  }

  private void InitializeShake()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GravityShakeComponent, ComponentInit>(new ComponentEventHandler<GravityShakeComponent, ComponentInit>((object) this, __methodptr(OnShakeInit)), (Type[]) null, (Type[]) null);
  }

  private void OnShakeInit(EntityUid uid, GravityShakeComponent component, ComponentInit args)
  {
    TransformComponent transformComponent;
    if (!this.TryComp(((ISharedPlayerManager) this._playerManager).LocalEntity, ref transformComponent))
      return;
    EntityUid? gridUid = transformComponent.GridUid;
    EntityUid entityUid1 = uid;
    if ((gridUid.HasValue ? (EntityUid.op_Inequality(gridUid.GetValueOrDefault(), entityUid1) ? 1 : 0) : 1) != 0)
    {
      EntityUid? mapUid = transformComponent.MapUid;
      EntityUid entityUid2 = uid;
      if ((mapUid.HasValue ? (EntityUid.op_Inequality(mapUid.GetValueOrDefault(), entityUid2) ? 1 : 0) : 1) != 0)
        return;
    }
    GravityComponent gravityComponent;
    if (!this.Timing.IsFirstTimePredicted || !this.TryComp<GravityComponent>(uid, ref gravityComponent))
      return;
    this._audio.PlayGlobal(gravityComponent.GravityShakeSound, Filter.Local(), true, new AudioParams?(((AudioParams) ref AudioParams.Default).WithVolume(-2f)));
  }

  protected override void ShakeGrid(EntityUid uid, GravityComponent? gravity = null)
  {
    base.ShakeGrid(uid, gravity);
    if (!this.Resolve<GravityComponent>(uid, ref gravity, true) || !this.Timing.IsFirstTimePredicted)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    TransformComponent transformComponent;
    if (!this.TryComp(localEntity, ref transformComponent))
      return;
    EntityUid? nullable = transformComponent.GridUid;
    EntityUid entityUid1 = uid;
    if ((nullable.HasValue ? (EntityUid.op_Inequality(nullable.GetValueOrDefault(), entityUid1) ? 1 : 0) : 1) != 0)
      return;
    nullable = transformComponent.GridUid;
    if (!nullable.HasValue)
    {
      nullable = transformComponent.MapUid;
      EntityUid entityUid2 = uid;
      if ((nullable.HasValue ? (EntityUid.op_Inequality(nullable.GetValueOrDefault(), entityUid2) ? 1 : 0) : 1) != 0)
        return;
    }
    Vector2 kickback = new Vector2(this._random.NextFloat(), this._random.NextFloat()) * 100f;
    this._sharedCameraRecoil.KickCamera(localEntity.Value, kickback);
  }
}
