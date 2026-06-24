using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Sticky.Components;

[RegisterComponent]
public sealed class StickyVisualizerComponent : Component, ISerializationGenerated<StickyVisualizerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int StuckDrawDepth = 10;

	[DataField(null, false, 1, false, false, null)]
	public int OriginalDrawDepth;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StickyVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StickyVisualizerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StickyVisualizerComponent>(this, ref target, hookCtx, false, context))
		{
			int StuckDrawDepthTemp = 0;
			if (!serialization.TryCustomCopy<int>(StuckDrawDepth, ref StuckDrawDepthTemp, hookCtx, false, context))
			{
				StuckDrawDepthTemp = StuckDrawDepth;
			}
			target.StuckDrawDepth = StuckDrawDepthTemp;
			int OriginalDrawDepthTemp = 0;
			if (!serialization.TryCustomCopy<int>(OriginalDrawDepth, ref OriginalDrawDepthTemp, hookCtx, false, context))
			{
				OriginalDrawDepthTemp = OriginalDrawDepth;
			}
			target.OriginalDrawDepth = OriginalDrawDepthTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StickyVisualizerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StickyVisualizerComponent cast = (StickyVisualizerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StickyVisualizerComponent cast = (StickyVisualizerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StickyVisualizerComponent def = (StickyVisualizerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StickyVisualizerComponent Instantiate()
	{
		return new StickyVisualizerComponent();
	}
}
