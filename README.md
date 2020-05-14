# Irc4netButSmarter

This is a fork of Suchiman's [Irc4netButSmarter](https://github.com/Suchiman/Irc4netButSmarter), which itself is a fork of Meebey's [SmartIrc4Net](https://github.com/meebey/SmartIrc4net). 

I made this to potentially use it for a project where I could use SmartIrc4Net based code with async. However, this project hasn't come to fruition yet, so I am not actively using or really mantaining this code right now and don't know to what extent my changes work bugfree.

API changes:

* Added `Microsoft.VisualStudio.Threading` package as a dependence to use AsyncEvents
* Changed methods in IrcClient, IrcConnection and IrcFeatures to be async Task-returning methods, as well as the Delegates handlers
** As it stands this change was somewhat hastily and lazily made and may not be 100% working yet
* Also changed FiniteThread to use Task-based workers
* Namespace name changed to StargazerG.Irc4NetButSmarter