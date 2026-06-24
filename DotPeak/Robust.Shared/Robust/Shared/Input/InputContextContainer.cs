// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.InputContextContainer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Input;

internal sealed class InputContextContainer : IInputContextContainer
{
  public const string DefaultContextName = "common";
  private readonly Dictionary<string, InputCmdContext> _contexts = new Dictionary<string, InputCmdContext>();
  private InputCmdContext _activeContext;
  private InputCmdContext? _deferredContextSwitch;
  private bool _deferringEnabled;

  public event EventHandler<ContextChangedEventArgs>? ContextChanged;

  public IInputCmdContext ActiveContext => (IInputCmdContext) this._activeContext;

  public bool DeferringEnabled
  {
    get => this._deferringEnabled;
    set
    {
      this._deferringEnabled = value;
      if (value || this._deferredContextSwitch == null)
        return;
      InputCmdContext deferredContextSwitch = this._deferredContextSwitch;
      this._deferredContextSwitch = (InputCmdContext) null;
      this._setActiveContextImmediately(deferredContextSwitch);
    }
  }

  public InputContextContainer()
  {
    this._contexts.Add("common", new InputCmdContext("common"));
    this.SetActiveContext("common");
  }

  public IInputCmdContext New(string uniqueName, string parentName)
  {
    if (string.IsNullOrWhiteSpace(uniqueName))
      throw new ArgumentException("String is null or whitespace.", nameof (uniqueName));
    if (string.IsNullOrWhiteSpace(parentName))
      throw new ArgumentException("String is null or whitespace.", nameof (parentName));
    InputCmdContext parent;
    if (!this._contexts.TryGetValue(parentName, out parent))
      throw new ArgumentException("Parent does not exist.", nameof (parentName));
    InputCmdContext inputCmdContext = !this._contexts.ContainsKey(uniqueName) ? new InputCmdContext((IInputCmdContext) parent, uniqueName) : throw new ArgumentException($"Context with name {uniqueName} already exists.", nameof (uniqueName));
    this._contexts.Add(uniqueName, inputCmdContext);
    return (IInputCmdContext) inputCmdContext;
  }

  public IInputCmdContext New(string uniqueName, IInputCmdContext parent)
  {
    if (string.IsNullOrWhiteSpace(uniqueName))
      throw new ArgumentException("String is null or whitespace.", nameof (uniqueName));
    if (this._contexts.ContainsKey(uniqueName))
      throw new ArgumentException($"Context with name {uniqueName} already exists.", nameof (uniqueName));
    InputCmdContext inputCmdContext = parent != null ? new InputCmdContext(parent, uniqueName) : throw new ArgumentNullException(nameof (parent));
    this._contexts.Add(uniqueName, inputCmdContext);
    return (IInputCmdContext) inputCmdContext;
  }

  public bool Exists(string uniqueName) => this._contexts.ContainsKey(uniqueName);

  public IInputCmdContext GetContext(string uniqueName)
  {
    return (IInputCmdContext) this._contexts[uniqueName];
  }

  public bool TryGetContext(string uniqueName, [NotNullWhen(true)] out IInputCmdContext? context)
  {
    InputCmdContext inputCmdContext;
    if (this._contexts.TryGetValue(uniqueName, out inputCmdContext))
    {
      context = (IInputCmdContext) inputCmdContext;
      return true;
    }
    context = (IInputCmdContext) null;
    return false;
  }

  public void Remove(string uniqueName)
  {
    if (uniqueName == "common")
      throw new ArgumentException("The default context cannot be removed.", nameof (uniqueName));
    this._contexts.Remove(uniqueName);
  }

  public void SetActiveContext(string uniqueName)
  {
    if (!this.DeferringEnabled)
      this._setActiveContextImmediately(this._contexts[uniqueName]);
    else
      this._deferredContextSwitch = this._contexts[uniqueName];
  }

  private void _setActiveContextImmediately(InputCmdContext icc)
  {
    ContextChangedEventArgs e = new ContextChangedEventArgs((IInputCmdContext) this._activeContext, (IInputCmdContext) icc);
    this._activeContext = icc;
    EventHandler<ContextChangedEventArgs> contextChanged = this.ContextChanged;
    if (contextChanged == null)
      return;
    contextChanged((object) this, e);
  }
}
