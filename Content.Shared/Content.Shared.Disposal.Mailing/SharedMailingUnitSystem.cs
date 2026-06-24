using System;
using System.Collections.Generic;
using Content.Shared.Configurable;
using Content.Shared.DeviceNetwork;
using Content.Shared.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork.Events;
using Content.Shared.DeviceNetwork.Systems;
using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Unit;
using Content.Shared.Disposal.Unit.Events;
using Content.Shared.Interaction;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Shared.Disposal.Mailing;

public abstract class SharedMailingUnitSystem : EntitySystem
{
	[Dependency]
	private SharedDeviceNetworkSystem _deviceNetworkSystem;

	[Dependency]
	private SharedPowerReceiverSystem _power;

	[Dependency]
	protected SharedUserInterfaceSystem UserInterfaceSystem;

	private const string MailTag = "mail";

	private const string TagConfigurationKey = "tag";

	private const string NetTag = "tag";

	private const string NetSrc = "src";

	private const string NetTarget = "target";

	private const string NetCmdSent = "mail_sent";

	private const string NetCmdRequest = "get_mailer_tag";

	private const string NetCmdResponse = "mailer_tag";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MailingUnitComponent, ComponentInit>((ComponentEventHandler<MailingUnitComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MailingUnitComponent, DeviceNetworkPacketEvent>((ComponentEventHandler<MailingUnitComponent, DeviceNetworkPacketEvent>)OnPacketReceived, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MailingUnitComponent, BeforeDisposalFlushEvent>((ComponentEventHandler<MailingUnitComponent, BeforeDisposalFlushEvent>)OnBeforeFlush, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MailingUnitComponent, ConfigurationUpdatedEvent>((ComponentEventHandler<MailingUnitComponent, ConfigurationUpdatedEvent>)OnConfigurationUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MailingUnitComponent, ActivateInWorldEvent>((ComponentEventHandler<MailingUnitComponent, ActivateInWorldEvent>)HandleActivate, new Type[1] { typeof(SharedDisposalUnitSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MailingUnitComponent, TargetSelectedMessage>((ComponentEventHandler<MailingUnitComponent, TargetSelectedMessage>)OnTargetSelected, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, MailingUnitComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateTargetList(uid, component);
	}

	private void OnPacketReceived(EntityUid uid, MailingUnitComponent component, DeviceNetworkPacketEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Data.TryGetValue("command", out string command) || !_power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(uid)))
		{
			return;
		}
		string text = command;
		if (!(text == "get_mailer_tag"))
		{
			if (text == "mailer_tag" && args.Data.TryGetValue("tag", out string tag))
			{
				component.TargetList.Add(tag);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			}
		}
		else
		{
			SendTagRequestResponse(uid, args, component.Tag);
		}
	}

	private void SendTagRequestResponse(EntityUid uid, DeviceNetworkPacketEvent args, string? tag)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (tag != null)
		{
			NetworkPayload payload = new NetworkPayload
			{
				["command"] = "mailer_tag",
				["tag"] = tag
			};
			_deviceNetworkSystem.QueuePacket(uid, args.Address, payload, args.Frequency);
		}
	}

	private void OnBeforeFlush(EntityUid uid, MailingUnitComponent component, BeforeDisposalFlushEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(component.Target))
		{
			((CancellableEntityEventArgs)args).Cancel();
			return;
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		args.Tags.Add("mail");
		args.Tags.Add(component.Target);
		BroadcastSentMessage(uid, component);
	}

	private void BroadcastSentMessage(EntityUid uid, MailingUnitComponent component, DeviceNetworkComponent? device = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(component.Tag) && !string.IsNullOrEmpty(component.Target) && ((EntitySystem)this).Resolve<DeviceNetworkComponent>(uid, ref device, true))
		{
			NetworkPayload payload = new NetworkPayload
			{
				["command"] = "mail_sent",
				["src"] = component.Tag,
				["target"] = component.Target
			};
			_deviceNetworkSystem.QueuePacket(uid, null, payload, null, null, device);
		}
	}

	private void UpdateTargetList(EntityUid uid, MailingUnitComponent component, DeviceNetworkComponent? device = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<DeviceNetworkComponent>(uid, ref device, false))
		{
			NetworkPayload payload = new NetworkPayload { ["command"] = "get_mailer_tag" };
			component.TargetList.Clear();
			_deviceNetworkSystem.QueuePacket(uid, null, payload, null, null, device);
		}
	}

	private void OnConfigurationUpdated(EntityUid uid, MailingUnitComponent component, ConfigurationUpdatedEvent args)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, string> configuration = args.Configuration.Config;
		if (!configuration.ContainsKey("tag") || configuration["tag"] == string.Empty)
		{
			component.Tag = null;
			return;
		}
		component.Tag = configuration["tag"];
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void HandleActivate(EntityUid uid, MailingUnitComponent component, ActivateInWorldEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		ActorComponent actor = default(ActorComponent);
		if (!((HandledEntityEventArgs)args).Handled && args.Complex && ((EntitySystem)this).TryComp<ActorComponent>(args.User, ref actor))
		{
			((HandledEntityEventArgs)args).Handled = true;
			UpdateTargetList(uid, component);
			UserInterfaceSystem.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)MailingUnitUiKey.Key, actor.PlayerSession, false);
		}
	}

	private void OnTargetSelected(EntityUid uid, MailingUnitComponent component, TargetSelectedMessage args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		component.Target = args.Target;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}
}
