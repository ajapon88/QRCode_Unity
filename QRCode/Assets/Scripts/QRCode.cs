using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QRCode : MonoBehaviour
{
    public GameObject decode;
    public GameObject encode;

    public RawImage qrCode;
    private Texture2D qrCodeTexture;

    public RawImage webCam;
    private WebCamTexture camTexture;

    public Text result;

    public float updateInterval = 1.0f;
    private float updateWaiting = 0.0f;

    private QRCodeController qrCodeController = new QRCodeController();
    private bool decodeMode = true;

    void OnEnable()
    {
        decode.SetActive(decodeMode);
        encode.SetActive(!decodeMode);
        if (decodeMode)
        {
            camTexture?.Play();
        }
    }

    void OnDisable()
    {
        camTexture?.Pause();
    }

    void OnDestroy()
    {
        camTexture?.Stop();
    }

    void Start()
    {
        qrCodeTexture = new Texture2D((int)qrCode.rectTransform.rect.width, (int)qrCode.rectTransform.rect.height);
        qrCode.texture = qrCodeTexture;

        var devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogWarning("カメラが見つかりません");
            return;
        }
        camTexture = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
        webCam.texture = camTexture;
#if UNITY_IOS && !UNITY_EDITOR
        var euler = webCam.transform.localRotation.eulerAngles;
        webCam.transform.localRotation = Quaternion.Euler(euler.x, euler.y - 180.0f, euler.z - 90.0f);
#elif UNITY_ANDROID && !UNITY_EDITOR
        var euler = webCam.transform.localRotation.eulerAngles;
        webCam.transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z - 90.0f);
#endif
        camTexture?.Play();
    }

    void Update()
    {
        if (decodeMode)
        {
            if (camTexture == null)
            {
                return;
            }
            var decodeStr = qrCodeController.Decode(camTexture);
            if (decodeStr != null)
            {
                result.text = decodeStr;
                result.rectTransform.sizeDelta = new Vector2(result.preferredWidth + 16, result.preferredHeight + 16);
            }
        }
        else
        {
            if (qrCodeTexture == null)
            {
                return;
            }
            updateWaiting -= Time.deltaTime;
            if (0.0f < updateWaiting)
            {
                return;
            }
            var textForEncoding = System.DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd(dddd) HH:mm:ss.ff");
            if (qrCodeTexture != null)
            {
                Debug.LogFormat("Encode: {0}", textForEncoding);
                qrCodeController.Encode(textForEncoding, qrCodeTexture);
            }
            updateWaiting = updateInterval;
        }
    }

    public void OnSwitchMode()
    {
        decodeMode = !decodeMode;
        decode.SetActive(decodeMode);
        encode.SetActive(!decodeMode);
        if (decodeMode)
        {
            if (camTexture != null && !camTexture.isPlaying)
            {
                camTexture?.Play();
            }
        }
        else
        {
            camTexture?.Pause();
        }
    }
}
