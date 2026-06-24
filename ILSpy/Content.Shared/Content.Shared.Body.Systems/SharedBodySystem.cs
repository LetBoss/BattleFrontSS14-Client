using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Organ;
using Content.Shared.Body.Part;
using Content.Shared.Body.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DragDrop;
using Content.Shared.Gibbing.Components;
using Content.Shared.Gibbing.Events;
using Content.Shared.Gibbing.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Body.Systems;

public abstract class SharedBodySystem : EntitySystem
{
	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private GibbingSystem _gibbingSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	private const float GibletLaunchImpulse = 8f;

	private const float GibletLaunchImpulseVariance = 3f;

	public const string PartSlotContainerIdPrefix = "body_part_slot_";

	public const string BodyRootContainerId = "body_root_part";

	public const string OrganSlotContainerIdPrefix = "body_organ_slot_";

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	protected IPrototypeManager Prototypes;

	[Dependency]
	protected DamageableSystem Damageable;

	[Dependency]
	protected MovementSpeedModifierSystem Movement;

	[Dependency]
	protected SharedContainerSystem Containers;

	[Dependency]
	protected SharedTransformSystem SharedTransform;

	[Dependency]
	protected StandingStateSystem Standing;

	private static readonly ProtoId<DamageTypePrototype> BloodlossDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Bloodloss");

