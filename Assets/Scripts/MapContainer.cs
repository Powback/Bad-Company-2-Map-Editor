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


	

		XmlWriterSettings settings = new XmlWriterSettings();
		settings.NewLineChars = "\r\n";
		settings.IndentChars = '\t'.ToString();
		settings.Encoding = Encoding.UTF8;
		settings.Indent = true;
		settings.NewLineHandling = NewLineHandling.Entitize;


		var serializer = new XmlSerializer(typeof(Partition));
		using(StreamWriter stream = new StreamWriter(path, false)) {


			var xmlWriter = XmlWriter.Create (stream, settings);
				serializer.Serialize (xmlWriter, obj);
		}

		RemoveEncoding (path); // dirty hack to remove encoding, which fucks up the dbx converter.
	}

	static void RemoveEncoding(string path) {
		byte[] orgBuffer = File.ReadAllBytes (path);
		byte[] buffer = new byte[orgBuffer.Length - 17];
		int i = 0;
		int i2 = 0;
		while (i2 < orgBuffer.Length) {
			if (i2 == 19) {
				i2 = 36;
			}
			buffer [i] = orgBuffer [i2];

			i++;
			i2++;
		}
		File.WriteAllBytes (path, buffer);
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