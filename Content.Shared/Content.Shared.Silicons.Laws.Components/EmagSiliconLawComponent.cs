using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.Laws.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedSiliconLawSystem) })]
public sealed class EmagSiliconLawComponent : Component, ISerializationGenerated<EmagSiliconLawComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? OwnerName;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool RequireOpenPanel = true;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan StunTime = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier EmaggedSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Ambience/Antag/emagged_borg.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EmagSiliconLawComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EmagSiliconLawComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<EmagSiliconLawComponent>(this, ref target, hookCtx, false, context))
		{
			string OwnerNameTemp = null;
			if (!serialization.TryCustomCopy<string>(OwnerName, ref OwnerNameTemp, hookCtx, false, context))
			{
				OwnerNameTemp = OwnerName;
			}
			target.OwnerName = OwnerNameTemp;
			bool RequireOpenPanelTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequireOpenPanel, ref RequireOpenPanelTemp, hookCtx, false, context))
			{
				RequireOpenPanelTemp = RequireOpenPanel;
			}
			target.RequireOpenPanel = RequireOpenPanelTemp;
			TimeSpan StunTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(StunTime, ref StunTimeTemp, hookCtx, false, context))
			{
				StunTimeTemp = serialization.CreateCopy<TimeSpan>(StunTime, hookCtx, context, false);
			}
			target.StunTime = StunTimeTemp;
			SoundSpecifier EmaggedSoundTemp = null;
			if (EmaggedSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(EmaggedSound, ref EmaggedSoundTemp, hookCtx, true, context))
			{
				EmaggedSoundTemp = serialization.CreateCopy<SoundSpecifier>(EmaggedSound, hookCtx, context, false);
			}
			target.EmaggedSound = EmaggedSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EmagSiliconLawComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmagSiliconLawComponent cast = (EmagSiliconLawComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmagSiliconLawComponent cast = (EmagSiliconLawComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmagSiliconLawComponent def = (EmagSiliconLawComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EmagSiliconLawComponent Instantiate()
	{
		return new EmagSiliconLawComponent();
	}
}
