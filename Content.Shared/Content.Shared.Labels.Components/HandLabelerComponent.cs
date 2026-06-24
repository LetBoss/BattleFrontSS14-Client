using System;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Labels.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedHandLabelerSystem) })]
public sealed class HandLabelerComponent : Component, ISerializationGenerated<HandLabelerComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[Access(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, false, false, null)]
	public string AssignedLabel = string.Empty;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, false, false, null)]
	public int MaxLabelChars = 50;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist Whitelist = new EntityWhitelist();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HandLabelerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HandLabelerComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<HandLabelerComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string AssignedLabelTemp = null;
		if (AssignedLabel == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(AssignedLabel, ref AssignedLabelTemp, hookCtx, false, context))
		{
			AssignedLabelTemp = AssignedLabel;
		}
		target.AssignedLabel = AssignedLabelTemp;
		int MaxLabelCharsTemp = 0;
		if (!serialization.TryCustomCopy<int>(MaxLabelChars, ref MaxLabelCharsTemp, hookCtx, false, context))
		{
			MaxLabelCharsTemp = MaxLabelChars;
		}
		target.MaxLabelChars = MaxLabelCharsTemp;
		EntityWhitelist WhitelistTemp = null;
		if (Whitelist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, true);
			}
		}
		target.Whitelist = WhitelistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HandLabelerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandLabelerComponent cast = (HandLabelerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandLabelerComponent cast = (HandLabelerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandLabelerComponent def = (HandLabelerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HandLabelerComponent Instantiate()
	{
		return new HandLabelerComponent();
	}
}
