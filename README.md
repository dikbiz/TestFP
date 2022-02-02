# TestFP
Testaufgabe Ilya Kuznetsov

Aufgabe:

Create a simple C#/.NET Restful API service, that

• reads from any URL with JSON-Data a list of product data.
The URL to read from is given below and contains mostly beers.

• has three different routes and questions to analyse the JSON-Data given.
o Most expensive and cheapest beer per litre.
o Which beers cost exactly €17.99?
Order the result by price per litre (cheapest first).
o Which one product comes in the most bottles?

• It also has one route to get the answer to all routes or questions at once.

• Any result or output should be in JSON, too.

A route may look like this: /api/mostBottles?url=http://urlto/productData.json
However the structure and naming of the routes is up to you and also part of your task.
You are invited to use and include whatever assets you find from our actual websites (as long as you
don’t host them publicly) in case you need/want to. You may use any tools that you like to
accomplish this task, including build/dependency management, IDE/editor, libraries, etc.
We should be able to build and run your project without needing to make any changes to it in a
recent version of a typical developer’s environment.
The URL for the JSON-Data is the following:

Anmerkungen
1. API ist mit Swagger UI implementiert
2. Code-Basis wurde aus dem swagger.Editor generiert (als .NET Core 3.1 und updated auf .NET 6.0)
3. Rückgabe ist ein JSON-Objekt, das eine Liste der Artikel in einer Eigenschaft beinhaltet
