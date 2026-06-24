using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SpaceWizards.Sodium;

namespace Robust.Shared.Utility;

internal sealed class Blake2BHasherStream : Stream
{
	private readonly bool _reader;

	public readonly int OutputLength;

	public readonly Stream WrappingStream;

	public State State;

	public override bool CanRead => _reader;

	public override bool CanSeek => false;

	public override bool CanWrite => !_reader;

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

	private Blake2BHasherStream(Stream wrapping, bool reader, ReadOnlySpan<byte> key, int outputLength)
	{
		OutputLength = outputLength;
		WrappingStream = wrapping;
		_reader = reader;
		CryptoGenericHashBlake2B.Init(ref State, key, outputLength);
	}

	public byte[] Finish()
	{
		byte[] array = new byte[OutputLength];
		CryptoGenericHashBlake2B.Final(ref State, (Span<byte>)array);
		return array;
	}

	public static Blake2BHasherStream CreateReader(Stream wrapping, ReadOnlySpan<byte> key, int outputLength)
	{
		if (!wrapping.CanRead)
		{
			throw new ArgumentException("Must pass readable stream.");
		}
		return new Blake2BHasherStream(wrapping, reader: true, key, outputLength);
	}

	public static Blake2BHasherStream CreateWriter(Stream wrapping, ReadOnlySpan<byte> key, int outputLength)
	{
		if (!wrapping.CanWrite)
		{
			throw new ArgumentException("Must pass writeable stream.");
		}
		return new Blake2BHasherStream(wrapping, reader: false, key, outputLength);
	}

	public override void Flush()
	{
		if (!CanWrite)
		{
			throw new InvalidOperationException();
		}
		WrappingStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (!CanRead)
		{
			throw new InvalidOperationException();
		}
		int num = WrappingStream.Read(buffer, offset, count);
		if (num > 0)
		{
			CryptoGenericHashBlake2B.Update(ref State, (ReadOnlySpan<byte>)buffer.AsSpan(offset, num));
		}
		return num;
	}

	public override int Read(Span<byte> buffer)
	{
		if (!CanRead)
		{
			throw new InvalidOperationException();
		}
		int num = WrappingStream.Read(buffer);
		if (num > 0)
		{
			CryptoGenericHashBlake2B.Update(ref State, (ReadOnlySpan<byte>)buffer.Slice(0, num));
		}
		return num;
	}

	public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (!CanRead)
		{
			throw new InvalidOperationException();
		}
		int num = await WrappingStream.ReadAsync(buffer, cancellationToken);
		if (num > 0)
		{
			CryptoGenericHashBlake2B.Update(ref State, (ReadOnlySpan<byte>)buffer.Slice(0, num).Span);
		}
		return num;
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
		if (!CanWrite)
		{
			throw new InvalidOperationException();
		}
		WrappingStream.Write(buffer, offset, count);
		CryptoGenericHashBlake2B.Update(ref State, (ReadOnlySpan<byte>)buffer.AsSpan(offset, count));
	}

	public override void Write(ReadOnlySpan<byte> buffer)
	{
		if (!CanWrite)
		{
			throw new InvalidOperationException();
		}
		WrappingStream.Write(buffer);
		CryptoGenericHashBlake2B.Update(ref State, buffer);
	}

	public override void WriteByte(byte value)
	{
		Span<byte> span = stackalloc byte[1];
		span[0] = value;
		Write(span);
	}
}
