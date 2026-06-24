using System;
using System.IO;
using Robust.Shared.Utility;
using Serilog.Events;

namespace Robust.Shared.Log;

internal sealed class FileLogHandler : ILogHandler, IDisposable
{
	private readonly TextWriter writer;

	public FileLogHandler(string path)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path));
		writer = TextWriter.Synchronized(new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read | FileShare.Delete), EncodingHelpers.UTF8));
	}

	public void Dispose()
	{
		writer.Dispose();
	}

	public void Log(string sawmillName, LogEvent message)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		string text = LogMessage.LogLevelToName(message.Level.ToRobust());
		TextWriter textWriter = writer;
		global::_003C_003Ey__InlineArray4<object> buffer = default(global::_003C_003Ey__InlineArray4<object>);
		buffer[0] = DateTime.Now;
		buffer[1] = text;
		buffer[2] = sawmillName;
		buffer[3] = message.RenderMessage((IFormatProvider)null);
		textWriter.WriteLine("{0:o} [{1}] {2}: {3}", (ReadOnlySpan<object?>)buffer);
		if (message.Exception != null)
		{
			writer.WriteLine(message.Exception.ToString());
		}
		writer.Flush();
	}
}
