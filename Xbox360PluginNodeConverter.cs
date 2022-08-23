using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ReClassNET.DataExchange.ReClass;
using ReClassNET.Logger;
using ReClassNET.Nodes;
using Xbox360Plugin.TextPointers;

namespace Xbox360Plugin.Pointer32
{
	public class Xbox360PluginNodeConverter : ICustomNodeSerializer
    {

        public bool CanHandleNode(BaseNode node) => node is Pointer32Node || node is Utf8TextPtr32Node || node is Utf16TextPtr32Node;

	    public bool CanHandleElement(XElement element)
        {
			return new[] {"Pointer32", "Text32Ptr32", "Text16Ptr32", "Text8Ptr32" }.Contains(element.Attribute(ReClassNetFile.XmlTypeAttribute)?.Value);
		}
		
		public BaseNode CreateNodeFromElement(XElement element, BaseNode parent, IEnumerable<ClassNode> classes, ILogger logger, CreateNodeFromElementHandler defaultHandler)
        {
            string value = element.Attribute(ReClassNetFile.XmlTypeAttribute)?.Value;
			if (value == "Pointer32") return new Pointer32Node();
            if (value == "Text16Ptr32") return new Utf16TextPtr32Node();
            if (value == "Text8Ptr32") return new Utf8TextPtr32Node();
            return null;
        }
		
		public XElement CreateElementFromNode(BaseNode node, ILogger logger, CreateElementFromNodeHandler defaultHandler)
        {
            string XmlType = "";

            if (node is Pointer32Node) XmlType = "Pointer32";
		    else if (node is Utf16TextPtr32Node) XmlType = "Text16Ptr32";
		    else if (node is Utf8TextPtr32Node) XmlType = "Text8Ptr32";
			
		    return new XElement(
				ReClassNetFile.XmlNodeElement,
				new XAttribute(ReClassNetFile.XmlTypeAttribute, XmlType)
			);
		}
	}
}
