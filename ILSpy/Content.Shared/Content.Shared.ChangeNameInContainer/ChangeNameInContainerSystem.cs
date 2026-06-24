using System;
using Content.Shared.Chat;
using Content.Shared.Speech;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.ChangeNameInContainer;

public sealed class ChangeNameInContainerSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ChangeVoiceInContainerComponent, TransformSpeakerNameEvent>((EntityEventRefHandler<ChangeVoiceInContainerComponent, TransformSpeakerNameEvent>)OnTransformSpeakerName, (Type[])null, (Type[])null);
	}

	private void OnTransformSpeakerName(Entity<ChangeVoiceInContainerComponent> ent, ref TransformSpeakerNameEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(Entity<ChangeVoiceInContainerComponent>.op_Implicit(ent), null, null)), ref container) && !_whitelist.IsWhitelistFail(ent.Comp.Whitelist, container.Owner))
		{
			args.VoiceName = ((EntitySystem)this).Name(container.Owner, (MetaDataComponent)null);
			SpeechComponent speechComp = default(SpeechComponent);
			if (((EntitySystem)this).TryComp<SpeechComponent>(container.Owner, ref speechComp))
			{
				args.SpeechVerb = speechComp.SpeechVerb;
			}
		}
	}
}
