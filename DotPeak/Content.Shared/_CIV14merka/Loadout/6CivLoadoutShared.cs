// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Loadout.CivLoadoutStateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Loadout;

[NetSerializable]
[Serializable]
public sealed class CivLoadoutStateEvent : EntityEventArgs
{
  public readonly Dictionary<string, List<string>> Disabled;
  public readonly Dictionary<string, List<string>> Selections;
  public readonly List<string> Owned;

  public CivLoadoutStateEvent(
    Dictionary<string, List<string>> disabled,
    Dictionary<string, List<string>> selections,
    List<string> owned)
  {
    this.Disabled = disabled;
    this.Selections = selections;
    this.Owned = owned;
  }
}
