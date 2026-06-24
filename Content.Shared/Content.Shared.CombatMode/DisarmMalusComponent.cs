using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.CombatMode;

[RegisterComponent]
[NetworkedComponent]
public sealed class DisarmMalusComponent : Component, ISerializationGenerated<DisarmMalusComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Malus = 0.3f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DisarmMalusComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DisarmMalusComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<DisarmMalusComponent>(this, ref target, hookCtx, false, context))
		{
			float MalusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Malus, ref MalusTemp, hookCtx, false, context))
			{
				MalusTemp = Malus;
			}
			target.Malus = MalusTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DisarmMalusComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DisarmMalusComponent cast = (DisarmMalusComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DisarmMalusComponent cast = (DisarmMalusComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DisarmMalusComponent def = (DisarmMalusComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DisarmMalusComponent Instantiate()
	{
		return new DisarmMalusComponent();
	}
}
