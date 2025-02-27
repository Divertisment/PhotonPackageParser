using NUnit.Framework;
using Protocol16.Photon;

namespace Protocol16.Tests;

public class DeserializerTest
{
    [Test]
    public void DeserializeShort()
    {
        var buffer = new byte[]
        {
            127, 255
        };
        int offset = 0;

        NumberDeserializer.Deserialize(out short result, buffer, ref offset);

        Assert.That(result, Is.EqualTo(short.MaxValue));
        Assert.That(offset, Is.EqualTo(2));
    }

    [Test]
    public void DeserializeInteger()
    {
        var buffer = new byte[]
        {
            0, 0, 127, 255
        };
        int offset = 0;

        NumberDeserializer.Deserialize(out int result, buffer, ref offset);

        Assert.That(result, Is.EqualTo(short.MaxValue));
        Assert.That(offset, Is.EqualTo(4));
    }
}