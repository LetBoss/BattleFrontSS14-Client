// Decompiled with JetBrains decompiler
// Type: Content.Client.Hands.Systems.HandsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.DisplacementMap;
using Content.Client.Examine;
using Content.Client.Strip;
using Content.Client.Verbs.UI;
using Content.Shared.DisplacementMap;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Client.Hands.Systems;

public sealed class HandsSystem : SharedHandsSystem
{
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private StrippableSystem _stripSys;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private ExamineSystem _examine;
  [Dependency]
  private DisplacementMapSystem _displacement;

  public event Action<string?>? OnPlayerSetActiveHand;

  public event Action<Entity<HandsComponent>>? OnPlayerHandsAdded;

  public event Action? OnPlayerHandsRemoved;

  public event Action<string, EntityUid>? OnPlayerItemAdded;

  public event Action<string, EntityUid>? OnPlayerItemRemoved;

  public event Action<string>? OnPlayerHandBlocked;

  public event Action<string>? OnPlayerHandUnblocked;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandsComponent, LocalPlayerAttachedEvent>(new ComponentEventHandler<HandsComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(HandlePlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandsComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<HandsComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(HandlePlayerDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandsComponent, ComponentStartup>(new ComponentEventHandler<HandsComponent, ComponentStartup>((object) this, __methodptr(OnHandsStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandsComponent, ComponentShutdown>(new ComponentEventHandler<HandsComponent, ComponentShutdown>((object) this, __methodptr(OnHandsShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandsComponent, ComponentHandleState>(new EntityEventRefHandler<HandsComponent, ComponentHandleState>((object) this, __methodptr(HandleComponentState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandsComponent, VisualsChangedEvent>(new ComponentEventHandler<HandsComponent, VisualsChangedEvent>((object) this, __methodptr(OnVisualsChanged)), (Type[]) null, (Type[]) null);
    this.OnHandSetActive += new Action<Entity<HandsComponent>?>(this.OnHandActivated);
  }

  private void HandleComponentState(Entity<HandsComponent> ent, ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is HandsComponentState current))
      return;
    IEnumerable<string> second = current.Hands.Keys.Except<string>((IEnumerable<string>) ent.Comp.Hands.Keys);
    foreach (string handName in ent.Comp.Hands.Keys.Except<string>((IEnumerable<string>) current.Hands.Keys))
      this.RemoveHand(ent.AsNullable(), handName);
    foreach (string str in current.SortedHands.Intersect<string>(second))
      this.AddHand(ent.AsNullable(), str, current.Hands[str]);
    ent.Comp.SortedHands = new List<string>((IEnumerable<string>) current.SortedHands);
    this.SetActiveHand(ent.AsNullable(), current.ActiveHandId);
    this._stripSys.UpdateUi(Entity<HandsComponent>.op_Implicit(ent));
  }

  public void ReloadHandButtons()
  {
    Entity<HandsComponent>? hands;
    if (!this.TryGetPlayerHands(out hands))
      return;
    Action<Entity<HandsComponent>> playerHandsAdded = this.OnPlayerHandsAdded;
    if (playerHandsAdded == null)
      return;
    playerHandsAdded(hands.Value);
  }

  public override void DoDrop(
    Entity<HandsComponent?> ent,
    string handId,
    bool doDropInteraction = true,
    bool log = true)
  {
    base.DoDrop(ent, handId, doDropInteraction, log);
    EntityUid? held;
    SpriteComponent spriteComponent;
    if (!this.TryGetHeldItem(ent, handId, out held) || !this.TryComp<SpriteComponent>(held, ref spriteComponent))
      return;
    spriteComponent.RenderOrder = this.EntityManager.CurrentTick.Value;
  }

  public EntityUid? GetActiveHandEntity()
  {
    Entity<HandsComponent>? hands;
    return !this.TryGetPlayerHands(out hands) ? new EntityUid?() : this.GetActiveItem(hands.Value.AsNullable());
  }

  public bool TryGetPlayerHands([NotNullWhen(true)] out Entity<HandsComponent>? hands)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    hands = new Entity<HandsComponent>?();
    HandsComponent handsComponent;
    if (!localEntity.HasValue || !this.TryComp<HandsComponent>(localEntity.Value, ref handsComponent))
      return false;
    hands = new Entity<HandsComponent>?(Entity<HandsComponent>.op_Implicit((localEntity.Value, handsComponent)));
    return true;
  }

  public void UIHandClick(Entity<HandsComponent> ent, string handName, bool switchHand = true)
  {
    HandsComponent comp = ent.Comp;
    if (comp.ActiveHandId == null)
      return;
    EntityUid? heldItem = this.GetHeldItem(ent.AsNullable(), handName);
    EntityUid? activeItem = this.GetActiveItem(ent.AsNullable());
    if (handName == comp.ActiveHandId && activeItem.HasValue)
      this.RaisePredictiveEvent<RequestUseInHandEvent>(new RequestUseInHandEvent());
    else if (switchHand && handName != comp.ActiveHandId && !heldItem.HasValue)
      this.RaisePredictiveEvent<RequestSetHandEvent>(new RequestSetHandEvent(handName));
    else if (handName != comp.ActiveHandId && heldItem.HasValue && activeItem.HasValue)
    {
      this.RaisePredictiveEvent<RequestHandInteractUsingEvent>(new RequestHandInteractUsingEvent(handName));
    }
    else
    {
      if (!(handName != comp.ActiveHandId) || !heldItem.HasValue || activeItem.HasValue)
        return;
      this.RaisePredictiveEvent<RequestMoveHandItemEvent>(new RequestMoveHandItemEvent(handName));
    }
  }

  public void UIHandActivate(string handName)
  {
    this.RaisePredictiveEvent<RequestActivateInHandEvent>(new RequestActivateInHandEvent(handName));
  }

  public void UIInventoryExamine(string handName)
  {
    Entity<HandsComponent>? hands;
    EntityUid? held;
    if (!this.TryGetPlayerHands(out hands) || !this.TryGetHeldItem(hands.Value.AsNullable(), handName, out held))
      return;
    this._examine.DoExamine(held.Value);
  }

  public void UIHandOpenContextMenu(string handName)
  {
    Entity<HandsComponent>? hands;
    EntityUid? held;
    if (!this.TryGetPlayerHands(out hands) || !this.TryGetHeldItem(hands.Value.AsNullable(), handName, out held))
      return;
    this._ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(held.Value);
  }

  public void UIHandAltActivateItem(string handName)
  {
    this.RaisePredictiveEvent<RequestHandAltInteractEvent>(new RequestHandAltInteractEvent(handName));
  }

  protected override void HandleEntityInserted(
    EntityUid uid,
    HandsComponent hands,
    EntInsertedIntoContainerMessage args)
  {
    base.HandleEntityInserted(uid, hands, args);
    if (!hands.Hands.ContainsKey(((ContainerModifiedMessage) args).Container.ID))
      return;
    this.UpdateHandVisuals(Entity<HandsComponent, SpriteComponent>.op_Implicit(uid), ((ContainerModifiedMessage) args).Entity, ((ContainerModifiedMessage) args).Container.ID);
    this._stripSys.UpdateUi(uid);
    EntityUid entityUid = uid;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    Action<string, EntityUid> onPlayerItemAdded = this.OnPlayerItemAdded;
    if (onPlayerItemAdded != null)
      onPlayerItemAdded(((ContainerModifiedMessage) args).Container.ID, ((ContainerModifiedMessage) args).Entity);
    if (!this.HasComp<VirtualItemComponent>(((ContainerModifiedMessage) args).Entity))
      return;
    Action<string> playerHandBlocked = this.OnPlayerHandBlocked;
    if (playerHandBlocked == null)
      return;
    playerHandBlocked(((ContainerModifiedMessage) args).Container.ID);
  }

  protected override void HandleEntityRemoved(
    EntityUid uid,
    HandsComponent hands,
    EntRemovedFromContainerMessage args)
  {
    base.HandleEntityRemoved(uid, hands, args);
    if (!hands.Hands.ContainsKey(((ContainerModifiedMessage) args).Container.ID))
      return;
    this.UpdateHandVisuals(Entity<HandsComponent, SpriteComponent>.op_Implicit(uid), ((ContainerModifiedMessage) args).Entity, ((ContainerModifiedMessage) args).Container.ID);
    this._stripSys.UpdateUi(uid);
    EntityUid entityUid = uid;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    Action<string, EntityUid> playerItemRemoved = this.OnPlayerItemRemoved;
    if (playerItemRemoved != null)
      playerItemRemoved(((ContainerModifiedMessage) args).Container.ID, ((ContainerModifiedMessage) args).Entity);
    if (!this.HasComp<VirtualItemComponent>(((ContainerModifiedMessage) args).Entity))
      return;
    Action<string> playerHandUnblocked = this.OnPlayerHandUnblocked;
    if (playerHandUnblocked == null)
      return;
    playerHandUnblocked(((ContainerModifiedMessage) args).Container.ID);
  }

  private void UpdateHandVisuals(
    Entity<HandsComponent?, SpriteComponent?> ent,
    EntityUid held,
    string handId)
  {
    if (!this.Resolve<HandsComponent, SpriteComponent>(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), ref ent.Comp1, ref ent.Comp2, false))
      return;
    HandsComponent comp1 = ent.Comp1;
    SpriteComponent comp2 = ent.Comp2;
    Hand? hand;
    if (!this.TryGetHand(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp1)), handId, out hand))
      return;
    EntityUid entityUid = Entity<HandsComponent, SpriteComponent>.op_Implicit(ent);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(entityUid, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) != 0)
    {
      Action<string, EntityUid> onPlayerItemAdded = this.OnPlayerItemAdded;
      if (onPlayerItemAdded != null)
        onPlayerItemAdded(handId, held);
    }
    if (!comp1.ShowInHands)
      return;
    HashSet<string> revealedLayers;
    if (comp1.RevealedLayers.TryGetValue(hand.Value.Location, out revealedLayers))
    {
      foreach (string str in revealedLayers)
        this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), str, true);
      revealedLayers.Clear();
    }
    else
    {
      revealedLayers = new HashSet<string>();
      comp1.RevealedLayers[hand.Value.Location] = revealedLayers;
    }
    if (this.HandIsEmpty(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp1)), handId))
    {
      this.RaiseLocalEvent<HeldVisualsUpdatedEvent>(held, new HeldVisualsUpdatedEvent(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), revealedLayers), true);
    }
    else
    {
      GetInhandVisualsEvent inhandVisualsEvent = new GetInhandVisualsEvent(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), hand.Value.Location);
      this.RaiseLocalEvent<GetInhandVisualsEvent>(held, inhandVisualsEvent, false);
      if (inhandVisualsEvent.Layers.Count == 0)
      {
        this.RaiseLocalEvent<HeldVisualsUpdatedEvent>(held, new HeldVisualsUpdatedEvent(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), revealedLayers), true);
      }
      else
      {
        foreach ((string key, PrototypeLayerData prototypeLayerData) in inhandVisualsEvent.Layers)
        {
          if (!revealedLayers.Add(key))
          {
            this.Log.Warning($"Duplicate key for in-hand visuals: {key}. Are multiple components attempting to modify the same layer? Entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(held))}");
          }
          else
          {
            int index = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), key);
            if (prototypeLayerData.RsiPath == null && prototypeLayerData.TexturePath == null && comp2[index].Rsi == null)
            {
              ItemComponent itemComponent;
              if (this.TryComp<ItemComponent>(held, ref itemComponent) && itemComponent.RsiPath != null)
              {
                this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), index, new ResPath(itemComponent.RsiPath), new RSI.StateId?());
              }
              else
              {
                SpriteComponent spriteComponent;
                if (this.TryComp<SpriteComponent>(held, ref spriteComponent))
                  this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), index, spriteComponent.BaseRSI, new RSI.StateId?());
              }
            }
            this._sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), index, prototypeLayerData);
            DisplacementData handDisplacement;
            switch (hand.Value.Location)
            {
              case HandLocation.Left:
                handDisplacement = comp1.LeftHandDisplacement;
                break;
              case HandLocation.Right:
                handDisplacement = comp1.RightHandDisplacement;
                break;
              default:
                handDisplacement = comp1.HandDisplacement;
                break;
            }
            DisplacementData data = handDisplacement;
            string displacementKey;
            if (data != null && this._displacement.TryAddDisplacement(data, Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), index, (object) key, out displacementKey))
              revealedLayers.Add(displacementKey);
          }
        }
        this.RaiseLocalEvent<HeldVisualsUpdatedEvent>(held, new HeldVisualsUpdatedEvent(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), revealedLayers), true);
      }
    }
  }

  private void OnVisualsChanged(EntityUid uid, HandsComponent component, VisualsChangedEvent args)
  {
    if (!component.Hands.ContainsKey(args.ContainerId))
      return;
    this.UpdateHandVisuals(Entity<HandsComponent, SpriteComponent>.op_Implicit((uid, component)), this.GetEntity(args.Item), args.ContainerId);
  }

  private void HandlePlayerAttached(
    EntityUid uid,
    HandsComponent component,
    LocalPlayerAttachedEvent args)
  {
    Action<Entity<HandsComponent>> playerHandsAdded = this.OnPlayerHandsAdded;
    if (playerHandsAdded == null)
      return;
    playerHandsAdded(Entity<HandsComponent>.op_Implicit((uid, component)));
  }

  private void HandlePlayerDetached(
    EntityUid uid,
    HandsComponent component,
    LocalPlayerDetachedEvent args)
  {
    Action playerHandsRemoved = this.OnPlayerHandsRemoved;
    if (playerHandsRemoved == null)
      return;
    playerHandsRemoved();
  }

  private void OnHandsStartup(EntityUid uid, HandsComponent component, ComponentStartup args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    Action<Entity<HandsComponent>> playerHandsAdded = this.OnPlayerHandsAdded;
    if (playerHandsAdded == null)
      return;
    playerHandsAdded(Entity<HandsComponent>.op_Implicit((uid, component)));
  }

  private void OnHandsShutdown(EntityUid uid, HandsComponent component, ComponentShutdown args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid entityUid = uid;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    Action playerHandsRemoved = this.OnPlayerHandsRemoved;
    if (playerHandsRemoved == null)
      return;
    playerHandsRemoved();
  }

  private void OnHandActivated(Entity<HandsComponent>? ent)
  {
    if (!ent.HasValue)
      return;
    Entity<HandsComponent> valueOrDefault = ent.GetValueOrDefault();
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    EntityUid owner = valueOrDefault.Owner;
    if ((localEntity.HasValue ? (EntityUid.op_Inequality(localEntity.GetValueOrDefault(), owner) ? 1 : 0) : 1) != 0)
      return;
    Action<string> playerSetActiveHand = this.OnPlayerSetActiveHand;
    if (playerSetActiveHand == null)
      return;
    playerSetActiveHand(valueOrDefault.Comp.ActiveHandId);
  }
}
