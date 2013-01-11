using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using ClrPlus.Powershell.Rest.Commands;

namespace RestableCmdletSample
{
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
