# import any modules needed for the cmdlets.
# Should be at least the clrplus.powershell.dll and the dll with your cmdlets
import-module .\ClrPlus.Powershell.dll
import-module .\RestableCmdletSample.dll


# start the service using the -config parameter to load the serivce definitions from a file
start-restservice -config .\service.properties

# now those are running in this powershell process.

echo see the README.MD file for information 


