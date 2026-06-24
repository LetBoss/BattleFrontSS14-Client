using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared._PUBG.Detonator;

[RegisterComponent]
public sealed class PubgRemoteDetonatorComponent : Component, ISerializationGenerated<PubgRemoteDetonatorComponent>, ISerializationGenerated
{
	[ViewVariables]
	public List<EntityUid> Bound = new List<EntityUid>();

	[DataField(null, false, 1, false, false, null)]
	public int MaxBound = 30;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgRemoteDetonatorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgRemoteDetonatorComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgRemoteDetonatorComponent>(this, ref target, hookCtx, false, context))
		{
			int MaxBoundTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxBound, ref MaxBoundTemp, hookCtx, false, context))
			{
				MaxBoundTemp = MaxBound;
			}
			target.MaxBound = MaxBoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgRemoteDetonatorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgRemoteDetonatorComponent cast = (PubgRemoteDetonatorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgRemoteDetonatorComponent cast = (PubgRemoteDetonatorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgRemoteDetonatorComponent def = (PubgRemoteDetonatorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgRemoteDetonatorComponent Instantiate()
	{
		return new PubgRemoteDetonatorComponent();
	}
}
