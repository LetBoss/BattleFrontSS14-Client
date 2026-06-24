using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences.Loadouts.Effects;

public sealed class PointsCostLoadoutEffect : LoadoutEffect, ISerializationGenerated<PointsCostLoadoutEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public int Cost = 1;

	public override bool Validate(HumanoidCharacterProfile profile, RoleLoadout loadout, ICommonSession? session, IDependencyCollection collection, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		reason = null;
		RoleLoadoutPrototype roleProto = default(RoleLoadoutPrototype);
		if (!collection.Resolve<IPrototypeManager>().TryIndex<RoleLoadoutPrototype>(loadout.Role, ref roleProto) || !roleProto.Points.HasValue)
		{
			return true;
		}
		if (loadout.Points <= Cost)
		{
			reason = FormattedMessage.FromUnformatted("loadout-group-points-insufficient");
			return false;
		}
		return true;
	}

	public override void Apply(RoleLoadout loadout)
	{
		loadout.Points -= Cost;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PointsCostLoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LoadoutEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PointsCostLoadoutEffect)definitionCast;
		if (!serialization.TryCustomCopy<PointsCostLoadoutEffect>(this, ref target, hookCtx, false, context))
		{
			int CostTemp = 0;
			if (!serialization.TryCustomCopy<int>(Cost, ref CostTemp, hookCtx, false, context))
			{
				CostTemp = Cost;
			}
			target.Cost = CostTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PointsCostLoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PointsCostLoadoutEffect cast = (PointsCostLoadoutEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PointsCostLoadoutEffect cast = (PointsCostLoadoutEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PointsCostLoadoutEffect Instantiate()
	{
		return new PointsCostLoadoutEffect();
	}
}
