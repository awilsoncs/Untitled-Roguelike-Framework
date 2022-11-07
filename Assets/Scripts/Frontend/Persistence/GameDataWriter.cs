using System.IO;
using UnityEngine;

public class GameDataWriter {
    BinaryWriter writer;

    public GameDataWriter (BinaryWriter writer) {
        this.writer = writer;
    }

    public void Write (float value) {
        writer.Write(value);
    }
    public void Write (bool value) {
        writer.Write(value);
    }

    public void Write (int value) {
        writer.Write(value);
    }

    public void Write (System.String value) {
        if (value == null) {
            value = "";
        }
        writer.Write(value);
    }

    public void Write (Vector3 value) {
        writer.Write(value.x);
        writer.Write(value.y);
        writer.Write(value.z);
    }

    public void Write (Color value) {
        writer.Write(value.r);
        writer.Write(value.g);
        writer.Write(value.b);
        writer.Write(value.a);
    }

    public void Write (Random.State value) {
        writer.Write(JsonUtility.ToJson(value));
    }
}