using System;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Implants.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class RattleComponent : Component, ISerializationGenerated<RattleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<RadioChannelPrototype> RadioChannel = ProtoId<RadioChannelPrototype>.op_Implicit("Syndicate");

	[DataField(null, false, 1, false, false, null)]
	public LocId CritMessage = LocId.op_Implicit("deathrattle-implant-critical-message");

	[DataField(null, false, 1, false, false, null)]
	public LocId DeathMessage = LocId.op_Implicit("deathrattle-implant-dead-message");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RattleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RattleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RattleComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<RadioChannelPrototype> RadioChannelTemp = default(ProtoId<RadioChannelPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>>(RadioChannel, ref RadioChannelTemp, hookCtx, false, context))
			{
				RadioChannelTemp = serialization.CreateCopy<ProtoId<RadioChannelPrototype>>(RadioChannel, hookCtx, context, false);
			}
			target.RadioChannel = RadioChannelTemp;
			LocId CritMessageTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(CritMessage, ref CritMessageTemp, hookCtx, false, context))
			{
				CritMessageTemp = serialization.CreateCopy<LocId>(CritMessage, hookCtx, context, false);
			}
			target.CritMessage = CritMessageTemp;
			LocId DeathMessageTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(DeathMessage, ref DeathMessageTemp, hookCtx, false, context))
			{
				DeathMessageTemp = serialization.CreateCopy<LocId>(DeathMessage, hookCtx, context, false);
			}
			target.DeathMessage = DeathMessageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RattleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RattleComponent cast = (RattleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RattleComponent cast = (RattleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RattleComponent def = (RattleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RattleComponent Instantiate()
	{
		return new RattleComponent();
	}
}
