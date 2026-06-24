using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Gibbing.Components;

[RegisterComponent]
public sealed class GibOnRoundEndComponent : Component, ISerializationGenerated<GibOnRoundEndComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public HashSet<EntProtoId> PreventGibbingObjectives = new HashSet<EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? SpawnProto;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GibOnRoundEndComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GibOnRoundEndComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GibOnRoundEndComponent>(this, ref target, hookCtx, false, context))
		{
			HashSet<EntProtoId> PreventGibbingObjectivesTemp = null;
			if (PreventGibbingObjectives == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntProtoId>>(PreventGibbingObjectives, ref PreventGibbingObjectivesTemp, hookCtx, true, context))
			{
				PreventGibbingObjectivesTemp = serialization.CreateCopy<HashSet<EntProtoId>>(PreventGibbingObjectives, hookCtx, context, false);
			}
			target.PreventGibbingObjectives = PreventGibbingObjectivesTemp;
			EntProtoId? SpawnProtoTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(SpawnProto, ref SpawnProtoTemp, hookCtx, false, context))
			{
				SpawnProtoTemp = serialization.CreateCopy<EntProtoId?>(SpawnProto, hookCtx, context, false);
			}
			target.SpawnProto = SpawnProtoTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GibOnRoundEndComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GibOnRoundEndComponent cast = (GibOnRoundEndComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GibOnRoundEndComponent cast = (GibOnRoundEndComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GibOnRoundEndComponent def = (GibOnRoundEndComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GibOnRoundEndComponent Instantiate()
	{
		return new GibOnRoundEndComponent();
	}
}
