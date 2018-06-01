using System;
using System.Collections.Generic;
using System.Linq;

namespace ErrorHandling {
    public partial class MultipleErrors : System.Web.UI.Page {

        public IEnumerable<string> GetErrorMessages() {

           
            if (Context.AllErrors != null/* && Context.AllErrors.Length > 0*/)
                return Context.AllErrors.Select(e => e.Message);
            else
            {
                return Enumerable.Empty<string>();
            }
        }
        public IEnumerable<string> GetHeaders() {
            return Request.Headers.AllKeys.Select(x => x + ": " + Request.Headers[x]);
        }
    }
}
