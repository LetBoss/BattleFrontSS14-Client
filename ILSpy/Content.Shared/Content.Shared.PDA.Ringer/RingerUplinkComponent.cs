using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.PDA.Ringer;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedRingerSystem) })]
public sealed class RingerUplinkComponent : Component, ISerializationGenerated<RingerUplinkComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Note[]? Code;

	[DataField(null, false, 1, false, false, null)]
	public bool Unlocked;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RingerUplinkComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RingerUplinkComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RingerUplinkComponent>(this, ref target, hookCtx, false, context))
		{
			Note[] CodeTemp = null;
			if (!serialization.TryCustomCopy<Note[]>(Code, ref CodeTemp, hookCtx, true, context))
			{
				CodeTemp = serialization.CreateCopy<Note[]>(Code, hookCtx, context, false);
			}
			target.Code = CodeTemp;
			bool UnlockedTemp = false;
			if (!serialization.TryCustomCopy<bool>(Unlocked, ref UnlockedTemp, hookCtx, false, context))
			{
				UnlockedTemp = Unlocked;
			}
			target.Unlocked = UnlockedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RingerUplinkComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RingerUplinkComponent cast = (RingerUplinkComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RingerUplinkComponent cast = (RingerUplinkComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RingerUplinkComponent def = (RingerUplinkComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RingerUplinkComponent Instantiate()
	{
		return new RingerUplinkComponent();
	}
}
