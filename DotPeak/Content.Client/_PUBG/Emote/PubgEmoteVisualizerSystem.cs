// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Emote.PubgEmoteVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Emote;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Emote;

public sealed class PubgEmoteVisualizerSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PlayEmoteEvent>(new EntityEventHandler<PlayEmoteEvent>(this.OnPlayEmote), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActiveEmoteComponent, ComponentShutdown>(new ComponentEventHandler<ActiveEmoteComponent, ComponentShutdown>((object) this, __methodptr(OnEmoteShutdown)), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<ActiveEmoteComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveEmoteComponent, SpriteComponent>();
    EntityUid uid;
    ActiveEmoteComponent activeEmote;
    SpriteComponent sprite;
    while (entityQueryEnumerator.MoveNext(ref uid, ref activeEmote, ref sprite))
    {
      TimeSpan timeSpan = this._timing.CurTime - activeEmote.StartTime;
      switch (activeEmote.PlayMode)
      {
        case EmotePlayMode.Once:
          if (this.HasAnimationFinished(uid, sprite, activeEmote))
          {
            this.RemCompDeferred<ActiveEmoteComponent>(uid);
            continue;
          }
          continue;
        case EmotePlayMode.Loop:
          if (activeEmote.Duration.HasValue && timeSpan.TotalSeconds >= (double) activeEmote.Duration.Value)
          {
            this.RemCompDeferred<ActiveEmoteComponent>(uid);
            continue;
          }
          continue;
        case EmotePlayMode.Count:
          if (activeEmote.RepeatCount.HasValue && this.GetAnimationCycles(uid, sprite, activeEmote) >= activeEmote.RepeatCount.Value)
          {
            this.RemCompDeferred<ActiveEmoteComponent>(uid);
            continue;
          }
          continue;
        default:
          continue;
      }
    }
  }

  private void OnPlayEmote(PlayEmoteEvent ev)
  {
    EntityUid entity = this.GetEntity(ev.Entity);
    if (!this.Exists(entity))
      return;
    ActiveEmoteComponent activeEmoteComponent = this.EnsureComp<ActiveEmoteComponent>(entity);
    activeEmoteComponent.PlayMode = ev.PlayMode;
    activeEmoteComponent.StartTime = this._timing.CurTime;
    activeEmoteComponent.Duration = ev.Duration;
    activeEmoteComponent.RepeatCount = ev.RepeatCount;
    activeEmoteComponent.CurrentRepeats = 0;
    activeEmoteComponent.RsiPath = ev.RsiPath;
    activeEmoteComponent.StateName = ev.StateName;
    this.AddEmoteLayer(entity, ev.RsiPath, ev.StateName, ev.Scale);
  }

  private void AddEmoteLayer(EntityUid uid, string rsiPath, string stateName, float scale = 1f)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent) || string.IsNullOrEmpty(rsiPath) || string.IsNullOrEmpty(stateName))
      return;
    int num1;
    if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) PubgEmoteVisualizerSystem.EmoteLayers.Emote, ref num1, false))
      this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num1, true);
    ResPath resPath;
    // ISSUE: explicit constructor call
    ((ResPath) ref resPath).\u002Ector(rsiPath);
    int num2 = this._sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (SpriteSpecifier) new SpriteSpecifier.Rsi(resPath, stateName), new int?());
    this._sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) PubgEmoteVisualizerSystem.EmoteLayers.Emote, num2);
    Box2 localBounds = this._sprite.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)));
    float num3 = (float) ((double) ((Box2) ref localBounds).Height / 2.0 + 5.0 / 16.0);
    float num4 = (float) ((double) num3 * ((double) scale - 1.0) * 0.20000000298023224);
    this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num2, new Vector2(0.0f, num3 + num4));
    this._sprite.LayerSetScale(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num2, new Vector2(scale, scale));
    spriteComponent.LayerSetShader(num2, "unshaded");
  }

  private void OnEmoteShutdown(
    EntityUid uid,
    ActiveEmoteComponent component,
    ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) PubgEmoteVisualizerSystem.EmoteLayers.Emote, ref num, false))
      return;
    this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, true);
  }

  private bool HasAnimationFinished(
    EntityUid uid,
    SpriteComponent sprite,
    ActiveEmoteComponent activeEmote)
  {
    int num1;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) PubgEmoteVisualizerSystem.EmoteLayers.Emote, ref num1, false))
      return true;
    ISpriteLayer ispriteLayer = sprite[num1];
    RSI.State state;
    if (ispriteLayer.Rsi == null || RSI.StateId.op_Equality(ispriteLayer.RsiState, RSI.StateId.op_Implicit((string) null)) || !ispriteLayer.Rsi.TryGetState(ispriteLayer.RsiState, ref state) || !state.IsAnimated)
      return true;
    float num2 = 0.0f;
    foreach (float delay in state.Delays)
      num2 += delay;
    return (this._timing.CurTime - activeEmote.StartTime).TotalSeconds >= (double) num2;
  }

  private int GetAnimationCycles(
    EntityUid uid,
    SpriteComponent sprite,
    ActiveEmoteComponent activeEmote)
  {
    int num1;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) PubgEmoteVisualizerSystem.EmoteLayers.Emote, ref num1, false))
      return 0;
    ISpriteLayer ispriteLayer = sprite[num1];
    RSI.State state;
    if (ispriteLayer.Rsi == null || RSI.StateId.op_Equality(ispriteLayer.RsiState, RSI.StateId.op_Implicit((string) null)) || !ispriteLayer.Rsi.TryGetState(ispriteLayer.RsiState, ref state) || !state.IsAnimated)
      return 0;
    float num2 = 0.0f;
    foreach (float delay in state.Delays)
      num2 += delay;
    return (double) num2 <= 0.0 ? 0 : (int) ((this._timing.CurTime - activeEmote.StartTime).TotalSeconds / (double) num2);
  }

  private enum EmoteLayers : byte
  {
    Emote,
  }
}
