using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Xenoarchaeology.XenoArtifacts;

[RegisterComponent]
public sealed class RandomArtifactSpriteComponent : Component, ISerializationGenerated<RandomArtifactSpriteComponent>, ISerializationGenerated
{
	[DataField("minSprite", false, 1, false, false, null)]
	public int MinSprite = 1;

	[DataField("maxSprite", false, 1, false, false, null)]
	public int MaxSprite = 14;

	[DataField("activationTime", false, 1, false, false, null)]
	public double ActivationTime = 0.4;

	public TimeSpan? ActivationStart;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RandomArtifactSpriteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RandomArtifactSpriteComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RandomArtifactSpriteComponent>(this, ref target, hookCtx, false, context))
		{
			int MinSpriteTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinSprite, ref MinSpriteTemp, hookCtx, false, context))
			{
				MinSpriteTemp = MinSprite;
			}
			target.MinSprite = MinSpriteTemp;
			int MaxSpriteTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxSprite, ref MaxSpriteTemp, hookCtx, false, context))
			{
				MaxSpriteTemp = MaxSprite;
			}
			target.MaxSprite = MaxSpriteTemp;
			double ActivationTimeTemp = 0.0;
			if (!serialization.TryCustomCopy<double>(ActivationTime, ref ActivationTimeTemp, hookCtx, false, context))
			{
				ActivationTimeTemp = ActivationTime;
			}
			target.ActivationTime = ActivationTimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RandomArtifactSpriteComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomArtifactSpriteComponent cast = (RandomArtifactSpriteComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomArtifactSpriteComponent cast = (RandomArtifactSpriteComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RandomArtifactSpriteComponent def = (RandomArtifactSpriteComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RandomArtifactSpriteComponent Instantiate()
	{
		return new RandomArtifactSpriteComponent();
	}
}
