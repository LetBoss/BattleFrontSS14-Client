using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.Gibbing.Components;
using Content.Shared.Gibbing.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Gibbing.Systems;

public sealed class GibbingSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	[Dependency]
	private SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedPhysicsSystem _physicsSystem;

	[Dependency]
	private IRobustRandom _random;

	public bool TryGibEntity(Entity<TransformComponent?> outerEntity, Entity<GibbableComponent?> gibbable, GibType gibType, GibContentsOption gibContentsOption, out HashSet<EntityUid> droppedEntities, bool launchGibs = true, Vector2 launchDirection = default(Vector2), float launchImpulse = 0f, float launchImpulseVariance = 0f, Angle launchCone = default(Angle), float randomSpreadMod = 1f, bool playAudio = true, List<string>? allowedContainers = null, List<string>? excludedContainers = null, bool logMissingGibable = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		droppedEntities = new HashSet<EntityUid>();
		return TryGibEntityWithRef(outerEntity, gibbable, gibType, gibContentsOption, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone, randomSpreadMod, playAudio, allowedContainers, excludedContainers, logMissingGibable);
	}

	public unsafe bool TryGibEntityWithRef(Entity<TransformComponent?> outerEntity, Entity<GibbableComponent?> gibbable, GibType gibType, GibContentsOption gibContentsOption, ref HashSet<EntityUid> droppedEntities, bool launchGibs = true, Vector2? launchDirection = null, float launchImpulse = 0f, float launchImpulseVariance = 0f, Angle launchCone = default(Angle), float randomSpreadMod = 1f, bool playAudio = true, List<string>? allowedContainers = null, List<string>? excludedContainers = null, bool logMissingGibable = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<GibbableComponent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibbable.Comp, false))
		{
			DropEntity(gibbable, Entity<TransformComponent>.op_Implicit((Entity<TransformComponent>.op_Implicit(outerEntity), ((EntitySystem)this).Transform(Entity<TransformComponent>.op_Implicit(outerEntity)))), randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
			if (logMissingGibable)
			{
				((EntitySystem)this).Log.Warning($"{((EntitySystem)this).ToPrettyString((EntityUid?)Entity<GibbableComponent>.op_Implicit(gibbable), (MetaDataComponent)null)} does not have a GibbableComponent! This is not required but may cause issues contained items to not be dropped.");
			}
			return false;
		}
		if (gibType == GibType.Skip && gibContentsOption == GibContentsOption.Skip)
		{
			return true;
		}
		if (launchGibs)
		{
			randomSpreadMod = 0f;
		}
		HashSet<BaseContainer> validContainers = new HashSet<BaseContainer>();
		AttemptEntityContentsGibEvent gibContentsAttempt = new AttemptEntityContentsGibEvent(Entity<GibbableComponent>.op_Implicit(gibbable), gibContentsOption, allowedContainers, excludedContainers);
		((EntitySystem)this).RaiseLocalEvent<AttemptEntityContentsGibEvent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibContentsAttempt, false);
		AllContainersEnumerable allContainers = _containerSystem.GetAllContainers(Entity<GibbableComponent>.op_Implicit(gibbable), (ContainerManagerComponent)null);
		AllContainersEnumerator enumerator = ((AllContainersEnumerable)(ref allContainers)).GetEnumerator();
		try
		{
			while (((AllContainersEnumerator)(ref enumerator)).MoveNext())
			{
				BaseContainer container = ((AllContainersEnumerator)(ref enumerator)).Current;
				bool valid = true;
				if (allowedContainers != null)
				{
					valid = allowedContainers.Contains(container.ID);
				}
				if (excludedContainers != null)
				{
					valid = valid && !excludedContainers.Contains(container.ID);
				}
				if (valid)
				{
					validContainers.Add(container);
				}
			}
		}
		finally
		{
			((IDisposable)(*(AllContainersEnumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		switch (gibContentsOption)
		{
		case GibContentsOption.Drop:
			foreach (BaseContainer item in validContainers)
			{
				foreach (EntityUid ent2 in item.ContainedEntities)
				{
					DropEntity(new Entity<GibbableComponent>(ent2, (GibbableComponent)null), outerEntity, randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
				}
			}
			break;
		case GibContentsOption.Gib:
			foreach (BaseContainer item2 in validContainers)
			{
				foreach (EntityUid ent in item2.ContainedEntities)
				{
					GibEntity(new Entity<GibbableComponent>(ent, (GibbableComponent)null), outerEntity, randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
				}
			}
			break;
		}
		switch (gibType)
		{
		case GibType.Drop:
			DropEntity(gibbable, outerEntity, randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
			break;
		case GibType.Gib:
			GibEntity(gibbable, outerEntity, randomSpreadMod, ref droppedEntities, launchGibs, launchDirection, launchImpulse, launchImpulseVariance, launchCone);
			break;
		}
		if (playAudio)
		{
			_audioSystem.PlayPredicted(gibbable.Comp.GibSound, Entity<TransformComponent>.op_Implicit(outerEntity), (EntityUid?)null, (AudioParams?)null);
		}
		if (gibType == GibType.Gib)
		{
			((EntitySystem)this).PredictedQueueDel(gibbable.Owner);
		}
		return true;
	}

	private void DropEntity(Entity<GibbableComponent?> gibbable, Entity<TransformComponent?> parent, float randomSpreadMod, ref HashSet<EntityUid> droppedEntities, bool flingEntity, Vector2? scatterDirection, float scatterImpulse, float scatterImpulseVariance, Angle scatterCone)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		int gibCount = 0;
		if (((EntitySystem)this).Resolve<GibbableComponent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibbable.Comp, false))
		{
			gibCount = gibbable.Comp.GibCount;
		}
		if (!((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(parent), ref parent.Comp, false))
		{
			return;
		}
		AttemptEntityGibEvent gibAttemptEvent = new AttemptEntityGibEvent(Entity<GibbableComponent>.op_Implicit(gibbable), gibCount, GibType.Drop);
		((EntitySystem)this).RaiseLocalEvent<AttemptEntityGibEvent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibAttemptEvent, false);
		switch (gibAttemptEvent.GibType)
		{
		case GibType.Skip:
			return;
		case GibType.Gib:
			GibEntity(gibbable, parent, randomSpreadMod, ref droppedEntities, flingEntity, scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone, deleteTarget: false);
			return;
		}
		_transformSystem.DropNextTo(Entity<TransformComponent>.op_Implicit(gibbable.Owner), parent);
		_transformSystem.SetWorldRotation(Entity<GibbableComponent>.op_Implicit(gibbable), _random.NextAngle());
		droppedEntities.Add(Entity<GibbableComponent>.op_Implicit(gibbable));
		if (flingEntity)
		{
			FlingDroppedEntity(Entity<GibbableComponent>.op_Implicit(gibbable), scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone);
		}
		EntityGibbedEvent gibbedEvent = new EntityGibbedEvent(Entity<GibbableComponent>.op_Implicit(gibbable), new List<EntityUid> { Entity<GibbableComponent>.op_Implicit(gibbable) });
		((EntitySystem)this).RaiseLocalEvent<EntityGibbedEvent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibbedEvent, false);
	}

	private List<EntityUid> GibEntity(Entity<GibbableComponent?> gibbable, Entity<TransformComponent?> parent, float randomSpreadMod, ref HashSet<EntityUid> droppedEntities, bool flingEntity, Vector2? scatterDirection, float scatterImpulse, float scatterImpulseVariance, Angle scatterCone, bool deleteTarget = true)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		List<EntityUid> localGibs = new List<EntityUid>();
		int gibCount = 0;
		int gibProtoCount = 0;
		if (((EntitySystem)this).Resolve<GibbableComponent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibbable.Comp, false))
		{
			gibCount = gibbable.Comp.GibCount;
			gibProtoCount = gibbable.Comp.GibPrototypes.Count;
		}
		if (!((EntitySystem)this).Resolve(Entity<TransformComponent>.op_Implicit(parent), ref parent.Comp, false))
		{
			return new List<EntityUid>();
		}
		AttemptEntityGibEvent gibAttemptEvent = new AttemptEntityGibEvent(Entity<GibbableComponent>.op_Implicit(gibbable), gibCount, GibType.Drop);
		((EntitySystem)this).RaiseLocalEvent<AttemptEntityGibEvent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibAttemptEvent, false);
		switch (gibAttemptEvent.GibType)
		{
		case GibType.Skip:
			return localGibs;
		case GibType.Drop:
			DropEntity(gibbable, parent, randomSpreadMod, ref droppedEntities, flingEntity, scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone);
			localGibs.Add(Entity<GibbableComponent>.op_Implicit(gibbable));
			return localGibs;
		default:
		{
			if (gibbable.Comp != null && gibProtoCount > 0)
			{
				if (flingEntity)
				{
					for (int i = 0; i < gibAttemptEvent.GibletCount; i++)
					{
						if (TryCreateRandomGiblet(gibbable.Comp, parent.Comp.Coordinates, playSound: false, out var giblet, randomSpreadMod))
						{
							FlingDroppedEntity(giblet.Value, scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone);
							droppedEntities.Add(giblet.Value);
						}
					}
				}
				else
				{
					for (int j = 0; j < gibAttemptEvent.GibletCount; j++)
					{
						if (TryCreateRandomGiblet(gibbable.Comp, parent.Comp.Coordinates, playSound: false, out var giblet2, randomSpreadMod))
						{
							droppedEntities.Add(giblet2.Value);
						}
					}
				}
			}
			_transformSystem.AttachToGridOrMap(Entity<GibbableComponent>.op_Implicit(gibbable), ((EntitySystem)this).Transform(Entity<GibbableComponent>.op_Implicit(gibbable)));
			if (flingEntity)
			{
				FlingDroppedEntity(Entity<GibbableComponent>.op_Implicit(gibbable), scatterDirection, scatterImpulse, scatterImpulseVariance, scatterCone);
			}
			EntityGibbedEvent gibbedEvent = new EntityGibbedEvent(Entity<GibbableComponent>.op_Implicit(gibbable), localGibs);
			((EntitySystem)this).RaiseLocalEvent<EntityGibbedEvent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibbedEvent, false);
			if (deleteTarget)
			{
				((EntitySystem)this).PredictedQueueDel(gibbable.Owner);
			}
			return localGibs;
		}
		}
	}

	public bool TryCreateRandomGiblet(Entity<GibbableComponent?> gibbable, [NotNullWhen(true)] out EntityUid? gibletEntity, float randomSpreadModifier = 1f, bool playSound = true)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		gibletEntity = null;
		if (((EntitySystem)this).Resolve<GibbableComponent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibbable.Comp, true))
		{
			return TryCreateRandomGiblet(gibbable.Comp, ((EntitySystem)this).Transform(Entity<GibbableComponent>.op_Implicit(gibbable)).Coordinates, playSound, out gibletEntity, randomSpreadModifier);
		}
		return false;
	}

	public bool TryCreateAndFlingRandomGiblet(Entity<GibbableComponent?> gibbable, [NotNullWhen(true)] out EntityUid? gibletEntity, Vector2 scatterDirection, float force, float scatterImpulseVariance, Angle scatterCone = default(Angle), bool playSound = true)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		gibletEntity = null;
		if (!((EntitySystem)this).Resolve<GibbableComponent>(Entity<GibbableComponent>.op_Implicit(gibbable), ref gibbable.Comp, true) || !TryCreateRandomGiblet(gibbable.Comp, ((EntitySystem)this).Transform(Entity<GibbableComponent>.op_Implicit(gibbable)).Coordinates, playSound, out gibletEntity))
		{
			return false;
		}
		FlingDroppedEntity(gibletEntity.Value, scatterDirection, force, scatterImpulseVariance, scatterCone);
		return true;
	}

	private void FlingDroppedEntity(EntityUid target, Vector2? direction, float impulse, float impulseVariance, Angle scatterConeAngle)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Angle scatterAngle = (direction.HasValue ? DirectionExtensions.ToAngle(direction.GetValueOrDefault()) : _random.NextAngle());
		Angle val = _random.NextAngle(scatterAngle - Angle.op_Implicit(Angle.op_Implicit(scatterConeAngle) / 2.0), scatterAngle + Angle.op_Implicit(Angle.op_Implicit(scatterConeAngle) / 2.0));
		Vector2 scatterVector = ((Angle)(ref val)).ToVec() * (impulse + _random.NextFloat(impulseVariance));
		_physicsSystem.ApplyLinearImpulse(target, scatterVector, (FixturesComponent)null, (PhysicsComponent)null);
	}

	private bool TryCreateRandomGiblet(GibbableComponent gibbable, EntityCoordinates coords, bool playSound, [NotNullWhen(true)] out EntityUid? gibletEntity, float? randomSpreadModifier = null)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		gibletEntity = null;
		if (gibbable.GibPrototypes.Count == 0)
		{
			return false;
		}
		gibletEntity = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(gibbable.GibPrototypes[_random.Next(0, gibbable.GibPrototypes.Count)]), (!randomSpreadModifier.HasValue) ? coords : ((EntityCoordinates)(ref coords)).Offset(_random.NextVector2(gibbable.GibScatterRange * randomSpreadModifier.Value)));
		if (playSound)
		{
			_audioSystem.PlayPredicted(gibbable.GibSound, coords, (EntityUid?)null, (AudioParams?)null);
		}
		_transformSystem.SetWorldRotation(gibletEntity.Value, _random.NextAngle());
		return true;
	}
}
