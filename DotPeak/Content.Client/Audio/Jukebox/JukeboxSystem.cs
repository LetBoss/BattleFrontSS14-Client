// Decompiled with JetBrains decompiler
// Type: Content.Client.Audio.Jukebox.JukeboxSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Audio.Jukebox;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Audio.Jukebox;

public sealed class JukeboxSystem : SharedJukeboxSystem
{
  [Dependency]
  private IPrototypeManager _protoManager;
  [Dependency]
  private AnimationPlayerSystem _animationPlayer;
  [Dependency]
  private SharedAppearanceSystem _appearanceSystem;
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<JukeboxComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<JukeboxComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<JukeboxComponent, AnimationCompletedEvent>(new ComponentEventHandler<JukeboxComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationCompleted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<JukeboxComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<JukeboxComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnJukeboxAfterState)), (Type[]) null, (Type[]) null);
    this._protoManager.PrototypesReloaded += new Action<PrototypesReloadedEventArgs>(this.OnProtoReload);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this._protoManager.PrototypesReloaded -= new Action<PrototypesReloadedEventArgs>(this.OnProtoReload);
  }

  private void OnProtoReload(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<JukeboxPrototype>())
      return;
    AllEntityQueryEnumerator<JukeboxComponent, UserInterfaceComponent> entityQueryEnumerator = this.AllEntityQuery<JukeboxComponent, UserInterfaceComponent>();
    EntityUid entityUid;
    JukeboxComponent jukeboxComponent;
    UserInterfaceComponent interfaceComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref jukeboxComponent, ref interfaceComponent))
    {
      JukeboxBoundUserInterface boundUserInterface;
      if (this._uiSystem.TryGetOpenUi<JukeboxBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit((entityUid, interfaceComponent)), (Enum) JukeboxUiKey.Key, ref boundUserInterface))
        boundUserInterface.PopulateMusic();
    }
  }

  private void OnJukeboxAfterState(Entity<JukeboxComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    JukeboxBoundUserInterface boundUserInterface;
    if (!this._uiSystem.TryGetOpenUi<JukeboxBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) JukeboxUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Reload();
  }

  private void OnAnimationCompleted(
    EntityUid uid,
    JukeboxComponent component,
    AnimationCompletedEvent args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    AppearanceComponent appearanceComponent;
    JukeboxVisualState visualState;
    if (!this.TryComp<AppearanceComponent>(uid, ref appearanceComponent) || !this._appearanceSystem.TryGetData<JukeboxVisualState>(uid, (Enum) JukeboxVisuals.VisualState, ref visualState, appearanceComponent))
      visualState = JukeboxVisualState.On;
    this.UpdateAppearance(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), visualState, component);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    JukeboxComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    object obj;
    if (!args.AppearanceData.TryGetValue((Enum) JukeboxVisuals.VisualState, out obj) || !(obj is JukeboxVisualState visualState))
      visualState = JukeboxVisualState.On;
    this.UpdateAppearance(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), visualState, component);
  }

  private void UpdateAppearance(
    Entity<SpriteComponent> entity,
    JukeboxVisualState visualState,
    JukeboxComponent component)
  {
    this.SetLayerState(JukeboxVisualLayers.Base, component.OffState, entity);
    switch (visualState)
    {
      case JukeboxVisualState.On:
        this.SetLayerState(JukeboxVisualLayers.Base, component.OnState, entity);
        break;
      case JukeboxVisualState.Off:
        this.SetLayerState(JukeboxVisualLayers.Base, component.OffState, entity);
        break;
      case JukeboxVisualState.Select:
        this.PlayAnimation(entity.Owner, JukeboxVisualLayers.Base, component.SelectState, 1f, Entity<SpriteComponent>.op_Implicit(entity));
        break;
    }
  }

  private void PlayAnimation(
    EntityUid uid,
    JukeboxVisualLayers layer,
    string? state,
    float animationTime,
    SpriteComponent sprite)
  {
    if (string.IsNullOrEmpty(state) || this._animationPlayer.HasRunningAnimation(uid, state))
      return;
    Animation animation = JukeboxSystem.GetAnimation(layer, state, animationTime);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) layer, true);
    this._animationPlayer.Play(uid, animation, state);
  }

  private static Animation GetAnimation(
    JukeboxVisualLayers layer,
    string state,
    float animationTime)
  {
    return new Animation()
    {
      Length = TimeSpan.FromSeconds((double) animationTime),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) layer,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(state), 0.0f)
          }
        }
      }
    };
  }

  private void SetLayerState(
    JukeboxVisualLayers layer,
    string? state,
    Entity<SpriteComponent> sprite)
  {
    if (string.IsNullOrEmpty(state))
      return;
    this._sprite.LayerSetVisible(sprite.AsNullable(), (Enum) layer, true);
    this._sprite.LayerSetAutoAnimated(sprite.AsNullable(), (Enum) layer, true);
    this._sprite.LayerSetRsiState(sprite.AsNullable(), (Enum) layer, RSI.StateId.op_Implicit(state));
  }
}