	private void InitializeBody()
	{
		((EntitySystem)this).SubscribeLocalEvent<BodyComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<BodyComponent, EntInsertedIntoContainerMessage>)OnBodyInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BodyComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<BodyComponent, EntRemovedFromContainerMessage>)OnBodyRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BodyComponent, ComponentInit>((EntityEventRefHandler<BodyComponent, ComponentInit>)OnBodyInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BodyComponent, MapInitEvent>((EntityEventRefHandler<BodyComponent, MapInitEvent>)OnBodyMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BodyComponent, CanDragEvent>((EntityEventRefHandler<BodyComponent, CanDragEvent>)OnBodyCanDrag, (Type[])null, (Type[])null);
	}

	private void OnBodyInserted(Entity<BodyComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		string slotId = ((ContainerModifiedMessage)args).Container.ID;
		if (!(slotId != "body_root_part"))
		{
			EntityUid insertedUid = ((ContainerModifiedMessage)args).Entity;
			BodyPartComponent part = default(BodyPartComponent);
			if (((EntitySystem)this).TryComp<BodyPartComponent>(insertedUid, ref part))
			{
				AddPart(Entity<BodyComponent>.op_Implicit((Entity<BodyComponent>.op_Implicit(ent), Entity<BodyComponent>.op_Implicit(ent))), Entity<BodyPartComponent>.op_Implicit((insertedUid, part)), slotId);
				RecursiveBodyUpdate(Entity<BodyPartComponent>.op_Implicit((insertedUid, part)), Entity<BodyComponent>.op_Implicit(ent));
			}
			OrganComponent organ = default(OrganComponent);
			if (((EntitySystem)this).TryComp<OrganComponent>(insertedUid, ref organ))
			{
				AddOrgan(Entity<OrganComponent>.op_Implicit((insertedUid, organ)), Entity<BodyComponent>.op_Implicit(ent), Entity<BodyComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnBodyRemoved(Entity<BodyComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		string slotId = ((ContainerModifiedMessage)args).Container.ID;
		if (!(slotId != "body_root_part"))
		{
			EntityUid removedUid = ((ContainerModifiedMessage)args).Entity;
			BodyPartComponent part = default(BodyPartComponent);
			if (((EntitySystem)this).TryComp<BodyPartComponent>(removedUid, ref part))
			{
				RemovePart(Entity<BodyComponent>.op_Implicit((Entity<BodyComponent>.op_Implicit(ent), Entity<BodyComponent>.op_Implicit(ent))), Entity<BodyPartComponent>.op_Implicit((removedUid, part)), slotId);
				RecursiveBodyUpdate(Entity<BodyPartComponent>.op_Implicit((removedUid, part)), null);
			}
			OrganComponent organ = default(OrganComponent);
			if (((EntitySystem)this).TryComp<OrganComponent>(removedUid, ref organ))
			{
				RemoveOrgan(Entity<OrganComponent>.op_Implicit((removedUid, organ)), Entity<BodyComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnBodyInit(Entity<BodyComponent> ent, ref ComponentInit args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.RootContainer = Containers.EnsureContainer<ContainerSlot>(Entity<BodyComponent>.op_Implicit(ent), "body_root_part", (ContainerManagerComponent)null);
	}

	private void OnBodyMapInit(Entity<BodyComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<BodyPrototype>? prototype = ent.Comp.Prototype;
		if (prototype.HasValue)
		{
			BodyPrototype prototype2 = Prototypes.Index<BodyPrototype>(ent.Comp.Prototype.Value);
			MapInitBody(Entity<BodyComponent>.op_Implicit(ent), prototype2);
		}
	}

	private void MapInitBody(EntityUid bodyEntity, BodyPrototype prototype)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		BodyPrototypeSlot protoRoot = prototype.Slots[prototype.Root];
		if (protoRoot.Part.HasValue)
		{
			EntProtoId? part = protoRoot.Part;
			EntityUid rootPartUid = ((EntitySystem)this).SpawnInContainerOrDrop(part.HasValue ? EntProtoId.op_Implicit(part.GetValueOrDefault()) : null, bodyEntity, "body_root_part", (TransformComponent)null, (ContainerManagerComponent)null, (ComponentRegistry)null);
			BodyPartComponent rootPart = ((EntitySystem)this).Comp<BodyPartComponent>(rootPartUid);
			rootPart.Body = bodyEntity;
			((EntitySystem)this).Dirty(rootPartUid, (IComponent)(object)rootPart, (MetaDataComponent)null);
			SetupOrgans(Entity<BodyPartComponent>.op_Implicit((rootPartUid, rootPart)), protoRoot.Organs);
			MapInitParts(rootPartUid, prototype);
		}
	}

	private void OnBodyCanDrag(Entity<BodyComponent> ent, ref CanDragEvent args)
	{
		args.Handled = true;
	}

	private void MapInitParts(EntityUid rootPartId, BodyPrototype prototype)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		string rootSlot = prototype.Root;
		Queue<string> frontier = new Queue<string>();
		frontier.Enqueue(rootSlot);
		Dictionary<string, string> cameFrom = new Dictionary<string, string>();
		cameFrom[rootSlot] = rootSlot;
		Dictionary<string, EntityUid> cameFromEntities = new Dictionary<string, EntityUid>();
		cameFromEntities[rootSlot] = rootPartId;
		string currentSlotId;
		while (frontier.TryDequeue(out currentSlotId))
		{
			foreach (string connection in prototype.Slots[currentSlotId].Connections)
			{
				if (cameFrom.TryAdd(connection, currentSlotId))
				{
					BodyPrototypeSlot connectionSlot = prototype.Slots[connection];
					EntityUid parentEntity = cameFromEntities[currentSlotId];
					BodyPartComponent parentPartComponent = ((EntitySystem)this).Comp<BodyPartComponent>(parentEntity);
					EntProtoId? part = connectionSlot.Part;
					EntityUid childPart = (cameFromEntities[connection] = ((EntitySystem)this).Spawn(part.HasValue ? EntProtoId.op_Implicit(part.GetValueOrDefault()) : null, new EntityCoordinates(parentEntity, Vector2.Zero)));
					BodyPartComponent childPartComponent = ((EntitySystem)this).Comp<BodyPartComponent>(childPart);
					BodyPartSlot? partSlot = CreatePartSlot(parentEntity, connection, childPartComponent.PartType, parentPartComponent);
					BaseContainer cont = Containers.GetContainer(parentEntity, GetPartSlotContainerId(connection), (ContainerManagerComponent)null);
					if (!partSlot.HasValue || !Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(childPart), cont, (TransformComponent)null, false))
					{
						((EntitySystem)this).Log.Error("Could not create slot for connection " + connection + " in body " + prototype.ID);
						((EntitySystem)this).QueueDel((EntityUid?)childPart);
					}
					else
					{
						SetupOrgans(Entity<BodyPartComponent>.op_Implicit((childPart, childPartComponent)), connectionSlot.Organs);
						frontier.Enqueue(connection);
					}
				}
			}
		}
	}

	private void SetupOrgans(Entity<BodyPartComponent> ent, Dictionary<string, string> organs)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, string> organ in organs)
		{
			organ.Deconstruct(out var key, out var value);
			string organSlotId = key;
			string organProto = value;
			OrganSlot? slot = CreateOrganSlot(Entity<BodyPartComponent>.op_Implicit((Entity<BodyPartComponent>.op_Implicit(ent), Entity<BodyPartComponent>.op_Implicit(ent))), organSlotId);
			((EntitySystem)this).SpawnInContainerOrDrop(organProto, Entity<BodyPartComponent>.op_Implicit(ent), GetOrganContainerId(organSlotId), (TransformComponent)null, (ContainerManagerComponent)null, (ComponentRegistry)null);
			if (!slot.HasValue)
			{
				((EntitySystem)this).Log.Error($"Could not create organ for slot {organSlotId} in {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<BodyPartComponent>.op_Implicit(ent), (MetaDataComponent)null)}");
			}
		}
	}

	public IEnumerable<BaseContainer> GetBodyContainers(EntityUid id, BodyComponent? body = null, BodyPartComponent? rootPart = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyComponent>(id, ref body, false) || !body.RootContainer.ContainedEntity.HasValue || !((EntitySystem)this).Resolve<BodyPartComponent>(body.RootContainer.ContainedEntity.Value, ref rootPart, true))
		{
			yield break;
		}
		yield return (BaseContainer)(object)body.RootContainer;
		foreach (BaseContainer partContainer in GetPartContainers(body.RootContainer.ContainedEntity.Value, rootPart))
		{
			yield return partContainer;
		}
	}

	public IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyChildren(EntityUid? id, BodyComponent? body = null, BodyPartComponent? rootPart = null)
	{
		if (!id.HasValue || !((EntitySystem)this).Resolve<BodyComponent>(id.Value, ref body, false) || !body.RootContainer.ContainedEntity.HasValue || !((EntitySystem)this).Resolve<BodyPartComponent>(body.RootContainer.ContainedEntity.Value, ref rootPart, true))
		{
			yield break;
		}
		foreach (var bodyPartChild in GetBodyPartChildren(body.RootContainer.ContainedEntity.Value, rootPart))
		{
			yield return bodyPartChild;
		}
	}

	public IEnumerable<(EntityUid Id, OrganComponent Component)> GetBodyOrgans(EntityUid? bodyId, BodyComponent? body = null)
	{
		if (!bodyId.HasValue || !((EntitySystem)this).Resolve<BodyComponent>(bodyId.Value, ref body, false))
		{
			yield break;
		}
		foreach (var part in GetBodyChildren(bodyId, body))
		{
			foreach (var partOrgan in GetPartOrgans(part.Id, part.Component))
			{
				yield return partOrgan;
			}
		}
	}

	public IEnumerable<BodyPartSlot> GetBodyAllSlots(EntityUid bodyId, BodyComponent? body = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyComponent>(bodyId, ref body, false) || !body.RootContainer.ContainedEntity.HasValue)
		{
			yield break;
		}
		foreach (BodyPartSlot allBodyPartSlot in GetAllBodyPartSlots(body.RootContainer.ContainedEntity.Value))
		{
			yield return allBodyPartSlot;
		}
	}

	public virtual HashSet<EntityUid> GibBody(EntityUid bodyId, bool gibOrgans = false, BodyComponent? body = null, bool launchGibs = true, Vector2? splatDirection = null, float splatModifier = 1f, Angle splatCone = default(Angle), SoundSpecifier? gibSoundOverride = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> gibs = new HashSet<EntityUid>();
		if (!((EntitySystem)this).Resolve<BodyComponent>(bodyId, ref body, false))
		{
			return gibs;
		}
		(EntityUid, BodyPartComponent)? root = GetRootPartOrNull(bodyId, body);
		GibbableComponent gibbable = default(GibbableComponent);
		if (root.HasValue && ((EntitySystem)this).TryComp<GibbableComponent>(root.Value.Item1, ref gibbable) && gibSoundOverride == null)
		{
			gibSoundOverride = gibbable.GibSound;
		}
		(EntityUid, BodyPartComponent)[] parts = GetBodyChildren(bodyId, body).ToArray();
		gibs.EnsureCapacity(parts.Length);
		(EntityUid, BodyPartComponent)[] array = parts;
		for (int i = 0; i < array.Length; i++)
		{
			(EntityUid, BodyPartComponent) part = array[i];
			_gibbingSystem.TryGibEntityWithRef(Entity<TransformComponent>.op_Implicit(bodyId), Entity<GibbableComponent>.op_Implicit(part.Item1), GibType.Gib, GibContentsOption.Skip, ref gibs, launchGibs: true, splatDirection, 8f * splatModifier, 3f, splatCone, 1f, playAudio: false);
			if (!gibOrgans)
			{
				continue;
			}
			foreach (var organ in GetPartOrgans(part.Item1, part.Item2))
			{
				GibbingSystem gibbingSystem = _gibbingSystem;
				Entity<TransformComponent> outerEntity = Entity<TransformComponent>.op_Implicit(bodyId);
				Entity<GibbableComponent> gibbable2 = Entity<GibbableComponent>.op_Implicit(organ.Id);
				float launchImpulse = 8f * splatModifier;
				gibbingSystem.TryGibEntityWithRef(outerEntity, gibbable2, GibType.Drop, GibContentsOption.Skip, ref gibs, launchGibs: true, null, launchImpulse, 3f, splatCone, 1f, playAudio: false);
			}
		}
		TransformComponent bodyTransform = ((EntitySystem)this).Transform(bodyId);
		InventoryComponent inventory = default(InventoryComponent);
		if (((EntitySystem)this).TryComp<InventoryComponent>(bodyId, ref inventory))
		{
			foreach (EntityUid item in _inventory.GetHandOrInventoryEntities(Entity<HandsComponent, InventoryComponent>.op_Implicit(bodyId)))
			{
				SharedTransform.DropNextTo(Entity<TransformComponent>.op_Implicit(item), Entity<TransformComponent>.op_Implicit((bodyId, bodyTransform)));
				gibs.Add(item);
			}
		}
		_audioSystem.PlayPredicted(gibSoundOverride, bodyTransform.Coordinates, (EntityUid?)null, (AudioParams?)null);
		return gibs;
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeBody();
		InitializeParts();
	}

	protected static string? GetPartSlotContainerIdFromContainer(string containerSlotId)
	{
		int slotIndex = containerSlotId.IndexOf("body_part_slot_", StringComparison.Ordinal);
		if (slotIndex < 0)
		{
			return null;
		}
		return containerSlotId.Remove(slotIndex, "body_part_slot_".Length);
	}

	public static string GetPartSlotContainerId(string slotId)
	{
		return "body_part_slot_" + slotId;
	}

	public static string GetOrganContainerId(string slotId)
	{
		return "body_organ_slot_" + slotId;
	}

	private void AddOrgan(Entity<OrganComponent> organEnt, EntityUid bodyUid, EntityUid parentPartUid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		organEnt.Comp.Body = bodyUid;
		OrganAddedEvent addedEv = new OrganAddedEvent(parentPartUid);
		((EntitySystem)this).RaiseLocalEvent<OrganAddedEvent>(Entity<OrganComponent>.op_Implicit(organEnt), ref addedEv, false);
		EntityUid? body = organEnt.Comp.Body;
		if (body.HasValue)
		{
			OrganAddedToBodyEvent addedInBodyEv = new OrganAddedToBodyEvent(bodyUid, parentPartUid);
			((EntitySystem)this).RaiseLocalEvent<OrganAddedToBodyEvent>(Entity<OrganComponent>.op_Implicit(organEnt), ref addedInBodyEv, false);
		}
		((EntitySystem)this).Dirty(Entity<OrganComponent>.op_Implicit(organEnt), (IComponent)(object)organEnt.Comp, (MetaDataComponent)null);
	}

	private void RemoveOrgan(Entity<OrganComponent> organEnt, EntityUid parentPartUid)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		OrganRemovedEvent removedEv = new OrganRemovedEvent(parentPartUid);
		((EntitySystem)this).RaiseLocalEvent<OrganRemovedEvent>(Entity<OrganComponent>.op_Implicit(organEnt), ref removedEv, false);
		EntityUid? body = organEnt.Comp.Body;
		if (body.HasValue)
		{
			EntityUid bodyUid = body.GetValueOrDefault();
			if (((EntityUid)(ref bodyUid)).Valid)
			{
				OrganRemovedFromBodyEvent removedInBodyEv = new OrganRemovedFromBodyEvent(bodyUid, parentPartUid);
				((EntitySystem)this).RaiseLocalEvent<OrganRemovedFromBodyEvent>(Entity<OrganComponent>.op_Implicit(organEnt), ref removedInBodyEv, false);
			}
		}
		organEnt.Comp.Body = null;
		((EntitySystem)this).Dirty(Entity<OrganComponent>.op_Implicit(organEnt), (IComponent)(object)organEnt.Comp, (MetaDataComponent)null);
	}

	private OrganSlot? CreateOrganSlot(Entity<BodyPartComponent?> parentEnt, string slotId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(Entity<BodyPartComponent>.op_Implicit(parentEnt), ref parentEnt.Comp, false))
		{
			return null;
		}
		Containers.EnsureContainer<ContainerSlot>(Entity<BodyPartComponent>.op_Implicit(parentEnt), GetOrganContainerId(slotId), (ContainerManagerComponent)null);
		OrganSlot slot = new OrganSlot(slotId);
		parentEnt.Comp.Organs.Add(slotId, slot);
		return slot;
	}

	public bool TryCreateOrganSlot(EntityUid? parent, string slotId, [NotNullWhen(true)] out OrganSlot? slot, BodyPartComponent? part = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		slot = null;
		if (!parent.HasValue || !((EntitySystem)this).Resolve<BodyPartComponent>(parent.Value, ref part, false))
		{
			return false;
		}
		Containers.EnsureContainer<ContainerSlot>(parent.Value, GetOrganContainerId(slotId), (ContainerManagerComponent)null);
		slot = new OrganSlot(slotId);
		return part.Organs.TryAdd(slotId, slot.Value);
	}

	public bool CanInsertOrgan(EntityUid partId, string slotId, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, true))
		{
			return part.Organs.ContainsKey(slotId);
		}
		return false;
	}

	public bool CanInsertOrgan(EntityUid partId, OrganSlot slot, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return CanInsertOrgan(partId, slot.Id, part);
	}

	public bool InsertOrgan(EntityUid partId, EntityUid organId, string slotId, BodyPartComponent? part = null, OrganComponent? organ = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<OrganComponent>(organId, ref organ, false) || !((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false) || !CanInsertOrgan(partId, slotId, part))
		{
			return false;
		}
		string containerId = GetOrganContainerId(slotId);
		BaseContainer container = default(BaseContainer);
		if (Containers.TryGetContainer(partId, containerId, ref container, (ContainerManagerComponent)null))
		{
			return Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(organId), container, (TransformComponent)null, false);
		}
		return false;
	}

	public bool RemoveOrgan(EntityUid organId, OrganComponent? organ = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!Containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(organId, null, null)), ref container))
		{
			return false;
		}
		EntityUid parent = container.Owner;
		if (((EntitySystem)this).HasComp<BodyPartComponent>(parent))
		{
			return Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(organId), container, true, false, (EntityCoordinates?)null, (Angle?)null);
		}
		return false;
	}

	public bool AddOrganToFirstValidSlot(EntityUid partId, EntityUid organId, BodyPartComponent? part = null, OrganComponent? organ = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false) || !((EntitySystem)this).Resolve<OrganComponent>(organId, ref organ, false))
		{
			return false;
		}
		using (Dictionary<string, OrganSlot>.KeyCollection.Enumerator enumerator = part.Organs.Keys.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				string slotId = enumerator.Current;
				InsertOrgan(partId, organId, slotId, part, organ);
				return true;
			}
		}
		return false;
	}

	public List<Entity<T, OrganComponent>> GetBodyOrganEntityComps<T>(Entity<BodyComponent?> entity) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return new List<Entity<T, OrganComponent>>();
		}
		EntityQuery<T> query = ((EntitySystem)this).GetEntityQuery<T>();
		List<Entity<T, OrganComponent>> list = new List<Entity<T, OrganComponent>>(3);
		T comp = default(T);
		foreach (var organ in GetBodyOrgans(entity.Owner, entity.Comp))
		{
			if (query.TryGetComponent(organ.Id, ref comp))
			{
				list.Add(Entity<T, OrganComponent>.op_Implicit((organ.Id, comp, organ.Component)));
			}
		}
		return list;
	}

	public bool TryGetBodyOrganEntityComps<T>(Entity<BodyComponent?> entity, [NotNullWhen(true)] out List<Entity<T, OrganComponent>>? comps) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyComponent>(entity.Owner, ref entity.Comp, true))
		{
			comps = null;
			return false;
		}
		comps = GetBodyOrganEntityComps<T>(entity);
		if (comps.Count != 0)
		{
			return true;
		}
		comps = null;
		return false;
	}

	private void InitializeParts()
	{
		((EntitySystem)this).SubscribeLocalEvent<BodyPartComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<BodyPartComponent, EntInsertedIntoContainerMessage>)OnBodyPartInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BodyPartComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<BodyPartComponent, EntRemovedFromContainerMessage>)OnBodyPartRemoved, (Type[])null, (Type[])null);
	}

	private void OnBodyPartInserted(Entity<BodyPartComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		EntityUid insertedUid = ((ContainerModifiedMessage)args).Entity;
		string slotId = ((ContainerModifiedMessage)args).Container.ID;
		EntityUid? body = ent.Comp.Body;
		if (body.HasValue)
		{
			BodyPartComponent part = default(BodyPartComponent);
			if (((EntitySystem)this).TryComp<BodyPartComponent>(insertedUid, ref part))
			{
				AddPart(Entity<BodyComponent>.op_Implicit(ent.Comp.Body.Value), Entity<BodyPartComponent>.op_Implicit((insertedUid, part)), slotId);
				RecursiveBodyUpdate(Entity<BodyPartComponent>.op_Implicit((insertedUid, part)), ent.Comp.Body.Value);
			}
			OrganComponent organ = default(OrganComponent);
			if (((EntitySystem)this).TryComp<OrganComponent>(insertedUid, ref organ))
			{
				AddOrgan(Entity<OrganComponent>.op_Implicit((insertedUid, organ)), ent.Comp.Body.Value, Entity<BodyPartComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnBodyPartRemoved(Entity<BodyPartComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid removedUid = ((ContainerModifiedMessage)args).Entity;
		string slotId = ((ContainerModifiedMessage)args).Container.ID;
		BodyPartComponent part = default(BodyPartComponent);
		if (((EntitySystem)this).TryComp<BodyPartComponent>(removedUid, ref part))
		{
			EntityUid? body = part.Body;
			if (body.HasValue)
			{
				RemovePart(Entity<BodyComponent>.op_Implicit(part.Body.Value), Entity<BodyPartComponent>.op_Implicit((removedUid, part)), slotId);
				RecursiveBodyUpdate(Entity<BodyPartComponent>.op_Implicit((removedUid, part)), null);
			}
		}
		OrganComponent organ = default(OrganComponent);
		if (((EntitySystem)this).TryComp<OrganComponent>(removedUid, ref organ))
		{
			RemoveOrgan(Entity<OrganComponent>.op_Implicit((removedUid, organ)), Entity<BodyPartComponent>.op_Implicit(ent));
		}
	}

	private void RecursiveBodyUpdate(Entity<BodyPartComponent> ent, EntityUid? bodyUid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Body = bodyUid;
		((EntitySystem)this).Dirty(Entity<BodyPartComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		BaseContainer container = default(BaseContainer);
		OrganComponent organComp = default(OrganComponent);
		foreach (string slotId in ent.Comp.Organs.Keys)
		{
			if (!Containers.TryGetContainer(Entity<BodyPartComponent>.op_Implicit(ent), GetOrganContainerId(slotId), ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid organ in container.ContainedEntities)
			{
				if (!((EntitySystem)this).TryComp<OrganComponent>(organ, ref organComp))
				{
					continue;
				}
				((EntitySystem)this).Dirty(organ, (IComponent)(object)organComp, (MetaDataComponent)null);
				EntityUid? body = organComp.Body;
				if (body.HasValue)
				{
					EntityUid oldBodyUid = body.GetValueOrDefault();
					if (((EntityUid)(ref oldBodyUid)).Valid)
					{
						OrganRemovedFromBodyEvent removedEv = new OrganRemovedFromBodyEvent(oldBodyUid, Entity<BodyPartComponent>.op_Implicit(ent));
						((EntitySystem)this).RaiseLocalEvent<OrganRemovedFromBodyEvent>(organ, ref removedEv, false);
					}
				}
				organComp.Body = bodyUid;
				if (bodyUid.HasValue)
				{
					OrganAddedToBodyEvent addedEv = new OrganAddedToBodyEvent(bodyUid.Value, Entity<BodyPartComponent>.op_Implicit(ent));
					((EntitySystem)this).RaiseLocalEvent<OrganAddedToBodyEvent>(organ, ref addedEv, false);
				}
			}
		}
		BaseContainer container2 = default(BaseContainer);
		BodyPartComponent childPart = default(BodyPartComponent);
		foreach (string slotId2 in ent.Comp.Children.Keys)
		{
			if (!Containers.TryGetContainer(Entity<BodyPartComponent>.op_Implicit(ent), GetPartSlotContainerId(slotId2), ref container2, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid containedUid in container2.ContainedEntities)
			{
				if (((EntitySystem)this).TryComp<BodyPartComponent>(containedUid, ref childPart))
				{
					RecursiveBodyUpdate(Entity<BodyPartComponent>.op_Implicit((containedUid, childPart)), bodyUid);
				}
			}
		}
	}

	protected virtual void AddPart(Entity<BodyComponent?> bodyEnt, Entity<BodyPartComponent> partEnt, string slotId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Dirty(Entity<BodyPartComponent>.op_Implicit(partEnt), (IComponent)(object)partEnt.Comp, (MetaDataComponent)null);
		partEnt.Comp.Body = Entity<BodyComponent>.op_Implicit(bodyEnt);
		BodyPartAddedEvent ev = new BodyPartAddedEvent(slotId, partEnt);
		((EntitySystem)this).RaiseLocalEvent<BodyPartAddedEvent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref ev, false);
		AddLeg(partEnt, bodyEnt);
	}

	protected virtual void RemovePart(Entity<BodyComponent?> bodyEnt, Entity<BodyPartComponent> partEnt, string slotId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref bodyEnt.Comp, false);
		((EntitySystem)this).Dirty(Entity<BodyPartComponent>.op_Implicit(partEnt), (IComponent)(object)partEnt.Comp, (MetaDataComponent)null);
		partEnt.Comp.Body = null;
		BodyPartRemovedEvent ev = new BodyPartRemovedEvent(slotId, partEnt);
		((EntitySystem)this).RaiseLocalEvent<BodyPartRemovedEvent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref ev, false);
		RemoveLeg(partEnt, bodyEnt);
		PartRemoveDamage(bodyEnt, partEnt);
	}

	private void AddLeg(Entity<BodyPartComponent> legEnt, Entity<BodyComponent?> bodyEnt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref bodyEnt.Comp, false) && legEnt.Comp.PartType == BodyPartType.Leg)
		{
			bodyEnt.Comp.LegEntities.Add(Entity<BodyPartComponent>.op_Implicit(legEnt));
			UpdateMovementSpeed(Entity<BodyComponent>.op_Implicit(bodyEnt));
			((EntitySystem)this).Dirty(Entity<BodyComponent>.op_Implicit(bodyEnt), (IComponent)(object)bodyEnt.Comp, (MetaDataComponent)null);
		}
	}

	private void RemoveLeg(Entity<BodyPartComponent> legEnt, Entity<BodyComponent?> bodyEnt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref bodyEnt.Comp, false) && legEnt.Comp.PartType == BodyPartType.Leg)
		{
			bodyEnt.Comp.LegEntities.Remove(Entity<BodyPartComponent>.op_Implicit(legEnt));
			UpdateMovementSpeed(Entity<BodyComponent>.op_Implicit(bodyEnt));
			((EntitySystem)this).Dirty(Entity<BodyComponent>.op_Implicit(bodyEnt), (IComponent)(object)bodyEnt.Comp, (MetaDataComponent)null);
			if (!bodyEnt.Comp.LegEntities.Any())
			{
				Standing.Down(Entity<BodyComponent>.op_Implicit(bodyEnt));
			}
		}
	}

	private void PartRemoveDamage(Entity<BodyComponent?> bodyEnt, Entity<BodyPartComponent> partEnt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BodyComponent>(Entity<BodyComponent>.op_Implicit(bodyEnt), ref bodyEnt.Comp, false) && !_timing.ApplyingState && partEnt.Comp.IsVital && !GetBodyChildrenOfType(Entity<BodyComponent>.op_Implicit(bodyEnt), partEnt.Comp.PartType, bodyEnt.Comp).Any())
		{
			DamageSpecifier damage = new DamageSpecifier(Prototypes.Index<DamageTypePrototype>(BloodlossDamageType), 300);
			Damageable.TryChangeDamage(Entity<BodyComponent>.op_Implicit(bodyEnt), damage);
		}
	}

	public EntityUid? GetParentPartOrNull(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!Containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, null)), ref container))
		{
			return null;
		}
		EntityUid parent = container.Owner;
		if (!((EntitySystem)this).HasComp<BodyPartComponent>(parent))
		{
			return null;
		}
		return parent;
	}

	public (EntityUid Parent, string Slot)? GetParentPartAndSlotOrNull(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!Containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(uid, null, null)), ref container))
		{
			return null;
		}
		string slotId = GetPartSlotContainerIdFromContainer(container.ID);
		if (string.IsNullOrEmpty(slotId))
		{
			return null;
		}
		EntityUid parent = container.Owner;
		BodyPartComponent parentBody = default(BodyPartComponent);
		if (!((EntitySystem)this).TryComp<BodyPartComponent>(parent, ref parentBody) || !parentBody.Children.ContainsKey(slotId))
		{
			return null;
		}
		return (parent, slotId);
	}

	public bool TryGetParentBodyPart(EntityUid partUid, [NotNullWhen(true)] out EntityUid? parentUid, [NotNullWhen(true)] out BodyPartComponent? parentComponent)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		parentUid = null;
		parentComponent = null;
		BaseContainer container = default(BaseContainer);
		if (Containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(partUid, null, null)), ref container) && ((EntitySystem)this).TryComp<BodyPartComponent>(container.Owner, ref parentComponent))
		{
			parentUid = container.Owner;
			return true;
		}
		return false;
	}

	private BodyPartSlot? CreatePartSlot(EntityUid partUid, string slotId, BodyPartType partType, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(partUid, ref part, false))
		{
			return null;
		}
		Containers.EnsureContainer<ContainerSlot>(partUid, GetPartSlotContainerId(slotId), (ContainerManagerComponent)null);
		BodyPartSlot partSlot = new BodyPartSlot(slotId, partType);
		part.Children.Add(slotId, partSlot);
		((EntitySystem)this).Dirty(partUid, (IComponent)(object)part, (MetaDataComponent)null);
		return partSlot;
	}

	public bool TryCreatePartSlot(EntityUid? partId, string slotId, BodyPartType partType, [NotNullWhen(true)] out BodyPartSlot? slot, BodyPartComponent? part = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		slot = null;
		if (!partId.HasValue || !((EntitySystem)this).Resolve<BodyPartComponent>(partId.Value, ref part, false))
		{
			return false;
		}
		Containers.EnsureContainer<ContainerSlot>(partId.Value, GetPartSlotContainerId(slotId), (ContainerManagerComponent)null);
		slot = new BodyPartSlot(slotId, partType);
		if (!part.Children.TryAdd(slotId, slot.Value))
		{
			return false;
		}
		((EntitySystem)this).Dirty(partId.Value, (IComponent)(object)part, (MetaDataComponent)null);
		return true;
	}

	public bool TryCreatePartSlotAndAttach(EntityUid parentId, string slotId, EntityUid childId, BodyPartType partType, BodyPartComponent? parent = null, BodyPartComponent? child = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (TryCreatePartSlot(parentId, slotId, partType, out var _, parent))
		{
			return AttachPart(parentId, slotId, childId, parent, child);
		}
		return false;
	}

	public bool IsPartRoot(EntityUid bodyId, EntityUid partId, BodyComponent? body = null, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, true) && ((EntitySystem)this).Resolve<BodyComponent>(bodyId, ref body, true) && Containers.TryGetContainingContainer(bodyId, partId, ref container, (ContainerManagerComponent)null))
		{
			return container.ID == "body_root_part";
		}
		return false;
	}

	public bool CanAttachToRoot(EntityUid bodyId, EntityUid partId, BodyComponent? body = null, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BodyComponent>(bodyId, ref body, true) && ((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, true) && !body.RootContainer.ContainedEntity.HasValue)
		{
			EntityUid? body2 = part.Body;
			if (!body2.HasValue)
			{
				return true;
			}
			return bodyId != body2.GetValueOrDefault();
		}
		return false;
	}

	public (EntityUid Entity, BodyPartComponent BodyPart)? GetRootPartOrNull(EntityUid bodyId, BodyComponent? body = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyComponent>(bodyId, ref body, true) || !body.RootContainer.ContainedEntity.HasValue)
		{
			return null;
		}
		return (body.RootContainer.ContainedEntity.Value, ((EntitySystem)this).Comp<BodyPartComponent>(body.RootContainer.ContainedEntity.Value));
	}

	public bool CanAttachPart(EntityUid parentId, BodyPartSlot slot, EntityUid partId, BodyPartComponent? parentPart = null, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false) && ((EntitySystem)this).Resolve<BodyPartComponent>(parentId, ref parentPart, false))
		{
			return CanAttachPart(parentId, slot.Id, partId, parentPart, part);
		}
		return false;
	}

	public bool CanAttachPart(EntityUid parentId, string slotId, EntityUid partId, BodyPartComponent? parentPart = null, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false) && ((EntitySystem)this).Resolve<BodyPartComponent>(parentId, ref parentPart, false) && parentPart.Children.TryGetValue(slotId, out var parentSlotData) && part.PartType == parentSlotData.Type && Containers.TryGetContainer(parentId, GetPartSlotContainerId(slotId), ref container, (ContainerManagerComponent)null))
		{
			return Containers.CanInsert(partId, container, false, (TransformComponent)null);
		}
		return false;
	}

	public bool AttachPartToRoot(EntityUid bodyId, EntityUid partId, BodyComponent? body = null, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BodyComponent>(bodyId, ref body, true) && ((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, true) && CanAttachToRoot(bodyId, partId, body, part))
		{
			return Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(partId), (BaseContainer)(object)body.RootContainer, (TransformComponent)null, false);
		}
		return false;
	}

	public bool AttachPart(EntityUid parentPartId, string slotId, EntityUid partId, BodyPartComponent? parentPart = null, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BodyPartComponent>(parentPartId, ref parentPart, false) && parentPart.Children.TryGetValue(slotId, out var slot))
		{
			return AttachPart(parentPartId, slot, partId, parentPart, part);
		}
		return false;
	}

	public bool AttachPart(EntityUid parentPartId, BodyPartSlot slot, EntityUid partId, BodyPartComponent? parentPart = null, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(parentPartId, ref parentPart, false) || !((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false) || !CanAttachPart(parentPartId, slot.Id, partId, parentPart, part) || !parentPart.Children.ContainsKey(slot.Id))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!Containers.TryGetContainer(parentPartId, GetPartSlotContainerId(slot.Id), ref container, (ContainerManagerComponent)null))
		{
			return false;
		}
		return Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(partId), container, (TransformComponent)null, false);
	}

	public void UpdateMovementSpeed(EntityUid bodyId, BodyComponent? body = null, MovementSpeedModifierComponent? movement = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyComponent, MovementSpeedModifierComponent>(bodyId, ref body, ref movement, false) || body.RequiredLegs <= 0)
		{
			return;
		}
		float walkSpeed = 0f;
		float sprintSpeed = 0f;
		float acceleration = 0f;
		MovementBodyPartComponent legModifier = default(MovementBodyPartComponent);
		foreach (EntityUid legEntity in body.LegEntities)
		{
			if (((EntitySystem)this).TryComp<MovementBodyPartComponent>(legEntity, ref legModifier))
			{
				walkSpeed += legModifier.WalkSpeed;
				sprintSpeed += legModifier.SprintSpeed;
				acceleration += legModifier.Acceleration;
			}
		}
		walkSpeed /= (float)body.RequiredLegs;
		sprintSpeed /= (float)body.RequiredLegs;
		acceleration /= (float)body.RequiredLegs;
		Movement.ChangeBaseSpeed(bodyId, walkSpeed, sprintSpeed, acceleration, movement);
	}

	public IEnumerable<(EntityUid Id, OrganComponent Component)> GetPartOrgans(EntityUid partId, BodyPartComponent? part = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false))
		{
			yield break;
		}
		BaseContainer container = default(BaseContainer);
		OrganComponent organ = default(OrganComponent);
		foreach (string key in part.Organs.Keys)
		{
			string containerSlotId = GetOrganContainerId(key);
			if (!Containers.TryGetContainer(partId, containerSlotId, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid containedEnt in container.ContainedEntities)
			{
				if (((EntitySystem)this).TryComp<OrganComponent>(containedEnt, ref organ))
				{
					yield return (Id: containedEnt, Component: organ);
				}
			}
		}
	}

	public IEnumerable<BaseContainer> GetPartContainers(EntityUid id, BodyPartComponent? part = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(id, ref part, false) || part.Children.Count == 0)
		{
			yield break;
		}
		BaseContainer container = default(BaseContainer);
		foreach (string key in part.Children.Keys)
		{
			string containerSlotId = GetPartSlotContainerId(key);
			if (!Containers.TryGetContainer(id, containerSlotId, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			yield return container;
			foreach (EntityUid ent in container.ContainedEntities)
			{
				foreach (BaseContainer partContainer in GetPartContainers(ent))
				{
					yield return partContainer;
				}
			}
			container = null;
		}
	}

	public IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyPartChildren(EntityUid partId, BodyPartComponent? part = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false))
		{
			yield break;
		}
		yield return (Id: partId, Component: part);
		BaseContainer container = default(BaseContainer);
		BodyPartComponent childPart = default(BodyPartComponent);
		foreach (string key in part.Children.Keys)
		{
			string containerSlotId = GetPartSlotContainerId(key);
			if (!Containers.TryGetContainer(partId, containerSlotId, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid containedEnt in container.ContainedEntities)
			{
				if (!((EntitySystem)this).TryComp<BodyPartComponent>(containedEnt, ref childPart))
				{
					continue;
				}
				foreach (var bodyPartChild in GetBodyPartChildren(containedEnt, childPart))
				{
					yield return bodyPartChild;
				}
			}
		}
	}

	public IEnumerable<BodyPartSlot> GetAllBodyPartSlots(EntityUid partId, BodyPartComponent? part = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false))
		{
			yield break;
		}
		BaseContainer container = default(BaseContainer);
		BodyPartComponent childPart = default(BodyPartComponent);
		foreach (var (slotId, bodyPartSlot2) in part.Children)
		{
			yield return bodyPartSlot2;
			string containerSlotId = GetOrganContainerId(slotId);
			if (!Containers.TryGetContainer(partId, containerSlotId, ref container, (ContainerManagerComponent)null))
			{
				continue;
			}
			foreach (EntityUid containedEnt in container.ContainedEntities)
			{
				if (!((EntitySystem)this).TryComp<BodyPartComponent>(containedEnt, ref childPart))
				{
					continue;
				}
				foreach (BodyPartSlot allBodyPartSlot in GetAllBodyPartSlots(containedEnt, childPart))
				{
					yield return allBodyPartSlot;
				}
			}
		}
	}

	public bool BodyHasPartType(EntityUid bodyId, BodyPartType type, BodyComponent? body = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetBodyChildrenOfType(bodyId, type, body).Any();
	}

	public bool PartHasChild(EntityUid parentId, EntityUid childId, BodyPartComponent? parent, BodyPartComponent? child)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(parentId, ref parent, false) || !((EntitySystem)this).Resolve<BodyPartComponent>(childId, ref child, false))
		{
			return false;
		}
		foreach (var bodyPartChild in GetBodyPartChildren(parentId, parent))
		{
			if (bodyPartChild.Id == childId)
			{
				return true;
			}
		}
		return false;
	}

	public bool BodyHasChild(EntityUid bodyId, EntityUid partId, BodyComponent? body = null, BodyPartComponent? part = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		BodyPartComponent rootPart = default(BodyPartComponent);
		if (((EntitySystem)this).Resolve<BodyComponent>(bodyId, ref body, false) && body.RootContainer.ContainedEntity.HasValue && ((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false) && ((EntitySystem)this).TryComp<BodyPartComponent>(body.RootContainer.ContainedEntity, ref rootPart))
		{
			return PartHasChild(body.RootContainer.ContainedEntity.Value, partId, rootPart, part);
		}
		return false;
	}

	public IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyChildrenOfType(EntityUid bodyId, BodyPartType type, BodyComponent? body = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		foreach (var part in GetBodyChildren(bodyId, body))
		{
			if (part.Component.PartType == type)
			{
				yield return part;
			}
		}
	}

	public List<(T Comp, OrganComponent Organ)> GetBodyPartOrganComponents<T>(EntityUid uid, BodyPartComponent? part = null) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(uid, ref part, true))
		{
			return new List<(T, OrganComponent)>();
		}
		EntityQuery<T> query = ((EntitySystem)this).GetEntityQuery<T>();
		List<(T, OrganComponent)> list = new List<(T, OrganComponent)>();
		T comp = default(T);
		foreach (var organ in GetPartOrgans(uid, part))
		{
			if (query.TryGetComponent(organ.Id, ref comp))
			{
				list.Add((comp, organ.Component));
			}
		}
		return list;
	}

	public bool TryGetBodyPartOrganComponents<T>(EntityUid uid, [NotNullWhen(true)] out List<(T Comp, OrganComponent Organ)>? comps, BodyPartComponent? part = null) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(uid, ref part, true))
		{
			comps = null;
			return false;
		}
		comps = GetBodyPartOrganComponents<T>(uid, part);
		if (comps.Count != 0)
		{
			return true;
		}
		comps = null;
		return false;
	}

	public IEnumerable<EntityUid> GetBodyPartAdjacentParts(EntityUid partId, BodyPartComponent? part = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false))
		{
			yield break;
		}
		if (TryGetParentBodyPart(partId, out EntityUid? parentUid, out BodyPartComponent _))
		{
			yield return parentUid.Value;
		}
		foreach (string slotId in part.Children.Keys)
		{
			BaseContainer container = Containers.GetContainer(partId, GetPartSlotContainerId(slotId), (ContainerManagerComponent)null);
			foreach (EntityUid containedEntity in container.ContainedEntities)
			{
				yield return containedEntity;
			}
		}
	}

	public IEnumerable<(EntityUid AdjacentId, T Component)> GetBodyPartAdjacentPartsComponents<T>(EntityUid partId, BodyPartComponent? part = null) where T : IComponent
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false))
		{
			yield break;
		}
		EntityQuery<T> query = ((EntitySystem)this).GetEntityQuery<T>();
		T component = default(T);
		foreach (EntityUid adjacentId in GetBodyPartAdjacentParts(partId, part))
		{
			if (query.TryGetComponent(adjacentId, ref component))
			{
				yield return (AdjacentId: adjacentId, Component: component);
			}
		}
	}

	public bool TryGetBodyPartAdjacentPartsComponents<T>(EntityUid partId, [NotNullWhen(true)] out List<(EntityUid AdjacentId, T Component)>? comps, BodyPartComponent? part = null) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<BodyPartComponent>(partId, ref part, false))
		{
			comps = null;
			return false;
		}
		EntityQuery<T> query = ((EntitySystem)this).GetEntityQuery<T>();
		comps = new List<(EntityUid, T)>();
		T component = default(T);
		foreach (EntityUid adjacentId in GetBodyPartAdjacentParts(partId, part))
		{
			if (query.TryGetComponent(adjacentId, ref component))
			{
				comps.Add((adjacentId, component));
			}
		}
		if (comps.Count != 0)
		{
			return true;
		}
		comps = null;
		return false;
	}
}
