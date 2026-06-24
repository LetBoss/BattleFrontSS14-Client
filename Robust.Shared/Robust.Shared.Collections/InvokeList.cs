using System;

namespace Robust.Shared.Collections;

internal struct InvokeList<T>
{
	public struct Entry
	{
		public T? Value;

		public object? Equality;
	}

	private Entry[]? _entries;

	public int Count
	{
		get
		{
			Entry[]? entries = _entries;
			if (entries == null)
			{
				return 0;
			}
			return entries.Length;
		}
	}

	public ReadOnlySpan<Entry> Entries => _entries;

	public void AddInPlace(T value, object equality)
	{
		this = Add(value, equality);
	}

	public readonly InvokeList<T> Add(T value, object equality)
	{
		if (_entries == null)
		{
			return new InvokeList<T>
			{
				_entries = new Entry[1]
				{
					new Entry
					{
						Value = value,
						Equality = equality
					}
				}
			};
		}
		Entry[] array = _entries;
		Array.Resize(ref array, array.Length + 1);
		array[^1] = new Entry
		{
			Value = value,
			Equality = equality
		};
		return new InvokeList<T>
		{
			_entries = array
		};
	}

	public void RemoveInPlace(object equality)
	{
		this = Remove(equality);
	}

	public readonly InvokeList<T> Remove(object equality)
	{
		if (_entries == null)
		{
			return this;
		}
		int num = -1;
		for (int i = 0; i < _entries.Length; i++)
		{
			Entry entry = _entries[i];
			if (equality.Equals(entry.Equality))
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return this;
		}
		if (_entries.Length == 1)
		{
			return default(InvokeList<T>);
		}
		Entry[] array = new Entry[_entries.Length - 1];
		int num2 = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (num2 == num)
			{
				num2++;
			}
			array[j] = _entries[num2];
			num2++;
		}
		return new InvokeList<T>
		{
			_entries = array
		};
	}
}
