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
      Get ["/copies"]= _ => {
        List<Copy> AllCopies = Copy.GetAll();
        return View["copies.cshtml", AllCopies];
      };
      Get ["/patrons"]= _ =>{
        List<Patron> allPatrons = Patron.GetAll();
        return View ["patrons.cshtml", allPatrons];
      };
      Get ["copies/new"]= _ =>{
        return View ["copy_form.cshtml"];
      };
      Post ["copies/new"]= _ =>{
        Copy newCopy = new Copy(Request.Form["copy-description"],Request.Form["day"],Request.Form["copy-complete"]);
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
      Post["copy/add_patron"] = _ => {
        Patron patron = Patron.Find(Request.Form["patron-id"]);
        Copy copy = Copy.Find(Request.Form["copy-id"]);
        copy.AddPatron(patron);
        return View["success.cshtml"];
      };
      Post["patron/add_copy"] = _ => {
        Patron patron = Patron.Find(Request.Form["patron-id"]);
        Copy copy = Copy.Find(Request.Form["copy-id"]);
        patron.AddCopy(copy);
        return View["success.cshtml"];
      };
      Post ["/copyComplete"]= _ =>{
        // copyName = (Request.Form["copy-id"]);
        // copyBool =(Request.Form["copy-complete"]);
        // Copy copy = new Copy (copyName, copyBool);
        // copy.Save();
        Copy copy = Copy.Find(Request.Form["GetId"]);
        copy.Update(Request.Form["copy-complete"]);
        return View["success.cshtml"];
      };
    }
  }
}
