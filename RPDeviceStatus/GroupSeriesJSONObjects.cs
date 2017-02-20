/*
 * 	File:		GroupSeriesJSONObjects.cs
 * 	Author:		Matthew Fuller
 * 	Desc:		Provides nested classes for capturing various JSON responses from the REST API
 * 				of Polycom Group Series systems.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class GroupSeriesJSONObjects
{
    public class loginJSONObject
    {
        // Captures JSON response from /rest/session
        public Dictionary<string, string> loginStatus { get; set; }
        public string reason { get; set; }
        public Dictionary<string, string> session { get; set; }
        public string success { get; set; }
    }

    public class systemJSONObject
    {
        // Captures JSON response from /rest/system 
        public string build { get; set; }
        public string buildType { get; set; }
        public string hardwareVersion { get; set; }
        public Dictionary<string, string> lanStatus { get; set; }
        public string model { get; set; }
        public string rcBatteryCondition { get; set; }
        public string timeServerState { get; set; }
        public string serialNumber { get; set; }
        public string softwareVersion { get; set; }
        public string state { get; set; }
        public string systemName { get; set; }
        public string systemTime { get; set; }
        public string uptime { get; set; }
        public string rebootNeeded { get; set; }
    }

    public class statusJSONObject
    {
        // Captures JSON response from /rest/system/status
        // Each returned parameter fills one statusJSONObject
        public string name { get; set; }
        public string languageTag { get; set; }
        public string[] state { get; set; }
    }

    public class mediaStatsJSONObject
    {
        // Captures JSON response from /rest/conferences/{conference}/mediastats
        public string activeAnnexes { get; set; }
        public string actualBitRate { get; set; }
        public string actualFrameRate { get; set; }
        public string address { get; set; }
        public string bitRate { get; set; }
        public string connectionID { get; set; }
        public string encryptionAlgorithm { get; set; }
        public string encryptionMode { get; set; }
        public string encryptionType { get; set; }
        public string errorConcealment { get; set; }
        public string frameRate { get; set; }
        public string index { get; set; }
        public string jitter { get; set; }
        public string latency { get; set; }
        public string maxJitter { get; set; }
        public string mediaAlgorithm { get; set; }
        public string mediaDirection { get; set; }
        public string mediaFormat { get; set; }
        public string mediaState { get; set; }
        public string mediaStream { get; set; }
        public string mediaType { get; set; }
        public string numberOfErrors { get; set; }
        public string packetLoss { get; set; }
        public string percentPacketLoss { get; set; }
        public string prevTermId { get; set; }
        public string qualityIndicator { get; set; }
        public string reservationError { get; set; }
        public string reservationProtocol { get; set; }
        public string reservationState { get; set; }
        public string termId { get; set; }
        public string totalPackets { get; set; }
        public string uniqueID { get; set; }
    }
}


