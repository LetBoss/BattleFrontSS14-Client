// Decompiled with JetBrains decompiler
// Type: Content.Client.RoundEnd.RoundEndSummaryUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.GameTicking.Managers;
using Content.Shared.GameTicking;
using Content.Shared.Input;
using Robust.Client.Input;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Player;

#nullable enable
namespace Content.Client.RoundEnd;

public sealed class RoundEndSummaryUIController : UIController, IOnSystemLoaded<ClientGameTicker>
{
  [Dependency]
  private IInputManager _input;
  private RoundEndSummaryWindow? _window;

  private void ToggleScoreboardWindow(ICommonSession? session = null)
  {
    if (this._window == null)
      return;
    if (((BaseWindow) this._window).IsOpen)
    {
      ((BaseWindow) this._window).Close();
    }
    else
    {
      ((BaseWindow) this._window).OpenCenteredRight();
      ((BaseWindow) this._window).MoveToFront();
    }
  }

  public void OpenRoundEndSummaryWindow(RoundEndMessageEvent message)
  {
    int? roundId1 = this._window?.RoundId;
    int roundId2 = message.RoundId;
    if (roundId1.GetValueOrDefault() == roundId2 & roundId1.HasValue)
      return;
    this._window = new RoundEndSummaryWindow(message.GamemodeTitle, message.RoundEndText, message.RoundDuration, message.RoundId, message.AllPlayersEndInfo, this.EntityManager);
  }

  public void OnSystemLoaded(ClientGameTicker system)
  {
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.ToggleRoundEndSummaryWindow, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(ToggleScoreboardWindow)), (StateInputCmdDelegate) null, true, true));
  }
}
