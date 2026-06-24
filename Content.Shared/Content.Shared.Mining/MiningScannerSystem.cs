using System;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Mining.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared.Mining;

public sealed class MiningScannerSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private InventorySystem _inventory;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MiningScannerComponent, EntGotInsertedIntoContainerMessage>((EntityEventRefHandler<MiningScannerComponent, EntGotInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MiningScannerComponent, EntGotRemovedFromContainerMessage>((EntityEventRefHandler<MiningScannerComponent, EntGotRemovedFromContainerMessage>)OnRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MiningScannerComponent, ItemToggledEvent>((EntityEventRefHandler<MiningScannerComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
	}

	private void OnInserted(Entity<MiningScannerComponent> ent, ref EntGotInsertedIntoContainerMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UpdateViewerComponent(((ContainerModifiedMessage)args).Container.Owner);
	}

	private void OnRemoved(Entity<MiningScannerComponent> ent, ref EntGotRemovedFromContainerMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UpdateViewerComponent(((ContainerModifiedMessage)args).Container.Owner);
	}

	private void OnToggled(Entity<MiningScannerComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(ent.Owner, null, null)), ref container))
		{
			UpdateViewerComponent(container.Owner);
		}
	}

	public void UpdateViewerComponent(EntityUid uid)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Entity<MiningScannerComponent>? scannerEnt = null;
		MiningScannerComponent scannerComponent = default(MiningScannerComponent);
		ItemToggleComponent toggle = default(ItemToggleComponent);
		foreach (EntityUid ent in _inventory.GetHandOrInventoryEntities(Entity<HandsComponent, InventoryComponent>.op_Implicit(uid)))
		{
			if (((EntitySystem)this).TryComp<MiningScannerComponent>(ent, ref scannerComponent) && ((EntitySystem)this).TryComp<ItemToggleComponent>(ent, ref toggle) && toggle.Activated && (!scannerEnt.HasValue || scannerComponent.Range > scannerEnt.Value.Comp.Range))
			{
				scannerEnt = Entity<MiningScannerComponent>.op_Implicit((ent, scannerComponent));
			}
		}
		if (!_net.IsServer)
		{
			return;
		}
		if (!scannerEnt.HasValue)
		{
			MiningScannerViewerComponent viewer = default(MiningScannerViewerComponent);
			if (((EntitySystem)this).TryComp<MiningScannerViewerComponent>(uid, ref viewer))
			{
				viewer.QueueRemoval = true;
			}
		}
		else
		{
			MiningScannerViewerComponent viewer2 = ((EntitySystem)this).EnsureComp<MiningScannerViewerComponent>(uid);
			viewer2.ViewRange = scannerEnt.Value.Comp.Range;
			viewer2.QueueRemoval = false;
			viewer2.NextPingTime = _timing.CurTime + viewer2.PingDelay;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)viewer2, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<MiningScannerViewerComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<MiningScannerViewerComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		MiningScannerViewerComponent viewer = default(MiningScannerViewerComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref viewer, ref xform))
		{
			if (viewer.QueueRemoval)
			{
				((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)viewer);
			}
			else if (!(_timing.CurTime < viewer.NextPingTime))
			{
				viewer.NextPingTime = _timing.CurTime + viewer.PingDelay;
				viewer.LastPingLocation = xform.Coordinates;
				if (_net.IsClient && _timing.IsFirstTimePredicted)
				{
					_audio.PlayEntity(viewer.PingSound, uid, uid, (AudioParams?)null);
				}
			}
		}
	}
}
