using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Paper;

public sealed class EnvelopeSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private ItemSlotsSystem _itemSlotsSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EnvelopeComponent, ItemSlotInsertAttemptEvent>((EntityEventRefHandler<EnvelopeComponent, ItemSlotInsertAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnvelopeComponent, ItemSlotEjectAttemptEvent>((EntityEventRefHandler<EnvelopeComponent, ItemSlotEjectAttemptEvent>)OnEjectAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnvelopeComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<EnvelopeComponent, GetVerbsEvent<AlternativeVerb>>)OnGetAltVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnvelopeComponent, EnvelopeDoAfterEvent>((EntityEventRefHandler<EnvelopeComponent, EnvelopeDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EnvelopeComponent, ExaminedEvent>((EntityEventRefHandler<EnvelopeComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
	}

	private void OnExamine(Entity<EnvelopeComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed)
		{
			args.PushMarkup(base.Loc.GetString("envelope-sealed-examine", (ValueTuple<string, object>)("envelope", ent.Owner)));
		}
		else if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Torn)
		{
			args.PushMarkup(base.Loc.GetString("envelope-torn-examine", (ValueTuple<string, object>)("envelope", ent.Owner)));
		}
	}

	private void OnGetAltVerbs(Entity<EnvelopeComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Hands != null && ent.Comp.State != EnvelopeComponent.EnvelopeState.Torn)
		{
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString((ent.Comp.State == EnvelopeComponent.EnvelopeState.Open) ? "envelope-verb-seal" : "envelope-verb-tear"),
				IconEntity = ((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					TryStartDoAfter(ent, user, (ent.Comp.State == EnvelopeComponent.EnvelopeState.Open) ? ent.Comp.SealDelay : ent.Comp.TearDelay);
				}
			});
		}
	}

	private void OnInsertAttempt(Entity<EnvelopeComponent> ent, ref ItemSlotInsertAttemptEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Cancelled |= ent.Comp.State != EnvelopeComponent.EnvelopeState.Open;
	}

	private void OnEjectAttempt(Entity<EnvelopeComponent> ent, ref ItemSlotEjectAttemptEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Cancelled |= ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed;
	}

	private void TryStartDoAfter(Entity<EnvelopeComponent> ent, EntityUid user, TimeSpan delay)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.EnvelopeDoAfter.HasValue)
		{
			DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, new EnvelopeDoAfterEvent(), ent.Owner, ent.Owner)
			{
				BreakOnDamage = true,
				NeedHand = true,
				BreakOnHandChange = true,
				MovementThreshold = 0.01f,
				DistanceThreshold = 1f
			};
			if (_doAfterSystem.TryStartDoAfter(doAfterEventArgs, out var doAfterId))
			{
				ent.Comp.EnvelopeDoAfter = doAfterId;
			}
		}
	}

	private void OnDoAfter(Entity<EnvelopeComponent> ent, ref EnvelopeDoAfterEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.EnvelopeDoAfter = null;
		if (args.Cancelled)
		{
			return;
		}
		if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Open)
		{
			_audioSystem.PlayPredicted((SoundSpecifier)(object)ent.Comp.SealSound, ent.Owner, (EntityUid?)args.User, (AudioParams?)null);
			ent.Comp.State = EnvelopeComponent.EnvelopeState.Sealed;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
		else if (ent.Comp.State == EnvelopeComponent.EnvelopeState.Sealed)
		{
			_audioSystem.PlayPredicted((SoundSpecifier)(object)ent.Comp.TearSound, ent.Owner, (EntityUid?)args.User, (AudioParams?)null);
			ent.Comp.State = EnvelopeComponent.EnvelopeState.Torn;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			if (_itemSlotsSystem.TryGetSlot(ent.Owner, ent.Comp.SlotId, out ItemSlot slotComp))
			{
				_itemSlotsSystem.TryEjectToHands(ent.Owner, slotComp, args.User);
			}
		}
	}
}
