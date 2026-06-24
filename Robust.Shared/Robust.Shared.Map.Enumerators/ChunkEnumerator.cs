using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Robust.Shared.Map.Enumerators;

internal ref struct ChunkEnumerator
{
	private Dictionary<Vector2i, MapChunk> _chunks;

	private Vector2i _chunkLB;

	private Vector2i _chunkRT;

	private int _xIndex;

	private int _yIndex;

	internal ChunkEnumerator(Dictionary<Vector2i, MapChunk> chunks, Box2 localAABB, int chunkSize)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		_chunks = chunks;
		_chunkLB = new Vector2i((int)Math.Floor(localAABB.Left / (float)chunkSize), (int)Math.Floor(localAABB.Bottom / (float)chunkSize));
		_chunkRT = new Vector2i((int)Math.Floor(localAABB.Right / (float)chunkSize), (int)Math.Floor(localAABB.Top / (float)chunkSize));
		_xIndex = _chunkLB.X;
		_yIndex = _chunkLB.Y;
	}

	public bool MoveNext([NotNullWhen(true)] out MapChunk? chunk)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (_yIndex > _chunkRT.Y)
		{
			_yIndex = _chunkLB.Y;
			_xIndex++;
		}
		Unsafe.SkipInit(out Vector2i key);
		for (int i = _xIndex; i <= _chunkRT.X; i++)
		{
			for (int j = _yIndex; j <= _chunkRT.Y; j++)
			{
				((Vector2i)(ref key))._002Ector(i, j);
				if (_chunks.TryGetValue(key, out chunk))
				{
					_xIndex = i;
					_yIndex = j + 1;
					return true;
				}
			}
			_yIndex = _chunkLB.Y;
		}
		chunk = null;
		return false;
	}
}
