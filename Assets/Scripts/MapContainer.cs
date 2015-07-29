using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("partition")]
public class MapContainer
{
	[XmlElement("instance")]
	public List<Inst> instance;
	
	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(MapContainer));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	public static MapContainer Load(string path)
	{
		var serializer = new XmlSerializer(typeof(MapContainer));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as MapContainer;
			
		}
	}
	
	//Loads the xml directly from the given string. Useful in combination with www.text.
	public static MapContainer LoadFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(MapContainer));
		return serializer.Deserialize(new StringReader(text)) as MapContainer;
	}
}