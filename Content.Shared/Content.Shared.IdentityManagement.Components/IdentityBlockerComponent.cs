using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.IdentityManagement.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class IdentityBlockerComponent : Component, ISerializationGenerated<IdentityBlockerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Enabled = true;

	[DataField(null, false, 1, false, false, null)]
	public IdentityBlockerCoverage Coverage = IdentityBlockerCoverage.FULL;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IdentityBlockerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (IdentityBlockerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<IdentityBlockerComponent>(this, ref target, hookCtx, false, context))
		{
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			IdentityBlockerCoverage CoverageTemp = IdentityBlockerCoverage.NONE;
			if (!serialization.TryCustomCopy<IdentityBlockerCoverage>(Coverage, ref CoverageTemp, hookCtx, false, context))
			{
				CoverageTemp = Coverage;
			}
			target.Coverage = CoverageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IdentityBlockerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IdentityBlockerComponent cast = (IdentityBlockerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IdentityBlockerComponent cast = (IdentityBlockerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IdentityBlockerComponent def = (IdentityBlockerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override IdentityBlockerComponent Instantiate()
	{
		return new IdentityBlockerComponent();
	}
}
