using System;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Zombies;

[RegisterComponent]
[NetworkedComponent]
public sealed class InitialInfectedComponent : Component, ISerializationGenerated<InitialInfectedComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<FactionIconPrototype> StatusIcon = ProtoId<FactionIconPrototype>.op_Implicit("InitialInfectedFaction");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InitialInfectedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (InitialInfectedComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<InitialInfectedComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<FactionIconPrototype> StatusIconTemp = default(ProtoId<FactionIconPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<FactionIconPrototype>>(StatusIcon, ref StatusIconTemp, hookCtx, false, context))
			{
				StatusIconTemp = serialization.CreateCopy<ProtoId<FactionIconPrototype>>(StatusIcon, hookCtx, context, false);
			}
			target.StatusIcon = StatusIconTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InitialInfectedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InitialInfectedComponent cast = (InitialInfectedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InitialInfectedComponent cast = (InitialInfectedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InitialInfectedComponent def = (InitialInfectedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InitialInfectedComponent Instantiate()
	{
		return new InitialInfectedComponent();
	}
}
