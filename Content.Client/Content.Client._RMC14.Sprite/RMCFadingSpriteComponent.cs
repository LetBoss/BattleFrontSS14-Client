using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Sprite;

[RegisterComponent]
[Access(new Type[] { typeof(RMCSpriteFadeSystem) })]
public sealed class RMCFadingSpriteComponent : Component, ISerializationGenerated<RMCFadingSpriteComponent>, ISerializationGenerated
{
	[ViewVariables]
	public float OriginalAlpha;

	[ViewVariables]
	public Dictionary<string, float> OriginalLayerAlphas = new Dictionary<string, float>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCFadingSpriteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (RMCFadingSpriteComponent)(object)val;
		serialization.TryCustomCopy<RMCFadingSpriteComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCFadingSpriteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCFadingSpriteComponent target2 = (RMCFadingSpriteComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCFadingSpriteComponent target2 = (RMCFadingSpriteComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCFadingSpriteComponent target2 = (RMCFadingSpriteComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCFadingSpriteComponent Instantiate()
	{
		return new RMCFadingSpriteComponent();
	}
}
