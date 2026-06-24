using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Markings;

[RegisterComponent]
[NetworkedComponent]
public sealed class PubgMarkingItemComponent : Component, ISerializationGenerated<PubgMarkingItemComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string MarkingId = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string TraitCategory = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgMarkingItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgMarkingItemComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgMarkingItemComponent>(this, ref target, hookCtx, false, context))
		{
			string MarkingIdTemp = null;
			if (MarkingId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(MarkingId, ref MarkingIdTemp, hookCtx, false, context))
			{
				MarkingIdTemp = MarkingId;
			}
			target.MarkingId = MarkingIdTemp;
			string TraitCategoryTemp = null;
			if (TraitCategory == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(TraitCategory, ref TraitCategoryTemp, hookCtx, false, context))
			{
				TraitCategoryTemp = TraitCategory;
			}
			target.TraitCategory = TraitCategoryTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgMarkingItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgMarkingItemComponent cast = (PubgMarkingItemComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgMarkingItemComponent cast = (PubgMarkingItemComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgMarkingItemComponent def = (PubgMarkingItemComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgMarkingItemComponent Instantiate()
	{
		return new PubgMarkingItemComponent();
	}
}
