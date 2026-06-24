using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Robust.Shared.Utility;

internal sealed class HasherStream : Stream
{
	private readonly Stream _wrapping;

	private readonly IncrementalHash _hash;

	private readonly bool _leaveOpen;

	public override bool CanRead => _wrapping.CanRead;

	public override bool CanSeek => false;

	public override bool CanWrite => _wrapping.CanWrite;

	public override long Length => _wrapping.Length;

	public override long Position
	{
		get
		{
			return _wrapping.Position;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public HasherStream(Stream wrapping, IncrementalHash hash, bool leaveOpen = false)
	{
		_wrapping = wrapping;
		_hash = hash;
		_leaveOpen = leaveOpen;
	}

	protected override void Dispose(bool disposing)
	{
		if (!_leaveOpen)
		{
			_wrapping.Dispose();
		}
	}

	public override ValueTask DisposeAsync()
	{
		if (!_leaveOpen)
		{
			return _wrapping.DisposeAsync();
		}
		return default(ValueTask);
	}

	public override void Flush()
	{
		_wrapping.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = _wrapping.Read(buffer, offset, count);
		if (num > 0)
		{
			_hash.AppendData(buffer, offset, num);
		}
		return num;
	}

	public override int Read(Span<byte> buffer)
	{
		int num = _wrapping.Read(buffer);
		if (num > 0)
		{
			_hash.AppendData(buffer.Slice(0, num));
		}
		return num;
	}

	public override int ReadByte()
	{
		Span<byte> buffer = stackalloc byte[1];
		if (Read(buffer) != 0)
		{
			return buffer[0];
		}
		return -1;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		_hash.AppendData(buffer, offset, count);
		_wrapping.Write(buffer, offset, count);
	}

	public override void Write(ReadOnlySpan<byte> buffer)
	{
		_hash.AppendData(buffer);
		_wrapping.Write(buffer);
	}

	public override void WriteByte(byte value)
	{
		Span<byte> span = stackalloc byte[1] { value };
		Write(span);
	}
}
