// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Exceptions.RuntimeLog
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

#nullable enable
namespace Robust.Shared.Exceptions;

internal sealed class RuntimeLog : IRuntimeLog, IPostInjectInit
{
  [Dependency]
  private readonly ILogManager _logManager;
  private readonly Dictionary<Type, List<RuntimeLog.LoggedException>> exceptions = new Dictionary<Type, List<RuntimeLog.LoggedException>>();
  private ISawmill _sawmill;

  public int ExceptionCount
  {
    get
    {
      return this.exceptions.Values.Sum<List<RuntimeLog.LoggedException>>((Func<List<RuntimeLog.LoggedException>, int>) (l => l.Count));
    }
  }

  public void LogException(Exception exception, string? catcher = null)
  {
    List<RuntimeLog.LoggedException> loggedExceptionList;
    if (!this.exceptions.TryGetValue(exception.GetType(), out loggedExceptionList))
    {
      loggedExceptionList = new List<RuntimeLog.LoggedException>();
      this.exceptions[exception.GetType()] = loggedExceptionList;
    }
    loggedExceptionList.Add(new RuntimeLog.LoggedException(exception, DateTime.Now, catcher));
    if (catcher != null)
      this._sawmill.Log(LogLevel.Error, exception, "Caught exception in {Catcher}", (object) catcher);
    else
      this._sawmill.Log(LogLevel.Error, exception, "Caught exception");
  }

  public string Display()
  {
    StringBuilder stringBuilder1 = new StringBuilder();
    foreach ((Type key, List<RuntimeLog.LoggedException> loggedExceptionList) in this.exceptions)
    {
      StringBuilder stringBuilder2 = stringBuilder1;
      StringBuilder stringBuilder3 = stringBuilder2;
      StringBuilder.AppendInterpolatedStringHandler interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(12, 3, stringBuilder2);
      interpolatedStringHandler.AppendFormatted<int>(loggedExceptionList.Count);
      interpolatedStringHandler.AppendLiteral(" exception ");
      interpolatedStringHandler.AppendFormatted(this.exceptions[key].Count > 1 ? "s" : "");
      interpolatedStringHandler.AppendLiteral(" ");
      interpolatedStringHandler.AppendFormatted<Type>(key);
      ref StringBuilder.AppendInterpolatedStringHandler local1 = ref interpolatedStringHandler;
      stringBuilder3.AppendLine(ref local1);
      foreach (RuntimeLog.LoggedException loggedException in loggedExceptionList)
      {
        Exception exception = loggedException.Exception;
        DateTime time = loggedException.Time;
        StringBuilder stringBuilder4 = stringBuilder1;
        StringBuilder stringBuilder5 = stringBuilder4;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(19, 2, stringBuilder4);
        interpolatedStringHandler.AppendLiteral("Exception in ");
        interpolatedStringHandler.AppendFormatted<MethodBase>(exception.TargetSite);
        interpolatedStringHandler.AppendLiteral(", at ");
        interpolatedStringHandler.AppendFormatted(time.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        interpolatedStringHandler.AppendLiteral(":");
        ref StringBuilder.AppendInterpolatedStringHandler local2 = ref interpolatedStringHandler;
        stringBuilder5.AppendLine(ref local2);
        StringBuilder stringBuilder6 = stringBuilder1;
        StringBuilder stringBuilder7 = stringBuilder6;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder6);
        interpolatedStringHandler.AppendLiteral("Message: ");
        interpolatedStringHandler.AppendFormatted(exception.Message);
        ref StringBuilder.AppendInterpolatedStringHandler local3 = ref interpolatedStringHandler;
        stringBuilder7.AppendLine(ref local3);
        StringBuilder stringBuilder8 = stringBuilder1;
        StringBuilder stringBuilder9 = stringBuilder8;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder8);
        interpolatedStringHandler.AppendLiteral("Catcher: ");
        interpolatedStringHandler.AppendFormatted(loggedException.Catcher);
        ref StringBuilder.AppendInterpolatedStringHandler local4 = ref interpolatedStringHandler;
        stringBuilder9.AppendLine(ref local4);
        StringBuilder stringBuilder10 = stringBuilder1;
        StringBuilder stringBuilder11 = stringBuilder10;
        interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder10);
        interpolatedStringHandler.AppendLiteral("Stack trace: ");
        interpolatedStringHandler.AppendFormatted(exception.StackTrace);
        ref StringBuilder.AppendInterpolatedStringHandler local5 = ref interpolatedStringHandler;
        stringBuilder11.AppendLine(ref local5);
        if (exception.Data.Count > 0)
        {
          stringBuilder1.AppendLine("Additional data:");
          foreach (object obj in exception.Data)
          {
            if (obj is DictionaryEntry dictionaryEntry)
            {
              StringBuilder stringBuilder12 = stringBuilder1;
              StringBuilder stringBuilder13 = stringBuilder12;
              interpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(2, 2, stringBuilder12);
              interpolatedStringHandler.AppendFormatted<object>(dictionaryEntry.Key);
              interpolatedStringHandler.AppendLiteral(": ");
              interpolatedStringHandler.AppendFormatted<object>(dictionaryEntry.Value);
              ref StringBuilder.AppendInterpolatedStringHandler local6 = ref interpolatedStringHandler;
              stringBuilder13.AppendLine(ref local6);
            }
          }
        }
      }
    }
    return stringBuilder1.ToString();
  }

  void IPostInjectInit.PostInject() => this._sawmill = this._logManager.GetSawmill("runtime");

  private sealed class LoggedException
  {
    public Exception Exception { get; }

    public DateTime Time { get; }

    public string? Catcher { get; }

    public LoggedException(Exception exception, DateTime time, string? catcher)
    {
      this.Exception = exception;
      this.Time = time;
      this.Catcher = catcher;
    }
  }
}
