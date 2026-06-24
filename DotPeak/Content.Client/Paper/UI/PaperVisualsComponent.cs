// Decompiled with JetBrains decompiler
// Type: Content.Client.Paper.UI.PaperVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Paper.UI;

[RegisterComponent]
public sealed class PaperVisualsComponent : 
  Component,
  ISerializationGenerated<PaperVisualsComponent>,
  ISerializationGenerated
{
  [DataField("backgroundImagePath", false, 1, false, false, null)]
  public string? BackgroundImagePath;
  [DataField("backgroundPatchMargin", false, 1, false, false, null)]
  public Box2 BackgroundPatchMargin;
  [DataField("backgroundModulate", false, 1, false, false, null)]
  public Color BackgroundModulate = Color.White;
  [DataField("backgroundImageTile", false, 1, false, false, null)]
  public bool BackgroundImageTile;
  [DataField("backgroundScale", false, 1, false, false, null)]
  public Vector2 BackgroundScale = Vector2.One;
  [DataField("headerImagePath", false, 1, false, false, null)]
  public string? HeaderImagePath;
  [DataField("headerImageModulate", false, 1, false, false, null)]
  public Color HeaderImageModulate = Color.White;
  [DataField("headerMargin", false, 1, false, false, null)]
  public Box2 HeaderMargin;
  [DataField(null, false, 1, false, false, null)]
  public ResPath? FooterImagePath;
  [DataField(null, false, 1, false, false, null)]
  public Color FooterImageModulate = Color.White;
  [DataField(null, false, 1, false, false, null)]
  public Box2 FooterMargin;
  [DataField("contentImagePath", false, 1, false, false, null)]
  public string? ContentImagePath;
  [DataField("contentImageModulate", false, 1, false, false, null)]
  public Color ContentImageModulate = Color.White;
  [DataField("contentMargin", false, 1, false, false, null)]
  public Box2 ContentMargin;
  [DataField("contentImageNumLines", false, 1, false, false, null)]
  public int ContentImageNumLines = 1;
  [DataField("fontAccentColor", false, 1, false, false, null)]
  public Color FontAccentColor = new Color((byte) 223, (byte) 223, (byte) 213, byte.MaxValue);
  [DataField("maxWritableArea", false, 1, false, false, null)]
  public Vector2? MaxWritableArea;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PaperVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PaperVisualsComponent) component;
    if (serialization.TryCustomCopy<PaperVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.BackgroundImagePath, ref str1, hookCtx, false, context))
      str1 = this.BackgroundImagePath;
    target.BackgroundImagePath = str1;
    Box2 box2_1 = new Box2();
    if (!serialization.TryCustomCopy<Box2>(this.BackgroundPatchMargin, ref box2_1, hookCtx, false, context))
      box2_1 = serialization.CreateCopy<Box2>(this.BackgroundPatchMargin, hookCtx, context, false);
    target.BackgroundPatchMargin = box2_1;
    Color color1 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.BackgroundModulate, ref color1, hookCtx, false, context))
      color1 = serialization.CreateCopy<Color>(this.BackgroundModulate, hookCtx, context, false);
    target.BackgroundModulate = color1;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.BackgroundImageTile, ref flag, hookCtx, false, context))
      flag = this.BackgroundImageTile;
    target.BackgroundImageTile = flag;
    Vector2 vector2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.BackgroundScale, ref vector2, hookCtx, false, context))
      vector2 = serialization.CreateCopy<Vector2>(this.BackgroundScale, hookCtx, context, false);
    target.BackgroundScale = vector2;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.HeaderImagePath, ref str2, hookCtx, false, context))
      str2 = this.HeaderImagePath;
    target.HeaderImagePath = str2;
    Color color2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.HeaderImageModulate, ref color2, hookCtx, false, context))
      color2 = serialization.CreateCopy<Color>(this.HeaderImageModulate, hookCtx, context, false);
    target.HeaderImageModulate = color2;
    Box2 box2_2 = new Box2();
    if (!serialization.TryCustomCopy<Box2>(this.HeaderMargin, ref box2_2, hookCtx, false, context))
      box2_2 = serialization.CreateCopy<Box2>(this.HeaderMargin, hookCtx, context, false);
    target.HeaderMargin = box2_2;
    ResPath? nullable1 = new ResPath?();
    if (!serialization.TryCustomCopy<ResPath?>(this.FooterImagePath, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<ResPath?>(this.FooterImagePath, hookCtx, context, false);
    target.FooterImagePath = nullable1;
    Color color3 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.FooterImageModulate, ref color3, hookCtx, false, context))
      color3 = serialization.CreateCopy<Color>(this.FooterImageModulate, hookCtx, context, false);
    target.FooterImageModulate = color3;
    Box2 box2_3 = new Box2();
    if (!serialization.TryCustomCopy<Box2>(this.FooterMargin, ref box2_3, hookCtx, false, context))
      box2_3 = serialization.CreateCopy<Box2>(this.FooterMargin, hookCtx, context, false);
    target.FooterMargin = box2_3;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ContentImagePath, ref str3, hookCtx, false, context))
      str3 = this.ContentImagePath;
    target.ContentImagePath = str3;
    Color color4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.ContentImageModulate, ref color4, hookCtx, false, context))
      color4 = serialization.CreateCopy<Color>(this.ContentImageModulate, hookCtx, context, false);
    target.ContentImageModulate = color4;
    Box2 box2_4 = new Box2();
    if (!serialization.TryCustomCopy<Box2>(this.ContentMargin, ref box2_4, hookCtx, false, context))
      box2_4 = serialization.CreateCopy<Box2>(this.ContentMargin, hookCtx, context, false);
    target.ContentMargin = box2_4;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.ContentImageNumLines, ref num, hookCtx, false, context))
      num = this.ContentImageNumLines;
    target.ContentImageNumLines = num;
    Color color5 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.FontAccentColor, ref color5, hookCtx, false, context))
      color5 = serialization.CreateCopy<Color>(this.FontAccentColor, hookCtx, context, false);
    target.FontAccentColor = color5;
    Vector2? nullable2 = new Vector2?();
    if (!serialization.TryCustomCopy<Vector2?>(this.MaxWritableArea, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<Vector2?>(this.MaxWritableArea, hookCtx, context, false);
    target.MaxWritableArea = nullable2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PaperVisualsComponent target,
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
    PaperVisualsComponent target1 = (PaperVisualsComponent) target;
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
    PaperVisualsComponent target1 = (PaperVisualsComponent) target;
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
    PaperVisualsComponent target1 = (PaperVisualsComponent) target;
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
  virtual PaperVisualsComponent Component.Instantiate() => new PaperVisualsComponent();
}
