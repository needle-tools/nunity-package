using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace NUnityPackage.Core
{
	public class NamespaceIgnorantXmlTextReader : XmlTextReader
	{
		public NamespaceIgnorantXmlTextReader(System.IO.TextReader reader) : base(reader)
		{
		}

		public override string NamespaceURI
		{
			get { return ""; }
		}
	}

	public static class XmlUtils
	{
		public static TClass Deserialize<TClass>(string xml) where TClass : class, new()
		{
			var tClass = new TClass();

			xml = RemoveTypeTagFromXml(xml);

			var xmlSerializer = new XmlSerializer(typeof(TClass));
			using (TextReader textReader = new StringReader(xml))
			{
				tClass = (TClass)xmlSerializer.Deserialize(textReader);
			}

			return tClass;
		}

		public static string RemoveTypeTagFromXml(string xml)
		{
			if (!string.IsNullOrEmpty(xml) && xml.Contains("xsi:type"))
			{
				xml = Regex.Replace(xml, @"\s+xsi:type=""\w+""", "");
			}

			return xml;
		}
	}
}