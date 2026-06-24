using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Atmos.Components;

[RegisterComponent]
public sealed class PipeAppearanceComponent : Component, ISerializationGenerated<PipeAppearanceComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Rsi[] Sprite = (Rsi[])(object)new Rsi[3]
	{
		new Rsi(new ResPath("Structures/Piping/Atmospherics/pipe.rsi"), "pipeConnector"),
		new Rsi(new ResPath("Structures/Piping/Atmospherics/pipe_alt1.rsi"), "pipeConnector"),
		new Rsi(new ResPath("Structures/Piping/Atmospherics/pipe_alt2.rsi"), "pipeConnector")
	};

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PipeAppearanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PipeAppearanceComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PipeAppearanceComponent>(this, ref target, hookCtx, false, context))
		{
			Rsi[] SpriteTemp = null;
			if (Sprite == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Rsi[]>(Sprite, ref SpriteTemp, hookCtx, true, context))
			{
				SpriteTemp = serialization.CreateCopy<Rsi[]>(Sprite, hookCtx, context, false);
			}
			target.Sprite = SpriteTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PipeAppearanceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PipeAppearanceComponent cast = (PipeAppearanceComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PipeAppearanceComponent cast = (PipeAppearanceComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PipeAppearanceComponent def = (PipeAppearanceComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PipeAppearanceComponent Instantiate()
	{
		return new PipeAppearanceComponent();
	}
}
