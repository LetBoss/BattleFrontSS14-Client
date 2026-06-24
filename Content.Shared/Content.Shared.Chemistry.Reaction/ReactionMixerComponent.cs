using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Reaction;

[RegisterComponent]
public sealed class ReactionMixerComponent : Component, ISerializationGenerated<ReactionMixerComponent>, ISerializationGenerated
{
	[ViewVariables]
	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<MixingCategoryPrototype>> ReactionTypes;

	[ViewVariables]
	[DataField(null, false, 1, false, false, null)]
	public LocId MixMessage = LocId.op_Implicit("default-mixing-success");

	[ViewVariables]
	[DataField(null, false, 1, false, false, null)]
	public bool MixOnInteract = true;

	[ViewVariables]
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan TimeToMix = TimeSpan.Zero;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ReactionMixerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ReactionMixerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ReactionMixerComponent>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<MixingCategoryPrototype>> ReactionTypesTemp = null;
			if (ReactionTypes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<MixingCategoryPrototype>>>(ReactionTypes, ref ReactionTypesTemp, hookCtx, true, context))
			{
				ReactionTypesTemp = serialization.CreateCopy<List<ProtoId<MixingCategoryPrototype>>>(ReactionTypes, hookCtx, context, false);
			}
			target.ReactionTypes = ReactionTypesTemp;
			LocId MixMessageTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(MixMessage, ref MixMessageTemp, hookCtx, false, context))
			{
				MixMessageTemp = serialization.CreateCopy<LocId>(MixMessage, hookCtx, context, false);
			}
			target.MixMessage = MixMessageTemp;
			bool MixOnInteractTemp = false;
			if (!serialization.TryCustomCopy<bool>(MixOnInteract, ref MixOnInteractTemp, hookCtx, false, context))
			{
				MixOnInteractTemp = MixOnInteract;
			}
			target.MixOnInteract = MixOnInteractTemp;
			TimeSpan TimeToMixTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(TimeToMix, ref TimeToMixTemp, hookCtx, false, context))
			{
				TimeToMixTemp = serialization.CreateCopy<TimeSpan>(TimeToMix, hookCtx, context, false);
			}
			target.TimeToMix = TimeToMixTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ReactionMixerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactionMixerComponent cast = (ReactionMixerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactionMixerComponent cast = (ReactionMixerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReactionMixerComponent def = (ReactionMixerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ReactionMixerComponent Instantiate()
	{
		return new ReactionMixerComponent();
	}
}
