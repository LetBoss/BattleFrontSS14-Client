using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Shields;

[RegisterComponent]
[NetworkedComponent]
public sealed class KingShieldComponent : Component, ISerializationGenerated<KingShieldComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float MaxDamagePercent = 0.1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref KingShieldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (KingShieldComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<KingShieldComponent>(this, ref target, hookCtx, false, context))
		{
			float MaxDamagePercentTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxDamagePercent, ref MaxDamagePercentTemp, hookCtx, false, context))
			{
				MaxDamagePercentTemp = MaxDamagePercent;
			}
			target.MaxDamagePercent = MaxDamagePercentTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref KingShieldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		KingShieldComponent cast = (KingShieldComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		KingShieldComponent cast = (KingShieldComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		KingShieldComponent def = (KingShieldComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override KingShieldComponent Instantiate()
	{
		return new KingShieldComponent();
	}
}
