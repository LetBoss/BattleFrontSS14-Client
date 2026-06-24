using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Xenonids.Acid;
using Content.Shared._RMC14.Xenonids.Spray;
using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Foldable;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Deploy;

public sealed class RMCDeploySystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private IEntityManager _entMan;

	[Dependency]
	private SharedTransformSystem _xform;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private FoldableSystem _foldable;

	[Dependency]
	private SharedBuckleSystem _buckle;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private SharedDestructibleSystem _destructible;

	[Dependency]
	private SharedEntityStorageSystem _entityStorage;

	[Dependency]
	private SharedAudioSystem _audio;

	private List<EntityUid> _toDelete = new List<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCDeployableComponent, UseInHandEvent>((EntityEventRefHandler<RMCDeployableComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDeployableComponent, RMCDeployDoAfterEvent>((EntityEventRefHandler<RMCDeployableComponent, RMCDeployDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDeployableComponent, DoAfterAttemptEvent<RMCDeployDoAfterEvent>>((EntityEventRefHandler<RMCDeployableComponent, DoAfterAttemptEvent<RMCDeployDoAfterEvent>>)OnDeployDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDeployedEntityComponent, ComponentShutdown>((EntityEventRefHandler<RMCDeployedEntityComponent, ComponentShutdown>)OnDeployedEntityShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDeployableComponent, ComponentShutdown>((EntityEventRefHandler<RMCDeployableComponent, ComponentShutdown>)OnDeployableShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDeployedEntityComponent, InteractUsingEvent>((EntityEventRefHandler<RMCDeployedEntityComponent, InteractUsingEvent>)OnParentalCollapseInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDeployedEntityComponent, RMCParentalCollapseDoAfterEvent>((EntityEventRefHandler<RMCDeployedEntityComponent, RMCParentalCollapseDoAfterEvent>)OnParentalCollapseDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDeployableComponent, ExaminedEvent>((EntityEventRefHandler<RMCDeployableComponent, ExaminedEvent>)OnDeployableExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCDeployedEntityComponent, ExaminedEvent>((EntityEventRefHandler<RMCDeployedEntityComponent, ExaminedEvent>)OnDeployedExamined, (Type[])null, (Type[])null);
	}

	private void OnUseInHand(Entity<RMCDeployableComponent> ent, ref UseInHandEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			TryStartDeploy(ent, args.User);
		}
	}

	private void TryStartDeploy(Entity<RMCDeployableComponent> ent, EntityUid user)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = ent.Owner;
		RMCDeployableComponent comp = ent.Comp;
		if (HasAnyAcid(uid))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-deploy-popup-acid", (ValueTuple<string, object>)("entity", ent)), uid, user, PopupType.SmallCaution);
			return;
		}
		EntityUid? gridUid = _xform.GetGrid(Entity<TransformComponent>.op_Implicit(user));
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref grid) || (comp.FailIfNotSurface && !CheckSurface(gridUid.Value, uid, user)))
		{
			return;
		}
		Vector2 userWorldPos = _xform.GetWorldPosition(user);
		Vector2i tileIndices = _map.WorldToTile(gridUid.Value, grid, userWorldPos);
		Vector2 areaCenter = _map.TileCenterToVector(gridUid.Value, grid, tileIndices);
		Transform transform = default(Transform);
		((Transform)(ref transform))._002Ector(areaCenter, 0f);
		Box2 area = comp.DeployArea.ComputeAABB(transform, 0);
		if (comp.AreaBlockedCheck && IsAreaBlocked(area, uid, user, ent))
		{
			return;
		}
		DoAfterArgs doAfter = new DoAfterArgs(_entMan, user, comp.DeployTime, new RMCDeployDoAfterEvent(area), uid)
		{
			BreakOnMove = true,
			BreakOnDamage = true,
			BreakOnHandChange = true,
			NeedHand = true,
			AttemptFrequency = AttemptFrequency.EveryTick
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			_popup.PopupClient(base.Loc.GetString("rmc-deploy-popup-start"), ent.Owner, user);
			if (_netManager.IsServer)
			{
				ent.Comp.CurrentDeployUser = user;
				((EntitySystem)this).Dirty<RMCDeployableComponent>(ent, (MetaDataComponent)null);
				RMCShowDeployAreaEvent showEvent = new RMCShowDeployAreaEvent(area, Color.Blue);
				((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)showEvent, user);
			}
		}
	}

	private void OnDoAfter(Entity<RMCDeployableComponent> ent, ref RMCDeployDoAfterEvent ev)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsClient)
		{
			return;
		}
		if (ev.Cancelled || ((HandledEntityEventArgs)ev).Handled)
		{
			ent.Comp.CurrentDeployUser = null;
			((EntitySystem)this).Dirty<RMCDeployableComponent>(ent, (MetaDataComponent)null);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCHideDeployAreaEvent(), ev.Args.User);
			return;
		}
		((HandledEntityEventArgs)ev).Handled = true;
		EntityUid user = ev.Args.User;
		EntityUid? gridUid = _xform.GetGrid(Entity<TransformComponent>.op_Implicit(user));
		MapGridComponent grid = default(MapGridComponent);
		if (((EntitySystem)this).TryComp<MapGridComponent>(gridUid, ref grid))
		{
			Vector2 userWorldPos = _xform.GetWorldPosition(user);
			Vector2i tileIndices = _map.WorldToTile(gridUid.Value, grid, userWorldPos);
			Vector2 tileCenter = _map.TileCenterToVector(gridUid.Value, grid, tileIndices);
			Transform areaTransform = default(Transform);
			((Transform)(ref areaTransform))._002Ector(tileCenter, 0f);
			Box2 area = ent.Comp.DeployArea.ComputeAABB(areaTransform, 0);
			Vector2 areaCenter = ((Box2)(ref area)).Center;
			DeploySetups(ent, areaCenter, user);
			if (ent.Comp.DeploySound != null)
			{
				_audio.PlayPvs(ent.Comp.DeploySound, user, (AudioParams?)null);
			}
			ent.Comp.CurrentDeployUser = null;
			((EntitySystem)this).Dirty<RMCDeployableComponent>(ent, (MetaDataComponent)null);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCHideDeployAreaEvent(), ev.Args.User);
		}
	}

	private void DeploySetups(Entity<RMCDeployableComponent> ent, Vector2 areaCenter, EntityUid user)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!_netManager.IsClient)
		{
			((EntitySystem)this).EnsureComp<ContainerManagerComponent>(ent.Owner);
			BaseContainer originalStorage = default(BaseContainer);
			if (_container.TryGetContainer(ent.Owner, "storage", ref originalStorage, (ContainerManagerComponent)null) && originalStorage.ContainedEntities.Count > 0)
			{
				RedeployExistingEntities(ent, areaCenter, originalStorage);
			}
			else
			{
				DeployAllSetups(ent, areaCenter, user);
			}
		}
	}

	private void RedeployExistingEntities(Entity<RMCDeployableComponent> ent, Vector2 areaCenter, BaseContainer originalStorage)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsClient)
		{
			return;
		}
		EntityUid? storageEntity = null;
		RMCDeployedEntityComponent deployedComp = default(RMCDeployedEntityComponent);
		FoldableComponent foldableComp = default(FoldableComponent);
		foreach (EntityUid containedEntity in originalStorage.ContainedEntities.ToList())
		{
			if (!((EntitySystem)this).TryComp<RMCDeployedEntityComponent>(containedEntity, ref deployedComp))
			{
				continue;
			}
			int setupIndex = deployedComp.SetupIndex;
			if (setupIndex < 0 || setupIndex >= ent.Comp.DeploySetups.Count)
			{
				continue;
			}
			RMCDeploySetup setup = ent.Comp.DeploySetups[setupIndex];
			if (setup.NeverRedeployableSetup)
			{
				continue;
			}
			_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(containedEntity), originalStorage, true, false, (EntityCoordinates?)null, (Angle?)null);
			if (((EntitySystem)this).TryComp<FoldableComponent>(containedEntity, ref foldableComp))
			{
				_foldable.SetFolded(containedEntity, foldableComp, folded: false);
			}
			Vector2 spawnPos = areaCenter + setup.Offset;
			_xform.SetWorldPosition(containedEntity, spawnPos);
			_xform.SetWorldRotation(containedEntity, Angle.FromDegrees((double)setup.Angle));
			if (setup.Anchor)
			{
				TransformComponent xform = ((EntitySystem)this).Transform(containedEntity);
				if (!xform.Anchored)
				{
					_xform.AnchorEntity(Entity<TransformComponent>.op_Implicit((containedEntity, xform)), (Entity<MapGridComponent>?)null);
				}
			}
			if (setup.StorageOriginalEntity && !storageEntity.HasValue)
			{
				storageEntity = containedEntity;
			}
		}
		if (storageEntity.HasValue)
		{
			Container container = _container.EnsureContainer<Container>(storageEntity.Value, "storage", (ContainerManagerComponent)null);
			if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(ent.Owner), (BaseContainer)(object)container, (TransformComponent)null, false))
			{
				((EntitySystem)this).Log.Error($"Failed to place original entity {ent.Owner} in container 'storage' of entity {storageEntity.Value}");
			}
		}
	}

	private void DeployAllSetups(Entity<RMCDeployableComponent> ent, Vector2 areaCenter, EntityUid user)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsClient)
		{
			return;
		}
		EntityUid? storageEntity = null;
		foreach (var item in ent.Comp.DeploySetups.Select((RMCDeploySetup s, int idx) => (s: s, idx: idx)))
		{
			RMCDeploySetup setup = item.s;
			int i = item.idx;
			Vector2 spawnPos = areaCenter + setup.Offset;
			((EntitySystem)this).Log.Debug($"RMCDeploySystem: Spawning entity {setup.Prototype} at position {spawnPos}");
			EntityUid spawned = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(setup.Prototype), new MapCoordinates(spawnPos, _xform.GetMapId(Entity<TransformComponent>.op_Implicit(user))), (ComponentRegistry)null, default(Angle));
			_xform.SetWorldPosition(spawned, spawnPos);
			_xform.SetWorldRotation(spawned, Angle.FromDegrees((double)setup.Angle));
			if (setup.Anchor)
			{
				TransformComponent xform = ((EntitySystem)this).Transform(spawned);
				if (!xform.Anchored)
				{
					_xform.AnchorEntity(Entity<TransformComponent>.op_Implicit((spawned, xform)), (Entity<MapGridComponent>?)null);
				}
			}
			RMCDeployedEntityComponent childComp = ((EntitySystem)this).EnsureComp<RMCDeployedEntityComponent>(spawned);
			childComp.OriginalEntity = ent.Owner;
			childComp.SetupIndex = i;
			((EntitySystem)this).Dirty(spawned, (IComponent)(object)childComp, (MetaDataComponent)null);
			if (setup.StorageOriginalEntity && !storageEntity.HasValue)
			{
				storageEntity = spawned;
			}
		}
		if (storageEntity.HasValue)
		{
			Container container = _container.EnsureContainer<Container>(storageEntity.Value, "storage", (ContainerManagerComponent)null);
			if (!_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(ent.Owner), (BaseContainer)(object)container, (TransformComponent)null, false))
			{
				((EntitySystem)this).Log.Error($"RMCDeploySystem: Failed to place original entity {ent.Owner} in container 'storage' of entity {storageEntity.Value}");
			}
		}
		else
		{
			((EntitySystem)this).Log.Error("RMCDeploySystem: Original entity with StorageOriginalEntity not found for placement");
		}
	}

	private void OnDeployDoAfterAttempt(Entity<RMCDeployableComponent> ent, ref DoAfterAttemptEvent<RMCDeployDoAfterEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		RMCDeployableComponent comp = ent.Comp;
		EntityUid uid = ent.Owner;
		EntityUid user = args.Event.User;
		Box2 area = args.Event.Area;
		if (HasAnyAcid(uid))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-deploy-popup-acid", (ValueTuple<string, object>)("entity", ent)), user, user, PopupType.SmallCaution);
			((CancellableEntityEventArgs)args).Cancel();
		}
		else if (comp.AreaBlockedCheck && IsAreaBlocked(area, uid, user, ent))
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-deploy-popup-blocked"), user, user, PopupType.SmallCaution);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private bool IsAreaBlocked(Box2 area, EntityUid ignore, EntityUid? user = null, Entity<RMCDeployableComponent>? ent = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		RMCDeployableComponent obj = ent?.Comp;
		MapId mapId = _xform.GetMapId(Entity<TransformComponent>.op_Implicit(ignore));
		bool found = false;
		bool failIfNotSurface = obj?.FailIfNotSurface ?? true;
		EntityUid? gridUid = _xform.GetGrid(Entity<TransformComponent>.op_Implicit(ignore));
		if (!((EntitySystem)this).HasComp<MapGridComponent>(gridUid))
		{
			return false;
		}
		if (failIfNotSurface && gridUid.HasValue && !CheckSurface(gridUid.Value, ignore, user))
		{
			return true;
		}
		PhysicsComponent physics = default(PhysicsComponent);
		foreach (EntityUid entId in _lookup.GetEntitiesIntersecting(mapId, area, (LookupFlags)110))
		{
			if (entId == ignore)
			{
				continue;
			}
			if (user.HasValue)
			{
				EntityUid val = entId;
				EntityUid? val2 = user;
				if (val2.HasValue && val == val2.GetValueOrDefault())
				{
					continue;
				}
			}
			if (!IsGhostRelated(entId) && ((EntitySystem)this).TryComp<PhysicsComponent>(entId, ref physics) && (physics.CanCollide || physics.Hard))
			{
				_ = ((EntitySystem)this).MetaData(entId).EntityName;
				found = true;
				break;
			}
		}
		if (found && user.HasValue && _netManager.IsClient)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-deploy-popup-blocked"), user.Value, user.Value, PopupType.SmallCaution);
		}
		return found;
	}

	private bool IsGhostRelated(EntityUid uid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		EntityUid current = uid;
		HashSet<EntityUid> visited = new HashSet<EntityUid>();
		BaseContainer container = default(BaseContainer);
		TransformComponent xform = default(TransformComponent);
		while (visited.Add(current))
		{
			if (((EntitySystem)this).HasComp<GhostComponent>(current))
			{
				return true;
			}
			if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(current), ref container))
			{
				current = container.Owner;
				continue;
			}
			if (!((EntitySystem)this).TryComp(current, ref xform))
			{
				break;
			}
			EntityUid parentUid = xform.ParentUid;
			if (!((EntityUid)(ref parentUid)).IsValid())
			{
				break;
			}
			current = xform.ParentUid;
		}
		return false;
	}

	private bool CheckSurface(EntityUid gridUid, EntityUid ignore, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<RMCPlanetComponent>(gridUid))
		{
			if (user.HasValue && _netManager.IsClient)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-deploy-popup-surface"), ignore, user.Value, PopupType.SmallCaution);
			}
			return false;
		}
		return true;
	}

	private void OnDeployedEntityShutdown(Entity<RMCDeployedEntityComponent> ent, ref ComponentShutdown args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsClient || ent.Comp.InShutdown)
		{
			return;
		}
		ent.Comp.InShutdown = true;
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		RMCDeployableComponent origComp = default(RMCDeployableComponent);
		if (!((EntitySystem)this).Exists(ent.Comp.OriginalEntity) || !((EntitySystem)this).TryComp<RMCDeployableComponent>(ent.Comp.OriginalEntity, ref origComp) || origComp == null || origComp.DeploySetups[ent.Comp.SetupIndex].Mode != RMCDeploySetupMode.ReactiveParental)
		{
			return;
		}
		List<EntityUid> toDelete = new List<EntityUid>();
		EntityQueryEnumerator<RMCDeployedEntityComponent> enumerator = ((EntitySystem)this).EntityQueryEnumerator<RMCDeployedEntityComponent>();
		EntityUid entity = default(EntityUid);
		RMCDeployedEntityComponent childComp = default(RMCDeployedEntityComponent);
		while (enumerator.MoveNext(ref entity, ref childComp))
		{
			if (!(childComp.OriginalEntity != ent.Comp.OriginalEntity) && childComp.SetupIndex != ent.Comp.SetupIndex)
			{
				RMCDeploySetupMode mode = origComp.DeploySetups[childComp.SetupIndex].Mode;
				if (mode == RMCDeploySetupMode.ReactiveParental || mode == RMCDeploySetupMode.Reactive)
				{
					toDelete.Add(entity);
				}
			}
		}
		foreach (EntityUid entity2 in toDelete)
		{
			_destructible.DestroyEntity(entity2);
		}
	}

	private void OnDeployableShutdown(Entity<RMCDeployableComponent> ent, ref ComponentShutdown args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsClient)
		{
			return;
		}
		if (ent.Comp.CurrentDeployUser.HasValue)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new RMCHideDeployAreaEvent(), ent.Comp.CurrentDeployUser.Value);
		}
		_toDelete.Clear();
		EntityQueryEnumerator<RMCDeployedEntityComponent> enumerator = ((EntitySystem)this).EntityQueryEnumerator<RMCDeployedEntityComponent>();
		EntityUid entity = default(EntityUid);
		RMCDeployedEntityComponent childComp = default(RMCDeployedEntityComponent);
		while (enumerator.MoveNext(ref entity, ref childComp))
		{
			if (!(childComp.OriginalEntity != ent.Owner) && childComp.SetupIndex >= 0 && childComp.SetupIndex < ent.Comp.DeploySetups.Count)
			{
				RMCDeploySetup setup = ent.Comp.DeploySetups[childComp.SetupIndex];
				if (setup.StorageOriginalEntity)
				{
					childComp.InShutdown = true;
					((EntitySystem)this).Dirty(entity, (IComponent)(object)childComp, (MetaDataComponent)null);
				}
				else if ((setup.Mode == RMCDeploySetupMode.ReactiveParental || setup.Mode == RMCDeploySetupMode.Reactive) && !childComp.InShutdown)
				{
					childComp.InShutdown = true;
					((EntitySystem)this).Dirty(entity, (IComponent)(object)childComp, (MetaDataComponent)null);
					_toDelete.Add(entity);
				}
			}
		}
		foreach (EntityUid entity2 in _toDelete)
		{
			_destructible.DestroyEntity(entity2);
		}
	}

	private void OnParentalCollapseInteractUsing(Entity<RMCDeployedEntityComponent> ent, ref InteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		RMCDeployableComponent deployable = default(RMCDeployableComponent);
		MetaDataComponent usedMeta = default(MetaDataComponent);
		ItemToggleComponent toggle = default(ItemToggleComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<RMCDeployableComponent>(ent.Comp.OriginalEntity, ref deployable) && deployable.DeploySetups[ent.Comp.SetupIndex].Mode == RMCDeploySetupMode.ReactiveParental && deployable.CollapseToolPrototype.HasValue && ((EntitySystem)this).TryComp(args.Used, ref usedMeta) && usedMeta.EntityPrototype != null && !(EntProtoId.op_Implicit(usedMeta.EntityPrototype.ID) != deployable.CollapseToolPrototype.Value) && (!((EntitySystem)this).TryComp<ItemToggleComponent>(args.Used, ref toggle) || toggle.Activated))
		{
			((HandledEntityEventArgs)args).Handled = true;
			DoAfterArgs doAfter = new DoAfterArgs(_entMan, args.User, TimeSpan.FromSeconds(deployable.CollapseTime), new RMCParentalCollapseDoAfterEvent(), ent.Owner)
			{
				BreakOnMove = true,
				BreakOnDamage = true,
				BreakOnHandChange = true,
				NeedHand = true
			};
			if (_doAfter.TryStartDoAfter(doAfter))
			{
				_popup.PopupClient(base.Loc.GetString("rmc-deployable-collapse-start"), args.User, args.User);
			}
		}
	}

	private void OnParentalCollapseDoAfter(Entity<RMCDeployedEntityComponent> ent, ref RMCParentalCollapseDoAfterEvent ev)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		if (_netManager.IsClient || ev.Cancelled || ((HandledEntityEventArgs)ev).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)ev).Handled = true;
		RMCDeployedEntityComponent comp = ent.Comp;
		EntityUid user = ev.Args.User;
		RMCDeployableComponent deployable = default(RMCDeployableComponent);
		if (!((EntitySystem)this).TryComp<RMCDeployableComponent>(comp.OriginalEntity, ref deployable))
		{
			return;
		}
		EntityUid? reactiveParentalWithOriginal = null;
		EntityQueryEnumerator<RMCDeployedEntityComponent> reactiveParentalEnumerator = ((EntitySystem)this).EntityQueryEnumerator<RMCDeployedEntityComponent>();
		EntityUid reactiveParentalUid = default(EntityUid);
		RMCDeployedEntityComponent reactiveParentalComp = default(RMCDeployedEntityComponent);
		RMCDeployableComponent origDeployable = default(RMCDeployableComponent);
		BaseContainer storage = default(BaseContainer);
		while (reactiveParentalEnumerator.MoveNext(ref reactiveParentalUid, ref reactiveParentalComp))
		{
			if (!(reactiveParentalComp.OriginalEntity != comp.OriginalEntity) && ((EntitySystem)this).TryComp<RMCDeployableComponent>(comp.OriginalEntity, ref origDeployable) && origDeployable.DeploySetups[reactiveParentalComp.SetupIndex].Mode == RMCDeploySetupMode.ReactiveParental && _container.TryGetContainer(reactiveParentalUid, "storage", ref storage, (ContainerManagerComponent)null) && storage.Contains(comp.OriginalEntity))
			{
				reactiveParentalWithOriginal = reactiveParentalUid;
				break;
			}
		}
		if (!reactiveParentalWithOriginal.HasValue)
		{
			return;
		}
		BaseContainer storage2 = default(BaseContainer);
		if (_container.TryGetContainer(reactiveParentalWithOriginal.Value, "storage", ref storage2, (ContainerManagerComponent)null) && storage2.Contains(comp.OriginalEntity))
		{
			_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(comp.OriginalEntity), storage2, true, false, (EntityCoordinates?)null, (Angle?)null);
			Vector2 userCoords = _xform.GetWorldPosition(user);
			_xform.SetWorldPosition(comp.OriginalEntity, userCoords);
		}
		Container origStorage = _container.EnsureContainer<Container>(comp.OriginalEntity, "storage", (ContainerManagerComponent)null);
		EntityQueryEnumerator<RMCDeployedEntityComponent> enumerator = ((EntitySystem)this).EntityQueryEnumerator<RMCDeployedEntityComponent>();
		EntityUid childUid = default(EntityUid);
		RMCDeployedEntityComponent childComp = default(RMCDeployedEntityComponent);
		FoldableComponent foldableComp = default(FoldableComponent);
		while (enumerator.MoveNext(ref childUid, ref childComp))
		{
			if (!(childComp.OriginalEntity != comp.OriginalEntity) && childComp.SetupIndex >= 0 && childComp.SetupIndex < deployable.DeploySetups.Count && !deployable.DeploySetups[childComp.SetupIndex].NeverRedeployableSetup && !(childUid == comp.OriginalEntity))
			{
				_entityStorage.EmptyContents(childUid);
				TryUnbuckleAll(childUid);
				if (((EntitySystem)this).TryComp<FoldableComponent>(childUid, ref foldableComp))
				{
					_foldable.SetFolded(childUid, foldableComp, folded: true);
				}
				_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(childUid), (BaseContainer)(object)origStorage, (TransformComponent)null, false);
			}
		}
		if (deployable.CollapseSound != null)
		{
			_audio.PlayPvs(deployable.CollapseSound, user, (AudioParams?)null);
		}
	}

	private void TryUnbuckleAll(EntityUid entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		StrapComponent strap = default(StrapComponent);
		if (((EntitySystem)this).TryComp<StrapComponent>(entity, ref strap) && strap.BuckledEntities.Count != 0)
		{
			EntityUid[] array = strap.BuckledEntities.ToArray();
			foreach (EntityUid buckled in array)
			{
				_buckle.Unbuckle(Entity<BuckleComponent>.op_Implicit((buckled, ((EntitySystem)this).CompOrNull<BuckleComponent>(buckled))), null);
			}
		}
	}

	private bool HasAnyAcid(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<TimedCorrodingComponent>(uid) && !((EntitySystem)this).HasComp<DamageableCorrodingComponent>(uid))
		{
			return ((EntitySystem)this).HasComp<SprayAcidedComponent>(uid);
		}
		return true;
	}

	private void OnDeployableExamined(Entity<RMCDeployableComponent> ent, ref ExaminedEvent args)
	{
		args.PushMarkup(base.Loc.GetString("rmc-deployable-examine-hint"));
	}

	private void OnDeployedExamined(Entity<RMCDeployedEntityComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		RMCDeployableComponent deployable = default(RMCDeployableComponent);
		if (!((EntitySystem)this).TryComp<RMCDeployableComponent>(ent.Comp.OriginalEntity, ref deployable) || ent.Comp.SetupIndex < 0 || ent.Comp.SetupIndex >= deployable.DeploySetups.Count || deployable.DeploySetups[ent.Comp.SetupIndex].Mode != RMCDeploySetupMode.ReactiveParental)
		{
			return;
		}
		EntProtoId? collapseToolPrototype = deployable.CollapseToolPrototype;
		if (collapseToolPrototype.HasValue)
		{
			EntProtoId toolProto = collapseToolPrototype.GetValueOrDefault();
			EntityPrototype proto = default(EntityPrototype);
			if (_prototypeManager.TryIndex<EntityPrototype>(EntProtoId.op_Implicit(toolProto), ref proto))
			{
				string toolName = proto.Name;
				args.PushMarkup(base.Loc.GetString("rmc-deployed-collapse-hint", (ValueTuple<string, object>)("tool", toolName)));
			}
		}
	}
}
