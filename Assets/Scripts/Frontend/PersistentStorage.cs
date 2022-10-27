using System.IO;
using UnityEngine;

/// <summary>
/// Specify a save path and set up a binary interface to it.
/// </summary>
public class PersistentStorage : MonoBehaviour
{
    string savePath;
    void Awake () {
		savePath = Path.Combine(Application.persistentDataPath, "saveFile");
	}

	public void Save (IPersistableObject o, int version) {
		using (
			var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))
		) {
			writer.Write(version);
			o.Save(new GameDataWriter(writer));
		}
		Debug.Log($"Game saved to {savePath}");
	}

	public void Load (IPersistableObject o) {
		byte[] data = File.ReadAllBytes(savePath);
		var reader = new BinaryReader(new MemoryStream(data));
		o.Load(new GameDataReader(reader, reader.ReadInt32()));
	}  
}