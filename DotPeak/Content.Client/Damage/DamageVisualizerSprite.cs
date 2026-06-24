// Decompiled with JetBrains decompiler
// Type: Content.Client.Damage.DamageVisualizerSprite
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Client.Damage;

[DataDefinition]
public sealed class DamageVisualizerSprite : 
  ISerializationGenerated<DamageVisualizerSprite>,
  ISerializationGenerated
{
  [DataField("sprite", false, 1, true, false, null)]
  public string Sprite;
  [DataField("color", false, 1, false, false, null)]
  public string? Color;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageVisualizerSprite target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<DamageVisualizerSprite>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.Sprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Sprite, ref str1, hookCtx, false, context))
      str1 = this.Sprite;
    target.Sprite = str1;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Color, ref str2, hookCtx, false, context))
      str2 = this.Color;
    target.Color = str2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageVisualizerSprite target,
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
    DamageVisualizerSprite target1 = (DamageVisualizerSprite) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public DamageVisualizerSprite Instantiate() => new DamageVisualizerSprite();
}
