// Decompiled with JetBrains decompiler
// Type: Content.Client.Verbs.VerbSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Examine;
using Content.Client.Gameplay;
using Content.Client.Popups;
using Content.Shared.CCVar;
using Content.Shared.Examine;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.LandMines;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Client.ComponentTrees;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Client.Verbs;

public sealed class VerbSystem : SharedVerbSystem
{
  [Dependency]
  private PopupSystem _popupSystem;
  [Dependency]
  private ExamineSystem _examine;
  [Dependency]
  private SpriteTreeSystem _tree;
  [Dependency]
  private TagSystem _tagSystem;
  [Dependency]
  private IStateManager _stateManager;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private EntityLookupSystem _lookup;
  private float _lookupSize;
  private static readonly ProtoId<TagPrototype> HideContextMenuTag = ProtoId<TagPrototype>.op_Implicit("HideContextMenu");
  public MenuVisibility Visibility;
  public Action<VerbsResponseEvent>? OnVerbsResponse;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<VerbsResponseEvent>(new EntityEventHandler<VerbsResponseEvent>(this.HandleVerbResponse), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<float>(this.Subs, this._cfg, CCVars.GameEntityMenuLookup, new Action<float>(this.OnLookupChanged), true);
  }

  private void OnLookupChanged(float val) => this._lookupSize = val;

