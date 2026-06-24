using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Marines.Mutiny;

[RegisterComponent]
[NetworkedComponent]
public sealed class MutineerComponent : Component, ISerializationGenerated<MutineerComponent>, ISerializationGenerated
{
	[DataField("icon", false, 1, false, false, null)]
	public SpriteSpecifier Icon = (SpriteSpecifier)new Rsi(new ResPath("_RMC14/Interface/cm_job_icons.rsi"), "hudmutineer");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MutineerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MutineerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MutineerComponent>(this, ref target, hookCtx, false, context))
		{
			SpriteSpecifier IconTemp = null;
			if (Icon == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SpriteSpecifier>(Icon, ref IconTemp, hookCtx, true, context))
			{
				IconTemp = serialization.CreateCopy<SpriteSpecifier>(Icon, hookCtx, context, false);
			}
			target.Icon = IconTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MutineerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MutineerComponent cast = (MutineerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MutineerComponent cast = (MutineerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MutineerComponent def = (MutineerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MutineerComponent Instantiate()
	{
		return new MutineerComponent();
	}
}
