// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Log.FileLogHandler
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using Serilog.Events;
using System;
using System.IO;

#nullable enable
namespace Robust.Shared.Log;

internal sealed class FileLogHandler : ILogHandler, IDisposable
{
  private readonly TextWriter writer;

  public FileLogHandler(string path)
  {
    Directory.CreateDirectory(Path.GetDirectoryName(path));
    this.writer = TextWriter.Synchronized((TextWriter) new StreamWriter((Stream) new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read | FileShare.Delete), EncodingHelpers.UTF8));
  }

  public void Dispose() => this.writer.Dispose();

  public void Log(string sawmillName, LogEvent message)
  {
    string name = LogMessage.LogLevelToName(message.Level.ToRobust());
    TextWriter writer = this.writer;
    \u003C\u003Ey__InlineArray4<object> buffer = new \u003C\u003Ey__InlineArray4<object>();
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 0) = (object) DateTime.Now;
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 1) = (object) name;
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 2) = (object) sawmillName;
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 3) = (object) message.RenderMessage();
    // ISSUE: reference to a compiler-generated method
    ReadOnlySpan<object> readOnlySpan = \u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray4<object>, object>(in buffer, 4);
    writer.WriteLine("{0:o} [{1}] {2}: {3}", readOnlySpan);
    if (message.Exception != null)
      this.writer.WriteLine(message.Exception.ToString());
    this.writer.Flush();
  }
}
