// Decompiled with JetBrains decompiler
// Type: Content.Shared.Paper.StampDisplayInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Paper;

[DataDefinition]
[NetSerializable]
[Serializable]
public struct StampDisplayInfo : ISerializationGenerated<StampDisplayInfo>, ISerializationGenerated
{
  [DataField("stampedName", false, 1, false, false, null)]
  public string StampedName;
  [DataField("stampedColor", false, 1, false, false, null)]
  public Color StampedColor;

  private StampDisplayInfo(string s)
  {
    this.StampedColor = new Color();
    this.StampedName = s;
  }

  public StampDisplayInfo()
  {
    this.StampedName = (string) null;
    this.StampedColor = new Color();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StampDisplayInfo target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<StampDisplayInfo>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.StampedName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StampedName, ref target1, hookCtx, false, context))
      target1 = this.StampedName;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.StampedColor, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.StampedColor, hookCtx, context);
    target = target with
    {
      StampedName = target1,
      StampedColor = target2
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StampDisplayInfo target,
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
    StampDisplayInfo target1 = (StampDisplayInfo) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public StampDisplayInfo Instantiate() => new StampDisplayInfo();
}
