using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BracketGenerator;

public class Program{
    public static void Main(string [] args)
    {
        BracketGen bg = new BracketGen("SeedFile.json", "AdvanceEvents.json");
        Console.WriteLine(bg.GetTournamentWinner());
        bg.PathToVictory();
    }
}

public class Root
{
    public List<string>? Events { get; set; }
    public string? Winner { get; set; }
}

public class Winners
{
    public List<string>? WinningTeams { get; set; }
    public int? Round { get; set; }
}

public class R16
{
    public string? Seed { get; set; }
    public string? Team { get; set; }
}

public class Teams
{
    public List<R16>? R16 { get; set; }
}

public class MatchInfo
{
    public int? Round { get; set; }
    public int? MatchNo { get; set; }
    public string? TeamOne { get; set; }
    public string? TeamTwo { get; set; }
    public string? Winner { get; set; }

}

