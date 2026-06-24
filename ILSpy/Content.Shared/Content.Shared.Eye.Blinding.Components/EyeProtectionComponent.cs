using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Eye.Blinding.Components;

[RegisterComponent]
public sealed class EyeProtectionComponent : Component, ISerializationGenerated<EyeProtectionComponent>, ISerializationGenerated
{
	[DataField("protectionTime", false, 1, false, false, null)]
	public TimeSpan ProtectionTime = TimeSpan.FromSeconds(10L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EyeProtectionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EyeProtectionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<EyeProtectionComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan ProtectionTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(ProtectionTime, ref ProtectionTimeTemp, hookCtx, false, context))
			{
				ProtectionTimeTemp = serialization.CreateCopy<TimeSpan>(ProtectionTime, hookCtx, context, false);
			}
			target.ProtectionTime = ProtectionTimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EyeProtectionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EyeProtectionComponent cast = (EyeProtectionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EyeProtectionComponent cast = (EyeProtectionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EyeProtectionComponent def = (EyeProtectionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EyeProtectionComponent Instantiate()
	{
		return new EyeProtectionComponent();
	}
}
