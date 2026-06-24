using System;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Labels.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Shared.Labels.EntitySystems;

public abstract class SharedHandLabelerSystem : EntitySystem
{
	[Dependency]
	protected SharedUserInterfaceSystem UserInterfaceSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private LabelSystem _labelSystem;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HandLabelerComponent, AfterInteractEvent>((ComponentEventHandler<HandLabelerComponent, AfterInteractEvent>)AfterInteractOn, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandLabelerComponent, GetVerbsEvent<UtilityVerb>>((ComponentEventHandler<HandLabelerComponent, GetVerbsEvent<UtilityVerb>>)OnUtilityVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandLabelerComponent, HandLabelerLabelChangedMessage>((ComponentEventHandler<HandLabelerComponent, HandLabelerLabelChangedMessage>)OnHandLabelerLabelChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandLabelerComponent, ComponentGetState>((EntityEventRefHandler<HandLabelerComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HandLabelerComponent, ComponentHandleState>((EntityEventRefHandler<HandLabelerComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnGetState(Entity<HandLabelerComponent> ent, ref ComponentGetState args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new HandLabelerComponentState(ent.Comp.AssignedLabel)
		{
			MaxLabelChars = ent.Comp.MaxLabelChars
		};
	}

	private void OnHandleState(Entity<HandLabelerComponent> ent, ref ComponentHandleState args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is HandLabelerComponentState state)
		{
			ent.Comp.MaxLabelChars = state.MaxLabelChars;
			if (!(ent.Comp.AssignedLabel == state.AssignedLabel))
			{
				ent.Comp.AssignedLabel = state.AssignedLabel;
				UpdateUI(ent);
			}
		}
	}

	protected virtual void UpdateUI(Entity<HandLabelerComponent> ent)
	{
	}

	private void AddLabelTo(EntityUid uid, HandLabelerComponent? handLabeler, EntityUid target, out string? result)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HandLabelerComponent>(uid, ref handLabeler, true))
		{
			result = null;
		}
		else if (handLabeler.AssignedLabel == string.Empty)
		{
			if (_netManager.IsServer)
			{
				_labelSystem.Label(target, null);
			}
			result = base.Loc.GetString("hand-labeler-successfully-removed");
		}
		else
		{
			if (_netManager.IsServer)
			{
				_labelSystem.Label(target, handLabeler.AssignedLabel);
			}
			result = base.Loc.GetString("hand-labeler-successfully-applied");
		}
	}

	private void OnUtilityVerb(EntityUid uid, HandLabelerComponent handLabeler, GetVerbsEvent<UtilityVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityUid target = args.Target;
		if (((EntityUid)(ref target)).Valid && !_whitelistSystem.IsWhitelistFail(handLabeler.Whitelist, target) && args.CanAccess)
		{
			string labelerText = ((handLabeler.AssignedLabel == string.Empty) ? base.Loc.GetString("hand-labeler-remove-label-text") : base.Loc.GetString("hand-labeler-add-label-text"));
			UtilityVerb verb = new UtilityVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					Labeling(uid, target, args.User, handLabeler);
				},
				Text = labelerText
			};
			args.Verbs.Add(verb);
		}
	}

	private void AfterInteractOn(EntityUid uid, HandLabelerComponent handLabeler, AfterInteractEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (((EntityUid)(ref target2)).Valid && !_whitelistSystem.IsWhitelistFail(handLabeler.Whitelist, target2) && args.CanReach)
			{
				Labeling(uid, target2, args.User, handLabeler);
			}
		}
	}

	private void Labeling(EntityUid uid, EntityUid target, EntityUid User, HandLabelerComponent handLabeler)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		AddLabelTo(uid, handLabeler, target, out string result);
		if (result != null)
		{
			_popupSystem.PopupClient(result, User, User);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(15, 3);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(User)), "user", "ToPrettyString(User)");
			handler.AppendLiteral(" labeled ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "target", "ToPrettyString(target)");
			handler.AppendLiteral(" with ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "labeler", "ToPrettyString(uid)");
			adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
		}
	}

	private void OnHandLabelerLabelChanged(EntityUid uid, HandLabelerComponent handLabeler, HandLabelerLabelChangedMessage args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		string label = args.Label.Trim();
		handLabeler.AssignedLabel = label.Substring(0, Math.Min(handLabeler.MaxLabelChars, label.Length));
		UpdateUI(Entity<HandLabelerComponent>.op_Implicit((uid, handLabeler)));
		((EntitySystem)this).Dirty(uid, (IComponent)(object)handLabeler, (MetaDataComponent)null);
		ISharedAdminLogManager adminLogger = _adminLogger;
		LogStringHandler handler = new LogStringHandler(23, 3);
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(((BaseBoundUserInterfaceEvent)args).Actor)), "user", "ToPrettyString(args.Actor)");
		handler.AppendLiteral(" set ");
		handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "labeler", "ToPrettyString(uid)");
		handler.AppendLiteral(" to apply label \"");
		handler.AppendFormatted(handLabeler.AssignedLabel);
		handler.AppendLiteral("\"");
		adminLogger.Add(LogType.Action, LogImpact.Low, ref handler);
	}
}
