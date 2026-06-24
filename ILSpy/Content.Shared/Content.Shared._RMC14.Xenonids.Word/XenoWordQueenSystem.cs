using System;
using System.Text.RegularExpressions;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.CCVar;
using Content.Shared.Popups;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Xenonids.Word;

public sealed class XenoWordQueenSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedCMChatSystem _cmChat;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private SharedXenoAnnounceSystem _xenoAnnounce;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	private readonly Regex _newLineRegex = new Regex("\n{3,}", RegexOptions.Compiled);

	private int _characterLimit = 1000;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoWordQueenComponent, XenoWordQueenActionEvent>((EntityEventRefHandler<XenoWordQueenComponent, XenoWordQueenActionEvent>)OnXenoWordQueenAction, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<XenoWordQueenComponent>(((EntitySystem)this).Subs, (object)XenoWordQueenUI.Key, (BuiEventSubscriber<XenoWordQueenComponent>)delegate(Subscriber<XenoWordQueenComponent> subs)
		{
			subs.Event<XenoWordQueenBuiMsg>((EntityEventRefHandler<XenoWordQueenComponent, XenoWordQueenBuiMsg>)OnXenoWordQueenBui);
		});
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, CCVars.ChatMaxMessageLength, (Action<int>)delegate(int limit)
		{
			_characterLimit = limit;
		}, true);
	}

	private void OnXenoWordQueenAction(Entity<XenoWordQueenComponent> queen, ref XenoWordQueenActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(queen.Owner), (Enum)XenoWordQueenUI.Key, Entity<XenoWordQueenComponent>.op_Implicit(queen), false);
		}
	}

	private void OnXenoWordQueenBui(Entity<XenoWordQueenComponent> queen, ref XenoWordQueenBuiMsg args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(queen.Owner), (Enum)XenoWordQueenUI.Key, (EntityUid?)Entity<XenoWordQueenComponent>.op_Implicit(queen), false);
		string text = args.Text.Trim();
		if (string.IsNullOrWhiteSpace(text) || !_xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(queen.Owner), queen.Comp.PlasmaCost))
		{
			return;
		}
		Entity<HiveComponent>? hive = _hive.GetHive(Entity<HiveMemberComponent>.op_Implicit(queen.Owner));
		if (hive.HasValue)
		{
			Entity<HiveComponent> hive2 = hive.GetValueOrDefault();
			if (_net.IsClient)
			{
				return;
			}
			if (text.Length > _characterLimit)
			{
				text = text.Substring(0, _characterLimit).Trim();
			}
			Filter xenos = Filter.Empty().AddWhereAttachedEntity((Predicate<EntityUid>)((EntityUid ent) => _hive.IsMember(Entity<HiveMemberComponent>.op_Implicit(ent), Entity<HiveComponent>.op_Implicit(hive2))));
			if (xenos.Count > 1)
			{
				_xenoPlasma.TryRemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit(queen.Owner), queen.Comp.PlasmaCost);
				text = _newLineRegex.Replace(text, "\n\n");
				text = _cmChat.SanitizeMessageReplaceWords(Entity<XenoWordQueenComponent>.op_Implicit(queen), text);
				string headerText = base.Loc.GetString("rmc-xeno-words-of-the-queen-header");
				string wrapped = FormattedMessage.EscapeText(text);
				string message = (_xenoAnnounce.WrapHive(headerText) ?? "") + "[color=red][font size=14][bold]" + wrapped + "[/bold][/font][/color]";
				_xenoAnnounce.Announce(Entity<XenoWordQueenComponent>.op_Implicit(queen), xenos, text, message, queen.Comp.Sound);
				{
					EntityUid val = default(EntityUid);
					ActionComponent actionComponent = default(ActionComponent);
					foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoWordQueenComponent>.op_Implicit(queen)))
					{
						action.Deconstruct(ref val, ref actionComponent);
						EntityUid actionId = val;
						if (((EntitySystem)this).HasComp<XenoWordQueenActionComponent>(actionId))
						{
							_actions.StartUseDelay(Entity<ActionComponent>.op_Implicit(actionId));
						}
					}
					return;
				}
			}
			_popup.PopupEntity(base.Loc.GetString("cm-xeno-words-of-the-queen-nobody-hear-you"), Entity<XenoWordQueenComponent>.op_Implicit(queen), Entity<XenoWordQueenComponent>.op_Implicit(queen), PopupType.LargeCaution);
		}
		else
		{
			_popup.PopupClient(base.Loc.GetString("cm-xeno-words-of-the-queen-nobody-hear-you"), Entity<XenoWordQueenComponent>.op_Implicit(queen), Entity<XenoWordQueenComponent>.op_Implicit(queen), PopupType.LargeCaution);
		}
	}
}
