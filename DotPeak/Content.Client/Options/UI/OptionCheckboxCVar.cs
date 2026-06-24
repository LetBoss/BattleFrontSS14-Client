// Decompiled with JetBrains decompiler
// Type: Content.Client.Options.UI.OptionCheckboxCVar
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using System;

#nullable enable
namespace Content.Client.Options.UI;

public sealed class OptionCheckboxCVar : BaseOptionCVar<bool>
{
  private readonly CheckBox _checkBox;
  private readonly bool _invert;

  protected override bool Value
  {
    get => ((BaseButton) this._checkBox).Pressed ^ this._invert;
    set => ((BaseButton) this._checkBox).Pressed = value ^ this._invert;
  }

  public OptionCheckboxCVar(
    OptionsTabControlRow controller,
    IConfigurationManager cfg,
    CVarDef<bool> cVar,
    CheckBox checkBox,
    bool invert)
    : base(controller, cfg, cVar)
  {
    this._checkBox = checkBox;
    this._invert = invert;
    ((BaseButton) checkBox).OnToggled += (Action<BaseButton.ButtonToggledEventArgs>) (_ => this.ValueChanged());
  }
}
