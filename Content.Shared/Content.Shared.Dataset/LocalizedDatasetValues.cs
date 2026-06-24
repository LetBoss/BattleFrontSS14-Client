using System;
using System.Collections;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Dataset;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class LocalizedDatasetValues : IReadOnlyList<string>, IEnumerable<string>, IEnumerable, IReadOnlyCollection<string>, ISerializationGenerated<LocalizedDatasetValues>, ISerializationGenerated
{
	public sealed class Enumerator : IEnumerator<string>, IEnumerator, IDisposable
	{
		private int _index;

		private readonly LocalizedDatasetValues _values;

		public string Current => _values.Prefix + _index;

		object IEnumerator.Current => Current;

		public Enumerator(LocalizedDatasetValues values)
		{
			_values = values;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			_index++;
			return _index <= _values.Count;
		}

		public void Reset()
		{
			_index = 0;
		}
	}

	[DataField(null, false, 1, true, false, null)]
	public string Prefix { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public int Count { get; private set; }

	public string this[int index]
	{
		get
		{
			if (index >= Count || index < 0)
			{
				throw new IndexOutOfRangeException();
			}
			return Prefix + (index + 1);
		}
	}

	public IEnumerator<string> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LocalizedDatasetValues target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<LocalizedDatasetValues>(this, ref target, hookCtx, false, context))
		{
			string PrefixTemp = null;
			if (Prefix == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Prefix, ref PrefixTemp, hookCtx, false, context))
			{
				PrefixTemp = Prefix;
			}
			target.Prefix = PrefixTemp;
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LocalizedDatasetValues target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LocalizedDatasetValues cast = (LocalizedDatasetValues)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public LocalizedDatasetValues Instantiate()
	{
		return new LocalizedDatasetValues();
	}
}
