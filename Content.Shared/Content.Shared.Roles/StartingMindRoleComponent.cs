using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Roles;

[RegisterComponent]
[NetworkedComponent]
public sealed class StartingMindRoleComponent : Component, ISerializationGenerated<StartingMindRoleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId MindRole;

	[DataField(null, false, 1, false, false, null)]
	public bool Silent = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StartingMindRoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StartingMindRoleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StartingMindRoleComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId MindRoleTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(MindRole, ref MindRoleTemp, hookCtx, false, context))
			{
				MindRoleTemp = serialization.CreateCopy<EntProtoId>(MindRole, hookCtx, context, false);
			}
			target.MindRole = MindRoleTemp;
			bool SilentTemp = false;
			if (!serialization.TryCustomCopy<bool>(Silent, ref SilentTemp, hookCtx, false, context))
			{
				SilentTemp = Silent;
			}
			target.Silent = SilentTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StartingMindRoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StartingMindRoleComponent cast = (StartingMindRoleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StartingMindRoleComponent cast = (StartingMindRoleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StartingMindRoleComponent def = (StartingMindRoleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StartingMindRoleComponent Instantiate()
	{
		return new StartingMindRoleComponent();
	}
}
