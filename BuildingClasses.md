# Introduction #

This page will describe how you can manually construct classes for use with the LightDataModel.


# Details #

The data fetcher can use any .Net object (POCO: Plain-Old-Clr-Object), and will read/writee private members of the given objects. If the objects derive from DataClassBase?, some events and flags will be activated.

The Class name will be used directly as the table name. The fetcher will execute a `"SELECT * FROM TABLENAME"` on the datasource. When reading the results, it will look for private members where the name is the same as the column name with "m_" prepended to it. If such a member variable exists, the database value is written into the variable._

Null values are converted to sane values, using the DbNullValues on the provider.