// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.Cartridges.NanoTaskItem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.CartridgeLoader.Cartridges;

[NetSerializable]
[DataRecord]
[Serializable]
public sealed class NanoTaskItem
{
  public static int MaximumStringLength = 30;
  public readonly string Description;
  public readonly string TaskIsFor;
  public readonly bool IsTaskDone;
  public readonly NanoTaskPriority Priority;

  public NanoTaskItem(
    string description,
    string taskIsFor,
    bool isTaskDone,
    NanoTaskPriority priority)
  {
    this.Description = description;
    this.TaskIsFor = taskIsFor;
    this.IsTaskDone = isTaskDone;
    this.Priority = priority;
  }

  public bool Validate()
  {
    return this.Description.Length <= NanoTaskItem.MaximumStringLength && this.TaskIsFor.Length <= NanoTaskItem.MaximumStringLength;
  }
}
