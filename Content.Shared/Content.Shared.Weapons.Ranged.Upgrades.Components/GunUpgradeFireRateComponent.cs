using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Weapons.Ranged.Upgrades.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(GunUpgradeSystem) })]
public sealed class GunUpgradeFireRateComponent : Component, ISerializationGenerated<GunUpgradeFireRateComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Coefficient = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GunUpgradeFireRateComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GunUpgradeFireRateComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GunUpgradeFireRateComponent>(this, ref target, hookCtx, false, context))
		{
			float CoefficientTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Coefficient, ref CoefficientTemp, hookCtx, false, context))
			{
				CoefficientTemp = Coefficient;
			}
			target.Coefficient = CoefficientTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GunUpgradeFireRateComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunUpgradeFireRateComponent cast = (GunUpgradeFireRateComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunUpgradeFireRateComponent cast = (GunUpgradeFireRateComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunUpgradeFireRateComponent def = (GunUpgradeFireRateComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GunUpgradeFireRateComponent Instantiate()
	{
		return new GunUpgradeFireRateComponent();
	}
}
