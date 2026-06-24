// Decompiled with JetBrains decompiler
// Type: Content.Client.Salvage.FultonSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Salvage.Fulton;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Spawners;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Salvage;

public sealed class FultonSystem : SharedFultonSystem
{
  [Dependency]
  private ISerializationManager _serManager;
  [Dependency]
  private AnimationPlayerSystem _player;
  [Dependency]
  private SpriteSystem _sprite;
  private static readonly TimeSpan AnimationDuration = TimeSpan.FromSeconds(0.4);
  private static readonly Animation InitialAnimation = new Animation()
  {
    Length = FultonSystem.AnimationDuration,
    AnimationTracks = {
      (AnimationTrack) new AnimationTrackSpriteFlick()
      {
        LayerKey = (object) FultonSystem.FultonVisualLayers.Base,
        KeyFrames = {
          new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("fulton_expand"), 0.0f),
          new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("fulton_balloon"), 0.4f)
        }
      }
    }
  };
  private static readonly Animation FultonAnimation;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FultonedComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<FultonedComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<SharedFultonSystem.FultonAnimationMessage>(new EntityEventHandler<SharedFultonSystem.FultonAnimationMessage>(this.OnFultonMessage), (Type[]) null, (Type[]) null);
  }

  private void OnFultonMessage(SharedFultonSystem.FultonAnimationMessage ev)
  {
    EntityUid entity = this.GetEntity(ev.Entity);
    EntityCoordinates coordinates = this.GetCoordinates(ev.Coordinates);
    SpriteComponent spriteComponent1;
    if (this.Deleted(entity, (MetaDataComponent) null) || !this.TryComp<SpriteComponent>(entity, ref spriteComponent1))
      return;
    EntityUid entityUid = this.Spawn((string) null, coordinates);
    SpriteComponent spriteComponent2 = this.AddComp<SpriteComponent>(entityUid);
    this._serManager.CopyTo<SpriteComponent>(spriteComponent1, ref spriteComponent2, (ISerializationContext) null, false, true);
    AppearanceComponent appearanceComponent1;
    if (this.TryComp<AppearanceComponent>(entity, ref appearanceComponent1))
    {
      AppearanceComponent appearanceComponent2 = this.AddComp<AppearanceComponent>(entityUid);
      this._serManager.CopyTo<AppearanceComponent>(appearanceComponent1, ref appearanceComponent2, (ISerializationContext) null, false, true);
    }
    spriteComponent2.NoRotation = true;
    int num = this._sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent2)), (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("Objects/Tools/fulton_balloon.rsi"), "fulton_balloon"), new int?());
    this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent2)), num, SharedFultonSystem.EffectOffset + new Vector2(0.0f, 0.5f));
    this.AddComp<TimedDespawnComponent>(entityUid).Lifetime = 1.5f;
    this._player.Play(entityUid, FultonSystem.FultonAnimation, "fulton-animation");
  }

  private void OnHandleState(
    EntityUid uid,
    FultonedComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateAppearance(uid, component);
  }

  protected override void UpdateAppearance(EntityUid uid, FultonedComponent component)
  {
    EntityUid effect = component.Effect;
    if (!((EntityUid) ref effect).IsValid() || this.Timing.CurTime - (component.NextFulton - component.FultonDuration) >= FultonSystem.AnimationDuration)
      return;
    this._player.Play(component.Effect, FultonSystem.InitialAnimation, "fulton");
  }

  static FultonSystem()
  {
    Animation animation = new Animation();
    animation.Length = TimeSpan.FromSeconds(0.800000011920929);
    List<AnimationTrack> animationTracks = animation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Offset";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) new Vector2(0.0f, -0.3f), 0.3f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) new Vector2(0.0f, 20f), 0.5f, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    FultonSystem.FultonAnimation = animation;
  }

  public enum FultonVisualLayers : byte
  {
    Base,
  }
}
