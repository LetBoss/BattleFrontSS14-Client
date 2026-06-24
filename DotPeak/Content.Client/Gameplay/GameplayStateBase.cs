// Decompiled with JetBrains decompiler
// Type: Content.Client.Gameplay.GameplayStateBase
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Interaction;
using Content.Client.Clickable;
using Content.Client.Sprite;
using Content.Client.UserInterface;
using Content.Client.Viewport;
using Content.Shared.Input;
using Robust.Client.ComponentTrees;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Analyzers;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Gameplay;

[Virtual]
public class GameplayStateBase : Robust.Client.State.State, IEntityEventSubscriber
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IEntitySystemManager _entitySystemManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  protected IUserInterfaceManager UserInterfaceManager;
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IViewVariablesManager _vvm;
  [Dependency]
  private IConsoleHost _conHost;
  private GameplayStateBase.ClickableEntityComparer _comparer;

  private (ViewVariablesPath? path, string[] segments) ResolveVvHoverObject(string path)
  {
    string[] strArray = path.Split('/');
    NetEntity? netEntity = this._entityManager.GetNetEntity(this.RecursivelyFindUiEntity(this.UserInterfaceManager.CurrentlyHovered), (MetaDataComponent) null);
    return (netEntity.HasValue ? (ViewVariablesPath) new ViewVariablesInstancePath((object) netEntity) : (ViewVariablesPath) null, strArray);
  }

  private EntityUid? RecursivelyFindUiEntity(Control? control)
  {
    switch (control)
    {
      case null:
        return new EntityUid?();
      case IViewportControl iviewportControl:
        ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
        return ((ScreenCoordinates) ref mouseScreenPosition).IsValid ? this.GetClickedEntity(iviewportControl.PixelToMap(this._inputManager.MouseScreenPosition.Position)) : new EntityUid?();
      case SpriteView spriteView:
        Entity<SpriteComponent, TransformComponent>? entity = spriteView.Entity;
        return !entity.HasValue ? new EntityUid?() : new EntityUid?(Entity<SpriteComponent, TransformComponent>.op_Implicit(entity.GetValueOrDefault()));
      case IEntityControl entityControl:
        return entityControl.UiEntity;
      default:
        return this.RecursivelyFindUiEntity(control.Parent);
    }
  }

  private IEnumerable<string>? ListVVHoverPaths(string[] segments) => (IEnumerable<string>) null;

  protected virtual void Startup()
  {
    // ISSUE: method pointer
    // ISSUE: method pointer
    this._vvm.RegisterDomain("enthover", new DomainResolveObject((object) this, __methodptr(ResolveVvHoverObject)), new DomainListPaths((object) this, __methodptr(ListVVHoverPaths)));
    this._inputManager.KeyBindStateChanged += new Action<ViewportBoundKeyEventArgs>(this.OnKeyBindStateChanged);
    this._comparer = new GameplayStateBase.ClickableEntityComparer();
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.InspectEntity, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate((object) this, __methodptr(HandleInspect)), true, true)).Register<GameplayStateBase>();
  }

  protected virtual void Shutdown()
  {
    this._vvm.UnregisterDomain("enthover");
    this._inputManager.KeyBindStateChanged -= new Action<ViewportBoundKeyEventArgs>(this.OnKeyBindStateChanged);
    CommandBinds.Unregister<GameplayStateBase>();
  }

  private bool HandleInspect(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    this._conHost.ExecuteCommand("vv /c/enthover");
    return true;
  }

  public EntityUid? GetClickedEntity(MapCoordinates coordinates)
  {
    return this.GetClickedEntity(coordinates, this._eyeManager.CurrentEye);
  }

  public EntityUid? GetClickedEntity(MapCoordinates coordinates, IEye? eye)
  {
    if (eye == null)
      return new EntityUid?();
    EntityUid entityUid = this.GetClickableEntities(coordinates, eye).FirstOrDefault<EntityUid>();
    return !((EntityUid) ref entityUid).IsValid() ? new EntityUid?() : new EntityUid?(entityUid);
  }

  public IEnumerable<EntityUid> GetClickableEntities(
    EntityCoordinates coordinates,
    bool excludeFaded = true)
  {
    return this.GetClickableEntities(this._entitySystemManager.GetEntitySystem<SharedTransformSystem>().ToMapCoordinates(coordinates, true), excludeFaded);
  }

  public IEnumerable<EntityUid> GetClickableEntities(MapCoordinates coordinates, bool excludeFaded = true)
  {
    return this.GetClickableEntities(coordinates, this._eyeManager.CurrentEye, excludeFaded);
  }

  public IEnumerable<EntityUid> GetClickableEntities(
    MapCoordinates coordinates,
    IEye? eye,
    bool excludeFaded = true,
    bool ignoreInteractionTransparency = false)
  {
    if (eye == null)
      return (IEnumerable<EntityUid>) Array.Empty<EntityUid>();
    HashSet<ComponentTreeEntry<SpriteComponent>> componentTreeEntrySet = ((ComponentTreeSystem<SpriteTreeComponent, SpriteComponent>) this._entityManager.EntitySysManager.GetEntitySystem<SpriteTreeSystem>()).QueryAabb(coordinates.MapId, Box2.CenteredAround(coordinates.Position, new Vector2(1f, 1f)), true);
    List<(EntityUid, int, uint, float)> source = new List<(EntityUid, int, uint, float)>(componentTreeEntrySet.Count);
    EntityQuery<ClickableComponent> entityQuery = this._entityManager.GetEntityQuery<ClickableComponent>();
    ClickableSystem clickableSystem = this._entityManager.System<ClickableSystem>();
    foreach (ComponentTreeEntry<SpriteComponent> componentTreeEntry in componentTreeEntrySet)
    {
      ClickableComponent clickableComponent;
      int drawDepth;
      uint renderOrder;
      float bottom;
      if ((ignoreInteractionTransparency || !this._entitySystemManager.GetEntitySystem<RMCClientInteractionSystem>().IsInteractionTransparency(componentTreeEntry.Uid, ((ISharedPlayerManager) this._playerManager).LocalEntity, eye)) && entityQuery.TryGetComponent(componentTreeEntry.Uid, ref clickableComponent) && clickableSystem.CheckClick(Entity<ClickableComponent, SpriteComponent, TransformComponent, FadingSpriteComponent>.op_Implicit((componentTreeEntry.Uid, clickableComponent, componentTreeEntry.Component, componentTreeEntry.Transform)), coordinates.Position, eye, excludeFaded, out drawDepth, out renderOrder, out bottom))
        source.Add((componentTreeEntry.Uid, drawDepth, renderOrder, bottom));
    }
    if (source.Count == 0)
      return (IEnumerable<EntityUid>) Array.Empty<EntityUid>();
    source.Sort((IComparer<(EntityUid, int, uint, float)>) this._comparer);
    return source.Select<(EntityUid, int, uint, float), EntityUid>((Func<(EntityUid, int, uint, float), EntityUid>) (a => a.Item1));
  }

  protected virtual void OnKeyBindStateChanged(ViewportBoundKeyEventArgs args)
  {
    InputSystem inputSystem;
    if (!this._entitySystemManager.TryGetEntitySystem<InputSystem>(ref inputSystem))
      return;
    BoundKeyEventArgs keyEventArgs = args.KeyEventArgs;
    BoundKeyFunction function = keyEventArgs.Function;
    KeyFunctionId keyFunctionId = this._inputManager.NetworkBindMap.KeyFunctionID(function);
    EntityCoordinates entityCoordinates1 = new EntityCoordinates();
    EntityUid? nullable = new EntityUid?();
    EntityCoordinates entityCoordinates2;
    if (args.Viewport is IViewportControl viewport)
    {
      ScreenCoordinates pointerLocation = keyEventArgs.PointerLocation;
      if (((ScreenCoordinates) ref pointerLocation).IsValid)
      {
        MapCoordinates map = viewport.PixelToMap(keyEventArgs.PointerLocation.Position);
        nullable = !(viewport is ScalingViewport scalingViewport) ? this.GetClickedEntity(map) : this.GetClickedEntity(map, scalingViewport.Eye);
        SharedTransformSystem entitySystem1 = this._entitySystemManager.GetEntitySystem<SharedTransformSystem>();
        MapSystem entitySystem2 = this._entitySystemManager.GetEntitySystem<MapSystem>();
        EntityUid entityUid;
        MapGridComponent mapGridComponent;
        entityCoordinates2 = !((SharedMapSystem) entitySystem2).MapExists(new MapId?(map.MapId)) ? EntityCoordinates.Invalid : (this._mapManager.TryFindGridAt(map, ref entityUid, ref mapGridComponent) ? ((SharedMapSystem) entitySystem2).MapToGrid(entityUid, map) : entitySystem1.ToCoordinates(map));
        goto label_6;
      }
    }
    entityCoordinates2 = EntityCoordinates.Invalid;
label_6:
    ClientFullInputCmdMessage fullInputCmdMessage = new ClientFullInputCmdMessage(this._timing.CurTick, this._timing.TickFraction, keyFunctionId)
    {
      State = keyEventArgs.State,
      Coordinates = entityCoordinates2,
      ScreenCoordinates = keyEventArgs.PointerLocation,
      Uid = nullable.GetValueOrDefault()
    };
    ICommonSession localSession = ((ISharedPlayerManager) this._playerManager).LocalSession;
    if (!inputSystem.HandleInputCommand(localSession, function, (IFullInputCmdMessage) fullInputCmdMessage, false))
      return;
    keyEventArgs.Handle();
  }

  private sealed class ClickableEntityComparer : 
    IComparer<(EntityUid clicked, int depth, uint renderOrder, float bottom)>
  {
    public int Compare(
      (EntityUid clicked, int depth, uint renderOrder, float bottom) x,
      (EntityUid clicked, int depth, uint renderOrder, float bottom) y)
    {
      int num1 = y.depth.CompareTo(x.depth);
      if (num1 != 0)
        return num1;
      int num2 = y.renderOrder.CompareTo(x.renderOrder);
      if (num2 != 0)
        return num2;
      int num3 = -y.bottom.CompareTo(x.bottom);
      // ISSUE: explicit reference operation
      return num3 != 0 ? num3 : ((EntityUid) @y.clicked).CompareTo(x.clicked);
    }
  }
}
