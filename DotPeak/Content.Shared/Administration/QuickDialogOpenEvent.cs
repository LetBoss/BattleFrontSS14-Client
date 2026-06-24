// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.QuickDialogOpenEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Administration;

[NetSerializable]
[Serializable]
public sealed class QuickDialogOpenEvent : EntityEventArgs
{
  public string Title;
  public int DialogId;
  public List<QuickDialogEntry> Prompts;
  public QuickDialogButtonFlag Buttons = QuickDialogButtonFlag.OkButton | QuickDialogButtonFlag.CancelButton;

  public QuickDialogOpenEvent(
    string title,
    List<QuickDialogEntry> prompts,
    int dialogId,
    QuickDialogButtonFlag buttons)
  {
    this.Title = title;
    this.Prompts = prompts;
    this.Buttons = buttons;
    this.DialogId = dialogId;
  }
}
