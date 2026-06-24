using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Gulag.Components;

[RegisterComponent]
public sealed class GulagFighterComponent : Component, ISerializationGenerated<GulagFighterComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid ArenaEntity;

	[DataField(null, false, 1, false, false, null)]
	public bool IsAlive = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GulagFighterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (GulagFighterComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GulagFighterComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid ArenaEntityTemp = default(EntityUid);
			if (!serialization.TryCustomCopy<EntityUid>(ArenaEntity, ref ArenaEntityTemp, hookCtx, false, context))
			{
				ArenaEntityTemp = serialization.CreateCopy<EntityUid>(ArenaEntity, hookCtx, context, false);
			}
			target.ArenaEntity = ArenaEntityTemp;
			bool IsAliveTemp = false;
			if (!serialization.TryCustomCopy<bool>(IsAlive, ref IsAliveTemp, hookCtx, false, context))
			{
				IsAliveTemp = IsAlive;
			}
			target.IsAlive = IsAliveTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GulagFighterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagFighterComponent cast = (GulagFighterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagFighterComponent cast = (GulagFighterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GulagFighterComponent def = (GulagFighterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GulagFighterComponent Instantiate()
	{
		return new GulagFighterComponent();
	}
}
