using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Rangefinder;
using Content.Shared._RMC14.Rangefinder.Spotting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Targeting;

public abstract class SharedRMCTargetingSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedTransformSystem _transform;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<TargetingComponent, ComponentRemove>((EntityEventRefHandler<TargetingComponent, ComponentRemove>)OnTargetingRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TargetingComponent, DroppedEvent>((EntityEventRefHandler<TargetingComponent, DroppedEvent>)OnTargetingDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TargetingComponent, RMCDroppedEvent>((EntityEventRefHandler<TargetingComponent, RMCDroppedEvent>)OnTargetingDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TargetingComponent, GotUnequippedHandEvent>((EntityEventRefHandler<TargetingComponent, GotUnequippedHandEvent>)OnTargetingDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TargetingComponent, HandDeselectedEvent>((EntityEventRefHandler<TargetingComponent, HandDeselectedEvent>)OnTargetingDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTargetedComponent, ComponentRemove>((EntityEventRefHandler<RMCTargetedComponent, ComponentRemove>)OnTargetedRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCTargetedComponent, EntityTerminatingEvent>((EntityEventRefHandler<RMCTargetedComponent, EntityTerminatingEvent>)OnTargetedRemove, (Type[])null, (Type[])null);
	}

	private void OnTargetingRemoved<T>(Entity<TargetingComponent> targeting, ref T args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			RangefinderComponent rangefinder = default(RangefinderComponent);
			if (((EntitySystem)this).TryComp<RangefinderComponent>(Entity<TargetingComponent>.op_Implicit(targeting), ref rangefinder))
			{
				_appearance.SetData(Entity<TargetingComponent>.op_Implicit(targeting), (Enum)RangefinderLayers.Layer, (object)rangefinder.Mode, (AppearanceComponent)null);
			}
			while (targeting.Comp.Targets.Count > 0)
			{
				StopTargeting(Entity<TargetingComponent>.op_Implicit((Entity<TargetingComponent>.op_Implicit(targeting), Entity<TargetingComponent>.op_Implicit(targeting))), targeting.Comp.Targets[0]);
			}
		}
	}

	private void OnTargetingDropped<T>(Entity<TargetingComponent> targeting, ref T args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		TargetingCancelledEvent ev = default(TargetingCancelledEvent);
		((EntitySystem)this).RaiseLocalEvent<TargetingCancelledEvent>(Entity<TargetingComponent>.op_Implicit(targeting), ref ev, false);
		while (targeting.Comp.Targets.Count > 0)
		{
			StopTargeting(Entity<TargetingComponent>.op_Implicit((Entity<TargetingComponent>.op_Implicit(targeting), Entity<TargetingComponent>.op_Implicit(targeting))), targeting.Comp.Targets[0]);
		}
	}

	private void OnTargetedRemove<T>(Entity<RMCTargetedComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		TargetingComponent targetingComp = default(TargetingComponent);
		foreach (EntityUid targeting in ent.Comp.TargetedBy)
		{
			if (((EntitySystem)this).TryComp<TargetingComponent>(targeting, ref targetingComp))
			{
				targetingComp.Targets.Remove(Entity<RMCTargetedComponent>.op_Implicit(ent));
				targetingComp.LaserDurations.Remove(Entity<RMCTargetedComponent>.op_Implicit(ent));
				targetingComp.OriginalLaserDurations.Remove(Entity<RMCTargetedComponent>.op_Implicit(ent));
				((EntitySystem)this).Dirty(targeting, (IComponent)(object)targetingComp, (MetaDataComponent)null);
			}
		}
	}

	public void StopTargeting(Entity<TargetingComponent?> targeting, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<TargetingComponent>(Entity<TargetingComponent>.op_Implicit(targeting), ref targeting.Comp, false))
		{
			return;
		}
		targeting.Comp.Targets.Remove(target);
		((EntitySystem)this).Dirty<TargetingComponent>(targeting, (MetaDataComponent)null);
		RMCTargetedComponent targeted = default(RMCTargetedComponent);
		if (!((EntitySystem)this).TryComp<RMCTargetedComponent>(target, ref targeted))
		{
			return;
		}
		targeted.TargetedBy.Remove(Entity<TargetingComponent>.op_Implicit(targeting));
		((EntitySystem)this).Dirty(target, (IComponent)(object)targeted, (MetaDataComponent)null);
		TargetedEffects highestMark = TargetedEffects.None;
		DirectionTargetedEffects highestDirection = DirectionTargetedEffects.None;
		bool spotters = false;
		TargetingComponent comp = default(TargetingComponent);
		foreach (EntityUid activeLaser in targeted.TargetedBy)
		{
			if (((EntitySystem)this).TryComp<TargetingComponent>(activeLaser, ref comp))
			{
				if ((int)comp.LaserType > (int)highestMark)
				{
					highestMark = comp.LaserType;
				}
				if ((int)comp.DirectionEffect > (int)highestDirection)
				{
					highestDirection = comp.DirectionEffect;
				}
				if (comp.LaserType == TargetedEffects.Spotted)
				{
					spotters = true;
				}
			}
		}
		if (!spotters)
		{
			((EntitySystem)this).RemComp<SpottedComponent>(target);
		}
		UpdateTargetMarker(target, highestMark, highestDirection, force: true);
		if (targeted.TargetedBy.Count == 0)
		{
			((EntitySystem)this).RemComp<RMCTargetedComponent>(target);
			if (targeting.Comp.Targets.Count == 0)
			{
				((EntitySystem)this).RemComp<TargetingComponent>(Entity<TargetingComponent>.op_Implicit(targeting));
			}
		}
	}

	public void Target(EntityUid equipment, EntityUid user, EntityUid target, float targetingDuration, TargetedEffects targetedEffect = TargetedEffects.None, DirectionTargetedEffects directionEffect = DirectionTargetedEffects.None)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		TargetingStartedEvent ev = new TargetingStartedEvent(directionEffect, targetedEffect, target);
		((EntitySystem)this).RaiseLocalEvent<TargetingStartedEvent>(equipment, ref ev, false);
		targetedEffect = ev.TargetedEffect;
		directionEffect = ev.DirectionEffect;
		RMCTargetedComponent targeted = ((EntitySystem)this).EnsureComp<RMCTargetedComponent>(target);
		targeted.TargetedBy.Add(equipment);
		((EntitySystem)this).Dirty(target, (IComponent)(object)targeted, (MetaDataComponent)null);
		TargetingComponent targeting = ((EntitySystem)this).EnsureComp<TargetingComponent>(equipment);
		if (!targeting.LaserDurations.TryAdd(target, new List<float> { targetingDuration }))
		{
			targeting.LaserDurations[target].Add(targetingDuration);
		}
		if (!targeting.OriginalLaserDurations.TryAdd(target, new List<float> { targetingDuration }))
		{
			targeting.OriginalLaserDurations[target].Add(targetingDuration);
		}
		targeting.Source = equipment;
		targeting.Targets.Add(target);
		targeting.Origin = ((EntitySystem)this).Transform(user).Coordinates;
		targeting.User = user;
		targeting.LaserType = targetedEffect;
		targeting.DirectionEffect = directionEffect;
		((EntitySystem)this).Dirty(equipment, (IComponent)(object)targeting, (MetaDataComponent)null);
		GotTargetedEvent ev2 = new GotTargetedEvent();
		((EntitySystem)this).RaiseLocalEvent<GotTargetedEvent>(target, ref ev2, false);
		UpdateTargetMarker(target, targetedEffect, directionEffect);
	}

	private void UpdateTargetMarker(EntityUid target, TargetedEffects newMarker, DirectionTargetedEffects directionEffect, bool force = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		TargetedEffects marker = default(TargetedEffects);
		_appearance.TryGetData<TargetedEffects>(target, (Enum)TargetedVisuals.Targeted, ref marker, (AppearanceComponent)null);
		if (force || (int)newMarker > (int)marker)
		{
			_appearance.SetData(target, (Enum)TargetedVisuals.Targeted, (object)newMarker, (AppearanceComponent)null);
		}
		bool directionVisual = (int)directionEffect > 0;
		bool directionVisualIntense = (int)directionEffect > 1;
		_appearance.SetData(target, (Enum)TargetedVisuals.TargetedDirection, (object)(directionVisual && !directionVisualIntense), (AppearanceComponent)null);
		_appearance.SetData(target, (Enum)TargetedVisuals.TargetedDirectionIntense, (object)(directionVisual && directionVisualIntense), (AppearanceComponent)null);
	}

	private void RemoveLaser(Entity<TargetingComponent> ent, EntityUid target, int laserNumber)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.LaserDurations[target].RemoveAt(laserNumber);
		ent.Comp.OriginalLaserDurations[target].RemoveAt(laserNumber);
		if (ent.Comp.LaserDurations[target].Count <= 0)
		{
			ent.Comp.LaserDurations.Remove(target);
			ent.Comp.OriginalLaserDurations.Remove(target);
		}
		((EntitySystem)this).Dirty<TargetingComponent>(ent, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<TargetingComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<TargetingComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		TargetingComponent targeting = default(TargetingComponent);
		TransformComponent xform = default(TransformComponent);
		RMCTargetedComponent targeted = default(RMCTargetedComponent);
		TransformComponent targetTransform = default(TransformComponent);
		TransformComponent parentTransform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref targeting, ref xform))
		{
			int targetNumber = 0;
			List<EntityUid> checkedTargets = new List<EntityUid>();
			while (targetNumber < targeting.Targets.Count)
			{
				EntityUid target = targeting.Targets[targetNumber];
				if (!targeting.LaserDurations.Keys.Contains(target) || checkedTargets.Contains(target))
				{
					targetNumber++;
					continue;
				}
				checkedTargets.Add(target);
				for (int laserNumber = 0; laserNumber < targeting.LaserDurations[target].Count; laserNumber++)
				{
					targeting.LaserDurations[target][laserNumber] -= frameTime;
					((EntitySystem)this).Dirty(uid, (IComponent)(object)targeting, (MetaDataComponent)null);
					if (((EntitySystem)this).TryComp<RMCTargetedComponent>(target, ref targeted))
					{
						float newAlpha = 1f - targeting.LaserDurations[target][laserNumber] / targeting.OriginalLaserDurations[target][laserNumber];
						targeted.AlphaMultipliers.TryAdd(uid, newAlpha);
						if (newAlpha > targeted.AlphaMultipliers[uid])
						{
							targeted.AlphaMultipliers[uid] = newAlpha;
						}
					}
					if (targeting.LaserDurations[target][laserNumber] <= 0f)
					{
						RemoveLaser(Entity<TargetingComponent>.op_Implicit((uid, targeting)), target, laserNumber);
						if (((EntitySystem)this).TryComp(target, ref targetTransform))
						{
							TargetingFinishedEvent ev = new TargetingFinishedEvent(targeting.User, targetTransform.Coordinates, target);
							((EntitySystem)this).RaiseLocalEvent<TargetingFinishedEvent>(uid, ref ev, false);
						}
						break;
					}
				}
				targetNumber++;
			}
			if (((EntitySystem)this).TryComp(xform.ParentUid, ref parentTransform) && !_transform.InRange(parentTransform.Coordinates, targeting.Origin, 0.1f))
			{
				TargetingCancelledEvent ev2 = default(TargetingCancelledEvent);
				((EntitySystem)this).RaiseLocalEvent<TargetingCancelledEvent>(uid, ref ev2, false);
				targeting.LaserDurations.Clear();
				targeting.OriginalLaserDurations.Clear();
				((EntitySystem)this).Dirty(uid, (IComponent)(object)targeting, (MetaDataComponent)null);
				while (targeting.Targets.Count > 0)
				{
					StopTargeting(Entity<TargetingComponent>.op_Implicit((uid, targeting)), targeting.Targets[0]);
				}
			}
		}
	}
}
