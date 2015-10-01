using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
namespace BC2
{
	[System.Serializable]
	[XmlRoot(ElementName="field")]
	public class Field {
		
		[XmlAttribute(AttributeName="name")]
		public string name;
		[XmlText]
		public string value;
		[XmlAttribute(AttributeName="ref")]
		public string reference;
	}
	[System.Serializable]
	[XmlRoot(ElementName="item")]
	public class Item {
		[XmlAttribute(AttributeName="ref")]
		public string reference;
	}
	[System.Serializable]
	[XmlRoot(ElementName="array")]
	public class BC2Array {
		[XmlElement(ElementName="item")]
		public List<Item> item;
		[XmlAttribute(AttributeName="name")]
		public string name;
        //[System.NonSerialized] // Ugly hack to fix Serialization Depth issue 
        // private List<Complex> m_complex;
        [XmlElement(ElementName = "complex")]
        public List<Complex> complex;
        //public List<Complex> complex { get { return m_complex; } set { m_complex = value; } }
		[XmlText]
		public string value;
	}
	[System.Serializable]
	[XmlRoot(ElementName="complex")]
	public class Complex {
		[XmlElement(ElementName="field")]
		public List<Field> field;
		[XmlAttribute(AttributeName="name")]
		public string name;
        // [System.NonSerialized] // Ugly hack to fix Serialization Depth issue 
        // private List<Complex> m_complex;
        [XmlElement(ElementName = "complex")]
        public List<Complex> complex;
        //public List<Complex> complex { get { return m_complex; } set { m_complex = value; } }
        //[System.NonSerialized] // Ugly hack to fix Serialization Depth issue 
        //private List<BC2Array> m_array;
        [XmlElement(ElementName = "array")]
        public List<BC2Array> array;
        //public List<BC2Array> array { get { return m_array; } set { m_array = value; } }
		[XmlText]
		public string value;
	}
	[System.Serializable]
	[XmlRoot(ElementName="instance")]
	public class Inst {
		[XmlAttribute(AttributeName="type")]
		public string type;
		[XmlAttribute(AttributeName="guid")]
		public string guid;
		[XmlAttribute(AttributeName="id")]
		public string id;
		[XmlElement(ElementName="field")]
		public List<Field> field;
		[XmlElement(ElementName="array")]
		public List<BC2Array> array;
		[XmlElement(ElementName="complex")]
		public List<Complex> complex;
	}
	[System.Serializable]
	[XmlRoot(ElementName="partition")]
	public class Partition {
		[XmlElement(ElementName="instance")]
		public List<Inst> instance;
		[XmlAttribute(AttributeName="guid")]
		public string guid;
		[XmlAttribute(AttributeName="primaryInstance")]
		public string primaryInstance;
		[XmlAttribute(AttributeName="exportMode")]
		public string exportMode;
	}
}
