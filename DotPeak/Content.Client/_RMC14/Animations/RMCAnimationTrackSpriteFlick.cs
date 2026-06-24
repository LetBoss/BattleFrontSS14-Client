// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Animations.RMCAnimationTrackSpriteFlick
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Animations;

public sealed class RMCAnimationTrackSpriteFlick : AnimationTrack
{
  public required List<RMCAnimationTrackSpriteFlick.KeyFrame> KeyFrames { get; init; }

  public required string LayerKey { get; init; }

  public virtual (int KeyFrameIndex, float FramePlayingTime) InitPlayback() => (-1, 0.0f);

  public virtual (int KeyFrameIndex, float FramePlayingTime) AdvancePlayback(
    object context,
    int prevKeyFrameIndex,
    float prevPlayingTime,
    float frameTime)
  {
    EntityUid entityUid = (EntityUid) context;
    IEntityManager ientityManager = IoCManager.Resolve<IEntityManager>();
    SpriteComponent component = ientityManager.GetComponent<SpriteComponent>(entityUid);
    Entity<SpriteComponent> entity = new Entity<SpriteComponent>(entityUid, component);
    SpriteSystem spriteSystem = ientityManager.System<SpriteSystem>();
    float val2 = prevPlayingTime + frameTime;
    int index;
    for (index = prevKeyFrameIndex; index != this.KeyFrames.Count - 1; ++index)
    {
      RMCAnimationTrackSpriteFlick.KeyFrame keyFrame = this.KeyFrames[index + 1];
      if ((double) keyFrame.KeyTime < (double) val2)
      {
        double num = (double) val2;
        keyFrame = this.KeyFrames[index + 1];
        double keyTime = (double) keyFrame.KeyTime;
        val2 = (float) (num - keyTime);
      }
      else
        break;
    }
    if (index >= 0)
    {
      RMCAnimationTrackSpriteFlick.KeyFrame keyFrame = this.KeyFrames[index];
      SpriteComponent.Layer layer;
      if (!spriteSystem.TryGetLayer(entity.AsNullable(), this.LayerKey, ref layer, false))
        return (index, val2);
      RSI actualRsi = layer.ActualRsi;
      RSI.State state;
      if (actualRsi != null && actualRsi.TryGetState(RSI.StateId.op_Implicit(keyFrame.Rsi.RsiState), ref state))
      {
        float num = Math.Min(state.AnimationLength - 0.01f, val2);
        spriteSystem.LayerSetAutoAnimated(layer, false);
        spriteSystem.LayerSetSprite(layer, (SpriteSpecifier) keyFrame.Rsi);
        spriteSystem.LayerSetRsiState(layer, RSI.StateId.op_Implicit(keyFrame.Rsi.RsiState), false);
        spriteSystem.LayerSetAnimationTime(layer, num);
      }
    }
    return (index, val2);
  }

  public readonly record struct KeyFrame(SpriteSpecifier.Rsi Rsi, float KeyTime);
}
