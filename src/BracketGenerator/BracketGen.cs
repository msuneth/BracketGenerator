using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BracketGenerator
{
    public class BracketGen
    {
        int numOfMatches;
        int countRound;
        int numberOfRounds;
        int[] round_matches;
        List<Winners> allWinners = new List<Winners>();
        List<MatchInfo> roundmatches = new List<MatchInfo>();
        List<R16> teamlist;
        List<string> winners;
        public BracketGen(string seedFile,string winnersFile )
        {
            StreamReader teams = new StreamReader(seedFile);  // read json file  
            string json = teams.ReadToEnd();
            Teams teamJ = JsonConvert.DeserializeObject<Teams>(json);
            teamlist = teamJ.R16.ToList();
            var teamCount = teamJ.R16.Count();

            StreamReader adEvents = new StreamReader(winnersFile);  // read json file   
            string json1 = adEvents.ReadToEnd();
            Root advancedEvents = JsonConvert.DeserializeObject<Root>(json1);
            winners = advancedEvents.Events.ToList();


            numOfMatches = teamCount - 1;
            countRound = 0;
            numberOfRounds = 0;
            while (true)
            {
                var temp = (int)Math.Pow(2, countRound);         //logic for calculating number of match rounds
                if (temp == teamCount)
                {
                    numberOfRounds = countRound;
                    break;
                }
                countRound++;
            }

            round_matches = new int[numberOfRounds];
            int count = 0;
            while (count < numberOfRounds)
            {
                round_matches[count] = (teamCount / (int)Math.Pow(2, (count + 1)));          //round_matches[0] =round 1 matches
                if (round_matches[count] < 1)
                {
                    break;
                }
                count++;
            }

            //List<Winners> allWinners = new List<Winners>();
            GetTournamentWins();                                    //get each round match winning teams
           // List<MatchInfo> roundmatches = new List<MatchInfo>();

            if (teamCount == 16)
            {
                for (int i = 1; i <= numberOfRounds; i++)
                {
                    GenerateRoundMatches(i);                        // genenrate match schedule for each round
                }

                Console.WriteLine(GetTournamentWinner());               //print tounament winner
                //PathToVictory();                                    //write to csv
            }
            else
            {
                Console.WriteLine("Currently 16 team round is supported");
            }
               
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
                        MatchNo = i + 1,
                        Round = round,
                        TeamOne = roundmatches.Where(s => s.Round == (round - 1) && s.MatchNo == (2 * i + 1)).First().Winner,
                        TeamTwo = roundmatches.Where(s => s.Round == (round - 1) && s.MatchNo == (2 * i + 2)).First().Winner,
                    });
                }
            }
            foreach (var win in allWinners.Where(s => s.Round == round).ToList().First().WinningTeams)      // add winners for each match round
            {
                roundmatches.Where(w => w.TeamOne == win || w.TeamTwo == win).ToList().ForEach(s => s.Winner = win);
            }
        }

        public void GetTournamentWins()
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
        public string GetTournamentWinner()
        {
            string tounamentWinner = roundmatches.Last().Winner;
            return tounamentWinner;
        }
        public bool PathToVictory()
        {
            var winner = GetTournamentWinner();
            var path = roundmatches.Where(s => s.Winner == winner).ToList();
            using (var writer = new StreamWriter("PathToVictory.csv"))                      //writing to csv file
            try
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(path);
                }
                return true;
            }
                catch
                {
                    return false;
                }

        }
    }
}
