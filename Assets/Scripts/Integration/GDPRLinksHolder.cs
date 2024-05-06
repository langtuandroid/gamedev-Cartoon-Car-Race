using UnityEngine;

namespace Integration
{
    [CreateAssetMenu(fileName = "GDPRLinks", menuName = "Subscription/GDPRLinks", order = 1)]
    public class GDPRLinksHolder : ScriptableObject
    {
#if UNITY_ANDROID
        [Header("Production Links Android")]
        [SerializeField] private string _privacy;
        [SerializeField] private string _terms;
        
#elif UNITY_IOS
        [Header("Production Links IOS")]
        [SerializeField] private string _privacy;
        [SerializeField] private string _terms;
#endif
        
        [Header("Test Link to Google")]
        [SerializeField] private string _privacyTest;
        [SerializeField] private string _termsTest;
        
        public string PrivacyPolicy => _privacy;
        public string TermsOfUse => _terms;
        
        public string PrivacyPolicyTest => _privacyTest;
        public string TermsOfUseTest => _termsTest;
    }
}
