using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SharpZstd.Interop;

namespace Robust.Shared.Utility;

public sealed class ZStdDecompressStream : Stream
{
	private readonly Stream _baseStream;

	private readonly bool _ownStream;

	private unsafe readonly ZSTD_DCtx* _ctx;

	private readonly byte[] _buffer;

	private int _bufferPos;

	private int _bufferSize;

	private bool _disposed;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

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

	public unsafe ZStdDecompressStream(Stream baseStream, bool ownStream = true)
	{
		_baseStream = baseStream;
		_ownStream = ownStream;
		_ctx = Zstd.ZSTD_createDCtx();
		_buffer = ArrayPool<byte>.Shared.Rent((int)(nuint)Zstd.ZSTD_DStreamInSize());
	}

	protected unsafe override void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}
		_disposed = true;
		Zstd.ZSTD_freeDCtx(_ctx);
		if (disposing)
		{
			if (_ownStream)
			{
				_baseStream.Dispose();
			}
			ArrayPool<byte>.Shared.Return(_buffer);
		}
	}

	public override void Flush()
	{
		ThrowIfDisposed();
		_baseStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return Read(buffer.AsSpan(offset, count));
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

	public unsafe override int Read(Span<byte> buffer)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfDisposed();
		while (true)
		{
			if (_bufferSize == 0 || _bufferPos == _bufferSize)
			{
				_bufferPos = 0;
				_bufferSize = _baseStream.Read(_buffer);
				if (_bufferSize == 0)
				{
					break;
				}
			}
			fixed (byte* buffer2 = _buffer)
			{
				fixed (byte* dst = buffer)
				{
					ZSTD_outBuffer val = new ZSTD_outBuffer
					{
						dst = dst,
						pos = (nuint)0u,
						size = (nuint)buffer.Length
					};
					ZSTD_inBuffer val2 = new ZSTD_inBuffer
					{
						src = buffer2,
						pos = (nuint)_bufferPos,
						size = (nuint)_bufferSize
					};
					UIntPtr code = Zstd.ZSTD_decompressStream(_ctx, &val, &val2);
					_bufferPos = (int)(nuint)val2.pos;
					ZStdException.ThrowIfError(code);
					if (val.pos != (UIntPtr)(nuint)0u)
					{
						return (int)(nuint)val.pos;
					}
				}
			}
		}
		return 0;
	}

	public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfDisposed();
		nuint num;
		do
		{
			if (_bufferSize == 0 || _bufferPos == _bufferSize)
			{
				_bufferPos = 0;
				_bufferSize = await _baseStream.ReadAsync(_buffer, cancellationToken);
				if (_bufferSize == 0)
				{
					return 0;
				}
			}
			num = DecompressChunk(this, buffer.Span);
		}
		while (num == 0);
		return (int)num;
		unsafe static nuint DecompressChunk(ZStdDecompressStream stream, Span<byte> span)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			fixed (byte* buffer2 = stream._buffer)
			{
				fixed (byte* dst = span)
				{
					ZSTD_outBuffer val = new ZSTD_outBuffer
					{
						dst = dst,
						pos = (nuint)0u,
						size = (nuint)span.Length
					};
					ZSTD_inBuffer val2 = new ZSTD_inBuffer
					{
						src = buffer2,
						pos = (nuint)stream._bufferPos,
						size = (nuint)stream._bufferSize
					};
					UIntPtr code = Zstd.ZSTD_decompressStream(stream._ctx, &val, &val2);
					stream._bufferPos = (int)(nuint)val2.pos;
					ZStdException.ThrowIfError(code);
					return val.pos;
				}
			}
		}
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
		throw new NotSupportedException();
	}

	private void ThrowIfDisposed()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ZStdDecompressStream");
		}
	}
}
