using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Dragon;

[NetworkedComponent]
[EntityCategory(new string[] { "Spawner" })]
public abstract class SharedDragonRiftComponent : Component, ISerializationGenerated<SharedDragonRiftComponent>, ISerializationGenerated
{
	[DataField("state", false, 1, false, false, null)]
	public DragonRiftState State;

	public SharedDragonRiftComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedDragonRiftComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedDragonRiftComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SharedDragonRiftComponent>(this, ref target, hookCtx, false, context))
		{
			DragonRiftState StateTemp = DragonRiftState.Charging;
			if (!serialization.TryCustomCopy<DragonRiftState>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target.State = StateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedDragonRiftComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedDragonRiftComponent cast = (SharedDragonRiftComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedDragonRiftComponent cast = (SharedDragonRiftComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedDragonRiftComponent def = (SharedDragonRiftComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedDragonRiftComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
