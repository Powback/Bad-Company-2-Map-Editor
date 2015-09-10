using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class Inst
{
	[XmlAttribute]
	public string guid;
	
	[XmlAttribute]
	public string type;
	
	[XmlElement]
	public List<BC2Array> array { get; set; }
	
	[XmlElement]
	public List<Field> field { get; set; }
	
	[XmlElement]
	public List<Complex> complex { get; set;}


}


public class Field
{

	[XmlAttribute]
	public string name;

	[XmlAttribute("ref")]
	public string reference;

	[XmlText]
	public string value;
	
}

public class Complex
{
	[XmlAttribute]
	public string name;

	[XmlText]
	public string value;

	[XmlElement]
	public List<Item> item { get; set; }

	[XmlElement]
	public List<Field> field { get; set; }

	[XmlElement]
	public List<Complex> complex { get; set; }	
}
//
//public enum Axis
//{
//	[XmlEnum("x")]
//	X,
//	[XmlEnum("y")]
//	Y,
//	[XmlEnum("z")]
//	Z
//}

public class BC2Array
{
	[XmlAttribute]
	public string name;
	
	[XmlText]
	public string value;

	[XmlElement]
	public List<Item> item { get; set; }

	[XmlElement]
	public List<Complex> complex { get; set; }
}


public class Item 
{
	[XmlAttribute("ref")]
	public string refference;

	[XmlText]
	public string value;

}