using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.IdentityManagement;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Forensics;
using Content.Shared.IdentityManagement;
using Content.Shared.Implants.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Implants;

public abstract class SharedImplanterSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private ItemSlotsSystem _itemSlots;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private DamageableSystem _damageableSystem;

	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	[Dependency]
	private IPrototypeManager _proto;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ImplanterComponent, ComponentInit>((ComponentEventHandler<ImplanterComponent, ComponentInit>)OnImplanterInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImplanterComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<ImplanterComponent, EntInsertedIntoContainerMessage>)OnEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImplanterComponent, ExaminedEvent>((ComponentEventHandler<ImplanterComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImplanterComponent, UseInHandEvent>((ComponentEventHandler<ImplanterComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImplanterComponent, GetVerbsEvent<InteractionVerb>>((ComponentEventHandler<ImplanterComponent, GetVerbsEvent<InteractionVerb>>)OnVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ImplanterComponent, DeimplantChangeVerbMessage>((ComponentEventHandler<ImplanterComponent, DeimplantChangeVerbMessage>)OnSelected, (Type[])null, (Type[])null);
	}

	private void OnImplanterInit(EntityUid uid, ImplanterComponent component, ComponentInit args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId? implant;
		if (component.Implant.HasValue)
		{
			ItemSlot implanterSlot = component.ImplanterSlot;
			implant = component.Implant;
			implanterSlot.StartingItem = (implant.HasValue ? EntProtoId.op_Implicit(implant.GetValueOrDefault()) : null);
		}
		_itemSlots.AddItemSlot(uid, "implanter_slot", component.ImplanterSlot);
		implant = component.DeimplantChosen;
		if (!implant.HasValue)
		{
			component.DeimplantChosen = Extensions.FirstOrNull<EntProtoId>((IEnumerable<EntProtoId>)component.DeimplantWhitelist);
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnEntInserted(EntityUid uid, ImplanterComponent component, EntInsertedIntoContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent implantData = ((EntitySystem)this).Comp<MetaDataComponent>(((ContainerModifiedMessage)args).Entity);
		component.ImplantData = (implantData.EntityName, implantData.EntityDescription);
	}

	private void OnExamine(EntityUid uid, ImplanterComponent component, ExaminedEvent args)
	{
		if (component.ImplanterSlot.HasItem && args.IsInDetailsRange)
		{
			args.PushMarkup(base.Loc.GetString("implanter-contained-implant-text", (ValueTuple<string, object>)("desc", component.ImplantData.Item2)));
		}
	}

	public bool CheckSameImplant(EntityUid target, EntityUid implant)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ImplantedComponent implanted = default(ImplantedComponent);
		if (!((EntitySystem)this).TryComp<ImplantedComponent>(target, ref implanted))
		{
			return false;
		}
		EntityPrototype implantPrototype = ((EntitySystem)this).Prototype(implant, (MetaDataComponent)null);
		return ((BaseContainer)implanted.ImplantContainer).ContainedEntities.Any((EntityUid entity) => ((EntitySystem)this).Prototype(entity, (MetaDataComponent)null) == implantPrototype);
	}

	private void OnVerb(EntityUid uid, ImplanterComponent component, GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && component.CurrentMode == ImplanterToggleMode.Draw)
		{
			args.Verbs.Add(new InteractionVerb
			{
				Text = base.Loc.GetString("implanter-set-draw-verb"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					TryOpenUi(uid, args.User, component);
				}
			});
		}
	}

	private void OnUseInHand(EntityUid uid, ImplanterComponent? component, UseInHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ImplanterComponent>(uid, ref component, true) && component.CurrentMode == ImplanterToggleMode.Draw)
		{
			TryOpenUi(uid, args.User, component);
		}
	}

	private void OnSelected(EntityUid uid, ImplanterComponent component, DeimplantChangeVerbMessage args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		component.DeimplantChosen = EntProtoId.op_Implicit(args.Implant);
		SetSelectedDeimplant(uid, args.Implant, component);
	}

	private void TryOpenUi(EntityUid uid, EntityUid user, ImplanterComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ImplanterComponent>(uid, ref component, true))
		{
			_uiSystem.TryToggleUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)DeimplantUiKey.Key, user);
			ImplanterComponent implanterComponent = component;
			EntProtoId? deimplantChosen = implanterComponent.DeimplantChosen;
			if (!deimplantChosen.HasValue)
			{
				implanterComponent.DeimplantChosen = Extensions.FirstOrNull<EntProtoId>((IEnumerable<EntProtoId>)component.DeimplantWhitelist);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}

	public void Implant(EntityUid user, EntityUid target, EntityUid implanter, ImplanterComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (!CanImplant(user, target, implanter, component, out EntityUid? implant, out SubdermalImplantComponent implantComp))
		{
			return;
		}
		if (!component.AllowMultipleImplants && CheckSameImplant(target, implant.Value))
		{
			IdentityEntity name = Identity.Name(target, (IEntityManager)(object)base.EntityManager, user);
			string msg = base.Loc.GetString("implanter-component-implant-already", (ValueTuple<string, object>)("implant", implant), (ValueTuple<string, object>)("target", name));
			_popup.PopupEntity(msg, target, user);
			return;
		}
		Container implantContainer = ((EntitySystem)this).EnsureComp<ImplantedComponent>(target).ImplantContainer;
		if (component.ImplanterSlot.ContainerSlot != null)
		{
			_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(implant.Value), (BaseContainer)(object)component.ImplanterSlot.ContainerSlot, true, false, (EntityCoordinates?)null, (Angle?)null);
		}
		implantComp.ImplantedEntity = target;
		((BaseContainer)implantContainer).OccludesLight = false;
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(implant.Value), (BaseContainer)(object)implantContainer, (TransformComponent)null, false);
		if (component.CurrentMode == ImplanterToggleMode.Inject && !component.ImplantOnly)
		{
			DrawMode(implanter, component);
		}
		else
		{
			ImplantMode(implanter, component);
		}
		TransferDnaEvent transferDnaEvent = new TransferDnaEvent();
		transferDnaEvent.Donor = target;
		transferDnaEvent.Recipient = implanter;
		TransferDnaEvent ev = transferDnaEvent;
		((EntitySystem)this).RaiseLocalEvent<TransferDnaEvent>(target, ref ev, false);
		((EntitySystem)this).Dirty(implanter, (IComponent)(object)component, (MetaDataComponent)null);
	}

	public bool CanImplant(EntityUid user, EntityUid target, EntityUid implanter, ImplanterComponent component, [NotNullWhen(true)] out EntityUid? implant, [NotNullWhen(true)] out SubdermalImplantComponent? implantComp)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot? containerSlot = component.ImplanterSlot.ContainerSlot;
		implant = ((containerSlot != null) ? Extensions.FirstOrNull<EntityUid>((IEnumerable<EntityUid>)((BaseContainer)containerSlot).ContainedEntities) : ((EntityUid?)null));
		if (!((EntitySystem)this).TryComp<SubdermalImplantComponent>(implant, ref implantComp))
		{
			return false;
		}
		if (!CheckTarget(target, component.Whitelist, component.Blacklist) || !CheckTarget(target, implantComp.Whitelist, implantComp.Blacklist))
		{
			return false;
		}
		AddImplantAttemptEvent ev = new AddImplantAttemptEvent(user, target, implant.Value, implanter);
		((EntitySystem)this).RaiseLocalEvent<AddImplantAttemptEvent>(target, ev, false);
		return !((CancellableEntityEventArgs)ev).Cancelled;
	}

	protected bool CheckTarget(EntityUid target, EntityWhitelist? whitelist, EntityWhitelist? blacklist)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (_whitelistSystem.IsWhitelistPassOrNull(whitelist, target))
		{
			return _whitelistSystem.IsBlacklistFailOrNull(blacklist, target);
		}
		return false;
	}

	public void Draw(EntityUid implanter, EntityUid user, EntityUid target, ImplanterComponent component)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		ContainerSlot implanterContainer = component.ImplanterSlot.ContainerSlot;
		if (implanterContainer == null)
		{
			return;
		}
		bool permanentFound = false;
		BaseContainer implantContainer = default(BaseContainer);
		if (_container.TryGetContainer(target, "implant", ref implantContainer, (ContainerManagerComponent)null))
		{
			EntityQuery<SubdermalImplantComponent> implantCompQuery = ((EntitySystem)this).GetEntityQuery<SubdermalImplantComponent>();
			if (component.AllowDeimplantAll)
			{
				SubdermalImplantComponent implantComp = default(SubdermalImplantComponent);
				foreach (EntityUid implant in implantContainer.ContainedEntities)
				{
					if (implantCompQuery.TryGetComponent(implant, ref implantComp))
					{
						if (_container.CanRemove(implant, implantContainer))
						{
							DrawImplantIntoImplanter(implanter, target, implant, implantContainer, implanterContainer, implantComp);
							permanentFound = implantComp.Permanent;
							break;
						}
						DrawPermanentFailurePopup(implant, target, user);
						permanentFound = implantComp.Permanent;
					}
				}
				if (component.CurrentMode == ImplanterToggleMode.Draw && !component.ImplantOnly && !permanentFound)
				{
					ImplantMode(implanter, component);
				}
			}
			else
			{
				EntityUid? implant2 = null;
				SubdermalImplantComponent subdermalComp = default(SubdermalImplantComponent);
				foreach (EntityUid implantEntity in implantContainer.ContainedEntities)
				{
					if (!((EntitySystem)this).TryComp<SubdermalImplantComponent>(implantEntity, ref subdermalComp))
					{
						continue;
					}
					EntProtoId? deimplantChosen = component.DeimplantChosen;
					EntProtoId? drawableProtoIdOverride = subdermalComp.DrawableProtoIdOverride;
					if (deimplantChosen.HasValue != drawableProtoIdOverride.HasValue || (deimplantChosen.HasValue && !(deimplantChosen.GetValueOrDefault() == drawableProtoIdOverride.GetValueOrDefault())))
					{
						if (((EntitySystem)this).Prototype(implantEntity, (MetaDataComponent)null) == null)
						{
							continue;
						}
						drawableProtoIdOverride = component.DeimplantChosen;
						EntProtoId val = EntProtoId.op_Implicit(((EntitySystem)this).Prototype(implantEntity, (MetaDataComponent)null));
						if (!drawableProtoIdOverride.HasValue || !(drawableProtoIdOverride.GetValueOrDefault() == val))
						{
							continue;
						}
					}
					implant2 = implantEntity;
				}
				SubdermalImplantComponent implantComp2 = default(SubdermalImplantComponent);
				if (implant2.HasValue && implantCompQuery.TryGetComponent(implant2, ref implantComp2))
				{
					if (!_container.CanRemove(implant2.Value, implantContainer))
					{
						DrawPermanentFailurePopup(implant2.Value, target, user);
						permanentFound = implantComp2.Permanent;
					}
					else
					{
						DrawImplantIntoImplanter(implanter, target, implant2.Value, implantContainer, implanterContainer, implantComp2);
						permanentFound = implantComp2.Permanent;
					}
					if (component.CurrentMode == ImplanterToggleMode.Draw && !component.ImplantOnly && !permanentFound)
					{
						ImplantMode(implanter, component);
					}
				}
				else
				{
					DrawCatastrophicFailure(implanter, component, user);
				}
			}
			((EntitySystem)this).Dirty(implanter, (IComponent)(object)component, (MetaDataComponent)null);
		}
		else
		{
			DrawCatastrophicFailure(implanter, component, user);
		}
	}

	private void DrawPermanentFailurePopup(EntityUid implant, EntityUid target, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		EntityUid implantName = Identity.Entity(implant, (IEntityManager)(object)base.EntityManager);
		EntityUid targetName = Identity.Entity(target, (IEntityManager)(object)base.EntityManager);
		string failedPermanentMessage = base.Loc.GetString("implanter-draw-failed-permanent", (ValueTuple<string, object>)("implant", implantName), (ValueTuple<string, object>)("target", targetName));
		_popup.PopupEntity(failedPermanentMessage, target, user);
	}

	private void DrawImplantIntoImplanter(EntityUid implanter, EntityUid target, EntityUid implant, BaseContainer implantContainer, ContainerSlot implanterContainer, SubdermalImplantComponent implantComp)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(implant), implantContainer, true, false, (EntityCoordinates?)null, (Angle?)null);
		implantComp.ImplantedEntity = null;
		_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(implant), (BaseContainer)(object)implanterContainer, (TransformComponent)null, false);
		TransferDnaEvent transferDnaEvent = new TransferDnaEvent();
		transferDnaEvent.Donor = target;
		transferDnaEvent.Recipient = implanter;
		TransferDnaEvent ev = transferDnaEvent;
		((EntitySystem)this).RaiseLocalEvent<TransferDnaEvent>(target, ref ev, false);
	}

	private void DrawCatastrophicFailure(EntityUid implanter, ImplanterComponent component, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		_damageableSystem.TryChangeDamage(user, component.DeimplantFailureDamage, ignoreResistances: true, interruptsDoAfters: true, null, implanter);
		EntityUid userName = Identity.Entity(user, (IEntityManager)(object)base.EntityManager);
		string failedCatastrophicallyMessage = base.Loc.GetString("implanter-draw-failed-catastrophically", (ValueTuple<string, object>)("user", userName));
		_popup.PopupEntity(failedCatastrophicallyMessage, user, PopupType.MediumCaution);
	}

	private void ImplantMode(EntityUid uid, ImplanterComponent component)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		component.CurrentMode = ImplanterToggleMode.Inject;
		ChangeOnImplantVisualizer(uid, component);
	}

	private void DrawMode(EntityUid uid, ImplanterComponent component)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		component.CurrentMode = ImplanterToggleMode.Draw;
		ChangeOnImplantVisualizer(uid, component);
	}

	private void ChangeOnImplantVisualizer(EntityUid uid, ImplanterComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			bool implantFound = (component.ImplanterSlot.HasItem ? true : false);
			if (component.CurrentMode == ImplanterToggleMode.Inject && !component.ImplantOnly)
			{
				_appearance.SetData(uid, (Enum)ImplanterVisuals.Full, (object)implantFound, appearance);
			}
			else if (component.CurrentMode == ImplanterToggleMode.Inject && component.ImplantOnly)
			{
				_appearance.SetData(uid, (Enum)ImplanterVisuals.Full, (object)implantFound, appearance);
				_appearance.SetData(uid, (Enum)ImplanterImplantOnlyVisuals.ImplantOnly, (object)component.ImplantOnly, appearance);
			}
			else
			{
				_appearance.SetData(uid, (Enum)ImplanterVisuals.Full, (object)implantFound, appearance);
			}
		}
	}

	public void SetSelectedDeimplant(EntityUid uid, string? implant, ImplanterComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ImplanterComponent>(uid, ref component, false))
		{
			EntityPrototype proto = default(EntityPrototype);
			if (implant != null && _proto.TryIndex<EntityPrototype>(implant, ref proto))
			{
				component.DeimplantChosen = EntProtoId.op_Implicit(proto);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}
}
