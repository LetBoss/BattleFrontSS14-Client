using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Damage;

[DataDefinition]
public sealed class DamageVisualizerSprite : ISerializationGenerated<DamageVisualizerSprite>, ISerializationGenerated
{
	[DataField("sprite", false, 1, true, false, null)]
	public string Sprite;

	[DataField("color", false, 1, false, false, null)]
	public string? Color;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DamageVisualizerSprite target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<DamageVisualizerSprite>(this, ref target, hookCtx, false, context))
		{
			string sprite = null;
			if (Sprite == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Sprite, ref sprite, hookCtx, false, context))
			{
				sprite = Sprite;
			}
			target.Sprite = sprite;
			string color = null;
			if (!serialization.TryCustomCopy<string>(Color, ref color, hookCtx, false, context))
			{
				color = Color;
			}
			target.Color = color;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DamageVisualizerSprite target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DamageVisualizerSprite target2 = (DamageVisualizerSprite)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public DamageVisualizerSprite Instantiate()
	{
		return new DamageVisualizerSprite();
	}
}
