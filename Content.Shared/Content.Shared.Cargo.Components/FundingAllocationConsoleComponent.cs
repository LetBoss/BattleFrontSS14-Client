using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Cargo.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedCargoSystem) })]
public sealed class FundingAllocationConsoleComponent : Component, ISerializationGenerated<FundingAllocationConsoleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier SetDistributionSound = (SoundSpecifier)new SoundCollectionSpecifier("CargoPing", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FundingAllocationConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FundingAllocationConsoleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FundingAllocationConsoleComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier SetDistributionSoundTemp = null;
			if (SetDistributionSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(SetDistributionSound, ref SetDistributionSoundTemp, hookCtx, true, context))
			{
				SetDistributionSoundTemp = serialization.CreateCopy<SoundSpecifier>(SetDistributionSound, hookCtx, context, false);
			}
			target.SetDistributionSound = SetDistributionSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FundingAllocationConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FundingAllocationConsoleComponent cast = (FundingAllocationConsoleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FundingAllocationConsoleComponent cast = (FundingAllocationConsoleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FundingAllocationConsoleComponent def = (FundingAllocationConsoleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FundingAllocationConsoleComponent Instantiate()
	{
		return new FundingAllocationConsoleComponent();
	}
}
