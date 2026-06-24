// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.TacticalMap.TacticalMapSettingRegistration
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Client._RMC14.TacticalMap;

[DataDefinition]
[Serializable]
public struct TacticalMapSettingRegistration : 
  ISerializationGenerated<TacticalMapSettingRegistration>,
  ISerializationGenerated
{
  [DataField("Key", false, 1, false, false, null)]
  public string? Key { get; set; }

  [DataField("Value", false, 1, false, false, null)]
  public object? Value { get; set; }

  [DataField("PlanetId", false, 1, false, false, null)]
  public string? PlanetId { get; set; }

  public TacticalMapSettingRegistration()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CKey\u003Ek__BackingField = (string) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003CValue\u003Ek__BackingField = (object) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003CPlanetId\u003Ek__BackingField = (string) null;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TacticalMapSettingRegistration target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<TacticalMapSettingRegistration>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Key, ref str1, hookCtx, false, context))
      str1 = this.Key;
    object obj = (object) null;
    if (!serialization.TryCustomCopy<object>(this.Value, ref obj, hookCtx, true, context))
      obj = serialization.CreateCopy(this.Value, hookCtx, context, false);
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.PlanetId, ref str2, hookCtx, false, context))
      str2 = this.PlanetId;
    target = target with
    {
      Key = str1,
      Value = obj,
      PlanetId = str2
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TacticalMapSettingRegistration target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TacticalMapSettingRegistration target1 = (TacticalMapSettingRegistration) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public TacticalMapSettingRegistration Instantiate() => new TacticalMapSettingRegistration();
}
