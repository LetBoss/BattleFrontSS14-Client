using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

public abstract class SharedBwoinkSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public sealed class BwoinkTextMessage : EntityEventArgs
	{
		public readonly bool AdminOnly;

		public DateTime SentAt { get; }

		public NetUserId UserId { get; }

		public NetUserId TrueSender { get; }

		public string Text { get; }

		public bool PlaySound { get; }

		public BwoinkTextMessage(NetUserId userId, NetUserId trueSender, string text, DateTime? sentAt = null, bool playSound = true, bool adminOnly = false)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			SentAt = sentAt ?? DateTime.Now;
			UserId = userId;
			TrueSender = trueSender;
			Text = text;
			PlaySound = playSound;
			AdminOnly = adminOnly;
		}
	}

	public static NetUserId SystemUserId { get; } = new NetUserId(Guid.Empty);

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<BwoinkTextMessage>((EntitySessionEventHandler<BwoinkTextMessage>)OnBwoinkTextMessage, (Type[])null, (Type[])null);
	}

	protected virtual void OnBwoinkTextMessage(BwoinkTextMessage message, EntitySessionEventArgs eventArgs)
	{
	}

	protected void LogBwoink(BwoinkTextMessage message)
	{
	}
}
