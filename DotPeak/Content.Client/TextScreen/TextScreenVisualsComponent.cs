// Decompiled with JetBrains decompiler
// Type: Content.Client.TextScreen.TextScreenVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.TextScreen;

[RegisterComponent]
public sealed class TextScreenVisualsComponent : 
  Component,
  ISerializationGenerated<TextScreenVisualsComponent>,
  ISerializationGenerated
{
  public const float PixelSize = 0.03125f;
  [DataField("color", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Color Color = new Color((byte) 15, (byte) 151, (byte) 251, byte.MaxValue);
  [DataField("textOffset", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 TextOffset = Vector2.Zero;
  [DataField("timerOffset", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 TimerOffset = Vector2.Zero;
  [DataField("rows", false, 1, false, false, null)]
  public int Rows = 2;
  [DataField("rowOffset", false, 1, false, false, null)]
  public int RowOffset = 7;
  [DataField("rowLength", false, 1, false, false, null)]
  public int RowLength = 5;
  [DataField("text", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string?[] Text = new string[2];
  public string?[] TextToDraw = new string[2];
  [DataField("layerStatesToDraw", false, 1, false, false, null)]
  public Dictionary<string, string?> LayerStatesToDraw = new Dictionary<string, string>();
  [DataField("hourFormat", false, 1, false, false, null)]
  public string HourFormat = "D2";
  [DataField("minuteFormat", false, 1, false, false, null)]
  public string MinuteFormat = "D2";
  [DataField("secondFormat", false, 1, false, false, null)]
  public string SecondFormat = "D2";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TextScreenVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (TextScreenVisualsComponent) component;
    if (serialization.TryCustomCopy<TextScreenVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    Color color = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref color, hookCtx, false, context))
      color = serialization.CreateCopy<Color>(this.Color, hookCtx, context, false);
    target.Color = color;
    Vector2 vector2_1 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.TextOffset, ref vector2_1, hookCtx, false, context))
      vector2_1 = serialization.CreateCopy<Vector2>(this.TextOffset, hookCtx, context, false);
    target.TextOffset = vector2_1;
    Vector2 vector2_2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.TimerOffset, ref vector2_2, hookCtx, false, context))
      vector2_2 = serialization.CreateCopy<Vector2>(this.TimerOffset, hookCtx, context, false);
    target.TimerOffset = vector2_2;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.Rows, ref num1, hookCtx, false, context))
      num1 = this.Rows;
    target.Rows = num1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.RowOffset, ref num2, hookCtx, false, context))
      num2 = this.RowOffset;
    target.RowOffset = num2;
    int num3 = 0;
    if (!serialization.TryCustomCopy<int>(this.RowLength, ref num3, hookCtx, false, context))
      num3 = this.RowLength;
    target.RowLength = num3;
    string[] strArray = (string[]) null;
    if (this.Text == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string[]>(this.Text, ref strArray, hookCtx, true, context))
      strArray = serialization.CreateCopy<string[]>(this.Text, hookCtx, context, false);
    target.Text = strArray;
    Dictionary<string, string> dictionary = (Dictionary<string, string>) null;
    if (this.LayerStatesToDraw == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, string>>(this.LayerStatesToDraw, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, string>>(this.LayerStatesToDraw, hookCtx, context, false);
    target.LayerStatesToDraw = dictionary;
    string str1 = (string) null;
    if (this.HourFormat == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HourFormat, ref str1, hookCtx, false, context))
      str1 = this.HourFormat;
    target.HourFormat = str1;
    string str2 = (string) null;
    if (this.MinuteFormat == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.MinuteFormat, ref str2, hookCtx, false, context))
      str2 = this.MinuteFormat;
    target.MinuteFormat = str2;
    string str3 = (string) null;
    if (this.SecondFormat == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SecondFormat, ref str3, hookCtx, false, context))
      str3 = this.SecondFormat;
    target.SecondFormat = str3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TextScreenVisualsComponent target,
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
    TextScreenVisualsComponent target1 = (TextScreenVisualsComponent) target;
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
    TextScreenVisualsComponent target1 = (TextScreenVisualsComponent) target;
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
    TextScreenVisualsComponent target1 = (TextScreenVisualsComponent) target;
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
  virtual TextScreenVisualsComponent Component.Instantiate() => new TextScreenVisualsComponent();
}
