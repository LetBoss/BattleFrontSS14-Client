// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.MemoryContentRoot
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

#nullable enable
namespace Robust.Shared.ContentPack;

public sealed class MemoryContentRoot : IContentRoot, IDisposable
{
  private readonly Dictionary<ResPath, byte[]> _files = new Dictionary<ResPath, byte[]>();
  private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

  public void AddOrUpdateFile(ResPath relPath, byte[] data)
  {
    relPath = relPath.Clean().ToRelativePath();
    this._lock.EnterWriteLock();
    try
    {
      this._files[relPath] = data;
    }
    finally
    {
      this._lock.ExitWriteLock();
    }
  }

  public bool RemoveFile(ResPath relPath)
  {
    this._lock.EnterWriteLock();
    try
    {
      return this._files.Remove(relPath);
    }
    finally
    {
      this._lock.ExitWriteLock();
    }
  }

  public void Clear()
  {
    this._lock.EnterWriteLock();
    try
    {
      this._files.Clear();
    }
    finally
    {
      this._lock.ExitWriteLock();
    }
  }

  public bool FileExists(ResPath relPath)
  {
    this._lock.EnterReadLock();
    try
    {
      return this._files.ContainsKey(relPath);
    }
    finally
    {
      this._lock.ExitReadLock();
    }
  }

  public bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream)
  {
    this._lock.EnterReadLock();
    try
    {
      byte[] buffer;
      if (!this._files.TryGetValue(relPath, out buffer))
      {
        stream = (Stream) null;
        return false;
      }
      stream = (Stream) new MemoryStream(buffer, false);
      return true;
    }
    finally
    {
      this._lock.ExitReadLock();
    }
  }

  public IEnumerable<ResPath> FindFiles(ResPath path)
  {
    this._lock.EnterReadLock();
    try
    {
      Dictionary<ResPath, byte[]>.Enumerator enumerator = this._files.GetEnumerator();
      while (enumerator.MoveNext())
      {
        (ResPath key, byte[] _) = enumerator.Current;
        if (key.TryRelativeTo(path, out ResPath? _))
          yield return key;
      }
      enumerator = new Dictionary<ResPath, byte[]>.Enumerator();
    }
    finally
    {
      this._lock.ExitReadLock();
    }
  }

  public IEnumerable<string> GetRelativeFilePaths()
  {
    this._lock.EnterReadLock();
    try
    {
      Dictionary<ResPath, byte[]>.Enumerator enumerator = this._files.GetEnumerator();
      while (enumerator.MoveNext())
      {
        (ResPath key, byte[] _) = enumerator.Current;
        yield return key.ToString();
      }
      enumerator = new Dictionary<ResPath, byte[]>.Enumerator();
    }
    finally
    {
      this._lock.ExitReadLock();
    }
  }

  public IEnumerable<(ResPath relPath, byte[] data)> GetAllFiles()
  {
    this._lock.EnterReadLock();
    try
    {
      Dictionary<ResPath, byte[]>.Enumerator enumerator = this._files.GetEnumerator();
      while (enumerator.MoveNext())
      {
        (ResPath key, byte[] numArray) = enumerator.Current;
        yield return (key, numArray);
      }
      enumerator = new Dictionary<ResPath, byte[]>.Enumerator();
    }
    finally
    {
      this._lock.ExitReadLock();
    }
  }

  public void Mount()
  {
  }

  public void Dispose() => this._lock.Dispose();
}
