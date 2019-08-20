using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Intelbras.CRM2013.Application.Workflow.Helper
{
    public class ActivityHelper : CodeActivity
    {
        //declare the params for input and output, and a default value if needed 
        [RequiredArgument]
        [Input("Bool Value")]
        public InArgument<string> Value { get; set; }

        [Output("Result")]
        public OutArgument<bool> Result { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            bool result = false;

            //get the input value 
            string input = Value.Get<string>(context);
            if (!string.IsNullOrWhiteSpace(input))
            {
                //check if it's a true value (1, true, or yes), anything else will be false 
                string[] trueValues = { "1", "true", "yes" };
                result = trueValues.Contains(input.ToLower());
            }

            //assign the output param back to the workflow 
            this.Result.Set(context, result);
        }
    }
}