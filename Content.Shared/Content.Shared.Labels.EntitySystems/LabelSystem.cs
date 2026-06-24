using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Examine;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Labels.Components;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Paper;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared.Labels.EntitySystems;

public sealed class LabelSystem : EntitySystem
{
	[Dependency]
	private NameModifierSystem _nameModifier;

	[Dependency]
	private ItemSlotsSystem _itemSlots;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private CMExamineSystem _rmcExamine;

	public const string ContainerName = "paper_label";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<LabelComponent, MapInitEvent>((EntityEventRefHandler<LabelComponent, MapInitEvent>)OnLabelCompMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LabelComponent, ExaminedEvent>((EntityEventRefHandler<LabelComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LabelComponent, RefreshNameModifiersEvent>((EntityEventRefHandler<LabelComponent, RefreshNameModifiersEvent>)OnRefreshNameModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperLabelComponent, ComponentInit>((EntityEventRefHandler<PaperLabelComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperLabelComponent, ComponentRemove>((EntityEventRefHandler<PaperLabelComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperLabelComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<PaperLabelComponent, EntInsertedIntoContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperLabelComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<PaperLabelComponent, EntRemovedFromContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PaperLabelComponent, ExaminedEvent>((EntityEventRefHandler<PaperLabelComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnLabelCompMapInit(Entity<LabelComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(ent.Comp.CurrentLabel))
		{
			ent.Comp.CurrentLabel = base.Loc.GetString(ent.Comp.CurrentLabel);
			((EntitySystem)this).Dirty<LabelComponent>(ent, (MetaDataComponent)null);
		}
		_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
	}

	public void Label(EntityUid uid, string? text, MetaDataComponent? metadata = null, LabelComponent? label = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (label == null)
		{
			label = ((EntitySystem)this).EnsureComp<LabelComponent>(uid);
		}
		label.CurrentLabel = text;
		_nameModifier.RefreshNameModifiers(Entity<NameModifierComponent>.op_Implicit(uid));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)label, (MetaDataComponent)null);
	}

	private void OnExamine(Entity<LabelComponent> ent, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (_rmcExamine.CanExamine(Entity<BlockExamineComponent>.op_Implicit(ent.Owner), args.Examiner) && ent.Comp.Examinable && ent.Comp.CurrentLabel != null)
		{
			FormattedMessage message = new FormattedMessage();
			message.AddText(base.Loc.GetString("hand-labeler-has-label", (ValueTuple<string, object>)("label", ent.Comp.CurrentLabel)));
			args.PushMessage(message);
		}
	}

	private void OnRefreshNameModifiers(Entity<LabelComponent> entity, ref RefreshNameModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(entity.Comp.CurrentLabel))
		{
			args.AddModifier(LocId.op_Implicit("comp-label-format"), 0, ("label", entity.Comp.CurrentLabel));
		}
	}

	private void OnComponentInit(Entity<PaperLabelComponent> ent, ref ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		_itemSlots.AddItemSlot(Entity<PaperLabelComponent>.op_Implicit(ent), "paper_label", ent.Comp.LabelSlot);
		UpdateAppearance(Entity<PaperLabelComponent, AppearanceComponent>.op_Implicit(ent));
	}

	private void OnComponentRemove(Entity<PaperLabelComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_itemSlots.RemoveItemSlot(Entity<PaperLabelComponent>.op_Implicit(ent), ent.Comp.LabelSlot);
	}

	private void OnExamined(Entity<PaperLabelComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? item = ent.Comp.LabelSlot.Item;
		if (!item.HasValue)
		{
			return;
		}
		EntityUid item2 = item.GetValueOrDefault();
		if (!((EntityUid)(ref item2)).Valid)
		{
			return;
		}
		using (args.PushGroup("PaperLabelComponent"))
		{
			PaperComponent paper = default(PaperComponent);
			if (!args.IsInDetailsRange)
			{
				args.PushMarkup(base.Loc.GetString("comp-paper-label-has-label-cant-read"));
			}
			else if (((EntitySystem)this).TryComp<PaperComponent>(item2, ref paper))
			{
				if (string.IsNullOrWhiteSpace(paper.Content))
				{
					args.PushMarkup(base.Loc.GetString("comp-paper-label-has-label-blank"));
					return;
				}
				args.PushMarkup(base.Loc.GetString("comp-paper-label-has-label"));
				string text = paper.Content;
				args.PushMarkup(text.TrimEnd());
			}
		}
	}

	private void OnContainerModified(EntityUid uid, PaperLabelComponent label, ContainerModifiedMessage args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)label).Initialized && !(args.Container.ID != label.LabelSlot.ID))
		{
			UpdateAppearance(Entity<PaperLabelComponent, AppearanceComponent>.op_Implicit((uid, label)));
		}
	}

	private void UpdateAppearance(Entity<PaperLabelComponent, AppearanceComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AppearanceComponent>(Entity<PaperLabelComponent, AppearanceComponent>.op_Implicit(ent), ref ent.Comp2, false))
		{
			ItemSlot slot = ent.Comp1.LabelSlot;
			_appearance.SetData(Entity<PaperLabelComponent, AppearanceComponent>.op_Implicit(ent), (Enum)PaperLabelVisuals.HasLabel, (object)slot.HasItem, ent.Comp2);
			PaperLabelTypeComponent type = default(PaperLabelTypeComponent);
			if (((EntitySystem)this).TryComp<PaperLabelTypeComponent>(slot.Item, ref type))
			{
				_appearance.SetData(Entity<PaperLabelComponent, AppearanceComponent>.op_Implicit(ent), (Enum)PaperLabelVisuals.LabelType, (object)type.PaperType, ent.Comp2);
			}
		}
	}

	public bool TryGetLabel<T>(Entity<PaperLabelComponent?> ent, [NotNullWhen(true)] out Entity<T>? label) where T : Component
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		label = null;
		if (!((EntitySystem)this).Resolve<PaperLabelComponent>(Entity<PaperLabelComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return false;
		}
		EntityUid? item = ent.Comp.LabelSlot.Item;
		if (item.HasValue)
		{
			EntityUid labelEnt = item.GetValueOrDefault();
			T labelComp = default(T);
			if (!((EntitySystem)this).TryComp<T>(labelEnt, ref labelComp))
			{
				return false;
			}
			label = Entity<T>.op_Implicit((labelEnt, labelComp));
			return true;
		}
		return false;
	}
}
