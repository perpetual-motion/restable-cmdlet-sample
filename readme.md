# Sample for RestableCmdlets

## Setup 
You need the ClrPlus Restable cmdlet support. You probably just want to use the
monolithic build (which has all of the dependencies rolled up inside it)

From nuget just install the ClrPlus.Powershell package (referenced in this project already)

> Note: Make sure that port 80 is opened up in your firewall (or whatever port you're listing on)

## Implementation


## Setting up the service 


### The service.properties file


### The users.properties file


## Working with example cmdlets
First, try the cmdlet in the current session:

``` powershell 

> get-helloworld  -first this -second that -third blue
Hello World. Response: [this]+[that]+[blue]

```

### Making it easy: remember serviceurl and credentials 

# REFERENCE

## Cmdlets for using Rest Services

### add-restcmdlet
### remove-restcmdlet

### start-restservice
### stop-restservice

### set-defaultremoteservice

### set-servicepassword 


Walkthru:

First, try the cmdlet in the current session:
 
     > get-helloworld  -first this -second that -third blue
	 Hello World. Response: [this]+[that]+[blue]

It will show you the response.

Now, Try the same cmdlet, but call it remotely:

     > get-helloworld -serviceurl http://localhost -first this -second that -third blue
	 Hello World. Response: [this]+[that]+[ CAN'T CHANGE]

You'll see the third parameter is forced to a particular value--this was set in the 


get-helloworld -serviceurl http://localhost 

"
