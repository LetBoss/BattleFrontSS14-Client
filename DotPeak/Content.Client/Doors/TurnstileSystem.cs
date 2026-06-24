// Decompiled with JetBrains decompiler
// Type: Content.Client.Doors.TurnstileSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Examine;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Doors;

public sealed class TurnstileSystem : SharedTurnstileSystem
{
  [Dependency]
  private AnimationPlayerSystem _animationPlayer;
  [Dependency]
  private SpriteSystem _sprite;
  private static readonly EntProtoId ExamineArrow = EntProtoId.op_Implicit("TurnstileArrow");
  private const string AnimationKey = "Turnstile";

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TurnstileComponent, AnimationCompletedEvent>(new EntityEventRefHandler<TurnstileComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationCompleted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TurnstileComponent, ExaminedEvent>(new EntityEventRefHandler<TurnstileComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
  }

  private void OnAnimationCompleted(
    Entity<TurnstileComponent> ent,
    ref AnimationCompletedEvent args)
  {
    SpriteComponent spriteComponent;
    if (args.Key != "Turnstile" || !this.TryComp<SpriteComponent>(Entity<TurnstileComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), (Enum) TurnstileVisualLayers.Base, new RSI.StateId(ent.Comp.DefaultState));
  }

  private void OnExamined(Entity<TurnstileComponent> ent, ref ExaminedEvent args)
  {
    this.Spawn(EntProtoId.op_Implicit(TurnstileSystem.ExamineArrow), new EntityCoordinates(Entity<TurnstileComponent>.op_Implicit(ent), 0.0f, 0.0f));
  }

  protected override void PlayAnimation(EntityUid uid, string stateId)
  {
    AnimationPlayerComponent animationPlayerComponent;
    SpriteComponent spriteComponent;
    if (!this.TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent) || !this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    (EntityUid, AnimationPlayerComponent) valueTuple = (uid, animationPlayerComponent);
    if (this._animationPlayer.HasRunningAnimation(animationPlayerComponent, "Turnstile"))
      this._animationPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit(valueTuple), "Turnstile");
    RSI.State state;
    if (spriteComponent.BaseRSI == null || !spriteComponent.BaseRSI.TryGetState(RSI.StateId.op_Implicit(stateId), ref state))
      return;
    float animationLength = state.AnimationLength;
    Animation animation = new Animation()
    {
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) TurnstileVisualLayers.Base,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(state.StateId, 0.0f)
          }
        }
      },
      Length = TimeSpan.FromSeconds((double) animationLength)
    };
    this._animationPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit(valueTuple), animation, "Turnstile");
  }
}
