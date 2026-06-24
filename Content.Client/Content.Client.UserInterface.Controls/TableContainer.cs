using System;
using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls;

[Virtual]
public class TableContainer : Container
{
	private struct ColumnData
	{
		public float MaxWidth;

		public float MinWidth;

		public float Slack;

		public float AssignedWidth;

		public float ArrangedWidth;

		public float ArrangedX;
	}

	private struct RowData
	{
		public float MeasuredHeight;
	}

	private int _columns = 1;

	private ColumnData[] _columnDataCache = Array.Empty<ColumnData>();

	private RowData[] _rowDataCache = Array.Empty<RowData>();

	public float MinForcedColumnWidth { get; set; } = 50f;

	public int Columns
	{
		get
		{
			return _columns;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(value, 1, "value");
			_columns = value;
		}
	}

	protected unsafe override Vector2 MeasureOverride(Vector2 availableSize)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		ResetCachedArrays();
		int num = 0;
		Enumerator enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current = ((Enumerator)(ref enumerator)).Current;
				ref ColumnData reference = ref _columnDataCache[num];
				current.Measure(new Vector2(float.PositiveInfinity, float.PositiveInfinity));
				reference.MaxWidth = Math.Max(reference.MaxWidth, current.DesiredSize.X);
				num++;
				if (num == _columns)
				{
					num = 0;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < _columns; i++)
		{
			ref ColumnData reference2 = ref _columnDataCache[i];
			reference2.MinWidth = Math.Min(reference2.MaxWidth, MinForcedColumnWidth);
			reference2.Slack = reference2.MaxWidth - reference2.MinWidth;
			num2 += reference2.MinWidth;
			num3 += reference2.MaxWidth;
			num4 += reference2.Slack;
		}
		if (num3 <= availableSize.X)
		{
			for (int j = 0; j < _columns; j++)
			{
				ref ColumnData reference3 = ref _columnDataCache[j];
				reference3.AssignedWidth = reference3.MaxWidth;
			}
		}
		else
		{
			float num5 = Math.Max(0f, availableSize.X - num2);
			for (int k = 0; k < _columns; k++)
			{
				ref ColumnData reference4 = ref _columnDataCache[k];
				float num6 = reference4.Slack / num4;
				reference4.AssignedWidth = reference4.MinWidth + num6 * num5;
			}
		}
		num = 0;
		int num7 = 0;
		enumerator = ((Control)this).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				Control current2 = ((Enumerator)(ref enumerator)).Current;
				ref ColumnData reference5 = ref _columnDataCache[num];
				ref RowData reference6 = ref _rowDataCache[num7];
				current2.Measure(new Vector2(reference5.AssignedWidth, float.PositiveInfinity));
				reference6.MeasuredHeight = Math.Max(reference6.MeasuredHeight, current2.DesiredSize.Y);
				num++;
				if (num == _columns)
				{
					num = 0;
					num7++;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
		float num8 = 0f;
		for (int l = 0; l < _rowDataCache.Length; l++)
		{
			num8 += _rowDataCache[l].MeasuredHeight;
		}
		return new Vector2(Math.Min(availableSize.X, num3), num8);
	}

	protected override Vector2 ArrangeOverride(Vector2 finalSize)
	{
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < _columns; i++)
		{
			ref ColumnData reference = ref _columnDataCache[i];
			num += reference.MinWidth;
			num2 += reference.Slack;
		}
		float num3 = Math.Max(0f, finalSize.X - num);
		float num4 = 0f;
		for (int j = 0; j < _columns; j++)
		{
			ref ColumnData reference2 = ref _columnDataCache[j];
			float num5 = reference2.Slack / num2;
			reference2.ArrangedWidth = reference2.MinWidth + num5 * num3;
			reference2.ArrangedX = num4;
			num4 += reference2.ArrangedWidth;
		}
		float num6 = 0f;
		for (int k = 0; k < _rowDataCache.Length; k++)
		{
			ref RowData reference3 = ref _rowDataCache[k];
			for (int l = 0; l < _columns; l++)
			{
				ref ColumnData reference4 = ref _columnDataCache[l];
				if (l + k * _columns >= ((Control)this).ChildCount)
				{
					break;
				}
				((Control)this).GetChild(l + k * _columns).Arrange(UIBox2.FromDimensions(reference4.ArrangedX, num6, reference4.ArrangedWidth, reference3.MeasuredHeight));
			}
			num6 += reference3.MeasuredHeight;
		}
		Vector2 result = finalSize;
		result.Y = num6;
		return result;
	}

	private void ResetCachedArrays()
	{
		if (_columnDataCache.Length != _columns)
		{
			_columnDataCache = new ColumnData[_columns];
		}
		Array.Clear(_columnDataCache, 0, _columnDataCache.Length);
		int num = ((Control)this).ChildCount / _columns;
		if (((Control)this).ChildCount % _columns != 0)
		{
			num++;
		}
		if (num != _rowDataCache.Length)
		{
			_rowDataCache = new RowData[num];
		}
		Array.Clear(_rowDataCache, 0, _rowDataCache.Length);
	}
}
