# DataAccessLayer
A lightweight data access layer (DAL) made for SQLite databases. I find myself using SQLite as my database of choice. My work at the time of writing this also does not use any 3rd party libraries for the database other than SQLite. This library is a lightweight DAL that aims to reduce a lot of the boilerplate code that comes with the territory of using a relational datbase without a 3rd part object relational mapper (ORM).
## DAL Structure
The basic components of the DAL include an abstract database object, an IDBTable interface, and an abstract implementation of a DBTable object. The database object only knows how to interact with the IDBTable interface, leveraging generics to allow for basic CRUD operations on a database with both collections and single table objects. The abstract implementation of the database object can be inherited and things such as custom initializations can be added on top those basic CRUD operations.

The abstract implementation of the DBTable object works in sync with custom attributes to generate table objects with automatically written SQL string commands. Due to the slow nature of using reflection with custom attributes, the DAL uses a write once, store for later approach. This prevents the need to write a lot of standard SQL statements within C# source code while avoiding the costs of using reflection on every database operation. These SQL strings are then accessed using the Getters defined in the IDBTable interface.

This entire approach was done in an attempt to try and capture what is consistent across most SQL tables and abstract it away. This would, in theory, make implementing a SQL table much easier and with less boilerplate code. This is by no means a full and complete DAL. There are also likely better ways to do this type of implementation. This is just my best attempt at trying to make my life easier when using a SQLite database.
## Basic CRUD
The DAL provides ways to perform basic CRUD operations on tables. These operations include:
- Create a table
- Insert a single object
- Insert a collection of objects
- Update a single object
- Update a collection of objects
- Read all contents of a table
- Delete a single object from a table
- Delete a collection of objects from a table

## Advanced CRUD
TODO Implement the following operations
- Read single object by ID
- Drop table
- Update child IDs when foreign key is present and changes state
- Write SQL statements with foreign key when present

Research best ways to handle the tree like nature that comes from using foreign keys. Should the child/referencing tables be inserted as soon as the parent/referenced table is inserted? Or should each object that is considered a child/referencing table just update it's foreign key field and when it is inserted is determined later?
