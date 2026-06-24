using System;
using Content.Client.Clothing;
using Content.Client.Items.Systems;
using Content.Shared._RMC14.Webbing;
using Content.Shared.Clothing;
using Content.Shared.Inventory.Events;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Webbing;

public sealed class WebbingSystem : SharedWebbingSystem
{
	[Dependency]
	private ItemSystem _item;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SpriteSystem _sprite;

	public event Action? PlayerWebbingUpdated;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<WebbingClothingComponent, AfterAutoHandleStateEvent>)OnClothingState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, GetEquipmentVisualsEvent>((EntityEventRefHandler<WebbingClothingComponent, GetEquipmentVisualsEvent>)OnWebbingClothingEquipmentVisuals, (Type[])null, new Type[1] { typeof(ClientClothingSystem) });
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, GotEquippedEvent>((EntityEventRefHandler<WebbingClothingComponent, GotEquippedEvent>)OnClothingEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingClothingComponent, GotUnequippedEvent>((EntityEventRefHandler<WebbingClothingComponent, GotUnequippedEvent>)OnClothingUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WebbingTransferComponent, ComponentRemove>((EntityEventRefHandler<WebbingTransferComponent, ComponentRemove>)OnWebbingTransferRemove, (Type[])null, (Type[])null);
	}

	private void OnWebbingClothingEquipmentVisuals(Entity<WebbingClothingComponent> ent, ref GetEquipmentVisualsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		WebbingComponent webbingComponent = default(WebbingComponent);
		if (!((EntitySystem)this).TryComp<WebbingComponent>(ent.Comp.Webbing, ref webbingComponent))
		{
			return;
		}
		SpriteComponent val = default(SpriteComponent);
		if (webbingComponent.PlayerSprite == null && ((EntitySystem)this).TryComp<SpriteComponent>(ent.Comp.Webbing, ref val))
		{
			WebbingComponent webbingComponent2 = webbingComponent;
			RSI baseRSI = val.BaseRSI;
			webbingComponent2.PlayerSprite = new Rsi((ResPath)((baseRSI != null) ? baseRSI.Path : new ResPath("_RMC14/Objects/Clothing/Webbing/webbing.rsi")), "equipped");
		}
		Rsi playerSprite = webbingComponent.PlayerSprite;
		if (playerSprite != null)
		{
			SpriteComponent item = default(SpriteComponent);
			int num = default(int);
			if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<WebbingClothingComponent>.op_Implicit(ent), ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), (Enum)WebbingVisualLayers.Base, ref num, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), num, true);
				_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), num, playerSprite.RsiPath, (StateId?)null);
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), num, StateId.op_Implicit(playerSprite.RsiState));
			}
			args.Layers.Add(("enum.WebbingVisualLayers.Base", new PrototypeLayerData
			{
				RsiPath = playerSprite.RsiPath.CanonPath,
				State = playerSprite.RsiState
			}));
		}
	}

	private void OnClothingState(Entity<WebbingClothingComponent> clothing, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<WebbingClothingComponent>.op_Implicit(clothing), ref item) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((clothing.Owner, item)), (Enum)WebbingVisualLayers.Base, ref num, false))
		{
			WebbingComponent webbingComponent = default(WebbingComponent);
			if (((EntitySystem)this).TryComp<WebbingComponent>(clothing.Comp.Webbing, ref webbingComponent))
			{
				Rsi playerSprite = webbingComponent.PlayerSprite;
				if (playerSprite != null)
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((clothing.Owner, item)), num, true);
					_sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((clothing.Owner, item)), num, playerSprite.RsiPath, (StateId?)null);
					_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((clothing.Owner, item)), num, StateId.op_Implicit(playerSprite.RsiState));
					goto IL_00ef;
				}
			}
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((clothing.Owner, item)), num, false);
		}
		goto IL_00ef;
		IL_00ef:
		_item.VisualsChanged(Entity<WebbingClothingComponent>.op_Implicit(clothing));
		this.PlayerWebbingUpdated?.Invoke();
	}

	private void OnClothingEquipped(Entity<WebbingClothingComponent> clothing, ref GotEquippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid equipee = args.Equipee;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == equipee)
		{
			this.PlayerWebbingUpdated?.Invoke();
		}
	}

	private void OnClothingUnequipped(Entity<WebbingClothingComponent> clothing, ref GotUnequippedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid equipee = args.Equipee;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == equipee)
		{
			this.PlayerWebbingUpdated?.Invoke();
		}
	}

	protected override void OnClothingInserted(Entity<WebbingClothingComponent> clothing, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.OnClothingInserted(clothing, ref args);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid owner = ((ContainerModifiedMessage)args).Container.Owner;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == owner)
		{
			this.PlayerWebbingUpdated?.Invoke();
		}
	}

	protected override void OnClothingRemoved(Entity<WebbingClothingComponent> clothing, ref EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.OnClothingRemoved(clothing, ref args);
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid owner = ((ContainerModifiedMessage)args).Container.Owner;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == owner)
		{
			this.PlayerWebbingUpdated?.Invoke();
		}
	}

	private void OnWebbingTransferRemove(Entity<WebbingTransferComponent> ent, ref ComponentRemove args)
	{
		this.PlayerWebbingUpdated?.Invoke();
	}
}
