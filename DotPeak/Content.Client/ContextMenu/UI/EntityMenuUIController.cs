// Decompiled with JetBrains decompiler
// Type: Content.Client.ContextMenu.UI.EntityMenuUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CombatMode;
using Content.Client.Examine;
using Content.Client.Gameplay;
using Content.Client.Verbs;
using Content.Client.Verbs.UI;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared.CCVar;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.ContextMenu.UI;

public sealed class EntityMenuUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  public const int GroupingTypesCount = 2;
  [Dependency]
  private IEntitySystemManager _systemManager;
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IStateManager _stateManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private ContextMenuUIController _context;
  [Dependency]
  private VerbMenuUIController _verb;
  [UISystemDependency]
  private readonly VerbSystem _verbSystem;
  [UISystemDependency]
  private readonly ExamineSystem _examineSystem;
  [UISystemDependency]
  private readonly TransformSystem _xform;
  [UISystemDependency]
  private readonly CombatModeSystem _combatMode;
  private bool _updating;
  public Dictionary<EntityUid, EntityMenuElement> Elements = new Dictionary<EntityUid, EntityMenuElement>();

  private int GroupingContextMenuType { get; set; }

  public void OnGroupingChanged(int obj)
  {
    this._context.Close();
    this.GroupingContextMenuType = obj;
  }

  private List<List<EntityUid>> GroupEntities(IEnumerable<EntityUid> entities, int depth = 0)
  {
    return this.GroupingContextMenuType == 0 ? entities.GroupBy<EntityUid, IdentityEntity>((Func<EntityUid, IdentityEntity>) (e => Identity.Name(e, this._entityManager, ((ISharedPlayerManager) this._playerManager).LocalEntity))).ToList<IGrouping<IdentityEntity, EntityUid>>().Select<IGrouping<IdentityEntity, EntityUid>, List<EntityUid>>((Func<IGrouping<IdentityEntity, EntityUid>, List<EntityUid>>) (grp => grp.ToList<EntityUid>())).ToList<List<EntityUid>>() : entities.GroupBy<EntityUid, EntityUid>((Func<EntityUid, EntityUid>) (e => e), (IEqualityComparer<EntityUid>) new EntityMenuUIController.PrototypeAndStatesContextMenuComparer(depth, this._entityManager)).ToList<IGrouping<EntityUid, EntityUid>>().Select<IGrouping<EntityUid, EntityUid>, List<EntityUid>>((Func<IGrouping<EntityUid, EntityUid>, List<EntityUid>>) (grp => grp.ToList<EntityUid>())).ToList<List<EntityUid>>();
  }

  public void OnStateEntered(GameplayState state)
  {
    this._updating = true;
    this._cfg.OnValueChanged<int>(CCVars.EntityMenuGroupingType, new Action<int>(this.OnGroupingChanged), true);
    this._context.OnContextKeyEvent += new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown);
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(EngineKeyFunctions.UseSecondary, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleOpenEntityMenu)), true, true)).Register<EntityMenuUIController>();
  }

  public void OnStateExited(GameplayState state)
  {
    this._updating = false;
    this.Elements.Clear();
    this._cfg.UnsubValueChanged<int>(CCVars.EntityMenuGroupingType, new Action<int>(this.OnGroupingChanged));
    this._context.OnContextKeyEvent -= new Action<ContextMenuElement, GUIBoundKeyEventArgs>(this.OnKeyBindDown);
    CommandBinds.Unregister<EntityMenuUIController>();
  }

  public void OpenRootMenu(List<EntityUid> entities)
  {
    if (((Control) this._context.RootMenu).Visible)
      this._context.Close();
    List<List<EntityUid>> list = this.GroupEntities((IEnumerable<EntityUid>) entities).ToList<List<EntityUid>>();
    list.Sort((Comparison<List<EntityUid>>) ((x, y) => string.Compare((string) Identity.Name(x.First<EntityUid>(), this._entityManager, ((ISharedPlayerManager) this._playerManager).LocalEntity), (string) Identity.Name(y.First<EntityUid>(), this._entityManager, ((ISharedPlayerManager) this._playerManager).LocalEntity), StringComparison.CurrentCulture)));
    this.Elements.Clear();
    this.AddToUI(list);
    this._context.RootMenu.Open(new UIBox2?(UIBox2.FromDimensions(this._userInterfaceManager.MousePositionScaled.Position, new Vector2(1f, 1f))), new Vector2?(), new Vector2?());
  }

  public void OnKeyBindDown(ContextMenuElement element, GUIBoundKeyEventArgs args)
  {
    if (!(element is EntityMenuElement entityMenuElement))
      return;
    EntityUid? nullable1 = entityMenuElement.Entity;
    EntityUid? nullable2 = nullable1;
    if (!nullable2.HasValue)
      nullable1 = this.GetFirstEntityOrNull(element.SubMenu);
    if (this._entityManager.Deleted(nullable1))
      return;
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ExamineEntity))
    {
      ExamineSystem entitySystem = this._systemManager.GetEntitySystem<ExamineSystem>();
      EntityUid entity = nullable1.Value;
      nullable2 = new EntityUid?();
      EntityUid? userOverride = nullable2;
      entitySystem.DoExamine(entity, userOverride: userOverride);
      ((BoundKeyEventArgs) args).Handle();
    }
    else
    {
      if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.Use) && !BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.ActivateItemInWorld) && !BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.AltActivateItemInWorld) && !BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.Point) && !BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.TryPullObject) && !BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.MovePulledObject))
        return;
      InputSystem entitySystem = this._systemManager.GetEntitySystem<InputSystem>();
      BoundKeyFunction function = ((BoundKeyEventArgs) args).Function;
      ClientFullInputCmdMessage fullInputCmdMessage = new ClientFullInputCmdMessage(this._gameTiming.CurTick, this._gameTiming.TickFraction, this._inputManager.NetworkBindMap.KeyFunctionID(function))
      {
        State = (BoundKeyState) 1,
        Coordinates = this._entityManager.GetComponent<TransformComponent>(nullable1.Value).Coordinates,
        ScreenCoordinates = ((BoundKeyEventArgs) args).PointerLocation,
        Uid = nullable1.Value
      };
      ICommonSession localSession = ((ISharedPlayerManager) this._playerManager).LocalSession;
      if (localSession != null)
        entitySystem.HandleInputCommand(localSession, function, (IFullInputCmdMessage) fullInputCmdMessage, false);
      this._context.Close();
      ((BoundKeyEventArgs) args).Handle();
    }
  }

  private bool HandleOpenEntityMenu(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State != 1 || !(this._stateManager.CurrentState is GameplayStateBase) || this._combatMode.IsInCombatMode((EntityUid?) args.Session?.AttachedEntity))
      return false;
    List<EntityUid> entities;
    if (this._verbSystem.TryGetEntityMenuEntities(((SharedTransformSystem) this._xform).ToMapCoordinates(args.Coordinates, true), out entities))
      this.OpenRootMenu(entities);
    return true;
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    if (!this._updating || this._context.RootMenu == null || !((Control) this._context.RootMenu).Visible)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid player = localEntity.GetValueOrDefault();
    if (!((EntityUid) ref player).IsValid())
      return;
    MenuVisibility visibility1 = this._verbSystem.Visibility;
    if (!this._eyeManager.CurrentEye.DrawFov)
      visibility1 &= ~MenuVisibility.NoFov;
    MenuVisibilityEvent menuVisibilityEvent = new MenuVisibilityEvent()
    {
      Visibility = visibility1
    };
    ((IDirectedEventBus) this._entityManager.EventBus).RaiseLocalEvent<MenuVisibilityEvent>(player, ref menuVisibilityEvent, false);
    MenuVisibility visibility2 = menuVisibilityEvent.Visibility;
    ExaminerComponent examinerComp;
    this._entityManager.TryGetComponent<ExaminerComponent>(player, ref examinerComp);
    EntityQuery<TransformComponent> entityQuery = this._entityManager.GetEntityQuery<TransformComponent>();
    foreach (EntityUid entityUid in this.Elements.Keys.ToList<EntityUid>())
    {
      EntityUid entity = entityUid;
      TransformComponent transformComponent;
      if (!entityQuery.TryGetComponent(entity, ref transformComponent))
        this.RemoveEntity(entity);
      else if ((visibility2 & MenuVisibility.NoFov) != MenuVisibility.NoFov)
      {
        MapCoordinates target;
        // ISSUE: explicit constructor call
        ((MapCoordinates) ref target).\u002Ector(((SharedTransformSystem) this._xform).GetWorldPosition(transformComponent, entityQuery), transformComponent.MapID);
        if (!this._examineSystem.CanExamine(player, target, (SharedInteractionSystem.Ignored) (e => EntityUid.op_Equality(e, player) || EntityUid.op_Equality(e, entity)), new EntityUid?(entity), examinerComp))
          this.RemoveEntity(entity);
      }
    }
  }

  private void AddToUI(List<List<EntityUid>> entityGroups)
  {
    if (entityGroups.Count == 1)
    {
      this.AddGroupToMenu(entityGroups[0], this._context.RootMenu);
    }
    else
    {
      foreach (List<EntityUid> entityGroup in entityGroups)
      {
        if (entityGroup.Count > 1)
          this.AddGroupToUI(entityGroup);
        else
          this.AddEntityToMenu(entityGroup[0], this._context.RootMenu);
      }
    }
  }

  private void AddGroupToUI(List<EntityUid> group)
  {
    EntityMenuElement entityMenuElement = new EntityMenuElement();
    ContextMenuPopup menu = new ContextMenuPopup(this._context, (ContextMenuElement) entityMenuElement);
    this.AddGroupToMenu(group, menu);
    this.UpdateElement(entityMenuElement);
    this._context.AddElement(this._context.RootMenu, (ContextMenuElement) entityMenuElement);
  }

  private void AddGroupToMenu(List<EntityUid> group, ContextMenuPopup menu)
  {
    foreach (EntityUid entity in group)
      this.AddEntityToMenu(entity, menu);
  }

  private void AddEntityToMenu(EntityUid entity, ContextMenuPopup menu)
  {
    EntityMenuElement element = new EntityMenuElement(new EntityUid?(entity));
    element.SubMenu = new ContextMenuPopup(this._context, (ContextMenuElement) element);
    element.SubMenu.OnPopupOpen += (Action) (() => this._verb.OpenVerbMenu(entity, popup: element.SubMenu));
    element.SubMenu.OnPopupHide += new Action(((Control) element.SubMenu.MenuBody).DisposeAllChildren);
    this._context.AddElement(menu, (ContextMenuElement) element);
    this.Elements.TryAdd(entity, element);
  }

  private void RemoveEntity(EntityUid entity)
  {
    EntityMenuElement entityMenuElement;
    if (!this.Elements.TryGetValue(entity, out entityMenuElement))
    {
      this.Log.Error($"Attempted to remove unknown entity from the entity menu: {this._entityManager.GetComponent<MetaDataComponent>(entity).EntityName} ({entity})");
    }
    else
    {
      ContextMenuElement parentElement = entityMenuElement.ParentMenu?.ParentElement;
      ((Control) entityMenuElement).Orphan();
      this.Elements.Remove(entity);
      if (parentElement is EntityMenuElement element)
        this.UpdateElement(element);
      if (((Control) this._context.RootMenu.MenuBody).ChildCount != 0)
        return;
      this._context.Close();
    }
  }

  private void UpdateElement(EntityMenuElement element)
  {
    if (element.SubMenu == null)
      return;
    EntityUid? firstEntityOrNull = this.GetFirstEntityOrNull(element.SubMenu);
    if (!firstEntityOrNull.HasValue)
    {
      ((Control) element).Orphan();
    }
    else
    {
      element.UpdateEntity(firstEntityOrNull);
      element.UpdateCount();
      if (element.Count == 1)
      {
        element.Entity = firstEntityOrNull;
        ((Control) element.SubMenu).Orphan();
        element.SubMenu = (ContextMenuPopup) null;
        this.Elements[firstEntityOrNull.Value] = element;
      }
      if (!(element.ParentMenu?.ParentElement is EntityMenuElement parentElement))
        return;
      this.UpdateElement(parentElement);
    }
  }

  private EntityUid? GetFirstEntityOrNull(ContextMenuPopup? menu)
  {
    if (menu == null)
      return new EntityUid?();
    foreach (Control child in ((Control) menu.MenuBody).Children)
    {
      if (child is EntityMenuElement entityMenuElement)
      {
        if (entityMenuElement.Entity.HasValue)
        {
          if (!this._entityManager.Deleted(entityMenuElement.Entity))
            return entityMenuElement.Entity;
        }
        else
        {
          EntityUid? firstEntityOrNull = this.GetFirstEntityOrNull(entityMenuElement.SubMenu);
          if (firstEntityOrNull.HasValue)
            return firstEntityOrNull;
        }
      }
    }
    return new EntityUid?();
  }

  private sealed class PrototypeAndStatesContextMenuComparer : IEqualityComparer<EntityUid>
  {
    private static readonly List<Func<EntityUid, EntityUid, IEntityManager, bool>> EqualsList = new List<Func<EntityUid, EntityUid, IEntityManager, bool>>()
    {
      (Func<EntityUid, EntityUid, IEntityManager, bool>) ((a, b, entMan) => entMan.GetComponent<MetaDataComponent>(a).EntityPrototype.ID == entMan.GetComponent<MetaDataComponent>(b).EntityPrototype.ID),
      (Func<EntityUid, EntityUid, IEntityManager, bool>) ((a, b, entMan) =>
      {
        SpriteComponent spriteComponent1;
        entMan.TryGetComponent<SpriteComponent>(a, ref spriteComponent1);
        SpriteComponent spriteComponent2;
        entMan.TryGetComponent<SpriteComponent>(b, ref spriteComponent2);
        if (spriteComponent1 == null || spriteComponent2 == null)
          return spriteComponent1 == spriteComponent2;
        IEnumerable<string> source1 = spriteComponent1.AllLayers.Where<ISpriteLayer>((Func<ISpriteLayer, bool>) (e => e.Visible)).Select<ISpriteLayer, string>((Func<ISpriteLayer, string>) (s => s.RsiState.Name));
        IEnumerable<string> source2 = spriteComponent2.AllLayers.Where<ISpriteLayer>((Func<ISpriteLayer, bool>) (e => e.Visible)).Select<ISpriteLayer, string>((Func<ISpriteLayer, string>) (s => s.RsiState.Name));
        return source1.OrderBy<string, string>((Func<string, string>) (t => t)).SequenceEqual<string>((IEnumerable<string>) source2.OrderBy<string, string>((Func<string, string>) (t => t)));
      })
    };
    private static readonly List<Func<EntityUid, IEntityManager, int>> GetHashCodeList = new List<Func<EntityUid, IEntityManager, int>>()
    {
      (Func<EntityUid, IEntityManager, int>) ((e, entMan) => EqualityComparer<string>.Default.GetHashCode(entMan.GetComponent<MetaDataComponent>(e).EntityPrototype.ID)),
      (Func<EntityUid, IEntityManager, int>) ((e, entMan) =>
      {
        int num = 0;
        foreach (string str in entMan.GetComponent<SpriteComponent>(e).AllLayers.Where<ISpriteLayer>((Func<ISpriteLayer, bool>) (obj => obj.Visible)).Select<ISpriteLayer, string>((Func<ISpriteLayer, string>) (s => s.RsiState.Name)))
          num ^= EqualityComparer<string>.Default.GetHashCode(str);
        return num;
      })
    };
    private readonly int _depth;
    private readonly IEntityManager _entMan;

    private static int Count
    {
      get => EntityMenuUIController.PrototypeAndStatesContextMenuComparer.EqualsList.Count - 1;
    }

    public PrototypeAndStatesContextMenuComparer(int step = 0, IEntityManager? entMan = null)
    {
      IoCManager.Resolve<IEntityManager>(ref entMan);
      this._depth = step > EntityMenuUIController.PrototypeAndStatesContextMenuComparer.Count ? EntityMenuUIController.PrototypeAndStatesContextMenuComparer.Count : step;
      this._entMan = entMan;
    }

    public bool Equals(EntityUid x, EntityUid y)
    {
      if (EntityUid.op_Equality(x, new EntityUid()))
        return EntityUid.op_Equality(y, new EntityUid());
      return EntityUid.op_Inequality(y, new EntityUid()) && EntityMenuUIController.PrototypeAndStatesContextMenuComparer.EqualsList[this._depth](x, y, this._entMan);
    }

    public int GetHashCode(EntityUid e)
    {
      return EntityMenuUIController.PrototypeAndStatesContextMenuComparer.GetHashCodeList[this._depth](e, this._entMan);
    }
  }
}
