// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetEncryption
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using SpaceWizards.Sodium;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading;

#nullable enable
namespace Robust.Shared.Network;

internal sealed class NetEncryption
{
  private ulong _nonce;
  private readonly byte[] _key;

  public NetEncryption(byte[] key, bool isServer)
  {
    if (key.Length != 32 /*0x20*/)
      throw new ArgumentException("Key is of wrong size!");
    this._nonce = (ulong) !isServer;
    this._key = key;
  }

  public void Encrypt(NetOutgoingMessage message)
  {
    ulong num1 = Interlocked.Add(ref this._nonce, 2UL);
    int lengthBytes = ((NetBuffer) message).LengthBytes;
    int length = 16 /*0x10*/ + lengthBytes + 8;
    Span<byte> span = ((NetBuffer) message).Data.AsSpan<byte>(0, lengthBytes);
    byte[] array = (byte[]) null;
    Span<byte> destination1;
    Span<byte> destination2;
    if (((NetBuffer) message).Data.Length >= length)
    {
      array = ArrayPool<byte>.Shared.Rent(lengthBytes);
      destination1 = array.AsSpan<byte>(0, lengthBytes);
      span.CopyTo(destination1);
      destination2 = ((NetBuffer) message).Data.AsSpan<byte>(0, length);
    }
    else
    {
      destination1 = span;
      destination2 = (Span<byte>) (((NetBuffer) message).Data = new byte[length]);
    }
    Span<byte> destination3 = stackalloc byte[24];
    destination3.Fill((byte) 0);
    BinaryPrimitives.WriteUInt64LittleEndian(destination3, num1);
    BinaryPrimitives.WriteUInt64LittleEndian(destination2, num1);
    ref Span<byte> local = ref destination2;
    int num2;
    CryptoAeadXChaCha20Poly1305Ietf.Encrypt(local.Slice(8, local.Length - 8), ref num2, (ReadOnlySpan<byte>) destination1, ReadOnlySpan<byte>.Empty, (ReadOnlySpan<byte>) destination3, (ReadOnlySpan<byte>) this._key);
    ((NetBuffer) message).LengthBytes = length;
    if (array == null)
      return;
    ArrayPool<byte>.Shared.Return(array);
  }

  public void Decrypt(NetIncomingMessage message)
  {
    ulong num1 = ((NetBuffer) message).ReadUInt64();
    Span<byte> span = ((NetBuffer) message).Data.AsSpan<byte>(8, ((NetBuffer) message).LengthBytes - 8);
    byte[] numArray = ArrayPool<byte>.Shared.Rent(span.Length);
    span.CopyTo((Span<byte>) numArray);
    Span<byte> destination = stackalloc byte[24];
    destination.Fill((byte) 0);
    BinaryPrimitives.WriteUInt64LittleEndian(destination, num1);
    int num2;
    int num3 = CryptoAeadXChaCha20Poly1305Ietf.Decrypt((Span<byte>) ((NetBuffer) message).Data, ref num2, (ReadOnlySpan<byte>) numArray.AsSpan<byte>(0, span.Length), ReadOnlySpan<byte>.Empty, (ReadOnlySpan<byte>) destination, (ReadOnlySpan<byte>) this._key) ? 1 : 0;
    ((NetBuffer) message).Position = 0L;
    ((NetBuffer) message).LengthBytes = num2;
    ArrayPool<byte>.Shared.Return(numArray);
    if (num3 == 0)
      throw new SodiumException("Decryption operation failed!");
  }
}
