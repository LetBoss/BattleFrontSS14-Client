using System;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Maths;

namespace Content.Client.Weapons.Ranged.ItemStatus;

public abstract class BaseBulletRenderer : Control
{
	protected struct LayoutParameters
	{
		public int ItemHeight;

		public int ItemSeparation;

		public int ItemWidth;

		public int VerticalSeparation;

		public int MinCountPerRow;
	}

	private int _capacity;

	private LayoutParameters _params;

	public int Rows { get; set; } = 2;

	public int Count { get; set; }

	public int Capacity
	{
		get
		{
			return _capacity;
		}
		set
		{
			if (_capacity != value)
			{
				_capacity = value;
				((Control)this).InvalidateMeasure();
			}
		}
	}

	protected LayoutParameters Parameters
	{
		get
		{
			return _params;
		}
		set
		{
			_params = value;
			((Control)this).InvalidateMeasure();
		}
	}

	protected override Vector2 MeasureOverride(Vector2 availableSize)
	{
		int num = Math.Min(Capacity, CountPerRow(availableSize.X));
		int num2 = Math.Min((int)MathF.Ceiling((float)Capacity / (float)num), Rows);
		int num3 = _params.ItemHeight * num2 + (_params.VerticalSeparation * num2 - 1);
		return new Vector2(RowWidth(num), num3);
	}

	protected override void Draw(DrawingHandleScreen handle)
	{
		Matrix3x2 transform = ((DrawingHandleBase)handle).GetTransform();
		Vector2 vector = new Vector2(((Control)this).UIScale);
		Matrix3x2 matrix3x = Matrix3Helpers.CreateScale(ref vector) * transform;
		((DrawingHandleBase)handle).SetTransform(ref matrix3x);
		int num = CountPerRow(((Control)this).Size.X);
		Vector2 vector2 = default(Vector2);
		int num2 = Capacity - Count;
		int num3 = 0;
		for (int i = 0; i < Rows; i++)
		{
			bool flag = false;
			int num4 = Math.Min(num, Capacity - num3);
			if (num4 > 0)
			{
				int num5 = Capacity - num3 - num4;
				if (num5 < _params.MinCountPerRow && i != Rows - 1 && _params.MinCountPerRow < num && num5 > 0)
				{
					num4 -= _params.MinCountPerRow - num5;
				}
				int num6 = RowWidth(num4);
				vector2.X += ((Control)this).Size.X - (float)num6;
				for (int j = 0; j < num4; j++)
				{
					int num7 = Capacity - num3 - num4 + j;
					Vector2 renderPos = vector2;
					renderPos.Y = ((Control)this).Size.Y - renderPos.Y - (float)_params.ItemHeight;
					DrawItem(handle, renderPos, num7 < num2, flag);
					vector2.X += _params.ItemSeparation;
					flag = !flag;
				}
				num3 += num4;
				vector2.X = 0f;
				vector2.Y += _params.ItemHeight + _params.VerticalSeparation;
				continue;
			}
			break;
		}
	}

	protected abstract void DrawItem(DrawingHandleScreen handle, Vector2 renderPos, bool spent, bool altColor);

	private int CountPerRow(float width)
	{
		return (int)((width - (float)_params.ItemWidth + (float)_params.ItemSeparation) / (float)_params.ItemSeparation);
	}

	private int RowWidth(int count)
	{
		return (count - 1) * _params.ItemSeparation + _params.ItemWidth;
	}
}
