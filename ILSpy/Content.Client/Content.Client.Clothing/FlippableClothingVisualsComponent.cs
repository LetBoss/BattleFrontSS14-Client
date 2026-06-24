using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Clothing;

[RegisterComponent]
[Access(new Type[] { typeof(FlippableClothingVisualizerSystem) })]
public sealed class FlippableClothingVisualsComponent : Component, ISerializationGenerated<FlippableClothingVisualsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string FoldingLayer = "foldedLayer";

	[DataField(null, false, 1, false, false, null)]
	public string UnfoldingLayer = "unfoldedLayer";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FlippableClothingVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (FlippableClothingVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<FlippableClothingVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string foldingLayer = null;
			if (FoldingLayer == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FoldingLayer, ref foldingLayer, hookCtx, false, context))
			{
				foldingLayer = FoldingLayer;
			}
			target.FoldingLayer = foldingLayer;
			string unfoldingLayer = null;
			if (UnfoldingLayer == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(UnfoldingLayer, ref unfoldingLayer, hookCtx, false, context))
			{
				unfoldingLayer = UnfoldingLayer;
			}
			target.UnfoldingLayer = unfoldingLayer;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FlippableClothingVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlippableClothingVisualsComponent target2 = (FlippableClothingVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlippableClothingVisualsComponent target2 = (FlippableClothingVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FlippableClothingVisualsComponent target2 = (FlippableClothingVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FlippableClothingVisualsComponent Instantiate()
	{
		return new FlippableClothingVisualsComponent();
	}
}
