using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Botany.Components;

[RegisterComponent]
public sealed class PotencyVisualsComponent : Component, ISerializationGenerated<PotencyVisualsComponent>, ISerializationGenerated
{
	[DataField("minimumScale", false, 1, false, false, null)]
	public float MinimumScale = 1f;

	[DataField("maximumScale", false, 1, false, false, null)]
	public float MaximumScale = 2f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PotencyVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (PotencyVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<PotencyVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			float minimumScale = 0f;
			if (!serialization.TryCustomCopy<float>(MinimumScale, ref minimumScale, hookCtx, false, context))
			{
				minimumScale = MinimumScale;
			}
			target.MinimumScale = minimumScale;
			float maximumScale = 0f;
			if (!serialization.TryCustomCopy<float>(MaximumScale, ref maximumScale, hookCtx, false, context))
			{
				maximumScale = MaximumScale;
			}
			target.MaximumScale = maximumScale;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PotencyVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PotencyVisualsComponent target2 = (PotencyVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PotencyVisualsComponent target2 = (PotencyVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PotencyVisualsComponent target2 = (PotencyVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PotencyVisualsComponent Instantiate()
	{
		return new PotencyVisualsComponent();
	}
}
