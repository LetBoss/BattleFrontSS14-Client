using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Access.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.Contraband;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Clothing.EntitySystems;

public abstract class SharedChameleonClothingSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private ClothingSystem _clothingSystem;

	[Dependency]
	private ContrabandSystem _contraband;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private SharedItemSystem _itemSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	protected IGameTiming _timing;

	private static readonly SlotFlags[] IgnoredSlots = new SlotFlags[3]
	{
		SlotFlags.All,
		SlotFlags.PREVENTEQUIP,
		SlotFlags.NONE
	};

	private static readonly SlotFlags[] Slots = Enum.GetValues<SlotFlags>().Except(IgnoredSlots).ToArray();

	private readonly Dictionary<SlotFlags, List<EntProtoId>> _data = new Dictionary<SlotFlags, List<EntProtoId>>();

	public readonly Dictionary<SlotFlags, List<string>> ValidVariants = new Dictionary<SlotFlags, List<string>>();

	[Dependency]
	protected SharedUserInterfaceSystem UI;

	private static readonly ProtoId<TagPrototype> WhitelistChameleonTag = ProtoId<TagPrototype>.op_Implicit("WhitelistChameleon");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ChameleonClothingComponent, GotEquippedEvent>((ComponentEventHandler<ChameleonClothingComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonClothingComponent, GotUnequippedEvent>((ComponentEventHandler<ChameleonClothingComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonClothingComponent, GetVerbsEvent<InteractionVerb>>((EntityEventRefHandler<ChameleonClothingComponent, GetVerbsEvent<InteractionVerb>>)OnVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonClothingComponent, PrototypesReloadedEventArgs>((ComponentEventHandler<ChameleonClothingComponent, PrototypesReloadedEventArgs>)OnPrototypeReload, (Type[])null, (Type[])null);
		PrepareAllVariants();
	}

	private void OnPrototypeReload(EntityUid uid, ChameleonClothingComponent component, PrototypesReloadedEventArgs args)
	{
		PrepareAllVariants();
	}

	private void OnGotEquipped(EntityUid uid, ChameleonClothingComponent component, GotEquippedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		component.User = args.Equipee;
	}

	private void OnGotUnequipped(EntityUid uid, ChameleonClothingComponent component, GotUnequippedEvent args)
	{
		component.User = null;
	}

	protected void UpdateVisuals(EntityUid uid, ChameleonClothingComponent component)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId? val = component.Default;
		EntityPrototype proto = default(EntityPrototype);
		if (!string.IsNullOrEmpty(val.HasValue ? EntProtoId.op_Implicit(val.GetValueOrDefault()) : null) && _proto.TryIndex(component.Default, ref proto))
		{
			UpdateSprite(uid, proto);
			if (!((EntitySystem)this).HasComp<IdCardComponent>(uid))
			{
				MetaDataComponent meta = ((EntitySystem)this).MetaData(uid);
				_metaData.SetEntityName(uid, proto.Name, meta, true);
				_metaData.SetEntityDescription(uid, proto.Description, meta);
			}
			ItemComponent item = default(ItemComponent);
			ItemComponent otherItem = default(ItemComponent);
			if (((EntitySystem)this).TryComp<ItemComponent>(uid, ref item) && proto.TryGetComponent<ItemComponent>(ref otherItem, ((EntitySystem)this).Factory))
			{
				_itemSystem.CopyVisuals(uid, otherItem, item);
			}
			ClothingComponent clothing = default(ClothingComponent);
			ClothingComponent otherClothing = default(ClothingComponent);
			if (((EntitySystem)this).TryComp<ClothingComponent>(uid, ref clothing) && proto.TryGetComponent<ClothingComponent>("Clothing", ref otherClothing))
			{
				_clothingSystem.CopyVisuals(uid, otherClothing, clothing);
			}
			AppearanceComponent appearance = default(AppearanceComponent);
			AppearanceComponent appearanceOther = default(AppearanceComponent);
			if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance) && proto.TryGetComponent<AppearanceComponent>("Appearance", ref appearanceOther))
			{
				_appearance.AppendData(appearanceOther, Entity<AppearanceComponent>.op_Implicit(uid));
				((EntitySystem)this).Dirty(uid, (IComponent)(object)appearance, (MetaDataComponent)null);
			}
			ContrabandComponent contra = default(ContrabandComponent);
			if (proto.TryGetComponent<ContrabandComponent>("Contraband", ref contra))
			{
				ContrabandComponent current = default(ContrabandComponent);
				((EntitySystem)this).EnsureComp<ContrabandComponent>(uid, ref current);
				_contraband.CopyDetails(uid, contra, current);
			}
			else
			{
				((EntitySystem)this).RemComp<ContrabandComponent>(uid);
			}
		}
	}

	private void OnVerb(Entity<ChameleonClothingComponent> ent, ref GetVerbsEvent<InteractionVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		if (!args.CanAccess || !args.CanInteract)
		{
			return;
		}
		EntityUid? user = ent.Comp.User;
		EntityUid user2 = args.User;
		if (user.HasValue && !(user.GetValueOrDefault() != user2))
		{
			EntityUid user3 = args.User;
			args.Verbs.Add(new InteractionVerb
			{
				Text = base.Loc.GetString("chameleon-component-verb-text"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png")),
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					UI.TryToggleUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)ChameleonUiKey.Key, user3);
				}
			});
		}
	}

	protected virtual void UpdateSprite(EntityUid uid, EntityPrototype proto)
	{
	}

	public bool IsValidTarget(EntityPrototype proto, SlotFlags chameleonSlot = SlotFlags.NONE, string? requiredTag = null)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (proto.Abstract || proto.HideSpawnMenu)
		{
			return false;
		}
		TagComponent tag = default(TagComponent);
		if (!proto.TryGetComponent<TagComponent>(ref tag, ((EntitySystem)this).Factory) || !_tag.HasTag(tag, WhitelistChameleonTag))
		{
			return false;
		}
		if (requiredTag != null && !_tag.HasTag(tag, ProtoId<TagPrototype>.op_Implicit(requiredTag)))
		{
			return false;
		}
		ClothingComponent clothing = default(ClothingComponent);
		if (!proto.TryGetComponent<ClothingComponent>("Clothing", ref clothing))
		{
			return false;
		}
		if (!clothing.Slots.HasFlag(chameleonSlot))
		{
			return false;
		}
		return true;
	}

	public IEnumerable<EntProtoId> GetValidTargets(SlotFlags slot, string? tag = null)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		List<EntProtoId> validTargets = new List<EntProtoId>();
		if (tag != null)
		{
			foreach (EntProtoId proto in _data[slot])
			{
				if (IsValidTarget(_proto.Index(proto), slot, tag))
				{
					validTargets.Add(proto);
				}
			}
		}
		else
		{
			validTargets = _data[slot];
		}
		return validTargets;
	}

	protected void PrepareAllVariants()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		_data.Clear();
		ClothingComponent item = default(ClothingComponent);
		foreach (EntityPrototype proto in _proto.EnumeratePrototypes<EntityPrototype>())
		{
			if (!IsValidTarget(proto) || !proto.TryGetComponent<ClothingComponent>(ref item, ((EntitySystem)this).Factory))
			{
				continue;
			}
			SlotFlags[] slots = Slots;
			foreach (SlotFlags slot in slots)
			{
				if (item.Slots.HasFlag(slot))
				{
					if (!_data.ContainsKey(slot))
					{
						_data.Add(slot, new List<EntProtoId>());
					}
					_data[slot].Add(EntProtoId.op_Implicit(proto.ID));
				}
			}
		}
	}
}
