// Decompiled with JetBrains decompiler
// Type: Content.Client.Construction.ConstructionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Popups;
using Content.Shared._RMC14.Construction;
using Content.Shared.Construction;
using Content.Shared.Construction.Conditions;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Construction.Steps;
using Content.Shared.Examine;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Wall;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Client.Construction;

public sealed class ConstructionSystem : SharedConstructionSystem
{
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private ExamineSystemShared _examineSystem;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private PopupSystem _popupSystem;
  [Dependency]
  private RMCConstructionSystem _rmcConstruction;
  private readonly Dictionary<int, EntityUid> _ghosts = new Dictionary<int, EntityUid>();
  private readonly Dictionary<string, ConstructionGuide> _guideCache = new Dictionary<string, ConstructionGuide>();
  private readonly Dictionary<string, string> _recipesMetadataCache = new Dictionary<string, string>();

  public bool CraftingEnabled { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.WarmupRecipesCache();
    this.UpdatesOutsidePrediction = true;
    this.SubscribeLocalEvent<LocalPlayerAttachedEvent>(new EntityEventHandler<LocalPlayerAttachedEvent>(this.HandlePlayerAttached), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<AckStructureConstructionMessage>(new EntityEventHandler<AckStructureConstructionMessage>(this.HandleAckStructure), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<ResponseConstructionGuide>(new EntityEventHandler<ResponseConstructionGuide>(this.OnConstructionGuideReceived), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.OpenCraftingMenu, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleOpenCraftingMenu)), true, true)).Bind(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleUse)), true, true)).Bind(ContentKeyFunctions.EditorFlipObject, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleFlip)), true, true)).Register<ConstructionSystem>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ConstructionGhostComponent, ExaminedEvent>(new ComponentEventHandler<ConstructionGhostComponent, ExaminedEvent>((object) this, __methodptr(HandleConstructionGhostExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ConstructionGhostComponent, ComponentShutdown>(new ComponentEventHandler<ConstructionGhostComponent, ComponentShutdown>((object) this, __methodptr(HandleGhostComponentShutdown)), (Type[]) null, (Type[]) null);
  }

  private void HandleGhostComponentShutdown(
    EntityUid uid,
    ConstructionGhostComponent component,
    ComponentShutdown args)
  {
    this.ClearGhost(component.GhostId);
  }

  public bool TryGetRecipePrototype(string constructionProtoId, [NotNullWhen(true)] out string? targetProtoId)
  {
    if (this._recipesMetadataCache.TryGetValue(constructionProtoId, out targetProtoId))
      return true;
    targetProtoId = (string) null;
    return false;
  }

  private void WarmupRecipesCache()
  {
    foreach (ConstructionPrototype enumeratePrototype in this.PrototypeManager.EnumeratePrototypes<ConstructionPrototype>())
    {
      ConstructionGraphPrototype constructionGraphPrototype;
      if (this.PrototypeManager.TryIndex<ConstructionGraphPrototype>(enumeratePrototype.Graph, ref constructionGraphPrototype))
      {
        string targetNode = enumeratePrototype.TargetNode;
        ConstructionGraphNode constructionGraphNode1;
        if (targetNode != null && constructionGraphPrototype.Nodes.TryGetValue(targetNode, out constructionGraphNode1))
        {
          Stack<ConstructionGraphNode> constructionGraphNodeStack = new Stack<ConstructionGraphNode>();
          constructionGraphNodeStack.Push(constructionGraphNode1);
          do
          {
            ConstructionGraphNode constructionGraphNode2 = constructionGraphNodeStack.Pop();
            string id = constructionGraphNode2.Entity.GetId(new EntityUid?(), new EntityUid?(), new GraphNodeEntityArgs((IEntityManager) this.EntityManager));
            if (id == null)
            {
              if (constructionGraphNodeStack.Count == 0)
              {
                foreach (ConstructionGraphEdge edge in (IEnumerable<ConstructionGraphEdge>) constructionGraphNode2.Edges)
                {
                  ConstructionGraphNode constructionGraphNode3;
                  if (constructionGraphPrototype.Nodes.TryGetValue(edge.Target, out constructionGraphNode3))
                    constructionGraphNodeStack.Push(constructionGraphNode3);
                }
              }
            }
            else
            {
              constructionGraphNodeStack.Clear();
              ConstructionPrototype constructionPrototype;
              EntityPrototype entityPrototype;
              if (this.PrototypeManager.TryIndex<ConstructionPrototype>(enumeratePrototype.ID, ref constructionPrototype) && this.PrototypeManager.TryIndex(EntProtoId.op_Implicit(id), ref entityPrototype))
              {
                string str1;
                string str2 = constructionPrototype.SetName != null ? (this.Loc.TryGetString(constructionPrototype.SetName, ref str1) ? str1 : constructionPrototype.SetName) : entityPrototype.Name;
                string str3;
                string str4 = constructionPrototype.SetDescription != null ? (this.Loc.TryGetString(constructionPrototype.SetDescription, ref str3) ? str3 : constructionPrototype.SetDescription) : entityPrototype.Description;
                constructionPrototype.Name = str2;
                constructionPrototype.Description = str4;
                this._recipesMetadataCache.Add(enumeratePrototype.ID, id);
              }
            }
          }
          while (constructionGraphNodeStack.Count > 0);
        }
      }
    }
  }

  private void OnConstructionGuideReceived(ResponseConstructionGuide ev)
  {
    this._guideCache[ev.ConstructionId] = ev.Guide;
    EventHandler<string> constructionGuideAvailable = this.ConstructionGuideAvailable;
    if (constructionGuideAvailable == null)
      return;
    constructionGuideAvailable((object) this, ev.ConstructionId);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<ConstructionSystem>();
  }

  public ConstructionGuide? GetGuide(ConstructionPrototype prototype)
  {
    ConstructionGuide guide;
    if (this._guideCache.TryGetValue(prototype.ID, out guide))
      return guide;
    this.RaiseNetworkEvent((EntityEventArgs) new RequestConstructionGuide(prototype.ID));
    return (ConstructionGuide) null;
  }

  private void HandleConstructionGhostExamined(
    EntityUid uid,
    ConstructionGhostComponent component,
    ExaminedEvent args)
  {
    if (component.Prototype?.Name == null)
      return;
    using (args.PushGroup("ConstructionGhostComponent"))
    {
      args.PushMarkup(this.Loc.GetString("construction-ghost-examine-message", ("name", (object) component.Prototype.Name)));
      ConstructionGraphPrototype constructionGraphPrototype;
      if (!this.PrototypeManager.TryIndex<ConstructionGraphPrototype>(component.Prototype.Graph, ref constructionGraphPrototype))
        return;
      ConstructionGraphNode node = constructionGraphPrototype.Nodes[component.Prototype.StartNode];
      ConstructionGraphNode[] path;
      ConstructionGraphEdge edge;
      if (!constructionGraphPrototype.TryPath(component.Prototype.StartNode, component.Prototype.TargetNode, out path) || !node.TryGetEdge(path[0].Name, out edge))
        return;
      foreach (ConstructionGraphStep step in (IEnumerable<ConstructionGraphStep>) edge.Steps)
        step.DoExamine(args);
    }
  }

  public event EventHandler<CraftingAvailabilityChangedArgs>? CraftingAvailabilityChanged;

  public event EventHandler<string>? ConstructionGuideAvailable;

  public event EventHandler? ToggleCraftingWindow;

  public event EventHandler? FlipConstructionPrototype;

  private void HandleAckStructure(AckStructureConstructionMessage msg)
  {
    this.ClearGhost(msg.GhostId);
  }

  private void HandlePlayerAttached(LocalPlayerAttachedEvent msg)
  {
    bool available = ConstructionSystem.IsCraftingAvailable(new EntityUid?(msg.Entity));
    if (!this._rmcConstruction.CanConstruct(new EntityUid?(msg.Entity)))
      available = false;
    this.UpdateCraftingAvailability(available);
  }

  private bool HandleOpenCraftingMenu(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State == 1)
    {
      EventHandler toggleCraftingWindow = this.ToggleCraftingWindow;
      if (toggleCraftingWindow != null)
        toggleCraftingWindow((object) this, EventArgs.Empty);
    }
    return true;
  }

  private bool HandleFlip(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (args.State == 1)
    {
      EventHandler constructionPrototype = this.FlipConstructionPrototype;
      if (constructionPrototype != null)
        constructionPrototype((object) this, EventArgs.Empty);
    }
    return true;
  }

  private void UpdateCraftingAvailability(bool available)
  {
    if (this.CraftingEnabled == available)
      return;
    EventHandler<CraftingAvailabilityChangedArgs> availabilityChanged = this.CraftingAvailabilityChanged;
    if (availabilityChanged != null)
      availabilityChanged((object) this, new CraftingAvailabilityChangedArgs(available));
    this.CraftingEnabled = available;
  }

  private static bool IsCraftingAvailable(EntityUid? entity) => entity.HasValue;

  private bool HandleUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!((EntityUid) ref args.EntityUid).IsValid() || !this.IsClientSide(args.EntityUid, (MetaDataComponent) null) || !this.HasComp<ConstructionGhostComponent>(args.EntityUid))
      return false;
    this.TryStartConstruction(args.EntityUid);
    return true;
  }

  public void SpawnGhost(ConstructionPrototype prototype, EntityCoordinates loc, Direction dir)
  {
    this.TrySpawnGhost(prototype, loc, dir, out EntityUid? _);
  }

  public bool TrySpawnGhost(
    ConstructionPrototype prototype,
    EntityCoordinates loc,
    Direction dir,
    [NotNullWhen(true)] out EntityUid? ghost)
  {
    ghost = new EntityUid?();
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (localEntity.HasValue)
    {
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      string targetProtoId;
      EntityPrototype entityPrototype;
      if (((EntityUid) ref valueOrDefault).IsValid() && this.TryGetRecipePrototype(prototype.ID, out targetProtoId) && this.PrototypeManager.TryIndex<EntityPrototype>(targetProtoId, ref entityPrototype) && !this.GhostPresent(loc))
      {
        SharedInteractionSystem.Ignored predicate = this.GetPredicate(prototype.CanBuildInImpassable, this._transformSystem.ToMapCoordinates(loc, true));
        if (!this._examineSystem.InRangeUnOccluded(valueOrDefault, loc, 20f, predicate) || !this.CheckConstructionConditions(prototype, loc, dir, valueOrDefault, true))
          return false;
        ghost = new EntityUid?(this.Spawn("constructionghost", loc));
        ConstructionGhostComponent constructionGhostComponent = this.Comp<ConstructionGhostComponent>(ghost.Value);
        constructionGhostComponent.Prototype = prototype;
        constructionGhostComponent.GhostId = ghost.GetHashCode();
        this.Comp<TransformComponent>(ghost.Value).LocalRotation = DirectionExtensions.ToAngle(dir);
        this._ghosts.Add(constructionGhostComponent.GhostId, ghost.Value);
        SpriteComponent spriteComponent1 = this.Comp<SpriteComponent>(ghost.Value);
        this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((ghost.Value, spriteComponent1)), new Color((byte) 48 /*0x30*/, byte.MaxValue, (byte) 48 /*0x30*/, (byte) 128 /*0x80*/));
        IconComponent iconComponent;
        if (entityPrototype.TryGetComponent<IconComponent>(ref iconComponent, this.EntityManager.ComponentFactory))
        {
          this._sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((ghost.Value, spriteComponent1)), new int?(0));
          this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ghost.Value, spriteComponent1)), 0, (SpriteSpecifier) iconComponent.Icon);
          spriteComponent1.LayerSetShader(0, "unshaded");
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ghost.Value, spriteComponent1)), 0, true);
        }
        else
        {
          if (!((Dictionary<string, EntityPrototype.ComponentRegistryEntry>) entityPrototype.Components).TryGetValue("Sprite", out EntityPrototype.ComponentRegistryEntry _))
            return false;
          EntityUid entityUid = this.EntityManager.SpawnEntity(targetProtoId, MapCoordinates.Nullspace, (ComponentRegistry) null);
          SpriteComponent spriteComponent2 = this.EnsureComp<SpriteComponent>(entityUid);
          this.EntityManager.System<AppearanceSystem>().OnChangeData(entityUid, spriteComponent2, (AppearanceComponent) null);
          for (int index = 0; index < spriteComponent2.AllLayers.Count<ISpriteLayer>(); ++index)
          {
            if (spriteComponent2[index].Visible)
            {
              RSI.StateId rsiState = spriteComponent2[index].RsiState;
              if (((RSI.StateId) ref rsiState).IsValid)
              {
                RSI rsi = spriteComponent2[index].Rsi ?? spriteComponent2.BaseRSI;
                RSI.State state;
                if (rsi != null && rsi.TryGetState(spriteComponent2[index].RsiState, ref state) && state.StateId.Name != null)
                {
                  this._sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((ghost.Value, spriteComponent1)), new int?(index));
                  this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((ghost.Value, spriteComponent1)), index, (SpriteSpecifier) new SpriteSpecifier.Rsi(rsi.Path, state.StateId.Name));
                  spriteComponent1.LayerSetShader(index, "unshaded");
                  this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ghost.Value, spriteComponent1)), index, true);
                }
              }
            }
          }
          this.Del(new EntityUid?(entityUid));
        }
        if (prototype.CanBuildInImpassable)
          this.EnsureComp<WallMountComponent>(ghost.Value).Arc = new Angle(2.0 * Math.PI);
        return true;
      }
    }
    return false;
  }

  private bool CheckConstructionConditions(
    ConstructionPrototype prototype,
    EntityCoordinates loc,
    Direction dir,
    EntityUid user,
    bool showPopup = false)
  {
    RMCConstructionAttemptEvent constructionAttemptEvent = new RMCConstructionAttemptEvent(loc, prototype.Name);
    this.RaiseLocalEvent<RMCConstructionAttemptEvent>(ref constructionAttemptEvent);
    if (constructionAttemptEvent.Cancelled)
    {
      string popup = constructionAttemptEvent.Popup;
      if (popup != null)
        this._popupSystem.PopupCoordinates(popup, loc, PopupType.Small);
      return false;
    }
    foreach (IConstructionCondition condition in (IEnumerable<IConstructionCondition>) prototype.Conditions)
    {
      if (!condition.Condition(user, loc, dir))
      {
        if (showPopup)
        {
          string localization = condition.GenerateGuideEntry()?.Localization;
          if (localization != null)
            this._popupSystem.PopupCoordinates(this.Loc.GetString(localization), loc, PopupType.Small);
        }
        return false;
      }
    }
    return true;
  }

  private bool GhostPresent(EntityCoordinates loc)
  {
    foreach (KeyValuePair<int, EntityUid> ghost in this._ghosts)
    {
      EntityCoordinates coordinates = this.Comp<TransformComponent>(ghost.Value).Coordinates;
      if (((EntityCoordinates) ref coordinates).Equals(loc))
        return true;
    }
    return false;
  }

  public void TryStartConstruction(EntityUid ghostId, ConstructionGhostComponent? ghostComp = null)
  {
    if (!this.Resolve<ConstructionGhostComponent>(ghostId, ref ghostComp, true))
      return;
    if (ghostComp.Prototype == null)
      throw new ArgumentException($"Can't start construction for a ghost with no prototype. Ghost id: {ghostId}");
    TransformComponent transformComponent = this.Comp<TransformComponent>(ghostId);
    this.RaiseNetworkEvent((EntityEventArgs) new TryStartStructureConstructionMessage(this.GetNetCoordinates(transformComponent.Coordinates, (MetaDataComponent) null), ghostComp.Prototype.ID, transformComponent.LocalRotation, ghostId.GetHashCode()));
  }

  public void TryStartItemConstruction(string prototypeName)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new TryStartItemConstructionMessage(prototypeName));
  }

  public void ClearGhost(int ghostId)
  {
    EntityUid entityUid;
    if (!this._ghosts.TryGetValue(ghostId, out entityUid))
      return;
    this.QueueDel(new EntityUid?(entityUid));
    this._ghosts.Remove(ghostId);
  }

  public void ClearAllGhosts()
  {
    foreach (EntityUid entityUid in this._ghosts.Values)
      this.QueueDel(new EntityUid?(entityUid));
    this._ghosts.Clear();
  }
}
