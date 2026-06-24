using System;
using System.Collections.Generic;
using Content.Shared.Damage;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedCursedMaskSystem) })]
public sealed class CursedMaskComponent : Component, ISerializationGenerated<CursedMaskComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public CursedMaskExpression CurrentState;

	[DataField(null, false, 1, false, false, null)]
	public float JoySpeedModifier = 1.15f;

	[DataField(null, false, 1, false, false, null)]
	public DamageModifierSet DespairDamageModifier = new DamageModifierSet();

	[DataField(null, false, 1, false, false, null)]
	public bool HasNpc;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? StolenMind;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<NpcFactionPrototype> CursedMaskFaction = ProtoId<NpcFactionPrototype>.op_Implicit("SimpleHostile");

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<NpcFactionPrototype>> OldFactions = new HashSet<ProtoId<NpcFactionPrototype>>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CursedMaskComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CursedMaskComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<CursedMaskComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		CursedMaskExpression CurrentStateTemp = CursedMaskExpression.Neutral;
		if (!serialization.TryCustomCopy<CursedMaskExpression>(CurrentState, ref CurrentStateTemp, hookCtx, false, context))
		{
			CurrentStateTemp = CurrentState;
		}
		target.CurrentState = CurrentStateTemp;
		float JoySpeedModifierTemp = 0f;
		if (!serialization.TryCustomCopy<float>(JoySpeedModifier, ref JoySpeedModifierTemp, hookCtx, false, context))
		{
			JoySpeedModifierTemp = JoySpeedModifier;
		}
		target.JoySpeedModifier = JoySpeedModifierTemp;
		DamageModifierSet DespairDamageModifierTemp = null;
		if (DespairDamageModifier == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<DamageModifierSet>(DespairDamageModifier, ref DespairDamageModifierTemp, hookCtx, true, context))
		{
			if (DespairDamageModifier == null)
			{
				DespairDamageModifierTemp = null;
			}
			else
			{
				serialization.CopyTo<DamageModifierSet>(DespairDamageModifier, ref DespairDamageModifierTemp, hookCtx, context, true);
			}
		}
		target.DespairDamageModifier = DespairDamageModifierTemp;
		bool HasNpcTemp = false;
		if (!serialization.TryCustomCopy<bool>(HasNpc, ref HasNpcTemp, hookCtx, false, context))
		{
			HasNpcTemp = HasNpc;
		}
		target.HasNpc = HasNpcTemp;
		EntityUid? StolenMindTemp = null;
		if (!serialization.TryCustomCopy<EntityUid?>(StolenMind, ref StolenMindTemp, hookCtx, false, context))
		{
			StolenMindTemp = serialization.CreateCopy<EntityUid?>(StolenMind, hookCtx, context, false);
		}
		target.StolenMind = StolenMindTemp;
		ProtoId<NpcFactionPrototype> CursedMaskFactionTemp = default(ProtoId<NpcFactionPrototype>);
		if (!serialization.TryCustomCopy<ProtoId<NpcFactionPrototype>>(CursedMaskFaction, ref CursedMaskFactionTemp, hookCtx, false, context))
		{
			CursedMaskFactionTemp = serialization.CreateCopy<ProtoId<NpcFactionPrototype>>(CursedMaskFaction, hookCtx, context, false);
		}
		target.CursedMaskFaction = CursedMaskFactionTemp;
		HashSet<ProtoId<NpcFactionPrototype>> OldFactionsTemp = null;
		if (OldFactions == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<HashSet<ProtoId<NpcFactionPrototype>>>(OldFactions, ref OldFactionsTemp, hookCtx, true, context))
		{
			OldFactionsTemp = serialization.CreateCopy<HashSet<ProtoId<NpcFactionPrototype>>>(OldFactions, hookCtx, context, false);
		}
		target.OldFactions = OldFactionsTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CursedMaskComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CursedMaskComponent cast = (CursedMaskComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CursedMaskComponent cast = (CursedMaskComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CursedMaskComponent def = (CursedMaskComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CursedMaskComponent Instantiate()
	{
		return new CursedMaskComponent();
	}
}
