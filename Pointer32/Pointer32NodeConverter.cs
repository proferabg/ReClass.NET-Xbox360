using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ReClassNET.DataExchange.ReClass;
using ReClassNET.Logger;
using ReClassNET.Nodes;

namespace Xbox360Plugin.Pointer32
{
	public class Pointer32NodeConverter : ICustomNodeSerializer
	{
		private const string XmlType = "Pointer32";
		
		public bool CanHandleNode(BaseNode node) => node is Pointer32Node;
		
		public bool CanHandleElement(XElement element) => element.Attribute(ReClassNetFile.XmlTypeAttribute)?.Value == XmlType;
		
		public BaseNode CreateNodeFromElement(XElement element, BaseNode parent, IEnumerable<ClassNode> classes, ILogger logger, CreateNodeFromElementHandler defaultHandler)
		{
			return new Pointer32Node();
		}
		
		public XElement CreateElementFromNode(BaseNode node, ILogger logger, CreateElementFromNodeHandler defaultHandler)
		{
			return new XElement(
				ReClassNetFile.XmlNodeElement,
				new XAttribute(ReClassNetFile.XmlTypeAttribute, XmlType)
			);
		}
	}
}
