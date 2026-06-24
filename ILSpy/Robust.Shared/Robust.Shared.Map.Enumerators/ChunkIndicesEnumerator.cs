using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Robust.Shared.Maths;

namespace Robust.Shared.Map.Enumerators;

public struct ChunkIndicesEnumerator
{
	private readonly Vector2i _chunkLB;

	private readonly Vector2i _chunkRT;

	private int _xIndex;

	private int _yIndex;

	public ChunkIndicesEnumerator(Vector2 viewPos, float range, float chunkSize)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = new Vector2(range, range);
		_chunkLB = Vector2Helpers.Floored((viewPos - vector) / chunkSize);
		_chunkRT = Vector2Helpers.Floored((viewPos + vector) / chunkSize);
		_xIndex = _chunkLB.X;
		_yIndex = _chunkLB.Y;
	}

	public ChunkIndicesEnumerator(Box2 localAABB, int chunkSize)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		_chunkLB = Vector2Helpers.Floored(localAABB.BottomLeft / chunkSize);
		_chunkRT = Vector2Helpers.Floored(localAABB.TopRight / chunkSize);
		_xIndex = _chunkLB.X;
		_yIndex = _chunkLB.Y;
	}

	public bool MoveNext([NotNullWhen(true)] out Vector2i? indices)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (_yIndex > _chunkRT.Y)
		{
			_yIndex = _chunkLB.Y;
			_xIndex++;
		}
		if (_xIndex > _chunkRT.X)
		{
			indices = null;
			return false;
		}
		indices = new Vector2i(_xIndex, _yIndex);
		_yIndex++;
		return true;
	}
}
