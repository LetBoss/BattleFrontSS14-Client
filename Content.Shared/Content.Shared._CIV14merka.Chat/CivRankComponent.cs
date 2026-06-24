using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Chat;

[RegisterComponent]
public sealed class CivRankComponent : Component, ISerializationGenerated<CivRankComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string? CachedRank;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivRankComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CivRankComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CivRankComponent>(this, ref target, hookCtx, false, context))
		{
			string CachedRankTemp = null;
			if (!serialization.TryCustomCopy<string>(CachedRank, ref CachedRankTemp, hookCtx, false, context))
			{
				CachedRankTemp = CachedRank;
			}
			target.CachedRank = CachedRankTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivRankComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivRankComponent cast = (CivRankComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivRankComponent cast = (CivRankComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivRankComponent def = (CivRankComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CivRankComponent Instantiate()
	{
		return new CivRankComponent();
	}
}
