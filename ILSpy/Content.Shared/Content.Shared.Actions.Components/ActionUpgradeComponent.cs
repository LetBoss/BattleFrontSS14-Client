using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Actions.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ActionUpgradeSystem) })]
[EntityCategory(new string[] { "Actions" })]
public sealed class ActionUpgradeComponent : Component, ISerializationGenerated<ActionUpgradeComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Level = 1;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<int, EntProtoId> EffectedLevels = new Dictionary<int, EntProtoId>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActionUpgradeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActionUpgradeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ActionUpgradeComponent>(this, ref target, hookCtx, false, context))
		{
			int LevelTemp = 0;
			if (!serialization.TryCustomCopy<int>(Level, ref LevelTemp, hookCtx, false, context))
			{
				LevelTemp = Level;
			}
			target.Level = LevelTemp;
			Dictionary<int, EntProtoId> EffectedLevelsTemp = null;
			if (EffectedLevels == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<int, EntProtoId>>(EffectedLevels, ref EffectedLevelsTemp, hookCtx, true, context))
			{
				EffectedLevelsTemp = serialization.CreateCopy<Dictionary<int, EntProtoId>>(EffectedLevels, hookCtx, context, false);
			}
			target.EffectedLevels = EffectedLevelsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActionUpgradeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActionUpgradeComponent cast = (ActionUpgradeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActionUpgradeComponent cast = (ActionUpgradeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActionUpgradeComponent def = (ActionUpgradeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActionUpgradeComponent Instantiate()
	{
		return new ActionUpgradeComponent();
	}
}
