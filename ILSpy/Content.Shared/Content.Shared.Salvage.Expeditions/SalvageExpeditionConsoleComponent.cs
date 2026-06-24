using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Salvage.Expeditions;

[RegisterComponent]
[NetworkedComponent]
public sealed class SalvageExpeditionConsoleComponent : Component, ISerializationGenerated<SalvageExpeditionConsoleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier PrintSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Machines/terminal_insert_disc.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SalvageExpeditionConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SalvageExpeditionConsoleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SalvageExpeditionConsoleComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier PrintSoundTemp = null;
			if (PrintSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(PrintSound, ref PrintSoundTemp, hookCtx, true, context))
			{
				PrintSoundTemp = serialization.CreateCopy<SoundSpecifier>(PrintSound, hookCtx, context, false);
			}
			target.PrintSound = PrintSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SalvageExpeditionConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SalvageExpeditionConsoleComponent cast = (SalvageExpeditionConsoleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SalvageExpeditionConsoleComponent cast = (SalvageExpeditionConsoleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SalvageExpeditionConsoleComponent def = (SalvageExpeditionConsoleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SalvageExpeditionConsoleComponent Instantiate()
	{
		return new SalvageExpeditionConsoleComponent();
	}
}
