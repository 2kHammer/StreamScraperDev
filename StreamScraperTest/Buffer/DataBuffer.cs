using System.Xml.Serialization;

namespace StreamScraperTest.Buffer;

public class DataBuffer<T> where T: class
{
    //Save the data
    public static void Bufferdata(T data, string bufferpath)
    {
        using (var writer = new System.IO.StreamWriter(bufferpath))
        {
            var serializer = new XmlSerializer(data.GetType());
            serializer.Serialize(writer, data);
            writer.Flush();
        }
    }

    //Restore the data
    public static T? Restoredata(string bufferpath)
    {
        using (var stream = System.IO.File.OpenRead(bufferpath))
        {
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(stream) as T;
        } 
    }
}