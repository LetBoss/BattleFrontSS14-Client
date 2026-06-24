using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

public sealed class MemoryContentRoot : IContentRoot, IDisposable
{
	private readonly Dictionary<ResPath, byte[]> _files = new Dictionary<ResPath, byte[]>();

	private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

	public void AddOrUpdateFile(ResPath relPath, byte[] data)
	{
		relPath = relPath.Clean().ToRelativePath();
		_lock.EnterWriteLock();
		try
		{
			_files[relPath] = data;
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public bool RemoveFile(ResPath relPath)
	{
		_lock.EnterWriteLock();
		try
		{
			return _files.Remove(relPath);
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public void Clear()
	{
		_lock.EnterWriteLock();
		try
		{
			_files.Clear();
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public bool FileExists(ResPath relPath)
	{
		_lock.EnterReadLock();
		try
		{
			return _files.ContainsKey(relPath);
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public bool TryGetFile(ResPath relPath, [NotNullWhen(true)] out Stream? stream)
	{
		_lock.EnterReadLock();
		try
		{
			if (!_files.TryGetValue(relPath, out byte[] value))
			{
				stream = null;
				return false;
			}
			stream = new MemoryStream(value, writable: false);
			return true;
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public IEnumerable<ResPath> FindFiles(ResPath path)
	{
		_lock.EnterReadLock();
		try
		{
			foreach (var (resPath2, _) in _files)
			{
				if (resPath2.TryRelativeTo(path, out var _))
				{
					yield return resPath2;
				}
			}
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public IEnumerable<string> GetRelativeFilePaths()
	{
		_lock.EnterReadLock();
		try
		{
			foreach (var (resPath2, _) in _files)
			{
				yield return resPath2.ToString();
			}
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public IEnumerable<(ResPath relPath, byte[] data)> GetAllFiles()
	{
		_lock.EnterReadLock();
		try
		{
			foreach (var (item, item2) in _files)
			{
				yield return (relPath: item, data: item2);
			}
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public void Mount()
	{
	}

	public void Dispose()
	{
		_lock.Dispose();
	}
}
