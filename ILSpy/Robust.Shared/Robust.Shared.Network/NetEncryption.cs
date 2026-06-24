using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Threading;
using Lidgren.Network;
using SpaceWizards.Sodium;

namespace Robust.Shared.Network;

internal sealed class NetEncryption
{
	private ulong _nonce;

	private readonly byte[] _key;

	public NetEncryption(byte[] key, bool isServer)
	{
		if (key.Length != 32)
		{
			throw new ArgumentException("Key is of wrong size!");
		}
		_nonce = (ulong)((!isServer) ? 1 : 0);
		_key = key;
	}

	public void Encrypt(NetOutgoingMessage message)
	{
		ulong value = Interlocked.Add(ref _nonce, 2uL);
		int lengthBytes = ((NetBuffer)message).LengthBytes;
		int num = 16 + lengthBytes + 8;
		Span<byte> span = ((NetBuffer)message).Data.AsSpan(0, lengthBytes);
		byte[] array = null;
		Span<byte> span2;
		Span<byte> destination;
		if (((NetBuffer)message).Data.Length >= num)
		{
			array = ArrayPool<byte>.Shared.Rent(lengthBytes);
			span2 = array.AsSpan(0, lengthBytes);
			span.CopyTo(span2);
			destination = ((NetBuffer)message).Data.AsSpan(0, num);
		}
		else
		{
			span2 = span;
			byte[] array2 = (((NetBuffer)message).Data = new byte[num]);
			destination = array2;
		}
		Span<byte> span3 = stackalloc byte[24];
		span3.Fill(0);
		BinaryPrimitives.WriteUInt64LittleEndian(span3, value);
		BinaryPrimitives.WriteUInt64LittleEndian(destination, value);
		Unsafe.SkipInit(out int num2);
		CryptoAeadXChaCha20Poly1305Ietf.Encrypt(destination.Slice(8, destination.Length - 8), ref num2, (ReadOnlySpan<byte>)span2, ReadOnlySpan<byte>.Empty, (ReadOnlySpan<byte>)span3, (ReadOnlySpan<byte>)_key);
		((NetBuffer)message).LengthBytes = num;
		if (array != null)
		{
			ArrayPool<byte>.Shared.Return(array);
		}
	}

	public void Decrypt(NetIncomingMessage message)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		ulong value = ((NetBuffer)message).ReadUInt64();
		Span<byte> span = ((NetBuffer)message).Data.AsSpan(8, ((NetBuffer)message).LengthBytes - 8);
		byte[] array = ArrayPool<byte>.Shared.Rent(span.Length);
		span.CopyTo(array);
		Span<byte> span2 = stackalloc byte[24];
		span2.Fill(0);
		BinaryPrimitives.WriteUInt64LittleEndian(span2, value);
		Unsafe.SkipInit(out int lengthBytes);
		bool num = CryptoAeadXChaCha20Poly1305Ietf.Decrypt((Span<byte>)((NetBuffer)message).Data, ref lengthBytes, (ReadOnlySpan<byte>)array.AsSpan(0, span.Length), ReadOnlySpan<byte>.Empty, (ReadOnlySpan<byte>)span2, (ReadOnlySpan<byte>)_key);
		((NetBuffer)message).Position = 0L;
		((NetBuffer)message).LengthBytes = lengthBytes;
		ArrayPool<byte>.Shared.Return(array);
		if (!num)
		{
			throw new SodiumException("Decryption operation failed!");
		}
	}
}
