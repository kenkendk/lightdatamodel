A light OR mapper for .Net.

This is an alternative to Object Mappers such as NPersist or NHibernate (or the built-in typed DataSets in Visual Studio). Unlike the mentioned libraries, the focus on this project is to keep it as simple as possible (and more flexible than the typed DataSets).

We have worked with the existing Object Mappers for a while, and found that the items we want to model, usually has small deviations from the simple model. This makes it difficult to use the mappers, and requires some heavy manual coding. The complexity of the projects makes this a non-trivial task. The LightDataModel project seeks to remove the complexity.

As with all Object Mappers, the idea is to create typed C# classes as representation for database data, and remove the database problems from the programming. The methods usually involved in this is named CRUD: Create, Retrive, Update and Delete.

As this project focuses on simplicity, there is no Aspect oriented programming involved. There are no options to configure, and the project simply reads data from the database, and updates the private member variables with the data value. The naming scheme is fixed, and so is the access method. This reduction in flexibility makes the project much more simple. We feel that the removed options are rarely used, and can be fixed by some extra code.

The basic structure is this:
  * IDataProvider (executes SQL against a datasource, database dependant)
  * IDataFetcher (registers and loads objects from an IDataProvider)
  * DataClassBase (base class for database bound objects (optional))

The data fetcher can use any .Net object (POCO: Plain-Old-Clr-Object), and will read/write private members of the given objects. If the objects derive from DataClassBase, some events and flags will be activated.

If you are just starting out with the LightDataModel, you should see the page on [Automatic Project Creation](AutomaticProjectCreation.md).

If you want to do the class creation manually, please look at [Building Classes](BuildingClasses.md).

On top of the basic CRUD implementations a usual requirement is a "memory-transaction" aka a nested provider. This is most commonly used in GUI development, where the user presses a button and recieves a dialog. In this dialog, the data should reflect the current memory state of the data, and if the user clicks cancel, the changes should be rolled back. We do this with the class NestedDataFetcher:
```
IDataProvider provider = new SQLiteDataProvider("myfile.sqlite3");
IDataFetcher fetcher = new DataFetcher(provider);
IDataFetcher nestedFetcher = new NestedDataFetcher(fetcher);
```
Any objects loaded in the nestedFetcher will appear as they are in the fetcher, and data from nestedFetcher will only be written back on a call to the commit method.

In this model, class names map to table names, and field names map to column names. There is no built in relation management, but it is very easy to implement this manually:
```
//One to one, or Many-to-one
public ClassA Other
{
    get { return (ClassA)m_dataparent.GetObjectById(typeof(ClassA), m_otherid); }
    set { if (value == null) m_otherid = 0; else m_otherid = value.ID; }
}
//One to many
public object[] Others
{
    get { return m_dataparent.GetObjects(typeof(ClassA), "otherID = " + m_otherId.ToString()); }
}
```

A little extra is required to manage collections with Add/Remove methods. An abstract template class for this is in the making. A many-to-many relation can be created by implementing two one-to-may relations on the cross table.

To be truly usefull, the LightDataModel comes with a primitive [SQL parser](SqlParser.md).

An introduction to more advanced usage of the parser can be found on the [Using SQL parser with values](SqlParserValues.md) page.

There are many limitiations, but we feel that they can be overridden in the individual projects, and thus avoid complicating the object mapper:
  * Class names are used as table names, thus table names with spaces are not allowed
    * Override the tablename by using a simple view
  * Member values names are always the column name prefixed with `"m_"`
    * Adjust acessor code to use these values, or create views to rename columns
  * No shadowed properties (aka. delay load or ghosted)
    * Split the class into two, use a view to exclude the properties, and add acessor code that loads this
  * Supports only single column identity tables
    * Combine columns in a view

If you want a complex engine that handles all the above limitiations and has a nice GUI editor, we suggest [NPersist](http://www.puzzleframework.com).

**Beware**: The LightDatamodel is functional, but not tested enough to be publishable.