// Decompiled with JetBrains decompiler
// Type: Content.Client.Outline.InteractionOutlineSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.ContextMenu.UI;
using Content.Client.Gameplay;
using Content.Client.Interactable.Components;
using Content.Client.Viewport;
using Content.Shared.CCVar;
using Content.Shared.Interaction;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Outline;

public sealed class InteractionOutlineSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _configManager;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IStateManager _stateManager;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  private bool _enabled = true;
  private bool _cvarEnabled = true;
  private EntityUid? _lastHoveredEntity;

  public virtual void Initialize()
  {
    base.Initialize();
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._configManager, CCVars.OutlineEnabled, new Action<bool>(this.SetCvarEnabled), false);
    this.UpdatesAfter.Add(typeof (SharedEyeSystem));
  }

  public void SetCvarEnabled(bool cvarEnabled)
  {
    this._cvarEnabled = cvarEnabled;
    InteractionOutlineComponent outlineComponent;
    if (this._cvarEnabled || !this._lastHoveredEntity.HasValue || this.Deleted(this._lastHoveredEntity) || !this.TryComp<InteractionOutlineComponent>(this._lastHoveredEntity, ref outlineComponent))
      return;
    outlineComponent.OnMouseLeave(this._lastHoveredEntity.Value);
  }

  public void SetEnabled(bool enabled)
  {
    if (enabled == this._enabled)
      return;
    this._enabled = enabled;
    InteractionOutlineComponent outlineComponent;
    if (enabled || !this._lastHoveredEntity.HasValue || this.Deleted(this._lastHoveredEntity) || !this.TryComp<InteractionOutlineComponent>(this._lastHoveredEntity, ref outlineComponent))
      return;
    outlineComponent.OnMouseLeave(this._lastHoveredEntity.Value);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    if (!this._enabled || !this._cvarEnabled)
      return;
    ICommonSession localSession = ((ISharedPlayerManager) this._playerManager).LocalSession;
    if (localSession == null || !(this._stateManager.CurrentState is GameplayStateBase currentState))
      return;
    EntityUid? nullable1 = new EntityUid?();
    int renderScale = 1;
    if (this._uiManager.CurrentlyHovered is IViewportControl currentlyHovered1)
    {
      ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
      if (((ScreenCoordinates) ref mouseScreenPosition).IsValid)
      {
        MapCoordinates map = currentlyHovered1.PixelToMap(this._inputManager.MouseScreenPosition.Position);
        if (currentlyHovered1 is ScalingViewport scalingViewport)
        {
          renderScale = scalingViewport.CurrentRenderScale;
          nullable1 = currentState.GetClickedEntity(map, scalingViewport.Eye);
          goto label_11;
        }
        nullable1 = currentState.GetClickedEntity(map);
        goto label_11;
      }
    }
    if (this._uiManager.CurrentlyHovered is EntityMenuElement currentlyHovered2)
    {
      nullable1 = currentlyHovered2.Entity;
      renderScale = this._eyeManager.MainViewport.GetRenderScale();
    }
label_11:
    bool inInteractionRange = false;
    EntityUid? nullable2 = localSession.AttachedEntity;
    if (nullable2.HasValue && !this.Deleted(nullable1))
    {
      SharedInteractionSystem interactionSystem = this._interactionSystem;
      nullable2 = localSession.AttachedEntity;
      Entity<TransformComponent> origin = Entity<TransformComponent>.op_Implicit(nullable2.Value);
      Entity<TransformComponent> other = Entity<TransformComponent>.op_Implicit(nullable1.Value);
      nullable2 = new EntityUid?();
      EntityUid? user = nullable2;
      inInteractionRange = interactionSystem.InRangeUnobstructed(origin, other, user: user);
    }
    nullable2 = nullable1;
    EntityUid? lastHoveredEntity = this._lastHoveredEntity;
    if ((nullable2.HasValue == lastHoveredEntity.HasValue ? (nullable2.HasValue ? (EntityUid.op_Equality(nullable2.GetValueOrDefault(), lastHoveredEntity.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0)
    {
      InteractionOutlineComponent outlineComponent;
      if (!nullable1.HasValue || !this.TryComp<InteractionOutlineComponent>(nullable1, ref outlineComponent))
        return;
      outlineComponent.UpdateInRange(nullable1.Value, inInteractionRange, renderScale);
    }
    else
    {
      InteractionOutlineComponent outlineComponent;
      if (this._lastHoveredEntity.HasValue && !this.Deleted(this._lastHoveredEntity) && this.TryComp<InteractionOutlineComponent>(this._lastHoveredEntity, ref outlineComponent))
        outlineComponent.OnMouseLeave(this._lastHoveredEntity.Value);
      this._lastHoveredEntity = nullable1;
      if (!this._lastHoveredEntity.HasValue || !this.TryComp<InteractionOutlineComponent>(this._lastHoveredEntity, ref outlineComponent))
        return;
      outlineComponent.OnMouseEnter(this._lastHoveredEntity.Value, inInteractionRange, renderScale);
    }
  }
}
