using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared.Inventory.Events;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;
using Content.Shared.Radio.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Radio;

public sealed class RMCRadioSystem : EntitySystem
{
	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private EncryptionKeySystem _encryptionKey;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	private readonly HashSet<Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent>> _toUpdate = new HashSet<Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCHeadsetComponent, EncryptionChannelsChangedEvent>((EntityEventRefHandler<RMCHeadsetComponent, EncryptionChannelsChangedEvent>)OnHeadsetEncryptionChannelsChanged, new Type[1] { typeof(SharedHeadsetSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCRadioFilterComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCRadioFilterComponent, GetVerbsEvent<AlternativeVerb>>)OnGetAltVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeadsetAutoSquadComponent, MapInitEvent>((EntityEventRefHandler<HeadsetAutoSquadComponent, MapInitEvent>)OnHeadsetAutoSquadRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeadsetAutoSquadComponent, GotEquippedEvent>((EntityEventRefHandler<HeadsetAutoSquadComponent, GotEquippedEvent>)OnHeadsetAutoSquadRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeadsetAutoSquadComponent, EncryptionChannelsChangedEvent>((EntityEventRefHandler<HeadsetAutoSquadComponent, EncryptionChannelsChangedEvent>)OnHeadsetAutoSquadEncryptionChannelsChanged, new Type[1] { typeof(SharedHeadsetSystem) }, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<RMCRadioFilterComponent>(((EntitySystem)this).Subs, (object)RMCRadioFilterUI.Key, (BuiEventSubscriber<RMCRadioFilterComponent>)delegate(Subscriber<RMCRadioFilterComponent> subs)
		{
			subs.Event<RMCRadioFilterBuiMsg>((EntityEventRefHandler<RMCRadioFilterComponent, RMCRadioFilterBuiMsg>)OnRadioFilterBuiMsg);
		});
	}

	private void OnHeadsetEncryptionChannelsChanged(Entity<RMCHeadsetComponent> ent, ref EncryptionChannelsChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_toUpdate.Add(Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent>.op_Implicit((Entity<RMCHeadsetComponent>.op_Implicit(ent), Entity<RMCHeadsetComponent>.op_Implicit(ent), args.Component)));
	}

	private void OnGetAltVerbs(Entity<RMCRadioFilterComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanAccess && args.CanInteract && args.Hands != null)
		{
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = "Tune Radio",
				IconEntity = ((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null),
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					_ui.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)RMCRadioFilterUI.Key, (EntityUid?)user, false);
				}
			});
		}
	}

	private void OnHeadsetAutoSquadRefresh<T>(Entity<HeadsetAutoSquadComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		EncryptionKeyHolderComponent holder = default(EncryptionKeyHolderComponent);
		if (((EntitySystem)this).TryComp<EncryptionKeyHolderComponent>(ent.Owner, ref holder) && holder.KeyContainer != null)
		{
			_encryptionKey.UpdateChannels(Entity<HeadsetAutoSquadComponent>.op_Implicit(ent), holder);
		}
	}

	private void OnHeadsetAutoSquadEncryptionChannelsChanged(Entity<HeadsetAutoSquadComponent> ent, ref EncryptionChannelsChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		SquadMemberComponent member = default(SquadMemberComponent);
		SquadTeamComponent team = default(SquadTeamComponent);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(Entity<HeadsetAutoSquadComponent>.op_Implicit(ent), null)), ref container) && ((EntitySystem)this).TryComp<SquadMemberComponent>(container.Owner, ref member) && ((EntitySystem)this).TryComp<SquadTeamComponent>(member.Squad, ref team))
		{
			ProtoId<RadioChannelPrototype>? radio = team.Radio;
			if (radio.HasValue)
			{
				ProtoId<RadioChannelPrototype> radio2 = radio.GetValueOrDefault();
				args.Component.Channels.Add(ProtoId<RadioChannelPrototype>.op_Implicit(radio2));
			}
		}
	}

	private void OnRadioFilterBuiMsg(Entity<RMCRadioFilterComponent> ent, ref RMCRadioFilterBuiMsg args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (args.Toggle)
		{
			ent.Comp.DisabledChannels.Remove(args.Channel);
		}
		else
		{
			ent.Comp.DisabledChannels.Add(args.Channel);
		}
		((EntitySystem)this).Dirty<RMCRadioFilterComponent>(ent, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent> ent in _toUpdate)
			{
				if (((EntitySystem)this).TerminatingOrDeleted(Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent>.op_Implicit(ent), (MetaDataComponent)null) || !((Component)ent.Comp1).Running || !((Component)ent.Comp2).Running)
				{
					continue;
				}
				foreach (ProtoId<RadioChannelPrototype> channel in ent.Comp1.Channels)
				{
					ent.Comp2.Channels.Add(ProtoId<RadioChannelPrototype>.op_Implicit(channel));
					((EntitySystem)this).Dirty(Entity<RMCHeadsetComponent, EncryptionKeyHolderComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp2, (MetaDataComponent)null);
				}
			}
		}
		finally
		{
			_toUpdate.Clear();
		}
	}
}
