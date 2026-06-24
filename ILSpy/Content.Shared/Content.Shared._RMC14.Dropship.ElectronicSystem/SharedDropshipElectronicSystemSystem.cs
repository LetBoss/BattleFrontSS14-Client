using System;
using Content.Shared._RMC14.Camera;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.PowerLoader;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Dropship.ElectronicSystem;

public abstract class SharedDropshipElectronicSystemSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDropshipSystem _dropship;

	[Dependency]
	private SharedRMCCameraSystem _rmcCamera;

	private const int MinSpread = 0;

	private const int MinBulletSpread = 1;

	private const float MinTravelTime = 1f;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<DropshipComponent, DropshipWeaponShotEvent>((EntityEventRefHandler<DropshipComponent, DropshipWeaponShotEvent>)OnDropshipWeaponShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, DropShipAttachmentInsertedEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, DropShipAttachmentInsertedEvent>)OnDropShipAttachmentInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipElectronicSystemPointComponent, DropShipAttachmentDetachedEvent>((EntityEventRefHandler<DropshipElectronicSystemPointComponent, DropShipAttachmentDetachedEvent>)OnDropShipAttachmentDetached, (Type[])null, (Type[])null);
	}

	private void OnDropshipWeaponShot(Entity<DropshipComponent> ent, ref DropshipWeaponShotEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		DropshipElectronicSystemPointComponent electronic = default(DropshipElectronicSystemPointComponent);
		BaseContainer container = default(BaseContainer);
		DropshipTargetingSystemComponent targeting = default(DropshipTargetingSystemComponent);
		foreach (EntityUid point in ent.Comp.AttachmentPoints)
		{
			if (!((EntitySystem)this).TryComp<DropshipElectronicSystemPointComponent>(point, ref electronic) || !_container.TryGetContainer(point, electronic.ContainerId, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid contained in container.ContainedEntities)
			{
				if (((EntitySystem)this).TryComp<DropshipTargetingSystemComponent>(contained, ref targeting))
				{
					args.Spread = Math.Max(0f, args.Spread + (float)targeting.SpreadModifier);
					args.BulletSpread = Math.Max(1, args.BulletSpread + targeting.BulletSpreadModifier);
					args.TravelTime = TimeSpan.FromSeconds(Math.Max(1.0, args.TravelTime.TotalSeconds + targeting.TravelingTimeModifier.TotalSeconds));
				}
			}
		}
	}

	protected virtual void OnDropShipAttachmentInserted(Entity<DropshipElectronicSystemPointComponent> ent, ref DropShipAttachmentInsertedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		CameraSignalGranterComponent signalGranter = default(CameraSignalGranterComponent);
		if (_dropship.TryGetGridDropship(Entity<DropshipElectronicSystemPointComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropship) && ((EntitySystem)this).TryComp<CameraSignalGranterComponent>(args.Inserted, ref signalGranter))
		{
			ModifyCameraSignals(Entity<CameraSignalGranterComponent>.op_Implicit((args.Inserted, signalGranter)), dropship);
		}
	}

	protected virtual void OnDropShipAttachmentDetached(Entity<DropshipElectronicSystemPointComponent> ent, ref DropShipAttachmentDetachedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (_dropship.TryGetGridDropship(Entity<DropshipElectronicSystemPointComponent>.op_Implicit(ent), out Entity<DropshipComponent> dropship))
		{
			CameraSignalGranterComponent signalGranter = default(CameraSignalGranterComponent);
			if (((EntitySystem)this).TryComp<CameraSignalGranterComponent>(args.Detached, ref signalGranter))
			{
				ModifyCameraSignals(Entity<CameraSignalGranterComponent>.op_Implicit((args.Detached, signalGranter)), dropship, remove: true);
			}
			DropshipSpotlightComponent spotlight = default(DropshipSpotlightComponent);
			if (((EntitySystem)this).TryComp<DropshipSpotlightComponent>(args.Detached, ref spotlight))
			{
				spotlight.Enabled = false;
				((EntitySystem)this).Dirty(args.Detached, (IComponent)(object)spotlight, (MetaDataComponent)null);
			}
		}
	}

	private void ModifyCameraSignals(Entity<CameraSignalGranterComponent> ent, Entity<DropshipComponent> dropship, bool remove = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		TransformChildrenEnumerator query = ((EntitySystem)this).Transform(Entity<DropshipComponent>.op_Implicit(dropship)).ChildEnumerator;
		EntityUid uid = default(EntityUid);
		RMCCameraComputerComponent cameraComputer = default(RMCCameraComputerComponent);
		while (((TransformChildrenEnumerator)(ref query)).MoveNext(ref uid))
		{
			if (!((EntitySystem)this).TryComp<RMCCameraComputerComponent>(uid, ref cameraComputer))
			{
				continue;
			}
			foreach (EntProtoId protoId in ent.Comp.ProtoIds)
			{
				if (remove)
				{
					_rmcCamera.RemoveProtoId(cameraComputer, protoId);
				}
				else
				{
					_rmcCamera.AddProtoId(cameraComputer, protoId);
				}
				_rmcCamera.RefreshCameras(protoId);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)cameraComputer, (MetaDataComponent)null);
		}
	}
}
