using System;
using System.Collections;
using System.Threading.Tasks;
using Integration;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubcscribeDescription : MonoBehaviour , IPointerClickHandler
{
    [SerializeField] 
    private AdMobController _adMobController;
    [SerializeField] 
    private GDPRLinksHolder _gdprLinksHolder;
    [SerializeField] 
    private TextMeshProUGUI _descriptionText;
    
    private string _privacyLink;
    private string _termsLink;
    private bool _externalOpeningUrlDelayFlag;
    
    private void Start()
    {
        _privacyLink = _adMobController.IsProdaction ? _gdprLinksHolder.PrivacyPolicy : _gdprLinksHolder.PrivacyPolicyTest;
        _termsLink = _adMobController.IsProdaction ? _gdprLinksHolder.TermsOfUse : _gdprLinksHolder.TermsOfUseTest;
        RefreshDescription();
    }

    private void RefreshDescription()
    {
        string descriptionText = "Auto-renewal subscription info:\n" +
                                 "Payment will be charged to your iTunes Account at confirmation of purchase.\n" +
                                 "Free trial subscription is automatically renewed unless cancelled 24 hours before the renewal.\n" +
                                 "Subscription automatically renews unless auto-renew is turned off at least 24-hours before the end of the current period.\n" +
                                 "Any unused portion of a free trial period, if offered, will be forfeited when the user purchases a subscription to that publication.\n" +
                                 "Subscriptions may be managed by the user and auto-renewal may be turned off by going to the user’s Account Settings after purchase.\n" +
                                 $"Privacy Policy: <link=\"PrivacyPolicy\"><b><u>{_privacyLink}</u></b></link>\n" +
                                 $"Terms of Service: <link=\"TermsOfService\"><b><u>{_termsLink}</u></b></link>";

        _descriptionText.text = descriptionText;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_descriptionText, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = _descriptionText.textInfo.linkInfo[linkIndex];
            switch (linkInfo.GetLinkID())
            {
                case "PrivacyPolicy":
                    OpenUrl(_privacyLink);
                    break;
                case "TermsOfService":
                    OpenUrl(_termsLink);
                    break;
            }
        }
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
        await Task.Delay(1);
        try
        {
            Application.OpenURL(url);
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
