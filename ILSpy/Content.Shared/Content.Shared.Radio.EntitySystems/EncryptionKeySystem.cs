using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Chat;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Radio.Components;
using Content.Shared.Tools.Components;
using Content.Shared.Tools.Systems;
using Content.Shared.Wires;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Radio.EntitySystems;

public sealed class EncryptionKeySystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public sealed class EncryptionRemovalFinishedEvent : SimpleDoAfterEvent, ISerializationGenerated<EncryptionRemovalFinishedEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref EncryptionRemovalFinishedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (EncryptionRemovalFinishedEvent)definitionCast;
			serialization.TryCustomCopy<EncryptionRemovalFinishedEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref EncryptionRemovalFinishedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			EncryptionRemovalFinishedEvent cast = (EncryptionRemovalFinishedEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			EncryptionRemovalFinishedEvent cast = (EncryptionRemovalFinishedEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override EncryptionRemovalFinishedEvent Instantiate()
		{
			return new EncryptionRemovalFinishedEvent();
		}
	}

	[Dependency]
	private IPrototypeManager _protoManager;

	[Dependency]
	private SharedToolSystem _tool;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedWiresSystem _wires;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EncryptionKeyComponent, ExaminedEvent>((ComponentEventHandler<EncryptionKeyComponent, ExaminedEvent>)OnKeyExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EncryptionKeyHolderComponent, ExaminedEvent>((ComponentEventHandler<EncryptionKeyHolderComponent, ExaminedEvent>)OnHolderExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EncryptionKeyHolderComponent, ComponentStartup>((ComponentEventHandler<EncryptionKeyHolderComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EncryptionKeyHolderComponent, InteractUsingEvent>((ComponentEventHandler<EncryptionKeyHolderComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EncryptionKeyHolderComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<EncryptionKeyHolderComponent, EntInsertedIntoContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EncryptionKeyHolderComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<EncryptionKeyHolderComponent, EntRemovedFromContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EncryptionKeyHolderComponent, EncryptionRemovalFinishedEvent>((ComponentEventHandler<EncryptionKeyHolderComponent, EncryptionRemovalFinishedEvent>)OnKeyRemoval, (Type[])null, (Type[])null);
	}

	private void OnKeyRemoval(EntityUid uid, EncryptionKeyHolderComponent component, EncryptionRemovalFinishedEvent args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			EntityUid[] array = ((BaseContainer)component.KeyContainer).ContainedEntities.ToArray();
			_container.EmptyContainer((BaseContainer)(object)component.KeyContainer, false, (EntityCoordinates?)null, false);
			EntityUid[] array2 = array;
			foreach (EntityUid ent in array2)
			{
				_hands.PickupOrDrop(args.User, ent, checkActionBlocker: true, animateUser: false, animate: true, dropNear: true);
			}
			_popup.PopupPredicted(base.Loc.GetString("encryption-keys-all-extracted"), uid, args.User);
			_audio.PlayPredicted(component.KeyExtractionSound, uid, (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	public void UpdateChannels(EntityUid uid, EncryptionKeyHolderComponent component)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!((Component)component).Initialized)
		{
			return;
		}
		component.Channels.Clear();
		component.DefaultChannel = null;
		component.ReadOnlyChannels.Clear();
		EncryptionKeyComponent key = default(EncryptionKeyComponent);
		foreach (EntityUid ent in ((BaseContainer)component.KeyContainer).ContainedEntities)
		{
			if (((EntitySystem)this).TryComp<EncryptionKeyComponent>(ent, ref key))
			{
				component.Channels.UnionWith(key.Channels);
				component.ReadOnlyChannels.UnionWith(key.ReadOnlyChannels);
				if (component.DefaultChannel == null)
				{
					component.DefaultChannel = key.DefaultChannel;
				}
			}
		}
		((EntitySystem)this).RaiseLocalEvent<EncryptionChannelsChangedEvent>(uid, new EncryptionChannelsChangedEvent(component), false);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnContainerModified(EntityUid uid, EncryptionKeyHolderComponent component, ContainerModifiedMessage args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (args.Container.ID == "key_slots")
		{
			UpdateChannels(uid, component);
		}
	}

	private void OnInteractUsing(EntityUid uid, EncryptionKeyHolderComponent component, InteractUsingEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			ToolComponent tool = default(ToolComponent);
			if (((EntitySystem)this).HasComp<EncryptionKeyComponent>(args.Used))
			{
				((HandledEntityEventArgs)args).Handled = true;
				TryInsertKey(uid, component, args);
			}
			else if (((EntitySystem)this).TryComp<ToolComponent>(args.Used, ref tool) && _tool.HasQuality(args.Used, component.KeysExtractionMethod, tool) && ((BaseContainer)component.KeyContainer).ContainedEntities.Count > 0)
			{
				((HandledEntityEventArgs)args).Handled = true;
				TryRemoveKey(uid, component, args, tool);
			}
		}
	}

	private void TryInsertKey(EntityUid uid, EncryptionKeyHolderComponent component, InteractUsingEvent args)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		WiresPanelComponent panel = default(WiresPanelComponent);
		if (!component.KeysUnlocked)
		{
			_popup.PopupClient(base.Loc.GetString("encryption-keys-are-locked"), uid, args.User);
		}
		else if (((EntitySystem)this).TryComp<WiresPanelComponent>(uid, ref panel) && !panel.Open)
		{
			_popup.PopupClient(base.Loc.GetString("encryption-keys-panel-locked"), uid, args.User);
		}
		else if (component.KeySlots <= ((BaseContainer)component.KeyContainer).ContainedEntities.Count)
		{
			_popup.PopupClient(base.Loc.GetString("encryption-key-slots-already-full"), uid, args.User);
		}
		else if (_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), (BaseContainer)(object)component.KeyContainer, (TransformComponent)null, false))
		{
			_popup.PopupClient(base.Loc.GetString("encryption-key-successfully-installed"), uid, args.User);
			_audio.PlayPredicted(component.KeyInsertionSound, args.Target, (EntityUid?)args.User, (AudioParams?)null);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void TryRemoveKey(EntityUid uid, EncryptionKeyHolderComponent component, InteractUsingEvent args, ToolComponent? tool)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!component.KeysUnlocked)
		{
			_popup.PopupClient(base.Loc.GetString("encryption-keys-are-locked"), uid, args.User);
		}
		else if (!_wires.IsPanelOpen(Entity<WiresPanelComponent>.op_Implicit(uid)))
		{
			_popup.PopupClient(base.Loc.GetString("encryption-keys-panel-locked"), uid, args.User);
		}
		else if (((BaseContainer)component.KeyContainer).ContainedEntities.Count == 0)
		{
			_popup.PopupClient(base.Loc.GetString("encryption-keys-no-keys"), uid, args.User);
		}
		else
		{
			_tool.UseTool(args.Used, args.User, uid, 1f, component.KeysExtractionMethod, new EncryptionRemovalFinishedEvent(), 0f, tool);
		}
	}

	private void OnStartup(EntityUid uid, EncryptionKeyHolderComponent component, ComponentStartup args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		component.KeyContainer = _container.EnsureContainer<Container>(uid, "key_slots", (ContainerManagerComponent)null);
		UpdateChannels(uid, component);
	}

	private void OnHolderExamined(EntityUid uid, EncryptionKeyHolderComponent component, ExaminedEvent args)
	{
		if (!args.IsInDetailsRange)
		{
			return;
		}
		if (((BaseContainer)component.KeyContainer).ContainedEntities.Count == 0 && component.Channels.Count == 0)
		{
			args.PushMarkup(base.Loc.GetString("encryption-keys-no-keys"));
		}
		else if (component.Channels.Count > 0)
		{
			using (args.PushGroup("EncryptionKeyComponent"))
			{
				args.PushMarkup(base.Loc.GetString("examine-encryption-channels-prefix"));
				AddChannelsExamine(component.Channels, component.DefaultChannel, args, _protoManager, "examine-encryption-channel", component.ReadOnlyChannels);
			}
		}
	}

	private void OnKeyExamined(EntityUid uid, EncryptionKeyComponent component, ExaminedEvent args)
	{
		if (args.IsInDetailsRange && component.Channels.Count > 0)
		{
			args.PushMarkup(base.Loc.GetString("examine-encryption-channels-prefix"));
			AddChannelsExamine(component.Channels, component.DefaultChannel, args, _protoManager, "examine-encryption-channel", component.ReadOnlyChannels);
		}
	}

	public void AddChannelsExamine(HashSet<string> channels, string? defaultChannel, ExaminedEvent examineEvent, IPrototypeManager protoManager, string channelFTLPattern, HashSet<ProtoId<RadioChannelPrototype>>? ReadonlyChannels)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		RadioChannelPrototype proto = default(RadioChannelPrototype);
		foreach (string id in channels)
		{
			proto = _protoManager.Index<RadioChannelPrototype>(id);
			string key = ((ProtoId<RadioChannelPrototype>.op_Implicit(id) == SharedChatSystem.CommonChannel) ? ';'.ToString() : $"{':'}{proto.KeyCode}");
			string readOnlyMarkup = "";
			if (ReadonlyChannels != null && ReadonlyChannels.Contains(ProtoId<RadioChannelPrototype>.op_Implicit(id)))
			{
				readOnlyMarkup = " Read Only";
			}
			examineEvent.PushMarkup(base.Loc.GetString(channelFTLPattern, new(string, object)[4]
			{
				("color", proto.Color),
				("key", key),
				("id", proto.LocalizedName),
				("freq", (float)proto.Frequency / 10f)
			}) + readOnlyMarkup);
		}
		if (defaultChannel != null && _protoManager.TryIndex<RadioChannelPrototype>(defaultChannel, ref proto))
		{
			if (((EntitySystem)this).HasComp<HeadsetComponent>(examineEvent.Examined))
			{
				string msg = base.Loc.GetString("examine-headset-default-channel", new(string, object)[3]
				{
					("prefix", SharedChatSystem.DefaultChannelPrefix),
					("channel", proto.LocalizedName),
					("color", proto.Color)
				});
				examineEvent.PushMarkup(msg);
			}
			if (((EntitySystem)this).HasComp<EncryptionKeyComponent>(examineEvent.Examined))
			{
				string msg2 = base.Loc.GetString("examine-encryption-default-channel", (ValueTuple<string, object>)("channel", proto.LocalizedName), (ValueTuple<string, object>)("color", proto.Color));
				examineEvent.PushMarkup(msg2);
			}
		}
	}
}
