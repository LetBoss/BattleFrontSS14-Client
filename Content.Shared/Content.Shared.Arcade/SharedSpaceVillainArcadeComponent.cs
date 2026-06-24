using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Arcade;

public abstract class SharedSpaceVillainArcadeComponent : Component, ISerializationGenerated<SharedSpaceVillainArcadeComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	public enum Indicators
	{
		HealthManager,
		HealthLimiter
	}

	[Serializable]
	[NetSerializable]
	public enum PlayerAction
	{
		Attack,
		Heal,
		Recharge,
		NewGame,
		RequestData
	}

	[Serializable]
	[NetSerializable]
	public enum SpaceVillainArcadeVisualState
	{
		Normal,
		Off,
		Broken,
		Win,
		GameOver
	}

	[Serializable]
	[NetSerializable]
	public enum SpaceVillainArcadeUiKey
	{
		Key
	}

	[Serializable]
	[NetSerializable]
	public sealed class SpaceVillainArcadePlayerActionMessage : BoundUserInterfaceMessage
	{
		public readonly PlayerAction PlayerAction;

		public SpaceVillainArcadePlayerActionMessage(PlayerAction playerAction)
		{
			PlayerAction = playerAction;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class SpaceVillainArcadeMetaDataUpdateMessage : SpaceVillainArcadeDataUpdateMessage
	{
		public readonly string GameTitle;

		public readonly string EnemyName;

		public readonly bool ButtonsDisabled;

		public SpaceVillainArcadeMetaDataUpdateMessage(int playerHp, int playerMp, int enemyHp, int enemyMp, string playerActionMessage, string enemyActionMessage, string gameTitle, string enemyName, bool buttonsDisabled)
			: base(playerHp, playerMp, enemyHp, enemyMp, playerActionMessage, enemyActionMessage)
		{
			GameTitle = gameTitle;
			EnemyName = enemyName;
			ButtonsDisabled = buttonsDisabled;
		}
	}

	[Serializable]
	[NetSerializable]
	[Virtual]
	public class SpaceVillainArcadeDataUpdateMessage : BoundUserInterfaceMessage
	{
		public readonly int PlayerHP;

		public readonly int PlayerMP;

		public readonly int EnemyHP;

		public readonly int EnemyMP;

		public readonly string PlayerActionMessage;

		public readonly string EnemyActionMessage;

		public SpaceVillainArcadeDataUpdateMessage(int playerHp, int playerMp, int enemyHp, int enemyMp, string playerActionMessage, string enemyActionMessage)
		{
			PlayerHP = playerHp;
			PlayerMP = playerMp;
			EnemyHP = enemyHp;
			EnemyMP = enemyMp;
			EnemyActionMessage = enemyActionMessage;
			PlayerActionMessage = playerActionMessage;
		}
	}

	public SharedSpaceVillainArcadeComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedSpaceVillainArcadeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedSpaceVillainArcadeComponent)(object)definitionCast;
		serialization.TryCustomCopy<SharedSpaceVillainArcadeComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedSpaceVillainArcadeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedSpaceVillainArcadeComponent cast = (SharedSpaceVillainArcadeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedSpaceVillainArcadeComponent cast = (SharedSpaceVillainArcadeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedSpaceVillainArcadeComponent def = (SharedSpaceVillainArcadeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedSpaceVillainArcadeComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
