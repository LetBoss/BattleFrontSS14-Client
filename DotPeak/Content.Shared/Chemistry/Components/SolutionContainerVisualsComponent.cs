// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.SolutionContainerVisualsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
public sealed class SolutionContainerVisualsComponent : 
  Component,
  ISerializationGenerated<SolutionContainerVisualsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int MaxFillLevels;
  [DataField(null, false, 1, false, false, null)]
  public string? FillBaseName;
  [DataField(null, false, 1, false, false, null)]
  public SolutionContainerLayers Layer;
  [DataField(null, false, 1, false, false, null)]
  public SolutionContainerLayers BaseLayer = SolutionContainerLayers.Base;
  [DataField(null, false, 1, false, false, null)]
  public SolutionContainerLayers OverlayLayer = SolutionContainerLayers.Overlay;
  [DataField(null, false, 1, false, false, null)]
  public bool ChangeColor = true;
  [DataField(null, false, 1, false, false, null)]
  public string? EmptySpriteName;
  [DataField(null, false, 1, false, false, null)]
  public Color EmptySpriteColor = Color.White;
  [DataField(null, false, 1, false, false, null)]
  public bool Metamorphic;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? MetamorphicDefaultSprite;
  [DataField(null, false, 1, false, false, null)]
  public LocId MetamorphicNameFull = LocId.op_Implicit("transformable-container-component-glass");
  [DataField(null, false, 1, false, false, null)]
  public string? SolutionName;
  [DataField(null, false, 1, false, false, null)]
  public string InitialDescription = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string? InHandsFillBaseName;
  [DataField(null, false, 1, false, false, null)]
  public int InHandsMaxFillLevels;
  [DataField(null, false, 1, false, false, null)]
  public string? EquippedFillBaseName;
  [DataField(null, false, 1, false, false, null)]
  public int EquippedMaxFillLevels;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SolutionContainerVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SolutionContainerVisualsComponent) component;
    if (serialization.TryCustomCopy<SolutionContainerVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxFillLevels, ref num1, hookCtx, false, context))
      num1 = this.MaxFillLevels;
    target.MaxFillLevels = num1;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.FillBaseName, ref str1, hookCtx, false, context))
      str1 = this.FillBaseName;
    target.FillBaseName = str1;
    SolutionContainerLayers solutionContainerLayers1 = SolutionContainerLayers.Fill;
    if (!serialization.TryCustomCopy<SolutionContainerLayers>(this.Layer, ref solutionContainerLayers1, hookCtx, false, context))
      solutionContainerLayers1 = this.Layer;
    target.Layer = solutionContainerLayers1;
    SolutionContainerLayers solutionContainerLayers2 = SolutionContainerLayers.Fill;
    if (!serialization.TryCustomCopy<SolutionContainerLayers>(this.BaseLayer, ref solutionContainerLayers2, hookCtx, false, context))
      solutionContainerLayers2 = this.BaseLayer;
    target.BaseLayer = solutionContainerLayers2;
    SolutionContainerLayers solutionContainerLayers3 = SolutionContainerLayers.Fill;
    if (!serialization.TryCustomCopy<SolutionContainerLayers>(this.OverlayLayer, ref solutionContainerLayers3, hookCtx, false, context))
      solutionContainerLayers3 = this.OverlayLayer;
    target.OverlayLayer = solutionContainerLayers3;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.ChangeColor, ref flag1, hookCtx, false, context))
      flag1 = this.ChangeColor;
    target.ChangeColor = flag1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.EmptySpriteName, ref str2, hookCtx, false, context))
      str2 = this.EmptySpriteName;
    target.EmptySpriteName = str2;
    Color color = new Color();
    if (!serialization.TryCustomCopy<Color>(this.EmptySpriteColor, ref color, hookCtx, false, context))
      color = serialization.CreateCopy<Color>(this.EmptySpriteColor, hookCtx, context, false);
    target.EmptySpriteColor = color;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Metamorphic, ref flag2, hookCtx, false, context))
      flag2 = this.Metamorphic;
    target.Metamorphic = flag2;
    SpriteSpecifier spriteSpecifier = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.MetamorphicDefaultSprite, ref spriteSpecifier, hookCtx, true, context))
      spriteSpecifier = serialization.CreateCopy<SpriteSpecifier>(this.MetamorphicDefaultSprite, hookCtx, context, false);
    target.MetamorphicDefaultSprite = spriteSpecifier;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.MetamorphicNameFull, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.MetamorphicNameFull, hookCtx, context, false);
    target.MetamorphicNameFull = locId;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.SolutionName, ref str3, hookCtx, false, context))
      str3 = this.SolutionName;
    target.SolutionName = str3;
    string str4 = (string) null;
    if (this.InitialDescription == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.InitialDescription, ref str4, hookCtx, false, context))
      str4 = this.InitialDescription;
    target.InitialDescription = str4;
    string str5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.InHandsFillBaseName, ref str5, hookCtx, false, context))
      str5 = this.InHandsFillBaseName;
    target.InHandsFillBaseName = str5;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.InHandsMaxFillLevels, ref num2, hookCtx, false, context))
      num2 = this.InHandsMaxFillLevels;
    target.InHandsMaxFillLevels = num2;
    string str6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.EquippedFillBaseName, ref str6, hookCtx, false, context))
      str6 = this.EquippedFillBaseName;
    target.EquippedFillBaseName = str6;
    int num3 = 0;
    if (!serialization.TryCustomCopy<int>(this.EquippedMaxFillLevels, ref num3, hookCtx, false, context))
      num3 = this.EquippedMaxFillLevels;
    target.EquippedMaxFillLevels = num3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SolutionContainerVisualsComponent target,
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
    SolutionContainerVisualsComponent target1 = (SolutionContainerVisualsComponent) target;
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
    SolutionContainerVisualsComponent target1 = (SolutionContainerVisualsComponent) target;
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
    SolutionContainerVisualsComponent target1 = (SolutionContainerVisualsComponent) target;
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
  virtual SolutionContainerVisualsComponent Component.Instantiate()
  {
    return new SolutionContainerVisualsComponent();
  }
}
