using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using BC2;
using System.Text;

public class MapContainer
{
	
	public static void Save(object obj, string path)
	{
		var serializer = new XmlSerializer(typeof(Partition));
	
		using(StreamWriter stream = new StreamWriter(path, false))
		{
			
			serializer.Serialize(stream, obj);
		}
	}
	
	public static Partition Load(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(Partition));
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            string spPattern = "sp";
			if( Regex.IsMatch(path.ToLower(), spPattern)) {
                using (StreamReader reader = new StreamReader(stream, true))
                {
                    string content = reader.ReadToEnd();
                    string pattern = "&";
                    string ret = Regex.Replace(content, pattern, "and");
                    return LoadFromText(ret) as Partition;
                }
            } else {
                return serializer.Deserialize(stream) as Partition;
            }
         
                
			
		}
	}
	
	//Loads the xml directly from the given string. Useful in combination with www.text.
	public static Partition LoadFromText(string text) 
	{
		var serializer = new XmlSerializer(typeof(Partition));
		return serializer.Deserialize(new StringReader(text)) as Partition;
	}
}