using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Sound.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class EmitSoundOnActivateComponent : BaseEmitSoundComponent, ISerializationGenerated<EmitSoundOnActivateComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Handle = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EmitSoundOnActivateComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BaseEmitSoundComponent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EmitSoundOnActivateComponent)definitionCast;
		if (!serialization.TryCustomCopy<EmitSoundOnActivateComponent>(this, ref target, hookCtx, false, context))
		{
			bool HandleTemp = false;
			if (!serialization.TryCustomCopy<bool>(Handle, ref HandleTemp, hookCtx, false, context))
			{
				HandleTemp = Handle;
			}
			target.Handle = HandleTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EmitSoundOnActivateComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref BaseEmitSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmitSoundOnActivateComponent cast = (EmitSoundOnActivateComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmitSoundOnActivateComponent cast = (EmitSoundOnActivateComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmitSoundOnActivateComponent def = (EmitSoundOnActivateComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EmitSoundOnActivateComponent Instantiate()
	{
		return new EmitSoundOnActivateComponent();
	}
}
