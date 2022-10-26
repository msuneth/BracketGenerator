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

public class Program{
    public static void Main(string [] args)
    {  
        StreamReader teams = new StreamReader("SeedFile.json");  // read json file 
        string json = teams.ReadToEnd();
        Teams teamJ = JsonConvert.DeserializeObject<Teams>(json);
        List<R16> teamlist= teamJ.R16.ToList();
        var teamCount = teamJ.R16.Count();

        StreamReader adEvents = new StreamReader("AdvanceEvents.json");  // read json file 
        string json1 = adEvents.ReadToEnd();
        Root advancedEvents = JsonConvert.DeserializeObject<Root>(json1);
        List<string> winners = advancedEvents.Events.ToList();

        int numOfMatches = teamCount - 1;
        int countRound=0;
        int numberOfRounds = 0;
        while (true)
        {
            var temp = (int)Math.Pow(2,countRound);         //logic for calculating number of match rounds
            if (temp == teamCount)
            {
                numberOfRounds=countRound;
                break;
            }
            countRound++;
        }

        int[] round_matches= new int[numberOfRounds];
        int count = 0;
        while (count< numberOfRounds)
        {
            round_matches[count] = (teamCount /(int)Math.Pow(2, (count + 1)));          //round_matches[0] =round 1 matches
            if (round_matches[count] < 1)
            {
                break;
            }
            count++;
        }

        List<Winners> allWinners = new List<Winners>();
        GetTournamentWins();                                    //get each round match winning teams
        List<MatchInfo> roundmatches = new List<MatchInfo>();

        if (teamCount == 16)
        {
            for (int i = 1; i <= numberOfRounds; i++)
            {
                GenerateRoundMatches(i);                        // genenrate match schedule for each round
            }

            Console.WriteLine(GetTournamentWinner());               //print tounament winner
            PathToVictory();                                    //write to csv
        }
        else
        {
            Console.WriteLine("Currently 16 team round is supported");
        }

        void GenerateRoundMatches(int round)
        {
            if (round == 1) //round 1 
            {
                string[] round1TeamOne = { "1A", "1C", "1E", "1G", "1B", "1D", "1F", "1H" };      //round 1 match fixure for team1 from pdf
                string[] round1TeamTwo = { "2B", "2D", "2F", "2H", "2A", "2C", "2E", "2G" };      //round 1 match fixure for team2 from pdf


                for (int i = 0; i < round_matches[round - 1]; i++)
                {
                    roundmatches.Add(new MatchInfo()
                    {
                        MatchNo = i + 1,
                        Round = round,
                        TeamOne = teamlist.Where(s => s.Seed == round1TeamOne[i]).ToList().First().Team,
                        TeamTwo = teamlist.Where(s => s.Seed == round1TeamTwo[i]).ToList().First().Team,
                    });
                }
            }
            else //round 2 onwards
            {
                for (int i = 0; i < round_matches[round - 1]; i++)
                {
                    roundmatches.Add(new MatchInfo()
                    {
                        MatchNo = i+1,
                        Round = round,
                        TeamOne = roundmatches.Where(s => s.Round == (round-1) && s.MatchNo == (2*i + 1)).First().Winner,
                        TeamTwo = roundmatches.Where(s => s.Round == (round-1) && s.MatchNo == (2*i + 2)).First().Winner,
                    });
                }
            }
            foreach (var win in allWinners.Where(s => s.Round == round).ToList().First().WinningTeams)      // add winners for each match round
            {
                roundmatches.Where(w => w.TeamOne == win || w.TeamTwo == win).ToList().ForEach(s => s.Winner = win);
            }
        }

        void GetTournamentWins()
        {
            for (int i = 0; i < numberOfRounds; i++)
            {
                allWinners.Add(new Winners()
                {
                    Round = 1 + i,
                    WinningTeams = winners.GroupBy(x => x).Where(g => g.Count() > i).Select(x => x.Key).Distinct().ToList()     
                });
            }
        }

        string GetTournamentWinner()
        {
            string tounamentWinner = roundmatches.Last().Winner;
            return tounamentWinner;
        }

        void PathToVictory()
        {
            var winner = GetTournamentWinner();
            var path = roundmatches.Where(s => s.Winner == winner).ToList();
            using (var writer = new StreamWriter("PathToVictory.csv"))                      //writing to csv file
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(path);
            }

        }
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

