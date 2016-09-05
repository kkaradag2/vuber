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
create a logical group.

	c:\sql\WorkingDirectory\DPLY-100
	c:\sql\WorkingDirectory\DPLY-101

After that you can save your .sql files to subfolders.

	c:\sql\WorkingDirectory\DPLY-100\01-CreateEmployeesTable.sql
	c:\sql\WorkingDirectory\DPLY-100\02-AddSomeSampleEployees.sql
	c:\sql\WorkingDirectory\DPLY-100\03-AlterEployeesTable.sql
									
After save your sql files in DPLY-100 folder, use info command and display
pending files.

	vuber info

![alt text](https://github.com/kkaradag2/vuber/blob/master/images/info01.PNG "Display pendig files")


 You can see two logical groups that DPLY-100 and DPLY-101. You can also see
 logical groups files as state is pendig.  

 Now we are ready to migrate our files to SQL server. To do this use migrate command.
 migrate command is spacial command, read your logical groups "DPLY-100 nad DPLY-101"
 and collect each group files eachother and execute on sql server.

	vuver migrate

![alt text](https://github.com/kkaradag2/vuber/blob/master/images/migrate01.PNG "migrate")

migration is complated successfully , vuber move folder and files to the other special directory 
named Executed. Because of may you are configure second instance Vuber and migrate Executed files
to other SQl server. As like as test and production enviorements.

migration is complate with error, for example object þs already on database. Vuber rollback logical
group on database. That's mean all files on DPLY-100 folder is rollbacked.

![alt text](https://github.com/kkaradag2/vuber/blob/master/images/rollback01.PNG "Rollback logical group")

And your woring directory files that rollbacked move special folder named Rollback.


##Configration of Vuber

To configure Vuber first you need to understand some of special configration keys.

*Working Directory* : Working directory is a special folder that you can save your .sql files to migrate them.

*Executed Directory*: Executed directory is a special folder that after migration is complate successfuly your
files move.

*Rollback		  *: Rollback the other special folder that migration is not complates your scripts is rollbacked Database
and your sql files moved this directory.

*Connection*	   : SQL server connection string.
					Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;

To configure Vuber first time it is enoght to use congif command. To more information you can use --help config

		vuber --help config

![alt text](https://github.com/kkaradag2/vuber/blob/master/images/config01.PNG "Configration")
					

Some of configration sample;

###Set SQL server connection string

	vuber config -c Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;

###Set Working directory 

	vuber config -d c:\sql\workingDirectory

###set Executed directory

	vuber config -m c:\sql\Executed

###set Rollback folder

	vuber config -r c:\sql\rollback

###Configure Vuber with single line

	vuber config -c Server=myServerAddress;Database=myDataBase;Trusted_Connection=True; -d c:\sql\WorkingDirectory
	-m c:\sql\Executed -r c:\sql\rollback

###Display Current configration

	vuber config -l

###Test Vuber Configration

After complate Vuber configration, you can test Vuber configration with config -t command. This command is test enviroments
and display configration status. Every thing is well you can start to use Vuber.

	vuber config -t

![alt text](https://github.com/kkaradag2/vuber/blob/master/images/config02.PNG "Configration")

