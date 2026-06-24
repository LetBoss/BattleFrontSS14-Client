using System;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Weapons.Ranged;

[Serializable]
[NetSerializable]
public sealed class RMCBattleExecuteEvent : SimpleDoAfterEvent, ISerializationGenerated<RMCBattleExecuteEvent>, ISerializationGenerated
{
	public new NetEntity User;

	public new NetEntity Target;

	public DamageSpecifier BattleExecuteDamage;

	public RMCBattleExecuteEvent(NetEntity user, NetEntity target, DamageSpecifier battleExecuteDamage)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Target = target;
		BattleExecuteDamage = battleExecuteDamage;
	}

	public RMCBattleExecuteEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCBattleExecuteEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCBattleExecuteEvent)definitionCast;
		serialization.TryCustomCopy<RMCBattleExecuteEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCBattleExecuteEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCBattleExecuteEvent cast = (RMCBattleExecuteEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCBattleExecuteEvent cast = (RMCBattleExecuteEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCBattleExecuteEvent Instantiate()
	{
		return new RMCBattleExecuteEvent();
	}
}
