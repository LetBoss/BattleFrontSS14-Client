using System;
using Content.Shared.ParcelWrap.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.ParcelWrap.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ParcelWrappingSystem) })]
public sealed class WrappedParcelComponent : Component, ISerializationGenerated<WrappedParcelComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ContainerSlot Contents;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? UnwrapTrash;

	[DataField(null, false, 1, true, false, null)]
	public TimeSpan UnwrapDelay = TimeSpan.FromSeconds(1L);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? UnwrapSound;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ContainerId = "contents";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WrappedParcelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (WrappedParcelComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<WrappedParcelComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId? UnwrapTrashTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(UnwrapTrash, ref UnwrapTrashTemp, hookCtx, false, context))
			{
				UnwrapTrashTemp = serialization.CreateCopy<EntProtoId?>(UnwrapTrash, hookCtx, context, false);
			}
			target.UnwrapTrash = UnwrapTrashTemp;
			TimeSpan UnwrapDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(UnwrapDelay, ref UnwrapDelayTemp, hookCtx, false, context))
			{
				UnwrapDelayTemp = serialization.CreateCopy<TimeSpan>(UnwrapDelay, hookCtx, context, false);
			}
			target.UnwrapDelay = UnwrapDelayTemp;
			SoundSpecifier UnwrapSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(UnwrapSound, ref UnwrapSoundTemp, hookCtx, true, context))
			{
				UnwrapSoundTemp = serialization.CreateCopy<SoundSpecifier>(UnwrapSound, hookCtx, context, false);
			}
			target.UnwrapSound = UnwrapSoundTemp;
			string ContainerIdTemp = null;
			if (ContainerId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ContainerId, ref ContainerIdTemp, hookCtx, false, context))
			{
				ContainerIdTemp = ContainerId;
			}
			target.ContainerId = ContainerIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WrappedParcelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WrappedParcelComponent cast = (WrappedParcelComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WrappedParcelComponent cast = (WrappedParcelComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WrappedParcelComponent def = (WrappedParcelComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WrappedParcelComponent Instantiate()
	{
		return new WrappedParcelComponent();
	}
}
