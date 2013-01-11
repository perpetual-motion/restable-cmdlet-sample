using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using ClrPlus.Powershell.Rest.Commands;
 
namespace RestableCmdletSample {
    [Cmdlet(VerbsCommon.Add, "Numbers")]
    public class AddNumbersCmdlet : RestableCmdlet<AddNumbersCmdlet> {
        [Parameter(HelpMessage = "a set of numbers")]
        public int[] Numbers { get; set; }

        protected override void ProcessRecord() {
            // must check if this is a remote invocation of the cmdlet
            if(Remote) {
                ProcessRecordViaRest();
                return;
            }
            // after this, it's just like normal cmdlet handling...

            var answer = Numbers.Sum();
            
            WriteObject(answer);
        }
    }
}
