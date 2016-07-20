using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
  public class Author
  {
    private int _id;
    private string _name;

    public Author(string Name, int Id = 0)
    {
      _id= Id;
      _name = Name;
    }

    public override bool Equals(System.Object otherAuthor)
    {
      if (!(otherAuthor is Author))
      {
        return false;
      }
      else
      {
        Author newAuthor = (Author) otherAuthor;
        bool idEquality = this.GetId() == newAuthor.GetId();
        bool nameEquality = this.GetName() == newAuthor.GetName();
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
    public static List<Author> GetAll()
    {
      List<Author> allAuthors = new List<Author>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM authors", conn);
      rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        int authorId = rdr.GetInt32(0);
        string authorName = rdr.GetString(1);
        Author newAuthor = new Author(authorName, authorId);
        allAuthors.Add(newAuthor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allAuthors;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO authors (name) OUTPUT INSERTED.id VALUES (@AuthorName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@AuthorName";
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
      SqlCommand cmd = new SqlCommand("DELETE FROM authors;", conn);
      cmd.ExecuteNonQuery();
    }
    public static Author Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM authors WHERE id = @AuthorId;", conn);
      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "AuthorId";
      authorIdParameter.Value = id.ToString();
      cmd.Parameters.Add(authorIdParameter);
      rdr = cmd.ExecuteReader();

      int foundAuthorId = 0;
      string foundAuthorDescription = null;

      while(rdr.Read())
      {
        foundAuthorId = rdr.GetInt32(0);
        foundAuthorDescription = rdr.GetString(1);
      }
      Author foundAuthor = new Author(foundAuthorDescription, foundAuthorId);
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundAuthor;
    }
    public void AddBook(Book newBook)
   {
     SqlConnection conn = DB.Connection();
     conn.Open();

     SqlCommand cmd = new SqlCommand("INSERT INTO books_authors (author_id, book_id) VALUES (@AuthorId, @BookId)", conn);
     SqlParameter authorIdParameter = new SqlParameter();
     authorIdParameter.ParameterName = "@AuthorId";
     authorIdParameter.Value = this.GetId();
     cmd.Parameters.Add(authorIdParameter);

     SqlParameter bookIdParameter = new SqlParameter();
     bookIdParameter.ParameterName = "@BookId";
     bookIdParameter.Value = newBook.GetId();
     cmd.Parameters.Add(bookIdParameter);

     cmd.ExecuteNonQuery();

     if (conn != null)
     {
       conn.Close();
     }
   }
   public List<Book> GetBooks()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT book_id FROM books_authors WHERE author_id = @AuthorId;", conn);
      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = this.GetId();
      cmd.Parameters.Add(authorIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> bookIds = new List<int> {};
      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        bookIds.Add(bookId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Book> books = new List<Book> {};
      foreach (int bookId in bookIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand bookQuery = new SqlCommand("SELECT * FROM books WHERE id = @BookId;", conn);

        SqlParameter bookIdParameter = new SqlParameter();
        bookIdParameter.ParameterName = "@BookId";
        bookIdParameter.Value = bookId;
        bookQuery.Parameters.Add(bookIdParameter);

        queryReader = bookQuery.ExecuteReader();
        while(queryReader.Read())
        {
          int thisBookId = queryReader.GetInt32(0);
          string bookName = queryReader.GetString(1);

          Book foundBook = new Book(bookName, thisBookId);
          books.Add(foundBook);
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
      return books;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM authors WHERE id = @AuthorId; DELETE FROM books_authors WHERE author_id = @AuthorId;", conn);
      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = this.GetId();

      cmd.Parameters.Add(authorIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
