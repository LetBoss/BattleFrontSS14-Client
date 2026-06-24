// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.IInputContextContainer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Input;

[NotContentImplementable]
public interface IInputContextContainer
{
  IInputCmdContext ActiveContext { get; }

  bool DeferringEnabled { get; set; }

  event EventHandler<ContextChangedEventArgs> ContextChanged;

  IInputCmdContext New(string uniqueName, string parentName);

  IInputCmdContext New(string uniqueName, IInputCmdContext parent);

  bool Exists(string uniqueName);

  IInputCmdContext GetContext(string uniqueName);

  bool TryGetContext(string uniqueName, [NotNullWhen(true)] out IInputCmdContext? context);

  void Remove(string uniqueName);

  void SetActiveContext(string uniqueName);
}
