using System;
using System.IO;
using System.Web.UI;
using System.Xml.Schema;
using ServiceStack.Host;
using ServiceStack.Host.AspNet;
using ServiceStack.Host.Handlers;
using ServiceStack.Web;

namespace ServiceStack.Metadata
{
    public abstract class BaseSoapMetadataHandler : BaseMetadataHandler, IServiceStackHttpHandler
    {
		protected BaseSoapMetadataHandler()
		{
			OperationName = GetType().Name.Replace("Handler", "");
		}
		
		public string OperationName { get; set; }
    	
    	public override void Execute(System.Web.HttpContext context)
    	{
			ProcessRequest(
				new AspNetRequest(OperationName, context.Request),
				new AspNetResponse(context.Response), 
				OperationName);
    	}

		public new void ProcessRequest(IHttpRequest httpReq, IHttpResponse httpRes, string operationName)
    	{
            if (!AssertAccess(httpReq, httpRes, httpReq.QueryString["op"])) return;

			var operationTypes = HostContext.Metadata.GetAllTypes();

    		if (httpReq.QueryString["xsd"] != null)
    		{
				var xsdNo = Convert.ToInt32(httpReq.QueryString["xsd"]);
                var schemaSet = XsdUtils.GetXmlSchemaSet(operationTypes);
    			var schemas = schemaSet.Schemas();
    			var i = 0;
    			if (xsdNo >= schemas.Count)
    			{
    				throw new ArgumentOutOfRangeException("xsd");
    			}
    			httpRes.ContentType = "text/xml";
    			foreach (XmlSchema schema in schemaSet.Schemas())
    			{
    				if (xsdNo != i++) continue;
    				schema.Write(httpRes.OutputStream);
    				break;
    			}
    			return;
    		}

			using (var sw = new StreamWriter(httpRes.OutputStream))
			{
				var writer = new HtmlTextWriter(sw);
				httpRes.ContentType = "text/html";
				ProcessOperations(writer, httpReq, httpRes);
			}
    	}

    }
}