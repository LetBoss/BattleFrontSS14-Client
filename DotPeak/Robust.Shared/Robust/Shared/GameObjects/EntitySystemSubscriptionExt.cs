// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntitySystemSubscriptionExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

public static class EntitySystemSubscriptionExt
{
  public static void CVar<T>(
    this EntitySystem.Subscriptions subs,
    IConfigurationManager cfg,
    string name,
    Action<T> onValueChanged,
    bool invokeImmediately = false)
    where T : notnull
  {
    cfg.OnValueChanged<T>(name, onValueChanged, invokeImmediately);
    subs.RegisterUnsubscription((Action) (() => cfg.UnsubValueChanged<T>(name, onValueChanged)));
  }

  public static void CVar<T>(
    this EntitySystem.Subscriptions subs,
    IConfigurationManager cfg,
    CVarDef<T> cVar,
    Action<T> onValueChanged,
    bool invokeImmediately = false)
    where T : notnull
  {
    cfg.OnValueChanged<T>(cVar, onValueChanged, invokeImmediately);
    subs.RegisterUnsubscription((Action) (() => cfg.UnsubValueChanged<T>(cVar, onValueChanged)));
  }

  public static void CVar<T>(
    this EntitySystem.Subscriptions subs,
    IConfigurationManager cfg,
    string name,
    CVarChanged<T> onValueChanged,
    bool invokeImmediately = false)
    where T : notnull
  {
    cfg.OnValueChanged<T>(name, onValueChanged, invokeImmediately);
    subs.RegisterUnsubscription((Action) (() => cfg.UnsubValueChanged<T>(name, onValueChanged)));
  }

  public static void CVar<T>(
    this EntitySystem.Subscriptions subs,
    IConfigurationManager cfg,
    CVarDef<T> cVar,
    CVarChanged<T> onValueChanged,
    bool invokeImmediately = false)
    where T : notnull
  {
    cfg.OnValueChanged<T>(cVar, onValueChanged, invokeImmediately);
    subs.RegisterUnsubscription((Action) (() => cfg.UnsubValueChanged<T>(cVar, onValueChanged)));
  }
}
