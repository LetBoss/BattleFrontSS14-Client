using System;
using System.Buffers;
using System.IO;
using SharpZstd.Interop;

namespace Robust.Shared.Utility;

public sealed class ZStdCompressStream : Stream
{
	private readonly Stream _baseStream;

	private readonly bool _ownStream;

	private readonly byte[] _buffer;

	private int _bufferPos;

	private bool _disposed;

	private bool _hasSession;

	public ZStdCompressionContext Context { get; }

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

	public override long Length
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public override long Position
	{
		get
		{
			throw new NotSupportedException();
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public ZStdCompressStream(Stream baseStream, bool ownStream = true)
	{
		Context = new ZStdCompressionContext();
		_baseStream = baseStream;
		_ownStream = ownStream;
		_buffer = ArrayPool<byte>.Shared.Rent((int)(nuint)Zstd.ZSTD_CStreamOutSize());
	}

	public override void Flush()
	{
		FlushInternal((ZSTD_EndDirective)1);
	}

	public void FlushEnd()
	{
		_hasSession = false;
		FlushInternal((ZSTD_EndDirective)2);
	}

	private unsafe void FlushInternal(ZSTD_EndDirective directive)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		fixed (byte* buffer = _buffer)
		{
			ZSTD_outBuffer val = new ZSTD_outBuffer
			{
				size = (nuint)_buffer.Length,
				pos = (nuint)_bufferPos,
				dst = buffer
			};
			ZSTD_inBuffer val2 = default(ZSTD_inBuffer);
			UIntPtr num;
			do
			{
				num = Zstd.ZSTD_compressStream2(Context.Context, &val, &val2, directive);
				ZStdException.ThrowIfError(num);
				_bufferPos = (int)(nuint)val.pos;
				_baseStream.Write(_buffer.AsSpan(0, (int)(nuint)val.pos));
				_bufferPos = 0;
				val.pos = (nuint)0u;
			}
			while (num != (UIntPtr)(nuint)0u);
		}
		_baseStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
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
		Write(buffer.AsSpan(offset, count));
	}

	public unsafe override void Write(ReadOnlySpan<byte> buffer)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfDisposed();
		_hasSession = true;
		fixed (byte* buffer2 = _buffer)
		{
			fixed (byte* src = buffer)
			{
				ZSTD_outBuffer val = new ZSTD_outBuffer
				{
					size = (nuint)_buffer.Length,
					pos = (nuint)_bufferPos,
					dst = buffer2
				};
				ZSTD_inBuffer val2 = new ZSTD_inBuffer
				{
					pos = (nuint)0u,
					size = (nuint)buffer.Length,
					src = src
				};
				while (true)
				{
					ZStdException.ThrowIfError(Zstd.ZSTD_compressStream2(Context.Context, &val, &val2, (ZSTD_EndDirective)0));
					_bufferPos = (int)(nuint)val.pos;
					if ((nuint)val2.pos >= (nuint)val2.size)
					{
						break;
					}
					_baseStream.Write(_buffer.AsSpan(0, (int)(nuint)val.pos));
					_bufferPos = 0;
					val.pos = (nuint)0u;
				}
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (_disposed)
		{
			return;
		}
		if (disposing)
		{
			if (_hasSession)
			{
				FlushEnd();
			}
			if (_ownStream)
			{
				_baseStream.Dispose();
			}
			ArrayPool<byte>.Shared.Return(_buffer);
			Context.Dispose();
		}
		_disposed = true;
	}

	private void ThrowIfDisposed()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ZStdCompressStream");
		}
	}
}
