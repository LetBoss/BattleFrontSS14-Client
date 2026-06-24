// Decompiled with JetBrains decompiler
// Type: Content.Client.Actions.ActionTargetAttemptEvent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;

#nullable enable
namespace Content.Client.Actions;

[ByRefEvent]
public record struct ActionTargetAttemptEvent(
  PointerInputCmdHandler.PointerInputCmdArgs Input,
  Entity<ActionsComponent> User,
  ActionComponent Action,
  bool Handled = false,
  bool FoundTarget = false)
;
