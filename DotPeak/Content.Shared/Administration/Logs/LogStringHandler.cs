// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.Logs.LogStringHandler
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Administration.Logs;

[InterpolatedStringHandler]
public ref struct LogStringHandler
{
  private DefaultInterpolatedStringHandler _handler;
  public readonly Dictionary<string, object?> Values;

  public LogStringHandler(int literalLength, int formattedCount)
  {
    this._handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount);
    this.Values = new Dictionary<string, object>();
  }

  public LogStringHandler(int literalLength, int formattedCount, IFormatProvider? provider)
  {
    this._handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount, provider);
    this.Values = new Dictionary<string, object>();
  }

  public LogStringHandler(
    int literalLength,
    int formattedCount,
    IFormatProvider? provider,
    Span<char> initialBuffer)
  {
    this._handler = new DefaultInterpolatedStringHandler(literalLength, formattedCount, provider, initialBuffer);
    this.Values = new Dictionary<string, object>();
  }

  private void AddFormat<T>(string? format, T value, string? argument = null)
  {
    if (format == null)
    {
      if (argument == null)
        return;
      string str1;
      if (argument[0] != '@')
      {
        str1 = argument;
      }
      else
      {
        string str2 = argument;
        str1 = str2.Substring(1, str2.Length - 1);
      }
      format = str1;
    }
    if (this.Values.TryAdd(format, (object) value) || this.Values[format] == (object) value)
      return;
    string str = format;
    int num = 2;
    format = $"{str}_{num}";
    while (!this.Values.TryAdd(format, (object) value))
    {
      format = $"{str}_{num}";
      ++num;
    }
  }

  public void AppendLiteral(string value) => this._handler.AppendLiteral(value);

  public void AppendFormatted<T>(T value, [CallerArgumentExpression("value")] string? argument = null)
  {
    this.AddFormat<T>((string) null, value, argument);
    this._handler.AppendFormatted<T>(value);
  }

  public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression("value")] string? argument = null)
  {
    this.AddFormat<T>(format, value, argument);
    this._handler.AppendFormatted<T>(value, format);
  }

  public void AppendFormatted<T>(T value, int alignment, [CallerArgumentExpression("value")] string? argument = null)
  {
    this.AddFormat<T>((string) null, value, argument);
    this._handler.AppendFormatted<T>(value, alignment);
  }

  public void AppendFormatted<T>(T value, int alignment, string? format, [CallerArgumentExpression("value")] string? argument = null)
  {
    this.AddFormat<T>(format, value, argument);
    this._handler.AppendFormatted<T>(value, alignment, format);
  }

  public void AppendFormatted(ReadOnlySpan<char> value) => this._handler.AppendFormatted(value);

  public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
  {
    this.AddFormat<string>(format, value.ToString());
    this._handler.AppendFormatted(value, alignment, format);
  }

  public void AppendFormatted(string? value) => this._handler.AppendFormatted(value);

  public void AppendFormatted(string? value, int alignment = 0, string? format = null)
  {
    this.AddFormat<string>(format, value);
    this._handler.AppendFormatted(value, alignment, format);
  }

  public void AppendFormatted(object? value, int alignment = 0, string? format = null)
  {
    this.AddFormat<object>((string) null, value, format);
    this._handler.AppendFormatted(value, alignment, format);
  }

  public string ToStringAndClear()
  {
    this.Values.Clear();
    return this._handler.ToStringAndClear();
  }
}
