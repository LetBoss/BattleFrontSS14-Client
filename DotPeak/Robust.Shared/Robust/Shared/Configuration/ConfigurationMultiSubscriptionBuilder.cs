// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.ConfigurationMultiSubscriptionBuilder
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Configuration;

public sealed class ConfigurationMultiSubscriptionBuilder(IConfigurationManager manager) : 
  IDisposable
{
  private readonly List<Action> _unsubscribeActions = new List<Action>();

  public ConfigurationMultiSubscriptionBuilder OnValueChanged<T>(
    CVarDef<T> cVar,
    CVarChanged<T> onValueChanged,
    bool invokeImmediately = false)
    where T : notnull
  {
    manager.OnValueChanged<T>(cVar, onValueChanged, invokeImmediately);
    this._unsubscribeActions.Add((Action) (() => manager.UnsubValueChanged<T>(cVar, onValueChanged)));
    return this;
  }

  public ConfigurationMultiSubscriptionBuilder OnValueChanged<T>(
    string name,
    CVarChanged<T> onValueChanged,
    bool invokeImmediately = false)
    where T : notnull
  {
    manager.OnValueChanged<T>(name, onValueChanged, invokeImmediately);
    this._unsubscribeActions.Add((Action) (() => manager.UnsubValueChanged<T>(name, onValueChanged)));
    return this;
  }

  public ConfigurationMultiSubscriptionBuilder OnValueChanged<T>(
    CVarDef<T> cVar,
    Action<T> onValueChanged,
    bool invokeImmediately = false)
    where T : notnull
  {
    manager.OnValueChanged<T>(cVar, onValueChanged, invokeImmediately);
    this._unsubscribeActions.Add((Action) (() => manager.UnsubValueChanged<T>(cVar, onValueChanged)));
    return this;
  }

  public ConfigurationMultiSubscriptionBuilder OnValueChanged<T>(
    string name,
    Action<T> onValueChanged,
    bool invokeImmediately = false)
    where T : notnull
  {
    manager.OnValueChanged<T>(name, onValueChanged, invokeImmediately);
    this._unsubscribeActions.Add((Action) (() => manager.UnsubValueChanged<T>(name, onValueChanged)));
    return this;
  }

  public void Dispose()
  {
    foreach (Action unsubscribeAction in this._unsubscribeActions)
      unsubscribeAction();
    this._unsubscribeActions.Clear();
  }
}
