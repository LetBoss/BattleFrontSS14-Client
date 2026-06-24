using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Magic.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedMagicSystem) })]
public sealed class MagicComponent : Component, ISerializationGenerated<MagicComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool RequiresClothes;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool RequiresSpeech;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MagicComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MagicComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MagicComponent>(this, ref target, hookCtx, false, context))
		{
			bool RequiresClothesTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequiresClothes, ref RequiresClothesTemp, hookCtx, false, context))
			{
				RequiresClothesTemp = RequiresClothes;
			}
			target.RequiresClothes = RequiresClothesTemp;
			bool RequiresSpeechTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequiresSpeech, ref RequiresSpeechTemp, hookCtx, false, context))
			{
				RequiresSpeechTemp = RequiresSpeech;
			}
			target.RequiresSpeech = RequiresSpeechTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MagicComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicComponent cast = (MagicComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicComponent cast = (MagicComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicComponent def = (MagicComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MagicComponent Instantiate()
	{
		return new MagicComponent();
	}
}
