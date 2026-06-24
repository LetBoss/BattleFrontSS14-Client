using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.AlertLevel;

[RegisterComponent]
public sealed class AlertLevelDisplayComponent : Component, ISerializationGenerated<AlertLevelDisplayComponent>, ISerializationGenerated
{
	[DataField("alertVisuals", false, 1, false, false, null)]
	public Dictionary<string, string> AlertVisuals = new Dictionary<string, string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AlertLevelDisplayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AlertLevelDisplayComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AlertLevelDisplayComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, string> AlertVisualsTemp = null;
			if (AlertVisuals == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, string>>(AlertVisuals, ref AlertVisualsTemp, hookCtx, true, context))
			{
				AlertVisualsTemp = serialization.CreateCopy<Dictionary<string, string>>(AlertVisuals, hookCtx, context, false);
			}
			target.AlertVisuals = AlertVisualsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AlertLevelDisplayComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AlertLevelDisplayComponent cast = (AlertLevelDisplayComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AlertLevelDisplayComponent cast = (AlertLevelDisplayComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AlertLevelDisplayComponent def = (AlertLevelDisplayComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AlertLevelDisplayComponent Instantiate()
	{
		return new AlertLevelDisplayComponent();
	}
}
