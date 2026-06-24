using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Humanoid;
using Content.Shared._RMC14.UniformAccessories;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Item;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.UniformAccessories;

public sealed class UniformAccessorySystem : SharedUniformAccessorySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private RMCHumanoidAppearanceSystem _rmcHumanoid;

	[Dependency]
	private SpriteSystem _sprite;

	public event Action? PlayerMedalUpdated;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<UniformAccessoryHolderComponent, GetEquipmentVisualsEvent>((EntityEventRefHandler<UniformAccessoryHolderComponent, GetEquipmentVisualsEvent>)OnHolderGetEquipmentVisuals, (Type[])null, new Type[1] { typeof(ClothingSystem) });
		((EntitySystem)this).SubscribeLocalEvent<UniformAccessoryHolderComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<UniformAccessoryHolderComponent, AfterAutoHandleStateEvent>)OnHolderAfterState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UniformAccessoryHolderComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<UniformAccessoryHolderComponent, EntInsertedIntoContainerMessage>)OnHolderInsertedContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UniformAccessoryHolderComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<UniformAccessoryHolderComponent, EntRemovedFromContainerMessage>)OnHolderRemovedContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UniformAccessoryHolderComponent, EquipmentVisualsUpdatedEvent>((EntityEventRefHandler<UniformAccessoryHolderComponent, EquipmentVisualsUpdatedEvent>)OnHolderVisualsUpdated, (Type[])null, new Type[1] { typeof(ClothingSystem) });
	}

	private void OnHolderGetEquipmentVisuals(Entity<UniformAccessoryHolderComponent> ent, ref GetEquipmentVisualsEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		if (_rmcHumanoid.HidePlayerIdentities && ((EntitySystem)this).HasComp<XenoComponent>(((ISharedPlayerManager)_player).LocalEntity))
		{
			return;
		}
		SpriteComponent val = ((EntitySystem)this).CompOrNull<SpriteComponent>(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
		BaseContainer val2 = default(BaseContainer);
		if (!_container.TryGetContainer(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref val2, (ContainerManagerComponent)null))
		{
			return;
		}
		int num = 0;
		UniformAccessoryComponent uniformAccessoryComponent = default(UniformAccessoryComponent);
		SpriteComponent val3 = default(SpriteComponent);
		foreach (EntityUid containedEntity in val2.ContainedEntities)
		{
			if (!((EntitySystem)this).TryComp<UniformAccessoryComponent>(containedEntity, ref uniformAccessoryComponent))
			{
				continue;
			}
			string layer = GetKey(containedEntity, uniformAccessoryComponent, num);
			if (uniformAccessoryComponent.PlayerSprite == null && ((EntitySystem)this).TryComp<SpriteComponent>(containedEntity, ref val3))
			{
				UniformAccessoryComponent uniformAccessoryComponent2 = uniformAccessoryComponent;
				RSI baseRSI = val3.BaseRSI;
				uniformAccessoryComponent2.PlayerSprite = new Rsi((ResPath)((baseRSI != null) ? baseRSI.Path : new ResPath("_RMC14/Objects/Medals/bronze.rsi")), "equipped");
			}
			Rsi playerSprite = uniformAccessoryComponent.PlayerSprite;
			if (playerSprite != null && (!ent.Comp.HideAccessories || !uniformAccessoryComponent.HiddenByJacketRolling))
			{
				if (val != null && uniformAccessoryComponent.HasIconSprite)
				{
					int num2 = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), layer);
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), num2, !uniformAccessoryComponent.Hidden);
					_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), num2, playerSprite.RsiPath, (StateId?)null);
					_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), num2, StateId.op_Implicit(playerSprite.RsiState));
				}
				if (!args.Layers.Any<(string, PrototypeLayerData)>(((string, PrototypeLayerData) t) => t.Item1 == layer))
				{
					args.Layers.Add((layer, new PrototypeLayerData
					{
						RsiPath = ((object)playerSprite.RsiPath/*cast due to constrained. prefix*/).ToString(),
						State = playerSprite.RsiState,
						Visible = !uniformAccessoryComponent.Hidden
					}));
					num++;
				}
			}
		}
		this.PlayerMedalUpdated?.Invoke();
	}

	private void OnHolderAfterState(Entity<UniformAccessoryHolderComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
	}

	private void OnHolderInsertedContainer(Entity<UniformAccessoryHolderComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
	}

	private void OnHolderRemovedContainer(Entity<UniformAccessoryHolderComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((ContainerModifiedMessage)args).Entity;
		UniformAccessoryComponent component = default(UniformAccessoryComponent);
		if (!((EntitySystem)this).TryComp<UniformAccessoryComponent>(entity, ref component))
		{
			return;
		}
		int num = 0;
		using (IEnumerator<EntityUid> enumerator = ((ContainerModifiedMessage)args).Container.ContainedEntities.GetEnumerator())
		{
			while (enumerator.MoveNext() && !(enumerator.Current == entity))
			{
				num++;
			}
		}
		string key = GetKey(entity, component, num);
		SpriteComponent item = default(SpriteComponent);
		int num2 = default(int);
		if (((EntitySystem)this).TryComp<SpriteComponent>(ent.Owner, ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), key, ref num2, false))
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), num2, false);
		}
		_item.VisualsChanged(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent));
	}

	private void OnHolderVisualsUpdated(Entity<UniformAccessoryHolderComponent> ent, ref EquipmentVisualsUpdatedEvent args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer val = default(BaseContainer);
		if ((_rmcHumanoid.HidePlayerIdentities && ((EntitySystem)this).HasComp<XenoComponent>(((ISharedPlayerManager)_player).LocalEntity)) || !_container.TryGetContainer(Entity<UniformAccessoryHolderComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref val, (ContainerManagerComponent)null))
		{
			return;
		}
		string text = string.Empty;
		UniformAccessoryComponent uniformAccessoryComponent = default(UniformAccessoryComponent);
		SpriteComponent val2 = default(SpriteComponent);
		foreach (EntityUid containedEntity in val.ContainedEntities)
		{
			if (!((EntitySystem)this).TryComp<UniformAccessoryComponent>(containedEntity, ref uniformAccessoryComponent))
			{
				return;
			}
			if (uniformAccessoryComponent.PlayerSprite == null && ((EntitySystem)this).TryComp<SpriteComponent>(containedEntity, ref val2))
			{
				UniformAccessoryComponent uniformAccessoryComponent2 = uniformAccessoryComponent;
				RSI baseRSI = val2.BaseRSI;
				uniformAccessoryComponent2.PlayerSprite = new Rsi((ResPath)((baseRSI != null) ? baseRSI.Path : new ResPath("_RMC14/Objects/Medals/bronze.rsi")), "equipped");
			}
			if (uniformAccessoryComponent.LayerKey != null)
			{
				text = uniformAccessoryComponent.LayerKey;
			}
		}
		SpriteComponent item = default(SpriteComponent);
		Layer val3 = default(Layer);
		int num = default(int);
		if (!(text == string.Empty) && args.RevealedLayers.Contains(text) && ((EntitySystem)this).TryComp<SpriteComponent>(args.Equipee, ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((args.Equipee, item)), text, ref num, false) && _sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((args.Equipee, item)), num, ref val3, false))
		{
			PrototypeLayerData val4 = val3.ToPrototypeData();
			_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((args.Equipee, item)), num, true);
			num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((args.Equipee, item)), text);
			_sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((args.Equipee, item)), num, val4);
		}
	}

	private string GetKey(EntityUid uid, UniformAccessoryComponent component, int index)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		string result = $"enum.{"UniformAccessoryLayer"}.{UniformAccessoryLayer.Base}{index}_{((EntitySystem)this).Name(uid, (MetaDataComponent)null)}_{uid.Id}";
		if (component.LayerKey != null)
		{
			result = component.LayerKey;
		}
		return result;
	}
}
