using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Fluids.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedPuddleSystem) })]
public sealed class PuddleComponent : Component, ISerializationGenerated<PuddleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier SpillSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 OverflowVolume = FixedPoint2.New(10000);

	[DataField("solution", false, 1, false, false, null)]
	public string SolutionName = "puddle";

	[DataField(null, false, 1, false, false, null)]
	public float DefaultSlippery = 5.5f;

	[ViewVariables]
	public Entity<SolutionComponent>? Solution;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PuddleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PuddleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PuddleComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier SpillSoundTemp = null;
			if (SpillSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(SpillSound, ref SpillSoundTemp, hookCtx, true, context))
			{
				SpillSoundTemp = serialization.CreateCopy<SoundSpecifier>(SpillSound, hookCtx, context, false);
			}
			target.SpillSound = SpillSoundTemp;
			FixedPoint2 OverflowVolumeTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(OverflowVolume, ref OverflowVolumeTemp, hookCtx, false, context))
			{
				OverflowVolumeTemp = serialization.CreateCopy<FixedPoint2>(OverflowVolume, hookCtx, context, false);
			}
			target.OverflowVolume = OverflowVolumeTemp;
			string SolutionNameTemp = null;
			if (SolutionName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SolutionName, ref SolutionNameTemp, hookCtx, false, context))
			{
				SolutionNameTemp = SolutionName;
			}
			target.SolutionName = SolutionNameTemp;
			float DefaultSlipperyTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DefaultSlippery, ref DefaultSlipperyTemp, hookCtx, false, context))
			{
				DefaultSlipperyTemp = DefaultSlippery;
			}
			target.DefaultSlippery = DefaultSlipperyTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PuddleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PuddleComponent cast = (PuddleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PuddleComponent cast = (PuddleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PuddleComponent def = (PuddleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PuddleComponent Instantiate()
	{
		return new PuddleComponent();
	}
}
