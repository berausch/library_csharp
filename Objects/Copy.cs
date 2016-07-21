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
    private bool _checkedOut;
    private DateTime _dueDate;

    public Copy(int bookId, int number, DateTime dueDate, bool checkedOut = false,  int Id = 0)
    {
      _id = Id;
      _bookId = bookId;
      _number = number;
      _checkedOut = checkedOut;
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
    public string GetJustDate()
    {
      DateTime dateOnly = _dueDate.Date;
      string dateOnlyString = dateOnly.ToString("d");
      return dateOnlyString;
    }
    public void SetCopyDueDate(DateTime newDueDate)
    {
      _dueDate = newDueDate;
    }
    public bool GetCheckedOut()
    {
      return _checkedOut;
    }
    public void SetCheckedOut(bool newCheckedOut)
    {
      _checkedOut = newCheckedOut;
    }

    public Book GetBook()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE id = @BookId;", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      int bookId =0;
      string bookDescription = null;
      while(rdr.Read())
      {
        bookId = rdr.GetInt32(0);
        bookDescription = rdr.GetString(1);
      }
      Book newBook = new Book(bookDescription, bookId);
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return newBook;
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
        bool checkedOut = rdr.GetBoolean(3);
        DateTime copyDueDate = rdr.GetDateTime(4);
        Copy newCopy = new Copy(copyBookId, copyNumber, copyDueDate, checkedOut, copyId);
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

      SqlCommand cmd = new SqlCommand("INSERT INTO copies (book_id, number, checked_out, due) OUTPUT INSERTED.id VALUES (@CopyBookId, @CopyNumber, @checkedOut, @CopyDueDate)", conn);

      SqlParameter bookIdParam = new SqlParameter();
      bookIdParam.ParameterName = "@CopyBookId";
      bookIdParam.Value = this.GetBookId();

      cmd.Parameters.Add(bookIdParam);

      SqlParameter numberParam = new SqlParameter();
      numberParam.ParameterName = "@CopyNumber";
      numberParam.Value = this.GetNumber();

      cmd.Parameters.Add(numberParam);

      SqlParameter checkedOutParam = new SqlParameter();
      checkedOutParam.ParameterName = "@checkedOut";
      checkedOutParam.Value = this.GetCheckedOut();

      cmd.Parameters.Add(checkedOutParam);

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

    public static List<Copy> GetCopies(string bookTitle)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT copies.* FROM books JOIN copies ON (copies.book_id = books.id) WHERE books.title = @BookTitle", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookTitle";
      bookIdParameter.Value = bookTitle;
      cmd.Parameters.Add(bookIdParameter);
      SqlDataReader rdr = cmd.ExecuteReader();

      List<Copy> copies = new List<Copy> {};
      while(rdr.Read())
      {
        int copyId = rdr.GetInt32(0);
        int bookd = rdr.GetInt32(1);
        int copynumber = rdr.GetInt32(2);
        bool overDue = rdr.GetBoolean(3);
        DateTime due = rdr.GetDateTime(4);
        Copy newCopy = new Copy(bookd, copynumber, due, overDue, copyId);
        copies.Add(newCopy);
      }
      Console.Write(copies.Count);
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return copies;
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
      bool foundcheckedOut = false;
      DateTime foundCopyDueDate = DateTime.MinValue;

      while(rdr.Read())
      {
        foundCopyId = rdr.GetInt32(0);
        foundCopyBookId = rdr.GetInt32(1);
        foundNumber = rdr.GetInt32(2);
        foundcheckedOut = rdr.GetBoolean(3);
        foundCopyDueDate = rdr.GetDateTime(4);
      }
      Copy foundCopy = new Copy(foundCopyBookId, foundNumber, foundCopyDueDate, foundcheckedOut, foundCopyId);

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

    public void Return()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM checkouts WHERE copy_id = @CopyId;", conn);
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

    public void Update(bool newCheckedOut, DateTime newDate)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE copies SET checked_out = @NewCheck, due = @NewDate OUTPUT INSERTED.checked_out, INSERTED.due WHERE id = @PatronId;", conn);

      SqlParameter newNameParameter = new SqlParameter();
      newNameParameter.ParameterName = "@NewCheck";
      newNameParameter.Value = newCheckedOut;
      cmd.Parameters.Add(newNameParameter);

      SqlParameter newDateParameter = new SqlParameter();
      newDateParameter.ParameterName = "@NewDate";
      newDateParameter.Value = newDate;
      cmd.Parameters.Add(newDateParameter);

      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patronIdParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._checkedOut = rdr.GetBoolean(0);
        this._dueDate = rdr.GetDateTime(1);
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
