using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Unit;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Placeable;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Shared.Storage.EntitySystems;

public sealed class DumpableSystem : EntitySystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDisposalUnitSystem _disposalUnitSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	private EntityQuery<ItemComponent> _itemQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_itemQuery = ((EntitySystem)this).GetEntityQuery<ItemComponent>();
		((EntitySystem)this).SubscribeLocalEvent<DumpableComponent, AfterInteractEvent>((ComponentEventHandler<DumpableComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, new Type[1] { typeof(SharedEntityStorageSystem) });
		((EntitySystem)this).SubscribeLocalEvent<DumpableComponent, GetVerbsEvent<AlternativeVerb>>((ComponentEventHandler<DumpableComponent, GetVerbsEvent<AlternativeVerb>>)AddDumpVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DumpableComponent, GetVerbsEvent<UtilityVerb>>((ComponentEventHandler<DumpableComponent, GetVerbsEvent<UtilityVerb>>)AddUtilityVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DumpableComponent, DumpableDoAfterEvent>((ComponentEventHandler<DumpableComponent, DumpableDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
	}

	private void OnAfterInteract(EntityUid uid, DumpableComponent component, AfterInteractEvent args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent storage = default(StorageComponent);
		if (args.CanReach && !((HandledEntityEventArgs)args).Handled && (((EntitySystem)this).HasComp<DisposalUnitComponent>(args.Target) || ((EntitySystem)this).HasComp<PlaceableSurfaceComponent>(args.Target)) && ((EntitySystem)this).TryComp<StorageComponent>(uid, ref storage) && ((BaseContainer)storage.Container).ContainedEntities.Any())
		{
			StartDoAfter(uid, args.Target.Value, args.User, component);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void AddDumpVerb(EntityUid uid, DumpableComponent dumpable, GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		StorageComponent storage = default(StorageComponent);
		if (args.CanAccess && args.CanInteract && ((EntitySystem)this).TryComp<StorageComponent>(uid, ref storage) && ((BaseContainer)storage.Container).ContainedEntities.Any())
		{
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					StartDoAfter(uid, args.Target, args.User, dumpable);
				},
				Text = base.Loc.GetString("dump-verb-name"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/drop.svg.192dpi.png"))
			};
			args.Verbs.Add(verb);
		}
	}

	private void AddUtilityVerbs(EntityUid uid, DumpableComponent dumpable, GetVerbsEvent<UtilityVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent storage = default(StorageComponent);
		if (!args.CanAccess || !args.CanInteract || !((EntitySystem)this).TryComp<StorageComponent>(uid, ref storage) || !((BaseContainer)storage.Container).ContainedEntities.Any())
		{
			return;
		}
		if (((EntitySystem)this).HasComp<DisposalUnitComponent>(args.Target))
		{
			UtilityVerb verb = new UtilityVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					StartDoAfter(uid, args.Target, args.User, dumpable);
				},
				Text = base.Loc.GetString("dump-disposal-verb-name", (ValueTuple<string, object>)("unit", args.Target)),
				IconEntity = ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null)
			};
			args.Verbs.Add(verb);
		}
		if (((EntitySystem)this).HasComp<PlaceableSurfaceComponent>(args.Target))
		{
			UtilityVerb verb2 = new UtilityVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					StartDoAfter(uid, args.Target, args.User, dumpable);
				},
				Text = base.Loc.GetString("dump-placeable-verb-name", (ValueTuple<string, object>)("surface", args.Target)),
				IconEntity = ((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null)
			};
			args.Verbs.Add(verb2);
		}
	}

	private void StartDoAfter(EntityUid storageUid, EntityUid targetUid, EntityUid userUid, DumpableComponent dumpable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent storage = default(StorageComponent);
		if (!((EntitySystem)this).TryComp<StorageComponent>(storageUid, ref storage))
		{
			return;
		}
		float delay = 0f;
		ItemComponent itemComp = default(ItemComponent);
		ItemSizePrototype itemSize = default(ItemSizePrototype);
		foreach (EntityUid entity in ((BaseContainer)storage.Container).ContainedEntities)
		{
			if (_itemQuery.TryGetComponent(entity, ref itemComp) && _prototypeManager.TryIndex<ItemSizePrototype>(itemComp.Size, ref itemSize))
			{
				delay += (float)itemSize.Weight;
			}
		}
		delay *= (float)dumpable.DelayPerItem.TotalSeconds * dumpable.Multiplier;
		_doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, userUid, delay, new DumpableDoAfterEvent(), storageUid, targetUid, storageUid)
		{
			BreakOnMove = true,
			NeedHand = true
		});
	}

	private void OnDoAfter(EntityUid uid, DumpableComponent component, DumpableDoAfterEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		StorageComponent storage = default(StorageComponent);
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled || !((EntitySystem)this).TryComp<StorageComponent>(uid, ref storage) || ((BaseContainer)storage.Container).ContainedEntities.Count == 0)
		{
			return;
		}
		Queue<EntityUid> dumpQueue = new Queue<EntityUid>(((BaseContainer)storage.Container).ContainedEntities);
		bool dumped = false;
		if (((EntitySystem)this).HasComp<DisposalUnitComponent>(args.Args.Target))
		{
			dumped = true;
			foreach (EntityUid entity in dumpQueue)
			{
				_disposalUnitSystem.DoInsertDisposalUnit(args.Args.Target.Value, entity, args.Args.User);
			}
		}
		else if (((EntitySystem)this).HasComp<PlaceableSurfaceComponent>(args.Args.Target))
		{
			dumped = true;
			var (targetPos, targetRot) = _transformSystem.GetWorldPositionRotation(args.Args.Target.Value);
			foreach (EntityUid entity2 in dumpQueue)
			{
				_transformSystem.SetWorldPositionRotation(entity2, targetPos + _random.NextVector2Box(1f, 1f) / 4f, targetRot, (TransformComponent)null);
			}
		}
		else
		{
			Vector2 targetPos2 = _transformSystem.GetWorldPosition(uid);
			foreach (EntityUid entity3 in dumpQueue)
			{
				TransformComponent transform = ((EntitySystem)this).Transform(entity3);
				_transformSystem.SetWorldPositionRotation(entity3, targetPos2 + _random.NextVector2Box(1f, 1f) / 4f, _random.NextAngle(), transform);
			}
		}
		if (dumped)
		{
			_audio.PlayPredicted(component.DumpSound, uid, (EntityUid?)args.User, (AudioParams?)null);
		}
	}
}
