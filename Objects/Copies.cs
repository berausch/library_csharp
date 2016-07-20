using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
  public class Copy
  {
    private int _id;
    private int _bookId;
    private int _number;
    private bool _overdue;
    private DateTime _dueDate;

    public Copy(int bookId, int number, DateTime dueDate, bool overdue = false,  int Id = 0)
    {
      _id = Id;
      _bookId = bookId;
      _number = number;
      _overdue = overdue;
      _dueDate = dueDate;
    }

    public override bool Equals(System.Object otherCopy)
    {
        if (!(otherCopy is Copy))
        {
          return false;
        }
        else {
          Copy newCopy = (Copy) otherCopy;
          bool idEquality = this.GetId() == newCopy.GetId();
          bool bookIdEquality = this.GetBookId() == newCopy.GetBookId();
          return (idEquality && bookIdEquality);
        }
    }

    public int GetId()
    {
      return _id;
    }
    public int GetBookId()
    {
      return _bookId;
    }
    public void SetBookId(int newBookId)
    {
      _bookId = newBookId;
    }
    public int GetNumber()
    {
      return _number;
    }
    public void SetNumber(int newNumber)
    {
      _number = newNumber;
    }
    public DateTime GetCopyDueDate()
    {
      return _dueDate;
    }
    public void SetCopyDueDate(DateTime newDueDate)
    {
      _dueDate = newDueDate;
    }
    public bool GetOverdue()
    {
      return _overdue;
    }
    public void SetOverdue(bool newOverdue)
    {
      _overdue = newOverdue;
    }
    public static List<Copy> GetAll()
    {
      List<Copy> AllCopys = new List<Copy>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM copies ORDER BY due", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int copyId = rdr.GetInt32(0);
        int copyBookId = rdr.GetInt32(1);
        int copyNumber = rdr.GetInt32(2);
        bool overdue = rdr.GetBoolean(3);
        DateTime copyDueDate = rdr.GetDateTime(4);
        Copy newCopy = new Copy(copyBookId, copyNumber, copyDueDate, overdue, copyId);
        AllCopys.Add(newCopy);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return AllCopys;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO copies (book_id, number, overdue, due) OUTPUT INSERTED.id VALUES (@CopyBookId, @CopyNumber, @overdue, @CopyDueDate)", conn);

      SqlParameter bookIdParam = new SqlParameter();
      bookIdParam.ParameterName = "@CopyBookId";
      bookIdParam.Value = this.GetBookId();

      cmd.Parameters.Add(bookIdParam);

      SqlParameter numberParam = new SqlParameter();
      numberParam.ParameterName = "@CopyNumber";
      numberParam.Value = this.GetNumber();

      cmd.Parameters.Add(numberParam);

      SqlParameter overdueParam = new SqlParameter();
      overdueParam.ParameterName = "@overdue";
      overdueParam.Value = this.GetOverdue();

      cmd.Parameters.Add(overdueParam);

      SqlParameter CopyDueDateParam = new SqlParameter();
      CopyDueDateParam.ParameterName = "@CopyDueDate";
      CopyDueDateParam.Value = this.GetCopyDueDate();

      cmd.Parameters.Add(CopyDueDateParam);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM copies;", conn);
      cmd.ExecuteNonQuery();
    }

    public static Copy Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM copies WHERE id = @CopyId", conn);
      SqlParameter copyIdParameter = new SqlParameter();
      copyIdParameter.ParameterName = "@CopyId";
      copyIdParameter.Value = id.ToString();
      cmd.Parameters.Add(copyIdParameter);
      rdr = cmd.ExecuteReader();

      int foundCopyId = 0;
      int foundCopyBookId = 0;
      int foundNumber = 0;
      bool foundoverdue = false;
      DateTime foundCopyDueDate = DateTime.MinValue;

      while(rdr.Read())
      {
        foundCopyId = rdr.GetInt32(0);
        foundCopyBookId = rdr.GetInt32(1);
        foundNumber = rdr.GetInt32(2);
        foundoverdue = rdr.GetBoolean(3);
        foundCopyDueDate = rdr.GetDateTime(4);
      }
      Copy foundCopy = new Copy(foundCopyBookId, foundNumber, foundCopyDueDate, foundoverdue, foundCopyId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCopy;
    }

    public void AddPatron(Patron newPatron)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO checkouts (patron_id, copy_id) VALUES (@PatronId, @CopyId);", conn);

      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = newPatron.GetId();
      cmd.Parameters.Add(patronIdParameter);

      SqlParameter copyIdParameter = new SqlParameter();
      copyIdParameter.ParameterName = "@CopyId";
      copyIdParameter.Value = this.GetId();
      cmd.Parameters.Add(copyIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Patron> GetPatrons()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT patron_id FROM checkouts WHERE copy_id = @CopyId;", conn);

      SqlParameter copyIdParameter = new SqlParameter();
      copyIdParameter.ParameterName = "@CopyId";
      copyIdParameter.Value = this.GetId();
      cmd.Parameters.Add(copyIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> patronIds = new List<int> {};

      while (rdr.Read())
      {
        int patronId = rdr.GetInt32(0);
        patronIds.Add(patronId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Patron> patrons = new List<Patron> {};

      foreach (int patronId in patronIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand patronQuery = new SqlCommand("SELECT * FROM patrons WHERE id = @PatronId;", conn);

        SqlParameter patronIdParameter = new SqlParameter();
        patronIdParameter.ParameterName = "@PatronId";
        patronIdParameter.Value = patronId;
        patronQuery.Parameters.Add(patronIdParameter);

        queryReader = patronQuery.ExecuteReader();
        while (queryReader.Read())
        {
          int thisPatronId = queryReader.GetInt32(0);
          string patronName = queryReader.GetString(1);
          Patron foundPatron = new Patron(patronName, thisPatronId);
          patrons.Add(foundPatron);
        }
        if (queryReader != null)
        {
          queryReader.Close();
        }
      }
      if (conn != null)
      {
        conn.Close();
      }
      return patrons;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM copies WHERE id = @CopyId; DELETE FROM checkouts WHERE copy_id = @CopyId;", conn);
      SqlParameter copyIdParameter = new SqlParameter();
      copyIdParameter.ParameterName = "@CopyId";
      copyIdParameter.Value = this.GetId();

      cmd.Parameters.Add(copyIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void Update(bool newComplete)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE copies SET overdue = @NewName OUTPUT INSERTED.overdue WHERE id = @PatronId;", conn);

      SqlParameter newNameParameter = new SqlParameter();
      newNameParameter.ParameterName = "@NewName";
      newNameParameter.Value = newComplete;
      cmd.Parameters.Add(newNameParameter);


      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patronIdParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._overdue = rdr.GetBoolean(0);
      }

      if (rdr != null)
      {
        rdr.Close();
      }

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
