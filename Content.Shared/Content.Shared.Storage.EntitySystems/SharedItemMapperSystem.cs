using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Storage.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Storage.EntitySystems;

public abstract class SharedItemMapperSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemMapperComponent, ComponentInit>((ComponentEventHandler<ItemMapperComponent, ComponentInit>)InitLayers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemMapperComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<ItemMapperComponent, EntInsertedIntoContainerMessage>)MapperEntityInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemMapperComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<ItemMapperComponent, EntRemovedFromContainerMessage>)MapperEntityRemoved, (Type[])null, (Type[])null);
	}

	private void InitLayers(EntityUid uid, ItemMapperComponent component, ComponentInit args)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, SharedMapLayerData> mapLayer in component.MapLayers)
		{
			mapLayer.Deconstruct(out var key, out var value);
			string layerName = key;
			value.Layer = layerName;
		}
		AppearanceComponent appearanceComponent = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearanceComponent))
		{
			List<string> list = new List<string>(component.MapLayers.Keys);
			_appearance.SetData(uid, (Enum)StorageMapVisuals.InitLayers, (object)new ShowLayerData(list), appearanceComponent);
		}
		UpdateAppearance(uid, component);
	}

	private void MapperEntityRemoved(EntityUid uid, ItemMapperComponent itemMapper, EntRemovedFromContainerMessage args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (itemMapper.ContainerWhitelist == null || itemMapper.ContainerWhitelist.Contains(((ContainerModifiedMessage)args).Container.ID))
		{
			UpdateAppearance(uid, itemMapper);
		}
	}

	private void MapperEntityInserted(EntityUid uid, ItemMapperComponent itemMapper, EntInsertedIntoContainerMessage args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (itemMapper.ContainerWhitelist == null || itemMapper.ContainerWhitelist.Contains(((ContainerModifiedMessage)args).Container.ID))
		{
			UpdateAppearance(uid, itemMapper);
		}
	}

	private void UpdateAppearance(EntityUid uid, ItemMapperComponent? itemMapper = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearanceComponent = default(AppearanceComponent);
		if (((EntitySystem)this).Resolve<ItemMapperComponent>(uid, ref itemMapper, true) && ((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearanceComponent) && TryGetLayers(uid, itemMapper, out List<string> containedLayers))
		{
			_appearance.SetData(uid, (Enum)StorageMapVisuals.LayerChanged, (object)new ShowLayerData(containedLayers), appearanceComponent);
		}
	}

	private bool TryGetLayers(EntityUid uid, ItemMapperComponent itemMapper, out List<string> showLayers)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] containedLayers = ((IEnumerable<BaseContainer>)(object)_container.GetAllContainers(uid, (ContainerManagerComponent)null)).Where((BaseContainer c) => itemMapper.ContainerWhitelist?.Contains(c.ID) ?? true).SelectMany((BaseContainer cont) => cont.ContainedEntities).ToArray();
		List<string> list = new List<string>();
		foreach (SharedMapLayerData mapLayerData in itemMapper.MapLayers.Values)
		{
			int count = Enumerable.Count(containedLayers, (EntityUid ent) => _whitelistSystem.IsWhitelistPassOrNull(mapLayerData.Whitelist, ent));
			if (count >= mapLayerData.MinCount && count <= mapLayerData.MaxCount)
			{
				list.Add(mapLayerData.Layer);
			}
		}
		showLayers = list;
		return true;
	}
}
