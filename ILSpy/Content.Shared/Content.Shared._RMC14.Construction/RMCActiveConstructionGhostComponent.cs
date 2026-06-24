using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Construction;

[RegisterComponent]
public sealed class RMCActiveConstructionGhostComponent : Component, ISerializationGenerated<RMCActiveConstructionGhostComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Ghost;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCActiveConstructionGhostComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCActiveConstructionGhostComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCActiveConstructionGhostComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? GhostTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Ghost, ref GhostTemp, hookCtx, false, context))
			{
				GhostTemp = serialization.CreateCopy<EntityUid?>(Ghost, hookCtx, context, false);
			}
			target.Ghost = GhostTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCActiveConstructionGhostComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCActiveConstructionGhostComponent cast = (RMCActiveConstructionGhostComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCActiveConstructionGhostComponent cast = (RMCActiveConstructionGhostComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCActiveConstructionGhostComponent def = (RMCActiveConstructionGhostComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCActiveConstructionGhostComponent Instantiate()
	{
		return new RMCActiveConstructionGhostComponent();
	}
}
