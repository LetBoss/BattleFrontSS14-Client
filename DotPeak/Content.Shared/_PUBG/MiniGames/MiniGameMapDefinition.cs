// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.MiniGames.MiniGameMapDefinition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared._PUBG.MiniGames;

[DataDefinition]
public sealed class MiniGameMapDefinition : 
  ISerializationGenerated<MiniGameMapDefinition>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Id { get; private set; } = string.Empty;

  [DataField(null, false, 1, true, false, null)]
  public string Name { get; private set; } = string.Empty;

  [DataField(null, false, 1, true, false, null)]
  public string Path { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public string Description { get; private set; } = string.Empty;

  public MiniGameMapDefinition()
  {
  }

  public MiniGameMapDefinition(string id, string name, string path, string description = "")
  {
    this.Id = id;
    this.Name = name;
    this.Path = path;
    this.Description = description;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MiniGameMapDefinition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<MiniGameMapDefinition>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Id == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Id, ref target1, hookCtx, false, context))
      target1 = this.Id;
    target.Id = target1;
    string target2 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target2, hookCtx, false, context))
      target2 = this.Name;
    target.Name = target2;
    string target3 = (string) null;
    if (this.Path == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Path, ref target3, hookCtx, false, context))
      target3 = this.Path;
    target.Path = target3;
    string target4 = (string) null;
    if (this.Description == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Description, ref target4, hookCtx, false, context))
      target4 = this.Description;
    target.Description = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MiniGameMapDefinition target,
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
    MiniGameMapDefinition target1 = (MiniGameMapDefinition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public MiniGameMapDefinition Instantiate() => new MiniGameMapDefinition();
}
