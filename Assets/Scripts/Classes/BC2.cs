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
        [XmlAttribute(AttributeName = "name")]
        public string name;
        [XmlElement(ElementName="item")]
		public List<Item> item;
		[XmlElement(ElementName = "complex")]
		public List<Complex> complex;
		[XmlText]
		public string value;
	}

	[System.Serializable]
	[XmlRoot(ElementName="complex")]
	public class Complex {
        [XmlAttribute(AttributeName = "name")]
        public string name;
        [XmlElement(ElementName="field")]
		public List<Field> field;
		[XmlElement(ElementName = "complex")]
		public List<Complex> complex;
		[XmlElement(ElementName = "array")]
		public List<BC2Array> array;
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
        [XmlAttribute(AttributeName = "guid")]
        public string guid;
        [XmlAttribute(AttributeName = "primaryInstance")]
        public string primaryInstance;
        [XmlAttribute(AttributeName = "exportMode")]
        public string exportMode;
        [XmlElement(ElementName="instance")]
		public List<Inst> instance;

	}
}
