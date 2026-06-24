// Decompiled with JetBrains decompiler
// Type: Content.Shared.DoAfter.DoAfterComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.DoAfter;

[NetSerializable]
[Serializable]
public sealed class DoAfterComponentState : ComponentState
{
  public readonly ushort NextId;
  public readonly Dictionary<ushort, Content.Shared.DoAfter.DoAfter> DoAfters;

  public DoAfterComponentState(IEntityManager entManager, DoAfterComponent component)
  {
    this.NextId = component.NextId;
    this.DoAfters = component.DoAfters;
  }
}
