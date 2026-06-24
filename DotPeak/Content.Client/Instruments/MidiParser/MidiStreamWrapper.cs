// Decompiled with JetBrains decompiler
// Type: Content.Client.Instruments.MidiParser.MidiStreamWrapper
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System;
using System.IO;
using System.Text;

#nullable enable
namespace Content.Client.Instruments.MidiParser;

public sealed class MidiStreamWrapper
{
  private readonly MemoryStream _stream;
  private byte[] _buffer;

  public long StreamPosition => this._stream.Position;

  public MidiStreamWrapper(byte[] data)
  {
    this._stream = new MemoryStream(data, false);
    this._buffer = new byte[4];
  }

  public void Skip(int count)
  {
    if (count == 0)
      return;
    this._stream.Seek((long) count, SeekOrigin.Current);
  }

  public byte ReadByte()
  {
    int num = this._stream.ReadByte();
    return num != -1 ? (byte) num : throw new Exception("Unexpected end of stream");
  }

  public byte[] ReadBytes(int count)
  {
    if (this._buffer.Length < count)
      Array.Resize<byte>(ref this._buffer, count);
    if (this._stream.Read(this._buffer, 0, count) != count)
      throw new Exception("Unexpected end of stream");
    return this._buffer;
  }

  public uint ReadUInt32()
  {
    byte[] numArray = this.ReadBytes(4);
    return (uint) ((int) numArray[0] << 24 | (int) numArray[1] << 16 /*0x10*/ | (int) numArray[2] << 8) | (uint) numArray[3];
  }

  public ushort ReadUInt16()
  {
    byte[] numArray = this.ReadBytes(2);
    return (ushort) ((uint) numArray[0] << 8 | (uint) numArray[1]);
  }

  public string ReadString(int count) => Encoding.UTF8.GetString(this.ReadBytes(count), 0, count);

  public uint ReadVariableLengthQuantity()
  {
    uint num1 = 0;
    byte num2;
    do
    {
      num2 = this.ReadByte();
      num1 = (uint) ((int) num1 << 7 | (int) num2 & (int) sbyte.MaxValue);
    }
    while (((int) num2 & 128 /*0x80*/) != 0);
    return num1;
  }
}
