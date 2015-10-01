using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using BC2;

public class MapContainer
{
	
	public static void Save(object obj, string path)
	{
		var serializer = new XmlSerializer(typeof(Partition));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, obj);
		}
	}
	
	public static Partition Load(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(Partition));
        using (FileStream stream = new FileStream(path, FileMode.Open))
        { 
            return serializer.Deserialize(stream) as Partition;
			
		}
	}
	
	//Loads the xml directly from the given string. Useful in combination with www.text.
	public static Partition LoadFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(Partition));
		return serializer.Deserialize(new StringReader(text)) as Partition;
	}
}