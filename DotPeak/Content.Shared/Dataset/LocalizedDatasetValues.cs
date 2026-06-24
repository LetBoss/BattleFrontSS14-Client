// Decompiled with JetBrains decompiler
// Type: Content.Shared.Dataset.LocalizedDatasetValues
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Dataset;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class LocalizedDatasetValues : 
  IReadOnlyList<string>,
  IEnumerable<string>,
  IEnumerable,
  IReadOnlyCollection<string>,
  ISerializationGenerated<LocalizedDatasetValues>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Prefix { get; private set; }

  [DataField(null, false, 1, true, false, null)]
  public int Count { get; private set; }

  public string this[int index]
  {
    get
    {
      if (index >= this.Count || index < 0)
        throw new IndexOutOfRangeException();
      return this.Prefix + (index + 1).ToString();
    }
  }

  public IEnumerator<string> GetEnumerator()
  {
    return (IEnumerator<string>) new LocalizedDatasetValues.Enumerator(this);
  }

  IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LocalizedDatasetValues target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<LocalizedDatasetValues>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Prefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Prefix, ref str, hookCtx, false, context))
      str = this.Prefix;
    target.Prefix = str;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref num, hookCtx, false, context))
      num = this.Count;
    target.Count = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LocalizedDatasetValues target,
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
    LocalizedDatasetValues target1 = (LocalizedDatasetValues) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public LocalizedDatasetValues Instantiate() => new LocalizedDatasetValues();

  public sealed class Enumerator : IEnumerator<string>, IEnumerator, IDisposable
  {
    private int _index;
    private readonly LocalizedDatasetValues _values;

    public Enumerator(LocalizedDatasetValues values) => this._values = values;

    public string Current => this._values.Prefix + this._index.ToString();

    object IEnumerator.Current => (object) this.Current;

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
      ++this._index;
      return this._index <= this._values.Count;
    }

    public void Reset() => this._index = 0;
  }
}
