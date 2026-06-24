// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.HappyEyeballsHttp
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Network;

internal static class HappyEyeballsHttp
{
  private const int ConnectionAttemptDelay = 250;

  public static SocketsHttpHandler CreateHttpHandler(bool autoRedirect = true)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return new SocketsHttpHandler()
    {
      ConnectCallback = HappyEyeballsHttp.\u003C\u003EO.\u003C0\u003E__OnConnect ?? (HappyEyeballsHttp.\u003C\u003EO.\u003C0\u003E__OnConnect = new Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>>(HappyEyeballsHttp.OnConnect)),
      AutomaticDecompression = DecompressionMethods.All,
      AllowAutoRedirect = autoRedirect
    };
  }

  private static async ValueTask<Stream> OnConnect(
    SocketsHttpConnectionContext context,
    CancellationToken cancellationToken)
  {
    DnsEndPoint endPoint = context.DnsEndPoint;
    IPAddress[] addresses = await HappyEyeballsHttp.GetIpsForHost(endPoint, cancellationToken).ConfigureAwait(false);
    IPAddress[] ips = addresses.Length != 0 ? HappyEyeballsHttp.SortInterleaved(addresses) : throw new Exception($"Host {context.DnsEndPoint.Host} resolved to no IPs!");
    return (Stream) new NetworkStream((await HappyEyeballsHttp.ParallelTask<Socket>(ips.Length, (Func<int, CancellationToken, Task<Socket>>) ((i, cancel) => HappyEyeballsHttp.AttemptConnection(i, ips[i], endPoint.Port, cancel)), TimeSpan.FromMilliseconds(250L), cancellationToken)).Item1, true);
  }

  private static async Task<Socket> AttemptConnection(
    int index,
    IPAddress address,
    int port,
    CancellationToken cancel)
  {
    Socket socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
    {
      NoDelay = true
    };
    Socket socket1;
    try
    {
      await socket.ConnectAsync((EndPoint) new IPEndPoint(address, port), cancel).ConfigureAwait(false);
      socket1 = socket;
    }
    catch (Exception ex)
    {
      socket.Dispose();
      throw;
    }
    socket = (Socket) null;
    return socket1;
  }

  private static async Task<IPAddress[]> GetIpsForHost(
    DnsEndPoint endPoint,
    CancellationToken cancel)
  {
    IPAddress address;
    if (!IPAddress.TryParse(endPoint.Host, out address))
      return (await Dns.GetHostEntryAsync(endPoint.Host, cancel).ConfigureAwait(false)).AddressList;
    return new IPAddress[1]{ address };
  }

  private static IPAddress[] SortInterleaved(IPAddress[] addresses)
  {
    IPAddress[] array1 = ((IEnumerable<IPAddress>) addresses).Where<IPAddress>((Func<IPAddress, bool>) (x => x.AddressFamily == AddressFamily.InterNetworkV6)).ToArray<IPAddress>();
    IPAddress[] array2 = ((IEnumerable<IPAddress>) addresses).Where<IPAddress>((Func<IPAddress, bool>) (x => x.AddressFamily == AddressFamily.InterNetwork)).ToArray<IPAddress>();
    int start = Math.Min(array1.Length, array2.Length);
    IPAddress[] array3 = new IPAddress[addresses.Length];
    for (int index = 0; index < start; ++index)
    {
      array3[index * 2] = array1[index];
      array3[1 + index * 2] = array2[index];
    }
    if (array2.Length > array1.Length)
      array2.AsSpan<IPAddress>(start).CopyTo(array3.AsSpan<IPAddress>(start * 2));
    else if (array1.Length > array2.Length)
      array1.AsSpan<IPAddress>(start).CopyTo(array3.AsSpan<IPAddress>(start * 2));
    return array3;
  }

  internal static async Task<(T, int)> ParallelTask<T>(
    int candidateCount,
    Func<int, CancellationToken, Task<T>> taskBuilder,
    TimeSpan delay,
    CancellationToken cancel)
    where T : IDisposable
  {
    ArgumentOutOfRangeException.ThrowIfNegativeOrZero<int>(candidateCount, nameof (candidateCount));
    List<Task<T>> allTasks;
    List<Task<T>> tasks;
    Task<T> successTask;
    (T, int) valueTuple;
    using (CancellationTokenSource successCts = CancellationTokenSource.CreateLinkedTokenSource(cancel))
    {
      allTasks = new List<Task<T>>();
      tasks = new List<Task<T>>();
      successTask = (Task<T>) null;
      while (successTask == null && (allTasks.Count < candidateCount || tasks.Count > 0))
      {
        if (allTasks.Count < candidateCount)
        {
          Task<T> task = taskBuilder(allTasks.Count, successCts.Token);
          tasks.Add(task);
          allTasks.Add(task);
        }
        Task<Task<T>> whenAnyDone = Task.WhenAny<T>((IEnumerable<Task<T>>) tasks);
        Task<T> task1;
        if (allTasks.Count < candidateCount)
        {
          if (await Task.WhenAny((Task) whenAnyDone, Task.Delay(delay, successCts.Token)).ConfigureAwait(false) == whenAnyDone)
            task1 = whenAnyDone.Result;
          else
            continue;
        }
        else
          task1 = await whenAnyDone.ConfigureAwait(false);
        if (task1.IsCompletedSuccessfully)
        {
          successTask = task1;
          break;
        }
        tasks.Remove(task1);
        whenAnyDone = (Task<Task<T>>) null;
      }
      cancel.ThrowIfCancellationRequested();
      await successCts.CancelAsync().ConfigureAwait(false);
      if (successTask == null)
        throw new AggregateException(allTasks.Where<Task<T>>((Func<Task<T>, bool>) (x => x.IsFaulted)).SelectMany<Task<T>, Exception>((Func<Task<T>, IEnumerable<Exception>>) (x => (IEnumerable<Exception>) x.Exception.InnerExceptions)));
      foreach (Task<T> task in allTasks)
      {
        if (task.IsCompletedSuccessfully && task != successTask)
          task.Result.Dispose();
      }
      valueTuple = (successTask.Result, allTasks.IndexOf(successTask));
    }
    allTasks = (List<Task<T>>) null;
    tasks = (List<Task<T>>) null;
    successTask = (Task<T>) null;
    return valueTuple;
  }
}