  public bool TryGetEntityMenuEntities(MapCoordinates targetPos, [NotNullWhen(true)] out List<EntityUid>? entities)
  {
    entities = (List<EntityUid>) null;
    if (!(this._stateManager.CurrentState is GameplayStateBase))
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return false;
    EntityUid player = localEntity.GetValueOrDefault();
    MenuVisibility menuVisibility = this._eyeManager.CurrentEye.DrawFov ? this.Visibility : this.Visibility | MenuVisibility.NoFov;
    MenuVisibilityEvent menuVisibilityEvent = new MenuVisibilityEvent()
    {
      TargetPos = targetPos,
      Visibility = menuVisibility
    };
    this.RaiseLocalEvent<MenuVisibilityEvent>(player, ref menuVisibilityEvent, false);
    MenuVisibility visibility = menuVisibilityEvent.Visibility;
    Box2 box2 = Box2.CenteredAround(targetPos.Position, new Vector2(this._lookupSize, this._lookupSize));
    HashSet<ComponentTreeEntry<SpriteComponent>> componentTreeEntrySet = ((ComponentTreeSystem<SpriteTreeComponent, SpriteComponent>) this._tree).QueryAabb(targetPos.MapId, box2, true);
    entities = new List<EntityUid>(componentTreeEntrySet.Count);
    foreach (ComponentTreeEntry<SpriteComponent> componentTreeEntry in componentTreeEntrySet)
      entities.Add(componentTreeEntry.Uid);
    BaseContainer baseContainer1;
    BaseContainer baseContainer2;
    if (this._containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((player, (TransformComponent) null)), ref baseContainer1) && (entities.Contains(baseContainer1.Owner) || this._containers.TryGetOuterContainer(baseContainer1.Owner, this.Transform(baseContainer1.Owner), ref baseContainer2) && entities.Contains(baseContainer2.Owner)))
    {
      if (!entities.Contains(baseContainer1.Owner))
        entities.Add(baseContainer1.Owner);
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) baseContainer1.ContainedEntities)
      {
        if (!entities.Contains(containedEntity))
          entities.Add(containedEntity);
      }
    }
    if ((visibility & MenuVisibility.InContainer) != MenuVisibility.Default)
    {
      LookupFlags lookupFlags = (LookupFlags) 46;
      foreach (EntityUid entityUid in this._lookup.GetEntitiesInRange(targetPos, this._lookupSize, lookupFlags))
      {
        if (!entities.Contains(entityUid))
          entities.Add(entityUid);
      }
    }
    if ((visibility & MenuVisibility.NoFov) == MenuVisibility.Default)
    {
      ExaminerComponent examinerComp;
      this.TryComp<ExaminerComponent>(player, ref examinerComp);
      for (int index = entities.Count - 1; index >= 0; --index)
      {
        if (!this._examine.CanExamine(player, targetPos, (SharedInteractionSystem.Ignored) (e => EntityUid.op_Equality(e, player)), new EntityUid?(entities[index]), examinerComp))
          Extensions.RemoveSwap<EntityUid>((IList<EntityUid>) entities, index);
      }
    }
    if ((visibility & MenuVisibility.Invisible) != MenuVisibility.Default)
      return entities.Count != 0;
    for (int index = entities.Count - 1; index >= 0; --index)
    {
      if (this._tagSystem.HasTag(entities[index], VerbSystem.HideContextMenuTag))
        Extensions.RemoveSwap<EntityUid>((IList<EntityUid>) entities, index);
    }
    if (!this.HasComp<GhostComponent>(player))
    {
      for (int index = entities.Count - 1; index >= 0; --index)
      {
        if (this.HasComp<LandMineComponent>(entities[index]))
          Extensions.RemoveSwap<EntityUid>((IList<EntityUid>) entities, index);
      }
    }
    if (baseContainer1 == null && (visibility & MenuVisibility.InContainer) == MenuVisibility.Default)
      return entities.Count != 0;
    EntityQuery<SpriteComponent> entityQuery = this.GetEntityQuery<SpriteComponent>();
    for (int index = entities.Count - 1; index >= 0; --index)
    {
      SpriteComponent spriteComponent;
      if (!entityQuery.TryGetComponent(entities[index], ref spriteComponent) || !spriteComponent.Visible)
        Extensions.RemoveSwap<EntityUid>((IList<EntityUid>) entities, index);
    }
    return entities.Count != 0;
  }

  public SortedSet<Verb> GetVerbs(
    NetEntity target,
    EntityUid user,
    List<Type> verbTypes,
    out List<VerbCategory> extraCategories,
    bool force = false)
  {
    if (!((NetEntity) ref target).IsClientSide())
    {
      NetEntity entityUid = target;
      List<Type> verbTypes1 = verbTypes;
      bool flag = force;
      NetEntity? slotOwner = new NetEntity?();
      int num = flag ? 1 : 0;
      this.RaiseNetworkEvent((EntityEventArgs) new RequestServerVerbsEvent(entityUid, (IEnumerable<Type>) verbTypes1, slotOwner, num != 0));
    }
    EntityUid? nullable;
    if (this.TryGetEntity(target, ref nullable))
      return this.GetLocalVerbs(nullable.Value, user, verbTypes, out extraCategories, force);
    extraCategories = new List<VerbCategory>();
    return new SortedSet<Verb>();
  }

  public void ExecuteVerb(EntityUid target, Verb verb)
  {
    this.ExecuteVerb(this.GetNetEntity(target, (MetaDataComponent) null), verb);
  }

  public void ExecuteVerb(NetEntity target, Verb verb)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    if (verb.Disabled)
    {
      if (string.IsNullOrWhiteSpace(verb.Message))
        return;
      this._popupSystem.PopupEntity(FormattedMessage.RemoveMarkupOrThrow(verb.Message), valueOrDefault, PopupType.Small);
    }
    else if (verb.ClientExclusive || ((NetEntity) ref target).IsClientSide())
      this.ExecuteVerb(verb, valueOrDefault, this.GetEntity(target));
    else
      this.RaisePredictiveEvent<ExecuteVerbEvent>(new ExecuteVerbEvent(target, verb));
  }

  private void HandleVerbResponse(VerbsResponseEvent msg)
  {
    Action<VerbsResponseEvent> onVerbsResponse = this.OnVerbsResponse;
    if (onVerbsResponse == null)
      return;
    onVerbsResponse(msg);
  }
}
