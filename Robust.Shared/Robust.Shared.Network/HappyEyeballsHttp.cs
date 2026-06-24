using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Robust.Shared.Network;

internal static class HappyEyeballsHttp
{
	private const int ConnectionAttemptDelay = 250;

	public static SocketsHttpHandler CreateHttpHandler(bool autoRedirect = true)
	{
		return new SocketsHttpHandler
		{
			ConnectCallback = OnConnect,
			AutomaticDecompression = DecompressionMethods.All,
			AllowAutoRedirect = autoRedirect
		};
	}

	private static async ValueTask<Stream> OnConnect(SocketsHttpConnectionContext context, CancellationToken cancellationToken)
	{
		DnsEndPoint endPoint = context.DnsEndPoint;
		IPAddress[] array = await GetIpsForHost(endPoint, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		if (array.Length == 0)
		{
			throw new Exception("Host " + context.DnsEndPoint.Host + " resolved to no IPs!");
		}
		IPAddress[] ips = SortInterleaved(array);
		Socket item = (await ParallelTask(ips.Length, (int i, CancellationToken cancel) => AttemptConnection(i, ips[i], endPoint.Port, cancel), TimeSpan.FromMilliseconds(250L), cancellationToken)).Item1;
		return new NetworkStream(item, ownsSocket: true);
	}

	private static async Task<Socket> AttemptConnection(int index, IPAddress address, int port, CancellationToken cancel)
	{
		Socket socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
		{
			NoDelay = true
		};
		try
		{
			await socket.ConnectAsync(new IPEndPoint(address, port), cancel).ConfigureAwait(continueOnCapturedContext: false);
			return socket;
		}
		catch (Exception)
		{
			socket.Dispose();
			throw;
		}
	}

	private static async Task<IPAddress[]> GetIpsForHost(DnsEndPoint endPoint, CancellationToken cancel)
	{
		if (IPAddress.TryParse(endPoint.Host, out IPAddress address))
		{
			return new IPAddress[1] { address };
		}
		return (await Dns.GetHostEntryAsync(endPoint.Host, cancel).ConfigureAwait(continueOnCapturedContext: false)).AddressList;
	}

	private static IPAddress[] SortInterleaved(IPAddress[] addresses)
	{
		IPAddress[] array = addresses.Where((IPAddress x) => x.AddressFamily == AddressFamily.InterNetworkV6).ToArray();
		IPAddress[] array2 = addresses.Where((IPAddress x) => x.AddressFamily == AddressFamily.InterNetwork).ToArray();
		int num = Math.Min(array.Length, array2.Length);
		IPAddress[] array3 = new IPAddress[addresses.Length];
		for (int num2 = 0; num2 < num; num2++)
		{
			array3[num2 * 2] = array[num2];
			array3[1 + num2 * 2] = array2[num2];
		}
		if (array2.Length > array.Length)
		{
			array2.AsSpan(num).CopyTo(array3.AsSpan(num * 2));
		}
		else if (array.Length > array2.Length)
		{
			array.AsSpan(num).CopyTo(array3.AsSpan(num * 2));
		}
		return array3;
	}

	internal static async Task<(T, int)> ParallelTask<T>(int candidateCount, Func<int, CancellationToken, Task<T>> taskBuilder, TimeSpan delay, CancellationToken cancel) where T : IDisposable
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(candidateCount, "candidateCount");
		using CancellationTokenSource successCts = CancellationTokenSource.CreateLinkedTokenSource(cancel);
		List<Task<T>> allTasks = new List<Task<T>>();
		List<Task<T>> tasks = new List<Task<T>>();
		Task<T> successTask = null;
		while (successTask == null && (allTasks.Count < candidateCount || tasks.Count > 0))
		{
			if (allTasks.Count < candidateCount)
			{
				Task<T> item = taskBuilder(allTasks.Count, successCts.Token);
				tasks.Add(item);
				allTasks.Add(item);
			}
			Task<Task<T>> whenAnyDone = Task.WhenAny(tasks);
			Task<T> task2;
			if (allTasks.Count < candidateCount)
			{
				Task task = Task.Delay(delay, successCts.Token);
				if (await Task.WhenAny(whenAnyDone, task).ConfigureAwait(continueOnCapturedContext: false) != whenAnyDone)
				{
					continue;
				}
				task2 = whenAnyDone.Result;
			}
			else
			{
				task2 = await whenAnyDone.ConfigureAwait(continueOnCapturedContext: false);
			}
			if (task2.IsCompletedSuccessfully)
			{
				successTask = task2;
				break;
			}
			tasks.Remove(task2);
		}
		cancel.ThrowIfCancellationRequested();
		await successCts.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
		if (successTask == null)
		{
			throw new AggregateException(allTasks.Where((Task<T> x) => x.IsFaulted).SelectMany((Task<T> x) => x.Exception.InnerExceptions));
		}
		foreach (Task<T> item2 in allTasks)
		{
			if (item2.IsCompletedSuccessfully && item2 != successTask)
			{
				item2.Result.Dispose();
			}
		}
		return (successTask.Result, allTasks.IndexOf(successTask));
	}
}
