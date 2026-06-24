using System;
using System.IO;
using System.Text;

namespace Content.Client.Instruments.MidiParser;

public sealed class MidiStreamWrapper
{
	private readonly MemoryStream _stream;

	private byte[] _buffer;

	public long StreamPosition => _stream.Position;

	public MidiStreamWrapper(byte[] data)
	{
		_stream = new MemoryStream(data, writable: false);
		_buffer = new byte[4];
	}

	public void Skip(int count)
	{
		if (count != 0)
		{
			_stream.Seek(count, SeekOrigin.Current);
		}
	}

	public byte ReadByte()
	{
		int num = _stream.ReadByte();
		if (num == -1)
		{
			throw new Exception("Unexpected end of stream");
		}
		return (byte)num;
	}

	public byte[] ReadBytes(int count)
	{
		if (_buffer.Length < count)
		{
			Array.Resize(ref _buffer, count);
		}
		if (_stream.Read(_buffer, 0, count) != count)
		{
			throw new Exception("Unexpected end of stream");
		}
		return _buffer;
	}

	public uint ReadUInt32()
	{
		byte[] array = ReadBytes(4);
		return (uint)((array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3]);
	}

	public ushort ReadUInt16()
	{
		byte[] array = ReadBytes(2);
		return (ushort)((array[0] << 8) | array[1]);
	}

	public string ReadString(int count)
	{
		byte[] bytes = ReadBytes(count);
		return Encoding.UTF8.GetString(bytes, 0, count);
	}

	public uint ReadVariableLengthQuantity()
	{
		uint num = 0u;
		byte b;
		do
		{
			b = ReadByte();
			num = (num << 7) | (uint)(b & 0x7F);
		}
		while ((b & 0x80) != 0);
		return num;
	}
}
