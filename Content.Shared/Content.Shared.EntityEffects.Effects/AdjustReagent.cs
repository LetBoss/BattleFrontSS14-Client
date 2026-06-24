using System;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.EntityEffects.Effects;

public sealed class AdjustReagent : EntityEffect, ISerializationGenerated<AdjustReagent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
	public string? Reagent;

	[DataField(null, false, 1, false, false, typeof(PrototypeIdSerializer<MetabolismGroupPrototype>))]
	public string? Group;

	[DataField(null, false, 1, true, false, null)]
	public FixedPoint2 Amount;

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			if (reagentArgs.Source == null)
			{
				return;
			}
			FixedPoint2 amount = Amount;
			amount *= reagentArgs.Scale;
			if (Reagent != null)
			{
				if (amount < 0 && reagentArgs.Source.ContainsPrototype(Reagent))
				{
					reagentArgs.Source.RemoveReagent(Reagent, -amount);
				}
				if (amount > 0)
				{
					reagentArgs.Source.AddReagent(Reagent, amount);
				}
			}
			else
			{
				if (Group == null)
				{
					return;
				}
				RMCReagentSystem reagentSys = IoCManager.Resolve<IEntityManager>().System<RMCReagentSystem>();
				ReagentQuantity[] array = reagentArgs.Source.Contents.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					ReagentQuantity quant = array[i];
					Reagent proto = reagentSys.Index(ProtoId<ReagentPrototype>.op_Implicit(quant.Reagent.Prototype));
					if (proto.Metabolisms != null && proto.Metabolisms.ContainsKey(ProtoId<MetabolismGroupPrototype>.op_Implicit(Group)))
					{
						if (amount < 0)
						{
							reagentArgs.Source.RemoveReagent(quant.Reagent, amount);
						}
						if (amount > 0)
						{
							reagentArgs.Source.AddReagent(quant.Reagent, amount);
						}
					}
				}
			}
			return;
		}
		throw new NotImplementedException();
	}

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (Reagent != null && entSys.GetEntitySystem<RMCReagentSystem>().TryIndex(ProtoId<ReagentPrototype>.op_Implicit(Reagent), out Reagent reagentProto))
		{
			return Loc.GetString("reagent-effect-guidebook-adjust-reagent-reagent", new(string, object)[4]
			{
				("chance", Probability),
				("deltasign", MathF.Sign(Amount.Float())),
				("reagent", reagentProto.LocalizedName),
				("amount", MathF.Abs(Amount.Float()))
			});
		}
		MetabolismGroupPrototype groupProto = default(MetabolismGroupPrototype);
		if (Group != null && prototype.TryIndex<MetabolismGroupPrototype>(Group, ref groupProto))
		{
			return Loc.GetString("reagent-effect-guidebook-adjust-reagent-group", new(string, object)[4]
			{
				("chance", Probability),
				("deltasign", MathF.Sign(Amount.Float())),
				("group", groupProto.LocalizedName),
				("amount", MathF.Abs(Amount.Float()))
			});
		}
		throw new NotImplementedException();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AdjustReagent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AdjustReagent)definitionCast;
		if (!serialization.TryCustomCopy<AdjustReagent>(this, ref target, hookCtx, false, context))
		{
			string ReagentTemp = null;
			if (!serialization.TryCustomCopy<string>(Reagent, ref ReagentTemp, hookCtx, false, context))
			{
				ReagentTemp = Reagent;
			}
			target.Reagent = ReagentTemp;
			string GroupTemp = null;
			if (!serialization.TryCustomCopy<string>(Group, ref GroupTemp, hookCtx, false, context))
			{
				GroupTemp = Group;
			}
			target.Group = GroupTemp;
			FixedPoint2 AmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = serialization.CreateCopy<FixedPoint2>(Amount, hookCtx, context, false);
			}
			target.Amount = AmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AdjustReagent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AdjustReagent cast = (AdjustReagent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AdjustReagent cast = (AdjustReagent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AdjustReagent Instantiate()
	{
		return new AdjustReagent();
	}
}
