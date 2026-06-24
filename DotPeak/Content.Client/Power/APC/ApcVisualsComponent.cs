// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.APC.ApcVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Power.APC;

[RegisterComponent]
[Access(new Type[] {typeof (ApcVisualizerSystem)})]
public sealed class ApcVisualsComponent : 
  Component,
  ISerializationGenerated<ApcVisualsComponent>,
  ISerializationGenerated
{
  [DataField("numLockIndicators", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public byte LockIndicators = 2;
  [DataField("lockIndicatorPrefix", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string LockPrefix = "lock";
  [DataField("lockIndicatorSuffixes", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string[] LockSuffixes = new string[2]
  {
    "unlocked",
    "locked"
  };
  [DataField("numChannelIndicators", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public byte ChannelIndicators = 3;
  [DataField("channelIndicatorPrefix", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string ChannelPrefix = "channel";
  [DataField("channelIndicatorSuffixes", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string[] ChannelSuffixes = new string[4]
  {
    "auto_off",
    "manual_off",
    "auto_on",
    "manual_on"
  };
  [DataField("screenStatePrefix", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string ScreenPrefix = "display";
  [DataField("screenStateSuffixes", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string[] ScreenSuffixes = new string[4]
  {
    "lack",
    "charging",
    "full",
    "remote"
  };
  [DataField("screenColors", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Color[] ScreenColors = new Color[4]
  {
    Color.FromHex((ReadOnlySpan<char>) "#d1332e", new Color?()),
    Color.FromHex((ReadOnlySpan<char>) "#dcdc28", new Color?()),
    Color.FromHex((ReadOnlySpan<char>) "#82ff4c", new Color?()),
    Color.FromHex((ReadOnlySpan<char>) "#ffac1c", new Color?())
  };
  [DataField("emaggedScreenState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string EmaggedScreenState = "emag-unlit";
  [DataField("emaggedScreenColor", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Color EmaggedScreenColor = Color.FromHex((ReadOnlySpan<char>) "#1f48d6", new Color?());

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ApcVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ApcVisualsComponent) component;
    if (serialization.TryCustomCopy<ApcVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    byte num1 = 0;
    if (!serialization.TryCustomCopy<byte>(this.LockIndicators, ref num1, hookCtx, false, context))
      num1 = this.LockIndicators;
    target.LockIndicators = num1;
    string str1 = (string) null;
    if (this.LockPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LockPrefix, ref str1, hookCtx, false, context))
      str1 = this.LockPrefix;
    target.LockPrefix = str1;
    string[] strArray1 = (string[]) null;
    if (this.LockSuffixes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string[]>(this.LockSuffixes, ref strArray1, hookCtx, true, context))
      strArray1 = serialization.CreateCopy<string[]>(this.LockSuffixes, hookCtx, context, false);
    target.LockSuffixes = strArray1;
    byte num2 = 0;
    if (!serialization.TryCustomCopy<byte>(this.ChannelIndicators, ref num2, hookCtx, false, context))
      num2 = this.ChannelIndicators;
    target.ChannelIndicators = num2;
    string str2 = (string) null;
    if (this.ChannelPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ChannelPrefix, ref str2, hookCtx, false, context))
      str2 = this.ChannelPrefix;
    target.ChannelPrefix = str2;
    string[] strArray2 = (string[]) null;
    if (this.ChannelSuffixes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string[]>(this.ChannelSuffixes, ref strArray2, hookCtx, true, context))
      strArray2 = serialization.CreateCopy<string[]>(this.ChannelSuffixes, hookCtx, context, false);
    target.ChannelSuffixes = strArray2;
    string str3 = (string) null;
    if (this.ScreenPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ScreenPrefix, ref str3, hookCtx, false, context))
      str3 = this.ScreenPrefix;
    target.ScreenPrefix = str3;
    string[] strArray3 = (string[]) null;
    if (this.ScreenSuffixes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string[]>(this.ScreenSuffixes, ref strArray3, hookCtx, true, context))
      strArray3 = serialization.CreateCopy<string[]>(this.ScreenSuffixes, hookCtx, context, false);
    target.ScreenSuffixes = strArray3;
    Color[] colorArray = (Color[]) null;
    if (this.ScreenColors == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Color[]>(this.ScreenColors, ref colorArray, hookCtx, true, context))
      colorArray = serialization.CreateCopy<Color[]>(this.ScreenColors, hookCtx, context, false);
    target.ScreenColors = colorArray;
    string str4 = (string) null;
    if (this.EmaggedScreenState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.EmaggedScreenState, ref str4, hookCtx, false, context))
      str4 = this.EmaggedScreenState;
    target.EmaggedScreenState = str4;
    Color color = new Color();
    if (!serialization.TryCustomCopy<Color>(this.EmaggedScreenColor, ref color, hookCtx, false, context))
      color = serialization.CreateCopy<Color>(this.EmaggedScreenColor, hookCtx, context, false);
    target.EmaggedScreenColor = color;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ApcVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ApcVisualsComponent target1 = (ApcVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ApcVisualsComponent target1 = (ApcVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ApcVisualsComponent target1 = (ApcVisualsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ApcVisualsComponent Component.Instantiate() => new ApcVisualsComponent();
}
