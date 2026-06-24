// Decompiled with JetBrains decompiler
// Type: Content.Client.Atmos.UI.GasMixerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Atmos.Piping.Trinary.Components;
using Content.Shared.Localizations;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Atmos.UI;

public sealed class GasMixerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private const float MaxPressure = 4500f;
  [Robust.Shared.ViewVariables.ViewVariables]
  private GasMixerWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<GasMixerWindow>((BoundUserInterface) this);
    this._window.ToggleStatusButtonPressed += new Action(this.OnToggleStatusButtonPressed);
    this._window.MixerOutputPressureChanged += new Action<string>(this.OnMixerOutputPressurePressed);
    this._window.MixerNodePercentageChanged += new Action<string>(this.OnMixerSetPercentagePressed);
  }

  private void OnToggleStatusButtonPressed()
  {
    if (this._window == null)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new GasMixerToggleStatusMessage(this._window.MixerStatus));
  }

  private void OnMixerOutputPressurePressed(string value)
  {
    float result;
    float pressure = UserInputParser.TryFloat((ReadOnlySpan<char>) value, out result) ? result : 0.0f;
    if ((double) pressure > 4500.0)
      pressure = 4500f;
    this.SendMessage((BoundUserInterfaceMessage) new GasMixerChangeOutputPressureMessage(pressure));
  }

  private void OnMixerSetPercentagePressed(string value)
  {
    float result;
    float nodeOne = Math.Clamp(UserInputParser.TryFloat((ReadOnlySpan<char>) value, out result) ? result : 1f, 0.0f, 100f);
    if (this._window != null)
      nodeOne = this._window.NodeOneLastEdited ? nodeOne : 100f - nodeOne;
    this.SendMessage((BoundUserInterfaceMessage) new GasMixerChangeNodePercentageMessage(nodeOne));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is GasMixerBoundUserInterfaceState userInterfaceState))
      return;
    this._window.Title = userInterfaceState.MixerLabel;
    this._window.SetMixerStatus(userInterfaceState.Enabled);
    this._window.SetOutputPressure(userInterfaceState.OutputPressure);
    this._window.SetNodePercentages(userInterfaceState.NodeOne);
  }
}
