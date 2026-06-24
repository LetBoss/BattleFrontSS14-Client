using System;
using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityTable.Conditions;

public sealed class PlayerCountCondition : EntityTableCondition, ISerializationGenerated<PlayerCountCondition>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Min = int.MinValue;

	[DataField(null, false, 1, false, false, null)]
	public int Max = int.MaxValue;

	private static ISharedPlayerManager? _playerManager;

	protected override bool EvaluateImplementation(EntityTableSelector root, IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
	{
		if (_playerManager == null)
		{
			_playerManager = IoCManager.Resolve<ISharedPlayerManager>();
		}
		int playerCount = _playerManager.PlayerCount;
		if (playerCount >= Min)
		{
			return playerCount <= Max;
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlayerCountCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTableCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlayerCountCondition)definitionCast;
		if (!serialization.TryCustomCopy<PlayerCountCondition>(this, ref target, hookCtx, false, context))
		{
			int MinTemp = 0;
			if (!serialization.TryCustomCopy<int>(Min, ref MinTemp, hookCtx, false, context))
			{
				MinTemp = Min;
			}
			target.Min = MinTemp;
			int MaxTemp = 0;
			if (!serialization.TryCustomCopy<int>(Max, ref MaxTemp, hookCtx, false, context))
			{
				MaxTemp = Max;
			}
			target.Max = MaxTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlayerCountCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTableCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlayerCountCondition cast = (PlayerCountCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlayerCountCondition cast = (PlayerCountCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlayerCountCondition Instantiate()
	{
		return new PlayerCountCondition();
	}
}
