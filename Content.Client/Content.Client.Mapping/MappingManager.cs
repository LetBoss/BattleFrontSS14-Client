using System.IO;
using System.Text;
using System.Threading.Tasks;
using Content.Shared.Mapping;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Client.Mapping;

public sealed class MappingManager : IPostInjectInit
{
	[Dependency]
	private IFileDialogManager _file;

	[Dependency]
	private IClientNetManager _net;

	private Stream? _saveStream;

	private MappingMapDataMessage? _mapData;

	public void PostInject()
	{
		((INetManager)_net).RegisterNetMessage<MappingSaveMapMessage>((ProcessMessage<MappingSaveMapMessage>)null, (NetMessageAccept)3);
		((INetManager)_net).RegisterNetMessage<MappingSaveMapErrorMessage>((ProcessMessage<MappingSaveMapErrorMessage>)OnSaveError, (NetMessageAccept)3);
		((INetManager)_net).RegisterNetMessage<MappingMapDataMessage>((ProcessMessage<MappingMapDataMessage>)OnMapData, (NetMessageAccept)3);
	}

	private void OnSaveError(MappingSaveMapErrorMessage message)
	{
		_saveStream?.DisposeAsync();
		_saveStream = null;
	}

	private async void OnMapData(MappingMapDataMessage message)
	{
		if (_saveStream == null)
		{
			_mapData = message;
			return;
		}
		await _saveStream.WriteAsync(Encoding.ASCII.GetBytes(message.Yml));
		await _saveStream.DisposeAsync();
		_saveStream = null;
		_mapData = null;
	}

	public async Task SaveMap()
	{
		if (_saveStream != null)
		{
			await _saveStream.DisposeAsync();
		}
		MappingSaveMapMessage mappingSaveMapMessage = new MappingSaveMapMessage();
		((INetManager)_net).ClientSendMessage((NetMessage)(object)mappingSaveMapMessage);
		(Stream, bool)? tuple = await _file.SaveFile((FileDialogFilters)null, true, FileAccess.ReadWrite, FileShare.None);
		if (tuple.HasValue)
		{
			var (stream, _) = tuple.GetValueOrDefault();
			if (_mapData != null)
			{
				await stream.WriteAsync(Encoding.ASCII.GetBytes(_mapData.Yml));
				_mapData = null;
				await stream.FlushAsync();
				await stream.DisposeAsync();
			}
			else
			{
				_saveStream = stream;
			}
		}
	}
}
