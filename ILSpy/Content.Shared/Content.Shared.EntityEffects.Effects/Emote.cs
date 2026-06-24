using System;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.EntityEffects.Effects;

public sealed class Emote : EventEntityEffect<Emote>, ISerializationGenerated<Emote>, ISerializationGenerated
{
	[DataField("emote", false, 1, true, false, typeof(PrototypeIdSerializer<EmotePrototype>))]
	public string EmoteId;

	[DataField(null, false, 1, false, false, null)]
	public bool ShowInChat;

	[DataField(null, false, 1, false, false, null)]
	public bool ShowInGuidebook;

	[DataField(null, false, 1, false, false, null)]
	public bool Force;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		if (!ShowInGuidebook)
		{
			return null;
		}
		EmotePrototype emotePrototype = prototype.Index<EmotePrototype>(EmoteId);
		return Loc.GetString("reagent-effect-guidebook-emote", new(string, object)[2]
		{
			("chance", Probability),
			("emote", Loc.GetString(emotePrototype.Name))
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Emote target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EventEntityEffect<Emote> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Emote)definitionCast;
		if (!serialization.TryCustomCopy<Emote>(this, ref target, hookCtx, false, context))
		{
			string EmoteIdTemp = null;
			if (EmoteId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(EmoteId, ref EmoteIdTemp, hookCtx, false, context))
			{
				EmoteIdTemp = EmoteId;
			}
			target.EmoteId = EmoteIdTemp;
			bool ShowInChatTemp = false;
			if (!serialization.TryCustomCopy<bool>(ShowInChat, ref ShowInChatTemp, hookCtx, false, context))
			{
				ShowInChatTemp = ShowInChat;
			}
			target.ShowInChat = ShowInChatTemp;
			bool ShowInGuidebookTemp = false;
			if (!serialization.TryCustomCopy<bool>(ShowInGuidebook, ref ShowInGuidebookTemp, hookCtx, false, context))
			{
				ShowInGuidebookTemp = ShowInGuidebook;
			}
			target.ShowInGuidebook = ShowInGuidebookTemp;
			bool ForceTemp = false;
			if (!serialization.TryCustomCopy<bool>(Force, ref ForceTemp, hookCtx, false, context))
			{
				ForceTemp = Force;
			}
			target.Force = ForceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Emote target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<Emote> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Emote cast = (Emote)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Emote cast = (Emote)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Emote Instantiate()
	{
		return new Emote();
	}
}
