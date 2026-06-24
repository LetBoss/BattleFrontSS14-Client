// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.Loot.IDungeonLoot
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Procedural.Loot;

[ImplicitDataDefinitionForInheritors]
public interface IDungeonLoot : ISerializationGenerated<IDungeonLoot>, ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void InternalCopy(
    ref IDungeonLoot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<IDungeonLoot>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref IDungeonLoot target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  new void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IDungeonLoot target1 = (IDungeonLoot) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  new IDungeonLoot Instantiate() => throw new NotImplementedException();
}
