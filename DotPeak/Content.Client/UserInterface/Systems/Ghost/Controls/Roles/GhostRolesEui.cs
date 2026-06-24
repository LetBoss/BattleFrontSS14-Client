// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Ghost.Controls.Roles.GhostRolesEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Client.Players.PlayTimeTracking;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles;

public sealed class GhostRolesEui : BaseEui
{
  private readonly GhostRolesWindow _window;
  private GhostRoleRulesWindow? _windowRules;
  private uint _windowRulesId;

  public GhostRolesEui()
  {
    this._window = new GhostRolesWindow();
    this._window.OnRoleRequestButtonClicked += (Action<GhostRoleInfo>) (info =>
    {
      ((BaseWindow) this._windowRules)?.Close();
      if (info.Kind == GhostRoleKind.RaffleJoined)
      {
        this.SendMessage((EuiMessageBase) new LeaveGhostRoleRaffleMessage(info.Identifier));
      }
      else
      {
        this._windowRules = new GhostRoleRulesWindow(info.Rules, (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          this.SendMessage((EuiMessageBase) new RequestGhostRoleMessage(info.Identifier));
          if (info.Kind == GhostRoleKind.FirstComeFirstServe)
            return;
          ((BaseWindow) this._windowRules)?.Close();
        }));
        this._windowRulesId = info.Identifier;
        ((BaseWindow) this._windowRules).OnClose += (Action) (() => this._windowRules = (GhostRoleRulesWindow) null);
        ((BaseWindow) this._windowRules).OpenCentered();
      }
    });
    this._window.OnRoleFollow += (Action<GhostRoleInfo>) (info => this.SendMessage((EuiMessageBase) new FollowGhostRoleMessage(info.Identifier)));
    ((BaseWindow) this._window).OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new CloseEuiMessage()));
  }

  public override void Opened()
  {
    base.Opened();
    ((BaseWindow) this._window).OpenCentered();
  }

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this._window).Close();
    ((BaseWindow) this._windowRules)?.Close();
  }

  public override void HandleState(EuiStateBase state)
  {
    base.HandleState(state);
    if (!(state is GhostRolesEuiState ghostRolesEuiState))
      return;
    this._window.SaveCollapsibleBoxesStates();
    this._window.ClearEntries();
    SpriteSystem entitySystem = IoCManager.Resolve<IEntityManager>().EntitySysManager.GetEntitySystem<SpriteSystem>();
    JobRequirementsManager requirementsManager = IoCManager.Resolve<JobRequirementsManager>();
    foreach (IGrouping<(string, string, HashSet<JobRequirement>), GhostRoleInfo> roles in ((IEnumerable<GhostRoleInfo>) ghostRolesEuiState.GhostRoles).GroupBy<GhostRoleInfo, (string, string, HashSet<JobRequirement>)>((Func<GhostRoleInfo, (string, string, HashSet<JobRequirement>)>) (role => (role.Name, role.Description, role.Requirements))))
    {
      FormattedMessage reason;
      this._window.AddEntry(roles.Key.Item1, roles.Key.Item2, requirementsManager.CheckRoleRequirements(roles.Key.Item3, (HumanoidCharacterProfile) null, out reason), reason, (IEnumerable<GhostRoleInfo>) roles, entitySystem);
    }
    this._window.RestoreCollapsibleBoxesStates();
    if (!((IEnumerable<GhostRoleInfo>) ghostRolesEuiState.GhostRoles).All<GhostRoleInfo>((Func<GhostRoleInfo, bool>) (role => (int) role.Identifier != (int) this._windowRulesId)))
      return;
    ((BaseWindow) this._windowRules)?.Close();
  }
}
