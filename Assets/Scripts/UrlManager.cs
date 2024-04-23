using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UrlManager : MonoBehaviour
{
    [SerializeField]
    private string _urlForPrivacyPolicy;
    [SerializeField] 
    private string _urlForTermsOfUse;
    [SerializeField]
    private Button _privacyButton;
    [SerializeField] 
    private Button _termsButton;

    private bool _externalOpeningUrlDelayFlag = false;

    private void Awake()
    {
        if (_termsButton != null)
            _termsButton.onClick.AddListener(() => OpenUrl(_urlForTermsOfUse));

        if (_privacyButton != null)
            _privacyButton.onClick.AddListener(() => OpenUrl(_urlForPrivacyPolicy));
    }

    private void OnDestroy()
    {
        if (_termsButton != null)
            _termsButton.onClick.RemoveListener(() => OpenUrl(_urlForTermsOfUse));

        if (_privacyButton != null)
            _privacyButton.onClick.RemoveListener(() => OpenUrl(_urlForPrivacyPolicy));
    }

    private async void OpenUrl(string url)
    {
        if (_externalOpeningUrlDelayFlag) return;
        _externalOpeningUrlDelayFlag = true;
        await OpenURLAsync(url);
        StartCoroutine(WaitForSeconds(1, () => _externalOpeningUrlDelayFlag = false));
    }
    
    private async Task OpenURLAsync(string url)
    {
        await Task.Delay(1); // Ждем один кадр, чтобы избежать блокировки основного потока
        try
        {
            Application.OpenURL(url); // Открываем ссылку
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при открытии ссылки {url}: {e.Message}");
        }
    }

    private IEnumerator WaitForSeconds(float seconds, Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    } 
}
