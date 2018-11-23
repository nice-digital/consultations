namespace Comments.Common
{
    public static class Constants
    {
        public const string ConsultationsBasePath = "/consultations";
	    public const string ConsultationsReplaceableRelativeUrl = ConsultationsBasePath + "/{0}/{1}/{2}";
	    public const string ConsultationsPreviewReplaceableRelativeUrl = ConsultationsBasePath + "/preview/{0}/consultation/{1}/document/{2}/chapter/{3}";
		public const string ErrorPath = "/error";
	    public const string StatusAPIKeyName = "ECStatusApiKey";

	    public static class AppSettings
	    {
		    public const string Keyword = "Keyword";
	    }
	}
}
