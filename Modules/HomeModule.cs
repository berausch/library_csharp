using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ViewEngines.Razor;

namespace Library
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get ["/"]= _ =>{
        return View ["index.cshtml"];
      };
      Get ["/librarian/start"]= _ =>{
        return View ["librarian_start.cshtml", Book.GetAll()];
      };

      Post ["/librarian/start"]= _ =>{
        Book newBook = new Book(Request.Form["book-name"]);
        newBook.Save();
        Author newAuthor = new Author(Request.Form["author-name"]);
        newAuthor.Save();
        for(int i=0; i<Request.Form["copies-number"]; i++){
          Copy newCopy = new Copy(newBook.GetId(), 0, (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
          newCopy.Save();
        }
        newBook.AddAuthor(newAuthor);
        return View ["librarian_start.cshtml", Book.GetAll()];
      };

      Post ["/librarian/search/title"] = _ =>{
        List<Book> selectedBook = Book.FindByTitle(Request.Form["search-title"]);
        return View ["search_result.cshtml", selectedBook];
      };

      Post ["/librarian/search/author"] = _ =>{
        List<Book> selectedBook = Book.FindByAuthor(Request.Form["search-author"]);
        return View ["search_result.cshtml", selectedBook];
      };

      Post ["/book/{id}"] = parameters =>{
        Book SelectedBook = Book.Find(parameters.id);
        Author newAuthor = new Author(Request.Form["author-name"]);
        newAuthor.Save();
        newAuthor.AddBook(SelectedBook);
        return View ["librarian_start.cshtml", Book.GetAll()];
      };
      Get["/book/{id}"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Book SelectedBook = Book.Find(parameters.id);
        List<Author> BookAuthors = SelectedBook.GetAuthors();
        model.Add("book", SelectedBook);
        model.Add("authors", BookAuthors);
        return View ["book.cshtml", model];
      };
      Delete["/delete_books/{id}"] = parameters => {
        Book SelectedBook = Book.Find(parameters.id);
        SelectedBook.Delete();
        return View["librarian_start.cshtml", Book.GetAll()];
      };

      Get["/edit_books/{id}"] = parameters => {
        Book SelectedBook = Book.Find(parameters.id);
        return View["book_update.cshtml", SelectedBook];
      };
      Patch["/edit_books/{id}"] = parameters => {
        Book SelectedBook = Book.Find(parameters.id);
        SelectedBook.UpdateTitle(Request.Form["book-name"]);
        return View["librarian_start.cshtml", Book.GetAll()];
      };
      Get["/edit_author/{id}"] = parameters => {
        Author SelectedAuthor = Author.Find(parameters.id);
        return View["author_update.cshtml", SelectedAuthor];
      };
      Patch["/edit_author/{id}"] = parameters => {
        Author SelectedAuthor = Author.Find(parameters.id);
        SelectedAuthor.Update(Request.Form["author-name"]);
        return View["librarian_start.cshtml", Book.GetAll()];
      };
      Get ["/patron/start"]= _ =>{
        return View ["patron_start.cshtml"];
      };
      Get ["/copies"]= _ => {
        List<Copy> AllCopies = Copy.GetAll();
        return View["copies.cshtml", AllCopies];
      };
      Post ["/copies/number"]= _ => {
        List<Copy> AllCopies = Copy.GetCopies(Request.Form["find-number-of-books"]);
        return View["copies_of_book.cshtml", AllCopies];
      };
      Get ["/patrons"]= _ =>{
        List<Patron> allPatrons = Patron.GetAll();
        return View ["patrons.cshtml", allPatrons];
      };
      Get ["copies/new"]= _ =>{
        return View ["copy_form.cshtml"];
      };
      Post ["copies/new"]= _ =>{
        Copy newCopy = new Copy(Request.Form["book-id"], Request.Form["number"], Request.Form["day"], Request.Form["pass-due"]);
        newCopy.Save();
        List<Copy> AllCopies = Copy.GetAll();
        return View ["copies.cshtml", AllCopies];
      };
      Get ["patrons/new"]= _ =>{
        return View ["patrons_form.cshtml"];
      };
      Post ["patrons/new"]= _ =>{
        Patron newPatron = new Patron(Request.Form["patron-name"]);
        newPatron.Save();
        List<Patron> allPatrons = Patron.GetAll();
        return View ["patrons.cshtml", allPatrons];
      };
      Get["copies/{id}"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Copy SelectedCopy = Copy.Find(parameters.id);
        List<Patron> CopyPatrons = SelectedCopy.GetPatrons();
        List<Patron> AllPatrons = Patron.GetAll();
        model.Add("copy", SelectedCopy);
        model.Add("copyPatrons", CopyPatrons);
        model.Add("allPatrons", AllPatrons);
        return View["copy.cshtml", model];
      };

      Get["patrons/{id}"] = parameters => {
        Dictionary<string, object> model = new Dictionary<string, object>();
        Patron SelectedPatron = Patron.Find(parameters.id);
        List<Copy> PatronCopies = SelectedPatron.GetCopies();
        List<Copy> AllCopies = Copy.GetAll();
        model.Add("patron", SelectedPatron);
        model.Add("patronCopies", PatronCopies);
        model.Add("allCopies", AllCopies);
        return View["patron.cshtml", model];
      };
      Post["/copy/add_patron"] = _ => {
        Patron patron = Patron.Find(Request.Form["patron-id"]);
        Copy copy = Copy.Find(Request.Form["copy-id"]);
        copy.AddPatron(patron);
        return View["success.cshtml"];
      };
      Post["/patron/add_copy"] = _ => {
        Patron patron = Patron.Find(Request.Form["patron-id"]);
        Copy copy = Copy.Find(Request.Form["copy-id"]);
        patron.AddCopy(copy);
        DateTime updateDate = Request.Form["due-date"];
        updateDate = updateDate.AddDays(14);
        copy.Update(true, updateDate);
        return View["success.cshtml"];
      };
      Post["/patron/return_book/{id}"] = parameters => {
        Copy copy = Copy.Find(parameters.id);
        copy.Update(false, (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
        return View["success.cshtml"];
      };
    }
  }
}
