using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.PDA;

[RegisterComponent]
public sealed class PdaBorderColorComponent : Component, ISerializationGenerated<PdaBorderColorComponent>, ISerializationGenerated
{
	[DataField("borderColor", false, 1, true, false, null)]
	public string? BorderColor;

	[DataField("accentHColor", false, 1, false, false, null)]
	public string? AccentHColor;

	[DataField("accentVColor", false, 1, false, false, null)]
	public string? AccentVColor;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PdaBorderColorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (PdaBorderColorComponent)(object)val;
		if (!serialization.TryCustomCopy<PdaBorderColorComponent>(this, ref target, hookCtx, false, context))
		{
			string borderColor = null;
			if (!serialization.TryCustomCopy<string>(BorderColor, ref borderColor, hookCtx, false, context))
			{
				borderColor = BorderColor;
			}
			target.BorderColor = borderColor;
			string accentHColor = null;
			if (!serialization.TryCustomCopy<string>(AccentHColor, ref accentHColor, hookCtx, false, context))
			{
				accentHColor = AccentHColor;
			}
			target.AccentHColor = accentHColor;
			string accentVColor = null;
			if (!serialization.TryCustomCopy<string>(AccentVColor, ref accentVColor, hookCtx, false, context))
			{
				accentVColor = AccentVColor;
			}
			target.AccentVColor = accentVColor;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PdaBorderColorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PdaBorderColorComponent target2 = (PdaBorderColorComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PdaBorderColorComponent target2 = (PdaBorderColorComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PdaBorderColorComponent target2 = (PdaBorderColorComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PdaBorderColorComponent Instantiate()
	{
		return new PdaBorderColorComponent();
	}
}
