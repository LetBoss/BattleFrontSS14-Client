using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Robust.Shared.Exceptions;

internal sealed class RuntimeLog : IRuntimeLog, IPostInjectInit
{
	private sealed class LoggedException
	{
		public Exception Exception { get; }

		public DateTime Time { get; }

		public string? Catcher { get; }

		public LoggedException(Exception exception, DateTime time, string? catcher)
		{
			Exception = exception;
			Time = time;
			Catcher = catcher;
		}
	}

	[Dependency]
	private readonly ILogManager _logManager;

	private readonly Dictionary<Type, List<LoggedException>> exceptions = new Dictionary<Type, List<LoggedException>>();

	private ISawmill _sawmill;

	public int ExceptionCount => exceptions.Values.Sum((List<LoggedException> l) => l.Count);

	public void LogException(Exception exception, string? catcher = null)
	{
		if (!exceptions.TryGetValue(exception.GetType(), out List<LoggedException> value))
		{
			value = new List<LoggedException>();
			exceptions[exception.GetType()] = value;
		}
		value.Add(new LoggedException(exception, DateTime.Now, catcher));
		if (catcher != null)
		{
			_sawmill.Log(LogLevel.Error, exception, "Caught exception in {Catcher}", catcher);
		}
		else
		{
			_sawmill.Log(LogLevel.Error, exception, "Caught exception");
		}
	}

	public string Display()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<Type, List<LoggedException>> exception2 in exceptions)
		{
			exception2.Deconstruct(out var key, out var value);
			Type type = key;
			List<LoggedException> list = value;
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(12, 3, stringBuilder2);
			handler.AppendFormatted(list.Count);
			handler.AppendLiteral(" exception ");
			handler.AppendFormatted((exceptions[type].Count > 1) ? "s" : "");
			handler.AppendLiteral(" ");
			handler.AppendFormatted(type);
			stringBuilder3.AppendLine(ref handler);
			foreach (LoggedException item in list)
			{
				Exception exception = item.Exception;
				DateTime time = item.Time;
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(19, 2, stringBuilder2);
				handler.AppendLiteral("Exception in ");
				handler.AppendFormatted(exception.TargetSite);
				handler.AppendLiteral(", at ");
				handler.AppendFormatted(time.ToString(CultureInfo.InvariantCulture));
				handler.AppendLiteral(":");
				stringBuilder4.AppendLine(ref handler);
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
				handler.AppendLiteral("Message: ");
				handler.AppendFormatted(exception.Message);
				stringBuilder5.AppendLine(ref handler);
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder6 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(9, 1, stringBuilder2);
				handler.AppendLiteral("Catcher: ");
				handler.AppendFormatted(item.Catcher);
				stringBuilder6.AppendLine(ref handler);
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder7 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder2);
				handler.AppendLiteral("Stack trace: ");
				handler.AppendFormatted(exception.StackTrace);
				stringBuilder7.AppendLine(ref handler);
				if (exception.Data.Count <= 0)
				{
					continue;
				}
				stringBuilder.AppendLine("Additional data:");
				foreach (object datum in exception.Data)
				{
					if (datum is DictionaryEntry dictionaryEntry)
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder8 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(2, 2, stringBuilder2);
						handler.AppendFormatted<object>(dictionaryEntry.Key);
						handler.AppendLiteral(": ");
						handler.AppendFormatted<object>(dictionaryEntry.Value);
						stringBuilder8.AppendLine(ref handler);
					}
				}
			}
		}
		return stringBuilder.ToString();
	}

	void IPostInjectInit.PostInject()
	{
		_sawmill = _logManager.GetSawmill("runtime");
	}
}
