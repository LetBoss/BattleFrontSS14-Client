using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATDamageThresholdReachedSystem : BaseXATSystem<XATDamageThresholdReachedComponent>
{
	[Dependency]
	private IPrototypeManager _prototype;

	public override void Initialize()
	{
		base.Initialize();
		XATSubscribeDirectEvent<DamageChangedEvent>(OnDamageChanged);
	}

	private void OnDamageChanged(Entity<XenoArtifactComponent> artifact, Entity<XATDamageThresholdReachedComponent, XenoArtifactNodeComponent> node, ref DamageChangedEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		if (!args.DamageIncreased || args.DamageDelta == null)
		{
			return;
		}
		EntityUid? origin = args.Origin;
		EntityUid owner = artifact.Owner;
		if (origin.HasValue && origin.GetValueOrDefault() == owner)
		{
			return;
		}
		XATDamageThresholdReachedComponent damageTriggerComponent = node.Comp1;
		if (Timing.IsFirstTimePredicted)
		{
			damageTriggerComponent.AccumulatedDamage += args.DamageDelta;
		}
		FixedPoint2 value;
		foreach (KeyValuePair<ProtoId<DamageTypePrototype>, FixedPoint2> item in damageTriggerComponent.TypesNeeded)
		{
			item.Deconstruct(out var key, out value);
			ProtoId<DamageTypePrototype> type = key;
			FixedPoint2 needed = value;
			if (damageTriggerComponent.AccumulatedDamage.DamageDict.GetValueOrDefault(ProtoId<DamageTypePrototype>.op_Implicit(type)) >= needed)
			{
				InvokeTrigger(artifact, node);
				return;
			}
		}
		foreach (KeyValuePair<ProtoId<DamageGroupPrototype>, FixedPoint2> item2 in damageTriggerComponent.GroupsNeeded)
		{
			item2.Deconstruct(out var key2, out value);
			ProtoId<DamageGroupPrototype> group = key2;
			FixedPoint2 needed2 = value;
			DamageGroupPrototype damageGroupPrototype = _prototype.Index<DamageGroupPrototype>(group);
			if (damageTriggerComponent.AccumulatedDamage.TryGetDamageInGroup(damageGroupPrototype, out var damage) && damage >= needed2)
			{
				InvokeTrigger(artifact, node);
				break;
			}
		}
	}

	private void InvokeTrigger(Entity<XenoArtifactComponent> artifact, Entity<XATDamageThresholdReachedComponent, XenoArtifactNodeComponent> node)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		XATDamageThresholdReachedComponent damageTriggerComponent = node.Comp1;
		damageTriggerComponent.AccumulatedDamage.DamageDict.Clear();
		((EntitySystem)this).Dirty(Entity<XATDamageThresholdReachedComponent, XenoArtifactNodeComponent>.op_Implicit(node), (IComponent)(object)damageTriggerComponent, (MetaDataComponent)null);
		Trigger(artifact, node);
	}
}
