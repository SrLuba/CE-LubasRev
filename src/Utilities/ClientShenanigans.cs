using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using MMXOnline;

namespace Utilities {
	public static class ClientShenanigans {
		
		public static void DirectConnect(string ip, int port) {
			Menu.change(new JoinMenuP2P(true));

			ServerPlayer me = new ServerPlayer(
				Options.main.playerName, 0, false,
				SelectCharacterMenu.playerData.charNum, null, Global.deviceId, null, 0, Utilities.SDKDiscord.localAvatar
			);

			Global.serverClient = ServerClient.CreateDirect(
				ip, port, new ServerPlayer(
				Options.main.playerName, 0, false,
				SelectCharacterMenu.playerData.charNum, 0, Global.deviceId, null, 0, Utilities.SDKDiscord.localAvatar
				),
				out JoinServerResponse joinServerResponse, out string error
			);

			if (joinServerResponse != null && error == null) {
				Menu.change(new WaitMenu(new MainMenu(), joinServerResponse.server, false));
			} else {
				Menu.change(new ErrorMenu(error, new MainMenu()));
			}

		}

	}
}
