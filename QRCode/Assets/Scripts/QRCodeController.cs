using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class QRCodeController
{
    public string Decode(WebCamTexture camTexture)
    {
        var reader = new BarcodeReader();
        var color = camTexture.GetPixels32();
        var result = reader.Decode(color, camTexture.width, camTexture.height);
        return result?.Text;
    }

    public Texture2D Encode(string textForEncoding, Texture2D texture)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = texture.height,
                Width = texture.width
            }
        };
        var colors = writer.Write(textForEncoding);
        texture.SetPixels32(colors);
        texture.Apply();
        return texture;
    }
}
