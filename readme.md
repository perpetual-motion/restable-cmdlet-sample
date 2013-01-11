# Sample for RestableCmdlets

## Setup 
You need the ClrPlus Restable cmdlet support. You probably just want to use the
monolithic build (which has all of the dependencies rolled up inside it)

From nuget just install the ClrPlus.Powershell package (referenced in this project already)

> Note: Make sure that port 80 is opened up in your firewall (or whatever port you're listing on)

## Implementation

The only thing you have to do to create a restable cmdlet is to derive it from `RestableCmdlet<>` (correctly!) and add a few lines into the beginning of ProcessRecord:

> HelloWorldCmdlet.cs 
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

### via the command line:

You can add a cmdlet to the service and start listening easily:

``` powershell
# import the modules we need
import-module .\ClrPlus.Powershell.dll
import-module .\RestableCmdletSample.dll

# add the cmdlet 
Add-RestCmdlet -Command get-helloworld

# start the listener (listens from this process, if you exit, it will close the listener!)
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

# (listens from this process, if you exit, it will close the listener!)
```



### The service.properties file


### The users.properties file


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

### Making it easy: remember serviceurl and credentials 

# REFERENCE

## Cmdlets for using Rest Services

### add-restcmdlet
### remove-restcmdlet

### start-restservice
### stop-restservice

### set-defaultremoteservice

### set-servicepassword 


