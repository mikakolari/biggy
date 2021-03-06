﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biggy.Postgres;
using Xunit;
using Biggy;

namespace Tests.Postgres {

  [Trait("PG Documents","")]
  public class PGDocs {

    PGDocumentList<CustomerDocument> docs;
    public PGDocs() {
      docs = new PGDocumentList<CustomerDocument>("dvds");
      //drop and reload
      docs.Clear();
    }

    [Fact(DisplayName = "Creates a table if none exists")]
    public void CreatesTable() {
      Assert.True(docs.Count() == 0);
    }

    [Fact(DisplayName = "Adds a document")]
    public void AddsDocument() {
      var newCustomer = new CustomerDocument { Email = "rob@tekpub.com", First = "Rob", Last = "Conery" };
      docs.Add(newCustomer);
      Assert.Equal(1, docs.Count);
    }

    [Fact(DisplayName = "Updates a document")]
    public void UpdatesDocument() {
      var newCustomer = new CustomerDocument { Email = "rob@tekpub.com", First = "Rob", Last = "Conery" };
      docs.Add(newCustomer);
      newCustomer.First = "Bill";
      var updated = docs.Update(newCustomer);
      Assert.Equal(1, updated);
    }

    [Fact(DisplayName = "All records are populated correctly")]
    public void RecordsPopulated() {
      docs.Clear();
      var newCustomer = new CustomerDocument { Email = "buddy@tekpub.com", First = "Buddy", Last = "Conery" };
      docs.Add(newCustomer);

      //load a new, separate list
      var customers = new PGDocumentList<CustomerDocument>("dvds");
      //there should be some records here based on the above
      Assert.Equal("buddy@tekpub.com", customers.First().Email);
      Assert.Equal("Buddy", customers.First().First);
      Assert.Equal("Conery", customers.First().Last);
    }
    [Fact(DisplayName = "Deletes a document")]
    public void DeletesDocument() {
      var newCustomer = new CustomerDocument { Email = "rob@tekpub.com", First = "Rob", Last = "Conery" };
      docs.Add(newCustomer);
      var removed = docs.Remove(newCustomer);
      Assert.True(removed);
    }

    class Monkey
    {
      [PrimaryKey]
      public string Name { get; set; }
      public DateTime Birthday { get; set; }
      [FullText]
      public string Description { get; set; }
    }

    [Fact(DisplayName = "Inserts metric butt-load of new records as JSON documents with string key")]
    static void InsertsManyMonkeys()
    {
      int INSERT_QTY = 100;
      var monkies = new PGDocumentList<Monkey>("northwindPG");
      monkies.Clear();

      var addRange = new List<Monkey>();
      for (int i = 0; i < INSERT_QTY; i++)
      {
        addRange.Add(new Monkey { Name = "MONKEY " + i, Birthday = DateTime.Today, Description = "The Monkey on my back" });
      }
      var inserted = monkies.AddRange(addRange);
      Assert.True(inserted == INSERT_QTY && monkies.Count == inserted);
    }

    [Fact(DisplayName = "Will create a table with serial PK")]
    public void CreatesSerialPK() {
      var actors = new PGDocumentList<Actor>("northwindPG");
      var newActor = new Actor { First_Name = "Joe", Last_Name = "Blow" };
      actors.Add(newActor);
      int newId = newActor.Actor_ID;
      actors.Reload();
      Assert.True(actors.Any(a => a.Actor_ID == newId));
    }

    [Fact(DisplayName = "Inserts metric butt-load of new records as JSON documents with integer key")]
    static void InsertsManyActors() {
      int INSERT_QTY = 100;
      var actors = new PGDocumentList<Actor>("northwindPG");
      var bulkList = new List<Actor>();
      for (int i = 0; i < INSERT_QTY; i++) {
        var newActor = new Actor { First_Name = "Actor " + i, Last_Name = "Test" };
        bulkList.Add(newActor);
      }
      actors.AddRange(bulkList);
      Assert.True(actors.Last().Actor_ID > INSERT_QTY);
    }

  }
}
