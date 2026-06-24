using System;
using System.Numerics;
using Content.Shared._RMC14.Mimicry;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Maps;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Mimicry;

public sealed class MimicryVisualizerSystem : EntitySystem
{
	private const string SetSlot = "outerClothing";

	[Dependency]
	private readonly IPrototypeManager _proto;

	[Dependency]
	private readonly IResourceCache _resCache;

	[Dependency]
	private readonly SharedItemSystem _item;

	[Dependency]
	private readonly SpriteSystem _sprite;

	[Dependency]
	private readonly InventorySystem _inventory;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, EquipmentVisualsUpdatedEvent>((EntityEventRefHandler<ClothingComponent, EquipmentVisualsUpdatedEvent>)OnVisualsUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MimicryComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<MimicryComponent, AfterAutoHandleStateEvent>)OnState, (Type[])null, (Type[])null);
	}

	private void OnState(Entity<MimicryComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		EntityUid parentUid = ((EntitySystem)this).Transform(Entity<MimicryComponent>.op_Implicit(ent)).ParentUid;
		if (((EntitySystem)this).Exists(parentUid))
		{
			InventorySystem.InventorySlotEnumerator slotEnumerator = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(parentUid));
			EntityUid item;
			while (slotEnumerator.NextItem(out item))
			{
				_item.VisualsChanged(item);
			}
		}
	}

	private void OnVisualsUpdated(Entity<ClothingComponent> clothing, ref EquipmentVisualsUpdatedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		MimicryComponent mimicryComponent = default(MimicryComponent);
		SpriteComponent val = default(SpriteComponent);
		if (!_inventory.TryGetSlotEntity(args.Equipee, "outerClothing", out var entityUid) || !((EntitySystem)this).TryComp<MimicryComponent>(entityUid, ref mimicryComponent) || !((EntitySystem)this).TryComp<SpriteComponent>(args.Equipee, ref val))
		{
			return;
		}
		int num = default(int);
		if (mimicryComponent.HoodDown && _inventory.TryGetSlotEntity(args.Equipee, mimicryComponent.HoodSlot, out var entityUid2))
		{
			EntityUid? val2 = entityUid2;
			EntityUid owner = clothing.Owner;
			if (val2.HasValue && val2.GetValueOrDefault() == owner)
			{
				foreach (string revealedLayer in args.RevealedLayers)
				{
					if (val.LayerMapTryGet((object)revealedLayer, ref num, false))
					{
						val.LayerSetVisible((object)revealedLayer, false);
					}
				}
				return;
			}
		}
		ProtoId<ContentTileDefinition>? mimickedTile = mimicryComponent.MimickedTile;
		if (!mimickedTile.HasValue)
		{
			return;
		}
		ProtoId<ContentTileDefinition> valueOrDefault = mimickedTile.GetValueOrDefault();
		ContentTileDefinition contentTileDefinition = default(ContentTileDefinition);
		if (mimicryComponent.ExcludedSlots.Contains(args.Slot) || !_proto.TryIndex<ContentTileDefinition>(valueOrDefault, ref contentTileDefinition))
		{
			return;
		}
		ResPath? sprite = contentTileDefinition.Sprite;
		if (!sprite.HasValue)
		{
			return;
		}
		ResPath valueOrDefault2 = sprite.GetValueOrDefault();
		TextureResource val3 = default(TextureResource);
		if (!_resCache.TryGetResource<TextureResource>(valueOrDefault2, ref val3))
		{
			return;
		}
		Texture texture = val3.Texture;
		int num2 = ((texture.Height == 0) ? 1 : Math.Max(1, texture.Width / texture.Height));
		Vector4 vector = new Vector4(0f, 0f, 1f / (float)num2, 1f);
		ShaderInstance val4 = _proto.Index<ShaderPrototype>("Mimicry").InstanceUnique();
		val4.SetParameter("tileTex", texture);
		val4.SetParameter("tileRegion", vector);
		foreach (string revealedLayer2 in args.RevealedLayers)
		{
			if (val.LayerMapTryGet((object)revealedLayer2, ref num, false))
			{
				val.LayerSetShader((object)revealedLayer2, val4, (string)null);
			}
		}
	}
}
