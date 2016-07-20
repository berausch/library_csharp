using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
  public class Book
  {
    private int _id;
    private string _name;

    public Book(string Name, int Id = 0)
    {
      _id= Id;
      _name = Name;
    }

    public override bool Equals(System.Object otherBook)
    {
      if (!(otherBook is Book))
      {
        return false;
      }
      else
      {
        Book newBook = (Book) otherBook;
        bool idEquality = this.GetId() == newBook.GetId();
        bool nameEquality = this.GetName() == newBook.GetName();
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
    public static List<Book> GetAll()
    {
      List<Book> allBooks = new List<Book>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books", conn);
      rdr = cmd.ExecuteReader();

      while (rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookName = rdr.GetString(1);
        Book newBook = new Book(bookName, bookId);
        allBooks.Add(newBook);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allBooks;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO books (name) OUTPUT INSERTED.id VALUES (@BookName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@BookName";
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
      SqlCommand cmd = new SqlCommand("DELETE FROM books;", conn);
      cmd.ExecuteNonQuery();
    }
    public static Book Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE id = @BookId;", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "BookId";
      bookIdParameter.Value = id.ToString();
      cmd.Parameters.Add(bookIdParameter);
      rdr = cmd.ExecuteReader();

      int foundBookId = 0;
      string foundBookDescription = null;

      while(rdr.Read())
      {
        foundBookId = rdr.GetInt32(0);
        foundBookDescription = rdr.GetString(1);
      }
      Book foundBook = new Book(foundBookDescription, foundBookId);
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundBook;
    }
    public void AddAuthor(Author newAuthor)
   {
     SqlConnection conn = DB.Connection();
     conn.Open();

     SqlCommand cmd = new SqlCommand("INSERT INTO books_authors (book_id, author_id) VALUES (@BookId, @AuthorId)", conn);
     SqlParameter bookIdParameter = new SqlParameter();
     bookIdParameter.ParameterName = "@BookId";
     bookIdParameter.Value = this.GetId();
     cmd.Parameters.Add(bookIdParameter);

     SqlParameter authorIdParameter = new SqlParameter();
     authorIdParameter.ParameterName = "@AuthorId";
     authorIdParameter.Value = newAuthor.GetId();
     cmd.Parameters.Add(authorIdParameter);

     cmd.ExecuteNonQuery();

     if (conn != null)
     {
       conn.Close();
     }
   }
   public List<Author> GetAuthors()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT author_id FROM books_authors WHERE book_id = @BookId;", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      rdr = cmd.ExecuteReader();

      List<int> authorIds = new List<int> {};
      while(rdr.Read())
      {
        int authorId = rdr.GetInt32(0);
        authorIds.Add(authorId);
      }
      if (rdr != null)
      {
        rdr.Close();
      }

      List<Author> authors = new List<Author> {};
      foreach (int authorId in authorIds)
      {
        SqlDataReader queryReader = null;
        SqlCommand authorQuery = new SqlCommand("SELECT * FROM authors WHERE id = @AuthorId;", conn);

        SqlParameter authorIdParameter = new SqlParameter();
        authorIdParameter.ParameterName = "@AuthorId";
        authorIdParameter.Value = authorId;
        authorQuery.Parameters.Add(authorIdParameter);

        queryReader = authorQuery.ExecuteReader();
        while(queryReader.Read())
        {
          int thisAuthorId = queryReader.GetInt32(0);
          string authorName = queryReader.GetString(1);

          Author foundAuthor = new Author(authorName, thisAuthorId);
          authors.Add(foundAuthor);
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
      return authors;
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @BookId; DELETE FROM books_authors WHERE book_id = @BookId;", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = this.GetId();

      cmd.Parameters.Add(bookIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
  }
}
