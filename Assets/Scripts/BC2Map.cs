using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class BC2Map {
	
	public class Instances
	{
		[XmlAttribute("guid")]
		public string GUID { get; set; }

		[XmlAttribute("type")]
		public string Type { get; set; }
		
		[XmlElement]
		public List<BC2Array> Arrays { get; set; }
		
		[XmlElement("field")]
		public Field Field { get; set; }

		[XmlElement("complex")]
		public Field Complex { get; set; }
	}

	public class Field
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

	}

	public class Complex
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlText]
		public string Value { get; set; }
		
	}

	public enum Axis
	{
		[XmlEnum("x")]
		X,
		[XmlEnum("y")]
		Y,
		[XmlEnum("z")]
		Z
	}

	public class BC2Array
	{
		[XmlAttribute("name")]
		public BC2Array Name { get; set; }
		
		[XmlText]
		public double Value { get; set; }

	}
}
