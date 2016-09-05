# Vuber

Vuber is an open-source database migration tool. It's configured easly 
and using very simple command-line application that developed via C#. 
This version supports only Microsoft SQL Server at the moment. Our future 
plan is to cover MySQL and other databases (Oracle, PostgreSQL, Azure SQL).

## How Vuber Works

Vuber is a command-line application and it uses 4 simple commands
info, migrate, config and help.

Vuber waiting your .sql files a spacila folder that you set with config
command. For example; c:\sql\WorkingDirectory is aspecila folder that
you can save your .sql files to migrate. WorkingDirectory is not enough
to working. You must to create a sub folder to migration command meanning
create a logical group is created.

	c:\sql\WorkingDirectory\DPLY-100
	c:\sql\WorkingDirectory\DPLY-101

After that you can save your .sql files to subfolders.

	c:\sql\WorkingDirectory\DPLY-100\01-CreateEmployeesTable.sql
	c:\sql\WorkingDirectory\DPLY-100\02-AddSomeSampleEployees.sql
	c:\sql\WorkingDirectory\DPLY-100\03-AlterEployeesTable.sql
									
After save your sql files in DPLY-100 folder, use info command and display
pending files.

Inline-style: 
![alt text](https://github.com/kkaradag2/vuber/images/info01.png "Display pendig files")


 You can see two logical groups that DPLY-100 and DPLY-101. You can also see
 logical groups files as state is pendig.  


You can use --help to display command usage. For example;

	vuber --help config

This help diplay for you how you can use config command.



The sql files that you are working on are saved in to a special folder 
and Vuber takes these files and executes your SQL Server. If execution 
completed successfuly, Vuber will move your files in to other special 
folder named `executed`

If SQL Server fails to migrate while executing files, all operation 
will be rollbacked and files will be moved to `rollback` folder. 

## Using

You need to set 4 variables before you use Vuber:

    vuber config -c Server=yourSQLServer;Database=yourDatabase;Trusted_Connection=True;

