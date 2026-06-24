using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Robust.Shared.Profiling;

public sealed class ProfManager
{
	public readonly struct GroupGuard(ProfManager mgr, long startIndex, string groupName) : IDisposable
	{
		private readonly ProfManager _mgr = mgr;

		private readonly long _startIndex = startIndex;

		private readonly string _groupName = groupName;

		private readonly ProfSampler _sampler = ProfSampler.StartNew();

		public void Dispose()
		{
			_mgr.WriteGroupEnd(_startIndex, _groupName, in _sampler);
		}
	}

	public readonly struct ValueGuard(ProfManager mgr, string text) : IDisposable
	{
		private readonly ProfManager _mgr = mgr;

		private readonly string _text = text;

		private readonly ProfSampler _sampler = ProfSampler.StartNew();

		public void Dispose()
		{
			_mgr.WriteValue(_text, in _sampler);
		}
	}

	[Robust.Shared.IoC.Dependency]
	private readonly IConfigurationManager _cfg;

	[Robust.Shared.IoC.Dependency]
	private readonly ILogManager _logManager;

	private readonly Dictionary<string, int> _stringTreeIndices = new Dictionary<string, int>();

	private ValueList<string> _stringTree;

	public ProfBuffer Buffer;

	private ISawmill? _sawmill;

	public bool IsEnabled { get; private set; }

	internal void Initialize()
	{
		_sawmill = _logManager.GetSawmill("prof");
		_cfg.OnValueChanged(CVars.ProfIndexSize, delegate(int i)
		{
			if (!BitOperations.IsPow2(i))
			{
				_sawmill.Warning("Rounding prof.index_size to next POT");
				i = BufferHelpers.FittingPowerOfTwo(i);
			}
			Buffer.IndexBuffer = new ProfIndex[i];
			Buffer.IndexWriteOffset = 0L;
		}, invokeImmediately: true);
		_cfg.OnValueChanged(CVars.ProfBufferSize, delegate(int i)
		{
			if (!BitOperations.IsPow2(i))
			{
				_sawmill.Warning("Rounding prof.buffer_size to next POT");
				i = BufferHelpers.FittingPowerOfTwo(i);
			}
			Buffer.LogBuffer = new ProfLog[i];
			Buffer.LogWriteOffset += i;
		}, invokeImmediately: true);
		_cfg.OnValueChanged(CVars.ProfEnabled, delegate(bool b)
		{
			IsEnabled = b;
		}, invokeImmediately: true);
	}

	public void MarkIndex(long start, ProfIndexType type)
	{
		if (IsEnabled)
		{
			long idx = Buffer.IndexWriteOffset++;
			ProfIndex profIndex = new ProfIndex
			{
				StartPos = start,
				EndPos = Buffer.LogWriteOffset,
				Type = type
			};
			Buffer.Index(idx) = profIndex;
		}
	}

	public long WriteValue(string text, in ProfValue value)
	{
		if (!IsEnabled)
		{
			return 0L;
		}
		int stringId = InsertString(text);
		long logWriteOffset = Buffer.LogWriteOffset;
		ref ProfLog reference = ref WriteCmd();
		reference = default(ProfLog);
		reference.Type = ProfLogType.Value;
		reference.Value.Value = value;
		reference.Value.StringId = stringId;
		return logWriteOffset;
	}

	public long WriteValue(string text, in ProfSampler sampler)
	{
		return WriteValue(text, ProfData.TimeAlloc(in sampler));
	}

	public long WriteValue(string text, int int32)
	{
		return WriteValue(text, ProfData.Int32(int32));
	}

	public long WriteValue(string text, long int64)
	{
		return WriteValue(text, ProfData.Int64(int64));
	}

	public ValueGuard Value(string text)
	{
		return new ValueGuard(this, text);
	}

	public long WriteGroupStart()
	{
		if (!IsEnabled)
		{
			return 0L;
		}
		long logWriteOffset = Buffer.LogWriteOffset;
		ref ProfLog reference = ref WriteCmd();
		reference = default(ProfLog);
		reference.Type = ProfLogType.GroupStart;
		return logWriteOffset;
	}

	public void WriteGroupEnd(long startIndex, string text, in ProfValue value)
	{
		if (IsEnabled)
		{
			int stringId = InsertString(text);
			ref ProfLog reference = ref WriteCmd();
			reference = default(ProfLog);
			reference.Type = ProfLogType.GroupEnd;
			reference.GroupEnd.StringId = stringId;
			reference.GroupEnd.StartIndex = startIndex;
			reference.GroupEnd.Value = value;
		}
	}

	public void WriteGroupEnd(long startIndex, string text, in ProfSampler sampler)
	{
		WriteGroupEnd(startIndex, text, ProfData.TimeAlloc(in sampler));
	}

	public GroupGuard Group(string name)
	{
		long startIndex = WriteGroupStart();
		return new GroupGuard(this, startIndex, name);
	}

	public string GetString(int stringId)
	{
		return _stringTree[stringId];
	}

	public int? GetStringIdx(string stringValue)
	{
		if (_stringTreeIndices.TryGetValue(stringValue, out var value))
		{
			return value;
		}
		return null;
	}

	private int InsertString(string text)
	{
		bool exists;
		ref int valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(_stringTreeIndices, text, out exists);
		if (!exists)
		{
			valueRefOrAddDefault = _stringTree.Count;
			_stringTree.Add(text);
		}
		return valueRefOrAddDefault;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ref ProfLog WriteCmd()
	{
		ProfLog[] logBuffer = Buffer.LogBuffer;
		long num = Buffer.LogWriteOffset & (logBuffer.LongLength - 1);
		Buffer.LogWriteOffset++;
		return ref logBuffer[(int)num];
	}
}
