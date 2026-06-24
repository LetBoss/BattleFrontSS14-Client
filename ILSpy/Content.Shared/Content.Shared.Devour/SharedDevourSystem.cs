using System;
using Content.Shared.Actions;
using Content.Shared.Devour.Components;
using Content.Shared.DoAfter;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Devour;

public abstract class SharedDevourSystem : EntitySystem
{
	[Dependency]
	protected SharedAudioSystem _audioSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfterSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	protected SharedContainerSystem ContainerSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DevourerComponent, MapInitEvent>((ComponentEventHandler<DevourerComponent, MapInitEvent>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DevourerComponent, DevourActionEvent>((ComponentEventHandler<DevourerComponent, DevourActionEvent>)OnDevourAction, (Type[])null, (Type[])null);
	}

	protected void OnInit(EntityUid uid, DevourerComponent component, MapInitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		component.Stomach = ContainerSystem.EnsureContainer<Container>(uid, "stomach", (ContainerManagerComponent)null);
		_actionsSystem.AddAction(uid, ref component.DevourActionEntity, component.DevourAction);
	}

	protected void OnDevourAction(EntityUid uid, DevourerComponent component, DevourActionEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || _whitelistSystem.IsWhitelistFailOrNull(component.Whitelist, args.Target))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid target = args.Target;
		MobStateComponent targetState = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(target, ref targetState))
		{
			MobState currentState = targetState.CurrentState;
			if (currentState - 2 <= MobState.Alive)
			{
				_doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, uid, component.DevourTime, new DevourDoAfterEvent(), uid, target, uid)
				{
					BreakOnMove = true
				});
			}
			else
			{
				_popupSystem.PopupClient(base.Loc.GetString("devour-action-popup-message-fail-target-alive"), uid, uid);
			}
			return;
		}
		_popupSystem.PopupClient(base.Loc.GetString("devour-action-popup-message-structure"), uid, uid);
		if (component.SoundStructureDevour != null)
		{
			_audioSystem.PlayPredicted(component.SoundStructureDevour, uid, (EntityUid?)uid, (AudioParams?)component.SoundStructureDevour.Params);
		}
		_doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager)(object)base.EntityManager, uid, component.StructureDevourTime, new DevourDoAfterEvent(), uid, target, uid)
		{
			BreakOnMove = true
		});
	}
}
