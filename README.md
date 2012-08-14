sqlcc
=====

**The Skinny**

SQLCC is a code coverage tool that allows developers the ability to track which lines of code within stored procedures, functions, and triggers have been tested in SQL server by integration tests written by the developer.

**The Longer Skinny**

We all know it's just silly to stuff business logic in our database-- its harder to maintain, database servers don't scale well, etc.  We could go on and on with reasons why you shouldn't put business logic in SQL.  However, there are times when you inherit that really ancient code-base.  You know, the one with 80-90% of the business rules written in SQL-- yes, that bad boy.  How in Poseidon's Trident do you refactor without breaking something?

The answer?  Meet SQLCC.  It's simple-- just do what you would normally do when faced with legacy code: write some tests and then get refactoring.  To get started, write some integration tests that call a stored procedure, function, or trigger and then use SQLCC to detect the code-coverage for your tests in SQL prior to your refactor to ensure you have covered all possible code-paths.  Then, start refactoring and change that good old integration test to a unit test while moving the business logic into your application code.

For now, Microsoft SQL Server is supported, but has the capability of being expanded to support additional database servers.

**The Codez**

In its most simple form, modify the App.config with a few values (i.e. connection string, application name, etc.) and then execute the following:

    sqlcc --app.mode=start --dbp.traceFileName=12345

    C:\Code\ConsoleApplication1\ConsoleApplication1\bin\Debug\YourTestApp.exe

    sqlcc --app.mode=stop --dbp.traceFileName=12345

You can also literally call MSTest or any other test runner.  Running the above generates a set of HTML files or you can alternatively create your own OutputProvider and store the results in another file format or in the database for querying.

**How Does it Work?**

It works by simply running a trace with a few settings enabled in order to detect code execution paths.  In order to limit traffic to just your running tests, one would simply pass in an Application Name to sqlcc and set up your tests to run using a connection string like the following:

    Data Source=localhost;Initial Catalog=MY_DB;User Id=sa;Password=password;Application Name=SQLCC;

Notice "SQLCC" as the Application Name.

**Is this done?**

Yes, for the most part, its a really rough proof of concept (aka alpha).  Definitely needs some love and unfortunately I'm not able to fully devote myself to this project.