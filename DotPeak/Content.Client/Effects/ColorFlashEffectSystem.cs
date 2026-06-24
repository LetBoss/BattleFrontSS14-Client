// Decompiled with JetBrains decompiler
// Type: Content.Client.Effects.ColorFlashEffectSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Effects;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Effects;

public sealed class ColorFlashEffectSystem : SharedColorFlashEffectSystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private SpriteSystem _sprite;
  private const float AnimationLength = 0.3f;
  private const string AnimationKey = "color-flash-effect";
  private ValueList<EntityUid> _toRemove;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<ColorFlashEffectEvent>(new EntityEventHandler<ColorFlashEffectEvent>(this.OnColorFlashEffect), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ColorFlashEffectComponent, AnimationCompletedEvent>(new ComponentEventHandler<ColorFlashEffectComponent, AnimationCompletedEvent>((object) this, __methodptr(OnEffectAnimationCompleted)), (Type[]) null, (Type[]) null);
  }

  public override void RaiseEffect(Color color, List<EntityUid> entities, Filter filter)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    this.OnColorFlashEffect(new ColorFlashEffectEvent(color, this.GetNetEntityList(entities)));
  }

  private void OnEffectAnimationCompleted(
    EntityUid uid,
    ColorFlashEffectComponent component,
    AnimationCompletedEvent args)
  {
    SpriteComponent spriteComponent;
    if (args.Key != "color-flash-effect" || !this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), component.Color);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    AllEntityQueryEnumerator<ColorFlashEffectComponent> entityQueryEnumerator = this.AllEntityQuery<ColorFlashEffectComponent>();
    this._toRemove.Clear();
    EntityUid entityUid1;
    ColorFlashEffectComponent flashEffectComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid1, ref flashEffectComponent))
    {
      if (!this._animation.HasRunningAnimation(entityUid1, "color-flash-effect"))
        this._toRemove.Add(entityUid1);
    }
    foreach (EntityUid entityUid2 in this._toRemove)
      this.RemComp<ColorFlashEffectComponent>(entityUid2);
  }

  private Animation? GetDamageAnimation(EntityUid uid, Color color, SpriteComponent? sprite = null)
  {
    if (!this.Resolve<SpriteComponent>(uid, ref sprite, false))
      return (Animation) null;
    Animation damageAnimation = new Animation();
    damageAnimation.Length = TimeSpan.FromSeconds(0.30000001192092896);
    List<AnimationTrack> animationTracks = damageAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Color";
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) color, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) sprite.Color, 0.3f, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    return damageAnimation;
  }

  private void OnColorFlashEffect(ColorFlashEffectEvent ev)
  {
    Color color = ev.Color;
    foreach (NetEntity entity1 in ev.Entities)
    {
      EntityUid entity2 = this.GetEntity(entity1);
      SpriteComponent sprite;
      if (!this.Deleted(entity2, (MetaDataComponent) null) && this.TryComp<SpriteComponent>(entity2, ref sprite))
      {
        ColorFlashEffectComponent flashEffectComponent;
        this.TryComp<ColorFlashEffectComponent>(entity2, ref flashEffectComponent);
        this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(entity2), "color-flash-effect");
        Animation damageAnimation = this.GetDamageAnimation(entity2, color, sprite);
        if (damageAnimation != null)
        {
          GetFlashEffectTargetEvent effectTargetEvent = new GetFlashEffectTargetEvent(entity2);
          this.RaiseLocalEvent<GetFlashEffectTargetEvent>(entity2, ref effectTargetEvent, false);
          EntityUid target = effectTargetEvent.Target;
          this.EnsureComp<ColorFlashEffectComponent>(target, ref flashEffectComponent);
          flashEffectComponent.NetSyncEnabled = false;
          flashEffectComponent.Color = sprite.Color;
          this._animation.Play(target, damageAnimation, "color-flash-effect");
        }
      }
    }
  }
}
