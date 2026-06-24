using System;

namespace Robust.Shared.Audio.Sources;

internal sealed class DummyBufferedAudioSource : DummyAudioSource, IBufferedAudioSource, IAudioSource, IDisposable
{
	public new static DummyBufferedAudioSource Instance { get; } = new DummyBufferedAudioSource();

	public int SampleRate { get; set; }

	public void WriteBuffer(int handle, ReadOnlySpan<ushort> data)
	{
	}

	public void WriteBuffer(int handle, ReadOnlySpan<float> data)
	{
	}

	public void QueueBuffers(ReadOnlySpan<int> handles)
	{
	}

	public void EmptyBuffers()
	{
	}

	public void GetBuffersProcessed(Span<int> handles)
	{
	}

	public int GetNumberOfBuffersProcessed()
	{
		return 0;
	}
}
