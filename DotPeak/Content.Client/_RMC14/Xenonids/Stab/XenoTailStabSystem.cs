// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Stab.XenoTailStabSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Interactable;
using Content.Shared._RMC14.Xenonids.Stab;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Stab;

public sealed class XenoTailStabSystem : SharedXenoTailStabSystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private InteractionSystem _interaction;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IOverlayManager _overlays;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private TransformSystem _transform;
  private const string TailAnimationKey = "cm-xeno-tail";
  private const string TailFadeAnimationKey = "cm-xeno-tail-fade";
  private bool _showTailAttack;

  public override void Initialize() => base.Initialize();

  public virtual void Shutdown() => base.Shutdown();

  protected override void DoLunge(
    Entity<XenoTailStabComponent, TransformComponent> user,
    Vector2 localPos,
    EntProtoId animationId)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityUid entityUid = this.Spawn(EntProtoId.op_Implicit(animationId), user.Comp2.Coordinates);
    ((SharedTransformSystem) this._transform).SetParent(entityUid, Entity<XenoTailStabComponent, TransformComponent>.op_Implicit(user));
    SpriteComponent spriteComponent = this.EnsureComp<SpriteComponent>(entityUid);
    spriteComponent.NoRotation = true;
    this._sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), DirectionExtensions.ToWorldAngle(localPos));
    float num1 = localPos.Length() * 0.8f;
    MapCoordinates mapCoordinates = ((SharedTransformSystem) this._transform).GetMapCoordinates(Entity<XenoTailStabComponent, TransformComponent>.op_Implicit(user));
    float num2 = this._interaction.UnobstructedDistance(mapCoordinates, ((MapCoordinates) ref mapCoordinates).Offset(localPos));
    if ((double) num1 > (double) num2)
      num1 = num2;
    Angle rotation = spriteComponent.Rotation;
    ref Angle local1 = ref rotation;
    Vector2 vector2_1 = new Vector2(0.0f, (float) (-(double) num1 / 5.0));
    ref Vector2 local2 = ref vector2_1;
    Vector2 vector2_2 = ((Angle) ref local1).RotateVec(ref local2);
    rotation = spriteComponent.Rotation;
    ref Angle local3 = ref rotation;
    Vector2 vector2_3 = new Vector2(0.0f, -num1);
    ref Vector2 local4 = ref vector2_3;
    Vector2 vector2_4 = ((Angle) ref local3).RotateVec(ref local4);
    Animation animation1 = new Animation();
    animation1.Length = TimeSpan.FromSeconds(0.10000000149011612);
    List<AnimationTrack> animationTracks1 = animation1.AnimationTracks;
    AnimationTrackComponentProperty componentProperty1 = new AnimationTrackComponentProperty();
    componentProperty1.ComponentType = typeof (SpriteComponent);
    componentProperty1.Property = "Offset";
    ((AnimationTrackProperty) componentProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_2, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_4, 0.1f, (Func<float, float>) null));
    animationTracks1.Add((AnimationTrack) componentProperty1);
    Animation animation2 = animation1;
    this._animation.Play(entityUid, animation2, "cm-xeno-tail");
    Animation animation3 = new Animation();
    animation3.Length = TimeSpan.FromSeconds(0.15000000596046448);
    List<AnimationTrack> animationTracks2 = animation3.AnimationTracks;
    AnimationTrackComponentProperty componentProperty2 = new AnimationTrackComponentProperty();
    componentProperty2.ComponentType = typeof (SpriteComponent);
    componentProperty2.Property = "Color";
    ((AnimationTrackProperty) componentProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) spriteComponent.Color, 0.05f, (Func<float, float>) null));
    List<AnimationTrackProperty.KeyFrame> keyFrames = ((AnimationTrackProperty) componentProperty2).KeyFrames;
    Color color = spriteComponent.Color;
    AnimationTrackProperty.KeyFrame keyFrame = new AnimationTrackProperty.KeyFrame((object) ((Color) ref color).WithAlpha((byte) 0), 0.1f, (Func<float, float>) null);
    keyFrames.Add(keyFrame);
    animationTracks2.Add((AnimationTrack) componentProperty2);
    Animation animation4 = animation3;
    this._animation.Play(entityUid, animation4, "cm-xeno-tail-fade");
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    XenoTailStabSystem.TailStabOverlay tailStabOverlay;
    if (!this._overlays.TryGetOverlay<XenoTailStabSystem.TailStabOverlay>(ref tailStabOverlay))
      return;
    tailStabOverlay.Last = new Box2Rotated?(this.LastTailAttack);
  }

  private sealed class TailStabOverlay : Overlay
  {
    public Box2Rotated? Last;

    public virtual OverlaySpace Space => (OverlaySpace) 4;

    protected virtual void Draw(in OverlayDrawArgs args)
    {
      if (!this.Last.HasValue)
        return;
      DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
      Box2Rotated box2Rotated = this.Last.Value;
      ref Box2Rotated local = ref box2Rotated;
      Color red = Color.Red;
      worldHandle.DrawRect(ref local, red, true);
    }
  }
}
