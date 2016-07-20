using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class PatronTest : IDisposable
  {
    public PatronTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
    }
    [Fact]
    public void Test_PatronsEmptyAtFirst()
    {
      int result = Patron.GetAll().Count;
      Assert.Equal(0, result);
    }
    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      Patron firstPatron = new Patron("Household chores");
      Patron secondPatron = new Patron("Household chores");
      Assert.Equal(firstPatron, secondPatron);
    }
    [Fact]
    public void Test_Save_SavesPatronToDatabase()
    {
      Patron testPatron = new Patron("Household chores");
      testPatron.Save();
      List<Patron> result = Patron.GetAll();
      List<Patron> testList = new List<Patron>{testPatron};
      Assert.Equal(testList, result);
    }
    [Fact]
    public void Test_Save_AssignsIdToPatronObject()
    {
      Patron testPatron = new Patron("Household chores");
      testPatron.Save();
      Patron savedPatron = Patron.GetAll()[0];
      int result = savedPatron.GetId();
      int testId = testPatron.GetId();
      Assert.Equal (testId, result);
    }
    [Fact]
    public void Test_Find_FindsPatronInDatabase()
    {
      Patron testPatron = new Patron("Household chores");
      testPatron.Save();
      Patron foundPatron = Patron.Find(testPatron.GetId());
      Assert.Equal(testPatron, foundPatron);
    }
    [Fact]
    public void Test_AddCopy_AddsCopyToPatron()
    {
      //Arrange
      Patron testPatron = new Patron("Household chores");
      testPatron.Save();

      Copy testCopy = new Copy(1, 2, new DateTime(2016, 5, 4), true);
      testCopy.Save();

      Copy testCopy2 = new Copy(1, 2, new DateTime(2016, 5, 4), true);
      testCopy2.Save();

      //Act
      testPatron.AddCopy(testCopy);
      testPatron.AddCopy(testCopy2);
      List<Copy> result = testPatron.GetCopies();
      List<Copy> testList = new List<Copy>{testCopy, testCopy2};
      Console.WriteLine(result);

      //Assert
      Assert.Equal(testList, result);
    }
    [Fact]
    public void Test_Delete_DeletesPatronAssociationsFromDatabase()
    {
      //Arrange
      Copy testCopy = new Copy(1, 2, new DateTime(2016, 5, 4), true);
      testCopy.Save();

      string testName = "Home stuff";
      Patron testPatron = new Patron(testName);
      testPatron.Save();

      //Act
      testPatron.AddCopy(testCopy);
      testPatron.Delete();

      List<Patron> resultCopyPatrons = testCopy.GetPatrons();
      List<Patron> testCopyPatrons = new List<Patron> {};

      //Assert
      Assert.Equal(testCopyPatrons, resultCopyPatrons);
    }
    public void Dispose()
    {
      Copy.DeleteAll();
      Patron.DeleteAll();
    }
  }
}
