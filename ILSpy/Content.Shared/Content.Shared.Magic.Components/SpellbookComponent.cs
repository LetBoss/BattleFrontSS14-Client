using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Magic.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SpellbookSystem) })]
public sealed class SpellbookComponent : Component, ISerializationGenerated<SpellbookComponent>, ISerializationGenerated
{
	[ViewVariables]
	public readonly List<EntityUid> Spells = new List<EntityUid>();

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<EntProtoId, int?> SpellActions = new Dictionary<EntProtoId, int?>();

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float LearnTime = 0.75f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool LearnPermanently;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpellbookComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SpellbookComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SpellbookComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<EntProtoId, int?> SpellActionsTemp = null;
			if (SpellActions == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<EntProtoId, int?>>(SpellActions, ref SpellActionsTemp, hookCtx, true, context))
			{
				SpellActionsTemp = serialization.CreateCopy<Dictionary<EntProtoId, int?>>(SpellActions, hookCtx, context, false);
			}
			target.SpellActions = SpellActionsTemp;
			float LearnTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LearnTime, ref LearnTimeTemp, hookCtx, false, context))
			{
				LearnTimeTemp = LearnTime;
			}
			target.LearnTime = LearnTimeTemp;
			bool LearnPermanentlyTemp = false;
			if (!serialization.TryCustomCopy<bool>(LearnPermanently, ref LearnPermanentlyTemp, hookCtx, false, context))
			{
				LearnPermanentlyTemp = LearnPermanently;
			}
			target.LearnPermanently = LearnPermanentlyTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpellbookComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpellbookComponent cast = (SpellbookComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpellbookComponent cast = (SpellbookComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpellbookComponent def = (SpellbookComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpellbookComponent Instantiate()
	{
		return new SpellbookComponent();
	}
}
