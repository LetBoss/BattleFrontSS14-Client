// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Profiling.ProfManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.Log;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Profiling;

public sealed class ProfManager
{
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
    this._sawmill = this._logManager.GetSawmill("prof");
    this._cfg.OnValueChanged<int>(CVars.ProfIndexSize, (Action<int>) (i =>
    {
      if (!BitOperations.IsPow2(i))
      {
        this._sawmill.Warning("Rounding prof.index_size to next POT");
        i = BufferHelpers.FittingPowerOfTwo(i);
      }
      this.Buffer.IndexBuffer = new ProfIndex[i];
      this.Buffer.IndexWriteOffset = 0L;
    }), true);
    this._cfg.OnValueChanged<int>(CVars.ProfBufferSize, (Action<int>) (i =>
    {
      if (!BitOperations.IsPow2(i))
      {
        this._sawmill.Warning("Rounding prof.buffer_size to next POT");
        i = BufferHelpers.FittingPowerOfTwo(i);
      }
      this.Buffer.LogBuffer = new ProfLog[i];
      this.Buffer.LogWriteOffset += (long) i;
    }), true);
    this._cfg.OnValueChanged<bool>(CVars.ProfEnabled, (Action<bool>) (b => this.IsEnabled = b), true);
  }

  public void MarkIndex(long start, ProfIndexType type)
  {
    if (!this.IsEnabled)
      return;
    this.Buffer.Index(this.Buffer.IndexWriteOffset++) = new ProfIndex()
    {
      StartPos = start,
      EndPos = this.Buffer.LogWriteOffset,
      Type = type
    };
  }

  public unsafe long WriteValue(string text, in ProfValue value)
  {
    if (!this.IsEnabled)
      return 0;
    int num = this.InsertString(text);
    long logWriteOffset = this.Buffer.LogWriteOffset;
    ref ProfLog local = ref this.WriteCmd();
    *(ProfLog*) ref local = new ProfLog();
    local.Type = ProfLogType.Value;
    local.Value.Value = value;
    local.Value.StringId = num;
    return logWriteOffset;
  }

  public long WriteValue(string text, in ProfSampler sampler)
  {
    return this.WriteValue(text, ProfData.TimeAlloc(in sampler));
  }

  public long WriteValue(string text, int int32) => this.WriteValue(text, ProfData.Int32(int32));

  public long WriteValue(string text, long int64) => this.WriteValue(text, ProfData.Int64(int64));

  public ProfManager.ValueGuard Value(string text) => new ProfManager.ValueGuard(this, text);

  public unsafe long WriteGroupStart()
  {
    if (!this.IsEnabled)
      return 0;
    long logWriteOffset = this.Buffer.LogWriteOffset;
    ref ProfLog local = ref this.WriteCmd();
    *(ProfLog*) ref local = new ProfLog();
    local.Type = ProfLogType.GroupStart;
    return logWriteOffset;
  }

  public unsafe void WriteGroupEnd(long startIndex, string text, in ProfValue value)
  {
    if (!this.IsEnabled)
      return;
    int num = this.InsertString(text);
    ref ProfLog local = ref this.WriteCmd();
    *(ProfLog*) ref local = new ProfLog();
    local.Type = ProfLogType.GroupEnd;
    local.GroupEnd.StringId = num;
    local.GroupEnd.StartIndex = startIndex;
    local.GroupEnd.Value = value;
  }

  public void WriteGroupEnd(long startIndex, string text, in ProfSampler sampler)
  {
    this.WriteGroupEnd(startIndex, text, ProfData.TimeAlloc(in sampler));
  }

  public ProfManager.GroupGuard Group(string name)
  {
    return new ProfManager.GroupGuard(this, this.WriteGroupStart(), name);
  }

  public string GetString(int stringId) => this._stringTree[stringId];

  public int? GetStringIdx(string stringValue)
  {
    int num;
    return this._stringTreeIndices.TryGetValue(stringValue, out num) ? new int?(num) : new int?();
  }

  private int InsertString(string text)
  {
    bool exists;
    ref int local = ref CollectionsMarshal.GetValueRefOrAddDefault<string, int>(this._stringTreeIndices, text, out exists);
    if (!exists)
    {
      local = this._stringTree.Count;
      this._stringTree.Add(text);
    }
    return local;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private ref ProfLog WriteCmd()
  {
    ProfLog[] logBuffer = this.Buffer.LogBuffer;
    long index = this.Buffer.LogWriteOffset & (long) logBuffer.Length - 1L;
    ++this.Buffer.LogWriteOffset;
    return ref logBuffer[(int) index];
  }

  public readonly struct GroupGuard(ProfManager mgr, long startIndex, string groupName) : IDisposable
  {
    private readonly ProfManager _mgr = mgr;
    private readonly long _startIndex = startIndex;
    private readonly string _groupName = groupName;
    private readonly ProfSampler _sampler = ProfSampler.StartNew();

    public void Dispose()
    {
      this._mgr.WriteGroupEnd(this._startIndex, this._groupName, in this._sampler);
    }
  }

  public readonly struct ValueGuard(ProfManager mgr, string text) : IDisposable
  {
    private readonly ProfManager _mgr = mgr;
    private readonly string _text = text;
    private readonly ProfSampler _sampler = ProfSampler.StartNew();

    public void Dispose() => this._mgr.WriteValue(this._text, in this._sampler);
  }
}
