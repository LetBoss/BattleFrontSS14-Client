// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.ForwardMarkingEnumerator
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

public sealed class ForwardMarkingEnumerator : IEnumerable<Marking>, IEnumerable
{
  private List<Marking> _markings;

  public ForwardMarkingEnumerator(List<Marking> markings) => this._markings = markings;

  public IEnumerator<Marking> GetEnumerator()
  {
    return (IEnumerator<Marking>) new MarkingsEnumerator(this._markings, false);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
}
