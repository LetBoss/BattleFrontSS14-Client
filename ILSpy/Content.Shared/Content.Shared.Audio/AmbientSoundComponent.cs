using System;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Audio;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedAmbientSoundSystem) })]
public sealed class AmbientSoundComponent : Component, IComponentTreeEntry<AmbientSoundComponent>, ISerializationGenerated<AmbientSoundComponent>, ISerializationGenerated
{
	[DataField("sound", false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SoundSpecifier Sound;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("range", false, 1, false, false, null)]
	public float Range = 2f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("volume", false, 1, false, false, null)]
	public float Volume = -10f;

	[DataField("enabled", true, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Enabled { get; set; } = true;

	public Vector2 RangeVector => new Vector2(Range, Range);

	public EntityUid? TreeUid { get; set; }

	public DynamicTree<ComponentTreeEntry<AmbientSoundComponent>>? Tree { get; set; }

	public bool AddToTree => Enabled;

	public bool TreeUpdateQueued { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AmbientSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AmbientSoundComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AmbientSoundComponent>(this, ref target, hookCtx, false, context))
		{
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			SoundSpecifier SoundTemp = null;
			if (Sound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
			float RangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
			float VolumeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Volume, ref VolumeTemp, hookCtx, false, context))
			{
				VolumeTemp = Volume;
			}
			target.Volume = VolumeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AmbientSoundComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmbientSoundComponent cast = (AmbientSoundComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmbientSoundComponent cast = (AmbientSoundComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AmbientSoundComponent def = (AmbientSoundComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AmbientSoundComponent Instantiate()
	{
		return new AmbientSoundComponent();
	}
}
