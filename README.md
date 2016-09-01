# Vuber

Vuber is an open-source database migration tool. It's configured easly 
and using very simple command-line application that developed via C#. 
This version supports only Microsoft SQL Server at the moment. Our future 
plan is to cover MySQL and other databases (Oracle, PostgreSQL, Azure SQL).

## How Vuber Works

The sql files that you are working on are saved in to a special folder 
and Vuber takes these files and executes your SQL Server. If execution 
completed successfuly, Vuber will move your files in to other special 
folder named `executed`

If SQL Server fails to migrate while executing files, all operation 
will be rollbacked and files will be moved to `rollback` folder. 

## Using

You need to set 4 variables before you use Vuber:

    vuber config -c Server=yourSQLServer;Database=yourDatabase;Trusted_Connection=True;

