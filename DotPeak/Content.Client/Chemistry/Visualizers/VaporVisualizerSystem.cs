// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.Visualizers.VaporVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Vapor;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Chemistry.Visualizers;

public sealed class VaporVisualizerSystem : VisualizerSystem<VaporVisualsComponent>
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<VaporVisualsComponent, ComponentInit>(new ComponentEventHandler<VaporVisualsComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(EntityUid uid, VaporVisualsComponent comp, ComponentInit args)
  {
    comp.VaporFlick = new Animation()
    {
      Length = TimeSpan.FromSeconds((double) comp.AnimationTime),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) VaporVisualLayers.Base,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.AnimationState), 0.0f)
          }
        }
      }
    };
    bool flag;
    AnimationPlayerComponent animationPlayerComponent;
    if (!(((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) VaporVisuals.State, ref flag, (AppearanceComponent) null) & flag) || !((EntitySystem) this).TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent) || this.AnimationSystem.HasRunningAnimation(uid, animationPlayerComponent, "flick_animation"))
      return;
    this.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), comp.VaporFlick, "flick_animation");
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    VaporVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    Color color;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<Color>(uid, (Enum) VaporVisuals.Color, ref color, args.Component) || args.Sprite == null)
      return;
    this.SpriteSystem.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), color);
  }
}
