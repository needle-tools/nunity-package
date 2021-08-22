using System.IO;
using System.Xml;

namespace NUnityPackage.Core
{
	public class NamespaceIgnorantXmlTextReader : XmlTextReader
	{
		public NamespaceIgnorantXmlTextReader(TextReader reader) : base(reader)
		{
		}

		public override string NamespaceURI => "";
	}
}