# Arya

> A framework for personal assistance and automation through the loading of modular code.

**Note:** I added (updated?) this README 1/26/2018 after going through some old projects. This is an unfinished project, and I've just added a high level description.

This was a C# project I had started in 2013 based on some previous experience I had writing automation frameworks. The intention was to make a modular, multipurpose personal assistant tool for myself, to automate common tasks. 

Arya was intended to be the core framework to run on the machine, which provided logging, command line interface, I/O, and more. It would manage a number of modules. Modules were separate C# files compiled in to .DLLs that were intended to add various functionality as I needed it. The idea was that over time I would keep building on top of it as my knowledge progressed until I had a whole host of useful tools to make my life/job easier.

However, I cannot find any modules to plug in to it, and it appears not have gotten very far. At least it has comments! Back when I was a performance engineer, there were many rather mindless tasks that I had written small tools to automate, and I can see why I would have wanted this. Currently it looks like I had written:

 * A low level keyboard hook for windows. Such that you could press a certain key combo inside any application (such as CTRL+A) and it wuold pop up the Arya command line.
 * A very simple command line processor.
 * A static class that could load and manage "Modules" (.dll files which implemented a simple interface defined by Arya).
 * A (possibly thread safe? havent tested in years) task scheduler.
 * A (possibly thread safe? havent tested in years) threaded execution model used to build a form of Finite State Machine.
 
 While I've always been interested in writing an app like this... in recent years I either have less mind numbing, repetitive, tasks to do or have found easier ways to automate them, so I doubt that I will pick this up again.
