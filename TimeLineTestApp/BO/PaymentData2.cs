using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TimeLineTestApp
{
	public partial class PaymentData
	{
		public static PaymentData Load(string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Open))
			{
				return (new XmlSerializer(typeof(PaymentData))).Deserialize(XmlReader.Create(fs)) as PaymentData;

			}
		}
	}
}
