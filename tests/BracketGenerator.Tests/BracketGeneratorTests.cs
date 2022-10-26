using Xunit;
//using BracketGen;
namespace BracketGenerator.Tests;

public class BracketGeneratorTests
{
    [Fact]
    public void Test_GetTournamentWinner()
    {
        BracketGen bracketGen = new BracketGen("SeedFile.json", "AdvanceEvents.json");
        var win = bracketGen.GetTournamentWinner();
        Assert.NotNull(win);
    }

    [Fact]
    public void Test_PathToVictory()
    {    
            BracketGen bracketGen = new BracketGen("SeedFile.json", "AdvanceEvents.json");
            var path=bracketGen.PathToVictory();
            Assert.True(path);
    }


}