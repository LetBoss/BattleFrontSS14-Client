// Decompiled with JetBrains decompiler
// Type: Content.Client.Doors.DoorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Doors;

public sealed class DoorSystem : SharedDoorSystem
{
  [Dependency]
  private AnimationPlayerSystem _animationSystem;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DoorComponent, AppearanceChangeEvent>(new EntityEventRefHandler<DoorComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  protected override void OnComponentInit(Entity<DoorComponent> ent, ref ComponentInit args)
  {
    DoorComponent comp = ent.Comp;
    comp.OpenSpriteStates = new List<(DoorVisualLayers, string)>(2);
    comp.ClosedSpriteStates = new List<(DoorVisualLayers, string)>(2);
    comp.OpenSpriteStates.Add((DoorVisualLayers.Base, comp.OpenSpriteState));
    comp.ClosedSpriteStates.Add((DoorVisualLayers.Base, comp.ClosedSpriteState));
    comp.OpeningAnimation = (object) new Animation()
    {
      Length = TimeSpan.FromSeconds((double) comp.OpeningAnimationTime),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) DoorVisualLayers.Base,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.OpeningSpriteState), 0.0f)
          }
        }
      }
    };
    comp.ClosingAnimation = (object) new Animation()
    {
      Length = TimeSpan.FromSeconds((double) comp.ClosingAnimationTime),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) DoorVisualLayers.Base,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.ClosingSpriteState), 0.0f)
          }
        }
      }
    };
    comp.EmaggingAnimation = (object) new Animation()
    {
      Length = TimeSpan.FromSeconds((double) comp.EmaggingAnimationTime),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) DoorVisualLayers.BaseUnlit,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.EmaggingSpriteState), 0.0f)
          }
        }
      }
    };
  }

  private void OnAppearanceChange(Entity<DoorComponent> entity, ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    DoorState state;
    if (!this.AppearanceSystem.TryGetData<DoorState>(Entity<DoorComponent>.op_Implicit(entity), (Enum) DoorVisuals.State, ref state, args.Component))
      state = DoorState.Closed;
    string baseRsi;
    if (this.AppearanceSystem.TryGetData<string>(Entity<DoorComponent>.op_Implicit(entity), (Enum) DoorVisuals.BaseRSI, ref baseRsi, args.Component))
      this.UpdateSpriteLayers(Entity<SpriteComponent>.op_Implicit((entity.Owner, args.Sprite)), baseRsi);
    if (this._animationSystem.HasRunningAnimation(Entity<DoorComponent>.op_Implicit(entity), "door_animation"))
      this._animationSystem.Stop(Entity<AnimationPlayerComponent>.op_Implicit(entity.Owner), "door_animation");
    this.UpdateAppearanceForDoorState(entity, args.Sprite, state);
  }

  private void UpdateAppearanceForDoorState(
    Entity<DoorComponent> entity,
    SpriteComponent sprite,
    DoorState state)
  {
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((entity.Owner, sprite)), state == DoorState.Open ? entity.Comp.OpenDrawDepth : entity.Comp.ClosedDrawDepth);
    switch (state)
    {
      case DoorState.Closed:
        using (List<(DoorVisualLayers, string)>.Enumerator enumerator = entity.Comp.ClosedSpriteStates.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            (DoorVisualLayers doorVisualLayers, string str) = enumerator.Current;
            this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, sprite)), (Enum) doorVisualLayers, RSI.StateId.op_Implicit(str));
          }
          break;
        }
      case DoorState.Closing:
        if ((double) entity.Comp.ClosingAnimationTime == 0.0 || entity.Comp.CurrentlyCrushing.Count != 0)
          break;
        this._animationSystem.Play(Entity<DoorComponent>.op_Implicit(entity), (Animation) entity.Comp.ClosingAnimation, "door_animation");
        break;
      case DoorState.Open:
        using (List<(DoorVisualLayers, string)>.Enumerator enumerator = entity.Comp.OpenSpriteStates.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            (DoorVisualLayers doorVisualLayers, string str) = enumerator.Current;
            this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, sprite)), (Enum) doorVisualLayers, RSI.StateId.op_Implicit(str));
          }
          break;
        }
      case DoorState.Opening:
        if ((double) entity.Comp.OpeningAnimationTime == 0.0)
          break;
        this._animationSystem.Play(Entity<DoorComponent>.op_Implicit(entity), (Animation) entity.Comp.OpeningAnimation, "door_animation");
        break;
      case DoorState.Denying:
        this._animationSystem.Play(Entity<DoorComponent>.op_Implicit(entity), (Animation) entity.Comp.DenyingAnimation, "door_animation");
        break;
      case DoorState.Emagging:
        this._animationSystem.Play(Entity<DoorComponent>.op_Implicit(entity), (Animation) entity.Comp.EmaggingAnimation, "door_animation");
        break;
    }
  }

  private void UpdateSpriteLayers(Entity<SpriteComponent> sprite, string baseRsi)
  {
    RSIResource rsiResource;
    if (!this._resourceCache.TryGetResource<RSIResource>(ResPath.op_Division(SpriteSpecifierSerializer.TextureRoot, baseRsi), ref rsiResource))
      this.Log.Error("Unable to load RSI '{0}'. Trace:\n{1}", new object[2]
      {
        (object) baseRsi,
        (object) Environment.StackTrace
      });
    else
      this._sprite.SetBaseRsi(sprite.AsNullable(), rsiResource.RSI);
  }
}
