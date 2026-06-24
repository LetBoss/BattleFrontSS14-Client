using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class BorgModuleIconComponent : Component, ISerializationGenerated<BorgModuleIconComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Rsi Icon;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BorgModuleIconComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BorgModuleIconComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<BorgModuleIconComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		Rsi IconTemp = null;
		if (Icon == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Rsi>(Icon, ref IconTemp, hookCtx, false, context))
		{
			if (Icon == null)
			{
				IconTemp = null;
			}
			else
			{
				serialization.CopyTo<Rsi>(Icon, ref IconTemp, hookCtx, context, true);
			}
		}
		target.Icon = IconTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BorgModuleIconComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BorgModuleIconComponent cast = (BorgModuleIconComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BorgModuleIconComponent cast = (BorgModuleIconComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BorgModuleIconComponent def = (BorgModuleIconComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BorgModuleIconComponent Instantiate()
	{
		return new BorgModuleIconComponent();
	}
}
