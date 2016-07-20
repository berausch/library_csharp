using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace Library
{
  public class CopyTest : IDisposable
  {
    public CopyTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_AddPatron_AddsPatronToCopy()
    {
      //Arrange
      Copy testCopy = new Copy(1, 2, new DateTime(2016, 5, 4), true);
      testCopy.Save();

      Patron testPatron = new Patron("Home stuff");
      testPatron.Save();

      //Act
      testCopy.AddPatron(testPatron);

      List<Patron> result = testCopy.GetPatrons();
      List<Patron> testList = new List<Patron>{testPatron};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_GetPatrons_ReturnsAllCopyPatrons()
    {
      //Arrange
      Copy testCopy = new Copy(1, 2, new DateTime(2016, 5, 4), true);
      testCopy.Save();

      Patron testPatron1 = new Patron("Home stuff");
      testPatron1.Save();

      Patron testPatron2 = new Patron("Work stuff");
      testPatron2.Save();

      //Act
      testCopy.AddPatron(testPatron1);
      List<Patron> result = testCopy.GetPatrons();
      List<Patron> testList = new List<Patron> {testPatron1};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_Delete_DeletesCopyAssociationsFromDatabase()
    {
      //Arrange
      Patron testPatron = new Patron("Home stuff");
      testPatron.Save();

      DateTime testDuedate = new DateTime(2016, 5, 4);
      Copy testCopy = new Copy(1, 2, testDuedate, true);
      testCopy.Save();

      //Act
      testCopy.AddPatron(testPatron);
      testCopy.Delete();

      List<Copy> resultPatronCopies = testPatron.GetCopies();
      List<Copy> testPatronCopies = new List<Copy> {};

      //Assert
      Assert.Equal(testPatronCopies, resultPatronCopies);
    }
    public void Dispose()
    {
      Copy.DeleteAll();
      Patron.DeleteAll();
    }
  }
}
