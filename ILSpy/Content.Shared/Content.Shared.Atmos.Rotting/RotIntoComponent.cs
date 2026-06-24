using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Rotting;

[RegisterComponent]
[NetworkedComponent]
public sealed class RotIntoComponent : Component, ISerializationGenerated<RotIntoComponent>, ISerializationGenerated
{
	[DataField("entity", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string Entity = string.Empty;

	[DataField("stage", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int Stage;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RotIntoComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RotIntoComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RotIntoComponent>(this, ref target, hookCtx, false, context))
		{
			string EntityTemp = null;
			if (Entity == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Entity, ref EntityTemp, hookCtx, false, context))
			{
				EntityTemp = Entity;
			}
			target.Entity = EntityTemp;
			int StageTemp = 0;
			if (!serialization.TryCustomCopy<int>(Stage, ref StageTemp, hookCtx, false, context))
			{
				StageTemp = Stage;
			}
			target.Stage = StageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RotIntoComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RotIntoComponent cast = (RotIntoComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RotIntoComponent cast = (RotIntoComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RotIntoComponent def = (RotIntoComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RotIntoComponent Instantiate()
	{
		return new RotIntoComponent();
	}
}
