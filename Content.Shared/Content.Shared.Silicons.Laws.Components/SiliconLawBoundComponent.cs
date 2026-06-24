using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Silicons.Laws.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedSiliconLawSystem) })]
public sealed class SiliconLawBoundComponent : Component, ISerializationGenerated<SiliconLawBoundComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? LastLawProvider;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SiliconLawBoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SiliconLawBoundComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SiliconLawBoundComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? LastLawProviderTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(LastLawProvider, ref LastLawProviderTemp, hookCtx, false, context))
			{
				LastLawProviderTemp = serialization.CreateCopy<EntityUid?>(LastLawProvider, hookCtx, context, false);
			}
			target.LastLawProvider = LastLawProviderTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SiliconLawBoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawBoundComponent cast = (SiliconLawBoundComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawBoundComponent cast = (SiliconLawBoundComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawBoundComponent def = (SiliconLawBoundComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SiliconLawBoundComponent Instantiate()
	{
		return new SiliconLawBoundComponent();
	}
}
