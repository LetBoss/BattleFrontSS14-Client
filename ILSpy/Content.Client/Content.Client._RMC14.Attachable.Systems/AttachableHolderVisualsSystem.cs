using System;
using Content.Client._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Attachable.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Attachable.Systems;

public sealed class AttachableHolderVisualsSystem : EntitySystem
{
	[Dependency]
	private AttachableHolderSystem _attachableHolderSystem;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderVisualsComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<AttachableHolderVisualsComponent, EntRemovedFromContainerMessage>)OnDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableHolderVisualsComponent, AttachableHolderAttachablesAlteredEvent>((EntityEventRefHandler<AttachableHolderVisualsComponent, AttachableHolderAttachablesAlteredEvent>)OnAttachablesAltered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AttachableVisualsComponent, AppearanceChangeEvent>((EntityEventRefHandler<AttachableVisualsComponent, AppearanceChangeEvent>)OnAttachableAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnDetached(Entity<AttachableHolderVisualsComponent> holder, ref EntRemovedFromContainerMessage args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<AttachableVisualsComponent>(((ContainerModifiedMessage)args).Entity) && _attachableHolderSystem.HasSlot(Entity<AttachableHolderComponent>.op_Implicit(holder.Owner), ((ContainerModifiedMessage)args).Container.ID))
		{
			AttachableHolderAttachablesAlteredEvent attachableHolderAttachablesAlteredEvent = new AttachableHolderAttachablesAlteredEvent(((ContainerModifiedMessage)args).Entity, ((ContainerModifiedMessage)args).Container.ID, AttachableAlteredType.Detached);
			((EntitySystem)this).RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(Entity<AttachableHolderVisualsComponent>.op_Implicit(holder), ref attachableHolderAttachablesAlteredEvent, false);
		}
	}

	private void OnAttachablesAltered(Entity<AttachableHolderVisualsComponent> holder, ref AttachableHolderAttachablesAlteredEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		AttachableVisualsComponent attachableVisualsComponent = default(AttachableVisualsComponent);
		if (!((EntitySystem)this).TryComp<AttachableVisualsComponent>(args.Attachable, ref attachableVisualsComponent))
		{
			return;
		}
		string suffix = "";
		AttachableToggleableComponent attachableToggleableComponent = default(AttachableToggleableComponent);
		if (attachableVisualsComponent.ShowActive && ((EntitySystem)this).TryComp<AttachableToggleableComponent>(args.Attachable, ref attachableToggleableComponent) && attachableToggleableComponent.Active)
		{
			suffix = "-on";
		}
		Entity<AttachableVisualsComponent> attachable = default(Entity<AttachableVisualsComponent>);
		attachable._002Ector(args.Attachable, attachableVisualsComponent);
		switch (args.Alteration)
		{
		case AttachableAlteredType.Attached:
			SetAttachableOverlay(holder, attachable, args.SlotId, suffix);
			break;
		case AttachableAlteredType.Detached:
			RemoveAttachableOverlay(holder, args.SlotId);
			break;
		case AttachableAlteredType.Activated:
			if (attachableVisualsComponent.ShowActive)
			{
				SetAttachableOverlay(holder, attachable, args.SlotId, suffix);
			}
			break;
		case AttachableAlteredType.Deactivated:
			if (attachableVisualsComponent.ShowActive)
			{
				SetAttachableOverlay(holder, attachable, args.SlotId, suffix);
			}
			break;
		case AttachableAlteredType.Interrupted:
			if (attachableVisualsComponent.ShowActive)
			{
				SetAttachableOverlay(holder, attachable, args.SlotId);
			}
			break;
		case AttachableAlteredType.AppearanceChanged:
			SetAttachableOverlay(holder, attachable, args.SlotId, suffix);
			break;
		}
	}

	private void RemoveAttachableOverlay(Entity<AttachableHolderVisualsComponent> holder, string slotId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (holder.Comp.Offsets.ContainsKey(slotId) && ((EntitySystem)this).TryComp<SpriteComponent>(Entity<AttachableHolderVisualsComponent>.op_Implicit(holder), ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((holder.Owner, item)), slotId, ref num, false))
		{
			_sprite.LayerMapRemove(Entity<SpriteComponent>.op_Implicit((holder.Owner, item)), slotId);
			_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((holder.Owner, item)), num, true);
		}
	}

	private void SetAttachableOverlay(Entity<AttachableHolderVisualsComponent> holder, Entity<AttachableVisualsComponent> attachable, string slotId, string suffix = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		RefreshVisuals(holder, attachable, slotId, suffix);
	}

	private void OnAttachableAppearanceChange(Entity<AttachableVisualsComponent> attachable, ref AppearanceChangeEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (attachable.Comp.RedrawOnAppearanceChange && _attachableHolderSystem.TryGetHolder(attachable.Owner, out var holderUid) && _attachableHolderSystem.TryGetSlotId(holderUid.Value, attachable.Owner, out string slotId))
		{
			AttachableHolderAttachablesAlteredEvent attachableHolderAttachablesAlteredEvent = new AttachableHolderAttachablesAlteredEvent(attachable.Owner, slotId, AttachableAlteredType.AppearanceChanged);
			((EntitySystem)this).RaiseLocalEvent<AttachableHolderAttachablesAlteredEvent>(holderUid.Value, ref attachableHolderAttachablesAlteredEvent, false);
		}
	}

	public void RefreshVisuals(Entity<AttachableHolderVisualsComponent> holder, Entity<AttachableVisualsComponent> attachable, string slotId, string suffix)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		SpriteComponent item2 = default(SpriteComponent);
		if (holder.Comp.Offsets.ContainsKey(slotId) && ((EntitySystem)this).TryComp<SpriteComponent>(Entity<AttachableHolderVisualsComponent>.op_Implicit(holder), ref item) && ((EntitySystem)this).TryComp<SpriteComponent>(Entity<AttachableVisualsComponent>.op_Implicit(attachable), ref item2))
		{
			attachable.Comp.LastSlotId = slotId;
			attachable.Comp.LastSuffix = suffix;
			RSI obj = _sprite.LayerGetEffectiveRsi(Entity<SpriteComponent>.op_Implicit((attachable.Owner, item2)), attachable.Comp.Layer);
			ResPath? val = ((obj != null) ? new ResPath?(obj.Path) : ((ResPath?)null));
			string text = ((object)_sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((attachable.Owner, item2)), attachable.Comp.Layer)/*cast due to constrained. prefix*/).ToString();
			ResPath? rsi = attachable.Comp.Rsi;
			if (rsi.HasValue)
			{
				ResPath valueOrDefault = rsi.GetValueOrDefault();
				val = valueOrDefault;
			}
			if (!string.IsNullOrWhiteSpace(attachable.Comp.Prefix))
			{
				text = attachable.Comp.Prefix;
			}
			if (attachable.Comp.IncludeSlotName)
			{
				text += slotId;
			}
			if (!string.IsNullOrWhiteSpace(attachable.Comp.Suffix))
			{
				text += attachable.Comp.Suffix;
			}
			text += suffix;
			PrototypeLayerData val2 = new PrototypeLayerData
			{
				RsiPath = val.ToString(),
				State = text,
				Offset = holder.Comp.Offsets[slotId] + attachable.Comp.Offset,
				Visible = true
			};
			int num = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((holder.Owner, item)), slotId, ref num, false))
			{
				_sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((holder.Owner, item)), num, val2);
				return;
			}
			num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((holder.Owner, item)), slotId);
			_sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((holder.Owner, item)), num, val2);
		}
	}
}
