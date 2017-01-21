# Fuzzy-logic inference

This repository is a fork of a cooperative uni project
[(link)](https://github.com/Jeanosis/IntelligentSystemsLabs).

The fork is reduced to a purely console application.
It only contains the fuzzy-logic inference logic,
a simple console app to check it, and some docs.
These are the parts I put most of my effort in for the original repo,
so I felt like keeping it here for further experiments.

The project is powered by .Net Core, so, if you haven't already,
you'll have to install it. To build the project do the following:
1. `cd src`
2. `dotnet restore`
3. `cd ConsoleApplication`
4. `dotnet build`

The app itself is an attempt to write a fuzzy-logic inference
system, that can solve fuzzy-logic inference tasks,
described in JSON format. The format of the tasks is
~~not as ugly as it could have been~~ pretty nice and
humans should be able to read and edit it. That way
the tasks can be described by hand in any text editor
(and then checked via the console app).

Check out the docs to better understand what it is all about.
