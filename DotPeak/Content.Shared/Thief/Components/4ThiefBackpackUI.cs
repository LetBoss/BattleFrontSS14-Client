// Decompiled with JetBrains decompiler
// Type: Content.Shared.Thief.ThiefBackpackSetInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Thief;

[NetSerializable]
[DataDefinition]
[Serializable]
public struct ThiefBackpackSetInfo : 
  ISerializationGenerated<ThiefBackpackSetInfo>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string Name;
  [DataField(null, false, 1, false, false, null)]
  public string Description;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier Sprite;
  public bool Selected;

  public ThiefBackpackSetInfo(string name, string desc, SpriteSpecifier sprite, bool selected)
  {
    this.Name = name;
    this.Description = desc;
    this.Sprite = sprite;
    this.Selected = selected;
  }

  public ThiefBackpackSetInfo()
  {
    this.Name = (string) null;
    this.Description = (string) null;
    this.Sprite = (SpriteSpecifier) null;
    this.Selected = false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ThiefBackpackSetInfo target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ThiefBackpackSetInfo>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target1, hookCtx, false, context))
      target1 = this.Name;
    string target2 = (string) null;
    if (this.Description == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Description, ref target2, hookCtx, false, context))
      target2 = this.Description;
    SpriteSpecifier target3 = (SpriteSpecifier) null;
    if (this.Sprite == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.Sprite, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<SpriteSpecifier>(this.Sprite, hookCtx, context);
    target = target with
    {
      Name = target1,
      Description = target2,
      Sprite = target3
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ThiefBackpackSetInfo target,
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
    ThiefBackpackSetInfo target1 = (ThiefBackpackSetInfo) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ThiefBackpackSetInfo Instantiate() => new ThiefBackpackSetInfo();
}
