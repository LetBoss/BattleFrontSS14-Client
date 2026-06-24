using System;
using System.Collections.Generic;
using Content.Shared.Interaction;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Configurable;

public abstract class SharedConfigurationSystem : EntitySystem
{
	[Dependency]
	private SharedUserInterfaceSystem _uiSystem;

	[Dependency]
	private SharedToolSystem _toolSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ConfigurationComponent, ConfigurationComponent.ConfigurationUpdatedMessage>((ComponentEventHandler<ConfigurationComponent, ConfigurationComponent.ConfigurationUpdatedMessage>)OnUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ConfigurationComponent, InteractUsingEvent>((ComponentEventHandler<ConfigurationComponent, InteractUsingEvent>)OnInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ConfigurationComponent, ContainerIsInsertingAttemptEvent>((ComponentEventHandler<ConfigurationComponent, ContainerIsInsertingAttemptEvent>)OnInsert, (Type[])null, (Type[])null);
	}

	private void OnInteractUsing(EntityUid uid, ConfigurationComponent component, InteractUsingEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _toolSystem.HasQuality(args.Used, ProtoId<ToolQualityPrototype>.op_Implicit(component.QualityNeeded)))
		{
			((HandledEntityEventArgs)args).Handled = _uiSystem.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum)ConfigurationComponent.ConfigurationUiKey.Key, args.User, false);
		}
	}

	private void OnUpdate(EntityUid uid, ConfigurationComponent component, ConfigurationComponent.ConfigurationUpdatedMessage args)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		foreach (string key in component.Config.Keys)
		{
			string value = args.Config.GetValueOrDefault(key);
			if (!string.IsNullOrWhiteSpace(value) && (component.Validation == null || component.Validation.IsMatch(value)))
			{
				component.Config[key] = value;
			}
		}
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		ConfigurationUpdatedEvent updatedEvent = new ConfigurationUpdatedEvent(component);
		((EntitySystem)this).RaiseLocalEvent<ConfigurationUpdatedEvent>(uid, updatedEvent, false);
	}

	private void OnInsert(EntityUid uid, ConfigurationComponent component, ContainerIsInsertingAttemptEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (_toolSystem.HasQuality(((ContainerAttemptEventBase)args).EntityUid, ProtoId<ToolQualityPrototype>.op_Implicit(component.QualityNeeded)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
