// Decompiled with JetBrains decompiler
// Type: Content.Client.Interaction.DragDropSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CombatMode;
using Content.Client.Gameplay;
using Content.Client.Outline;
using Content.Shared.ActionBlocker;
using Content.Shared.CCVar;
using Content.Shared.DragDrop;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Interaction;

public sealed class DragDropSystem : SharedDragDropSystem
{
  private static readonly ProtoId<ShaderPrototype> ShaderDropTargetInRange = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutlineInrange");
  private static readonly ProtoId<ShaderPrototype> ShaderDropTargetOutOfRange = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutline");
  [Dependency]
  private IStateManager _stateManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IConfigurationManager _cfgMan;
  [Dependency]
  private InteractionOutlineSystem _outline;
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  private CombatModeSystem _combatMode;
  [Dependency]
  private InputSystem _inputSystem;
  [Dependency]
  private ActionBlockerSystem _actionBlockerSystem;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  [Dependency]
  private SpriteSystem _sprite;
  private const float TargetRecheckInterval = 0.25f;
  private const float MaxMouseDownTimeForReplayingClick = 0.85f;
  private EntityUid? _draggedEntity;
  private EntityUid? _dragShadow;
  private float _mouseDownTime;
  private float _targetRecheckTime;
  private PointerInputCmdHandler.PointerInputCmdArgs? _savedMouseDown;
  private bool _isReplaying;
  public float Deadzone;
  private DragState _state;
  private ScreenCoordinates? _mouseDownScreenPos;
  private ShaderInstance? _dropTargetInRangeShader;
  private ShaderInstance? _dropTargetOutOfRangeShader;
  private readonly List<SpriteComponent> _highlightedSprites = new List<SpriteComponent>();

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    this.UpdatesAfter.Add(typeof (SharedEyeSystem));
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._cfgMan, CCVars.DragDropDeadZone, new Action<float>(this.SetDeadZone), true);
    this._dropTargetInRangeShader = this._prototypeManager.Index<ShaderPrototype>(DragDropSystem.ShaderDropTargetInRange).Instance();
    this._dropTargetOutOfRangeShader = this._prototypeManager.Index<ShaderPrototype>(DragDropSystem.ShaderDropTargetOutOfRange).Instance();
    // ISSUE: method pointer
    CommandBinds.Builder.BindBefore(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(OnUse)), false, true), new Type[1]
    {
      typeof (SharedInteractionSystem)
    }).Register<DragDropSystem>();
  }

  private void SetDeadZone(float deadZone) => this.Deadzone = deadZone;

  public virtual void Shutdown()
  {
    CommandBinds.Unregister<DragDropSystem>();
    base.Shutdown();
  }

  private bool OnUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (this._inputSystem.Predicted || this._isReplaying)
      return false;
    if (args.State == 1)
      return this.OnUseMouseDown(in args);
    return args.State == null && this.OnUseMouseUp(in args);
  }

  private void EndDrag()
  {
    if (this._state == DragState.NotDragging)
      return;
    if (this._dragShadow.HasValue)
    {
      this.Del(new EntityUid?(this._dragShadow.Value));
      this._dragShadow = new EntityUid?();
    }
    this._draggedEntity = new EntityUid?();
    this._state = DragState.NotDragging;
    this._mouseDownScreenPos = new ScreenCoordinates?();
    this.RemoveHighlights();
    this._outline.SetEnabled(true);
    this._mouseDownTime = 0.0f;
    this._savedMouseDown = new PointerInputCmdHandler.PointerInputCmdArgs?();
  }

  private bool OnUseMouseDown(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    EntityUid? nullable = (EntityUid?) args.Session?.AttachedEntity;
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      if (((EntityUid) ref valueOrDefault).Valid && !this._combatMode.IsInCombatMode())
      {
        this.EndDrag();
        EntityUid entityUid = args.EntityUid;
        if (!this.Exists(entityUid))
          return false;
        SharedInteractionSystem interactionSystem = this._interactionSystem;
        Entity<TransformComponent> origin = Entity<TransformComponent>.op_Implicit(valueOrDefault);
        Entity<TransformComponent> other = Entity<TransformComponent>.op_Implicit(entityUid);
        nullable = new EntityUid?();
        EntityUid? user = nullable;
        if (!interactionSystem.InRangeUnobstructed(origin, other, user: user))
          return false;
        CanDragEvent canDragEvent = new CanDragEvent();
        this.RaiseLocalEvent<CanDragEvent>(entityUid, ref canDragEvent, false);
        if (!canDragEvent.Handled)
          return false;
        this._draggedEntity = new EntityUid?(entityUid);
        this._state = DragState.MouseDown;
        this._mouseDownScreenPos = new ScreenCoordinates?(args.ScreenCoordinates);
        this._mouseDownTime = 0.0f;
        this._savedMouseDown = new PointerInputCmdHandler.PointerInputCmdArgs?(args);
        return true;
      }
    }
    return false;
  }

  private void StartDrag()
  {
    if (!this.Exists(this._draggedEntity))
      return;
    this._state = DragState.Dragging;
    this._outline.SetEnabled(false);
    this.HighlightTargets();
    SpriteComponent spriteComponent1;
    if (this.TryComp<SpriteComponent>(this._draggedEntity, ref spriteComponent1))
    {
      ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
      if (!((ScreenCoordinates) ref mouseScreenPosition).IsValid)
        return;
      this._dragShadow = new EntityUid?(this.EntityManager.SpawnEntity("dragshadow", this._eyeManager.PixelToMap(mouseScreenPosition), (ComponentRegistry) null));
      SpriteComponent spriteComponent2 = this.Comp<SpriteComponent>(this._dragShadow.Value);
      this._sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((this._draggedEntity.Value, spriteComponent1)), Entity<SpriteComponent>.op_Implicit((this._dragShadow.Value, spriteComponent2)));
      spriteComponent2.RenderOrder = this.EntityManager.CurrentTick.Value;
      SpriteSystem sprite = this._sprite;
      Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((this._dragShadow.Value, spriteComponent2));
      Color color1 = spriteComponent2.Color;
      Color color2 = ((Color) ref color1).WithAlpha(0.7f);
      sprite.SetColor(entity, color2);
      this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((this._dragShadow.Value, spriteComponent2)), 13);
      if (spriteComponent2.NoRotation)
        return;
      this._transformSystem.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit(this._dragShadow.Value), this._transformSystem.GetWorldRotation(this._draggedEntity.Value));
    }
    else
      this.Log.Warning($"Unable to display drag shadow for {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(this._draggedEntity.Value))} because it has no sprite component.");
  }

  private bool UpdateDrag(float frameTime)
  {
    if (!this.Exists(this._draggedEntity) || this._combatMode.IsInCombatMode())
    {
      this.EndDrag();
      return false;
    }
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue || !this._interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(localEntity.Value), Entity<TransformComponent>.op_Implicit(this._draggedEntity.Value)) || !this._dragShadow.HasValue)
      return false;
    this._targetRecheckTime += frameTime;
    if ((double) this._targetRecheckTime > 0.25)
    {
      this.HighlightTargets();
      this._targetRecheckTime -= 0.25f;
    }
    return true;
  }

  private bool OnUseMouseUp(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (this._state == DragState.MouseDown)
    {
      try
      {
        if (this._savedMouseDown.HasValue)
        {
          if ((double) this._mouseDownTime < 0.85000002384185791)
          {
            PointerInputCmdHandler.PointerInputCmdArgs pointerInputCmdArgs = this._savedMouseDown.Value;
            this._isReplaying = true;
            IFullInputCmdMessage originalMessage = pointerInputCmdArgs.OriginalMessage;
            IFullInputCmdMessage ifullInputCmdMessage;
            switch (originalMessage)
            {
              case ClientFullInputCmdMessage fullInputCmdMessage1:
                ifullInputCmdMessage = (IFullInputCmdMessage) new ClientFullInputCmdMessage(args.OriginalMessage.Tick, args.OriginalMessage.SubTick, originalMessage.InputFunctionId)
                {
                  State = originalMessage.State,
                  Coordinates = fullInputCmdMessage1.Coordinates,
                  ScreenCoordinates = fullInputCmdMessage1.ScreenCoordinates,
                  Uid = fullInputCmdMessage1.Uid
                };
                break;
              case FullInputCmdMessage fullInputCmdMessage2:
                ifullInputCmdMessage = (IFullInputCmdMessage) new FullInputCmdMessage(args.OriginalMessage.Tick, args.OriginalMessage.SubTick, originalMessage.InputFunctionId, originalMessage.State, fullInputCmdMessage2.Coordinates, fullInputCmdMessage2.ScreenCoordinates, fullInputCmdMessage2.Uid);
                break;
              default:
                throw new ArgumentOutOfRangeException();
            }
            if (pointerInputCmdArgs.Session != null)
              this._inputSystem.HandleInputCommand(pointerInputCmdArgs.Session, EngineKeyFunctions.Use, ifullInputCmdMessage, true);
            this._isReplaying = false;
          }
        }
      }
      finally
      {
        this.EndDrag();
      }
      return false;
    }
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue || !this.Exists(this._draggedEntity))
    {
      this.EndDrag();
      return false;
    }
    EntityCoordinates coordinates = args.Coordinates;
    IEnumerable<EntityUid> entityUids = !(this._stateManager.CurrentState is GameplayState currentState) ? (IEnumerable<EntityUid>) Array.Empty<EntityUid>() : currentState.GetClickableEntities(coordinates);
    bool flag = false;
    EntityUid user1 = localEntity.Value;
    foreach (EntityUid target in entityUids)
    {
      EntityUid entityUid = target;
      EntityUid? nullable = this._draggedEntity;
      if ((nullable.HasValue ? (EntityUid.op_Equality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0 && this.ValidDragDrop(user1, this._draggedEntity.Value, target).GetValueOrDefault())
      {
        SharedInteractionSystem interactionSystem1 = this._interactionSystem;
        Entity<TransformComponent> origin1 = Entity<TransformComponent>.op_Implicit(user1);
        Entity<TransformComponent> other1 = Entity<TransformComponent>.op_Implicit(target);
        nullable = new EntityUid?();
        EntityUid? user2 = nullable;
        if (interactionSystem1.InRangeUnobstructed(origin1, other1, user: user2))
        {
          SharedInteractionSystem interactionSystem2 = this._interactionSystem;
          Entity<TransformComponent> origin2 = Entity<TransformComponent>.op_Implicit(user1);
          Entity<TransformComponent> other2 = Entity<TransformComponent>.op_Implicit(this._draggedEntity.Value);
          nullable = new EntityUid?();
          EntityUid? user3 = nullable;
          if (interactionSystem2.InRangeUnobstructed(origin2, other2, user: user3))
          {
            this.RaisePredictiveEvent<DragDropRequestEvent>(new DragDropRequestEvent(this.GetNetEntity(this._draggedEntity.Value, (MetaDataComponent) null), this.GetNetEntity(target, (MetaDataComponent) null)));
            this.EndDrag();
            return true;
          }
        }
        flag = true;
      }
    }
    if (flag)
      this._popup.PopupEntity(this.Loc.GetString("drag-drop-system-out-of-range-text"), this._draggedEntity.Value, Filter.Local(), true);
    this.EndDrag();
    return false;
  }

  private void HighlightTargets()
  {
    if (!this.Exists(this._draggedEntity) || !this.Exists(this._dragShadow))
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    this.RemoveHighlights();
    MapCoordinates map = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
    Vector2 vector2 = new Vector2(1.5f, 1.5f);
    Box2 box2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2).\u002Ector(map.Position - vector2, map.Position + vector2);
    HashSet<EntityUid> entitiesIntersecting = this._lookup.GetEntitiesIntersecting(map.MapId, box2, (LookupFlags) 110);
    EntityQuery<SpriteComponent> entityQuery = this.GetEntityQuery<SpriteComponent>();
    foreach (EntityUid target in entitiesIntersecting)
    {
      SpriteComponent spriteComponent;
      if (entityQuery.TryGetComponent(target, ref spriteComponent) && spriteComponent.Visible)
      {
        EntityUid entityUid = target;
        EntityUid? nullable1 = this._draggedEntity;
        if ((nullable1.HasValue ? (EntityUid.op_Equality(entityUid, nullable1.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
        {
          bool? nullable2 = this.ValidDragDrop(localEntity.Value, this._draggedEntity.Value, target);
          if (nullable2.HasValue)
          {
            if (nullable2.Value)
            {
              ref bool? local = ref nullable2;
              SharedInteractionSystem interactionSystem1 = this._interactionSystem;
              Entity<TransformComponent> origin1 = Entity<TransformComponent>.op_Implicit(localEntity.Value);
              Entity<TransformComponent> other1 = Entity<TransformComponent>.op_Implicit(this._draggedEntity.Value);
              nullable1 = new EntityUid?();
              EntityUid? user1 = nullable1;
              int num;
              if (interactionSystem1.InRangeUnobstructed(origin1, other1, user: user1))
              {
                SharedInteractionSystem interactionSystem2 = this._interactionSystem;
                Entity<TransformComponent> origin2 = Entity<TransformComponent>.op_Implicit(localEntity.Value);
                Entity<TransformComponent> other2 = Entity<TransformComponent>.op_Implicit(target);
                nullable1 = new EntityUid?();
                EntityUid? user2 = nullable1;
                num = interactionSystem2.InRangeUnobstructed(origin2, other2, user: user2) ? 1 : 0;
              }
              else
                num = 0;
              local = new bool?(num != 0);
            }
            if (spriteComponent.PostShader == null || spriteComponent.PostShader == this._dropTargetInRangeShader || spriteComponent.PostShader == this._dropTargetOutOfRangeShader)
            {
              spriteComponent.PostShader = nullable2.Value ? this._dropTargetInRangeShader : this._dropTargetOutOfRangeShader;
              spriteComponent.RenderOrder = this.EntityManager.CurrentTick.Value;
              this._highlightedSprites.Add(spriteComponent);
            }
          }
        }
      }
    }
  }

  private void RemoveHighlights()
  {
    foreach (SpriteComponent highlightedSprite in this._highlightedSprites)
    {
      if (highlightedSprite.PostShader == this._dropTargetInRangeShader || highlightedSprite.PostShader == this._dropTargetOutOfRangeShader)
      {
        highlightedSprite.PostShader = (ShaderInstance) null;
        highlightedSprite.RenderOrder = 0U;
      }
    }
    this._highlightedSprites.Clear();
  }

  private bool? ValidDragDrop(EntityUid user, EntityUid dragged, EntityUid target)
  {
    if (!this._actionBlockerSystem.CanInteract(user, new EntityUid?(target)))
      return new bool?();
    GettingInteractedWithAttemptEvent withAttemptEvent = new GettingInteractedWithAttemptEvent(user, new EntityUid?(dragged));
    this.RaiseLocalEvent<GettingInteractedWithAttemptEvent>(dragged, ref withAttemptEvent, false);
    if (withAttemptEvent.Cancelled)
      return new bool?(false);
    CanDropDraggedEvent dropDraggedEvent = new CanDropDraggedEvent(user, target);
    this.RaiseLocalEvent<CanDropDraggedEvent>(dragged, ref dropDraggedEvent, false);
    if (dropDraggedEvent.Handled && !dropDraggedEvent.CanDrop)
      return new bool?(false);
    CanDropTargetEvent canDropTargetEvent = new CanDropTargetEvent(user, dragged);
    this.RaiseLocalEvent<CanDropTargetEvent>(target, ref canDropTargetEvent, false);
    if (canDropTargetEvent.Handled)
      return new bool?(canDropTargetEvent.CanDrop);
    return dropDraggedEvent.Handled && dropDraggedEvent.CanDrop ? new bool?(true) : new bool?();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    switch (this._state)
    {
      case DragState.MouseDown:
        if ((double) (this._mouseDownScreenPos.Value.Position - this._inputManager.MouseScreenPosition.Position).Length() <= (double) this.Deadzone)
          break;
        this.StartDrag();
        break;
      case DragState.Dragging:
        this.UpdateDrag(frameTime);
        break;
    }
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    if (!this.Exists(this._dragShadow))
      return;
    this._transformSystem.SetWorldPosition(this._dragShadow.Value, this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition).Position);
  }
}
