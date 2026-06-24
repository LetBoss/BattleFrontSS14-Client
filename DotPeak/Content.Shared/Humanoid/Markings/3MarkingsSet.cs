// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.MarkingsEnumerator
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

public sealed class MarkingsEnumerator : IEnumerator<Marking>, IEnumerator, IDisposable
{
  private List<Marking> _markings;
  private bool _reverse;
  private int position;

  public MarkingsEnumerator(List<Marking> markings, bool reverse)
  {
    this._markings = markings;
    this._reverse = reverse;
    if (this._reverse)
      this.position = this._markings.Count;
    else
      this.position = -1;
  }

  public bool MoveNext()
  {
    if (this._reverse)
    {
      --this.position;
      return this.position >= 0;
    }
    ++this.position;
    return this.position < this._markings.Count;
  }

  public void Reset()
  {
    if (this._reverse)
      this.position = this._markings.Count;
    else
      this.position = -1;
  }

  public void Dispose()
  {
  }

  object IEnumerator.Current => (object) this._markings[this.position];

  public Marking Current => this._markings[this.position];
}
