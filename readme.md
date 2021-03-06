﻿# Sample for RestableCmdlets

## Setup 
You need the ClrPlus Restable cmdlet support. You probably just want to use the
monolithic build (which has all of the dependencies rolled up inside it)

From nuget just install the ClrPlus.Powershell package (referenced in this project already)

> Note: Make sure that port 80 is opened up in your firewall (or whatever port you're listing on)

## Implementation

The only thing you have to do to create a restable cmdlet is to derive it from `RestableCmdlet<>` (correctly!) and add a few lines into the beginning of ProcessRecord:

#### HelloWorldCmdlet.cs 

``` csharp

namespace RestableCmdletSample
{
    using System.Management.Automation;
    using ClrPlus.Powershell.Rest.Commands;

    [Cmdlet(VerbsCommon.Get, "HelloWorld")]
    public class GetHelloWorldCmdlet : RestableCmdlet<GetHelloWorldCmdlet>
    {
        [Parameter(HelpMessage = "first value")]
        public string First { get; set; }

        [Parameter(HelpMessage = "Second value")]
        public string Second { get; set; }

        [Parameter(HelpMessage = "Third value")]
        public string Third { get; set; }


        protected override void ProcessRecord() {
            // must check if this is a remote invocation of the cmdlet
            if (Remote)
            {
                ProcessRecordViaRest();
                return;
            }

            // after this, it's just like normal cmdlet handling...
            WriteObject(string.Format("Hello World. Response: [{0}]+[{1}]+[{2}]", First , Second , Third));
        }
    }
}

```

Note how the cmdlet must inherit from `RestableCmdlet<>` : 

``` csharp
	public class GetHelloWorldCmdlet : RestableCmdlet<GetHelloWorldCmdlet>	
```

You have to pass the class name as the parameter to the generic -- this lets the base class correctly handle some things.

## Setting up the service 

**NOTE** : The listener runs inside the current powershell process. 
If you need to run it outside the current process, spawn another powershell! 

You could also setup a scheduled task if you wanted to do this without having anyone logged in.

### via the command line:

You can add a cmdlet to the service and start listening easily:

``` powershell
# import the modules we need
import-module .\ClrPlus.Powershell.dll
import-module .\RestableCmdletSample.dll

# add the cmdlet 
Add-RestCmdlet -Command get-helloworld

# start the listener 
start-restservice 

```

### or using a service.properties file
``` powershell
# import the modules we need
import-module .\ClrPlus.Powershell.dll
import-module .\RestableCmdletSample.dll

# start the listener 
# load the configuration from the file given:
start-restservice -config .\service.properties

```

### The service.properties file

``` css
// REST Service Properties File

rest-service  {
	// the file that contains the user accounts (if you are using authentication)
	user-accounts : "users.properties";

	// http addresses to listen to. seperate with commas.
    listen-on: {
		"http://*/" 
	};
};

rest-command[gethelloworld] {
	// by commenting this out, this command is exposed via rest without any security at all 
	// roles : users;

	cmdlet : "get-HelloWorld";

	// optional: You can set default parameters for cmdlet values:
	default-parameters : {
        second = " this is a happy day"
    };

	// optional: You can set FORCE parameters for cmdlet values:
	forced-parameters : {
        third = " CAN'T CHANGE"
    };
}

rest-command[addnumbers] {
	// you can define any role required
	// just make sure that you have a user that has that role!
	roles : users;

    cmdlet : "add-numbers" ;
}


rest-command[setservicepassword] {
	// this cmdlet lets people reset their password. 
	// this one is built-into the ClrPlus assembly.

	roles: users;
	cmdlet: "set-servicepassword";
}

```

Some other examples:

``` css

rest-command {
	// you can specify many roles
	roles: {editors, admins};

	cmdlet : "some-command";

	// optional: You can set default parameters for cmdlet values that take arrays too
	default-parameters : {
		// if the user passes values for defaults, they get merged with the ones 
		// the admin sets here.
        someparam = { "garrett", "serack" }
		someotherparam = { "serack", "garrett" }
    };

	// optional: You can set default parameters for cmdlet values that take arrays too
	forced-parameters : {
		// if the user passes values for forced parameters, they get dropped with the ones 
		// and ONLY the ones the admin sets here are used.
        someparam = { "garrett", "serack" }
		someotherparam = { "serack", "garrett" }
    };
}



```


### The users.properties file

We have a simple authentication system in place that pulls from a properties file.

``` css
// declare each user 
user[garrett] { 

	// give a password
    password : "password";

	// and whatever roles you want. 
    roles : {
        admins,users,editors
    };
}
```


## Working with example cmdlets
First, try the cmdlet in the current session:

``` powershell 

> get-helloworld  -first this -second that -third blue
Hello World. Response: [this]+[that]+[blue]

```


It will show you the response.

Now, Try the same cmdlet, but call it remotely:

``` powershell 

> get-helloworld -serviceurl http://localhost -first this -second that -third blue
Hello World. Response: [this]+[that]+[ CAN'T CHANGE]

```

You'll see the third parameter is forced to a particular value--this was set in the service.properties file.


#### Calling cmdlets that require authentication

To use authentication, you have use the `-credential` parameter:

``` powershell
# you could have it popup and ask you when you run it:
> add-numbers -serviceurl http://localhost -credential (get-credential) -numbers 1,2,3,4
Supply values for the following parameters:
Credential

# (it pops up a GUI prompt)

10

```

You could store the credential in a variable first:

``` powershell
> $c = get-credential 
Supply values for the following parameters:
Credential

# (it pops up a GUI prompt)

# now call the service:
> add-numbers -serviceurl http://localhost -credential $c -numbers 1,2,3,4
10

```


### Making it easy: remember serviceurl and credentials 

Of course, who wants to actually keep giving the `-serviceurl` and `-credential` parameters, right?

Store them (encryped to the machine, in the user's registry)

``` powershell
> Set-DefaultRemoteService -ServiceUrl http://localhost -credential (get-credential) -remember
# this  will prompt for the credential, and then store that and the serviceurl in the registry.
# if you don't use -remember, it will store it only in the current powershell process.

# call a remote cmdlet, this time just say -remote
> add-numbers -remote -numbers 1,2,3,4,5

15
```

## Calling cmdlets from something other than powershell:

Since the cmdlets are hosted via REST, they can be called using anything! A browser, command line tools like `curl` and `wget` or languages like node.js.

``` bash
# example using curl:

# will return the results in JSON format:
$ curl -u garrett:password --basic http://localhost/add-numbers?numbers=1,2,3
[6]

# if you don't like JSON, you can change the response format (hmm. something not working right... sigh):

$ curl -u garrett:password --basic http://localhost/add-numbers?numbers=1,2,3&format=xml
<ArrayOfanyType xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays" />

```

# REFERENCE

## Cmdlets for using Rest Services

### add-restcmdlet
### remove-restcmdlet

### start-restservice
### stop-restservice

### set-defaultremoteservice

### set-servicepassword 


