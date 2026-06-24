using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Prayer;

[RegisterComponent]
[NetworkedComponent]
public sealed class PrayableComponent : Component, ISerializationGenerated<PrayableComponent>, ISerializationGenerated
{
	[DataField("bibleUserOnly", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool BibleUserOnly;

	[DataField("sentMessage", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string SentMessage = "prayer-popup-notify-pray-sent";

	[DataField("notificationPrefix", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string NotificationPrefix = "prayer-chat-notify-pray";

	[DataField("verb", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string Verb = "prayer-verbs-pray";

	[DataField("verbImage", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public SpriteSpecifier? VerbImage = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/pray.svg.png"));

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PrayableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PrayableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PrayableComponent>(this, ref target, hookCtx, false, context))
		{
			bool BibleUserOnlyTemp = false;
			if (!serialization.TryCustomCopy<bool>(BibleUserOnly, ref BibleUserOnlyTemp, hookCtx, false, context))
			{
				BibleUserOnlyTemp = BibleUserOnly;
			}
			target.BibleUserOnly = BibleUserOnlyTemp;
			string SentMessageTemp = null;
			if (SentMessage == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SentMessage, ref SentMessageTemp, hookCtx, false, context))
			{
				SentMessageTemp = SentMessage;
			}
			target.SentMessage = SentMessageTemp;
			string NotificationPrefixTemp = null;
			if (NotificationPrefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(NotificationPrefix, ref NotificationPrefixTemp, hookCtx, false, context))
			{
				NotificationPrefixTemp = NotificationPrefix;
			}
			target.NotificationPrefix = NotificationPrefixTemp;
			string VerbTemp = null;
			if (Verb == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Verb, ref VerbTemp, hookCtx, false, context))
			{
				VerbTemp = Verb;
			}
			target.Verb = VerbTemp;
			SpriteSpecifier VerbImageTemp = null;
			if (!serialization.TryCustomCopy<SpriteSpecifier>(VerbImage, ref VerbImageTemp, hookCtx, true, context))
			{
				VerbImageTemp = serialization.CreateCopy<SpriteSpecifier>(VerbImage, hookCtx, context, false);
			}
			target.VerbImage = VerbImageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PrayableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrayableComponent cast = (PrayableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrayableComponent cast = (PrayableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrayableComponent def = (PrayableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PrayableComponent Instantiate()
	{
		return new PrayableComponent();
	}
}
