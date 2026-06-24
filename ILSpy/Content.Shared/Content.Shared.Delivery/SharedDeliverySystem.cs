using System;
using System.Linq;
using Content.Shared.Examine;
using Content.Shared.FingerprintReader;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Objectives.Components;
using Content.Shared.Popups;
using Content.Shared.Shuttles.Components;
using Content.Shared.Tag;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Delivery;

public abstract class SharedDeliverySystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private FingerprintReaderSystem _fingerprintReader;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private NameModifierSystem _nameModifier;

	private static readonly ProtoId<TagPrototype> TrashTag = ProtoId<TagPrototype>.op_Implicit("Trash");

	private static readonly ProtoId<TagPrototype> RecyclableTag = ProtoId<TagPrototype>.op_Implicit("Recyclable");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DeliveryComponent, ExaminedEvent>((EntityEventRefHandler<DeliveryComponent, ExaminedEvent>)OnDeliveryExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryComponent, UseInHandEvent>((EntityEventRefHandler<DeliveryComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<DeliveryComponent, GetVerbsEvent<AlternativeVerb>>)OnGetDeliveryVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryComponent, AttemptSimpleToolUseEvent>((EntityEventRefHandler<DeliveryComponent, AttemptSimpleToolUseEvent>)OnAttemptSimpleToolUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryComponent, SimpleToolDoAfterEvent>((EntityEventRefHandler<DeliveryComponent, SimpleToolDoAfterEvent>)OnSimpleToolUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliverySpawnerComponent, ExaminedEvent>((EntityEventRefHandler<DeliverySpawnerComponent, ExaminedEvent>)OnSpawnerExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliverySpawnerComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<DeliverySpawnerComponent, GetVerbsEvent<AlternativeVerb>>)OnGetSpawnerVerbs, (Type[])null, (Type[])null);
	}

	private void OnDeliveryExamine(Entity<DeliveryComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		string jobTitle = ent.Comp.RecipientJobTitle ?? base.Loc.GetString("delivery-recipient-no-job");
		string recipientName = ent.Comp.RecipientName ?? base.Loc.GetString("delivery-recipient-no-name");
		using (args.PushGroup("DeliveryComponent", 1))
		{
			if (ent.Comp.IsOpened)
			{
				args.PushText(base.Loc.GetString("delivery-already-opened-examine"));
			}
			args.PushText(base.Loc.GetString("delivery-recipient-examine", (ValueTuple<string, object>)("recipient", recipientName), (ValueTuple<string, object>)("job", jobTitle)));
		}
		if (ent.Comp.IsLocked)
		{
			float multiplier = GetDeliveryMultiplier(ent);
			double totalSpesos = Math.Round((float)ent.Comp.BaseSpesoReward * multiplier);
			args.PushMarkup(base.Loc.GetString("delivery-earnings-examine", (ValueTuple<string, object>)("spesos", totalSpesos)), -1);
		}
	}

	private void OnSpawnerExamine(Entity<DeliverySpawnerComponent> ent, ref ExaminedEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.PushMarkup(base.Loc.GetString("delivery-teleporter-amount-examine", (ValueTuple<string, object>)("amount", ent.Comp.ContainedDeliveryAmount)), 50);
	}

	private void OnUseInHand(Entity<DeliveryComponent> ent, ref UseInHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		if (!ent.Comp.IsOpened)
		{
			if (ent.Comp.IsLocked)
			{
				TryUnlockDelivery(ent, args.User);
			}
			else
			{
				OpenDelivery(ent, args.User);
			}
		}
	}

	private void OnGetDeliveryVerbs(Entity<DeliveryComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || args.Hands == null || ent.Comp.IsOpened || _hands.IsHolding(Entity<HandsComponent>.op_Implicit(args.User), Entity<DeliveryComponent>.op_Implicit(ent)))
		{
			return;
		}
		EntityUid user = args.User;
		args.Verbs.Add(new AlternativeVerb
		{
			Act = delegate
			{
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				if (ent.Comp.IsLocked)
				{
					TryUnlockDelivery(ent, user);
				}
				else
				{
					OpenDelivery(ent, user, attemptPickup: false);
				}
			},
			Text = (ent.Comp.IsLocked ? base.Loc.GetString("delivery-unlock-verb") : base.Loc.GetString("delivery-open-verb"))
		});
	}

	private void OnAttemptSimpleToolUse(Entity<DeliveryComponent> ent, ref AttemptSimpleToolUseEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.IsOpened || !ent.Comp.IsLocked)
		{
			args.Cancelled = true;
		}
	}

	private void OnSimpleToolUse(Entity<DeliveryComponent> ent, ref SimpleToolDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.IsOpened && !args.Cancelled)
		{
			HandlePenalty(ent);
			TryUnlockDelivery(ent, args.User, rewardMoney: false, force: true);
			OpenDelivery(ent, args.User, attemptPickup: false, force: true);
		}
	}

	private void OnGetSpawnerVerbs(Entity<DeliverySpawnerComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract || args.Hands == null)
		{
			return;
		}
		EntityUid user = args.User;
		args.Verbs.Add(new AlternativeVerb
		{
			Act = delegate
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				_audio.PlayPredicted(ent.Comp.OpenSound, ent.Owner, (EntityUid?)user, (AudioParams?)null);
				if (ent.Comp.ContainedDeliveryAmount == 0)
				{
					_popup.PopupPredicted(base.Loc.GetString("delivery-teleporter-empty", (ValueTuple<string, object>)("entity", ent)), null, Entity<DeliverySpawnerComponent>.op_Implicit(ent), user);
				}
				else
				{
					SpawnDeliveries(Entity<DeliverySpawnerComponent>.op_Implicit(ent.Owner));
					UpdateDeliverySpawnerVisuals(Entity<DeliverySpawnerComponent>.op_Implicit(ent), ent.Comp.ContainedDeliveryAmount);
				}
			},
			Text = base.Loc.GetString("delivery-teleporter-empty-verb")
		});
	}

	private bool TryUnlockDelivery(Entity<DeliveryComponent> ent, EntityUid user, bool rewardMoney = true, bool force = false)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		FingerprintReaderComponent reader = default(FingerprintReaderComponent);
		if (!force && ((EntitySystem)this).TryComp<FingerprintReaderComponent>(Entity<DeliveryComponent>.op_Implicit(ent), ref reader) && !_fingerprintReader.IsAllowed(Entity<FingerprintReaderComponent>.op_Implicit((Entity<DeliveryComponent>.op_Implicit(ent), reader)), user))
		{
			return false;
		}
		string deliveryName = _nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
		if (!force)
		{
			_audio.PlayPredicted(ent.Comp.UnlockSound, user, (EntityUid?)user, (AudioParams?)null);
		}
		ent.Comp.IsLocked = false;
		UpdateAntiTamperVisuals(Entity<DeliveryComponent>.op_Implicit(ent), ent.Comp.IsLocked);
		((EntitySystem)this).DirtyField<DeliveryComponent>(Entity<DeliveryComponent>.op_Implicit(ent), ent.Comp, "IsLocked", (MetaDataComponent)null);
		((EntitySystem)this).RemCompDeferred<SimpleToolUsageComponent>(Entity<DeliveryComponent>.op_Implicit(ent));
		DeliveryUnlockedEvent ev = new DeliveryUnlockedEvent(user);
		((EntitySystem)this).RaiseLocalEvent<DeliveryUnlockedEvent>(Entity<DeliveryComponent>.op_Implicit(ent), ref ev, false);
		if (rewardMoney)
		{
			GrantSpesoReward(ent.AsNullable());
		}
		if (!force)
		{
			_popup.PopupPredicted(base.Loc.GetString("delivery-unlocked-self", (ValueTuple<string, object>)("delivery", deliveryName)), base.Loc.GetString("delivery-unlocked-others", new(string, object)[3]
			{
				("delivery", deliveryName),
				("recipient", Identity.Name(user, (IEntityManager)(object)base.EntityManager)),
				("possadj", user)
			}), user, user);
		}
		return true;
	}

	private void OpenDelivery(Entity<DeliveryComponent> ent, EntityUid user, bool attemptPickup = true, bool force = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		string deliveryName = _nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
		_audio.PlayPredicted(ent.Comp.OpenSound, user, (EntityUid?)user, (AudioParams?)null);
		DeliveryOpenedEvent ev = new DeliveryOpenedEvent(user);
		((EntitySystem)this).RaiseLocalEvent<DeliveryOpenedEvent>(Entity<DeliveryComponent>.op_Implicit(ent), ref ev, false);
		if (attemptPickup)
		{
			_hands.TryDrop(Entity<HandsComponent>.op_Implicit(user), Entity<DeliveryComponent>.op_Implicit(ent));
		}
		ent.Comp.IsOpened = true;
		_appearance.SetData(Entity<DeliveryComponent>.op_Implicit(ent), (Enum)DeliveryVisuals.IsTrash, (object)ent.Comp.IsOpened, (AppearanceComponent)null);
		_tag.AddTags(Entity<DeliveryComponent>.op_Implicit(ent), TrashTag, RecyclableTag);
		((EntitySystem)this).EnsureComp<SpaceGarbageComponent>(Entity<DeliveryComponent>.op_Implicit(ent));
		((EntitySystem)this).RemCompDeferred<StealTargetComponent>(Entity<DeliveryComponent>.op_Implicit(ent));
		((EntitySystem)this).DirtyField<DeliveryComponent>(ent.Owner, ent.Comp, "IsOpened", (MetaDataComponent)null);
		if (!force)
		{
			_popup.PopupPredicted(base.Loc.GetString("delivery-opened-self", (ValueTuple<string, object>)("delivery", deliveryName)), base.Loc.GetString("delivery-opened-others", new(string, object)[3]
			{
				("delivery", deliveryName),
				("recipient", Identity.Name(user, (IEntityManager)(object)base.EntityManager)),
				("possadj", user)
			}), user, user);
		}
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<DeliveryComponent>.op_Implicit(ent), ent.Comp.Container, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		if (attemptPickup)
		{
			EntityUid[] array = container.ContainedEntities.ToArray();
			foreach (EntityUid entity in array)
			{
				_hands.PickupOrDrop(user, entity);
			}
		}
		else
		{
			_container.EmptyContainer(container, true, (EntityCoordinates?)null, true);
		}
	}

	private void UpdateAntiTamperVisuals(EntityUid uid, bool isLocked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)DeliveryVisuals.IsLocked, (object)isLocked, (AppearanceComponent)null);
		if (((EntitySystem)this).HasComp<DeliveryPriorityComponent>(uid))
		{
			_appearance.SetData(uid, (Enum)DeliveryVisuals.PriorityState, (object)DeliveryPriorityState.Inactive, (AppearanceComponent)null);
		}
	}

	public void UpdatePriorityVisuals(Entity<DeliveryPriorityComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		DeliveryComponent delivery = default(DeliveryComponent);
		if (((EntitySystem)this).TryComp<DeliveryComponent>(Entity<DeliveryPriorityComponent>.op_Implicit(ent), ref delivery) && delivery.IsLocked && !delivery.IsOpened)
		{
			_appearance.SetData(Entity<DeliveryPriorityComponent>.op_Implicit(ent), (Enum)DeliveryVisuals.PriorityState, (object)((!ent.Comp.Expired) ? DeliveryPriorityState.Active : DeliveryPriorityState.Inactive), (AppearanceComponent)null);
		}
	}

	public void UpdateBrokenVisuals(Entity<DeliveryFragileComponent> ent, bool isFragile)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<DeliveryFragileComponent>.op_Implicit(ent), (Enum)DeliveryVisuals.IsBroken, (object)ent.Comp.Broken, (AppearanceComponent)null);
		_appearance.SetData(Entity<DeliveryFragileComponent>.op_Implicit(ent), (Enum)DeliveryVisuals.IsFragile, (object)isFragile, (AppearanceComponent)null);
	}

	public void UpdateBombVisuals(Entity<DeliveryBombComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		bool isPrimed = ((EntitySystem)this).HasComp<PrimedDeliveryBombComponent>(Entity<DeliveryBombComponent>.op_Implicit(ent));
		_appearance.SetData(Entity<DeliveryBombComponent>.op_Implicit(ent), (Enum)DeliveryVisuals.IsBomb, (object)((!isPrimed) ? DeliveryBombState.Inactive : DeliveryBombState.Primed), (AppearanceComponent)null);
	}

	protected void UpdateDeliverySpawnerVisuals(EntityUid uid, int contents)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(uid, (Enum)DeliverySpawnerVisuals.Contents, (object)(contents > 0), (AppearanceComponent)null);
	}

	protected float GetDeliveryMultiplier(Entity<DeliveryComponent> ent)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GetDeliveryMultiplierEvent ev = new GetDeliveryMultiplierEvent();
		((EntitySystem)this).RaiseLocalEvent<GetDeliveryMultiplierEvent>(Entity<DeliveryComponent>.op_Implicit(ent), ref ev, false);
		return Math.Max(ev.AdditiveMultiplier * ev.MultiplicativeMultiplier, 0f);
	}

	protected virtual void GrantSpesoReward(Entity<DeliveryComponent?> ent)
	{
	}

	protected virtual void HandlePenalty(Entity<DeliveryComponent> ent, string? reason = null)
	{
	}

	protected virtual void SpawnDeliveries(Entity<DeliverySpawnerComponent?> ent)
	{
	}
}
