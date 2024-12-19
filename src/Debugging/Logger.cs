﻿using System;
using System.Globalization;
using System.IO;

namespace MMXOnline;

// A logging class designed to log analytics and stats to some analytics server for analysis of game balance, trends, etc.
public class Logger {
	public static void logException(
		Exception ex, bool isServer,
		string additionalDetails = "", bool forceLog = false
	) {
		// TODO: Add normal data logging.
		if (!Global.debug) {
			return;
		}
		// Don't log this benign error
		if (ex.Message != null && ex.Message.ToLower().Contains("unable to read data from the transport connection")) {
			return;
		}

		string innerException = "";
		if (ex.InnerException != null) {
			innerException = "\nInner exception: " + ex.InnerException.Message + "\n" + ex.InnerException.StackTrace;
		}
		if (!string.IsNullOrEmpty(additionalDetails)) {
			additionalDetails = "\nAdditional details: " + additionalDetails;
		}

		string server = string.Format(CultureInfo.InvariantCulture, "SERVER (v{0}): ", Global.version);

		string custom = "";
		if (Global.level?.levelData?.isCustomMap == true || Global.checksum != Global.prodChecksum) {
			// Custom maps and modded games will generate a lot of logging noise, so deliniate them
			custom = " CUSTOM";
		}

		string client = string.Format(CultureInfo.InvariantCulture, "CLIENT (v{0}{1}): ", Global.version, custom);

		string errorMsg = ex.Message + "\n" + ex.StackTrace + innerException + additionalDetails;
		logEvent("error", (isServer ? server : client) + errorMsg, isServer: isServer, forceLog: forceLog);
	}

	public static void logEvent(
		string action, string label, long val = 0,
		bool isServer = false, bool forceLog = false
	) {
		// Does not log anything anywhere right now.
		/// TODO: Can add logging code here with analytics system of your choice or a custom solution.
	}

	public static string getMatchLabel(string selectedLevel, string gameMode) {
		return selectedLevel + ":" + gameMode;
	}

	public static double minSecondsBetweenLogs = 1;
	public const int maxLeeway = 4;
	public static DateTime? lastLogTime;
	public static int currentLeeway;
	public static bool throttleLogging() {
		if (lastLogTime == null) lastLogTime = DateTime.Now;
		else {
			var seconds = (DateTime.Now - lastLogTime.Value).TotalSeconds;
			lastLogTime = DateTime.Now;
			if (seconds < minSecondsBetweenLogs) {
				currentLeeway++;
				if (currentLeeway >= maxLeeway) {
					return true;
				}
			} else {
				if (currentLeeway > 0) currentLeeway--;
			}
		}
		return false;
	}

	public static void LogFatalException(Exception e) {
		string crashDump = e.Message + "\n\n" +
			e.StackTrace + "\n\nInner exception: " +
			e.InnerException?.Message + "\n\n" +
			e.InnerException?.StackTrace;

		Helpers.showMessageBox(crashDump.Truncate(1000), "Fatal Error!");

		try {
			if (!Directory.Exists(Global.writePath + "crashlogs")) {
				Directory.CreateDirectory(Global.writePath + "crashlogs");
			}
			string dateName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

			File.WriteAllText(Global.writePath + "crashlogs/" + dateName + ".txt", crashDump);
		} catch { }
	}

	public static void LogNonFatalError(string error) {
		try {
			if (!Directory.Exists(Global.writePath + "errorlogs")) {
				Directory.CreateDirectory(Global.writePath + "errorlogs");
			}
			string dateName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

			File.WriteAllText(Global.writePath + "errorlogs/" + dateName + ".txt", error);
		} catch { }
	}
}
