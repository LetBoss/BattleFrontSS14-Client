using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.SubFloor;

[NetworkedComponent]
[RegisterComponent]
[Access(new Type[] { typeof(SharedSubFloorHideSystem) })]
public sealed class SubFloorHideComponent : Component, ISerializationGenerated<SubFloorHideComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public HashSet<Enum> VisibleLayers = new HashSet<Enum>();

	[DataField(null, false, 1, false, false, null)]
	public int? OriginalDrawDepth;

	[ViewVariables]
	public bool IsUnderCover { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public bool BlockInteractions { get; set; } = true;

	[DataField(null, false, 1, false, false, null)]
	public bool BlockAmbience { get; set; } = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SubFloorHideComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SubFloorHideComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SubFloorHideComponent>(this, ref target, hookCtx, false, context))
		{
			bool BlockInteractionsTemp = false;
			if (!serialization.TryCustomCopy<bool>(BlockInteractions, ref BlockInteractionsTemp, hookCtx, false, context))
			{
				BlockInteractionsTemp = BlockInteractions;
			}
			target.BlockInteractions = BlockInteractionsTemp;
			bool BlockAmbienceTemp = false;
			if (!serialization.TryCustomCopy<bool>(BlockAmbience, ref BlockAmbienceTemp, hookCtx, false, context))
			{
				BlockAmbienceTemp = BlockAmbience;
			}
			target.BlockAmbience = BlockAmbienceTemp;
			HashSet<Enum> VisibleLayersTemp = null;
			if (VisibleLayers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<Enum>>(VisibleLayers, ref VisibleLayersTemp, hookCtx, true, context))
			{
				VisibleLayersTemp = serialization.CreateCopy<HashSet<Enum>>(VisibleLayers, hookCtx, context, false);
			}
			target.VisibleLayers = VisibleLayersTemp;
			int? OriginalDrawDepthTemp = null;
			if (!serialization.TryCustomCopy<int?>(OriginalDrawDepth, ref OriginalDrawDepthTemp, hookCtx, false, context))
			{
				OriginalDrawDepthTemp = OriginalDrawDepth;
			}
			target.OriginalDrawDepth = OriginalDrawDepthTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SubFloorHideComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SubFloorHideComponent cast = (SubFloorHideComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SubFloorHideComponent cast = (SubFloorHideComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SubFloorHideComponent def = (SubFloorHideComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SubFloorHideComponent Instantiate()
	{
		return new SubFloorHideComponent();
	}
}
