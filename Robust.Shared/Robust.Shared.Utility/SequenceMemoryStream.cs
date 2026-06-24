using System;
using System.Buffers;
using System.IO;

namespace Robust.Shared.Utility;

internal sealed class SequenceMemoryStream : Stream
{
	private sealed class SequenceSegment : ReadOnlySequenceSegment<byte>
	{
		public readonly byte[] Buffer;

		public SequenceSegment(int segmentLength, long runningIndex)
		{
			Buffer = GC.AllocateUninitializedArray<byte>(segmentLength);
			base.RunningIndex = runningIndex;
			base.Memory = Buffer;
		}

		public void Append(SequenceSegment segment)
		{
			base.Next = segment;
		}
	}

	private readonly int _segmentLength;

	private readonly SequenceSegment _startSegment;

	private SequenceSegment _curSegment;

	private int _curSegmentWritten;

	public ReadOnlySequence<byte> AsSequence => new ReadOnlySequence<byte>(_startSegment, 0, _curSegment, _curSegmentWritten);

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

	public override long Length => _curSegment.RunningIndex + _curSegmentWritten;

	public override long Position
	{
		get
		{
			return Length;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public SequenceMemoryStream(int segmentLength = 524288)
	{
		_segmentLength = segmentLength;
		_startSegment = (_curSegment = new SequenceSegment(segmentLength, 0L));
	}

	public override void Flush()
	{
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

	public override void Write(ReadOnlySpan<byte> buffer)
	{
		Span<byte> destination;
		while (true)
		{
			destination = _curSegment.Buffer.AsSpan(_curSegmentWritten);
			if (destination.Length >= buffer.Length)
			{
				break;
			}
			buffer.Slice(0, destination.Length).CopyTo(destination);
			int length = destination.Length;
			buffer = buffer.Slice(length, buffer.Length - length);
			SequenceSegment sequenceSegment = new SequenceSegment(_segmentLength, _curSegment.RunningIndex + _segmentLength);
			_curSegment.Append(sequenceSegment);
			_curSegment = sequenceSegment;
			_curSegmentWritten = 0;
		}
		buffer.CopyTo(destination);
		_curSegmentWritten += buffer.Length;
	}

	public override void WriteByte(byte value)
	{
		Span<byte> span = stackalloc byte[1];
		span[0] = value;
		Write(span);
	}
}
