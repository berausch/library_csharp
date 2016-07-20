using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
  public class Patron
  {
    private int _id;
    private string _name;

    public Patron(string Name, int Id = 0)
    {
      _id= Id;
      _name = Name;
    }

    public override bool Equals(System.Object otherPatron)
    {
      if (!(otherPatron is Patron))
      {
        return false;
      }
      else
      {
        Patron newPatron = (Patron) otherPatron;
        bool idEquality = this.GetId() == newPatron.GetId();
        bool nameEquality = this.GetName() == newPatron.GetName();
        return (idEquality && nameEquality);
      }
    }
    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public static List<Patron> GetAll()
    {
      List<Patron> allPatrons = new List<Patron>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM patrons", conn);
      rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        int patronId = rdr.GetInt32(0);
        string patronName = rdr.GetString(1);
        Patron newPatron = new Patron(patronName, patronId);
        allPatrons.Add(newPatron);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allPatrons;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO patrons (name) OUTPUT INSERTED.id VALUES (@PatronName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@PatronName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }
    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM patrons;", conn);
      cmd.ExecuteNonQuery();
    }
    public static Patron Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM patrons WHERE id = @PatronId;", conn);
      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "PatronId";
      patronIdParameter.Value = id.ToString();
      cmd.Parameters.Add(patronIdParameter);
      rdr = cmd.ExecuteReader();

      int foundPatronId = 0;
      string foundPatronDescription = null;

      while(rdr.Read())
      {
        foundPatronId = rdr.GetInt32(0);
        foundPatronDescription = rdr.GetString(1);
      }
      Patron foundPatron = new Patron(foundPatronDescription, foundPatronId);
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundPatron;
    }
    public void AddCopy(Copy newCopy)
   {
     SqlConnection conn = DB.Connection();
     conn.Open();

     SqlCommand cmd = new SqlCommand("INSERT INTO checkouts (patron_id, copy_id) VALUES (@PatronId, @CopyId)", conn);
     SqlParameter patronIdParameter = new SqlParameter();
     patronIdParameter.ParameterName = "@PatronId";
     patronIdParameter.Value = this.GetId();
     cmd.Parameters.Add(patronIdParameter);

     SqlParameter copyIdParameter = new SqlParameter();
     copyIdParameter.ParameterName = "@CopyId";
     copyIdParameter.Value = newCopy.GetId();
     cmd.Parameters.Add(copyIdParameter);

     cmd.ExecuteNonQuery();

     if (conn != null)
     {
       conn.Close();
     }
   }
   public List<Copy> GetCopies()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT copy_id FROM checkouts WHERE patron_id = @PatronId;", conn);
      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patronIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> copyIds = new List<int> {};
      while(rdr.Read())
      {
        int copyId = rdr.GetInt32(0);
        copyIds.Add(copyId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Copy> copies = new List<Copy> {};
      foreach (int copyId in copyIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand copyQuery = new SqlCommand("SELECT * FROM copies WHERE id = @CopyId;", conn);

        SqlParameter copyIdParameter = new SqlParameter();
        copyIdParameter.ParameterName = "@CopyId";
        copyIdParameter.Value = copyId;
        copyQuery.Parameters.Add(copyIdParameter);

        queryReader = copyQuery.ExecuteReader();
        while(queryReader.Read())
        {
          int thisCopyId = queryReader.GetInt32(0);
          int copyDescription = queryReader.GetInt32(1);
          int copyNumber = queryReader.GetInt32(2);
          bool copyComplete = queryReader.GetBoolean(3);
          DateTime copyDueDate = queryReader.GetDateTime(4);

          Copy foundCopy = new Copy(copyDescription, copyNumber, copyDueDate, copyComplete, thisCopyId);
          copies.Add(foundCopy);
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
      return copies;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM patrons WHERE id = @PatronId; DELETE FROM checkouts WHERE patron_id = @PatronId;", conn);
      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = this.GetId();

      cmd.Parameters.Add(patronIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
