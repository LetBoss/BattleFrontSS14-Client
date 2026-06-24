using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, LocalPlayerAttachedEvent>((ComponentEventHandler<HandsComponent, LocalPlayerAttachedEvent>)HandlePlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, LocalPlayerDetachedEvent>((ComponentEventHandler<HandsComponent, LocalPlayerDetachedEvent>)HandlePlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, ComponentStartup>((ComponentEventHandler<HandsComponent, ComponentStartup>)OnHandsStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, ComponentShutdown>((ComponentEventHandler<HandsComponent, ComponentShutdown>)OnHandsShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, ComponentHandleState>((EntityEventRefHandler<HandsComponent, ComponentHandleState>)HandleComponentState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandsComponent, VisualsChangedEvent>((ComponentEventHandler<HandsComponent, VisualsChangedEvent>)OnVisualsChanged, (Type[])null, (Type[])null);
		base.OnHandSetActive += OnHandActivated;
	}

	private void HandleComponentState(Entity<HandsComponent> ent, ref ComponentHandleState args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is HandsComponentState handsComponentState))
		{
			return;
		}
		IEnumerable<string> second = handsComponentState.Hands.Keys.Except(ent.Comp.Hands.Keys);
		foreach (string item in ent.Comp.Hands.Keys.Except(handsComponentState.Hands.Keys))
		{
			RemoveHand(ent.AsNullable(), item);
		}
		foreach (string item2 in handsComponentState.SortedHands.Intersect(second))
		{
			AddHand(ent.AsNullable(), item2, handsComponentState.Hands[item2]);
		}
		ent.Comp.SortedHands = new List<string>(handsComponentState.SortedHands);
		SetActiveHand(ent.AsNullable(), handsComponentState.ActiveHandId);
		_stripSys.UpdateUi(Entity<HandsComponent>.op_Implicit(ent));
	}

	public void ReloadHandButtons()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetPlayerHands(out Entity<HandsComponent>? hands))
		{
			this.OnPlayerHandsAdded?.Invoke(hands.Value);
		}
	}

	public override void DoDrop(Entity<HandsComponent?> ent, string handId, bool doDropInteraction = true, bool log = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.DoDrop(ent, handId, doDropInteraction, log);
		SpriteComponent val = default(SpriteComponent);
		if (TryGetHeldItem(ent, handId, out var held) && ((EntitySystem)this).TryComp<SpriteComponent>(held, ref val))
		{
			val.RenderOrder = ((EntitySystem)this).EntityManager.CurrentTick.Value;
		}
	}

	public EntityUid? GetActiveHandEntity()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetPlayerHands(out Entity<HandsComponent>? hands))
		{
			return null;
		}
		return GetActiveItem(hands.Value.AsNullable());
	}

	public bool TryGetPlayerHands([NotNullWhen(true)] out Entity<HandsComponent>? hands)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		hands = null;
		HandsComponent item = default(HandsComponent);
		if (!localEntity.HasValue || !((EntitySystem)this).TryComp<HandsComponent>(localEntity.Value, ref item))
		{
			return false;
		}
		hands = Entity<HandsComponent>.op_Implicit((localEntity.Value, item));
		return true;
	}

	public void UIHandClick(Entity<HandsComponent> ent, string handName, bool switchHand = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent comp = ent.Comp;
		if (comp.ActiveHandId != null)
		{
			EntityUid? heldItem = GetHeldItem(ent.AsNullable(), handName);
			EntityUid? activeItem = GetActiveItem(ent.AsNullable());
			if (handName == comp.ActiveHandId && activeItem.HasValue)
			{
				((EntitySystem)this).RaisePredictiveEvent<RequestUseInHandEvent>(new RequestUseInHandEvent());
			}
			else if (switchHand && handName != comp.ActiveHandId && !heldItem.HasValue)
			{
				((EntitySystem)this).RaisePredictiveEvent<RequestSetHandEvent>(new RequestSetHandEvent(handName));
			}
			else if (handName != comp.ActiveHandId && heldItem.HasValue && activeItem.HasValue)
			{
				((EntitySystem)this).RaisePredictiveEvent<RequestHandInteractUsingEvent>(new RequestHandInteractUsingEvent(handName));
			}
			else if (handName != comp.ActiveHandId && heldItem.HasValue && !activeItem.HasValue)
			{
				((EntitySystem)this).RaisePredictiveEvent<RequestMoveHandItemEvent>(new RequestMoveHandItemEvent(handName));
			}
		}
	}

	public void UIHandActivate(string handName)
	{
		((EntitySystem)this).RaisePredictiveEvent<RequestActivateInHandEvent>(new RequestActivateInHandEvent(handName));
	}

	public void UIInventoryExamine(string handName)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetPlayerHands(out Entity<HandsComponent>? hands) && TryGetHeldItem(hands.Value.AsNullable(), handName, out var held))
		{
			_examine.DoExamine(held.Value);
		}
	}

	public void UIHandOpenContextMenu(string handName)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetPlayerHands(out Entity<HandsComponent>? hands) && TryGetHeldItem(hands.Value.AsNullable(), handName, out var held))
		{
			_ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(held.Value);
		}
	}

	public void UIHandAltActivateItem(string handName)
	{
		((EntitySystem)this).RaisePredictiveEvent<RequestHandAltInteractEvent>(new RequestHandAltInteractEvent(handName));
	}

	protected override void HandleEntityInserted(EntityUid uid, HandsComponent hands, EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		base.HandleEntityInserted(uid, hands, args);
		if (!hands.Hands.ContainsKey(((ContainerModifiedMessage)args).Container.ID))
		{
			return;
		}
		UpdateHandVisuals(Entity<HandsComponent, SpriteComponent>.op_Implicit(uid), ((ContainerModifiedMessage)args).Entity, ((ContainerModifiedMessage)args).Container.ID);
		_stripSys.UpdateUi(uid);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(uid != localEntity.GetValueOrDefault()))
		{
			this.OnPlayerItemAdded?.Invoke(((ContainerModifiedMessage)args).Container.ID, ((ContainerModifiedMessage)args).Entity);
			if (((EntitySystem)this).HasComp<VirtualItemComponent>(((ContainerModifiedMessage)args).Entity))
			{
				this.OnPlayerHandBlocked?.Invoke(((ContainerModifiedMessage)args).Container.ID);
			}
		}
	}

	protected override void HandleEntityRemoved(EntityUid uid, HandsComponent hands, EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		base.HandleEntityRemoved(uid, hands, args);
		if (!hands.Hands.ContainsKey(((ContainerModifiedMessage)args).Container.ID))
		{
			return;
		}
		UpdateHandVisuals(Entity<HandsComponent, SpriteComponent>.op_Implicit(uid), ((ContainerModifiedMessage)args).Entity, ((ContainerModifiedMessage)args).Container.ID);
		_stripSys.UpdateUi(uid);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && !(uid != localEntity.GetValueOrDefault()))
		{
			this.OnPlayerItemRemoved?.Invoke(((ContainerModifiedMessage)args).Container.ID, ((ContainerModifiedMessage)args).Entity);
			if (((EntitySystem)this).HasComp<VirtualItemComponent>(((ContainerModifiedMessage)args).Entity))
			{
				this.OnPlayerHandUnblocked?.Invoke(((ContainerModifiedMessage)args).Container.ID);
			}
		}
	}

	private void UpdateHandVisuals(Entity<HandsComponent?, SpriteComponent?> ent, EntityUid held, string handId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandsComponent, SpriteComponent>(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), ref ent.Comp1, ref ent.Comp2, false))
		{
			return;
		}
		HandsComponent comp = ent.Comp1;
		SpriteComponent comp2 = ent.Comp2;
		if (!TryGetHand(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp)), handId, out var hand))
		{
			return;
		}
		EntityUid val = Entity<HandsComponent, SpriteComponent>.op_Implicit(ent);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && val == localEntity.GetValueOrDefault())
		{
			this.OnPlayerItemAdded?.Invoke(handId, held);
		}
		if (!comp.ShowInHands)
		{
			return;
		}
		if (comp.RevealedLayers.TryGetValue(hand.Value.Location, out HashSet<string> value))
		{
			foreach (string item in value)
			{
				_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), item, true);
			}
			value.Clear();
		}
		else
		{
			value = new HashSet<string>();
			comp.RevealedLayers[hand.Value.Location] = value;
		}
		if (HandIsEmpty(Entity<HandsComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp)), handId))
		{
			((EntitySystem)this).RaiseLocalEvent<HeldVisualsUpdatedEvent>(held, new HeldVisualsUpdatedEvent(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), value), true);
			return;
		}
		GetInhandVisualsEvent getInhandVisualsEvent = new GetInhandVisualsEvent(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), hand.Value.Location);
		((EntitySystem)this).RaiseLocalEvent<GetInhandVisualsEvent>(held, getInhandVisualsEvent, false);
		if (getInhandVisualsEvent.Layers.Count == 0)
		{
			((EntitySystem)this).RaiseLocalEvent<HeldVisualsUpdatedEvent>(held, new HeldVisualsUpdatedEvent(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), value), true);
			return;
		}
		ItemComponent itemComponent = default(ItemComponent);
		SpriteComponent val3 = default(SpriteComponent);
		foreach (var (text, val2) in getInhandVisualsEvent.Layers)
		{
			if (!value.Add(text))
			{
				((EntitySystem)this).Log.Warning($"Duplicate key for in-hand visuals: {text}. Are multiple components attempting to modify the same layer? Entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(held))}");
				continue;
			}
			int num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), text);
			if (val2.RsiPath == null && val2.TexturePath == null && comp2[num].Rsi == null)
			{
				if (((EntitySystem)this).TryComp<ItemComponent>(held, ref itemComponent) && itemComponent.RsiPath != null)
				{
					_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), num, new ResPath(itemComponent.RsiPath), (StateId?)null);
				}
				else if (((EntitySystem)this).TryComp<SpriteComponent>(held, ref val3))
				{
					_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), num, val3.BaseRSI, (StateId?)null);
				}
			}
			_sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), num, val2);
			DisplacementData displacementData = hand.Value.Location switch
			{
				HandLocation.Left => comp.LeftHandDisplacement, 
				HandLocation.Right => comp.RightHandDisplacement, 
				_ => comp.HandDisplacement, 
			};
			if (displacementData != null && _displacement.TryAddDisplacement(displacementData, Entity<SpriteComponent>.op_Implicit((Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), comp2)), num, text, out string displacementKey))
			{
				value.Add(displacementKey);
			}
		}
		((EntitySystem)this).RaiseLocalEvent<HeldVisualsUpdatedEvent>(held, new HeldVisualsUpdatedEvent(Entity<HandsComponent, SpriteComponent>.op_Implicit(ent), value), true);
	}

	private void OnVisualsChanged(EntityUid uid, HandsComponent component, VisualsChangedEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (component.Hands.ContainsKey(args.ContainerId))
		{
			UpdateHandVisuals(Entity<HandsComponent, SpriteComponent>.op_Implicit((uid, component)), ((EntitySystem)this).GetEntity(args.Item), args.ContainerId);
		}
	}

	private void HandlePlayerAttached(EntityUid uid, HandsComponent component, LocalPlayerAttachedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		this.OnPlayerHandsAdded?.Invoke(Entity<HandsComponent>.op_Implicit((uid, component)));
	}

	private void HandlePlayerDetached(EntityUid uid, HandsComponent component, LocalPlayerDetachedEvent args)
	{
		this.OnPlayerHandsRemoved?.Invoke();
	}

	private void OnHandsStartup(EntityUid uid, HandsComponent component, ComponentStartup args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
		{
			this.OnPlayerHandsAdded?.Invoke(Entity<HandsComponent>.op_Implicit((uid, component)));
		}
	}

	private void OnHandsShutdown(EntityUid uid, HandsComponent component, ComponentShutdown args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == uid)
		{
			this.OnPlayerHandsRemoved?.Invoke();
		}
	}

	private void OnHandActivated(Entity<HandsComponent>? ent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.HasValue)
		{
			Entity<HandsComponent> valueOrDefault = ent.GetValueOrDefault();
			EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			EntityUid owner = valueOrDefault.Owner;
			if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != owner))
			{
				this.OnPlayerSetActiveHand?.Invoke(valueOrDefault.Comp.ActiveHandId);
			}
		}
	}
}
