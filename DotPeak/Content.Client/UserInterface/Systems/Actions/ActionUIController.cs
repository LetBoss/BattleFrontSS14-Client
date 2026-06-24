// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Actions.ActionUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Actions;
using Content.Client.Actions;
using Content.Client.Construction;
using Content.Client.Gameplay;
using Content.Client.Hands;
using Content.Client.Interaction;
using Content.Client.Outline;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Actions.Controls;
using Content.Client.UserInterface.Systems.Actions.Widgets;
using Content.Client.UserInterface.Systems.Actions.Windows;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Actions.Components;
using Content.Shared.CombatMode;
using Content.Shared.Input;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Actions;

public sealed class ActionUIController : 
  UIController,
  IOnStateChanged<GameplayState>,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>,
  IOnSystemChanged<ActionsSystem>,
  IOnSystemLoaded<ActionsSystem>,
  IOnSystemUnloaded<ActionsSystem>
{
  [Dependency]
  private IOverlayManager _overlays;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IInputManager _input;
  [UISystemDependency]
  private readonly ActionsSystem? _actionsSystem;
  [UISystemDependency]
  private readonly InteractionOutlineSystem? _interactionOutline;
  [UISystemDependency]
  private readonly TargetOutlineSystem? _targetOutline;
  [UISystemDependency]
  private readonly SpriteSystem _spriteSystem;
  private ActionButtonContainer? _container;
  private readonly List<EntityUid?> _actions = new List<EntityUid?>();
  private readonly DragDropHelper<Content.Client.UserInterface.Systems.Actions.Controls.ActionButton> _menuDragHelper;
  private readonly TextureRect _dragShadow;
  private ActionsWindow? _window;
  private readonly List<EntityUid?> _vehicleActions = new List<EntityUid?>();
  private bool _vehicleHotbarOverride;

  private ActionsBar? ActionsBar => this.UIManager.GetActiveUIWidgetOrNull<ActionsBar>();

  private MenuButton? ActionButton
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.ActionButton;
  }

  public bool IsDragging => this._menuDragHelper.IsDragging;

  public EntityUid? SelectingTargetFor { get; private set; }

  public ActionUIController()
  {
    this._menuDragHelper = new DragDropHelper<Content.Client.UserInterface.Systems.Actions.Controls.ActionButton>(new OnBeginDrag(this.OnMenuBeginDrag), new OnContinueDrag(this.OnMenuContinueDrag), new OnEndDrag(this.OnMenuEndDrag));
    TextureRect textureRect = new TextureRect();
    ((Control) textureRect).MinSize = new Vector2(64f, 64f);
    textureRect.Stretch = (TextureRect.StretchMode) 1;
    ((Control) textureRect).Visible = false;
    ((Control) textureRect).SetSize = new Vector2(64f, 64f);
    ((Control) textureRect).MouseFilter = (Control.MouseFilterMode) 2;
    this._dragShadow = textureRect;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += new Action(this.OnScreenLoad);
    uiController.OnScreenUnload += new Action(this.OnScreenUnload);
  }

  private void OnScreenLoad() => this.LoadGui();

  private void OnScreenUnload() => this.UnloadGui();

  public void OnStateEntered(GameplayState state)
  {
    if (this._actionsSystem != null)
    {
      this._actionsSystem.OnActionAdded += new Action<EntityUid>(this.OnActionAdded);
      this._actionsSystem.OnActionRemoved += new Action<EntityUid>(this.OnActionRemoved);
      this._actionsSystem.ActionsUpdated += new Action(this.OnActionsUpdated);
    }
    this.UpdateFilterLabel();
    this.QueueWindowUpdate();
    ((Control) this._dragShadow).Orphan();
    ((Control) this.UIManager.PopupRoot).AddChild((Control) this._dragShadow);
    CommandBinds.BindingsBuilder bindingsBuilder = CommandBinds.Builder;
    BoundKeyFunction[] hotbarBoundKeys = ContentKeyFunctions.GetHotbarBoundKeys();
    for (int index = 0; index < hotbarBoundKeys.Length; ++index)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ActionUIController.\u003C\u003Ec__DisplayClass27_0 cDisplayClass270 = new ActionUIController.\u003C\u003Ec__DisplayClass27_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass270.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass270.boundId = index;
      BoundKeyFunction boundKeyFunction = hotbarBoundKeys[index];
      // ISSUE: method pointer
      bindingsBuilder = bindingsBuilder.Bind(boundKeyFunction, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) cDisplayClass270, __methodptr(\u003COnStateEntered\u003Eb__1)), false, true));
    }
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    bindingsBuilder.Bind(ContentKeyFunctions.OpenActionsMenu, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__27_0)), (StateInputCmdDelegate) null, true, true)).BindBefore(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(TargetingOnUse)), true, true), new Type[2]
    {
      typeof (ConstructionSystem),
      typeof (DragDropSystem)
    }).BindBefore(EngineKeyFunctions.UIRightClick, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(TargetingCancel)), true, true), Array.Empty<Type>()).Register<ActionUIController>();
  }

  private bool TargetingCancel(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!this._timing.IsFirstTimePredicted || !this.SelectingTargetFor.HasValue)
      return false;
    this.StopTargeting();
    return true;
  }

  private bool TargetingOnUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (this._timing.IsFirstTimePredicted && this._actionsSystem != null)
    {
      EntityUid? nullable = this.SelectingTargetFor;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
        nullable = ((ISharedPlayerManager) this._playerManager).LocalEntity;
        if (!nullable.HasValue)
          return false;
        EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
        ActionsComponent actionsComponent;
        if (!this.EntityManager.TryGetComponent<ActionsComponent>(valueOrDefault2, ref actionsComponent))
          return false;
        Entity<ActionComponent>? action = this._actionsSystem.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(valueOrDefault1)));
        if (action.HasValue)
        {
          Entity<ActionComponent> valueOrDefault3 = action.GetValueOrDefault();
          TargetActionComponent targetActionComponent;
          if (this.EntityManager.TryGetComponent<TargetActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault3), ref targetActionComponent))
          {
            if (!this._actionsSystem.ValidAction(valueOrDefault3))
              return !targetActionComponent.InteractOnMiss;
            ActionTargetAttemptEvent targetAttemptEvent = new ActionTargetAttemptEvent(args, Entity<ActionsComponent>.op_Implicit((valueOrDefault2, actionsComponent)), Entity<ActionComponent>.op_Implicit(valueOrDefault3));
            ((IDirectedEventBus) this.EntityManager.EventBus).RaiseLocalEvent<ActionTargetAttemptEvent>(Entity<ActionComponent>.op_Implicit(valueOrDefault3), ref targetAttemptEvent, false);
            if (!targetAttemptEvent.Handled)
            {
              this.Log.Error($"Action {this.EntityManager.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(valueOrDefault1))} did not handle ActionTargetAttemptEvent!");
              return false;
            }
            if ((targetAttemptEvent.FoundTarget ? (!targetActionComponent.Repeat ? 1 : 0) : (targetActionComponent.DeselectOnMiss ? 1 : 0)) != 0)
              this.StopTargeting();
            return true;
          }
        }
        return false;
      }
    }
    return false;
  }

  public void UnloadButton()
  {
    if (this.ActionButton == null)
      return;
    ((BaseButton) this.ActionButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.ActionButtonPressed);
  }

  public void LoadButton()
  {
    if (this.ActionButton == null)
      return;
    ((BaseButton) this.ActionButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.ActionButtonPressed);
  }

  private void OnWindowOpened()
  {
    ((BaseButton) this.ActionButton)?.SetClickPressed(true);
    this.SearchAndDisplay();
  }

  private void OnWindowClosed() => ((BaseButton) this.ActionButton)?.SetClickPressed(false);

  public void OnStateExited(GameplayState state)
  {
    if (this._actionsSystem != null)
    {
      this._actionsSystem.OnActionAdded -= new Action<EntityUid>(this.OnActionAdded);
      this._actionsSystem.OnActionRemoved -= new Action<EntityUid>(this.OnActionRemoved);
      this._actionsSystem.ActionsUpdated -= new Action(this.OnActionsUpdated);
    }
    CommandBinds.Unregister<ActionUIController>();
  }

  private void TriggerAction(int index)
  {
    IReadOnlyList<EntityUid?> activeHotbarActions = this.GetActiveHotbarActions();
    if (index < 0 || index >= activeHotbarActions.Count)
      return;
    EntityUid? nullable = activeHotbarActions[index];
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    Entity<ActionComponent>? action = (Entity<ActionComponent>?) this._actionsSystem?.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(valueOrDefault1)));
    if (!action.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault2 = action.GetValueOrDefault();
    TargetActionComponent targetActionComponent;
    if (this.EntityManager.TryGetComponent<TargetActionComponent>(valueOrDefault1, ref targetActionComponent))
      this.ToggleTargeting(Entity<ActionComponent, TargetActionComponent>.op_Implicit((valueOrDefault1, valueOrDefault2.Comp, targetActionComponent)));
    else
      this._actionsSystem?.TriggerAction(valueOrDefault2);
  }

  private void OnActionAdded(EntityUid actionId)
  {
    Entity<ActionComponent>? action = (Entity<ActionComponent>?) this._actionsSystem?.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(actionId)));
    if (!action.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
    TargetActionComponent targetActionComponent;
    if (valueOrDefault.Comp.Toggled && this.EntityManager.TryGetComponent<TargetActionComponent>(actionId, ref targetActionComponent))
      this.StartTargeting(Entity<ActionComponent, TargetActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault), targetActionComponent)));
    if (this.EntityManager.HasComponent<VehicleHardpointActionComponent>(actionId))
    {
      this.RefreshVehicleHotbarOverride(true);
    }
    else
    {
      if (this._actions.Contains(new EntityUid?(Entity<ActionComponent>.op_Implicit(valueOrDefault))))
        return;
      this._actions.Add(new EntityUid?(Entity<ActionComponent>.op_Implicit(valueOrDefault)));
      this.RefreshVehicleHotbarOverride(true);
    }
  }

  private void OnActionRemoved(EntityUid actionId)
  {
    EntityUid entityUid1 = actionId;
    EntityUid? selectingTargetFor = this.SelectingTargetFor;
    if ((selectingTargetFor.HasValue ? (EntityUid.op_Equality(entityUid1, selectingTargetFor.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
      this.StopTargeting();
    if (this.EntityManager.HasComponent<VehicleHardpointActionComponent>(actionId) || this._vehicleActions.Contains(new EntityUid?(actionId)))
    {
      this._vehicleActions.RemoveAll((Predicate<EntityUid?>) (x =>
      {
        EntityUid? nullable = x;
        EntityUid entityUid2 = actionId;
        return nullable.HasValue && EntityUid.op_Equality(nullable.GetValueOrDefault(), entityUid2);
      }));
      this.RefreshVehicleHotbarOverride();
    }
    else
    {
      this._actions.RemoveAll((Predicate<EntityUid?>) (x =>
      {
        EntityUid? nullable = x;
        EntityUid entityUid3 = actionId;
        return nullable.HasValue && EntityUid.op_Equality(nullable.GetValueOrDefault(), entityUid3);
      }));
      this.RefreshVehicleHotbarOverride(true);
    }
  }

  private void OnActionsUpdated()
  {
    this.QueueWindowUpdate();
    this.RefreshVehicleHotbarOverride(true);
  }

  private void ActionButtonPressed(BaseButton.ButtonEventArgs args) => this.ToggleWindow();

  private void ToggleWindow()
  {
    if (this._window == null)
      return;
    if (((BaseWindow) this._window).IsOpen)
      ((BaseWindow) this._window).Close();
    else
      ((BaseWindow) this._window).Open();
  }

  private void UpdateFilterLabel()
  {
    if (this._window == null)
      return;
    if (this._window.FilterButton.SelectedKeys.Count == 0)
    {
      ((Control) this._window.FilterLabel).Visible = false;
    }
    else
    {
      ((Control) this._window.FilterLabel).Visible = true;
      this._window.FilterLabel.Text = Loc.GetString("ui-actionmenu-filter-label", new (string, object)[1]
      {
        ("selectedLabels", (object) string.Join(", ", this._window.FilterButton.SelectedLabels))
      });
    }
  }

  private bool MatchesFilter(Entity<ActionComponent> ent, ActionsWindow.Filters filter)
  {
    EntityUid entityUid1;
    ActionComponent actionComponent1;
    ent.Deconstruct(ref entityUid1, ref actionComponent1);
    EntityUid entityUid2 = entityUid1;
    ActionComponent actionComponent2 = actionComponent1;
    switch (filter)
    {
      case ActionsWindow.Filters.Enabled:
        return actionComponent2.Enabled;
      case ActionsWindow.Filters.Item:
        int num1;
        if (actionComponent2.Container.HasValue)
        {
          EntityUid? container = actionComponent2.Container;
          EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
          num1 = container.HasValue == localEntity.HasValue ? (container.HasValue ? (EntityUid.op_Inequality(container.GetValueOrDefault(), localEntity.GetValueOrDefault()) ? 1 : 0) : 0) : 1;
        }
        else
          num1 = 0;
        return num1 != 0;
      case ActionsWindow.Filters.Innate:
        int num2;
        if (actionComponent2.Container.HasValue)
        {
          EntityUid? container = actionComponent2.Container;
          EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
          num2 = container.HasValue == localEntity.HasValue ? (container.HasValue ? (EntityUid.op_Equality(container.GetValueOrDefault(), localEntity.GetValueOrDefault()) ? 1 : 0) : 1) : 0;
        }
        else
          num2 = 1;
        return num2 != 0;
      case ActionsWindow.Filters.Instant:
        return this.EntityManager.HasComponent<InstantActionComponent>(entityUid2);
      case ActionsWindow.Filters.Targeted:
        return this.EntityManager.HasComponent<TargetActionComponent>(entityUid2);
      default:
        throw new ArgumentOutOfRangeException(nameof (filter), (object) filter, (string) null);
    }
  }

  private void ClearList()
  {
    ActionsWindow window = this._window;
    if ((window != null ? (!((Control) window).Disposed ? 1 : 0) : 0) == 0)
      return;
    ((Control) this._window.ResultsGrid).RemoveAllChildren();
  }

  private void PopulateActions(IEnumerable<Entity<ActionComponent>> actions)
  {
    ActionsWindow window = this._window;
    if (window == null || ((Control) window).Disposed || !((BaseWindow) window).IsOpen || this._actionsSystem == null)
      return;
    this._window.UpdateNeeded = false;
    List<Content.Client.UserInterface.Systems.Actions.Controls.ActionButton> actionButtonList = new List<Content.Client.UserInterface.Systems.Actions.Controls.ActionButton>(((Control) this._window.ResultsGrid).ChildCount);
    foreach (Control child in ((Control) this._window.ResultsGrid).Children)
    {
      if (child is Content.Client.UserInterface.Systems.Actions.Controls.ActionButton actionButton)
        actionButtonList.Add(actionButton);
    }
    int index = 0;
    foreach (Entity<ActionComponent> action in actions)
    {
      if (index < actionButtonList.Count)
      {
        actionButtonList[index++].UpdateData(new EntityUid?(Entity<ActionComponent>.op_Implicit(action)), this._actionsSystem);
      }
      else
      {
        Content.Client.UserInterface.Systems.Actions.Controls.ActionButton actionButton = new Content.Client.UserInterface.Systems.Actions.Controls.ActionButton(this.EntityManager, this._spriteSystem, this)
        {
          Locked = true
        };
        actionButton.ActionPressed += new Action<GUIBoundKeyEventArgs, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton>(this.OnWindowActionPressed);
        actionButton.ActionUnpressed += new Action<GUIBoundKeyEventArgs, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton>(this.OnWindowActionUnPressed);
        actionButton.ActionFocusExited += new Action<Content.Client.UserInterface.Systems.Actions.Controls.ActionButton>(this.OnWindowActionFocusExisted);
        actionButton.UpdateData(new EntityUid?(Entity<ActionComponent>.op_Implicit(action)), this._actionsSystem);
        ((Control) this._window.ResultsGrid).AddChild((Control) actionButton);
      }
    }
    for (; index < actionButtonList.Count; ++index)
      actionButtonList[index].Orphan();
  }

  public void QueueWindowUpdate()
  {
    if (this._window == null)
      return;
    this._window.UpdateNeeded = true;
  }

  private void SearchAndDisplay()
  {
    ActionsWindow window = this._window;
    if (window == null || ((Control) window).Disposed || !((BaseWindow) window).IsOpen || this._actionsSystem == null)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid player = localEntity.GetValueOrDefault();
    string search = this._window.SearchBar.Text;
    IReadOnlyList<ActionsWindow.Filters> filters = this._window.FilterButton.SelectedKeys;
    IEnumerable<Entity<ActionComponent>> clientActions = this._actionsSystem.GetClientActions();
    if (filters.Count == 0 && string.IsNullOrWhiteSpace(search))
      this.PopulateActions(clientActions);
    else
      this.PopulateActions(clientActions.Where<Entity<ActionComponent>>((Func<Entity<ActionComponent>, bool>) (action =>
      {
        if (filters.Count > 0 && filters.Any<ActionsWindow.Filters>((Func<ActionsWindow.Filters, bool>) (filter => !this.MatchesFilter(action, filter))))
          return false;
        if (action.Comp.Keywords.Any<string>((Func<string, bool>) (keyword => search.Contains(keyword, StringComparison.OrdinalIgnoreCase))) || this.EntityManager.GetComponent<MetaDataComponent>(Entity<ActionComponent>.op_Implicit(action)).EntityName.Contains(search, StringComparison.OrdinalIgnoreCase))
          return true;
        if (action.Comp.Container.HasValue)
        {
          EntityUid? container = action.Comp.Container;
          EntityUid entityUid = player;
          if ((container.HasValue ? (EntityUid.op_Equality(container.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
            return this.EntityManager.GetComponent<MetaDataComponent>(action.Comp.Container.Value).EntityName.Contains(search, StringComparison.OrdinalIgnoreCase);
        }
        return false;
      })));
  }

  private void SetAction(
    Content.Client.UserInterface.Systems.Actions.Controls.ActionButton button,
    EntityUid? actionId,
    bool updateSlots = true,
    bool allowOverrideClear = false)
  {
    if (this._actionsSystem == null || this._vehicleHotbarOverride && !actionId.HasValue && !allowOverrideClear)
      return;
    List<EntityUid?> editableHotbarActions = this.GetEditableHotbarActions();
    if (!actionId.HasValue)
    {
      button.ClearData();
      ActionButtonContainer container = this._container;
      int position;
      if ((container != null ? (container.TryGetButtonIndex(button, out position) ? 1 : 0) : 0) != 0 && editableHotbarActions.Count > position && position >= 0)
        editableHotbarActions.RemoveAt(position);
    }
    else
    {
      int position;
      if (button.TryReplaceWith(actionId.Value, this._actionsSystem) && this._container != null && this._container.TryGetButtonIndex(button, out position))
      {
        if (position >= editableHotbarActions.Count)
          editableHotbarActions.Add(actionId);
        else
          editableHotbarActions[position] = actionId;
      }
    }
    if (updateSlots)
      this._container?.SetActionData(this._actionsSystem, this.GetActiveHotbarActions().ToArray<EntityUid?>());
    if (this._vehicleHotbarOverride)
      return;
    this.EntityManager.SystemOrNull<RMCActionsSystem>()?.ActionsChanged(this._actions);
  }

  private void DragAction()
  {
    Content.Client.UserInterface.Systems.Actions.Controls.ActionButton dragged = this._menuDragHelper.Dragged;
    if (dragged != null)
    {
      Entity<ActionComponent>? action = dragged.Action;
      if (action.HasValue)
      {
        Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
        EntityUid? actionId = new EntityUid?();
        if (this.UIManager.MouseGetControl(this._input.MouseScreenPosition) is Content.Client.UserInterface.Systems.Actions.Controls.ActionButton control)
        {
          action = control.Action;
          actionId = action.HasValue ? new EntityUid?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : new EntityUid?();
          this.SetAction(control, new EntityUid?(Entity<ActionComponent>.op_Implicit(valueOrDefault)), false);
        }
        if (dragged.Parent is ActionButtonContainer)
          this.SetAction(dragged, actionId, false, true);
        if (this._actionsSystem != null)
          this._container?.SetActionData(this._actionsSystem, this.GetActiveHotbarActions().ToArray<EntityUid?>());
        this._menuDragHelper.EndDrag();
        return;
      }
    }
    this._menuDragHelper.EndDrag();
  }

  private void OnClearPressed(BaseButton.ButtonEventArgs args)
  {
    if (this._window == null)
      return;
    this._window.SearchBar.Clear();
    this._window.FilterButton.DeselectAll();
    this.UpdateFilterLabel();
    this.QueueWindowUpdate();
  }

  private void OnSearchChanged(LineEdit.LineEditEventArgs args) => this.QueueWindowUpdate();

  private void OnFilterSelected(
    MultiselectOptionButton<ActionsWindow.Filters>.ItemPressedEventArgs args)
  {
    this.UpdateFilterLabel();
    this.QueueWindowUpdate();
  }

  private void OnWindowActionPressed(GUIBoundKeyEventArgs args, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton action)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick) && BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.Use))
      return;
    this.HandleActionPressed(args, action);
  }

  private void OnWindowActionUnPressed(GUIBoundKeyEventArgs args, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton dragged)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick) && BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.Use))
      return;
    this.HandleActionUnpressed(args, dragged);
  }

  private void OnWindowActionFocusExisted(Content.Client.UserInterface.Systems.Actions.Controls.ActionButton button)
  {
    this._menuDragHelper.EndDrag();
  }

  private void OnActionPressed(GUIBoundKeyEventArgs args, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton button)
  {
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
    {
      if (this._vehicleHotbarOverride)
      {
        ((BoundKeyEventArgs) args).Handle();
      }
      else
      {
        this.SetAction(button, new EntityUid?());
        ((BoundKeyEventArgs) args).Handle();
      }
    }
    else
    {
      if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
        return;
      this.HandleActionPressed(args, button);
    }
  }

  private void HandleActionPressed(GUIBoundKeyEventArgs args, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton button)
  {
    ((BoundKeyEventArgs) args).Handle();
    if (!button.Action.HasValue)
      return;
    this._menuDragHelper.MouseDown(button);
  }

  private void OnActionUnpressed(GUIBoundKeyEventArgs args, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton button)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
      return;
    this.HandleActionUnpressed(args, button);
  }

  private void HandleActionUnpressed(GUIBoundKeyEventArgs args, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton button)
  {
    if (this._actionsSystem == null)
      return;
    ((BoundKeyEventArgs) args).Handle();
    if (this._menuDragHelper.IsDragging)
    {
      this.DragAction();
    }
    else
    {
      this._menuDragHelper.EndDrag();
      Entity<ActionComponent>? action = button.Action;
      if (!action.HasValue)
        return;
      Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
      TargetActionComponent targetActionComponent;
      if (!this.EntityManager.TryGetComponent<TargetActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), ref targetActionComponent))
        this._actionsSystem?.TriggerAction(valueOrDefault);
      else
        this.ToggleTargeting(Entity<ActionComponent, TargetActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, targetActionComponent)));
    }
  }

  private bool OnMenuBeginDrag()
  {
    Entity<ActionComponent>? action = (Entity<ActionComponent>?) this._menuDragHelper.Dragged?.Action;
    if (action.HasValue)
    {
      Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
      SpriteComponent spriteComponent;
      if (this.EntityManager.TryGetComponent<SpriteComponent>(valueOrDefault.Comp.EntityIcon, ref spriteComponent))
      {
        Texture frame = spriteComponent.Icon?.GetFrame((RsiDirection) 0, 0);
        if (frame != null)
        {
          this._dragShadow.Texture = frame;
          goto label_5;
        }
      }
      SpriteSpecifier icon = valueOrDefault.Comp.Icon;
      this._dragShadow.Texture = icon == null ? (Texture) null : this._spriteSystem.Frame0(icon);
    }
label_5:
    LayoutContainer.SetPosition((Control) this._dragShadow, this.UIManager.MousePositionScaled.Position - new Vector2(32f, 32f));
    return true;
  }

  private bool OnMenuContinueDrag(float frameTime)
  {
    LayoutContainer.SetPosition((Control) this._dragShadow, this.UIManager.MousePositionScaled.Position - new Vector2(32f, 32f));
    ((Control) this._dragShadow).Visible = true;
    return true;
  }

  private void OnMenuEndDrag()
  {
    this._dragShadow.Texture = (Texture) null;
    ((Control) this._dragShadow).Visible = false;
  }

  private void UnloadGui()
  {
    this._actionsSystem?.UnlinkAllActions();
    if (this.ActionsBar == null || this._window == null)
      return;
    ((BaseWindow) this._window).OnOpen -= new Action(this.OnWindowOpened);
    ((BaseWindow) this._window).OnClose -= new Action(this.OnWindowClosed);
    ((BaseButton) this._window.ClearButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.OnClearPressed);
    this._window.SearchBar.OnTextChanged -= new Action<LineEdit.LineEditEventArgs>(this.OnSearchChanged);
    this._window.FilterButton.OnItemSelected -= new Action<MultiselectOptionButton<ActionsWindow.Filters>.ItemPressedEventArgs>(this.OnFilterSelected);
    if (!((Control) this._window).Disposed)
      ((Control) this._window).Orphan();
    this._window = (ActionsWindow) null;
  }

  private void LoadGui()
  {
    this.UnloadGui();
    this._window = this.UIManager.CreateWindow<ActionsWindow>();
    LayoutContainer.SetAnchorPreset((Control) this._window, (LayoutContainer.LayoutPreset) 5, false);
    ((BaseWindow) this._window).OnOpen += new Action(this.OnWindowOpened);
    ((BaseWindow) this._window).OnClose += new Action(this.OnWindowClosed);
    ((BaseButton) this._window.ClearButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnClearPressed);
    this._window.SearchBar.OnTextChanged += new Action<LineEdit.LineEditEventArgs>(this.OnSearchChanged);
    this._window.FilterButton.OnItemSelected += new Action<MultiselectOptionButton<ActionsWindow.Filters>.ItemPressedEventArgs>(this.OnFilterSelected);
    if (this.ActionsBar == null)
      return;
    this.RegisterActionContainer(this.ActionsBar.ActionsContainer);
    this._actionsSystem?.LinkAllActions();
  }

  public void RegisterActionContainer(ActionButtonContainer container)
  {
    if (this._container != null)
    {
      this._container.ActionPressed -= new Action<GUIBoundKeyEventArgs, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton>(this.OnActionPressed);
      this._container.ActionUnpressed -= new Action<GUIBoundKeyEventArgs, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton>(this.OnActionUnpressed);
    }
    this._container = container;
    this._container.ActionPressed += new Action<GUIBoundKeyEventArgs, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton>(this.OnActionPressed);
    this._container.ActionUnpressed += new Action<GUIBoundKeyEventArgs, Content.Client.UserInterface.Systems.Actions.Controls.ActionButton>(this.OnActionUnpressed);
  }

  private void ClearActions()
  {
    this._actions.Clear();
    this._vehicleActions.Clear();
    this._vehicleHotbarOverride = false;
    this._container?.ClearActionData();
  }

  private void AssignSlots(List<ActionsSystem.SlotAssignment> assignments)
  {
    if (this._actionsSystem == null)
      return;
    this._actions.Clear();
    foreach (ActionsSystem.SlotAssignment assignment in assignments)
    {
      if (!this.EntityManager.HasComponent<VehicleHardpointActionComponent>(assignment.ActionId))
        this._actions.Add(new EntityUid?(assignment.ActionId));
    }
    this.RefreshVehicleHotbarOverride(true);
  }

  public void RemoveActionContainer() => this._container = (ActionButtonContainer) null;

  public void OnSystemLoaded(ActionsSystem system)
  {
    system.LinkActions += new Action<ActionsComponent>(this.OnComponentLinked);
    system.UnlinkActions += new Action(this.OnComponentUnlinked);
    system.ClearAssignments += new Action(this.ClearActions);
    system.AssignSlot += new Action<List<ActionsSystem.SlotAssignment>>(this.AssignSlots);
  }

  public void OnSystemUnloaded(ActionsSystem system)
  {
    system.LinkActions -= new Action<ActionsComponent>(this.OnComponentLinked);
    system.UnlinkActions -= new Action(this.OnComponentUnlinked);
    system.ClearAssignments -= new Action(this.ClearActions);
    system.AssignSlot -= new Action<List<ActionsSystem.SlotAssignment>>(this.AssignSlots);
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    this._menuDragHelper.Update(((FrameEventArgs) ref args).DeltaSeconds);
    this.RefreshVehicleHotbarOverride();
    ActionsWindow window = this._window;
    if (window == null || !window.UpdateNeeded)
      return;
    this.SearchAndDisplay();
  }

  private void OnComponentLinked(ActionsComponent component)
  {
    if (this._actionsSystem == null)
      return;
    this.LoadDefaultActions();
    this.RefreshVehicleHotbarOverride(true);
    this.QueueWindowUpdate();
  }

  private void OnComponentUnlinked()
  {
    this._vehicleHotbarOverride = false;
    this._vehicleActions.Clear();
    this._container?.ClearActionData();
    this.QueueWindowUpdate();
    this.StopTargeting();
  }

  private void LoadDefaultActions()
  {
    if (this._actionsSystem == null)
      return;
    List<Entity<ActionComponent>> list = this._actionsSystem.GetClientActions().Where<Entity<ActionComponent>>((Func<Entity<ActionComponent>, bool>) (action => action.Comp.AutoPopulate)).ToList<Entity<ActionComponent>>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    list.Sort(ActionUIController.\u003C\u003EO.\u003C0\u003E__ActionComparer ?? (ActionUIController.\u003C\u003EO.\u003C0\u003E__ActionComparer = new Comparison<Entity<ActionComponent>>(ActionsSystem.ActionComparer)));
    this._actions.Clear();
    foreach (Entity<ActionComponent> entity in list)
    {
      EntityUid entityUid1;
      ActionComponent actionComponent;
      entity.Deconstruct(ref entityUid1, ref actionComponent);
      EntityUid entityUid2 = entityUid1;
      if (!this.EntityManager.HasComponent<VehicleHardpointActionComponent>(entityUid2) && !this._actions.Contains(new EntityUid?(entityUid2)))
        this._actions.Add(new EntityUid?(entityUid2));
    }
  }

  private void ToggleTargeting(Entity<ActionComponent, TargetActionComponent> ent)
  {
    EntityUid? selectingTargetFor = this.SelectingTargetFor;
    EntityUid entityUid = Entity<ActionComponent, TargetActionComponent>.op_Implicit(ent);
    if ((selectingTargetFor.HasValue ? (EntityUid.op_Equality(selectingTargetFor.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0)
      this.StopTargeting();
    else
      this.StartTargeting(ent);
  }

  private void StartTargeting(Entity<ActionComponent, TargetActionComponent> ent)
  {
    EntityUid entityUid1;
    ActionComponent actionComponent1;
    TargetActionComponent targetActionComponent1;
    ent.Deconstruct(ref entityUid1, ref actionComponent1, ref targetActionComponent1);
    EntityUid entityUid2 = entityUid1;
    ActionComponent actionComponent2 = actionComponent1;
    TargetActionComponent targetActionComponent2 = targetActionComponent1;
    this.StopTargeting();
    this.SelectingTargetFor = new EntityUid?(entityUid2);
    this._actionsSystem?.SetToggled(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(entityUid2)), true);
    EntityUid? container = actionComponent2.Container;
    ShowHandItemOverlay showHandItemOverlay;
    if (targetActionComponent2.TargetingIndicator && this._overlays.TryGetOverlay<ShowHandItemOverlay>(ref showHandItemOverlay))
    {
      if (actionComponent2.ItemIconStyle == ItemActionIconStyle.BigItem && actionComponent2.Container.HasValue)
        showHandItemOverlay.EntityOverride = container;
      else if (actionComponent2.Toggled && actionComponent2.IconOn != null)
        showHandItemOverlay.IconOverride = this._spriteSystem.Frame0(actionComponent2.IconOn);
      else if (actionComponent2.Icon != null)
        showHandItemOverlay.IconOverride = this._spriteSystem.Frame0(actionComponent2.Icon);
    }
    if (this._container != null)
    {
      foreach (Content.Client.UserInterface.Systems.Actions.Controls.ActionButton button in this._container.GetButtons())
      {
        Entity<ActionComponent>? action = button.Action;
        ref Entity<ActionComponent>? local = ref action;
        if ((local.HasValue ? (EntityUid.op_Equality(local.GetValueOrDefault().Owner, entityUid2) ? 1 : 0) : 0) != 0)
          button.UpdateIcons();
      }
    }
    EntityTargetActionComponent targetActionComponent3;
    if (!this.EntityManager.TryGetComponent<EntityTargetActionComponent>(entityUid2, ref targetActionComponent3) || !targetActionComponent3.ToggleOutline)
      return;
    Func<EntityUid, bool> predicate = (Func<EntityUid, bool>) null;
    EntityUid? attachedEnt = actionComponent2.AttachedEntity;
    if (!targetActionComponent3.CanTargetSelf)
      predicate = (Func<EntityUid, bool>) (e =>
      {
        EntityUid entityUid3 = e;
        EntityUid? nullable = attachedEnt;
        return !nullable.HasValue || EntityUid.op_Inequality(entityUid3, nullable.GetValueOrDefault());
      });
    float range = targetActionComponent2.CheckCanAccess ? targetActionComponent2.Range : -1f;
    this._interactionOutline?.SetEnabled(false);
    this._targetOutline?.Enable(range, targetActionComponent2.CheckCanAccess, predicate, targetActionComponent3.Whitelist, targetActionComponent3.Blacklist, (CancellableEntityEventArgs) null);
  }

  private void StopTargeting()
  {
    if (!this.SelectingTargetFor.HasValue)
      return;
    EntityUid? selectingTargetFor = this.SelectingTargetFor;
    ActionsSystem actionsSystem = this._actionsSystem;
    EntityUid? nullable;
    if (actionsSystem != null)
    {
      nullable = selectingTargetFor;
      actionsSystem.SetToggled(nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?(), false);
    }
    nullable = new EntityUid?();
    this.SelectingTargetFor = nullable;
    this._targetOutline?.Disable();
    this._interactionOutline?.SetEnabled(true);
    if (this._container != null)
    {
      foreach (Content.Client.UserInterface.Systems.Actions.Controls.ActionButton button in this._container.GetButtons())
      {
        Entity<ActionComponent>? action = button.Action;
        ref Entity<ActionComponent>? local = ref action;
        int num;
        if (!local.HasValue)
        {
          num = !selectingTargetFor.HasValue ? 1 : 0;
        }
        else
        {
          EntityUid owner = local.GetValueOrDefault().Owner;
          nullable = selectingTargetFor;
          num = nullable.HasValue ? (EntityUid.op_Equality(owner, nullable.GetValueOrDefault()) ? 1 : 0) : 0;
        }
        if (num != 0)
          button.UpdateIcons();
      }
    }
    ShowHandItemOverlay showHandItemOverlay;
    if (!this._overlays.TryGetOverlay<ShowHandItemOverlay>(ref showHandItemOverlay))
      return;
    showHandItemOverlay.IconOverride = (Texture) null;
    showHandItemOverlay.EntityOverride = new EntityUid?();
  }

  private IReadOnlyList<EntityUid?> GetActiveHotbarActions()
  {
    return !this._vehicleHotbarOverride ? (IReadOnlyList<EntityUid?>) this._actions : (IReadOnlyList<EntityUid?>) this._vehicleActions;
  }

  private List<EntityUid?> GetEditableHotbarActions()
  {
    return !this._vehicleHotbarOverride ? this._actions : this._vehicleActions;
  }

  private void RefreshVehicleHotbarOverride(bool forceUpdate = false)
  {
    if (this._actionsSystem == null)
      return;
    bool flag1 = this.HasVehicleHotbarContext();
    bool flag2 = this.ShouldUseVehicleHotbarOverride();
    bool flag3 = false;
    if (!flag2 && !this._vehicleHotbarOverride && !forceUpdate)
      return;
    if (flag2)
    {
      if (!this._vehicleHotbarOverride)
      {
        this._vehicleHotbarOverride = true;
        flag3 = true;
      }
      if (this.RebuildVehicleActionList())
        flag3 = true;
    }
    else if (this._vehicleHotbarOverride)
    {
      this._vehicleHotbarOverride = false;
      if (!flag1)
        this._vehicleActions.Clear();
      flag3 = true;
    }
    else if (!flag1 && this._vehicleActions.Count > 0)
      this._vehicleActions.Clear();
    if (!(flag3 | forceUpdate))
      return;
    this._container?.SetActionData(this._actionsSystem, this.GetActiveHotbarActions().ToArray<EntityUid?>());
  }

  private bool ShouldUseVehicleHotbarOverride()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    return localEntity.HasValue && this.EntityManager.HasComponent<VehicleWeaponsOperatorComponent>(localEntity.GetValueOrDefault());
  }

  private bool HasVehicleHotbarContext()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    return localEntity.HasValue && this.EntityManager.HasComponent<VehicleWeaponsOperatorComponent>(localEntity.GetValueOrDefault());
  }

  private bool RebuildVehicleActionList()
  {
    if (this._actionsSystem == null)
      return false;
    List<EntityUid?> list = this._vehicleActions.ToList<EntityUid?>();
    List<EntityUid?> nullableList = new List<EntityUid?>();
    bool flag = true;
    EntityUid? nullable1 = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    VehicleViewToggleComponent viewToggleComponent1;
    if (nullable1.HasValue && this.EntityManager.TryGetComponent<VehicleViewToggleComponent>(nullable1.GetValueOrDefault(), ref viewToggleComponent1))
      flag = viewToggleComponent1.IsOutside;
    if (!flag)
    {
      foreach (EntityUid? action in this._actions)
      {
        if (action.HasValue)
        {
          EntityUid valueOrDefault = action.GetValueOrDefault();
          if (!this.EntityManager.HasComponent<VehicleHardpointActionComponent>(valueOrDefault))
            nullableList.Add(new EntityUid?(valueOrDefault));
        }
      }
    }
    else
    {
      nullable1 = ((ISharedPlayerManager) this._playerManager).LocalEntity;
      CombatModeComponent combatModeComponent;
      if (nullable1.HasValue && this.EntityManager.TryGetComponent<CombatModeComponent>(nullable1.GetValueOrDefault(), ref combatModeComponent))
      {
        nullable1 = combatModeComponent.CombatToggleActionEntity;
        if (nullable1.HasValue)
        {
          EntityUid valueOrDefault = nullable1.GetValueOrDefault();
          if (!this.EntityManager.HasComponent<VehicleHardpointActionComponent>(valueOrDefault))
            nullableList.Add(new EntityUid?(valueOrDefault));
        }
      }
    }
    if (flag)
    {
      VehicleHardpointActionComponent hardpointActionComponent1;
      foreach (Entity<ActionComponent> entity in (IEnumerable<Entity<ActionComponent>>) this._actionsSystem.GetClientActions().Where<Entity<ActionComponent>>((Func<Entity<ActionComponent>, bool>) (action => this.EntityManager.TryGetComponent<VehicleHardpointActionComponent>(Entity<ActionComponent>.op_Implicit(action), ref hardpointActionComponent1))).OrderBy<Entity<ActionComponent>, int>((Func<Entity<ActionComponent>, int>) (action =>
      {
        VehicleHardpointActionComponent hardpointActionComponent2;
        this.EntityManager.TryGetComponent<VehicleHardpointActionComponent>(Entity<ActionComponent>.op_Implicit(action), ref hardpointActionComponent2);
        return hardpointActionComponent2 == null ? 0 : hardpointActionComponent2.SortOrder;
      })))
      {
        EntityUid entityUid1;
        ActionComponent actionComponent;
        entity.Deconstruct(ref entityUid1, ref actionComponent);
        EntityUid entityUid2 = entityUid1;
        nullableList.Add(new EntityUid?(entityUid2));
      }
    }
    EntityUid? nullable2 = new EntityUid?();
    nullable1 = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    VehicleViewToggleComponent viewToggleComponent2;
    if (nullable1.HasValue && this.EntityManager.TryGetComponent<VehicleViewToggleComponent>(nullable1.GetValueOrDefault(), ref viewToggleComponent2))
    {
      nullable1 = viewToggleComponent2.Action;
      if (nullable1.HasValue)
      {
        EntityUid valueOrDefault = nullable1.GetValueOrDefault();
        if (!this.EntityManager.HasComponent<VehicleHardpointActionComponent>(valueOrDefault))
          nullable2 = new EntityUid?(valueOrDefault);
      }
    }
    if (!nullable2.HasValue)
    {
      foreach (Entity<ActionComponent> clientAction in this._actionsSystem.GetClientActions())
      {
        EntityUid owner = clientAction.Owner;
        MetaDataComponent metaDataComponent;
        if (!this.EntityManager.HasComponent<VehicleHardpointActionComponent>(owner) && this.EntityManager.TryGetComponent<MetaDataComponent>(owner, ref metaDataComponent) && !(metaDataComponent.EntityPrototype?.ID != "ActionVehicleToggleView"))
        {
          nullable2 = new EntityUid?(owner);
          break;
        }
      }
    }
    if (nullable2.HasValue)
    {
      EntityUid valueOrDefault = nullable2.GetValueOrDefault();
      nullableList.Add(new EntityUid?(valueOrDefault));
    }
    EntityUid? nullable3 = new EntityUid?();
    nullable1 = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    VehicleLockActionComponent lockActionComponent;
    if (nullable1.HasValue && this.EntityManager.TryGetComponent<VehicleLockActionComponent>(nullable1.GetValueOrDefault(), ref lockActionComponent))
    {
      nullable1 = lockActionComponent.Action;
      if (nullable1.HasValue)
      {
        EntityUid valueOrDefault = nullable1.GetValueOrDefault();
        if (!this.EntityManager.HasComponent<VehicleHardpointActionComponent>(valueOrDefault))
          nullable3 = new EntityUid?(valueOrDefault);
      }
    }
    if (!nullable3.HasValue)
    {
      foreach (Entity<ActionComponent> clientAction in this._actionsSystem.GetClientActions())
      {
        EntityUid owner = clientAction.Owner;
        MetaDataComponent metaDataComponent;
        if (!this.EntityManager.HasComponent<VehicleHardpointActionComponent>(owner) && this.EntityManager.TryGetComponent<MetaDataComponent>(owner, ref metaDataComponent) && !(metaDataComponent.EntityPrototype?.ID != "ActionVehicleLock"))
        {
          nullable3 = new EntityUid?(owner);
          break;
        }
      }
    }
    if (nullable3.HasValue)
    {
      EntityUid valueOrDefault = nullable3.GetValueOrDefault();
      nullableList.Add(new EntityUid?(valueOrDefault));
    }
    HashSet<EntityUid> entityUidSet = new HashSet<EntityUid>();
    foreach (EntityUid? nullable4 in nullableList)
    {
      if (nullable4.HasValue)
      {
        EntityUid valueOrDefault = nullable4.GetValueOrDefault();
        entityUidSet.Add(valueOrDefault);
      }
    }
    List<EntityUid?> collection = new List<EntityUid?>();
    foreach (EntityUid? vehicleAction in this._vehicleActions)
    {
      if (vehicleAction.HasValue)
      {
        EntityUid valueOrDefault = vehicleAction.GetValueOrDefault();
        if (entityUidSet.Remove(valueOrDefault))
          collection.Add(new EntityUid?(valueOrDefault));
      }
    }
    foreach (EntityUid? nullable5 in nullableList)
    {
      if (nullable5.HasValue)
      {
        EntityUid valueOrDefault = nullable5.GetValueOrDefault();
        if (entityUidSet.Remove(valueOrDefault))
          collection.Add(new EntityUid?(valueOrDefault));
      }
    }
    this._vehicleActions.Clear();
    this._vehicleActions.AddRange((IEnumerable<EntityUid?>) collection);
    if (list.Count != this._vehicleActions.Count)
      return true;
    for (int index = 0; index < list.Count; ++index)
    {
      EntityUid? nullable6 = list[index];
      EntityUid? vehicleAction = this._vehicleActions[index];
      if ((nullable6.HasValue == vehicleAction.HasValue ? (nullable6.HasValue ? (EntityUid.op_Inequality(nullable6.GetValueOrDefault(), vehicleAction.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
        return true;
    }
    return false;
  }
}
