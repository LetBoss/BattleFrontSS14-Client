using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Robust.Shared.Collections;
using Robust.Shared.Maths;

namespace Robust.Shared.Map.Enumerators;

public record struct NearestChunkEnumerator
{
	private readonly Vector2i _chunkLB;

	private readonly Vector2i _chunkRT;

	private ValueList<Vector2i> _chunks;

	private int _n;

	public NearestChunkEnumerator(Box2 localAABB, int chunkSize)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		_n = 0;
		_chunks = default(ValueList<Vector2i>);
		_chunkLB = Vector2Helpers.Floored(localAABB.BottomLeft / chunkSize);
		_chunkRT = Vector2Helpers.Floored(localAABB.TopRight / chunkSize);
		InitializeChunks(new Vector2i(chunkSize, chunkSize));
	}

	public NearestChunkEnumerator(Box2 localAABB, Vector2i chunkSize)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		_n = 0;
		_chunks = default(ValueList<Vector2i>);
		_chunkLB = Vector2Helpers.Floored(localAABB.BottomLeft / Vector2i.op_Implicit(chunkSize));
		_chunkRT = Vector2Helpers.Floored(localAABB.TopRight / Vector2i.op_Implicit(chunkSize));
		InitializeChunks(chunkSize);
	}

	private void InitializeChunks(Vector2i chunkSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = Vector2i.op_Implicit(_chunkLB) * Vector2i.op_Implicit(chunkSize);
		Vector2 vector2 = Vector2i.op_Implicit(_chunkRT) * Vector2i.op_Implicit(chunkSize);
		Vector2 halfChunk = new Vector2((float)chunkSize.X / 2f, (float)chunkSize.Y / 2f);
		Vector2 center = (vector2 - vector) / 2f + vector;
		for (int i = _chunkLB.X; i < _chunkRT.X; i++)
		{
			for (int j = _chunkLB.Y; j < _chunkRT.Y; j++)
			{
				_chunks.Add(new Vector2i(i, j) * chunkSize);
			}
		}
		_chunks.Sort((Vector2i c1, Vector2i c2) => (Vector2i.op_Implicit(c1) + halfChunk - center).LengthSquared().CompareTo((Vector2i.op_Implicit(c2) + halfChunk - center).LengthSquared()));
	}

	public bool MoveNext([NotNullWhen(true)] out Vector2i? indices)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (_n >= _chunks.Count)
		{
			indices = null;
			return false;
		}
		indices = _chunks[_n++];
		return true;
	}
}
