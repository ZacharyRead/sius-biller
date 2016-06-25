#Sius 1.0
by Zachary Read


##What is it?

Sius is configurable SubSpace/Continuum biller that
allows you to connect as many zones as you would like,
edit message limits, change the greet message and specify
as many chat channels as you want to allow. It safely
stores passwords and salts them, supports user banners,
logs events, and also has full cross-zone chat, messaging
and squad support. 

It is designed specifically to work with the game server
"A Small SubSpace Server (ASSS)".

##Getting started
This application is written in C# and was designed for
Windows, though it's possible to get it working on
Linux. You will need to open the project and build it
using a tool like Visual Studio. The project is also
reliant on SQLite: https://www.sqlite.org/.


##Launching the application

To use Sius, simply double click on the file named
"Sius.exe". To close the biller, simply press any key
while the console window is active.

Note that this biller uses the TCP Billing Protocol
(version 1.3.1), as proposed by Grelminar for the ASSS
server. It is therefore not compatible with Subgame.
For ASSS users, make sure that you load up the module
entitled "billing" in your modules.conf file.

The full protocol is detailed in the file
"new-biller-prot.html".


##Configuration

To have your zone successfully connect to the biller,
make sure that the respective billing information
matches. Open up the configuration file entitled
"Sius.exe.config" (any text editor will suffice),
and edit the port value and password value as needed.

<add key="port" value="500" />
<add key="password" value="password" />

For ASSS users, you will have to match those two values
in your global.conf file, along with the biller's IP
address (127.0.0.1 if running on the same computer)
and your zone's name under the billing section as so:

[Billing]
IP = 127.0.0.1
Port = 500
ServerName = My Zone
Password = password

You may edit other settings as needed.

ASSS users should also make sure that, in modules.conf,
you are running the "billing" module and not the
"billing_ssc" module.

##Supported in-game commands

###Messaging:
?message, ?messages, ?chat

###Squad:
?squad, ?squadcreate, ?squadjoin, ?squadowner,
?squadlist, ?squadkick, ?squadgrant, ?squadleave,
?squaddissolve, ?squadpassword

###Biller:
?buptime, ?btime, ?bversion, ?bzones (?zones),
?bzone

###Other:
?find, ?password, ?userid
