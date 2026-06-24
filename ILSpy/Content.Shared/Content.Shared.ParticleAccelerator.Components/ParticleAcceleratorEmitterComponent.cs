using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.ParticleAccelerator.Components;

[RegisterComponent]
public sealed class ParticleAcceleratorEmitterComponent : Component, ISerializationGenerated<ParticleAcceleratorEmitterComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId EmittedPrototype = EntProtoId.op_Implicit("ParticlesProjectile");

	[DataField("emitterType", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ParticleAcceleratorEmitterType Type = ParticleAcceleratorEmitterType.Fore;

	public override string ToString()
	{
		return ((object)this).ToString() + $" EmitterType:{Type}";
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ParticleAcceleratorEmitterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ParticleAcceleratorEmitterComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ParticleAcceleratorEmitterComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId EmittedPrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(EmittedPrototype, ref EmittedPrototypeTemp, hookCtx, false, context))
			{
				EmittedPrototypeTemp = serialization.CreateCopy<EntProtoId>(EmittedPrototype, hookCtx, context, false);
			}
			target.EmittedPrototype = EmittedPrototypeTemp;
			ParticleAcceleratorEmitterType TypeTemp = ParticleAcceleratorEmitterType.Port;
			if (!serialization.TryCustomCopy<ParticleAcceleratorEmitterType>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = Type;
			}
			target.Type = TypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ParticleAcceleratorEmitterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParticleAcceleratorEmitterComponent cast = (ParticleAcceleratorEmitterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParticleAcceleratorEmitterComponent cast = (ParticleAcceleratorEmitterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParticleAcceleratorEmitterComponent def = (ParticleAcceleratorEmitterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ParticleAcceleratorEmitterComponent Instantiate()
	{
		return new ParticleAcceleratorEmitterComponent();
	}
}
