using Content.Client.Actions;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.Actions.Components;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Xenonids.Rest;

public sealed class XenoRestKeybindSystem : EntitySystem
{
	[Dependency]
	private ActionsSystem _actionsSystem;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		CommandBinds.Builder.Bind(CMKeyFunctions.RMCXenoRest, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
			if (localEntity.HasValue)
			{
				EntityUid value = localEntity.Value;
				ActionComponent actionComponent = default(ActionComponent);
				if (((EntityUid)(ref value)).IsValid() && ((EntitySystem)this).HasComp<XenoComponent>(localEntity.Value))
				{
					foreach (Entity<ActionComponent> item2 in _rmcActions.GetActionsWithEvent<XenoRestActionEvent>(localEntity.Value))
					{
						item2.Deconstruct(ref value, ref actionComponent);
						EntityUid item = value;
						ActionComponent actionComponent2 = actionComponent;
						if (actionComponent2 != null && actionComponent2.Enabled)
						{
							EntityUid? attachedEntity = actionComponent2.AttachedEntity;
							value = localEntity.Value;
							if (attachedEntity.HasValue && !(attachedEntity.GetValueOrDefault() != value) && (!actionComponent2.Cooldown.HasValue || !(actionComponent2.Cooldown.Value.End > _timing.CurTime)))
							{
								_actionsSystem.TriggerAction(Entity<ActionComponent>.op_Implicit((item, actionComponent2)));
								break;
							}
						}
					}
				}
			}
		}, (StateInputCmdDelegate)null, true, true)).Register<XenoRestKeybindSystem>();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<XenoRestKeybindSystem>();
	}
}
