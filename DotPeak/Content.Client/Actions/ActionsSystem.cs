// Decompiled with JetBrains decompiler
// Type: Content.Client.Actions.ActionsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Movement;
using Content.Shared._RMC14.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Mapping;
using Content.Shared.Maps;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

#nullable enable
namespace Content.Client.Actions;

public sealed class ActionsSystem : SharedActionsSystem
{
  [Dependency]
  private SharedChargesSystem _sharedCharges;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private IResourceManager _resources;
  [Dependency]
  private MetaDataSystem _metaData;
  private readonly List<EntityUid> _removed = new List<EntityUid>();
  private readonly List<Entity<ActionComponent>> _added = new List<Entity<ActionComponent>>();
  public static readonly EntProtoId MappingEntityAction = EntProtoId.op_Implicit("BaseMappingEntityAction");
  [Dependency]
  private RMCLagCompensationSystem _rmcLagCompensation;

  public event Action<EntityUid>? OnActionAdded;

  public event Action<EntityUid>? OnActionRemoved;

  public event Action? ActionsUpdated;

  public event Action<ActionsComponent>? LinkActions;

  public event Action? UnlinkActions;

  public event Action? ClearAssignments;

  public event Action<List<ActionsSystem.SlotAssignment>>? AssignSlot;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<ActionsComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<ActionsComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsComponent, ComponentHandleState>(new EntityEventRefHandler<ActionsComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<ActionComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnActionAutoHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityTargetActionComponent, ActionTargetAttemptEvent>(new EntityEventRefHandler<EntityTargetActionComponent, ActionTargetAttemptEvent>((object) this, __methodptr(OnEntityTargetAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<WorldTargetActionComponent, ActionTargetAttemptEvent>(new EntityEventRefHandler<WorldTargetActionComponent, ActionTargetAttemptEvent>((object) this, __methodptr(OnWorldTargetAttempt)), (Type[]) null, (Type[]) null);
  }

  private void OnActionAutoHandleState(
    Entity<ActionComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateAction(ent);
  }

  public override void UpdateAction(Entity<ActionComponent> ent)
  {
    ent.Comp.IconColor = this._sharedCharges.GetCurrentCharges(Entity<LimitedChargesComponent, AutoRechargeComponent>.op_Implicit(ent.Owner)) == 0 ? ent.Comp.DisabledIconColor : ent.Comp.OriginalIconColor;
    base.UpdateAction(ent);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid? attachedEntity = ent.Comp.AttachedEntity;
    if ((localEntity.HasValue == attachedEntity.HasValue ? (localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), attachedEntity.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
      return;
    Action actionsUpdated = this.ActionsUpdated;
    if (actionsUpdated == null)
      return;
    actionsUpdated();
  }

  private void OnHandleState(Entity<ActionsComponent> ent, ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is ActionsComponentState current))
      return;
    EntityUid entityUid1;
    ActionsComponent actionsComponent1;
    ent.Deconstruct(ref entityUid1, ref actionsComponent1);
    EntityUid entityUid2 = entityUid1;
    ActionsComponent actionsComponent2 = actionsComponent1;
    this._added.Clear();
    this._removed.Clear();
    HashSet<EntityUid> entityUidSet = this.EnsureEntitySet<ActionsComponent>(current.Actions, entityUid2);
    foreach (EntityUid action in actionsComponent2.Actions)
    {
      if (!entityUidSet.Contains(action) && !this.IsClientSide(action, (MetaDataComponent) null))
        this._removed.Add(action);
    }
    actionsComponent2.Actions.ExceptWith((IEnumerable<EntityUid>) this._removed);
    foreach (EntityUid entityUid3 in entityUidSet)
    {
      if (((EntityUid) ref entityUid3).IsValid() && actionsComponent2.Actions.Add(entityUid3))
      {
        Entity<ActionComponent>? action = this.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(entityUid3)));
        if (action.HasValue)
          this._added.Add(action.GetValueOrDefault());
      }
    }
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid4 = entityUid2;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), entityUid4) ? 1 : 0) : 1) != 0)
      return;
    foreach (EntityUid entityUid5 in this._removed)
    {
      Action<EntityUid> onActionRemoved = this.OnActionRemoved;
      if (onActionRemoved != null)
        onActionRemoved(entityUid5);
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._added.Sort(ActionsSystem.\u003C\u003EO.\u003C0\u003E__ActionComparer ?? (ActionsSystem.\u003C\u003EO.\u003C0\u003E__ActionComparer = new Comparison<Entity<ActionComponent>>(ActionsSystem.ActionComparer)));
    foreach (Entity<ActionComponent> entity in this._added)
    {
      Action<EntityUid> onActionAdded = this.OnActionAdded;
      if (onActionAdded != null)
        onActionAdded(Entity<ActionComponent>.op_Implicit(entity));
    }
    Action actionsUpdated = this.ActionsUpdated;
    if (actionsUpdated == null)
      return;
    actionsUpdated();
  }

  public static int ActionComparer(Entity<ActionComponent> a, Entity<ActionComponent> b)
  {
    ActionComponent comp1 = a.Comp;
    int priority1 = comp1 != null ? comp1.Priority : 0;
    ActionComponent comp2 = b.Comp;
    int priority2 = comp2 != null ? comp2.Priority : 0;
    if (priority1 != priority2)
      return priority1 - priority2;
    ActionComponent comp3 = a.Comp;
    int? nullable1;
    if (comp3 == null)
    {
      nullable1 = new int?();
    }
    else
    {
      ref EntityUid? local = ref comp3.Container;
      nullable1 = local.HasValue ? new int?(local.GetValueOrDefault().Id) : new int?();
    }
    int valueOrDefault1 = nullable1.GetValueOrDefault();
    ActionComponent comp4 = b.Comp;
    int? nullable2;
    if (comp4 == null)
    {
      nullable2 = new int?();
    }
    else
    {
      ref EntityUid? local = ref comp4.Container;
      nullable2 = local.HasValue ? new int?(local.GetValueOrDefault().Id) : new int?();
    }
    int valueOrDefault2 = nullable2.GetValueOrDefault();
    return valueOrDefault1 - valueOrDefault2;
  }

  protected override void ActionAdded(
    Entity<ActionsComponent> performer,
    Entity<ActionComponent> action)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid owner = performer.Owner;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), owner) ? 1 : 0) : 1) != 0)
      return;
    Action<EntityUid> onActionAdded = this.OnActionAdded;
    if (onActionAdded != null)
      onActionAdded(Entity<ActionComponent>.op_Implicit(action));
    Action actionsUpdated = this.ActionsUpdated;
    if (actionsUpdated == null)
      return;
    actionsUpdated();
  }

  protected override void ActionRemoved(
    Entity<ActionsComponent> performer,
    Entity<ActionComponent> action)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid owner = performer.Owner;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), owner) ? 1 : 0) : 1) != 0)
      return;
    Action<EntityUid> onActionRemoved = this.OnActionRemoved;
    if (onActionRemoved != null)
      onActionRemoved(Entity<ActionComponent>.op_Implicit(action));
    Action actionsUpdated = this.ActionsUpdated;
    if (actionsUpdated == null)
      return;
    actionsUpdated();
  }

  public IEnumerable<Entity<ActionComponent>> GetClientActions()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    return localEntity.HasValue ? this.GetActions(localEntity.GetValueOrDefault()) : Enumerable.Empty<Entity<ActionComponent>>();
  }

  private void OnPlayerAttached(
    EntityUid uid,
    ActionsComponent component,
    LocalPlayerAttachedEvent args)
  {
    this.LinkAllActions(component);
  }

  private void OnPlayerDetached(
    EntityUid uid,
    ActionsComponent component,
    LocalPlayerDetachedEvent? args = null)
  {
    this.UnlinkAllActions();
  }

  public void UnlinkAllActions()
  {
    Action unlinkActions = this.UnlinkActions;
    if (unlinkActions == null)
      return;
    unlinkActions();
  }

  public void LinkAllActions(ActionsComponent? actions = null)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue || !this.Resolve<ActionsComponent>(localEntity.GetValueOrDefault(), ref actions, false))
      return;
    Action<ActionsComponent> linkActions = this.LinkActions;
    if (linkActions == null)
      return;
    linkActions(actions);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<ActionsSystem>();
  }

  public void TriggerAction(Entity<ActionComponent> action)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    if (!this.HasComp<InstantActionComponent>(Entity<ActionComponent>.op_Implicit(action)))
      return;
    if (action.Comp.ClientExclusive)
      this.PerformAction(Entity<ActionsComponent>.op_Implicit(valueOrDefault), action);
    else
      this.RaisePredictiveEvent<RequestPerformActionEvent>(new RequestPerformActionEvent(this.GetNetEntity(Entity<ActionComponent>.op_Implicit(action), (MetaDataComponent) null), this._rmcLagCompensation.GetLastRealTick(new NetUserId?())));
  }

  public void LoadActionAssignments(string path, bool userData)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault1 = localEntity.GetValueOrDefault();
    ResPath resPath = new ResPath(path);
    ResPath rootedPath = ((ResPath) ref resPath).ToRootedPath();
    TextReader textReader = userData ? (TextReader) WritableDirProviderExt.OpenText(this._resources.UserData, rootedPath) : (TextReader) this._resources.ContentFileReadText(rootedPath);
    YamlStream yamlStream = new YamlStream();
    yamlStream.Load(textReader);
    if (!(YamlNodeHelpers.ToDataNode(yamlStream.Documents[0].RootNode) is SequenceDataNode dataNode1))
      return;
    ActionsComponent actionsComponent = this.EnsureComp<ActionsComponent>(valueOrDefault1);
    Action clearAssignments = this.ClearAssignments;
    if (clearAssignments != null)
      clearAssignments();
    List<ActionsSystem.SlotAssignment> slotAssignmentList = new List<ActionsSystem.SlotAssignment>();
    foreach (DataNode dataNode2 in (IEnumerable<DataNode>) dataNode1.Sequence)
    {
      DataNode dataNode3;
      if (dataNode2 is MappingDataNode mappingDataNode && mappingDataNode.TryGet("assignments", ref dataNode3))
      {
        EntityUid invalid = EntityUid.Invalid;
        ValueDataNode valueDataNode1;
        EntityUid uid;
        if (mappingDataNode.TryGet<ValueDataNode>("action", ref valueDataNode1))
        {
          EntProtoId entProtoId;
          // ISSUE: explicit constructor call
          ((EntProtoId) ref entProtoId).\u002Ector(valueDataNode1.Value);
          uid = this.Spawn(EntProtoId.op_Implicit(entProtoId), (ComponentRegistry) null, true);
        }
        else
        {
          ValueDataNode valueDataNode2;
          if (mappingDataNode.TryGet<ValueDataNode>("entity", ref valueDataNode2))
          {
            EntProtoId entProtoId;
            // ISSUE: explicit constructor call
            ((EntProtoId) ref entProtoId).\u002Ector(valueDataNode2.Value);
            EntityPrototype entityPrototype = this._proto.Index(entProtoId);
            uid = this.Spawn(EntProtoId.op_Implicit(ActionsSystem.MappingEntityAction), (ComponentRegistry) null, true);
            this.SetIcon(Entity<ActionComponent>.op_Implicit(uid), (SpriteSpecifier) new SpriteSpecifier.EntityPrototype(EntProtoId.op_Implicit(entProtoId)));
            this.SetEvent(uid, (BaseActionEvent) new StartPlacementActionEvent()
            {
              PlacementOption = "SnapgridCenter",
              EntityType = new EntProtoId?(entProtoId)
            });
            this._metaData.SetEntityName(uid, entityPrototype.Name, (MetaDataComponent) null, true);
          }
          else
          {
            ValueDataNode valueDataNode3;
            if (mappingDataNode.TryGet<ValueDataNode>("tileId", ref valueDataNode3))
            {
              ProtoId<ContentTileDefinition> protoId = new ProtoId<ContentTileDefinition>(valueDataNode3.Value);
              ContentTileDefinition contentTileDefinition = this._proto.Index<ContentTileDefinition>(protoId);
              uid = this.Spawn(EntProtoId.op_Implicit(ActionsSystem.MappingEntityAction), (ComponentRegistry) null, true);
              ResPath? sprite = contentTileDefinition.Sprite;
              if (sprite.HasValue)
              {
                ResPath valueOrDefault2 = sprite.GetValueOrDefault();
                this.SetIcon(Entity<ActionComponent>.op_Implicit(uid), (SpriteSpecifier) new SpriteSpecifier.Texture(valueOrDefault2));
              }
              this.SetEvent(uid, (BaseActionEvent) new StartPlacementActionEvent()
              {
                PlacementOption = "AlignTileAny",
                TileId = new ProtoId<ContentTileDefinition>?(protoId)
              });
              this._metaData.SetEntityName(uid, this.Loc.GetString(contentTileDefinition.Name), (MetaDataComponent) null, true);
            }
            else
            {
              this.Log.Error($"Mapping actions from {path} had unknown action data!");
              continue;
            }
          }
        }
        this.AddActionDirect(Entity<ActionsComponent>.op_Implicit((valueOrDefault1, actionsComponent)), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(uid)));
      }
    }
  }

  private void OnWorldTargetAttempt(
    Entity<WorldTargetActionComponent> ent,
    ref ActionTargetAttemptEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    EntityUid entityUid1;
    WorldTargetActionComponent targetActionComponent1;
    ent.Deconstruct(ref entityUid1, ref targetActionComponent1);
    EntityUid entityUid2 = entityUid1;
    WorldTargetActionComponent targetActionComponent2 = targetActionComponent1;
    ActionComponent action = args.Action;
    EntityCoordinates coordinates = args.Input.Coordinates;
    Entity<ActionsComponent> user = args.User;
    if (!this.ValidateWorldTarget(Entity<ActionsComponent>.op_Implicit(user), coordinates, ent))
      return;
    EntityUid? nullable = new EntityUid?();
    EntityUid entityUid3 = args.Input.EntityUid;
    EntityTargetActionComponent targetActionComponent3;
    if (this.TryComp<EntityTargetActionComponent>(Entity<WorldTargetActionComponent>.op_Implicit(ent), ref targetActionComponent3) && ((EntityUid) ref entityUid3).Valid && this.ValidateEntityTarget(Entity<ActionsComponent>.op_Implicit(user), entityUid3, Entity<EntityTargetActionComponent>.op_Implicit((entityUid2, targetActionComponent3))))
      nullable = new EntityUid?(entityUid3);
    if (action.ClientExclusive)
    {
      WorldTargetActionEvent targetActionEvent = targetActionComponent2.Event;
      if (targetActionEvent != null)
      {
        targetActionEvent.Target = coordinates;
        targetActionEvent.Entity = nullable;
      }
      this.PerformAction(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(user), user.Comp)), Entity<ActionComponent>.op_Implicit((entityUid2, action)));
    }
    else
      this.RaisePredictiveEvent<RequestPerformActionEvent>(new RequestPerformActionEvent(this.GetNetEntity(entityUid2, (MetaDataComponent) null), this.GetNetEntity(nullable, (MetaDataComponent) null), this.GetNetCoordinates(coordinates, (MetaDataComponent) null), this._rmcLagCompensation.GetLastRealTick(new NetUserId?())));
    args.FoundTarget = true;
  }

  private void OnEntityTargetAttempt(
    Entity<EntityTargetActionComponent> ent,
    ref ActionTargetAttemptEvent args)
  {
    if (args.Handled)
      return;
    EntityUid entityUid1 = args.Input.EntityUid;
    if (!((EntityUid) ref entityUid1).Valid)
    {
      this.RaisePredictiveEvent<RMCMissedTargetActionEvent>(new RMCMissedTargetActionEvent(this.GetNetEntity(Entity<EntityTargetActionComponent>.op_Implicit(ent), (MetaDataComponent) null)));
    }
    else
    {
      EntityUid entityUid2;
      EntityTargetActionComponent targetActionComponent;
      ent.Deconstruct(ref entityUid2, ref targetActionComponent);
      EntityUid entityUid3 = entityUid2;
      EntityTargetActionEvent targetActionEvent = targetActionComponent.Event;
      if (targetActionEvent == null)
        return;
      args.Handled = true;
      ActionComponent action = args.Action;
      Entity<ActionsComponent> user = args.User;
      if (!this.ValidateEntityTarget(Entity<ActionsComponent>.op_Implicit(user), entityUid1, ent))
        return;
      if (action.ClientExclusive)
      {
        targetActionEvent.Target = entityUid1;
        this.PerformAction(Entity<ActionsComponent>.op_Implicit((Entity<ActionsComponent>.op_Implicit(user), user.Comp)), Entity<ActionComponent>.op_Implicit((entityUid3, action)));
      }
      else
        this.RaisePredictiveEvent<RequestPerformActionEvent>(new RequestPerformActionEvent(this.GetNetEntity(entityUid3, (MetaDataComponent) null), this.GetNetEntity(entityUid1, (MetaDataComponent) null), this._rmcLagCompensation.GetLastRealTick(new NetUserId?())));
      args.FoundTarget = true;
    }
  }

  public void SetAssignments(List<ActionsSystem.SlotAssignment> actions)
  {
    Action clearAssignments = this.ClearAssignments;
    if (clearAssignments != null)
      clearAssignments();
    Action<List<ActionsSystem.SlotAssignment>> assignSlot = this.AssignSlot;
    if (assignSlot == null)
      return;
    assignSlot(actions);
  }

  public delegate void OnActionReplaced(EntityUid actionId);

  public record struct SlotAssignment(byte Hotbar, byte Slot, EntityUid ActionId);
}
