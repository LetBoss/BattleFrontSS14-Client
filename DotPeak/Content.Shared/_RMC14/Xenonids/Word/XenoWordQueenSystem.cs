// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Word.XenoWordQueenSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Text.RegularExpressions;

#nullable enable
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
    this.SubscribeLocalEvent<XenoWordQueenComponent, XenoWordQueenActionEvent>(new EntityEventRefHandler<XenoWordQueenComponent, XenoWordQueenActionEvent>(this.OnXenoWordQueenAction));
    this.Subs.BuiEvents<XenoWordQueenComponent>((object) XenoWordQueenUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoWordQueenComponent>) (subs => subs.Event<XenoWordQueenBuiMsg>(new EntityEventRefHandler<XenoWordQueenComponent, XenoWordQueenBuiMsg>(this.OnXenoWordQueenBui))));
    this.Subs.CVar<int>(this._config, CCVars.ChatMaxMessageLength, (Action<int>) (limit => this._characterLimit = limit), true);
  }

  private void OnXenoWordQueenAction(
    Entity<XenoWordQueenComponent> queen,
    ref XenoWordQueenActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) queen.Owner, (Enum) XenoWordQueenUI.Key, (EntityUid) queen);
  }

  private void OnXenoWordQueenBui(
    Entity<XenoWordQueenComponent> queen,
    ref XenoWordQueenBuiMsg args)
  {
    this._ui.CloseUi((Entity<UserInterfaceComponent>) queen.Owner, (Enum) XenoWordQueenUI.Key, new EntityUid?((EntityUid) queen));
    string input = args.Text.Trim();
    if (string.IsNullOrWhiteSpace(input) || !this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) queen.Owner, queen.Comp.PlasmaCost))
      return;
    Entity<HiveComponent>? hive1 = this._hive.GetHive((Entity<HiveMemberComponent>) queen.Owner);
    if (hive1.HasValue)
    {
      Entity<HiveComponent> hive = hive1.GetValueOrDefault();
      if (this._net.IsClient)
        return;
      if (input.Length > this._characterLimit)
        input = input.Substring(0, this._characterLimit).Trim();
      Filter filter = Filter.Empty().AddWhereAttachedEntity((Predicate<EntityUid>) (ent => this._hive.IsMember((Entity<HiveMemberComponent>) ent, new EntityUid?((EntityUid) hive))));
      if (filter.Count <= 1)
      {
        this._popup.PopupEntity(this.Loc.GetString("cm-xeno-words-of-the-queen-nobody-hear-you"), (EntityUid) queen, (EntityUid) queen, PopupType.LargeCaution);
      }
      else
      {
        this._xenoPlasma.TryRemovePlasma((Entity<XenoPlasmaComponent>) queen.Owner, queen.Comp.PlasmaCost);
        string msg = this._newLineRegex.Replace(input, "\n\n");
        string str1 = this._cmChat.SanitizeMessageReplaceWords((EntityUid) queen, msg);
        string message = this.Loc.GetString("rmc-xeno-words-of-the-queen-header");
        string str2 = FormattedMessage.EscapeText(str1);
        string wrapped = $"{this._xenoAnnounce.WrapHive(message) ?? ""}[color=red][font size=14][bold]{str2}[/bold][/font][/color]";
        this._xenoAnnounce.Announce((EntityUid) queen, filter, str1, wrapped, queen.Comp.Sound);
        foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions((EntityUid) queen))
        {
          if (this.HasComp<XenoWordQueenActionComponent>(entityUid))
            this._actions.StartUseDelay(new Entity<ActionComponent>?((Entity<ActionComponent>) entityUid));
        }
      }
    }
    else
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-words-of-the-queen-nobody-hear-you"), (EntityUid) queen, new EntityUid?((EntityUid) queen), PopupType.LargeCaution);
  }
}
