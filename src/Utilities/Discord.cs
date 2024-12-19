using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MMXOnline;


using System.Threading;
using Discord;
using System.Text.RegularExpressions;
using System.Net;

namespace Utilities {
	
	public static class SDKDiscord {
		public static Discord.Discord ds;
		public static Discord.Activity currentActivity;
		public static Discord.ActivityManager activityManager;
		public static Discord.UserManager userManager;
		static User self;
		public static bool gotUser = false;

		public static string localAvatar = "";
		public static byte[] localAvatarDownload;
		public static void Start() {
			ds = new Discord.Discord(1317955826979307741, (UInt64)Discord.CreateFlags.Default);
			activityManager = ds.GetActivityManager();
			userManager = ds.GetUserManager();

			userManager.OnCurrentUserUpdate += UserUpdate;
			

		}
		public static void UserUpdate() {

			self = userManager.GetCurrentUser();
			Options.main.playerName = self.Username;
			Console.WriteLine("Nos van a follar a todos");
			localAvatar = "https://cdn.discordapp.com/avatars/" + self.Id + "/" + self.Avatar;
			
			localAvatarDownload = new System.Net.WebClient().DownloadData(localAvatar);
			

			gotUser = true;
		}
		public static void SetupPresence(string state, string details) {

			currentActivity = new Discord.Activity {
				State = state,
				Details = details,
				Assets =
					{
						  LargeImage = "embedded_background", // using asset from app's settings
						  LargeText = details,
						  SmallImage = "embedded_cover", // you can also use URLs
						  SmallText = "OMMM",
					},
				Instance = true,
			};

		}
		public static void SetupPresence(string state, string details, string partyID, string serverID) {
			if (Global.localServer == null) return;
			currentActivity = new Discord.Activity {
				State = state,
				Details = details,
				Assets =
					{
						  LargeImage = "embedded_background", // using asset from app's settings
						  LargeText = details,
						  SmallImage = "embedded_cover", // you can also use URLs
						  SmallText = "OMMM",
					},
				Party =
				  {
					  Id = partyID,
					  Size = {
						  CurrentSize = Global.localServer.players.Count,
						  MaxSize = Global.localServer.maxPlayers,
					  },
				  },
				Secrets =
				  {
					  Join = serverID,
				  },
				Instance = true,
			};

		}
		public static void OnJoin(string serverIP) {
		
			ClientShenanigans.DirectConnect(serverIP, 7777);
		
		}
		public static bool joining = false;
		public static void Update() {
			ds.RunCallbacks();
			
			activityManager.UpdateActivity(currentActivity, (result) => {
				
			});
			activityManager.OnActivityJoin += secret => {
				Console.WriteLine("OnJoin {0}", secret);
				if (!joining) { 
					OnJoin(secret);
					joining = true;
				}
			};
			
			activityManager.OnActivityJoinRequest += (ref Discord.User user) =>
			{
				Console.WriteLine("OnJoinRequest {0} {1}", user.Username, user.Id);
			};
	
		}
	}
}
