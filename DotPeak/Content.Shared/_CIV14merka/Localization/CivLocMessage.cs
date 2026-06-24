// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Localization.CivLocMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Localization;

[NetSerializable]
[Serializable]
public sealed class CivLocMessage
{
  public string Id = string.Empty;
  public List<CivLocArg> Args = new List<CivLocArg>();

  public CivLocMessage()
  {
  }

  public CivLocMessage(string id, params CivLocArg[] args)
  {
    this.Id = id;
    this.Args = new List<CivLocArg>((IEnumerable<CivLocArg>) args);
  }

  public string Resolve()
  {
    if (this.Args.Count == 0)
      return Loc.GetString(this.Id);
    (string, object)[] valueTupleArray = new (string, object)[this.Args.Count];
    for (int index = 0; index < this.Args.Count; ++index)
      valueTupleArray[index] = (this.Args[index].Name, this.Args[index].ResolveValue());
    return Loc.GetString(this.Id, valueTupleArray);
  }
}
