// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.Laws.Ui.SiliconLawBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Silicons.Laws;
using Content.Shared.Silicons.Laws.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Silicons.Laws.Ui;

public sealed class SiliconLawBoundUserInterface : BoundUserInterface
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private SiliconLawMenu? _menu;
  private EntityUid _owner;
  private List<SiliconLaw>? _laws;

  public SiliconLawBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._owner = owner;
  }

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<SiliconLawMenu>((BoundUserInterface) this);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is SiliconLawBuiState state1))
      return;
    if (this._laws != null && this._laws.Count == state1.Laws.Count)
    {
      bool flag = true;
      foreach (SiliconLaw law in state1.Laws)
      {
        if (!this._laws.Contains(law))
        {
          flag = false;
          break;
        }
      }
      if (flag)
        return;
    }
    this._laws = state1.Laws.ToList<SiliconLaw>();
    this._menu?.Update(this._owner, state1);
  }
}
